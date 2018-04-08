// 
// MamakaKaiaoInputDict.cs
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
using System.Text.RegularExpressions;

using HtmlAgilityPack;

namespace HawDict
{
    public class MamakaKaiaoInputDict : HtmlInputDict
    {
        public MamakaKaiaoInputDict(TranslationType translationType, LogLine logLine) : base("MamakaKaiao", translationType, logLine)
        {
            ShortTitle = string.Format("Māmaka Kaiao ({0})", TranslationType == TranslationType.HawToEng ? "HAW-ENG" : "ENG-HAW");
            LongTitle = string.Format("Māmaka Kaiao: A Modern Hawaiian Vocabulary ({0})", TranslationType == TranslationType.HawToEng ? "Hawaiian-English" : "English-Hawaiian");
            Description = "A compilation of Hawaiian words that have been created, collected, and approved by the Hawaiian Lexicon Committee from 1987 through 2000. Copyright (c) 2003 ʻAha Pūnana Leo and Hale Kuamoʻo, College of Hawaiian Language, University of Hawaiʻi at Hilo (ISBN 978-0824828035)";

            Authors.AddRange(new string[] { "Kōmike Huaʻōlelo", "Hale Kuamoʻo", "ʻAha Pūnana Leo" });

            switch (TranslationType)
            {
                case TranslationType.HawToEng:
                    SrcUrl = "http://www.ulukau.org/elib/cgi-bin/library?e=d-0mkd-000Sec--11en-50-20-frameset-book--1-010escapewin&a=d&d=D0.3&toc=0";
                    break;
                case TranslationType.EngToHaw:
                    SrcUrl = "http://www.ulukau.org/elib/cgi-bin/library?e=d-0mkd-000Sec--11en-50-20-frameset-book--1-010escapewin&a=d&d=D0.4&toc=0";
                    break;
            }

            EntryHtmlTag = "p";
        }

        protected override string CleanSourceHtml(string s)
        {
            s = Regex.Replace(s, "<h1 align=\\\"center\\\">I<\\/h1>.*<h1 align=\\\"center\\\">I<\\/h1>", @"<h1 align=""center"">I</h1>", RegexOptions.Singleline); // Remove doubled I-section in EngToHaw

            s = s
                .Replace("spanclass", "span class")
                .Replace("</td></tr></table><p>&nbsp;</p>\n<table style=\"margin-left:auto;margin-right:auto;width:700px;\"><tr><td>", "")
                .Replace("</p>\n\n\n \n\n\n <p align=\"Justify\">\n", " ")
                .Replace("</p>\n\n\n \n\n\n <p align=\"justify\"><i>", " <i>")
                .Replace("</i></span>", "</span>")
                .Replace("<SPAN CLASS=\"HEAD\">", "<span class=\"head\">")
                .Replace("</SPAN>", "</span>")
                .Replace("\n</p>\n <p align=\"justify\"><span class=\"head\"></i>", "</i></p>\n <p align=\"justify\"><span class=\"head\">")
                .Replace("\n</p>\n</i> <p align=\"justify\"><span class=\"head\">", "\n</i></p>\n <p align=\"justify\"><span class=\"head\">")
                .Replace("decade Kekeke.</span></p>", "decade</span> Kekeke.</p>")
                .Replace("</span ", "</span>")
                .Replace("<span class=\"head>", "<span class=\"head\">")
                .Replace("<span class=\"head\'>", "<span class=\"head\">")
                .Replace("&#43:", "&#43;").Replace("&#43 ", "&#43;").Replace("&#34 ", "&#43; ").Replace("&$ ", " &#43; ")
                .Replace("&#183m", "&#183;m").Replace("&tilde,", "&tilde;,").Replace("&#233r", "&#233;r").Replace("&#0116l", "&#x016B;l").Replace("&#014D;", "&#x014D;")
                .Replace("</span> renal vein", "renal vein</span>")
                .Replace("<p> l&#257;", "\n</p>\n <p align=\"justify\"><span class=\"head\">l&#257;</span>")
                .Replace("</p>\n\n\n \n\n\n <p align=\"justify\">laws.", " laws.")
                .Replace("<sub>0</sub>", "&#x2080;").Replace("<SUB>1</SUB>", "&#x2081;").Replace("<SUB>2</SUB>", "&#x2082;").Replace("<sub>1</sub>", "&#x2081;").Replace("<sub>2</sub>", "&#x2082;")
                .Replace("&ldquo;", "\"").Replace("&rdquo;", "\"")
                .Replace("</p>\n <p align=\"justify\"><span class=\"head\">&#256;.</span>", " &#256;.")
                .Replace("....", "&hellip;.").Replace("..", ".").Replace(".</i>.", ".</i>");

            string[] split = s.Split('\n');

            for (int i = 0; i < split.Length; i++)
            {
                if (split[i].StartsWith(" <p align=\"justify\">", StringComparison.InvariantCultureIgnoreCase))
                {
                    bool missingEnd = !split[i].EndsWith("</p>", StringComparison.InvariantCultureIgnoreCase);
                    int nextEntry = -1;

                    if (missingEnd)
                    {
                        int j;
                        for (j = i + 1; j < split.Length; j++)
                        {
                            if (split[j].StartsWith(" <p align=\"justify\">", StringComparison.InvariantCultureIgnoreCase))
                            {
                                nextEntry = j;
                                break;
                            }
                            else if (split[j].EndsWith("</p>"))
                            {
                                missingEnd = false;
                                break;
                            }
                        }
                    }

                    if (missingEnd)
                    {
                        if (nextEntry > i)
                        {
                            split[nextEntry - 1] += "</p>";
                        }
                        else
                        {
                            split[i] += "</p>";
                        }
                    }
                }
            }

            return string.Join("\n", split);
        }

        protected override bool IsEntryNode(HtmlNode node)
        {
            return string.Equals(node.Attributes["align"]?.Value, "justify", StringComparison.InvariantCultureIgnoreCase)
                && string.Equals(node.FirstChild?.Name, "span", StringComparison.InvariantCultureIgnoreCase);
        }

        protected override string[] ParseEntryNode(HtmlNode node)
        {
            string entryName = node.FirstChild.OuterHtml;
            string entryValue = node.InnerHtml.Remove(0, entryName.Length);

            return new string[] { StringUtils.NormalizeWhiteSpace(StringUtils.SingleLineNoTabs(entryName)), StringUtils.NormalizeWhiteSpace(StringUtils.SingleLineNoTabs(entryValue)) };
        }

        protected override void AddAbbreviations(OutputDictBase dict)
        {
            dict.Abbreviations.AddRange(new OutputAbbreviation[]
            {
                new OutputAbbreviation(dict, "abb.", "abbreviation"),
                new OutputAbbreviation(dict, "Bib.", "Bible"),
                new OutputAbbreviation(dict, "cf.", "compare", AbbreviationType.Auxiliary),
                new OutputAbbreviation(dict, "comb.", "combined form"),
                new OutputAbbreviation(dict, "dic.", "dictionary definition"),
                new OutputAbbreviation(dict, "e.g.", "for example", AbbreviationType.Auxiliary),
                new OutputAbbreviation(dict, "Eng.", "English"),
                new OutputAbbreviation(dict, "ext. mng.", "extended meaning"),
                new OutputAbbreviation(dict, "i.e.", "that is", AbbreviationType.Auxiliary),
                new OutputAbbreviation(dict, "inv.", "invention"),
                new OutputAbbreviation(dict, "Japn.", "Japanese"),
                new OutputAbbreviation(dict, "lit.", "literally"),
                new OutputAbbreviation(dict, "mān.", "mānaleo (native speaker)"),
                new OutputAbbreviation(dict, "new mng.", "new meaning"),
                new OutputAbbreviation(dict, "PPN", "Proto Polynesian"),
                new OutputAbbreviation(dict, "redup.", "reduplication"),
                new OutputAbbreviation(dict, "sh.", "shortened form"),
                new OutputAbbreviation(dict, "sp. var.", "spelling variation"),
                new OutputAbbreviation(dict, "Tah.", "Tahitian"),
                new OutputAbbreviation(dict, "trad.", "traditional literary sources"),
                new OutputAbbreviation(dict, "var.", "variation"),
                new OutputAbbreviation(dict, "ham", "hamani (transitive verb)", AbbreviationType.Grammatical),
                new OutputAbbreviation(dict, "heh", "hehele (intransitive verb)", AbbreviationType.Grammatical),
                new OutputAbbreviation(dict, "ʻaʻ", "ʻaʻano (stative verb)", AbbreviationType.Grammatical),
                new OutputAbbreviation(dict, "kik", "kikino (common noun)", AbbreviationType.Grammatical),
                new OutputAbbreviation(dict, "iʻoa", "iʻoa (proper noun)", AbbreviationType.Grammatical),
                new OutputAbbreviation(dict, "EK", "Elama Kanahele"),
                new OutputAbbreviation(dict, "HA", "Henry Auwae"),
                new OutputAbbreviation(dict, "HHLH", "Helen Haleola Lee Hong"),
                new OutputAbbreviation(dict, "HKM", "Harry Kunihi Mitchell"),
                new OutputAbbreviation(dict, "JPM", "Joseph Puipui Makaai"),
                new OutputAbbreviation(dict, "KKK", "Kaui Keola Keamoai"),
                new OutputAbbreviation(dict, "LK", "Louise Keliihoomalu"),
                new OutputAbbreviation(dict, "MMLH", "Martha Manoanoa Lum Ho"),
                new OutputAbbreviation(dict, "MW", "Minnie Whitford"),
                new OutputAbbreviation(dict, "Anatomia", "Judd, Gerrit P. Anatomia"),
                new OutputAbbreviation(dict, "Bihopa", "Bihopa, E. A. Haawina Mua o ka Hoailona Helu"),
                new OutputAbbreviation(dict, "Bounty", "HeMoolelo no na Luina Kipi o ka Moku Bounty"),
                new OutputAbbreviation(dict, "Legendre", "Legendre, A. M. Ke Anahonua"),
                new OutputAbbreviation(dict, "Judd", "Judd et al. Hawaiian Language Imprints, 1822-1899"),
                new OutputAbbreviation(dict, "Pakaa", "Nakuina, Moses K. Pakaa a me Ku-a-Pakaa"),
                new OutputAbbreviation(dict, "Wilcox", "Wilcox, Robert"),
        });
        }
    }
}
