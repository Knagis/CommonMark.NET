using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CommonMark.Tests
{
    [TestClass]
    public class SubscriptTests
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
                    s.AdditionalFeatures |= CommonMarkAdditionalFeatures.SubscriptTilde;
                    _settings = s;
                }
                return s;
            }
        }

        [TestMethod]
        [TestCategory("Inlines - Subscript")]
        public void SubscriptDisabledByDefault()
        {
            Helpers.ExecuteTest("foo ~bar~", "<p>foo ~bar~</p>");
        }

        [TestMethod]
        [TestCategory("Inlines - Subscript")]
        public void SubscriptExample0()
        {
            Helpers.ExecuteTest("foo ~bar~", "<p>foo <sub>bar</sub></p>", Settings);
        }

        [TestMethod]
        [TestCategory("Inlines - Subscript")]
        public void SubscriptExample1()
        {
            Helpers.ExecuteTest("foo ~~bar~", "<p>foo ~<sub>bar</sub></p>", Settings);
        }

        [TestMethod]
        [TestCategory("Inlines - Subscript")]
        public void SubscriptExample2()
        {
            Helpers.ExecuteTest("foo ~~bar~", "<p>foo ~<sub>bar</sub></p>", Settings);
        }

        [TestMethod]
        [TestCategory("Inlines - Subscript")]
        public void SubscriptExample3()
        {
            Helpers.ExecuteTest("foo ~~bar~ asd~", "<p>foo <sub><sub>bar</sub> asd</sub></p>", Settings);
        }

        [TestMethod]
        [TestCategory("Inlines - Subscript")]
        public void SubscriptExample4()
        {
            Helpers.ExecuteTest("foo ~*bar~*", "<p>foo <sub>*bar</sub>*</p>", Settings);
        }

        [TestMethod]
        [TestCategory("Inlines - Subscript")]
        public void SubscriptExample5()
        {
            Helpers.ExecuteTest("foo *~bar~*", "<p>foo <em><sub>bar</sub></em></p>", Settings);
        }

        [TestMethod]
        [TestCategory("Inlines - Subscript")]
        public void SubscriptExample6()
        {
            Helpers.ExecuteTest("foo **~bar**~", "<p>foo <strong>~bar</strong>~</p>", Settings);
        }

        [TestMethod]
        [TestCategory("Inlines - Subscript")]
        public void SubscriptExample7()
        {
            Helpers.ExecuteTest("~bar~~", "<p><sub>bar</sub>~</p>", Settings);
        }

        [TestMethod]
        [TestCategory("Inlines - Subscript")]
        public void SubscriptExample8()
        {
            // make sure that the fenced code blocks are not broken because of this feature
            Helpers.ExecuteTest("~~~foo\n~", "<pre><code class=\"language-foo\">~\n</code></pre>", Settings);
        }

        [TestMethod]
        [TestCategory("Inlines - Subscript")]
        public void SubscriptExample9()
        {
            Helpers.ExecuteTest("foo ~~bar~~", "<p>foo <sub><sub>bar</sub></sub></p>", Settings);
        }

        [TestMethod]
        [TestCategory("Inlines - Subscript")]
        public void SubscriptExample10()
        {
            // '[' char in the middle will delay the ~~ match to the post-process phase.
            Helpers.ExecuteTest("foo ~~ba[r~~", "<p>foo <sub><sub>ba[r</sub></sub></p>", Settings);
        }

        [TestMethod]
        [TestCategory("Inlines - Subscript")]
        public void SubscriptExample10a()
        {
            // '[' char in the middle will delay the ~~ match to the post-process phase.
            Helpers.ExecuteTest("foo ~~ba[r~", "<p>foo ~<sub>ba[r</sub></p>", Settings);
        }

        [TestMethod]
        [TestCategory("Inlines - Subscript")]
        public void SubscriptExample10b()
        {
            // '[' char in the middle will delay the ~~ match to the post-process phase.
            Helpers.ExecuteTest("foo ~ba[r~~", "<p>foo <sub>ba[r</sub>~</p>", Settings);
        }

        //No 10c for subscript

        [TestMethod]
        [TestCategory("Inlines - Subscript")]
        public void SubscriptExample10d()
        {
            // '[' char in the middle will delay the ~~ match to the post-process phase.
            Helpers.ExecuteTest("~~[foo~ bar", "<p>~<sub>[foo</sub> bar</p>", Settings);
        }

        [TestMethod]
        [TestCategory("Inlines - Subscript")]
        public void SubscriptExample101()
        {
            Helpers.ExecuteTest("foo ~ bar~", "<p>foo ~ bar~</p>", Settings);
        }

        [TestMethod]
        [TestCategory("Inlines - Subscript")]
        public void SubscriptExample102()
        {
            Helpers.ExecuteTest("foo ~bar ~", "<p>foo ~bar ~</p>", Settings);
        }

        [TestMethod]
        [TestCategory("Inlines - Subscript")]
        public void SubscriptExample103()
        {
            Helpers.ExecuteTest("foo ~ bar ~", "<p>foo ~ bar ~</p>", Settings);
        }
    }
}
