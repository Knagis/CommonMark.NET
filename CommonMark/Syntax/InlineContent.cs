using System;
using System.Collections.Generic;
using System.Text;

namespace CommonMark.Syntax
{
    public sealed class InlineContent
    {
        public string Literal { get; set; }

        public Inline Inlines { get; set; }

        private readonly InlineContentLinkable _linkable = new InlineContentLinkable();
        public InlineContentLinkable Linkable { get { return this._linkable; } }
    }
}
