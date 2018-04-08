// 
// OutputAbbreviation.cs
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

namespace HawDict
{
    public class OutputAbbreviation
    {
        public OutputDictBase OutputDict { get; private set; }

        public string Key { get; private set; } = null;
        public string Value { get; private set; } = null;

        public AbbreviationType AbbreviationType { get; private set; } = AbbreviationType.None;

        public string XdxfKey
        {
            get
            {
                string key = StringUtils.EscapeForXml(Key);

                key = StringUtils.WrapInTag(Key, "abbr_k");

                if (char.IsLower(Key[0]) && Key.Length > 1)
                {
                    key += StringUtils.WrapInTag(char.ToUpper(Key[0]) + Key.Substring(1), "abbr_k");
                }

                return key;
            }
        }

        public string XdxfValue
        {
            get
            {
                string value = StringUtils.EscapeForXml(Value);

                value = StringUtils.WrapInTag(Value, "abbr_v");

                return value;
            }
        }

        public OutputAbbreviation(OutputDictBase dict, string key, string value, AbbreviationType abbreviationType = AbbreviationType.None)
        {
            OutputDict = dict ?? throw new ArgumentNullException(nameof(dict));

            Key = !string.IsNullOrWhiteSpace(key) ? key.Trim() : throw new ArgumentNullException(nameof(key));
            Value = !string.IsNullOrWhiteSpace(value) ? value.Trim() : throw new ArgumentNullException(nameof(value));

            AbbreviationType = abbreviationType;
        }
    }

    public enum AbbreviationType
    {
        None,
        Grammatical,
        Stylistic,
        Knowledge,
        Auxiliary,
        Other
    }
}
