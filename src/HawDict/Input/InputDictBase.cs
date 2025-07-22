// Copyright (c) Jon Thysell <http://jonthysell.com>
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using QuickDict;

namespace HawDict
{
    public delegate void LogLine(string format, params object[] args);

    [Flags]
    public enum OutputFormats
    {
        None,
        CleanTxt = 0x1,
        StarDict = 0x2,
        Xdxf = 0x4,
        All = CleanTxt + StarDict + Xdxf,
    }

    public abstract class InputDictBase
    {
        public string ID { get; private set; }

        public TranslationType TranslationType { get; private set; }

        public string DictDir { get; private set; } = null;

        public string ShortTitle { get; protected set; } = null;

        public string LongTitle { get; protected set; } = null;

        public string Description { get; protected set; } = null;

        public List<string> Authors { get; private set; } = new List<string>();

        public string SrcUrl { get; protected set; } = null;

        private readonly LogLine _logLine;

        protected List<string[]> _rawData = new List<string[]>();

        public InputDictBase(string id, TranslationType translationType, LogLine logLine)
        {
            ID = !string.IsNullOrWhiteSpace(id) ? id.Trim() : throw new ArgumentNullException(nameof(id));
            TranslationType = translationType;
            _logLine = logLine ?? throw new ArgumentNullException(nameof(logLine));
        }

        public void Process(string rootDir, OutputFormats outputFormats = OutputFormats.All)
        {
            if (string.IsNullOrWhiteSpace(rootDir))
            {
                throw new ArgumentNullException(nameof(rootDir));
            }

            DictDir = GetDictDir(rootDir);

            Log("Getting raw data from source.");
            GetRawDataFromSource();
            Log("Got {0} entries.", _rawData.Count);

            if (outputFormats == OutputFormats.None)
            {
                Log("No formats selected to save.");
                return;
            }

            Log("Save start.");

            if (outputFormats.HasFlag(OutputFormats.CleanTxt))
            {
                string cleanFile = Path.Combine(DictDir, $"{ID}.{TranslationType}.clean.txt");
                SaveCleanFile(cleanFile);
            }

            if (outputFormats.HasFlag(OutputFormats.StarDict))
            {
                string starDictFile = Path.Combine(DictDir, $"{ID}.{TranslationType}.StarDict.ifo");
                SaveStarDictFile(starDictFile);
            }

            if (outputFormats.HasFlag(OutputFormats.Xdxf))
            {
                string xdxfFile = Path.Combine(DictDir, $"{ID}.{TranslationType}.dict.xdxf");
                SaveXdxfFile(xdxfFile);

                Log("Building XDXF dictionary.");
            }

            Log("Save end.");
        }

        private string GetDictDir(string rootDir)
        {
            string dictDir = Path.Combine(rootDir, ID);

            if (Directory.Exists(dictDir))
            {
                Log("Directory already exists.");
            }
            else
            {
                Log("Creating directory.");
                Directory.CreateDirectory(dictDir);
            }

            return dictDir;
        }

        protected void Log(string format, params object[] args)
        {
            _logLine("{0}\\{1}: {2}", ID, TranslationType.ToString(), string.Format(format, args));
        }

        private void SaveCleanFile(string cleanPath)
        {
            int count = 0;

            Log("Saving clean file.");
            using (StreamWriter sw = new StreamWriter(new FileStream(cleanPath, FileMode.Create), Encoding.UTF8))
            {
                foreach (KeyValuePair<string, string> kvp in GetCleanEntries())
                {
                    sw.WriteLine("{0}\t{1}", kvp.Key, kvp.Value);
                    count++;
                }
            }
            Log("Saved {0} entries.", count);
        }

        private DictionaryMetadata GetMetadata()
        {
            var metadata = new DictionaryMetadata();

            metadata.ShortTitle = ShortTitle;
            metadata.LongTitle = LongTitle;
            metadata.Description = Description;
            metadata.Authors.AddRange(Authors);
            metadata.SrcUrl = SrcUrl;
            metadata.ArticleKeyLangCode = TranslationType == TranslationType.EngToHaw ? "ENG" : "HAW";
            metadata.ArticleValueLangCode = TranslationType == TranslationType.EngToHaw ? "HAW" : "ENG";
            metadata.FileVersion = AppInfo.Version;

            return metadata;
        }

        private void SaveStarDictFile(string starDictPath)
        {
            var dict = new StarDictDictionary(GetMetadata());

            dict.GetStarDictSynonymsFromArticle = a =>
            {
                HashSet<string> synonyms = new HashSet<string>
                    {
                        a.Key
                    };

                foreach (string key in a.Key.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
                {
                    string s = key.Replace(StringUtils.SyllableDotUtf8, "").Replace(".", "").Replace("*", "").Replace("-", "");

                    synonyms.Add(s);
                    synonyms.Add(StringUtils.ReplaceOkina(s));
                    synonyms.Add(StringUtils.ReplaceOkina(s, ""));
                    synonyms.Add(StringUtils.RemoveDiacritics(s));
                    synonyms.Add(StringUtils.ReplaceOkina(StringUtils.RemoveDiacritics(s)));
                    synonyms.Add(StringUtils.ReplaceOkina(StringUtils.RemoveDiacritics(s), ""));
                }

                return synonyms;
            };

            dict.GetValueFromArticle = a =>
            {
                var valueSB = new StringBuilder();

                foreach (var definition in a.Value.GetDefinitions(true))
                {
                    var value = definition;
                    // Add abbreviations
                    foreach (var abbreviation in a.Parent.Abbreviations)
                    {
                        value = value.WrapInTag(abbreviation.Key, "i", StringWrapInTagOptions.WrapWholeWordsOnly);
                        if (abbreviation.Key.Length > 1 && char.IsLower(abbreviation.Key[0]))
                        {
                            value = value.WrapInTag(char.ToUpper(abbreviation.Key[0]) + abbreviation.Key.Substring(1), "i", StringWrapInTagOptions.WrapWholeWordsOnly);
                        }
                    }

                    value = value.WrapInTag("p");

                    // Add bold for numbering
                    value = Regex.Replace(value, "<p>([0-9]+)\\. ", "<p><b>$1</b>. ");
                    if (value.Contains("<b>2</b>. "))
                    {
                        // Fix bolding number one for pre-text
                        value = Regex.Replace(value, "<p>(.*[^>])1\\. ", "<p>$1<b>1</b>. ");
                    }

                    valueSB.Append(value);
                }

                return valueSB.ToString();
            };

            Log("Building StarDict dictionary.");

            foreach (var kvp in GetCleanEntries())
            {
                dict.AddArticle(kvp.Key, kvp.Value);
            }

            AddAbbreviations(dict);

            Log("Saving StarDict dictionary.");

            dict.Save(starDictPath);
        }

        private void SaveXdxfFile(string xdxfPath)
        {
            var dict = new XdxfDictionary(GetMetadata());

            dict.GetXdxfKeysFromAbbreviation = a =>
            {
                var list = new List<string>() { a.Key };
                if (a.Key.Length > 1 && char.IsLower(a.Key[0]))
                {
                    list.Add(char.ToUpper(a.Key[0]) + a.Key.Substring(1));
                }
                return list;
            };

            dict.GetXdxfKeysFromArticle = a =>
            {
                return a.Key.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList();
            };

            dict.GetXdxfKeyOptionalTerms = () =>
            {
                return new HashSet<string>() { ".", StringUtils.SyllableDotUtf8 };
            };

            dict.GetXdxfValuesFromArticle = a =>
            {
                return a.Value.GetDefinitions(false).ToList();
            };

            Log("Building XDXF dictionary.");

            foreach (var kvp in GetCleanEntries())
            {
                dict.AddArticle(kvp.Key, kvp.Value);
            }

            AddAbbreviations(dict);

            Log("Saving XDXF dictionary.");

            dict.Save(xdxfPath);
        }

        protected abstract void GetRawDataFromSource();

        protected abstract IEnumerable<KeyValuePair<string, string>> GetCleanEntries();

        protected abstract void AddAbbreviations(DictionaryBase dict);

    }

    public enum TranslationType
    {
        HawToEng = 0,
        EngToHaw,
    }
}
