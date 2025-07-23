// Copyright (c) Jon Thysell <http://jonthysell.com>
// Licensed under the MIT License.

using System;
using System.Text.RegularExpressions;

using HtmlAgilityPack;

namespace HawDict
{
    public static class StringUtils
    {
        public static string HtmlToUtf8(string s)
        {
            if (string.IsNullOrWhiteSpace(s))
            {
                throw new ArgumentNullException(nameof(s));
            }

            return StripHtmlTags(CleanHtmlEntities(s));
        }

        public static string StripHtmlTags(string s)
        {
            if (string.IsNullOrWhiteSpace(s))
            {
                throw new ArgumentNullException(nameof(s));
            }

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(s);

            return doc.DocumentNode.InnerText.Trim();
        }

        public static string CleanHtmlEntities(string s)
        {
            if (string.IsNullOrWhiteSpace(s))
            {
                throw new ArgumentNullException(nameof(s));
            }

            return HtmlEntity.DeEntitize(s).Trim();
        }


        public static string FixSentenceSpacing(string s)
        {
            s = AcronymSplitterRegex.Replace(s, @"$1 $3");
            s = ListSplitterRegex.Replace(s, @"$1$2 $3");
            s = SentenceSplitterRegex.Replace(s, @"$1$2 $3");
            return s;
        }

        public static string FixSentenceEnd(string s)
        {
            switch (s[^1])
            {
                case '!':
                case '?':
                case '.':
                case ')':
                case ']':
                case '"':
                case '…':
                    break;
                case ',':
                    s = s.TrimEnd(',') + ".";
                    break;
                default:
                    s += ".";
                    break;
            }
            return s;
        }

        private static readonly Regex AcronymSplitterRegex = new Regex(@"(([a-zA-Z]\.){2,})([a-zA-ZāēīōūĀĒĪŌŪʻ][^\.])", RegexOptions.Compiled);
        private static readonly Regex ListSplitterRegex = new Regex(@"([^\(][a-zāēīōū])([,;:])(ʻ?[a-zA-ZāēīōūĀĒĪŌŪʻ][^\)])", RegexOptions.Compiled);
        private static readonly Regex SentenceSplitterRegex = new Regex(@"([a-zāēīōū])([\.\!\?])(ʻ?[A-ZĀĒĪŌŪ])", RegexOptions.Compiled);

        public static string ReplaceOkina(string s, string replacement = "'")
        {
            if (string.IsNullOrWhiteSpace(s))
            {
                throw new ArgumentNullException(nameof(s));
            }

            return s.Replace(OkinaUtf8, replacement).Trim();
        }

        public const string OkinaUtf8 = "ʻ";
        public const string SyllableDotUtf8 = "·";
    }
}
