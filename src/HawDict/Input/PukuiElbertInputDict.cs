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
            ShortTitle = $"Hawaiian Dictionary ({(TranslationType == TranslationType.HawToEng ? "HAW-ENG" : "ENG-HAW")})";
            LongTitle = $"Hawaiian Dictionary, Revised and Enlarged Edition ({(TranslationType == TranslationType.HawToEng ? "Hawaiian-English" : "English-Hawaiian")})";
            Description = "The reference standard Hawaiian-English and English-Hawaiian dictionary. Copyright (c) 1986 University of Hawaii Press (ISBN 978-0824807030)";

            Authors.AddRange(new string[] { "Mary Kawena Pūkuʻi", "Samuel H. Elbert" });

            switch (TranslationType)
            {
                case TranslationType.HawToEng:
                    SrcUrl = "https://web.archive.org/web/20191218162453/http://ulukau.org/elib/cgi-bin/library?e=d-0ped-000Sec--11en-50-20-frameset-book--1-010escapewin&a=d&d=D0.3&toc=0";
                    break;
                case TranslationType.EngToHaw:
                    SrcUrl = "https://web.archive.org/web/20191218162453/http://ulukau.org/elib/cgi-bin/library?e=d-0ped-000Sec--11en-50-20-frameset-book--1-010escapewin&a=d&d=D0.4&toc=0";
                    break;
            }

            EntryHtmlTag = "div";
        }

        protected override string CleanSourceHtml(string s)
        {
            // Remove header comments
            s = Regex.Replace(s, "<p>In causative/simulative forms beginning with.*\n", "");
            return s
                .Replace("</td></tr></table><p>&nbsp;</p>\n<table style=\"word-break:break-word;margin-left:auto;margin-right:auto;width:700px;\"><tr><td>", "")
                .Replace("</td></tr></table><p>&nbsp;</p>\n<p>&nbsp;</p>\n<table style=\"word-break:break-word;margin-left:auto;margin-right:auto;width:700px;\"><tr><td>", "")
                .Replace("&amp;4 ", "Redup. ").Replace("&amp;;n", "n.").Replace("&amp;(PCP; ", "(PCP ").Replace("(Mele ", "(Mele. ")
                .Replace("<span lang=\"HAW\">N&#257;n&#257;</span> 3", "<span>N&#257;n&#257;.</span> 3").Replace("N&#257;n&#257; 2", "N&#257;n&#257;. 2").Replace("N&#257;n&#257; 1", "N&#257;n&#257;. 1")
                .Replace("..", ".").Replace(".</span>.", ".</span>").Replace(".</em>.", ".</em>")
                .Replace("&ldquo;", "\"").Replace("&rdquo;", "\"")
                .Replace("T.44&gt;", "")
                .Replace("h3", "span")
                // Typo fixes:
                .Replace("nuts containing while,", "nuts containing white,")
                .Replace("Country?f", "Country")
                .Replace("palapala ho&#699;oku,u", "palapala ho&#699;oku&#699;u")
                .Replace("see saying, &#699;ol&#275;,&#699;ol&#275;", "see saying, &#699;ol&#275;&#699;ol&#275;")
                .Replace("Haw.-Eng.&#699;", "Haw.-Eng.")
                .Replace("See &#699;ao&#699;ao &#699;ai,&#699;&#275;,", "See &#699;ao&#699;ao &#699;ai&#699;&#275;,")
                .Replace("(<span>Gram. 9.6.1</span>.)<span lang=\"HAW\">", "(<span>Gram. 9.6.1</span>.) <span lang=\"HAW\">")
                .Replace("(<span>Gram. 10.1</span>.)(PPN <span lang=\"HAW\">t-</span>.)", "(<span>Gram. 10.1</span>.) (PPN <span lang=\"HAW\">t-</span>.)")
                .Replace("<p>See <span lang=\"HAW\">m&#257;,au&#275;</span>.</p>", "<p>See <span lang=\"HAW\">m&#257;&#699;au&#275;</span>.</p>")
                .Replace("<span lang=\"HAW\">ho&#699;o.mo&#699;o,puna</span>", "<span lang=\"HAW\">ho&#699;o.mo&#699;o.puna</span>")
                .Replace("<span lang=\"HAW\">h&#333;,&#699;olo.p&#363;</span>", "<span lang=\"HAW\">h&#333;.&#699;olo.p&#363;</span>")
                .Replace("<span lang=\"HAW\">ho&#699;o,poe.poe</span>Redup.", "<span lang=\"HAW\">ho&#699;o.poe.poe</span> Redup.")
                .Replace("<span lang=\"HAW\">ho&#699;o.p&#333;,loli</span>", "<span lang=\"HAW\">ho&#699;o.p&#333;.loli</span>")
                .Replace("<span lang=\"HAW\">&#699;&#257;pe&#699;,&#699;ape&#699;a</span>", "<span lang=\"HAW\">&#699;&#257;pe&#699;ape&#699;a</span>")
                .Replace("<span lang=\"HAW\">kani&#257;,&#699;au</span>", "<span lang=\"HAW\">kani&#257;&#699;au</span>")
                .Replace("<span lang=\"HAW\">ho&#699;on&#257;,aikola</span>", "<span lang=\"HAW\">ho&#699;on&#257;&#699;aikola</span>")
                .Replace("<span lang=\"HAW\">ho&#699;o.lako</span> To insinuate", "<span lang=\"HAW\">ho&#699;o.loko</span> To insinuate")
                .Replace("<span lang=\"HAW\">&#699;&#257;l&#699;a</span>", "<span lang=\"HAW\">&#699;&#257;la&#699;a</span>")
                .Replace("<span lang=\"HAW\">l&#699;opihi kapu.a&#699;i lio </span>", "<span lang=\"HAW\">&#699;opihi kapu.a&#699;i lio </span>")
                .Replace("<span lang=\"HAW\">Pulo (Buro) Ho&#699;o.laha l&#699;a </span>", "<span lang=\"HAW\">Pulo (Buro) Ho&#699;o.laha I&#699;a </span>")
                .Replace("<span lang=\"HAW\">No k&#275;ia ma&#699;a o ke ail&#699;i</span>", "<span lang=\"HAW\">No k&#275;ia ma&#699;a o ke ali&#699;i</span>")
                .Replace("<span lang=\"HAW\">&#699;e&#699;lek&#363;</span>", "<span lang=\"HAW\">&#699;e&#699;elek&#363;</span>")
                .Replace("<span lang=\"HAW\">&#699;a&#699;ahu ali&#699;l</span>", "<span lang=\"HAW\">&#699;a&#699;ahu ali&#699;i</span>")
                .Replace("<span lang=\"ENG\">I&#699;ll ", "<span lang=\"ENG\">I'll ")
                .Replace("<span lang=\"HAW\">ilo<span>nvi.</span> </span>", "<span lang=\"HAW\">ilo</span>").Replace("<p><span>1.</span> Maggot,", "<p><span>1.</span> <span>nvi.</span> Maggot,")
                .Replace("<span lang=\"HAW\">p&#257; hao<span>n.</span> </span>", "<span lang=\"HAW\">p&#257; hao</span>").Replace("<p><span>1.</span>  Iron fence.</p>", "<p><span>1.</span> <span>n.</span> Iron fence.</p>")
                .Replace("<span lang=\"HAW\">p&#257; hau<span>n.</span> </span>", "<span lang=\"HAW\">p&#257; hau</span>").Replace("<p><span>1.</span> Enclosure or fence of <span lang=\"HAW\">hau</span> trees.</p>", "<p><span>1.</span> <span>n.</span> Enclosure or fence of <span lang=\"HAW\">hau</span> trees.</p>")
                .Replace("<span lang=\"HAW\">pahele<span>nvt.</span> </span>", "<span lang=\"HAW\">pahele</span>").Replace("<p><span>1.</span>  A snare, noose, trap;", "<p><span>1.</span> <span>nvt.</span> A snare, noose, trap;")
                .Replace("span lang=\"HAW\">(pulu)</span>)", "span lang=\"HAW\">(pulu)</span>")
                .Replace("sunlight. (PPN <span lang=\"HAW\">&#699;la&#699;aa</span>.)", "sunlight. (PPN <span lang=\"HAW\">la&#699;aa</span>.)")
                .Replace("<span lang=\"HAW\">ho&#699;o.li&#699;o&#699;li&#699;o</span>", "<span lang=\"HAW\">ho&#699;o.li&#699;o.li&#699;o</span>")
                .Replace("&ldquo;k&#257;nalua.rd;", "&ldquo;k&#257;nalua.&rdquo;").Replace("kona &#699;doia&#699;i&#699;o</span>", "kona &#699;oia&#699;i&#699;o</span>")
                .Replace("&#699;s", "'s").Replace("s&#699; ", "s' ").Replace("n&#699;t", "n't").Replace("I&#699;m", "I'm").Replace("you&#699;d", "you'd").Replace("I&#699;ve", "I've").Replace("o&#699;clock", "o'clock").Replace("ou&#699;re", "ou're").Replace("we&#699;re", "we're")
                .Replace("P.M. </span>", "P.M.</span>").Replace("O.K. </span>", "O.K.</span>").Replace(". </span>", " </span>")
                .Replace("<span lang=\"HAW\">&#699;a&#699;nali&#699;i</span>,", "<span lang=\"HAW\">&#699;a&#699;anali&#699;i</span>,")
                .Replace("<span lang=\"HAW\">Ko&#699;na &#699;awa</span>", "<span lang=\"HAW\">Ko&#699;ana &#699;awa</span>")
                .Replace("<span>Cf.</span> &#699;nina, <span lang=\"HAW\">pap&#257;lina</span>, &#699;ulika, &#699;ulina.</p>", "<span>Cf.</span> nina, <span lang=\"HAW\">pap&#257;lina</span>, &#699;&#363;lika, &#699;&#363;lina.</p>")
                .Replace("<span lang=\"HAW\">He k&#257;naenae aloha n&#699;au i&#257;&#699;oe,", "<span lang=\"HAW\">He k&#257;naenae aloha na&#699;u i&#257;&#699;oe,")
                .Replace("PPN<span", "PPN <span")
                .Replace("</span>(as on a horse).", "</span> (as on a horse).")
                .Replace("kani</span>(bell).", "kani</span> (bell).")
                .Replace("ilo</span>(young)", "ilo</span> (young)")
                .Replace("<span lang=\"HAW\">&#699;&#699;a&#699;ole", "<span lang=\"HAW\">&#699;a&#699;ole")
                .Replace("<span lang=\"HAW\">k&#243;lomoku</span>", "<span lang=\"HAW\">kolomoku</span>")
                .Replace("(Spanish, <span lang=\"SPA\">espa&ntilde;nol</span>.)", "(Spanish, <span lang=\"SPA\">espa&ntilde;ol</span>.)")
                .Replace("<span lang=\"HAW\">ku&#699;hike&#699;e</span>", "<span lang=\"HAW\">kuhike&#699;e</span>")
                .Replace(">&#699;Wai-pi&#699;o</span> is drowsy", ">Wai-pi&#699;o</span> is drowsy")
                .Replace(">&#699;Elim&#699;a keneka</span>", ">&#699;Elima keneka</span>")
                .Replace(">l&#257;&#699;au &#699;pili</span>", ">l&#257;&#699;au pili</span>")
                .Replace(">&#699;hio</span>", ">hio</span>")
                .Replace("(splitting);<span lang=\"HAW\">", "(splitting); <span lang=\"HAW\">")
                .Replace("<span lang=\"HAW\">h&#699;olimalima</span>", "<span lang=\"HAW\">ho&#699;olimalima</span>")
                .Replace("hors d&#699;oeuvre", "hors d'oeuvre")
                .Replace(">laho &#699;p&#333;ka&#699;oka&#699;o</span>", ">laho p&#333;ka&#699;oka&#699;o</span>")
                .Replace(">&#699;wai lohia</span>", ">wai lohia</span>")
                .Replace("jack-o&#699;-lantern", "jack-o'-lantern")
                .Replace(">ko&#699;oko&#699;o&#699;</span>", ">ko&#699;oko&#699;o</span>")
                .Replace(">&#699;hali&#699;a</span>", ">hali&#699;a</span>")
                .Replace(">k&#257;&#699;i&#699;_</span>", ">k&#257;&#699;i&#699;&#299;</span>")
                .Replace(">k&#699;u</span> (of sweet potato)", ">ki&#699;u</span> (of sweet potato)")
                .Replace(">&#699;Auted&#699;e</span>", ">&#699;Auhe&#699;e</span>")
                .Replace(">&#699;k&#363;kini</span>", ">k&#363;kini</span>")
                .Replace(">koloau; ho&#699;helei</span>", ">koloau; ho&#699;ohelei</span>")
                .Replace(">k&#299;&#699;o&#699;ki;", ">k&#299;&#699;oki;")
                .Replace(">h&#275;&#699;i&#699;_</span>", ">h&#275;&#699;i&#299;</span>")
                .Replace(">Noho pokakak&#699;a</span>", ">Noho pokaka&#699;a</span>")
                .Replace(">&#699;&#257;&#699;kena k&#257;.lepa ka&#699;a.hele </span>", ">&#699;&#257;.kena k&#257;.lepa ka&#699;a.hele </span>")
                .Replace(">ha&#699;a&#699;wina ho&#699;o.hano.hano </span>", ">ha&#699;a.wina ho&#699;o.hano.hano </span>")
                .Replace(">hao ho&#699;.pa&#699;a lima </span>", ">hao ho&#699;o.pa&#699;a lima </span>")
                .Replace(">h&#333;.&#699;ali,&#699;ali </span>", ">h&#333;.&#699;ali.&#699;ali </span>")
                .Replace(">Ni&#699;hau</span>", ">Ni&#699;ihau</span>")
                .Replace(">&#699;iu,&#699;iu </span>", ">&#699;iu.&#699;iu </span>")
                .Replace(">&#699;iwa,&#699;iwa </span>", ">&#699;iwa.&#699;iwa </span>")
                .Replace("kahi pu&#699;u&#699; pele</span>", "kahi pu&#699;u pele</span>")
                .Replace(">h&#699;o.kakale</span>", ">ho&#699;o.kakale</span>")
                .Replace(">ho&#699;Fo.kana.ha&#699;u</span>", ">ho&#699;o.kana.ha&#699;u</span>")
                .Replace(">kukua,&#699;au </span>", ">kukua.&#699;au </span>")
                .Replace(">p&#363;a&#699;_ pipi</span>", ">p&#363;&#699;&#257; pipi</span>")
                .Replace(">&#699;Ulu-pala-kua, &#699;Maui</span>", ">&#699;Ulu-pala-kua, Maui</span>")
                .Replace(">&#699;Ka lau luhea", ">Ka lau luhea")
                .Replace(">ho&#699;.m&#257;.h&#363;</span>", ">ho&#699;o.m&#257;.h&#363;</span>")
                .Replace(">ma._&#699;&#299;.&#699;&#299; </span>", ">m&#257;.&#699;&#299;.&#699;&#299; </span>")
                .Replace(">&#699;i&#699;wi haole</span>", ">&#699;i&#699;iwi haole</span>")
                .Replace(">&#699;ho&#699;o.m&#257;.&#699;ona</span>", ">ho&#699;o.m&#257;.&#699;ona</span>")
                .Replace(">Pu&#699;u&#699; ka nuku</span>", ">Pu&#699;u ka nuku</span>")
                .Replace(">&#699;oama,&#699;owama </span>", ">&#699;oama, &#699;owama </span>")
                .Replace(">k&#257;&#699;hili</span>", ">k&#257;hili</span>")
                .Replace(">&#699;&#333;,&#699;&#363;-holo-wai </span>", ">&#699;&#333;.&#699;&#363;-holo-wai </span>")
                .Replace("PPN <span lang=\"HAW\">pa&#699;a&#699;</span>", "PPN <span lang=\"HAW\">pa&#699;a</span>")
                .Replace(">p&#257;,&#699;ihi.&#699;ihi </span>", ">p&#257;.&#699;ihi.&#699;ihi </span>")
                .Replace(">pa&#699;u&#699;pa&#699;u </span>", ">pa&#699;u.pa&#699;u </span>")
                .Replace(">p&#257;&#699;.wehe p&#363;.p&#363; </span>", ">p&#257;.wehe p&#363;.p&#363; </span>")
                .Replace(">b&#699;rith</span>", ">b'rith</span>")
                .Replace(">nani Wai&#699;-ale&#699;ale</span>", ">nani Wai-&#699;ale&#699;ale</span>")
                .Replace(">Ua &#699;po&#699;ip&#363;", ">Ua po&#699;ip&#363;")
                .Replace(">P&#333;lena p&#699;aa", ">P&#333;lena pa&#699;a")
                .Replace(">pua&#699;ki&#699;o</span>", ">pu&#699;a ki&#699;o</span>")
                .Replace(">pu&#699;u&#699;.pu&#699;u l&#257;.&#699;au </span>", ">pu&#699;u.pu&#699;u l&#257;.&#699;au </span>")
                .Replace(">u&#275; (uw&#275;)&#699;ala.l&#257; </span>", ">u&#275; (uw&#275;) &#699;ala.l&#257; </span>")
                // Missing definition number fixes
                .Replace("<p><span>n.</span> Name of a large valley on", "<p>1. <span>n.</span> Name of a large valley on")
                ;
        }

        protected override bool IsEntryNode(HtmlNode node)
        {
            Regex regex = IsEntryDivId ??= new Regex(@"(ENG\.)?\w\.\d");

            string id = node.Attributes["id"]?.DeEntitizeValue;

            return !string.IsNullOrWhiteSpace(id) && regex.IsMatch(id);
        }

        protected override string[] ParseEntryNode(HtmlNode node)
        {
            return node.ChildNodes.Where(c => c.Name == "span" || c.Name == "p").Select(n => StringUtils.NormalizeWhiteSpace(StringUtils.SingleLineNoTabs(n.OuterHtml))).ToArray();
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
