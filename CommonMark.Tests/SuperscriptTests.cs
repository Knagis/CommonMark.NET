using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonMark.Tests
{
    [TestClass]
    public class SuperscriptTests
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
                    s.AdditionalFeatures |= CommonMarkAdditionalFeatures.SuperscriptCaret;
                    _settings = s;
                }
                return s;
            }
        }

        [TestMethod]
        [TestCategory("Inlines - Superscript")]
        public void SuperscriptDisabledByDefault()
        {
            Helpers.ExecuteTest("foo ^bar^", "<p>foo ^bar^</p>");
        }

        [TestMethod]
        [TestCategory("Inlines - Superscript")]
        public void SuperscriptExample0()
        {
            Helpers.ExecuteTest("foo ^bar^", "<p>foo <sup>bar</sup></p>", Settings);
        }

        [TestMethod]
        [TestCategory("Inlines - Superscript")]
        public void SuperscriptExample1()
        {
            Helpers.ExecuteTest("foo ^^bar^", "<p>foo ^<sup>bar</sup></p>", Settings);
        }

        [TestMethod]
        [TestCategory("Inlines - Superscript")]
        public void SuperscriptExample2()
        {
            Helpers.ExecuteTest("foo ^^bar^", "<p>foo ^<sup>bar</sup></p>", Settings);
        }

        [TestMethod]
        [TestCategory("Inlines - Superscript")]
        public void SuperscriptExample3()
        {
            Helpers.ExecuteTest("foo ^^bar^ asd^", "<p>foo <sup><sup>bar</sup> asd</sup></p>", Settings);
        }

        [TestMethod]
        [TestCategory("Inlines - Superscript")]
        public void SuperscriptExample4()
        {
            Helpers.ExecuteTest("foo ^*bar^*", "<p>foo <sup>*bar</sup>*</p>", Settings);
        }

        [TestMethod]
        [TestCategory("Inlines - Superscript")]
        public void SuperscriptExample5()
        {
            Helpers.ExecuteTest("foo *^bar^*", "<p>foo <em><sup>bar</sup></em></p>", Settings);
        }

        [TestMethod]
        [TestCategory("Inlines - Superscript")]
        public void SuperscriptExample6()
        {
            Helpers.ExecuteTest("foo **^bar**^", "<p>foo <strong>^bar</strong>^</p>", Settings);
        }

        [TestMethod]
        [TestCategory("Inlines - Superscript")]
        public void SuperscriptExample7()
        {
            Helpers.ExecuteTest("^bar^^", "<p><sup>bar</sup>^</p>", Settings);
        }

        [TestMethod]
        [TestCategory("Inlines - Superscript")]
        public void SuperscriptExample8()
        {
            // make sure that the fenced code blocks are not broken because of this feature
            Helpers.ExecuteTest("~~~foo\n^", "<pre><code class=\"language-foo\">^\n</code></pre>", Settings);
        }

        [TestMethod]
        [TestCategory("Inlines - Superscript")]
        public void SuperscriptExample9()
        {
            Helpers.ExecuteTest("foo ^^bar^^", "<p>foo <sup><sup>bar</sup></sup></p>", Settings);
        }

        [TestMethod]
        [TestCategory("Inlines - Superscript")]
        public void SuperscriptExample10()
        {
            // '[' char in the middle will delay the ^^ match to the post-process phase.
            Helpers.ExecuteTest("foo ^^ba[r^^", "<p>foo <sup><sup>ba[r</sup></sup></p>", Settings);
        }

        [TestMethod]
        [TestCategory("Inlines - Superscript")]
        public void SuperscriptExample10a()
        {
            // '[' char in the middle will delay the ^^ match to the post-process phase.
            Helpers.ExecuteTest("foo ^^ba[r^", "<p>foo ^<sup>ba[r</sup></p>", Settings);
        }

        [TestMethod]
        [TestCategory("Inlines - Superscript")]
        public void SuperscriptExample10b()
        {
            // '[' char in the middle will delay the ^^ match to the post-process phase.
            Helpers.ExecuteTest("foo ^ba[r^^", "<p>foo <sup>ba[r</sup>^</p>", Settings);
        }

        //No 10c for superscript

        [TestMethod]
        [TestCategory("Inlines - Superscript")]
        public void SuperscriptExample10d()
        {
            // '[' char in the middle will delay the ^^ match to the post-process phase.
            Helpers.ExecuteTest("^^[foo^ bar", "<p>^<sup>[foo</sup> bar</p>", Settings);
        }

        [TestMethod]
        [TestCategory("Inlines - Superscript")]
        public void SuperscriptExample101()
        {
            Helpers.ExecuteTest("foo ^ bar^", "<p>foo ^ bar^</p>", Settings);
        }

        [TestMethod]
        [TestCategory("Inlines - Superscript")]
        public void SuperscriptExample102()
        {
            Helpers.ExecuteTest("foo ^bar ^", "<p>foo ^bar ^</p>", Settings);
        }

        [TestMethod]
        [TestCategory("Inlines - Superscript")]
        public void SuperscriptExample103()
        {
            Helpers.ExecuteTest("foo ^ bar ^", "<p>foo ^ bar ^</p>", Settings);
        }
    }
}
