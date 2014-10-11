using System;
using System.Collections.Generic;
using System.Text;

namespace CommonMark
{
    /// <summary>
    /// Class used to configure the behavior of <see cref="CommonMarkConverter"/>.
    /// </summary>
    public sealed class CommonMarkSettings
    {
        /// <summary>
        /// Gets or sets the output format used by the last stage of conversion.
        /// </summary>
        public OutputFormat OutputFormat { get; set; }

        /// <summary>
        /// Gets or sets the delegate that is used to resolve addresses during rendering process. Can be used to process application relative URLs (<c>~/foo/bar</c>).
        /// </summary>
        /// <example><code>CommonMarkSettings.Default.UriResolver = VirtualPathUtility.ToAbsolute;</code></example>
        public Func<string, string> UriResolver { get; set; }

        private static readonly CommonMarkSettings _default = new CommonMarkSettings();

        /// <summary>
        /// The default settings for the converter. If the properties of this instance are modified, the changes will be applied to all
        /// future conversions that do not specify their own settings.
        /// </summary>
        public static CommonMarkSettings Default { get { return _default; } }

        /// <summary>
        /// Creates a copy of this configuration object.
        /// </summary>
        public CommonMarkSettings Clone()
        {
            return (CommonMarkSettings)this.MemberwiseClone();
        }
    }
}
