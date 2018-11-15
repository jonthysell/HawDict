// 
// OutputDictBase.cs
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
using System.Reflection;

namespace HawDict
{
    public abstract class OutputDictBase
    {
        public string ID { get; private set; }

        public string FormatType { get; private set; }

        public TranslationType TranslationType { get; private set; }

        #region MetaData

        public string Title { get; set; } = null;

        public string Description { get; set; } = null;

        public DateTime CreationDateTime { get; private set; } = DateTime.UtcNow;

        public List<string> Authors { get; private set; } = new List<string>();

        public string FileVersion
        {
            get
            {
                AssemblyName name = Assembly.GetEntryAssembly().GetName();
                return name.Version.ToString();
            }
        }

        #endregion

        public List<OutputArticle> Articles { get; private set; } = new List<OutputArticle>();

        public List<OutputAbbreviation> Abbreviations { get; private set; } = new List<OutputAbbreviation>();

        public OutputDictBase(string id, string formatType, TranslationType translationType)
        {
            ID = !string.IsNullOrWhiteSpace(id) ? id : throw new ArgumentNullException(nameof(id));
            FormatType = !string.IsNullOrWhiteSpace(formatType) ? formatType : throw new ArgumentNullException(nameof(formatType));
            TranslationType = translationType;
        }

        public abstract void Save(string dictDir);
    }
}
