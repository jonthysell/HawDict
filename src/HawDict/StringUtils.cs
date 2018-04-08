// 
// StringUtils.cs
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
