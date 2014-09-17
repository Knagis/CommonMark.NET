using System;
using System.Collections.Generic;
using System.Text;

namespace CommonMark.Syntax
{
    public sealed class Inline
    {
        public InlineTag Tag { get; set; }

        private readonly InlineContent _content = new InlineContent();
        public InlineContent Content { get { return this._content; } }

        private Inline _next;

        /// <summary>
        /// Gets the next sibling inline.
        /// </summary>
        public Inline Next 
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
