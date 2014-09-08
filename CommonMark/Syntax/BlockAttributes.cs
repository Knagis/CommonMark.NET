using System;
using System.Collections.Generic;
using System.Text;

namespace CommonMark.Syntax
{
    public class BlockAttributes
    {
        public ListData list_data = new ListData();
        public FencedCodeData fenced_code_data = new FencedCodeData();
        public int header_level;

        public Dictionary<string, Reference> refmap;
    }
}
