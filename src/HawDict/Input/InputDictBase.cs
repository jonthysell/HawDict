// 
// InputDictBase.cs
//  
// Author:
//       Jon Thysell <thysell@gmail.com>
// 
// Copyright (c) 2018 Jon Thysell <http://jonthysell.com>
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace HawDict
{
    public delegate void LogLine(string format, params object[] args);

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

        public void Process(string rootDir)
        {
            if(string.IsNullOrWhiteSpace(rootDir))
            {
                throw new ArgumentNullException(nameof(rootDir));
            }

            DictDir = GetDictDir(rootDir);

            Log("Getting raw data from source.");
            GetRawDataFromSource();
            Log("Got {0} entries.", _rawData.Count);

            Log("Save start.");

            string cleanFile = Path.Combine(DictDir, string.Format("{0}.{1}.clean.txt", ID, TranslationType.ToString()));
            SaveCleanFile(cleanFile);

            SaveOutputDict<XdxfDictionary>();
            SaveOutputDict<StarDictDictionary>();

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

        protected abstract void GetRawDataFromSource();

        protected abstract IEnumerable<KeyValuePair<string, string>> GetCleanEntries();

        private void AddArticles(OutputDictBase dict)
        {
            dict.Articles.AddRange(GetCleanEntries().Select(kvp => new OutputArticle(dict, kvp.Key, kvp.Value)));
        }

        private void SaveOutputDict<T>() where T : OutputDictBase
        {
            OutputDictBase outputDict = GetOutputDict<T>();

            Log("Building {0} dictionary.", outputDict.FormatType);
            AddArticles(outputDict);

            Log("Saving {0} dictionary.", outputDict.FormatType);
            outputDict.Save(DictDir);
        }

        private OutputDictBase GetOutputDict<T>() where T : OutputDictBase
        {
            OutputDictBase dict = null;

            if (typeof(T) == typeof(XdxfDictionary))
            {
                dict = new XdxfDictionary(ID, TranslationType)
                {
                    Title = ShortTitle,
                    FullTitle = LongTitle,
                    Description = Description,
                    SrcUrl = SrcUrl,
                };
            }
            else if (typeof(T) == typeof(StarDictDictionary))
            {
                dict = new StarDictDictionary(ID, TranslationType)
                {
                    Title = LongTitle,
                    Description = Description,
                };
            }

            dict.Authors.AddRange(Authors);

            AddAbbreviations(dict);

            return dict;
        }

        protected abstract void AddAbbreviations(OutputDictBase dict);
    }

    public enum TranslationType
    {
        HawToEng = 0,
        EngToHaw,
    }
}
