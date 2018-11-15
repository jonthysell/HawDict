// 
// StarDictDictionary.cs
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
using System.Linq;
using System.Text;

namespace HawDict
{
    public class StarDictDictionary : OutputDictBase
    {
        private static StarDictArticleComparer _keyComparer = new StarDictArticleComparer();

        public StarDictDictionary(string id, TranslationType translationType) : base(id, "StarDict", translationType) { }

        public override void Save(string dictDir)
        {
            if (string.IsNullOrWhiteSpace(dictDir))
            {
                throw new ArgumentNullException(nameof(dictDir));
            }

            long idxFileSize;
            int synWordCount;
            SaveDataFiles(dictDir, out idxFileSize, out synWordCount);

            SaveIfoFile(dictDir, idxFileSize, synWordCount);
        }

        private void SaveDataFiles(string dictDir, out long idxFileSize, out int synWordCount)
        {
            string dictFile = Path.Combine(dictDir, string.Format("{0}.{1}.StarDict.dict", ID, TranslationType.ToString()));
            string idxFile = Path.Combine(dictDir, string.Format("{0}.{1}.StarDict.idx", ID, TranslationType.ToString()));
            string synFile = Path.Combine(dictDir, string.Format("{0}.{1}.StarDict.syn", ID, TranslationType.ToString()));

            BinaryWriter dictWriter = new BinaryWriter(new FileStream(dictFile, FileMode.Create), Encoding.UTF8);
            BinaryWriter idxWriter = new BinaryWriter(new FileStream(idxFile, FileMode.Create), Encoding.UTF8);
            BinaryWriter synWriter = new BinaryWriter(new FileStream(synFile, FileMode.Create), Encoding.UTF8);

            Dictionary<OutputArticle, uint> articleIndexes = new Dictionary<OutputArticle, uint>();

            uint index = 0;
            foreach (OutputArticle article in Articles.OrderBy(a => a.StarDictKey, _keyComparer))
            {
                long dictArticleOffset = dictWriter.BaseStream.Length;
                
                idxWriter.Write(article.StarDictKey.ToCharArray());
                idxWriter.Write('\0');

                dictWriter.Write(article.StarDictValue.ToCharArray());

                dictWriter.Flush();

                long dictArticleLength = dictWriter.BaseStream.Length - dictArticleOffset;

                WriteBigEndian(idxWriter, (uint)dictArticleOffset);
                WriteBigEndian(idxWriter, (uint)dictArticleLength);

                idxWriter.Flush();

                articleIndexes[article] = index;
                index++;
            }

            dictWriter.Flush();
            dictWriter.Close();

            idxWriter.Flush();
            idxFileSize = idxWriter.BaseStream.Length;
            idxWriter.Close();

            List<KeyValuePair<string, uint>> synonyms = new List<KeyValuePair<string, uint>>();

            foreach (KeyValuePair<OutputArticle, uint> articleIndex in articleIndexes)
            {
                uint keyIndex = articleIndex.Value;
                foreach (string synonym in articleIndex.Key.StarDictKeySynonyms)
                {
                    synonyms.Add(new KeyValuePair<string, uint>(synonym, keyIndex));
                }
            }

            foreach (KeyValuePair<string, uint> synonym in synonyms.OrderBy(kvp => kvp.Key, _keyComparer))
            {
                synWriter.Write(synonym.Key.ToCharArray());
                synWriter.Write('\0');

                WriteBigEndian(synWriter, synonym.Value);
            }

            synWordCount = synonyms.Count;

            synWriter.Flush();
            synWriter.Close();
        }

        private void SaveIfoFile(string dictDir, long idxFileSize, int synWordCount)
        {
            string ifoFile = Path.Combine(dictDir, string.Format("{0}.{1}.StarDict.ifo", ID, TranslationType.ToString()));

            using (BinaryWriter ifoWriter = new BinaryWriter(new FileStream(ifoFile, FileMode.Create), Encoding.UTF8))
            {
                WriteLine(ifoWriter, "StarDict's dict ifo file");
                WriteLine(ifoWriter, "version=2.4.2");

                WriteLine(ifoWriter, "bookname={0}", Title);
                WriteLine(ifoWriter, "wordcount={0}", Articles.Count);
                WriteLine(ifoWriter, "synwordcount={0}", synWordCount);
                WriteLine(ifoWriter, "idxfilesize={0}", idxFileSize);
                WriteLine(ifoWriter, "sametypesequence=h");

                WriteLine(ifoWriter, "author={0}", string.Join(", ", Authors));
                WriteLine(ifoWriter, "description={0}", Description);
                WriteLine(ifoWriter, "date={0}", CreationDateTime.ToString("yyyy.MM.dd"));
            }
        }

        private static void WriteLine(BinaryWriter bw, string line, params object[] args)
        {
            bw.Write(string.Format(line, args).ToCharArray());
            bw.Write('\r');
            bw.Write('\n');
        }

        private static void WriteBigEndian(BinaryWriter bw, uint value)
        {
            byte[] bytes = BitConverter.GetBytes(value);

            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }

            bw.Write(bytes);
        }

        private class StarDictArticleComparer : IComparer<string>
        {
            public int Compare(string x, string y)
            {
                int result = AsciiStrCmp(x, y);
                return result == 0 ? StrCmp(x, y) : result;
            }

            private static int AsciiStrCmp(string x, string y)
            {
                int[] bx = Encoding.UTF8.GetBytes(x).Select(b => (int)(b)).ToArray();
                int[] by = Encoding.UTF8.GetBytes(y).Select(b => (int)(b)).ToArray();

                int minLength = Math.Min(bx.Length, by.Length);

                for (int i = 0; i < minLength; i++)
                {
                    int cx = AsciiLower(bx[i]);
                    int cy = AsciiLower(by[i]);

                    if (cx != cy)
                    {
                        return cx - cy;
                    }
                }

                return bx.Length - by.Length;
            }

            private static int AsciiLower(int c)
            {
                if (c >= 'A' && c <= 'Z')
                {
                    return (c - 'A' + 'a');
                }
                return c;
            }

            private static int StrCmp(string x, string y)
            {
                int[] bx = Encoding.UTF8.GetBytes(x).Select(b => (int)(b)).ToArray();
                int[] by = Encoding.UTF8.GetBytes(y).Select(b => (int)(b)).ToArray();

                int minLength = Math.Min(bx.Length, by.Length);

                for (int i = 0; i < minLength; i++)
                {
                    int cx = bx[i];
                    int cy = by[i];

                    if (cx != cy)
                    {
                        return cx - cy;
                    }
                }

                return bx.Length - by.Length;
            }
        }
    }
}