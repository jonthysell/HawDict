// Copyright (c) Jon Thysell <http://jonthysell.com>
// Licensed under the MIT License.

using System.Linq;
using System.Text.RegularExpressions;

using HtmlAgilityPack;

using QuickDict;

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
            s = s
                .Replace("</td></tr></table><p>&nbsp;</p>\n<table style=\"word-break:break-word;margin-left:auto;margin-right:auto;width:700px;\"><tr><td>", "")
                .Replace("</td></tr></table><p>&nbsp;</p>\n<p>&nbsp;</p>\n<table style=\"word-break:break-word;margin-left:auto;margin-right:auto;width:700px;\"><tr><td>", "")
                .Replace("&amp;4 ", "Redup. ").Replace("&amp;;n", "n.").Replace("&amp;(PCP; ", "(PCP ").Replace("(Mele. ", "(Mele ")
                .Replace("..", ".").Replace(".</span>.", ".</span>").Replace(".</em>.", ".</em>")
                .Replace("&ldquo;", "\"").Replace("&rdquo;", "\"")
                .Replace(" ,", ",")
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
                .Replace(">hale papa&#699; i</span>", ">hale papa&#699;i</span>")
                .Replace(">ho&#699; opa&#699;a mana&#699;o</span>", ">ho&#699;opa&#699;a mana&#699;o</span>")
                .Replace(">ho&#699; ohuli</span>", ">ho&#699;ohuli</span>")
                .Replace(">ma&#699; i hilo</span>", ">ma&#699;i hilo</span>")
                .Replace(">&#257;&#699; &#333;.&#699;&#333; </span>", ">&#257; &#699;&#333;.&#699;&#333; </span>")
                .Replace(">kauwa&#699;</span>", ">kauwa&#699;e</span>")
                .Replace(">Ho &#699;ohekili maila o&#699; I&#275;hova</span>", ">Ho&#699;ohekili maila &#699;o I&#275;hova</span>")
                .Replace(">E&#699; ho&#699;ohinuhinu ana &#699;a Ke-ola i kona k&#257;ma&#699;a</span>", ">E ho&#699;ohinuhinu ana &#699;a Ke-ola i kona k&#257;ma&#699;a</span>")
                .Replace(">k&#363;a.&#699;ai ho&#699; o.lilo </span>", ">k&#363;a.&#699;ai ho&#699;o.lilo </span>")
                .Replace(">Mo&#699;a le&#699; a</span>", ">Mo&#699;a le&#699;a</span>")
                .Replace(">lua&#699;eli p&#333;.haku </span>", ">lua &#699;eli p&#333;.haku </span>")
                .Replace(">lua&#699; eli wai.wai </span>", ">lua &#699;eli wai.wai </span>")
                .Replace("<span>Cf.</span> mea ho&#699; oulu", "<span>Cf.</span> mea ho&#699;oulu")
                .Replace("may myriadsu", "may myriads")
                .Replace("<span lang=\"HAW\">'ala.mo&#699;o", "<span lang=\"HAW\">&#699;ala.mo&#699;o")
                .Replace("<span lang=\"HAW\">&#699;ana.'ana.pu'u", "<span lang=\"HAW\">&#699;ana.&#699;ana.pu&#699;u")
                .Replace("<span lang=\"HAW\">&#699;Ana&#699;anapu'u ka uila", "<span lang=\"HAW\">&#699;Ana&#699;anapu&#699;u ka uila")
                .Replace("<span lang=\"HAW\">Ka-p&#363;,lehu", "<span lang=\"HAW\">Ka-p&#363;.lehu")
                .Replace("<span lang=\"HAW\">&#257;&hellip;paha", "<span lang=\"HAW\">&#257; &hellip; paha")
                // Typos with _
                .Replace("<span>Na_na_", "<span>N&#257;n&#257;.")
                .Replace(">Palaki &#699;an_ai</span>", ">Palaki &#699;&#257;nai</span>")
                .Replace("<span lang=\"HAW\">Pal_aha", "<span lang=\"HAW\">P&#257;laha")
                .Replace(">ka mea an_a", ">ka mea &#257;na")
                .Replace(">keiki ho&#699;op_p&#257;</span>", ">keiki ho&#699;op&#257;p&#257;</span>")
                .Replace(">ho&#699;o.ke._a.maka</span>", ">ho&#699;o.k&#275;.&#257;.maka</span>")
                .Replace("_a", "&#257;")
                .Replace(">Man_nele</span>", ">Man&#257;nele</span>")
                .Replace("Sweet_potato", "Sweet-potato")
                .Replace(">k&#363;_loulou</span>", ">k&#363;loulou</span>")
                .Replace(">-makap&#363;_ </span>", ">-makap&#363; </span>")
                .Replace(">m&#257;.nie.nie ma._hiki.hiki </span>", ">m&#257;.nie.nie m&#257;.hiki.hiki </span>")
                .Replace(">Aia &#257; k_iko&#699;o no n&#257; w&#257;wae ma &#699;&#333; &#257; ma&#699;ane &#699;i, &#257; laila na&#699;a</span>", ">Aia &#257; k&#299;ko&#699;o n&#257; w&#257;wae ma&#699;&#333; &#257; ma&#699;ane&#699;i, &#257; laila na&#699;a</span>")
                .Replace(">Ma kahi maika&#699;i e pa&#699;awela ana n_</span>", ">Ma kahi maika&#699;i e pa&#699;awela ana n&#333;</span>")
                .Replace(">p_u.&#699;ulu kaua </span>", ">p&#363;.&#699;ulu kaua </span>")
                .Replace(">K&#333; wai ka&#699;a k_el&#257;?</span>", ">K&#333; wai ka&#699;a k&#275;l&#257;?</span>")
                .Replace("A spindly banana . .", "A spindly banana &hellip;")
                // Missing definition number fixes
                .Replace("<p><span>n.</span> Name of a large valley on", "<p>1. <span>n.</span> Name of a large valley on")
                ;
            // Fix Nānā references
            s = Regex.Replace(s, @"N&#257;n&#257;;?</span> (\d)", @"N&#257;n&#257;.</span> $1");
            s = Regex.Replace(s, @"N&#257;n&#257;;? (\d)", @"N&#257;n&#257;. $1");
            return s;
        }

        protected override bool IsEntryNode(HtmlNode node)
        {
            Regex regex = IsEntryDivId ??= new Regex(@"(ENG\.)?\w\.\d");

            string id = node.Attributes["id"]?.DeEntitizeValue;

            return !string.IsNullOrWhiteSpace(id) && regex.IsMatch(id);
        }

        protected override string[] ParseEntryNode(HtmlNode node)
        {
            return node.ChildNodes.Where(c => c.Name == "span" || c.Name == "p").Select(n => n.OuterHtml.SingleLineNoTabs().NormalizeWhiteSpace()).ToArray();
        }

        protected override string FinalCleanValue(string value)
        {
            value = StringUtils.FixSentenceEnd(value);
            return StringUtils.FixSentenceSpacing(value);
        }

        protected override void AddAbbreviations(DictionaryBase dict)
        {
            dict.AddAbbreviation("And.", "Andrews dictionary, 1865; reference is given only if no evidence is available other than that in Andrews and Andrews-Parker (AP)");
            dict.AddAbbreviation("AP", "Andrews-Parker dictionary, 1922; reference is given only if no evidence is available other than that in Andrews (And.) and Andrews-Parker");
            dict.AddAbbreviation("Cap.", "beginning with a capital letter");
            dict.AddAbbreviation("caus/sim.", "causative/simulative", AbbreviationType.Grammatical);
            dict.AddAbbreviation("cf.", "compare", AbbreviationType.Auxiliary);
            dict.AddAbbreviation("conj.", "conjunction", AbbreviationType.Grammatical);
            dict.AddAbbreviation("demon.", "demonstrative", AbbreviationType.Grammatical);
            dict.AddAbbreviation("Eng.", "word borrowed from English");
            dict.AddAbbreviation("ex.", "example, examples", AbbreviationType.Auxiliary);
            dict.AddAbbreviation("f.", "form (in names of plants)");
            dict.AddAbbreviation("fig.", "figuratively");
            dict.AddAbbreviation("For.", "Fornander, Hawaiian Antiquities (For. 4:297 = Fornander Vol. 4, p. 297)");
            dict.AddAbbreviation("FS", "Elbert, Selections from Fornander");
            dict.AddAbbreviation("GP", "Green and Pukui, Legend of Kawelo");
            dict.AddAbbreviation("Gr.", "word probably borrowed from Greek");
            dict.AddAbbreviation("Gram.", "Elbert and Pukui, Hawaiian Grammar");
            dict.AddAbbreviation("Heb.", "word probably borrowed from Hebrew");
            dict.AddAbbreviation("HM", "Beckwith, Hawaiian Mythology");
            dict.AddAbbreviation("HP", "Handy, Hawaiian Planter");
            dict.AddAbbreviation("Ii", "Ii, Fragments of Hawaiian History");
            dict.AddAbbreviation("interr.", "interrogative", AbbreviationType.Grammatical);
            dict.AddAbbreviation("interj.", "interjection", AbbreviationType.Grammatical);
            dict.AddAbbreviation("Kam. 1964", "Kamakau, Ka Poʻe Kahiko");
            dict.AddAbbreviation("Kam. 1976", "Kamakau, The Works of the People of Old");
            dict.AddAbbreviation("Kel.", "Kelekona, Kaluaikoolau");
            dict.AddAbbreviation("Kep.", "Beckwith, Kepelino");
            dict.AddAbbreviation("KJV", "King James Version of the Bible");
            dict.AddAbbreviation("KL.", "Beckwith, Kumulipo");
            dict.AddAbbreviation("Laie", "Beckwith, Laieikawai");
            dict.AddAbbreviation("lit.", "literally");
            dict.AddAbbreviation("loc.n.", "locative noun", AbbreviationType.Grammatical);
            dict.AddAbbreviation("Malo", "Malo, Hawaiian Antiquities, 1951");
            dict.AddAbbreviation("MK", "Ke Alanui o ka Lani, Oia ka Manuale Kakolika");
            dict.AddAbbreviation("n.v.", "noun-verb", AbbreviationType.Grammatical);
            dict.AddAbbreviation("n.", "noun", AbbreviationType.Grammatical);
            dict.AddAbbreviation("Nak.", "Nakuina, Moolelo Hawaii ...");
            dict.AddAbbreviation("Nānā.", "Pukui, Haertig, Lee, Nānā i ke Kumu");
            dict.AddAbbreviation("Neal", "Neal, In Gardens of Hawaii, 1965");
            dict.AddAbbreviation("num.", "numeral", AbbreviationType.Grammatical);
            dict.AddAbbreviation("par.", "particle", AbbreviationType.Grammatical);
            dict.AddAbbreviation("pas/imp.", "passive/imperative", AbbreviationType.Grammatical);
            dict.AddAbbreviation("PH", "Emerson, Pele and Hiiaka");
            dict.AddAbbreviation("pl.", "plural", AbbreviationType.Grammatical);
            dict.AddAbbreviation("PCP", "Proto Central Polynesian");
            dict.AddAbbreviation("PEP", "Proto East Polynesian");
            dict.AddAbbreviation("PNP", "Proto Nuclear Polynesian");
            dict.AddAbbreviation("poss.", "possessive", AbbreviationType.Grammatical);
            dict.AddAbbreviation("PPN", "Proto Polynesian");
            dict.AddAbbreviation("prep.", "preposition", AbbreviationType.Grammatical);
            dict.AddAbbreviation("RC", "Kamakau, Ruling Chiefs");
            dict.AddAbbreviation("redup.", "reduplication (for meanings of reduplications, see Gram. 6.2.2)");
            dict.AddAbbreviation("RSV", "Holy Bible, Revised Standard Version");
            dict.AddAbbreviation("sg.", "singular", AbbreviationType.Grammatical);
            dict.AddAbbreviation("sp., spp.", "species");
            dict.AddAbbreviation("TC", "Taro Collection");
            dict.AddAbbreviation("UL", "Emerson, Unwritten Literature");
            dict.AddAbbreviation("v.", "verb", AbbreviationType.Grammatical);
            dict.AddAbbreviation("var.", "variant, variety");
            dict.AddAbbreviation("nvi.", "noun-intransitive verb", AbbreviationType.Grammatical);
            dict.AddAbbreviation("nvs.", "noun-stative verb", AbbreviationType.Grammatical);
            dict.AddAbbreviation("nvt.", "noun-transitive verb", AbbreviationType.Grammatical);
            dict.AddAbbreviation("vi.", "intransitive verb", AbbreviationType.Grammatical);
            dict.AddAbbreviation("vs.", "stative verb", AbbreviationType.Grammatical);
            dict.AddAbbreviation("vt.", "transitive verb", AbbreviationType.Grammatical);
            dict.AddAbbreviation("Am.", "Amosa (Amos)");
            dict.AddAbbreviation("Dan.", "Daniela (Daniel)");
            dict.AddAbbreviation("Epeso", "(Ephesians)");
            dict.AddAbbreviation("Eset.", "Esetera (Esther)");
            dict.AddAbbreviation("Ezek.", "Ezekiela (Ezekiel)");
            dict.AddAbbreviation("Ezera", "(Ezra)");
            dict.AddAbbreviation("Gal.", "Galatia (Galatians)");
            dict.AddAbbreviation("Hagai", "(Haggai)");
            dict.AddAbbreviation("Hal.", "Halelu (Psalms)");
            dict.AddAbbreviation("Heb.", "Hebera (Hebrews)");
            dict.AddAbbreviation("Hoik.", "Hoikeana (Revelation)");
            dict.AddAbbreviation("Hos.", "Hosea (Hosea)");
            dict.AddAbbreviation("Iak.", "Iakobo (James)");
            dict.AddAbbreviation("Ier.", "Ieremia (Jeremiah)");
            dict.AddAbbreviation("Ioane", "(John)");
            dict.AddAbbreviation("Ioba", "(Job)");
            dict.AddAbbreviation("Ioela", "(Joel)");
            dict.AddAbbreviation("Ios.", "Iosua (Joshua)");
            dict.AddAbbreviation("Isa.", "Isaia (Isaiah)");
            dict.AddAbbreviation("Iuda", "(Jude)");
            dict.AddAbbreviation("Kanl.", "Kanawailua (Deuteronomy)");
            dict.AddAbbreviation("Kekah.", "Kekahuna (Ecclesiastes)");
            dict.AddAbbreviation("Kin.", "Kinohi (Genesis)");
            dict.AddAbbreviation("Kol.", "Kolosa (Colosians)");
            dict.AddAbbreviation("Kor.", "Korineto (Corinthians)");
            dict.AddAbbreviation("Luka", "(Luke)");
            dict.AddAbbreviation("Lunk.", "Lunakanawai (Judges)");
            dict.AddAbbreviation("Mal.", "Malaki (Malachi)");
            dict.AddAbbreviation("Mar.", "Mareko (Mark)");
            dict.AddAbbreviation("Mat.", "Mataio (Matthew)");
            dict.AddAbbreviation("Mele", "Mele a Solomona (Songs of Solomon)");
            dict.AddAbbreviation("Mika", "(Micah)");
            dict.AddAbbreviation("Nah.", "Nahelu (Numbers)");
            dict.AddAbbreviation("Nal.", "Nalii (Kings)");
            dict.AddAbbreviation("Neh.", "Nehemia (Nehemia)");
            dict.AddAbbreviation("Oih.", "Oihana (Acts)");
            dict.AddAbbreviation("Oihk.", "Oihanakahuna (Leviticus)");
            dict.AddAbbreviation("Oihn.", "Oihanaalii (Chronicles)");
            dict.AddAbbreviation("Pet.", "Petero (Peter)");
            dict.AddAbbreviation("Pilipi", "(Philippians)");
            dict.AddAbbreviation("Puk.", "Pukaana (Exodus)");
            dict.AddAbbreviation("Roma", "(Romans)");
            dict.AddAbbreviation("Ruta", "(Ruth)");
            dict.AddAbbreviation("Sam.", "Samuela (Samuel)");
            dict.AddAbbreviation("Sol.", "Solomona (Proverbs)");
            dict.AddAbbreviation("Tes.", "Tesalonike (Thessalonians)");
            dict.AddAbbreviation("Tim.", "Timoteo (Timothy)");
            dict.AddAbbreviation("Tito", "(Titus)");
            dict.AddAbbreviation("Zek.", "Zekaria (Zechariah)");
            dict.AddAbbreviation("Zep.", "Zepania (Zephaniah)");
        }
    }
}
