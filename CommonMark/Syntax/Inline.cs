using System;
using System.Collections.Generic;
using System.Text;

namespace CommonMark.Syntax
{
    public sealed class Inline
    {
        public InlineTag Tag { get; set; }

        public string LiteralContent { get; set; }

        public Inline FirstChild { get; set; }

        private readonly InlineContentLinkable _linkable = new InlineContentLinkable();
        public InlineContentLinkable Linkable { get { return this._linkable; } }

        private Inline _next;

        /// <summary>
        /// Gets the next sibling inline.
        /// </summary>
        public Inline NextSibling 
        { 
            get 
            { 
                return this._next; 
            } 
            set 
            { 
                this._next = value;
            } 
        }

        /// <summary>
        /// Gets the last sibling of this inline. If no siblings are defined, returns self.
        /// </summary>
        public Inline LastSibling
        {
            get
            {
                var x = this._next;
                if (x == null)
                    return this;

                while (x._next != null)
                    x = x._next;

                return x;
            }
        }
    }
}
