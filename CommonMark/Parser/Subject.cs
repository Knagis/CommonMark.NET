using CommonMark.Syntax;
using System.Collections.Generic;

namespace CommonMark.Parser
{
    [System.Diagnostics.DebuggerDisplay("{DebugToString()}")]
    internal sealed class Subject
    {
        internal Subject(StringPart part, Dictionary<string, Reference> referenceMap)
            : this(part.Source, part.StartIndex, part.Length, referenceMap)
        {
        }

        public Subject(string buffer, Dictionary<string, Reference> referenceMap)
        {
            this.Buffer = buffer;
            this.Length = buffer.Length;
            this.ReferenceMap = referenceMap;
        }

        public Subject(string buffer, int startIndex, int length, Dictionary<string, Reference> referenceMap)
        {
            this.Buffer = buffer;
            this.ReferenceMap = referenceMap;
            this.Position = startIndex;
            this.Length = startIndex + length;

#if DEBUG
            this.StartIndex = startIndex;
#endif
        }

#if DEBUG
        private int StartIndex;
#endif

        /// <summary>
        /// Gets or sets the whole buffer this instance is created over.
        /// </summary>
        public readonly string Buffer;

        /// <summary>
        /// Gets or sets the current position in the buffer.
        /// </summary>
        public int Position;

        /// <summary>
        /// Gets or sets the length of the usable buffer. This can be less than the actual length of the
        /// buffer if some characters at the end of the buffer have to be ignored.
        /// </summary>
        public int Length;

        /// <summary>
        /// The last top-level inline parsed from this subject.
        /// </summary>
        public Inline LastInline;

        /// <summary>
        /// The last entry of the current stack of possible emphasis openers. Can be <c>null</c>.
        /// </summary>
        public InlineStack LastPendingInline;

        /// <summary>
        /// The first entry of the current stack of possible emphasis openers. Can be <c>null</c>.
        /// </summary>
        public InlineStack FirstPendingInline;

        public readonly Dictionary<string, Reference> ReferenceMap;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Used by [DebuggerDisplay]")]
        private string DebugToString()
        {
            var pos = (this.Position > this.Length) ? this.Position + 1 : this.Position;

            var res = this.Buffer.Insert(this.Length, "|");
            res = res.Insert(this.Position, "⁞");

#if DEBUG
            res.Insert(this.StartIndex, "|");
#endif

            return res;
        }
    }
}
