﻿// Copyright (c) Jon Thysell <http://jonthysell.com>
// Licensed under the MIT License.

using System;
using System.Globalization;
using System.Text;

using HtmlAgilityPack;

namespace HawDict
{
    public static class StringUtils
    {
        public static string StripNewLines(string s, string replace = "")
        {
            if (string.IsNullOrWhiteSpace(s))
            {
                throw new ArgumentNullException(nameof(s));
            }

            return s.Replace("\r\n", replace).Replace("\r", replace).Replace("\n", replace).Trim();
        }

        public static string StripTabs(string s, string replace = "")
        {
            if (string.IsNullOrWhiteSpace(s))
            {
                throw new ArgumentNullException(nameof(s));
            }

            return s.Replace("\t", replace).Trim();
        }

        public static string SingleLineNoTabs(string s)
        {
            if (string.IsNullOrWhiteSpace(s))
            {
                throw new ArgumentNullException(nameof(s));
            }

            return StripNewLines(StripTabs(s, " "), " ");
        }

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

        // Adapted from https://stackoverflow.com/a/39865783/1653267
        public static string NormalizeWhiteSpace(string s)
        {
            if (string.IsNullOrWhiteSpace(s))
            {
                throw new ArgumentNullException(nameof(s));
            }

            var len = s.Length;
            var src = s.ToCharArray();
            int dstIdx = 0;
            bool lastWasWS = false;
            for (int i = 0; i < len; i++)
            {
                var ch = src[i];
                if (char.IsWhiteSpace(ch))
                {
                    if (lastWasWS == false)
                    {
                        src[dstIdx++] = ' ';
                        lastWasWS = true;
                    }
                }
                else
                {
                    lastWasWS = false;
                    src[dstIdx++] = ch;
                }
            }
            return new string(src, 0, dstIdx);
        }

        public static string EscapeForXml(string s)
        {
            if (string.IsNullOrWhiteSpace(s))
            {
                throw new ArgumentNullException(nameof(s));
            }

            return s
                .Replace("&", "&amp;")
                .Replace("<", "&lt;")
                .Replace(">", "&gt;").Trim();
        }

        public static string WrapInTag(string s, string tag)
        {
            if (string.IsNullOrWhiteSpace(s))
            {
                throw new ArgumentNullException(nameof(s));
            }

            if (string.IsNullOrWhiteSpace(tag))
            {
                throw new ArgumentNullException(nameof(tag));
            }

            return string.Format("<{0}>{1}</{0}>", tag, s);
        }

        public static string WrapInTag(string s, string target, string tag)
        {
            if (string.IsNullOrWhiteSpace(s))
            {
                throw new ArgumentNullException(nameof(s));
            }

            if (string.IsNullOrWhiteSpace(target))
            {
                throw new ArgumentNullException(nameof(target));
            }

            if (string.IsNullOrWhiteSpace(tag))
            {
                throw new ArgumentNullException(nameof(tag));
            }

            return s.Replace(target, WrapInTag(target, tag)).Trim();
        }

        public static string ReplaceOkina(string s, string replacement = "'")
        {
            if (string.IsNullOrWhiteSpace(s))
            {
                throw new ArgumentNullException(nameof(s));
            }

            return s.Replace(OkinaUtf8, replacement).Trim();
        }

        // Adapted from http://archives.miloush.net/michkap/archive/2007/05/14/2629747.html
        public static string RemoveDiacritics(string s)
        {
            if (string.IsNullOrWhiteSpace(s))
            {
                throw new ArgumentNullException(nameof(s));
            }

            string sFormD = s.Normalize(NormalizationForm.FormD);
            StringBuilder sb = new StringBuilder();

            for (int ich = 0; ich < sFormD.Length; ich++)
            {
                UnicodeCategory uc = CharUnicodeInfo.GetUnicodeCategory(sFormD[ich]);
                if (uc != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(sFormD[ich]);
                }
            }

            return (sb.ToString().Normalize(NormalizationForm.FormC));
        }

        public const string OkinaUtf8 = "ʻ";
        public const string SyllableDotUtf8 = "·";
    }
}
