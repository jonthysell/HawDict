// Copyright (c) Jon Thysell <http://jonthysell.com>
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace HawDict
{
    public class OutputArticle
    {
        public OutputDictBase OutputDict { get; private set; }

        public string Key { get; private set; } = null;
        public string Value { get; private set; } = null;

        public string XdxfKey
        {
            get
            {
                string xdxfKey = "";
                foreach (string key in Key.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    string rawKey = StringUtils.EscapeForXml(key);
                    xdxfKey += GetXdxfKey(rawKey);
                }

                return xdxfKey;
            }
        }

        public string XdxfValue
        {
            get
            {
                return GetXdxfValue();
            }
        }

        public string StarDictKey
        {
            get
            {
                return Key;
            }
        }

        public IEnumerable<string> StarDictKeySynonyms
        {
            get
            {
                if (null == _starDictKeySynonyms)
                {
                    _starDictKeySynonyms = new HashSet<string>
                    {
                        StarDictKey
                    };

                    foreach (string key in StarDictKey.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        foreach (string synonym in GetSynonyms(key.Trim()))
                        {
                            _starDictKeySynonyms.Add(synonym);
                        }
                    }

                    _starDictKeySynonyms.Remove(StarDictKey);
                }
                return _starDictKeySynonyms;
            }
        }
        private HashSet<string> _starDictKeySynonyms;

        public string StarDictValue
        {
            get
            {
                string value = GetXdxfValue(true);

                value = value
                    .Replace("<abbr>", "<i>").Replace("</abbr>", "</i>");

                value = value
                    .Replace("<def>", "").Replace("</def>", "")
                    .Replace("<deftext>", "<p>").Replace("</deftext>", "</p>");

                value = Regex.Replace(value, "<p>([0-9]+)\\. ", "<p><b>$1</b>. ");

                if (value.Contains("<b>2</b>. "))
                {
                    // Fix bolding number one for pre-text
                    value = Regex.Replace(value, "<p>(.*[^>])1\\. ", "<p>$1<b>1</b>. ");
                }

                return value;
            }
        }

        public OutputArticle(OutputDictBase dict, string key, string value)
        {
            OutputDict = dict ?? throw new ArgumentNullException(nameof(dict));

            Key = !string.IsNullOrWhiteSpace(key) ? key.Trim() : throw new ArgumentNullException(nameof(key));
            Value = !string.IsNullOrWhiteSpace(value) ? value.Trim() : throw new ArgumentNullException(nameof(value));
        }

        private static string GetXdxfKey(string key)
        {
            key = StringUtils.WrapInTag(key, StringUtils.SyllableDotUtf8, "opt");
            key = StringUtils.WrapInTag(key, ".", "opt");

            key = StringUtils.WrapInTag(key, "k");

            return key;
        }

        private string GetXdxfValue(bool keepDefinitionNumbers = false)
        {
            string value = StringUtils.EscapeForXml(Value);

            // Add abbreviation tags
            foreach (OutputAbbreviation abbreviation in OutputDict.Abbreviations)
            {
                value = AddXdxfAbbreviationTags(value, abbreviation.Key);

                if (char.IsLower(abbreviation.Key[0]) && abbreviation.Key.Length > 1)
                {
                    value = AddXdxfAbbreviationTags(value, char.ToUpper(abbreviation.Key[0]) + abbreviation.Key.Substring(1));
                }
            }

            IEnumerable<string> definitions = GetDefinitions(value, keepDefinitionNumbers);

            if (definitions.Count() > 1)
            {
                value = string.Join("</deftext></def><def><deftext>", definitions);
                value = $"<def><def><deftext>{value}</deftext></def></def>";
            }
            else
            {
                value = $"<def><deftext>{value}</deftext></def>";
            }

            return value;
        }

        private static IEnumerable<string> GetDefinitions(string value, bool keepDefinitionNumbers, int num = 1)
        {
            string numStr = $"{num}. ";
            string nextNumStr = $" {num + 1}. ";

            int foundIndex = value.IndexOf(numStr);
            int nextFoundIndex = value.IndexOf(nextNumStr, foundIndex + 1);

            if (num == 1 && foundIndex > 0 && nextFoundIndex > 0)
            {
                // Numbered definition with some pre-text
                if (keepDefinitionNumbers)
                {
                    yield return value.Substring(0, nextFoundIndex);
                }
                else
                {
                    yield return value[0..foundIndex] + value[(foundIndex + numStr.Length)..nextFoundIndex];
                }
            }
            else if (foundIndex == 0 && nextFoundIndex > 0)
            {
                // Numbered definition without pre-text
                if (keepDefinitionNumbers)
                {
                    yield return value[0..nextFoundIndex];
                }
                else
                {
                    yield return value[numStr.Length..nextFoundIndex];
                }
            }
            else if (foundIndex == 0)
            {
                // Last numbered definition
                if (keepDefinitionNumbers)
                {
                    yield return value;
                }
                else
                {
                    yield return value.Substring(numStr.Length);
                }
            }
            else
            {
                // No numbers, just one definition
                yield return value;
            }

            if (nextFoundIndex > 0)
            {
                foreach (string def in GetDefinitions(value.Substring(nextFoundIndex + 1), keepDefinitionNumbers, num + 1))
                {
                    yield return def;
                }
            }
        }

        private static string AddXdxfAbbreviationTags(string value, string abbreviation)
        {
            value = value.Replace($" {abbreviation} ", $" <abbr>{abbreviation}</abbr> ");
            value = value.Replace($"({abbreviation} ", $"(<abbr>{abbreviation}</abbr> ");
            value = value.Replace($" {abbreviation})", $" <abbr>{abbreviation}</abbr>)");
            value = value.Replace($"({abbreviation})", $"(<abbr>{abbreviation}</abbr>)");

            value = value.Replace($"{abbreviation}.", $"<abbr>{abbreviation}</abbr>.");

            value = value.Replace($"{abbreviation};", $"<abbr>{abbreviation}</abbr>;");

            value = value.Replace($"{abbreviation},", $"<abbr>{abbreviation}</abbr>,");
            value = value.Replace($"({abbreviation},", $"(<abbr>{abbreviation}</abbr>,");

            value = value.Replace($"{abbreviation}/", $"<abbr>{abbreviation}</abbr>/");
            value = value.Replace($"/{abbreviation}", $"/<abbr>{abbreviation}</abbr>");

            value = value.Replace($"—{abbreviation}", $"—<abbr>{abbreviation}</abbr> ");

            if (value.StartsWith(abbreviation + " "))
            {
                value = $"<abbr>{abbreviation}</abbr>{value.Substring(abbreviation.Length)}";
            }

            if (value.EndsWith(" " + abbreviation))
            {
                value = $"{value.Substring(0, value.Length - abbreviation.Length)}<abbr>{abbreviation}</abbr>";
            }

            return value;
        }

        private static HashSet<string> GetSynonyms(string key)
        {
            HashSet<string> synonyms = new HashSet<string>
            {
                key
            };

            string s = key.Replace(StringUtils.SyllableDotUtf8, "").Replace(".", "").Replace("*", "").Replace("-", "");

            synonyms.Add(s);
            synonyms.Add(StringUtils.ReplaceOkina(s));
            synonyms.Add(StringUtils.ReplaceOkina(s, ""));
            synonyms.Add(StringUtils.RemoveDiacritics(s));
            synonyms.Add(StringUtils.ReplaceOkina(StringUtils.RemoveDiacritics(s)));
            synonyms.Add(StringUtils.ReplaceOkina(StringUtils.RemoveDiacritics(s), ""));

            return synonyms;
        }
    }
}
