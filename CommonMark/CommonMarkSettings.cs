using System;
using System.Collections.Generic;
using System.Text;

namespace CommonMark
{
    public sealed class CommonMarkSettings
    {
        public CommonMarkSettings()
        {
            this.OutputFormat = OutputFormat.Html;
        }

        /// <summary>
        /// Gets or sets the output format used by the last stage of conversion.
        /// </summary>
        public OutputFormat OutputFormat { get; set; }

        private static readonly CommonMarkSettings _default = new CommonMarkSettings();

        /// <summary>
        /// The default settings for the converter. If the properties of this instance are modified, the changes will be applied to all
        /// future conversions that do not specify their own settings.
        /// </summary>
        public static CommonMarkSettings Default { get { return _default; } }
    }
}
