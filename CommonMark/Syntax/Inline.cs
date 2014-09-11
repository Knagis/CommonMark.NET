using System;
using System.Collections.Generic;
using System.Text;

namespace CommonMark.Syntax
{
    public class Inline
    {
        public InlineTag Tag { get; set; }

        private readonly InlineContent _content = new InlineContent();
        public InlineContent Content { get { return this._content; } }

        public Inline Next { get; set; }
    }
}
