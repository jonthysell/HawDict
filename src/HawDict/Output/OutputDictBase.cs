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

namespace HawDict
{
    public abstract class OutputDictBase
    {
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
                DateTime buildTime = CreationDateTime;
                int major = 0;
                int minor = 9;
                int build = (major >= 1 && minor % 2 == 1) ? (1000 * (buildTime.Year % 100)) + buildTime.DayOfYear : 0;
                int revision = (major >= 1 && minor % 2 == 1) ? (100 * buildTime.Hour) + buildTime.Minute : 0;

                return string.Format("{0}.{1}.{2:0000}.{3:0000}", major, minor, build, revision);
            }
        }

        #endregion

        public List<OutputArticle> Articles { get; private set; } = new List<OutputArticle>();

        public List<OutputAbbreviation> Abbreviations { get; private set; } = new List<OutputAbbreviation>();

        public OutputDictBase(string formatType, TranslationType translationType)
        {
            FormatType = !string.IsNullOrWhiteSpace(formatType) ? formatType : throw new ArgumentNullException(nameof(formatType));
            TranslationType = translationType;
        }

        public abstract void Save(string dictDir);
    }
}
