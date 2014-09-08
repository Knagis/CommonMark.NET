using System;
using System.Collections.Generic;
using System.Text;

namespace CommonMark.Syntax
{
    public class InlineContent
    {
        public string Literal;
        public Inline inlines;
        public InlineContentLinkable linkable = new InlineContentLinkable();
    }
}
