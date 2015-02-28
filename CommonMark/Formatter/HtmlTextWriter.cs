﻿using System;
using System.Collections.Generic;
using System.Text;

namespace CommonMark.Formatter
{
    /// <summary>
    /// A wrapper for <see cref="HtmlBlockWriter"/> that keeps track if the last symbol has been a newline.
    /// </summary>
    internal sealed class HtmlTextWriter
    {
        private System.IO.TextWriter _inner;
        private char _last = '\n';
        private bool _windowsNewLine;
        private char[] _newline;
        
        /// <summary>
        /// A reusable char buffer. This is used internally in <see cref="Write(Syntax.StringPart)"/> (and thus will modify the buffer)
        /// but can also be used from <see cref="HtmlBlockWriter"/> class.
        /// </summary>
        internal char[] Buffer = new char[256];

        public HtmlTextWriter(System.IO.TextWriter inner)
        {
            this._inner = inner;

            var nl = inner.NewLine;
            this._newline = nl.ToCharArray();
            this._windowsNewLine = nl == "\r\n";
        }

        public void WriteLine()
        {
            this._inner.Write(this._newline);
            this._last = '\n';
        }

        public void WriteLine(char data)
        {
            if (data == '\n' && this._windowsNewLine && this._last != '\r')
                this._inner.Write('\r');

            this._inner.Write(data);
            this._inner.Write(this._newline);
            this._last = '\n';
        }

        public void Write(Syntax.StringPart value)
        {
            if (value.Length == 0)
                return;

            if (this.Buffer.Length < value.Length)
                this.Buffer = new char[value.Length];

            value.Source.CopyTo(value.StartIndex, this.Buffer, 0, value.Length);

            if (this._windowsNewLine)
            {
                var lastPos = value.StartIndex;
                var pos = lastPos;
                var lastC = this._last;

                while (-1 != (pos = value.Source.IndexOf('\n', pos, value.Length - pos + value.StartIndex)))
                {
                    lastC = pos == 0 ? this._last : value.Source[pos - 1];

                    if (lastC != '\r')
                    {
                        this._inner.Write(this.Buffer, lastPos - value.StartIndex, pos - lastPos);
                        this._inner.Write('\r');
                        lastPos = pos;
                    }

                    pos++;
                }

                this._inner.Write(this.Buffer, lastPos - value.StartIndex, value.Length - lastPos + value.StartIndex);
            }
            else
            {
                this._inner.Write(this.Buffer, 0, value.Length);
            }

            this._last = this.Buffer[value.Length - 1];
        }
        
        /// <summary>
        /// Writes a value that is known not to contain any newlines.
        /// </summary>
        public void WriteConstant(char[] value)
        {
            this._last = 'c';
            this._inner.Write(value, 0, value.Length);
        }

        /// <summary>
        /// Writes a value that is known not to contain any newlines.
        /// </summary>
        public void WriteConstant(char[] value, int startIndex, int length)
        {
            this._last = 'c';
            this._inner.Write(value, startIndex, length);
        }

        /// <summary>
        /// Writes a value that is known not to contain any newlines.
        /// </summary>
        public void WriteConstant(string value)
        {
            this._last = 'c';
            this._inner.Write(value);
        }

        /// <summary>
        /// Writes a value that is known not to contain any newlines.
        /// </summary>
        public void WriteLineConstant(string value)
        {
            this._last = '\n';
            this._inner.Write(value);
            this._inner.Write(this._newline);
        }

        public void Write(char[] value, int index, int count)
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

        public void Write(char value)
        {
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
