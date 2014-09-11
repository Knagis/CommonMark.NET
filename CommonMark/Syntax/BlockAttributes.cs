using System;
using System.Collections.Generic;
using System.Text;

namespace CommonMark.Syntax
{
    public class BlockAttributes
    {
        private ListData _listData = new ListData();
        /// <remarks>Original: list_data</remarks>
        public ListData ListData { get { return this._listData; } set { this._listData = value; } }

        private readonly FencedCodeData _fencedCodeData = new FencedCodeData();
        /// <remarks>Original: fenced_code_data</remarks>
        public FencedCodeData FencedCodeData { get { return this._fencedCodeData; } }

        /// <remarks>Original: header_level</remarks>
        public int HeaderLevel { get; set; }

        /// <remarks>Original: refmap</remarks>
        public Dictionary<string, Reference> ReferenceMap { get; set; }
    }
}
