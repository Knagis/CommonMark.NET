using System;
using System.Collections.Generic;
using System.Text;

namespace CommonMark.Syntax
{
    public sealed class Inline
    {
        public Inline()
        {
        }

        public Inline(InlineTag tag)
        {
            this.Tag = tag;
        }

        public Inline(InlineTag tag, string content)
        {
            this.Tag = tag;
            this.LiteralContent = content;
        }

        public Inline(string content)
        {
            // this is not assigned because it is the default value.
            ////this.Tag = InlineTag.String;

            this.LiteralContent = content;
        }

        public Inline(InlineTag tag, Inline content)
        {
            this.Tag = tag;
            this.FirstChild = content;
        }

        internal static Inline CreateLink(Inline label, string url, string title)
        {
            var i = new Inline();
            i.Tag = InlineTag.Link;
            i.FirstChild = label;
            i._linkable.Url = url;
            i._linkable.Title = title;
            return i;
        }

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
