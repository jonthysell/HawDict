// Copyright (c) Jon Thysell <http://jonthysell.com>
// Licensed under the MIT License.

using System;
using System.Collections.Generic;

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

        public static string FileVersion => AppInfo.Version;

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
