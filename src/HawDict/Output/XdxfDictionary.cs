// 
// XdxfDictionary.cs
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
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace HawDict
{
    public class XdxfDictionary : OutputDictBase
    {
        #region MetaData

        public string FullTitle { get; set; } = null;

        public string SrcUrl { get; set; } = null;

        #endregion

        public XdxfDictionary(string id, TranslationType translationType) : base(id, "XDXF", translationType) { }

        public override void Save(string dictDir)
        {
            if (string.IsNullOrWhiteSpace(dictDir))
            {
                throw new ArgumentNullException(nameof(dictDir));
            }

            string xdxfFile = Path.Combine(dictDir, string.Format("{0}.{1}.dict.xdxf", ID, TranslationType.ToString()));

            using (FileStream fs = new FileStream(xdxfFile, FileMode.Create))
            {
                SaveDictFile(fs);
            }
        }

        private void SaveDictFile(Stream output)
        {
            // Write to StringBuilder
            StringBuilder sb = new StringBuilder();
            using (XmlWriter xw = XmlWriter.Create(sb, new XmlWriterSettings() { Encoding = Encoding.UTF8, CloseOutput = false }))
            {
                xw.WriteStartDocument();

                xw.WriteStartElement("xdxf");

                xw.WriteAttributeString("format", "logical");
                xw.WriteAttributeString("revision", "33");
                xw.WriteAttributeString("lang_from", TranslationType == TranslationType.HawToEng ? "HAW" : "ENG");
                xw.WriteAttributeString("lang_to", TranslationType == TranslationType.HawToEng ? "ENG" : "HAW");

                WriteMetaInfoElements(xw);

                WriteArticles(xw);

                xw.WriteEndElement(); // xdxf

                xw.WriteEndDocument();
            }

            // Load from StringBuilder
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(sb.ToString());

            // Write to stream
            using (XmlWriter xw = XmlWriter.Create(output, new XmlWriterSettings() { Encoding = Encoding.UTF8, Indent = true, CloseOutput = false }))
            {
                doc.Save(xw);
            }
        }

        private void WriteMetaInfoElements(XmlWriter xw)
        {
            xw.WriteStartElement("meta_info");

            xw.WriteElementString("title", Title);

            xw.WriteElementString("full_title", FullTitle);

            if (Authors.Count > 0)
            {
                xw.WriteStartElement("authors");

                foreach (string author in Authors)
                {
                    WriteElementStringIfNotNull(xw, "author", author);
                }

                xw.WriteEndElement(); // authors
            }

            xw.WriteElementString("description", Description);

            if (Abbreviations.Count > 0)
            {
                xw.WriteStartElement("abbreviations");

                foreach (OutputAbbreviation abbreviation in Abbreviations)
                {
                    xw.WriteStartElement("abbr_def");

                    switch (abbreviation.AbbreviationType)
                    {
                        case AbbreviationType.Grammatical:
                            xw.WriteAttributeString("type", "grm");
                            break;
                        case AbbreviationType.Stylistic:
                            xw.WriteAttributeString("type", "stl");
                            break;
                        case AbbreviationType.Knowledge:
                            xw.WriteAttributeString("type", "knl");
                            break;
                        case AbbreviationType.Auxiliary:
                            xw.WriteAttributeString("type", "aux");
                            break;
                        case AbbreviationType.Other:
                            xw.WriteAttributeString("type", "oth");
                            break;
                    }

                    xw.WriteRaw(abbreviation.XdxfKey);
                    xw.WriteRaw(abbreviation.XdxfValue);

                    xw.WriteEndElement(); // abbr_def
                }

                xw.WriteEndElement(); // abbreviations
            }

            xw.WriteElementString("file_ver", FileVersion);

            xw.WriteElementString("creation_date", CreationDateTime.Date.ToString("dd-MM-yyyy"));

            WriteElementStringIfNotNull(xw, "dict_src_url", SrcUrl);

            xw.WriteEndElement(); // meta_info
        }

        private void WriteArticles(XmlWriter xw)
        {
            if (Articles.Count > 0)
            {
                xw.WriteStartElement("lexicon");

                foreach (OutputArticle article in Articles)
                {
                    xw.WriteStartElement("ar");

                    xw.WriteRaw(article.XdxfKey);
                    xw.WriteRaw(article.XdxfValue);

                    xw.WriteEndElement(); // ar
                }

                xw.WriteEndElement(); // lexicon
            }
        }

        private void WriteElementStringIfNotNull(XmlWriter xw, string localName, string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                xw.WriteElementString(localName, value);
            }
        }
    }
}
