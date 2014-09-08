using System;
using System.Collections.Generic;
using System.Text;

namespace CommonMark.Syntax
{
    public class Inline
    {
        public InlineTag tag;
        public InlineContent content = new InlineContent();
        public Inline next;
    }
}
