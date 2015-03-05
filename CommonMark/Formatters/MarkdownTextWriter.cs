using System;
using System.Collections.Generic;
using System.Text;

namespace CommonMark.Formatters
{
    /// <summary>
    /// A wrapper for <see cref="MarkdownFormatter"/> that keeps track if the last symbol has been a newline.
    /// and prefixes lines with given string.
    /// </summary>
    internal sealed class MarkdownTextWriter
    {
        private System.IO.TextWriter _inner;
        private char _last = '\n';
        private bool _windowsNewLine;
        private char[] _newline;
        private char[] Buffer = new char[256];

        public string Prefix { get; set; }

        public MarkdownTextWriter(System.IO.TextWriter inner)
        {
            this._inner = inner;

            var nl = inner.NewLine;
            this._newline = nl.ToCharArray();
            this._windowsNewLine = nl == "\r\n";
        }

        private void EnsurePrefix(char next)
        {
            if (this._last == '\n' && next != '\r' && next != '\n')
            {
                this._inner.Write(this.Prefix);
                this._last = 'c';
            }
        }

        public void WriteLine()
        {
            this.EnsurePrefix('\n');

            this._inner.Write(this._newline);
            this._last = '\n';
        }

        public void WriteLine(char data)
        {
            this.EnsurePrefix(data);

            if (data == '\n' && this._windowsNewLine && this._last != '\r')
                this._inner.Write('\r');

            this._inner.Write(data);
            this._inner.Write(this._newline);
            this._last = '\n';
        }

        public void Write(Syntax.StringContent value)
        {
            var parts = value.RetrieveParts();
            for (var i = parts.Offset; i < parts.Offset + parts.Count; i++)
                this.Write(parts.Array[i]);
        }

        public void Write(Syntax.StringPart value)
        {
            if (value.Length == 0)
                return;

            if (this.Buffer.Length < value.Length)
                this.Buffer = new char[value.Length];

            value.Source.CopyTo(value.StartIndex, this.Buffer, 0, value.Length);

            var lastPos = value.StartIndex;
            var pos = lastPos;
            var lastC = this._last;

            while (-1 != (pos = value.Source.IndexOf('\n', pos, value.Length - pos + value.StartIndex)))
            {
                this.EnsurePrefix(value.Source[lastPos]);

                lastC = pos == 0 ? this._last : value.Source[pos - 1];

                this._inner.Write(this.Buffer, lastPos - value.StartIndex, pos - lastPos);
                lastPos = pos;

                if (this._windowsNewLine && lastC != '\r')
                    this._inner.Write('\r');

                pos++;
            }

            this.EnsurePrefix(value.Source[lastPos]);
            this._inner.Write(this.Buffer, lastPos - value.StartIndex, value.Length - lastPos + value.StartIndex);

            this._last = this.Buffer[value.Length - 1];
        }

        /// <summary>
        /// Writes a value that is known not to contain any newlines.
        /// </summary>
        public void WriteConstant(char[] value)
        {
            this.EnsurePrefix('c');

            this._last = 'c';
            this._inner.Write(value, 0, value.Length);
        }

        /// <summary>
        /// Writes a value that is known not to contain any newlines.
        /// </summary>
        public void WriteConstant(char[] value, int startIndex, int length)
        {
            this.EnsurePrefix('c');

            this._last = 'c';
            this._inner.Write(value, startIndex, length);
        }

        /// <summary>
        /// Writes a value that is known not to contain any newlines.
        /// </summary>
        public void WriteConstant(string value)
        {
            this.EnsurePrefix('c');

            this._last = 'c';
            this._inner.Write(value);
        }

        /// <summary>
        /// Writes a value that is known not to contain any newlines.
        /// </summary>
        public void WriteLineConstant(string value)
        {
            this.EnsurePrefix('c');

            this._last = '\n';
            this._inner.Write(value);
            this._inner.Write(this._newline);
        }

        public void Write(char[] value, int index, int count)
        {
            if (value == null || count == 0)
                return;

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

                this.EnsurePrefix(value[lastPos]);

                lastC = pos == index ? this._last : value[pos - 1];

                this._inner.Write(value, lastPos, pos - lastPos);
                lastPos = pos;

                if (this._windowsNewLine && lastC != '\r')
                    this._inner.Write('\r');

                pos++;
            }

            this.EnsurePrefix(value[lastPos]);
            this._inner.Write(value, lastPos, index + count - lastPos);

            this._last = value[index + count - 1];
        }

        public void Write(char value)
        {
            this.EnsurePrefix(value);

            if (value == '\n' && this._windowsNewLine && this._last != '\r')
                this._inner.Write('\r');

            this._last = value;
            this._inner.Write(value);
        }

        /// <summary>
        /// Adds a newline if the writer does not currently end with a newline.
        /// </summary>
        public void EnsureLine()
        {
            if (this._last != '\n')
                this.WriteLine();
        }
    }
}
