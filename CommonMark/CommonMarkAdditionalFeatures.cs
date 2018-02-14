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
        /// The parser will recognize syntax <c>[foo]</c>, which will be encoded in a separate AST node that the host application may evaluate as desired.
        /// </summary>
        PlaceholderBracket = 2,

        /// <summary>
        /// Allow YAML blocks (delimited by a line starting with exactly <c>---</c> and a line ending
        /// with exactly <c>---</c> or <c>...</c>. YAML blocks will take precedence over a <c>---</c>
        /// that might otherwise yield a thematic break.
        /// </summary>
        YamlBlocks = 4,

        /// <summary>
        /// Like <see cref="YamlBlocks"/> but will yield a maximum of one block, and only if the first
        /// line is exactly <c>---</c>.
        /// </summary>
        YamlFrontMatterOnly = 8,

        /// <summary>
        /// All additional features are enabled.
        /// </summary>
        All = 0x7FFFFFFF
    }
}
