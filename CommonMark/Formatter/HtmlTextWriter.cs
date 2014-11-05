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
        private char[] _buffer = new char[256];

        public HtmlTextWriter(System.IO.TextWriter inner)
        {
            this._inner = inner;

            var nl = inner.NewLine;
            this.CoreNewLine = nl.ToCharArray();
            this._windowsNewLine = nl == "\r\n";
        }

        public override void WriteLine(string value)
        {
            this.Write(value);
            this._inner.Write(this.CoreNewLine);
            this._last = '\n';
        }

        public override void Write(string value)
        {
            if (value == null || value.Length == 0)
                return;

            if (this._windowsNewLine)
            {
                var lastPos = 0;
                var lastC = this._last;
                int pos = 0;

                if (this._buffer.Length < value.Length)
                    this._buffer = value.ToCharArray();
                else
                    value.CopyTo(0, this._buffer, 0, value.Length);

                while (-1 != (pos = value.IndexOf('\n', pos)))
                {
                    lastC = pos == 0 ? this._last : value[pos - 1];

                    if (lastC != '\r')
                    {
                        this._inner.Write(this._buffer, lastPos, pos - lastPos);
                        this._inner.Write('\r');
                        lastPos = pos;
                    }

                    pos++;
                }

                this._inner.Write(this._buffer, lastPos, value.Length - lastPos);
            }
            else
            {
                this._inner.Write(value);
            }

            this._last = value[value.Length - 1];
        }

        public override void Write(char[] value, int index, int count)
        {
            if (value == null || count == 0)
                return;

            if (this._windowsNewLine)
            {
                var lastPos = index;
                var lastC = this._last;
                int pos = index;

                while (pos < index + count)
                {
                    if (value[pos] != '\n')
                    {
                        pos++;
                        continue;
                    }

                    lastC = pos == index ? this._last : value[pos - 1];

                    if (lastC != '\r')
                    {
                        this._inner.Write(value, lastPos, pos - lastPos);
                        this._inner.Write('\r');
                        lastPos = pos;
                    }

                    pos++;
                }

                this._inner.Write(value, lastPos, index + count - lastPos);
            }
            else
            {
                this._inner.Write(value, index, count);
            }

            this._last = value[index + count - 1];
        }

        public override void Write(char value)
        {
            if (this._windowsNewLine && _last != '\r' && value == '\n')
                this._inner.Write('\r');

            this._last = value;
            this._inner.Write(value);
        }

        public override Encoding Encoding
        {
            get { return Encoding.UTF8; }
        }

        /// <summary>
        /// Adds a newline if the writer does not currently end with a newline.
        /// </summary>
        public virtual void EnsureLine()
        {
            if (this._last != '\n')
                this.WriteLine();
        }
    }
}
