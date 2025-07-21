// Copyright (c) Jon Thysell <http://jonthysell.com>
// Licensed under the MIT License.

using System;
using System.Text.RegularExpressions;

using HtmlAgilityPack;

using QuickDict;

namespace HawDict
{
    public class PlaceNamesInputDict : HtmlInputDict
    {
        public PlaceNamesInputDict(LogLine logLine) : base("PlaceNames", TranslationType.HawToEng, logLine)
        {
            ShortTitle = "Place Names of Hawaii";
            LongTitle = "Place Names of Hawaii: Revised and Expanded Edition";
            Description = "A compilation of Hawaiian words that have been created, collected, and approved by the Hawaiian Lexicon Committee from 1987 through 2000. Copyright (c) 2003 ʻAha Pūnana Leo and Hale Kuamoʻo, College of Hawaiian Language, University of Hawaiʻi at Hilo (ISBN 978-0824828035)";

            Authors.AddRange(new string[] { "Mary Kawena Pūkuʻi", "Samuel H. Elbert", "Esther T. Moʻokini" });

            SrcUrl = "https://web.archive.org/web/20210421194928/http://ulukau.org/elib/cgi-bin/library?e=d-0pepn-000Sec--11en-50-20-frameset-book--1-010escapewin&a=d&d=D0&toc=0";

            EntryHtmlTag = "p";
        }

        protected override string CleanSourceHtml(string s)
        {
            s = Regex.Replace(s, "<table style=\"word-break:break-word;margin-left:auto;margin-right:auto;width:700px;\"><tr><td>\n<br><br><br><br><br>\n<div align=\"right\">GLOSSARY</div>.*<h1>AaAaAa</h1>", "<table style=\"word-break:break-word;margin-left:auto;margin-right:auto;width:700px;\"><tr><td>\n<h1>AaAaAa</h1>", RegexOptions.Singleline);

            s = s.Replace("<span></p>Ke-au-kaha</span>", "<span>Ke-au-kaha</span>")
                .Replace("</td></tr></table><p>&nbsp;</p>\n<table style=\"word-break:break-word;margin-left:auto;margin-right:auto;width:700px;\"><tr><td>", " ")
                .Replace("<span></p>Ke-kaulike", "<span>Ke-kaulike")
                .Replace("-\n   ", "-")
                .Replace("-   ", "-")
                .Replace("-\n", "-")
                .Replace("K&#333;k&#299;-o- Wailau,</i>", "K&#333;k&#299;-o-Wailau,</i>")
                .Replace("surflng", "surfing")
                .Replace("<span>Ka-ua-kinikin</span>i.", "<span>Ka-ua-kinikini</span>.")
                .Replace("(Gabriel Bellinghausen,1848-1933)", "(Gabriel Bellinghausen, 1848-1933)")
                .Replace("Honolu.u.", "Honolulu.")
                .Replace("lawanui</span>\n", "lawanui</span>")
                .Replace("mythical.bird", "mythical bird")
                .Replace("(1823)missionary", "(1823) missionary")
                .Replace("<?>", ";")
                .Replace("lodged on a ledge known as &#699;lole-ka&#699;a.)</p>", "lodged on a ledge known as &#699;Iole-ka&#699;a.)</p>")
                .Replace("<span>*&#699;Kuinihu</span>. Cone", "<span>*Kuinihu</span>. Cone")
                .Replace("in 1871 and renamed &#699;lo-lani.", "in 1871 and renamed &#699;Io-lani.")
                .Replace("&#699;s", "'s").Replace("n&#699;t", "n't")
                .Replace("sandvicensis</i>(a fish).", "sandvicensis</i> (a fish).")
                .Replace("Ka-welo(Ka-welo", "Ka-welo (Ka-welo")
                .Replace("Peak(2", "Peak (2")
                .Replace("daintily(a poetic", "daintily (a poetic")
                .Replace("Smith( 1802-1891)", "Smith (1802-1891)")
                .Replace("McAll&#237;ster", "McAllister")
                .Replace("See Waha-&#699;ula&#699;", "See Waha-&#699;ula")
                ;

            return s;
        }

        protected override bool IsEntryNode(HtmlNode node)
        {
            return string.Equals(node.FirstChild?.Name, "span", StringComparison.InvariantCultureIgnoreCase);
        }

        protected override string[] ParseEntryNode(HtmlNode node)
        {
            string entryName = node.FirstChild.OuterHtml;
            string entryValue = node.InnerHtml.Remove(0, entryName.Length);

            try
            {
                return new string[] { StringUtils.NormalizeWhiteSpace(StringUtils.SingleLineNoTabs(entryName)), StringUtils.NormalizeWhiteSpace(StringUtils.SingleLineNoTabs(entryValue)) };
            }
            catch (Exception)
            {
                Log("Unable to parse Name: \"{0}\" Value: \"{1}\"", entryName, entryValue);
                return null;
            }
        }

        protected override string FinalCleanKey(string key)
        {
            return key.TrimEnd('.').Replace("- ", "-");
        }

        protected override string FinalCleanValue(string value)
        {
            value = value.TrimStart('.').TrimStart();
            value = StringUtils.FixSentenceEnd(value);
            return StringUtils.FixSentenceSpacing(value);
        }

        protected override void AddAbbreviations(DictionaryBase dict)
        {
            dict.AddAbbreviation("For. Sel.", "Elbert, Selections from Fornander");
            dict.AddAbbreviation("For.", "Fornander, Hawaiian Antiquities (e.g., For. 5:176 means Fornander, Volume 5, p. 176)");
            dict.AddAbbreviation("HM", "Beckwith, Hawaiian Mythology");
            dict.AddAbbreviation("Indices", "Indices of Awards...");
            dict.AddAbbreviation("Kuy. 1", "Kuykendall, The Hawaiian Kingdom, Volume 1");
            dict.AddAbbreviation("Kuy. 2", "Kuykendall, The Hawaiian Kingdom, Volume 2");
            dict.AddAbbreviation("Kuy. 3", "Kuykendall, The Hawaiian Kingdom, Volume 3");
            dict.AddAbbreviation("lit.", "literally");
            dict.AddAbbreviation("PE", "Pukui and Elbert, Hawaiian Dictionary");
            dict.AddAbbreviation("PH", "Emerson, Pele and Hiiaka");
            dict.AddAbbreviation("qd.", "quadrangle");
            dict.AddAbbreviation("qds.", "quadrangles (maps 2-4)");
            dict.AddAbbreviation("RC", "Ruling Chiefs");
            dict.AddAbbreviation("TM", "Taylor and Miranda, \"Honolulu Street Names\"");
            dict.AddAbbreviation("UL", "Emerson, Unwritten Literature...");
            dict.AddAbbreviation("*", "Pronunciation and meaning uncertain");
        }
    }
}
