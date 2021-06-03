// Copyright (c) Jon Thysell <http://jonthysell.com>
// Licensed under the MIT License.

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
