﻿// Copyright (c) Jon Thysell <http://jonthysell.com>
// Licensed under the MIT License.

using System;
using System.Text.RegularExpressions;

using HtmlAgilityPack;

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

            SrcUrl = "http://www.ulukau.org/elib/cgi-bin/library?e=d-0pepn-000Sec--11en-50-20-frameset-book--1-010escapewin&a=d&d=D0.3&toc=0";

            EntryHtmlTag = "p";
        }

        protected override string CleanSourceHtml(string s)
        {
            s = Regex.Replace(s, "<table style=\"word-break:break-word;margin-left:auto;margin-right:auto;width:700px;\"><tr><td>\n<br><br><br><br><br>\n<div align=\"right\">GLOSSARY</div>.*<h1>AaAaAa</h1>", "<table style=\"word-break:break-word;margin-left:auto;margin-right:auto;width:700px;\"><tr><td>\n<h1>AaAaAa</h1>", RegexOptions.Singleline);


            s = s.Replace("<span></p>Ke-au-kaha</span>", "<span>Ke-au-kaha</span>")
                .Replace("before the time of  </td></tr></table><p>&nbsp;</p>\n<table style=\"word-break:break-word;margin-left:auto;margin-right:auto;width:700px;\"><tr><td>Kiha-a-Pi&#699;ilani, and has been in continual use since then. <i>Lit.,</i> big Ke-awa.</p>", "before the time of Kiha-a-Pi&#699;ilani, and has been in continual use since then. <i>Lit.,</i> big Ke-awa.</p>")
                .Replace("<span></p>Ke-kaulike", "<span>Ke-kaulike")
                .Replace("<?>", ";");

            return s;
        }

        protected override bool IsEntryNode(HtmlNode node)
        {
            return string.Equals(node.FirstChild?.Name, "span", StringComparison.InvariantCultureIgnoreCase);
        }

        protected override string[] ParseEntryNode(HtmlNode node)
        {
            string entryName = node.FirstChild.OuterHtml;
            string entryValue = node.InnerHtml.Remove(0, entryName.Length + 1);

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

        protected override void AddAbbreviations(OutputDictBase dict)
        {
            dict.Abbreviations.AddRange(new OutputAbbreviation[]
            {
                new OutputAbbreviation(dict, "For. Sel.", "Elbert, Selections from Fornander"),
                new OutputAbbreviation(dict, "For.", "Fornander, Hawaiian Antiquities (e.g., For. 5:176 means Fornander, Volume 5, p. 176)"),
                new OutputAbbreviation(dict, "HM", "Beckwith, Hawaiian Mythology"),
                new OutputAbbreviation(dict, "Indices", "Indices of Awards..."),
                new OutputAbbreviation(dict, "Kuy. 1", "Kuykendall, The Hawaiian Kingdom, Volume 1"),
                new OutputAbbreviation(dict, "Kuy. 2", "Kuykendall, The Hawaiian Kingdom, Volume 2"),
                new OutputAbbreviation(dict, "Kuy. 3", "Kuykendall, The Hawaiian Kingdom, Volume 3"),
                new OutputAbbreviation(dict, "lit.", "literally"),
                new OutputAbbreviation(dict, "PE", "Pukui and Elbert, Hawaiian Dictionary"),
                new OutputAbbreviation(dict, "PH", "Emerson, Pele and Hiiaka"),
                new OutputAbbreviation(dict, "qd.", "quadrangle"),
                new OutputAbbreviation(dict, "qds.", "quadrangles (maps 2-4)"),
                new OutputAbbreviation(dict, "RC", "Ruling Chiefs"),
                new OutputAbbreviation(dict, "TM", "Taylor and Miranda, \"Honolulu Street Names\""),
                new OutputAbbreviation(dict, "UL", "Emerson, Unwritten Literature..."),
                new OutputAbbreviation(dict, "*", "Pronunciation and meaning uncertain"),
        });
        }
    }
}