using System;

namespace CommonMark
{
    /// <summary>
    /// Lists additional features that can be enabled in <see cref="CommonMarkSettings"/>.
    /// These features are not part of the standard and should not be used if interoperability with other
    /// CommonMark implementations is required.
    /// </summary>
    [Flags]
    public enum CommonMarkAdditionalFeatures
    {
        /// <summary>
        /// No additional features are enabled. This is the default.
        /// </summary>
        None = 0,

        /// <summary>
        /// The parser will recognize syntax <c>~~foo~~</c> that will be rendered as <c>&lt;del&gt;foo&lt;/del&gt;</c>.
        /// </summary>
        StrikethroughTilde = 1,

        /// <summary>
        /// The parser will recognize emphasis (text enclosed in single/double <c>*</c>/<c>_</c>) in indented code.
        /// This is useful in rendering commands, e.g. <c>format **c:**</c>.
        /// </summary>
        EmphasisInIndentedCode = 32,

        /// <summary>
        /// All additional features are enabled.
        /// </summary>
        All = 0x7FFFFFFF
    }
}
