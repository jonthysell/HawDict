// 
// HtmlInputDict.cs
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

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using HtmlAgilityPack;

namespace HawDict
{
    public abstract class HtmlInputDict : InputDictBase
    {
        public string EntryHtmlTag { get; protected set; } = null;

        private List<KeyValuePair<string, string>> _cleanedEntries = null;

        public HtmlInputDict(string id, TranslationType translationType, LogLine logLine) : base(id, translationType, logLine) { }

        protected override void GetRawDataFromSource()
        {
            string htmlFile = Path.Combine(DictDir, string.Format("{0}.html.tmp", TranslationType.ToString()));

            string html = "";

            if (File.Exists(htmlFile))
            {
                Log("HTML file already exists.");
            }
            else
            {
                Log("Downloading HTML from source.");

                Task<string> task = GetRawHtmlFromSourceAsync();
                task.Wait();

                Log("Saving HTML file.");
                File.WriteAllText(htmlFile, task.Result, Encoding.UTF8);
            }

            Log("Loading HTML file.");
            html = File.ReadAllText(htmlFile, Encoding.UTF8);

            Log("Cleaning HTML.");
            html = CleanSourceHtml(html);

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);

            _rawData.AddRange(GetRawDataFromHtml(doc));
        }

        private async Task<string> GetRawHtmlFromSourceAsync()
        {
            HttpWebRequest request = WebRequest.CreateHttp(SrcUrl);

            HttpWebResponse response = (HttpWebResponse)(await request.GetResponseAsync());

            string html = "";

            using (StreamReader sr = new StreamReader(response.GetResponseStream()))
            {
                html = await sr.ReadToEndAsync();
            }

            return html;
        }

        private IEnumerable<string[]> GetRawDataFromHtml(HtmlDocument doc)
        {
            foreach (HtmlNode entryNode in doc.DocumentNode.Descendants(EntryHtmlTag).Where(n => IsEntryNode(n)))
            {
                yield return ParseEntryNode(entryNode);
            }
        }

        protected override IEnumerable<KeyValuePair<string, string>> GetCleanEntries()
        {
            if (null == _cleanedEntries)
            {
                CleanRawEntries();
            }

            return _cleanedEntries;
        }

        private void CleanRawEntries()
        {
            _cleanedEntries = new List<KeyValuePair<string, string>>();

            foreach (string[] entry in _rawData)
            {
                string key = StringUtils.NormalizeWhiteSpace(StringUtils.HtmlToUtf8(entry[0]));
                string value = string.Join(" ", entry.Skip(1).Select(s => StringUtils.NormalizeWhiteSpace(StringUtils.HtmlToUtf8(s))));

                _cleanedEntries.Add(new KeyValuePair<string, string>(key, value));
            }
        }

        protected abstract string CleanSourceHtml(string html);

        protected abstract bool IsEntryNode(HtmlNode node);

        protected abstract string[] ParseEntryNode(HtmlNode node);
    }
}
