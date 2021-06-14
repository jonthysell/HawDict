// Copyright (c) Jon Thysell <http://jonthysell.com>
// Licensed under the MIT License.

using System.Linq;
using System.Text.RegularExpressions;

using HtmlAgilityPack;

namespace HawDict
{
    public class PukuiElbertInputDict : HtmlInputDict
    {
        private static Regex IsEntryDivId;

        public PukuiElbertInputDict(TranslationType translationType, LogLine logLine) : base("PukuiElbert", translationType, logLine)
        {
            ShortTitle = string.Format("Hawaiian Dictionary ({0})", TranslationType == TranslationType.HawToEng ? "HAW-ENG" : "ENG-HAW");
            LongTitle = string.Format("Hawaiian Dictionary, Revised and Enlarged Edition ({0})", TranslationType == TranslationType.HawToEng ? "Hawaiian-English" : "English-Hawaiian");
            Description = "The reference standard Hawaiian-English and English-Hawaiian dictionary. Copyright (c) 1986 University of Hawaii Press (ISBN 978-0824807030)";

            Authors.AddRange(new string[] { "Mary Kawena Pūkuʻi", "Samuel H. Elbert" });

            switch (TranslationType)
            {
                case TranslationType.HawToEng:
                    SrcUrl = "http://www.ulukau.org/elib/cgi-bin/library?e=d-0ped-000Sec--11en-50-20-frameset-book--1-010escapewin&a=d&d=D0.3&toc=0";
                    break;
                case TranslationType.EngToHaw:
                    SrcUrl = "http://www.ulukau.org/elib/cgi-bin/library?e=d-0ped-000Sec--11en-50-20-frameset-book--1-010escapewin&a=d&d=D0.4&toc=0";
                    break;
            }

            EntryHtmlTag = "div";
        }

        protected override string CleanSourceHtml(string s)
        {
            return s
                .Replace("</td></tr></table><p>&nbsp;</p>\n<table style=\"word-break:break-word;margin-left:auto;margin-right:auto;width:700px;\"><tr><td>", "")
                .Replace("</td></tr></table><p>&nbsp;</p>\n<p>&nbsp;</p>\n<table style=\"word-break:break-word;margin-left:auto;margin-right:auto;width:700px;\"><tr><td>", "")
                .Replace("&4 ", "Redup. ").Replace("&;n", "n.").Replace("&(PCP; ", "(PCP ").Replace("(Mele ", "(Mele. ")
                .Replace("<span lang=\"HAW\">N&#257;n&#257;</span> 3", "<span>N&#257;n&#257;.</span> 3").Replace("N&#257;n&#257; 2", "N&#257;n&#257;. 2").Replace("N&#257;n&#257; 1", "N&#257;n&#257;. 1")
                .Replace("..", ".").Replace(".</span>.", ".</span>").Replace(".</em>.", ".</em>")
                .Replace("&ldquo;", "\"").Replace("&rdquo;", "\"")
                .Replace("T.44>", "")
                .Replace("h3", "span")
                // Typo fixes:
                .Replace("nuts containing while,", "nuts containing white,")
                ;
        }

        protected override bool IsEntryNode(HtmlNode node)
        {
            Regex regex = IsEntryDivId ?? (IsEntryDivId = new Regex(@"(ENG\.)?\w\.\d"));

            string id = node.Attributes["id"]?.DeEntitizeValue;

            return !string.IsNullOrWhiteSpace(id) && regex.IsMatch(id);
        }

        protected override string[] ParseEntryNode(HtmlNode node)
        {
            return node.ChildNodes.Where(c => c.Name == "span" || c.Name == "p").Select(n => StringUtils.NormalizeWhiteSpace(StringUtils.SingleLineNoTabs(n.OuterHtml))).ToArray();
        }

        protected override void AddAbbreviations(OutputDictBase dict)
        {
            dict.Abbreviations.AddRange(new OutputAbbreviation[]
            {
                new OutputAbbreviation(dict, "And.", "Andrews dictionary, 1865; reference is given only if no evidence is available other than that in Andrews and Andrews-Parker (AP)"),
                new OutputAbbreviation(dict, "AP", "Andrews-Parker dictionary, 1922; reference is given only if no evidence is available other than that in Andrews (And.) and Andrews-Parker"),
                new OutputAbbreviation(dict, "Cap.", "beginning with a capital letter"),
                new OutputAbbreviation(dict, "caus/sim.", "causative/simulative", AbbreviationType.Grammatical),
                new OutputAbbreviation(dict, "cf.", "compare", AbbreviationType.Auxiliary),
                new OutputAbbreviation(dict, "conj.", "conjunction", AbbreviationType.Grammatical),
                new OutputAbbreviation(dict, "demon.", "demonstrative", AbbreviationType.Grammatical),
                new OutputAbbreviation(dict, "Eng.", "word borrowed from English"),
                new OutputAbbreviation(dict, "ex.", "example, examples", AbbreviationType.Auxiliary),
                new OutputAbbreviation(dict, "f.", "form (in names of plants)"),
                new OutputAbbreviation(dict, "fig.", "figuratively"),
                new OutputAbbreviation(dict, "For.", "Fornander, Hawaiian Antiquities (For. 4:297 = Fornander Vol. 4, p. 297)"),
                new OutputAbbreviation(dict, "FS", "Elbert, Selections from Fornander"),
                new OutputAbbreviation(dict, "GP", "Green and Pukui, Legend of Kawelo"),
                new OutputAbbreviation(dict, "Gr.", "word probably borrowed from Greek"),
                new OutputAbbreviation(dict, "Gram.", "Elbert and Pukui, Hawaiian Grammar"),
                new OutputAbbreviation(dict, "Heb.", "word probably borrowed from Hebrew"),
                new OutputAbbreviation(dict, "HM", "Beckwith, Hawaiian Mythology"),
                new OutputAbbreviation(dict, "HP", "Handy, Hawaiian Planter"),
                new OutputAbbreviation(dict, "Ii", "Ii, Fragments of Hawaiian History"),
                new OutputAbbreviation(dict, "interr.", "interrogative", AbbreviationType.Grammatical),
                new OutputAbbreviation(dict, "interj.", "interjection", AbbreviationType.Grammatical),
                new OutputAbbreviation(dict, "Kam. 1964", "Kamakau, Ka Poʻe Kahiko"),
                new OutputAbbreviation(dict, "Kam. 1976", "Kamakau, The Works of the People of Old"),
                new OutputAbbreviation(dict, "Kel.", "Kelekona, Kaluaikoolau"),
                new OutputAbbreviation(dict, "Kep.", "Beckwith, Kepelino"),
                new OutputAbbreviation(dict, "KJV", "King James Version of the Bible"),
                new OutputAbbreviation(dict, "KL.", "Beckwith, Kumulipo"),
                new OutputAbbreviation(dict, "Laie", "Beckwith, Laieikawai"),
                new OutputAbbreviation(dict, "lit.", "literally"),
                new OutputAbbreviation(dict, "loc.n.", "locative noun", AbbreviationType.Grammatical),
                new OutputAbbreviation(dict, "Malo", "Malo, Hawaiian Antiquities, 1951"),
                new OutputAbbreviation(dict, "MK", "Ke Alanui o ka Lani, Oia ka Manuale Kakolika"),
                new OutputAbbreviation(dict, "n.v.", "noun-verb", AbbreviationType.Grammatical),
                new OutputAbbreviation(dict, "n.", "noun", AbbreviationType.Grammatical),
                new OutputAbbreviation(dict, "Nak.", "Nakuina, Moolelo Hawaii ..."),
                new OutputAbbreviation(dict, "Nānā.", "Pukui, Haertig, Lee, Nānā i ke Kumu"),
                new OutputAbbreviation(dict, "Neal", "Neal, In Gardens of Hawaii, 1965"),
                new OutputAbbreviation(dict, "num.", "numeral", AbbreviationType.Grammatical),
                new OutputAbbreviation(dict, "par.", "particle", AbbreviationType.Grammatical),
                new OutputAbbreviation(dict, "pas/imp.", "passive/imperative", AbbreviationType.Grammatical),
                new OutputAbbreviation(dict, "PH", "Emerson, Pele and Hiiaka"),
                new OutputAbbreviation(dict, "pl.", "plural", AbbreviationType.Grammatical),
                new OutputAbbreviation(dict, "PCP", "Proto Central Polynesian"),
                new OutputAbbreviation(dict, "PEP", "Proto East Polynesian"),
                new OutputAbbreviation(dict, "PNP", "Proto Nuclear Polynesian"),
                new OutputAbbreviation(dict, "poss.", "possessive", AbbreviationType.Grammatical),
                new OutputAbbreviation(dict, "PPN", "Proto Polynesian"),
                new OutputAbbreviation(dict, "prep.", "preposition", AbbreviationType.Grammatical),
                new OutputAbbreviation(dict, "RC", "Kamakau, Ruling Chiefs"),
                new OutputAbbreviation(dict, "redup.", "reduplication (for meanings of reduplications, see Gram. 6.2.2)"),
                new OutputAbbreviation(dict, "RSV", "Holy Bible, Revised Standard Version"),
                new OutputAbbreviation(dict, "sg.", "singular", AbbreviationType.Grammatical),
                new OutputAbbreviation(dict, "sp., spp.", "species"),
                new OutputAbbreviation(dict, "TC", "Taro Collection"),
                new OutputAbbreviation(dict, "UL", "Emerson, Unwritten Literature"),
                new OutputAbbreviation(dict, "v.", "verb", AbbreviationType.Grammatical),
                new OutputAbbreviation(dict, "var.", "variant, variety"),
                new OutputAbbreviation(dict, "nvi.", "noun-intransitive verb", AbbreviationType.Grammatical),
                new OutputAbbreviation(dict, "nvs.", "noun-stative verb", AbbreviationType.Grammatical),
                new OutputAbbreviation(dict, "nvt.", "noun-transitive verb", AbbreviationType.Grammatical),
                new OutputAbbreviation(dict, "vi.", "intransitive verb", AbbreviationType.Grammatical),
                new OutputAbbreviation(dict, "vs.", "stative verb", AbbreviationType.Grammatical),
                new OutputAbbreviation(dict, "vt.", "transitive verb", AbbreviationType.Grammatical),
                new OutputAbbreviation(dict, "Am.", "Amosa (Amos)"),
                new OutputAbbreviation(dict, "Dan.", "Daniela (Daniel)"),
                new OutputAbbreviation(dict, "Epeso", "(Ephesians)"),
                new OutputAbbreviation(dict, "Eset.", "Esetera (Esther)"),
                new OutputAbbreviation(dict, "Ezek.", "Ezekiela (Ezekiel)"),
                new OutputAbbreviation(dict, "Ezera", "(Ezra)"),
                new OutputAbbreviation(dict, "Gal.", "Galatia (Galatians)"),
                new OutputAbbreviation(dict, "Hagai", "(Haggai)"),
                new OutputAbbreviation(dict, "Hal.", "Halelu (Psalms)"),
                new OutputAbbreviation(dict, "Heb.", "Hebera (Hebrews)"),
                new OutputAbbreviation(dict, "Hoik.", "Hoikeana (Revelation)"),
                new OutputAbbreviation(dict, "Hos.", "Hosea (Hosea)"),
                new OutputAbbreviation(dict, "Iak.", "Iakobo (James)"),
                new OutputAbbreviation(dict, "Ier.", "Ieremia (Jeremiah)"),
                new OutputAbbreviation(dict, "Ioane", "(John)"),
                new OutputAbbreviation(dict, "Ioba", "(Job)"),
                new OutputAbbreviation(dict, "Ioela", "(Joel)"),
                new OutputAbbreviation(dict, "Ios.", "Iosua (Joshua)"),
                new OutputAbbreviation(dict, "Isa.", "Isaia (Isaiah)"),
                new OutputAbbreviation(dict, "Iuda", "(Jude)"),
                new OutputAbbreviation(dict, "Kanl.", "Kanawailua (Deuteronomy)"),
                new OutputAbbreviation(dict, "Kekah.", "Kekahuna (Ecclesiastes)"),
                new OutputAbbreviation(dict, "Kin.", "Kinohi (Genesis)"),
                new OutputAbbreviation(dict, "Kol.", "Kolosa (Colosians)"),
                new OutputAbbreviation(dict, "Kor.", "Korineto (Corinthians)"),
                new OutputAbbreviation(dict, "Luka", "(Luke)"),
                new OutputAbbreviation(dict, "Lunk.", "Lunakanawai (Judges)"),
                new OutputAbbreviation(dict, "Mal.", "Malaki (Malachi)"),
                new OutputAbbreviation(dict, "Mar.", "Mareko (Mark)"),
                new OutputAbbreviation(dict, "Mat.", "Mataio (Matthew)"),
                new OutputAbbreviation(dict, "Mele.", "Mele a Solomona (Songs of Solomon)"),
                new OutputAbbreviation(dict, "Mika", "(Micah)"),
                new OutputAbbreviation(dict, "Nah.", "Nahelu (Numbers)"),
                new OutputAbbreviation(dict, "Nal.", "Nalii (Kings)"),
                new OutputAbbreviation(dict, "Neh.", "Nehemia (Nehemia)"),
                new OutputAbbreviation(dict, "Oih.", "Oihana (Acts)"),
                new OutputAbbreviation(dict, "Oihk.", "Oihanakahuna (Leviticus)"),
                new OutputAbbreviation(dict, "Oihn.", "Oihanaalii (Chronicles)"),
                new OutputAbbreviation(dict, "Pet.", "Petero (Peter)"),
                new OutputAbbreviation(dict, "Pilipi", "(Philippians)"),
                new OutputAbbreviation(dict, "Puk.", "Pukaana (Exodus)"),
                new OutputAbbreviation(dict, "Roma", "(Romans)"),
                new OutputAbbreviation(dict, "Ruta", "(Ruth)"),
                new OutputAbbreviation(dict, "Sam.", "Samuela (Samuel)"),
                new OutputAbbreviation(dict, "Sol.", "Solomona (Proverbs)"),
                new OutputAbbreviation(dict, "Tes.", "Tesalonike (Thessalonians)"),
                new OutputAbbreviation(dict, "Tim.", "Timoteo (Timothy)"),
                new OutputAbbreviation(dict, "Tito", "(Titus)"),
                new OutputAbbreviation(dict, "Zek.", "Zekaria (Zechariah)"),
                new OutputAbbreviation(dict, "Zep.", "Zepania (Zephaniah)"),
            });
        }
    }
}
