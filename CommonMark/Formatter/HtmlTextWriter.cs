using System;
using System.Collections.Generic;
using System.Text;

namespace CommonMark.Formatter
{
    /// <summary>
    /// A wrapper for <see cref="HtmlPrinter"/> that keeps track if the last symbol has been a newline.
    /// </summary>
    internal class HtmlTextWriter : System.IO.TextWriter
    {
        private System.IO.TextWriter _inner;
        private char _last = '\n';
        private bool _windowsNewLine;

        public HtmlTextWriter(System.IO.TextWriter inner)
        {
            this._inner = inner;

            var nl = inner.NewLine;
            this.CoreNewLine = nl.ToCharArray();
            this._windowsNewLine = nl == "\r\n";
        }

        public override void Write(char value)
        {
            if (this._windowsNewLine && _last != '\r' && value == '\n')
                this._inner.Write('\r');

            this._last = value;
            this._inner.Write(value);
        }

        public bool EndsWithNewline { get { return this._last == '\n'; } }

        public override Encoding Encoding
        {
            get { return Encoding.UTF8; }
        }
    }
}
