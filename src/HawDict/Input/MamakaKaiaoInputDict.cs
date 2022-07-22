// Copyright (c) Jon Thysell <http://jonthysell.com>
// Licensed under the MIT License.

using System;
using System.Text.RegularExpressions;

using HtmlAgilityPack;

namespace HawDict
{
    public class MamakaKaiaoInputDict : HtmlInputDict
    {
        public override string RawSourceFileName => $"{ID}.html.tmp";

        private static readonly object _tempFileLock = new object();

        public MamakaKaiaoInputDict(TranslationType translationType, LogLine logLine) : base("MamakaKaiao", translationType, logLine)
        {
            ShortTitle = $"Māmaka Kaiao ({(TranslationType == TranslationType.HawToEng ? "HAW-ENG" : "ENG-HAW")})";
            LongTitle = $"Māmaka Kaiao: A Modern Hawaiian Vocabulary ({(TranslationType == TranslationType.HawToEng ? "Hawaiian-English" : "English-Hawaiian")})";
            Description = "A compilation of Hawaiian words that have been created, collected, and approved by the Hawaiian Lexicon Committee from 1987 through 2000. Copyright (c) 2003 ʻAha Pūnana Leo and Hale Kuamoʻo, College of Hawaiian Language, University of Hawaiʻi at Hilo (ISBN 978-0824828035)";

            Authors.AddRange(new string[] { "Kōmike Huaʻōlelo", "Hale Kuamoʻo", "ʻAha Pūnana Leo" });

            SrcUrl = "https://web.archive.org/web/20201127014517/http://www.ulukau.org/elib/cgi-bin/library?e=d-0mkd-000Sec--11en-50-20-frameset-book--1-010escapewin&a=d&d=D0&toc=0";

            EntryHtmlTag = "p";
        }

        protected override void GetRawDataFromSource()
        {
            lock (_tempFileLock)
            {
                base.GetRawDataFromSource();
            }
        }

        private static readonly string HawToEngStart = "<h1 align=\"center\">M&#257;hele &#699;&#332;lelo Hawai&#699;i<br>\n<i>Hawaiian-English</i></h1>";
        private static readonly string EngToHawStart = "<h1 align=\"center\"> M&#257;hele &#699;&#332;lelo Pelek&#257;nia<br><i> English-Hawaiian</i></h1>";
        private static readonly string ErrataStart = "<h3 align=\"center\"> Hale Kuamo'o</h3>";
        private static readonly string ErrataEnd = "<h1 align=\"center\">&#699;Aha P&#363;nana Leo</h1>";

        protected override string CleanSourceHtml(string s)
        {
            // Remove doubled I-section in EngToHaw
            s = Regex.Replace(s, "<h1 align=\\\"center\\\">I<\\/h1>.*<h1 align=\\\" center\\\">I<\\/h1>", @"<h1 align=""center"">I</h1>", RegexOptions.Singleline);

            // Remove problematic errata section
            s = Regex.Replace(s, $"{ErrataStart}.*{ErrataEnd}", $"{ErrataStart}\n{ErrataEnd}", RegexOptions.Singleline);

            // Remove entries for wrong direction
            switch (TranslationType)
            {
                case TranslationType.HawToEng:
                    s = Regex.Replace(s, $"{EngToHawStart}.*{ErrataStart}", ErrataStart, RegexOptions.Singleline);
                    break;
                case TranslationType.EngToHaw:
                    s = Regex.Replace(s, $"{HawToEngStart}.*{EngToHawStart}", EngToHawStart, RegexOptions.Singleline);
                    break;
            }

            s = s
                .Replace("spanclass", "span class")
                .Replace("</td></tr></table><p>&nbsp;</p>\n<table style=\"word-break:break-word;margin-left:auto;margin-right:auto;width:700px;\"><tr><td>", "")
                .Replace("</td></tr></table><p>&nbsp;</p>\n<p>&nbsp;</p>\n<table style=\"word-break:break-word;margin-left:auto;margin-right:auto;width:700px;\"><tr><td>", "")
                .Replace("</p>\n\n\n \n\n\n <p align=\"Justify\">\n", " ")
                .Replace("</p>\n\n\n \n\n\n <p align=\"justify\"><i>", " <i>")
                .Replace("</p>\n\n\n \n\n\n <p align=\"Justify\"><i>", " <i>")
                .Replace("<p align=\"“Justify”\">", "<p align=\"Justify\">")
                .Replace("</i>\n</p>\n\n\n \n\n <p><i>", " ")
                .Replace("</p>\n\n\n \n\n\n <p>", " ")
                .Replace("</i></span>", "</span>")
                .Replace("<SPAN CLASS=\"HEAD\">", "<span class=\"head\">")
                .Replace("</SPAN>", "</span>")
                .Replace("align=\" justify\"", "align=\"justify\"")
                .Replace("\n</p>\n <p align=\"justify\"><span class=\"head\"></i>", "</i></p>\n <p align=\"justify\"><span class=\"head\">")
                .Replace("\n</p>\n</i> <p align=\"justify\"><span class=\"head\">", "\n</i></p>\n <p align=\"justify\"><span class=\"head\">")
                .Replace("decade Kekeke.</span></p>", "decade</span> Kekeke.</p>")
                .Replace("</span ", "</span>")
                .Replace("<span class=\"head>", "<span class=\"head\">")
                .Replace("<span class=\"head\'>", "<span class=\"head\">")
                .Replace("<span class=\"“head”\">", "<span class=\"head\">")
                .Replace("&#43;:", "&#43;").Replace("&#34; ", "&#43; ").Replace("&amp;$ ", " &#43; ")
                .Replace("&#183m", "&#183;m").Replace("&tilde,", "&tilde;,").Replace("&#233r", "&#233;r").Replace("&#0116l", "&#x016B;l").Replace("&#014D;", "&#x014D;")
                .Replace("</span> renal vein", "renal vein</span>")
                .Replace("<p> l&#257;", "\n</p>\n <p align=\"justify\"><span class=\"head\">l&#257;</span>")
                .Replace("</p>\n\n\n \n\n\n <p align=\"justify\">laws.", " laws.")
                .Replace("<i.handle.< i>", "<i>handle.</i>").Replace("period.&gt;", " period.").Replace("uila.<?p>", "uila.</p>")
                .Replace("<sub>0</sub>", "&#x2080;").Replace("<SUB>1</SUB>", "&#x2081;").Replace("<SUB>2</SUB>", "&#x2082;").Replace("<sub>1</sub>", "&#x2081;").Replace("<sub>2</sub>", "&#x2082;")
                .Replace("&ldquo;", "\"").Replace("&rdquo;", "\"")
                .Replace("</p>\n <p align=\"justify\"><span class=\"head\">&#256;.</span>", " &#256;.")
                .Replace("&#699;auk&#257;.See", "&#699;auk&#257;. See")
                .Replace("u.</i>s", "u.")
                .Replace("<i>.Abb.</i>", "<i>Abb.</i>")
                .Replace("<span class=\"head\">Au  Pala&#699;o  K&#363;.hou</span>", "<span class=\"head\">Au  Pala&#699;o  K&#363;&#183;hou</span>")
                .Replace("<span class=\"head\">Au  Pala&#699;o</span> K&#363; kahiko ", "<span class=\"head\">Au  Pala&#699;o K&#363;&#183;kahiko</span> ")
                .Replace("<span class=\"head\">Au Pala&#699;o K&#363;.waena</span>", "<span class=\"head\">Au Pala&#699;o K&#363;&#183;waena</span>")
                .Replace("i&#699;a puna&#183;kea</span>", "<p align=\"justify\"><span class=\"head\">i&#699;a puna&#183;kea</span>")
                .Replace("<span class=\"head\">k&#299;.like&#183;like</span>", "<span class=\"head\">k&#299;&#183;like&#183;like</span>")
                .Replace("Ho&#699;ok?pa&#699;i pukaaniani", "Ho&#699;ok&#363;pa&#699;i pukaaniani")
                .Replace("<i>Lit.,</i> cutting line.<br>\nchange should be made from the original, in proofreading; stet. <i>Lit.,</i> mark (for) leaving (as is).", "<i>Lit.,</i> cutting line.")
                .Replace("to show that no\n</p>", "to show that no change should be made from the original, in proofreading; stet. <i>Lit.,</i> mark (for) leaving (as is).\n</p>")
                .Replace("iodine &#699;", "iodine</span> &#699;")
                .Replace("i.e. the</i></p>", "i.e. the").Replace("pressure remains constant. ", "pressure remains constant.</i> ")
                .Replace("jack-o&#699;-lantern", "jack-o'-lantern")
                .Replace("June 10,1998", "June 10, 1998")
                .Replace("83, 93,103,113", "83, 93, 103, 113")
                .Replace("&#699;I;&#699;o wiliwili", "&#699;I&#699;o wiliwili")
                .Replace("k&#363;;kaelio", "k&#363;kaelio")
                .Replace("Organ</i></p>", "Organ</i> ")
                .Replace("<span class=\"head\">iliac</span> vein", "<span class=\"head\">iliac vein</span>")
                .Replace("<span class=\"head\">Ohta-san &#699;ukulele &#699;</span>Ukulele Ohta-san.", "<span class=\"head\">Ohta-san &#699;ukulele</span> &#699;Ukulele Ohta-san.")
                .Replace("K&#0116;lana.", "K&#363;lana.")
                .Replace("race <i>", "race</span> <i>")
                .Replace(">railing Ballustrade. </span>", ">railing</span> <i>Ballustrade</i>.")
                .Replace("reasonable K", "reasonable</span> K")
                .Replace("reasoning</span>.", "reasoning</span>")
                .Replace("receive <i>", "receive</span> <i>")
                .Replace("region <i>", "region</span> <i>")
                .Replace("renal artery A", "renal artery</span> A")
                .Replace("republic</sapn> <i>", "republic</span> <i>")
                .Replace("K&#363;h&amp;#014D;&#699;ailona.", "K&#363;h&#333;&#699;ailona.")
                .Replace("<i>To</i> &tilde; <i>smooth; also smooth</i> &tilde;<i>er;glib</i>", "<i>To &tilde; smooth; also smooth &tilde;er; glib</i>")
                .Replace("&tilde;  ejecta.", "&tilde; ejecta. ")
                .Replace("waena.</i>\nloa &#699;a&#699; ", "waena.</i></p>\n<p align=\"justify\"><span class=\"head\">loa</span> <i>&#699;a&#699;</i> ")
                .Replace("Slip &#699;n&#699; Slide", "Slip 'n' Slide")
                .Replace(".Double", "Double")
                .Replace("venugopal", "")
                .Replace("k&#333;&#183;pa&#699;a</span> Ma&#699;i&#699; <i>", "k&#333;&#183;pa&#699;a</span> <i>Ma&#699;i ")
                .Replace("s&#699;", "s'").Replace("&#699;s", "'s")
                .Replace("&#699;<i>a</i>&#699;", "<i>&#699;a&#699;</i>")
                .Replace("<i>&#699;a&#699; </i>", "<i>&#699;a&#699;</i> ")
                .Replace("</span> &#699;a&#699; ", "</span> <i>&#699;a&#699;</i> ")
                .Replace("</span> &#699; <i>a</i>&#699; ", "</span> <i>&#699;a&#699;</i> ")
                .Replace(".&#699;", ". &#699;").Replace(".</i>&#699;", ".</i> &#699;").Replace("&#699; ", "&#699;")
                .Replace(".<i>", ". <i>").Replace(",<i>", ", <i>")
                .Replace("&tilde;.", "&tilde;. ").Replace(".&tilde;", ". &tilde;")
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

        protected override string FinalCleanValue(string value)
        {
            value = StringUtils.FixSentenceEnd(value);
            return StringUtils.FixSentenceSpacing(value);
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
