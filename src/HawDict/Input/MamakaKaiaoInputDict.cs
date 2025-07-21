// Copyright (c) Jon Thysell <http://jonthysell.com>
// Licensed under the MIT License.

using System;
using System.Text.RegularExpressions;

using HtmlAgilityPack;

using QuickDict;

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
                .Replace("ack-o&#699;-lantern", "ack-o'-lantern")
                .Replace("June 10,1998", "June 10, 1998")
                .Replace("83, 93,103,113", "83, 93, 103, 113")
                .Replace("&#699;I;&#699;o wiliwili", "&#699;I&#699;o wiliwili")
                .Replace("k&#363;;kaelio", "k&#363;kaelio")
                .Replace("Organ</i></p>", "Organ</i> ")
                .Replace("<span class=\"head\">iliac</span> vein", "<span class=\"head\">iliac vein</span>")
                .Replace("<span class=\"head\">Ohta-san &#699;ukulele &#699;</span>Ukulele Ohta-san.", "<span class=\"head\">Ohta-san &#699;ukulele</span> &#699;Ukulele Ohta-san.")
                .Replace("K&#0116;lana.", "K&#363;lana.")
                .Replace(">a.m</span>.", ">a.m.</span>")
                .Replace(">Andaman &#699;Anadamana</span>.", ">Andaman</span> &#699;Anadamana.")
                .Replace(">ka&#699;a&#183;ne&#699;e.</span>", ">ka&#699;a&#183;ne&#699;e</span>")
                .Replace(">deceleration Emi m&#257;m&#257; holo.</span>", ">deceleration</span> Emi m&#257;m&#257; holo.")
                .Replace(">Marineris Malineli</span>.", ">Marineris</span> Malineli.")
                .Replace(">May Mei</span>", ">May</span> Mei.")
                .Replace(">mile Mile.</span>", ">mile</span> Mile.")
                .Replace(">muscle M&#257;kala.</span>", ">muscle</span> M&#257;kala.")
                .Replace(">nuclear  Nukelea.</span>", ">nuclear</span> Nukelea.")
                .Replace(">popsicle.</span>", ">popsicle</span>")
                .Replace("race <i>", "race</span> <i>")
                .Replace(">ranger Lanakia.</span>", ">ranger</span> Lanakia.")
                .Replace(">railing Ballustrade. </span>", ">railing</span> <i>Ballustrade</i>.")
                .Replace("reasonable K", "reasonable</span> K")
                .Replace("reasoning</span>.", "reasoning</span>")
                .Replace("receive <i>", "receive</span> <i>")
                .Replace("region <i>", "region</span> <i>")
                .Replace("renal artery A&#699;a pu&#699;wai", "renal artery</span> A&#699;a pu&#699;uwai")
                .Replace("republic</sapn> <i>", "republic</span> <i>")
                .Replace(">September  Kepakemapa.</span>", ">September</span> Kepakemapa.")
                .Replace(">-thon <i>Suffix.</span></i>", ">-thon</span> <i>Suffix.</i>")
                .Replace(">tile Kile</span>.", ">tile</span> Kile.")
                .Replace(">Water dispenser</span> <i>.", ">Water dispenser</span> <i>")
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
                .Replace("&tilde;.", "&tilde;. ").Replace("&tilde;.  ", "&tilde;. ").Replace(".&tilde;", ". &tilde;")
                .Replace("....", "&hellip;.").Replace("..", ".").Replace(".</i>.", ".</i>")
                .Replace("&#699;s", "'s").Replace("n&#699;t", "n't").Replace("I&#699;m", "I'm")
                .Replace("</span>.   Abbreviation for", ".</span> Abbreviation for").Replace("</span>.  Abbreviation for", ".</span> Abbreviation for").Replace("</span>. Abbreviation for", ".</span> Abbreviation for")
                .Replace("<i>Back&tilde;().</i>", "<i>Back&tilde; (\\).</i>")
                .Replace("&tilde;(<i>someone</i>)", "&tilde; (<i>someone</i>)")
                .Replace("Pa&#699;i<i>(preceded by</i> ke)", "Pa&#699;i (<i>preceded by</i> ke)")
                .Replace("iho.</i> Backslash ().", "iho.</i> Backslash (\\).")
                .Replace("backslash</span> <i>In printing ().</i>", "backslash</span> <i>In printing (\\).</i>")
                .Replace("&tilde; jar.</i> &#699;&#300;mole pepehi", "&tilde; jar.</i> &#699;&#332;mole pepehi")
                .Replace("curriculum vitae, r&#233;sum&#233;</i>.Mo&#699;om&#333;&#699;ali.", "curriculum vitae, r&#233;sum&#233;</i>. Mo&#699;om&#333;&#699;ali.")
                .Replace("<span class=\"head\">data</span> &#699;lkepili. See <i>database. To distri-bute &tilde;", "<span class=\"head\">data</span> &#699;Ikepili. See <i>database. To distribute &tilde;")
                .Replace("<i>In biology.</i> &#699;lli k&#363;waena.", "<i>In biology.</i> &#699;Ili k&#363;waena.")
                .Replace("<i>Thyroid</i> &tilde;. L&#699;ku&#699;u", "<i>Thyroid</i> &tilde;. L&#333;ku&#699;u")
                .Replace("hypothetical</span> &#699;ln&#257;&#699;in&#257;.", "hypothetical</span> &#699;In&#257;&#699;in&#257;.")
                .Replace(">Idaho</span> <i>Also ldahoan.</i> &#699;lkah&#333;.", ">Idaho</span> <i>Also Idahoan.</i> &#699;Ikah&#333;.")
                .Replace("&#699;lole", "&#699;Iole")
                .Replace("maidenhair fern</span> &#699;lwa&#699;iwa.", "maidenhair fern</span> &#699;Iwa&#699;iwa.")
                .Replace("<i>insurance.</i> &#699;lnikua olakino.", "<i>insurance.</i> &#699;Inikua olakino.")
                .Replace("magnet.</i> &#699;lne.", "magnet.</i> &#699;Ine.")
                .Replace("</i>&tilde;, <i>in volleyball.</i> Pa&#699;:i kulo.", "</i>&tilde;, <i>in volleyball.</i> Pa&#699;i kulo.")
                .Replace("skin</span> &#699;lli,", "skin</span> &#699;Ili,")
                .Replace("<i>Non-point</i> &#699;.", "<i>Non-point</i> &tilde;.")
                .Replace("stethoscope</span> &#699;lli", "stethoscope</span> &#699;Ili")
                .Replace("subcutus</span> <i>In biology.</i> &#699;lli", "subcutus</span> <i>In biology.</i> &#699;Ili")
                .Replace("surface area</span> <i>In math.</i> &#699;lli alo.", "surface area</span> <i>In math.</i> &#699;Ili alo.")
                .Replace("Inu i ka l&#257;&#699;au, &#699;ai i ka l&#257;&#699;'au.</p>", "Inu i ka l&#257;&#699;au, &#699;ai i ka l&#257;&#699;au.</p>")
                .Replace("Helu manawa; uaki, u&#699;ki. <i>To tell</i> &tilde; Helu uaki, helu u&#699;ki.", "Helu manawa; uaki, u&#257;ki. <i>To tell</i> &tilde; Helu uaki, helu u&#257;ki.")
                .Replace("Eng.+", "Eng. +").Replace("Eng. +<i>", "Eng. + <i>")
                .Replace(">hano   ho&#699;o&#183;kolo&#699;&#183;hua</span>", ">hano ho&#699;o&#183;kolo&#183;hua</span>")
                .Replace(">ho&#699;&#183;l&#257;&#183; lani</span>", ">ho&#699;o&#183;l&#257;&#183;lani</span>")
                .Replace(">ho&#699;&#183;maka&#183;&#699;&#257;i nana</span>", ">ho&#699;o&#183;maka&#183;&#699;&#257;i&#183;nana</span>")
                .Replace(">ho&#699;&#183;m&#257;&#183;lama&#183;lama</span>", ">ho&#699;o&#183;m&#257;&#183;lama&#183;lama</span>")
                .Replace("&#699;&#257;&#699;l.</i> Neck", "&#699;&#257;&#699;&#299;.</i> Neck")
                .Replace(">ho&#699;o&#183;&#699;puka&#183;puka</span>", ">ho&#699;o&#183;puka&#183;puka</span>")
                .Replace(">ho&#699;o&#183;&#699;pule&#183;lehua</span>", ">ho&#699;o&#183;pule&#183;lehua</span>")
                .Replace(">k&#363;&#699;kala   maka&#183;&#699;ala</span>", ">k&#363;&#183;kala maka&#183;&#699;ala</span>")
                .Replace("<i>&#699;lke ku&#699;una.</i> Traditional knowledge.", "<i>&#699;Ike ku&#699;una.</i> Traditional knowledge.")
                .Replace(">m&#257;&#699;kala   ke&#699;a&#183;haka</span>", ">m&#257;&#183;kala ke&#699;a&#183;haka</span>")
                .Replace(">wa&#699;a &#699;lli&#183;kini</span>", ">wa&#699;a &#699;Ili&#183;kini</span>")
                .Replace(">&#699;loa</span>", ">&#699;Ioa</span>")
                .Replace("as of body parts. <i>&#699;lke aku", "as of body parts. <i>&#699;Ike aku")
                .Replace("+0", "+").Replace("4+", "+")
                .Replace("+", " + ").Replace("=", " = ")
                .Replace(">pala&#183;h&#275; <i>&#699;a&#699;</span></i>", ">pala&#183;h&#275;</span> <i>&#699;a&#699;</i> ")
                .Replace(">&#699;&#257;&#183;ka&#699;a&#183;ka&#699;a <i>&#699;a&#699;</i></span>", ">&#699;&#257;&#183;ka&#699;a&#183;ka&#699;a</span> <i>&#699;a&#699;</i> ")
                ;

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

        protected override void AddAbbreviations(DictionaryBase dict)
        {
            dict.AddAbbreviation("abb.", "abbreviation");
            dict.AddAbbreviation("Bib.", "Bible");
            dict.AddAbbreviation("cf.", "compare", AbbreviationType.Auxiliary);
            dict.AddAbbreviation("comb.", "combined form");
            dict.AddAbbreviation("dic.", "dictionary definition");
            dict.AddAbbreviation("e.g.", "for example", AbbreviationType.Auxiliary);
            dict.AddAbbreviation("Eng.", "English");
            dict.AddAbbreviation("ext. mng.", "extended meaning");
            dict.AddAbbreviation("i.e.", "that is", AbbreviationType.Auxiliary);
            dict.AddAbbreviation("inv.", "invention");
            dict.AddAbbreviation("Japn.", "Japanese");
            dict.AddAbbreviation("lit.", "literally");
            dict.AddAbbreviation("mān.", "mānaleo (native speaker)");
            dict.AddAbbreviation("new mng.", "new meaning");
            dict.AddAbbreviation("PPN", "Proto Polynesian");
            dict.AddAbbreviation("redup.", "reduplication");
            dict.AddAbbreviation("sh.", "shortened form");
            dict.AddAbbreviation("sp. var.", "spelling variation");
            dict.AddAbbreviation("Tah.", "Tahitian");
            dict.AddAbbreviation("trad.", "traditional literary sources");
            dict.AddAbbreviation("var.", "variation");
            dict.AddAbbreviation("ham", "hamani (transitive verb)", AbbreviationType.Grammatical);
            dict.AddAbbreviation("heh", "hehele (intransitive verb)", AbbreviationType.Grammatical);
            dict.AddAbbreviation("ʻaʻ", "ʻaʻano (stative verb)", AbbreviationType.Grammatical);
            dict.AddAbbreviation("kik", "kikino (common noun)", AbbreviationType.Grammatical);
            dict.AddAbbreviation("iʻoa", "iʻoa (proper noun)", AbbreviationType.Grammatical);
            dict.AddAbbreviation("EK", "Elama Kanahele");
            dict.AddAbbreviation("HA", "Henry Auwae");
            dict.AddAbbreviation("HHLH", "Helen Haleola Lee Hong");
            dict.AddAbbreviation("HKM", "Harry Kunihi Mitchell");
            dict.AddAbbreviation("JPM", "Joseph Puipui Makaai");
            dict.AddAbbreviation("KKK", "Kaui Keola Keamoai");
            dict.AddAbbreviation("LK", "Louise Keliihoomalu");
            dict.AddAbbreviation("MMLH", "Martha Manoanoa Lum Ho");
            dict.AddAbbreviation("MW", "Minnie Whitford");
            dict.AddAbbreviation("Anatomia", "Judd, Gerrit P. Anatomia");
            dict.AddAbbreviation("Bihopa", "Bihopa, E. A. Haawina Mua o ka Hoailona Helu");
            dict.AddAbbreviation("Bounty", "HeMoolelo no na Luina Kipi o ka Moku Bounty");
            dict.AddAbbreviation("Legendre", "Legendre, A. M. Ke Anahonua");
            dict.AddAbbreviation("Judd", "Judd et al. Hawaiian Language Imprints, 1822-1899");
            dict.AddAbbreviation("Pakaa", "Nakuina, Moses K. Pakaa a me Ku-a-Pakaa");
            dict.AddAbbreviation("Wilcox", "Wilcox, Robert");
        }
    }
}
