using CommonMark.Syntax;
using System.Collections.Generic;

namespace CommonMark.Parser
{
    [System.Diagnostics.DebuggerDisplay("{DebugToString()}")]
    internal sealed class Subject
    {
        public Subject(string buffer, Dictionary<string, Reference> referenceMap)
        {
            this.Buffer = buffer;
            this.ReferenceMap = referenceMap;
        }

        /// <summary>
        /// Gets or sets the whole buffer this instance is created over.
        /// </summary>
        public readonly string Buffer;

        /// <summary>
        /// Gets or sets the current position in the buffer.
        /// </summary>
        public int Position;

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
            if (this.Position > this.Buffer.Length)
                return this.Buffer;

            return this.Buffer.Insert(this.Position, "⁞");
        }
    }
}
