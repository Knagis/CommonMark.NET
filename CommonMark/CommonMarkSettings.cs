using System;
using System.Collections.Generic;

namespace CommonMark
{
    /// <summary>
    /// Class used to configure the behavior of <see cref="CommonMarkConverter"/>.
    /// </summary>
    /// <remarks>This class is not thread-safe so any changes to a instance that is reused (for example, the 
    /// <see cref="CommonMarkSettings.Default"/>) has to be updated while it is not in use otherwise the
    /// behaviour is undefined.</remarks>
    public sealed class CommonMarkSettings
    {
        /// <summary>Initializes a new instance of the <see cref="CommonMarkSettings" /> class.</summary>
        [Obsolete("Use CommonMarkSettings.Default.Clone() instead", false)]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public CommonMarkSettings()
        {
            Reset();
        }

        /// <summary>
        /// Gets or sets the output format used by the last stage of conversion.
        /// </summary>
        public OutputFormat OutputFormat { get; set; }

        private Action<Syntax.Block, System.IO.TextWriter, CommonMarkSettings> _outputDelegate;
        /// <summary>
        /// Gets or sets the custom output delegate function used for formatting CommonMark output.
        /// Setting this to a non-null value will also set <see cref="OutputFormat"/> to <see cref="OutputFormat.CustomDelegate"/>.
        /// </summary>
        public Action<Syntax.Block, System.IO.TextWriter, CommonMarkSettings> OutputDelegate
        {
            get { return this._outputDelegate; }
            set
            {
                if (this._outputDelegate != value)
                {
                    this._outputDelegate = value;
                    this.OutputFormat = value == null ? default(OutputFormat) : OutputFormat.CustomDelegate;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether soft line breaks should be rendered as hard line breaks.
        /// </summary>
        public bool RenderSoftLineBreaksAsLineBreaks { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the parser tracks precise positions in the source data for
        /// block and inline elements. This is disabled by default because it incurs an additional performance cost to
        /// keep track of the original position.
        /// Setting this to <c>true</c> will populate <see cref="Syntax.Inline.SourcePosition"/>, 
        /// <see cref="Syntax.Inline.SourceLength"/>, <see cref="Syntax.Block.SourcePosition"/> and 
        /// <see cref="Syntax.Block.SourceLength"/> properties with correct information, otherwise the values
        /// of these properties are undefined.
        /// This also controls if these values will be written to the output.
        /// </summary>
        public bool TrackSourcePosition { get; set; }

        private CommonMarkAdditionalFeatures _additionalFeatures;

        /// <summary>
        /// Gets or sets any additional features (that are not present in the current CommonMark specification) that
        /// the parser and/or formatter will recognize.
        /// </summary>
        public CommonMarkAdditionalFeatures AdditionalFeatures
        {
            get { return this._additionalFeatures; }
            set
            {
                this._additionalFeatures = value;
                this.Reset();
            }
        }

        private Func<string, string> _uriResolver;
        /// <summary>
        /// Gets or sets the delegate that is used to resolve addresses during rendering process. Can be used to process application relative URLs (<c>~/foo/bar</c>).
        /// </summary>
        /// <example><code>CommonMarkSettings.Default.UriResolver = VirtualPathUtility.ToAbsolute;</code></example>
        public Func<string, string> UriResolver 
        {
            get { return this._uriResolver; }
            set 
            {
                if (value != null)
                {
                    var orig = value;
                    value = x =>
                    {
                        try
                        {
                            return orig(x);
                        }
                        catch (Exception ex)
                        {
                            throw new CommonMarkException("An error occurred while executing the CommonMarkSettings.UriResolver delegate. View inner exception for details.", ex);
                        }
                    };
                }

                this._uriResolver = value;
            }
        }

#pragma warning disable 0618
        private static readonly CommonMarkSettings _default = new CommonMarkSettings();
#pragma warning restore 0618

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
            var clone = (CommonMarkSettings)this.MemberwiseClone();
            clone.Reset();
            return clone;
        }

        private void Reset()
        {
#if OptimizeFor45 || v4_0
            this._inlineParsers = new Lazy<Func<Parser.Subject, Syntax.Inline>[]>(this.InitializeParsers, System.Threading.LazyThreadSafetyMode.None);
            this._inlineParserSpecialCharacters = new Lazy<char[]>(this.InitializeSpecialCharacters, System.Threading.LazyThreadSafetyMode.None);
#else
            this._inlineParsers = null;
            this._inlineParserSpecialCharacters = null;
#endif
        }

        #region [ Properties that cache structures used in the parsers ]

#if OptimizeFor45 || v4_0
        private Lazy<Func<Parser.Subject, Syntax.Inline>[]> _inlineParsers;
#else
        private Func<Parser.Subject, Syntax.Inline>[] _inlineParsers;
#endif

        /// <summary>
        /// Gets the delegates that parse inline elements according to these settings.
        /// </summary>
        internal Func<Parser.Subject, Syntax.Inline>[] InlineParsers
        {
            get
            {
#if OptimizeFor45 || v4_0
                return _inlineParsers.Value;
#else
                var p = this._inlineParsers;
                if (p == null)
                {
                    p = this._inlineParsers = InitializeParsers();
                }

                return p;
#endif
            }
        }

        private Func<Parser.Subject, Syntax.Inline>[] InitializeParsers()
        {
            return Parser.InlineMethods.InitializeParsers(this);
        }

#if OptimizeFor45 || v4_0
        private Lazy<char[]> _inlineParserSpecialCharacters;
#else
        private char[] _inlineParserSpecialCharacters;
#endif

        /// <summary>
        /// Gets the characters that have special meaning for inline element parsers according to these settings.
        /// </summary>
        internal char[] InlineParserSpecialCharacters
        {
            get
            {
#if OptimizeFor45 || v4_0
                return _inlineParserSpecialCharacters.Value;
#else
                var v = this._inlineParserSpecialCharacters;
                if (v == null)
                {
                    v = this._inlineParserSpecialCharacters = InitializeSpecialCharacters();
                }

                return v;
#endif
            }
        }

        private char[] InitializeSpecialCharacters()
        {
            var p = this.InlineParsers;
            var vs = new List<char>(20);
            for (var i = 0; i < p.Length; i++)
                if (p[i] != null)
                    vs.Add((char)i);

            return vs.ToArray();
        }

        #endregion
    }
}
