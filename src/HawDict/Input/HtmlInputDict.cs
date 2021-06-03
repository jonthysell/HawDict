// Copyright (c) Jon Thysell <http://jonthysell.com>
// Licensed under the MIT License.

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
            string htmlFile = Path.Combine(DictDir, string.Format("{0}.{1}.html.tmp", ID, TranslationType.ToString()));

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
