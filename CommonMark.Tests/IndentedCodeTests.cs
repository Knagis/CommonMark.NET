using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CommonMark.Tests
{
    [TestClass]
    public class IndentedCodeTests
    {
        private static CommonMarkSettings _settings;
        private static CommonMarkSettings Settings
        {
            get
            {
                var s = _settings;
                if (s == null)
                {
                    s = CommonMarkSettings.Default.Clone();
                    s.AdditionalFeatures |= CommonMarkAdditionalFeatures.EmphasisInIndentedCode;
                    _settings = s;
                }
                return s;
            }
        }

        [TestMethod]
        [TestCategory("Indented Code - Emphasis and strong emphasis")]
        public void EmphasisInIndentedCodeDisabledByDefault()
        {
            Helpers.ExecuteTest("\t_Hello_", "<pre><code>_Hello_\n</code></pre>");
        }

        [TestMethod]
        [TestCategory("Indented Code - Emphasis and strong emphasis")]
        public void EmphasisInIndentedCodeDisabledByDefault2()
        {
            Helpers.ExecuteTest("\t*Hello*", "<pre><code>*Hello*\n</code></pre>");
        }

        [TestMethod]
        [TestCategory("Indented Code - Emphasis and strong emphasis")]
        public void StrongEmphasisInIndentedCodeDisabledByDefault()
        {
            Helpers.ExecuteTest("\t__Hello__", "<pre><code>__Hello__\n</code></pre>");
        }

        [TestMethod]
        [TestCategory("Indented Code - Emphasis and strong emphasis")]
        public void StrongEmphasisInIndentedCodeDisabledByDefault2()
        {
            Helpers.ExecuteTest("\t**Hello**", "<pre><code>**Hello**\n</code></pre>");
        }

        [TestMethod]
        [TestCategory("Indented Code - Emphasis and strong emphasis")]
        public void EmphasisInIndentedCode()
        {
            Helpers.ExecuteTest("\t_Hello_", "<pre><code><em>Hello</em></code></pre>", Settings);
        }

        [TestMethod]
        [TestCategory("Indented Code - Emphasis and strong emphasis")]
        public void EmphasisInIndentedCode2()
        {
            Helpers.ExecuteTest("\t*Hello*", "<pre><code><em>Hello</em></code></pre>", Settings);
        }

        [TestMethod]
        [TestCategory("Indented Code - Emphasis and strong emphasis")]
        public void StrongEmphasisInIndentedCode()
        {
            Helpers.ExecuteTest("\t__Hello__", "<pre><code><strong>Hello</strong></code></pre>", Settings);
        }

        [TestMethod]
        [TestCategory("Indented Code - Emphasis and strong emphasis")]
        public void StrongEmphasisInIndentedCode2()
        {
            Helpers.ExecuteTest("\t**Hello**", "<pre><code><strong>Hello</strong></code></pre>", Settings);
        }
    }
}
