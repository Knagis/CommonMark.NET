using System;
using System.Collections.Generic;
using System.Text;

namespace CommonMark.Syntax
{
    public class InlineContentLinkable
    {
        public Inline Label { get; set; }
        public string Url { get; set; }
        public string Title { get; set; }
    }
}
