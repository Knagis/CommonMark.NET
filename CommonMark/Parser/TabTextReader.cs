using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CommonMark.Parser
{
    internal class TabTextReader
    {
        private const int _bufferSize = 4000;
        private readonly TextReader _inner;
        private readonly char[] _buffer;
        private int _bufferLength;
        private int _bufferPosition;
        private readonly StringBuilder _builder;
        private bool _endOfStream;

        public TabTextReader(TextReader inner)
        {
            this._inner = inner;
            this._buffer = new char[_bufferSize];
            this._builder = new StringBuilder(256);
        }

        public bool ReadBuffer()
        {
            if (this._endOfStream)
                return false;

            this._bufferLength = this._inner.Read(this._buffer, 0, _bufferSize);
            this._endOfStream = this._bufferLength == 0;
            this._bufferPosition = 0;
            return !this._endOfStream;
        }

        public string ReadLine()
        {
            if (this._bufferPosition == this._bufferLength && !this.ReadBuffer())
                return null;

            bool useBuilder = false;
            int num;
            char c;
            while (true)
            {
                num = this._bufferPosition;
                do
                {
                    c = this._buffer[num];
                    if (c == '\r' || c == '\n')
                    {
                        goto IL_4A;
                    }
                    if (c == '\t')
                    {
                        if (!useBuilder)
                        {
                            useBuilder = true;
                            this._builder.Length = 0;
                        }

                        this._builder.Append(this._buffer, this._bufferPosition, num - this._bufferPosition);
                        this._builder.Append(' ', 4 - (this._builder.Length % 4));
                        this._bufferPosition = num + 1;
                    }

                    num++;
                }
                while (num < this._bufferLength);

                num = this._bufferLength - this._bufferPosition;
                if (!useBuilder)
                {
                    useBuilder = true;
                    this._builder.Length = 0;
                }

                this._builder.Append(this._buffer, this._bufferPosition, num);
                if (!this.ReadBuffer())
                {
                    this._builder.Append('\n');
                    return this._builder.ToString();
                }
            }

        IL_4A:
            string result;
            this._buffer[num] = '\n';
            if (useBuilder)
            {
                this._builder.Append(this._buffer, this._bufferPosition, num - this._bufferPosition + 1);
                result = this._builder.ToString();
            }
            else
            {
                result = new string(this._buffer, this._bufferPosition, num - this._bufferPosition + 1);
            }
            this._bufferPosition = num + 1;
            if (c == '\r' && (this._bufferPosition < this._bufferLength || this.ReadBuffer()) && this._buffer[this._bufferPosition] == '\n')
            {
                this._bufferPosition++;
            }
            return result;

        }

        public bool EndOfStream()
        {
            return this._endOfStream || (this._bufferPosition == this._bufferLength && !this.ReadBuffer());
        }
    }
}
