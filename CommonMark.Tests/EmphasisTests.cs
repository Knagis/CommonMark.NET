using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CommonMark.Tests
{
    [TestClass]
    public class EmphasisTests
    {
        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        public void UnderscoreWithinEmphasis()
        {
            // See https://github.com/jgm/stmd/issues/51 for additional info
            // The rule is that inlines are processed left-to-right

            Helpers.ExecuteTest("*_*_", "<p><em>_</em>_</p>");
        }

        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        public void UnderscoreWithinEmphasis2()
        {
            // See https://github.com/jgm/stmd/issues/51 for additional info

            Helpers.ExecuteTest("*a _b _c d_ e*", "<p><em>a _b <em>c d</em> e</em></p>");
        }

        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        public void EmphasisWithCommas()
        {
            Helpers.ExecuteTest("**foo, *bar*, abc**", "<p><strong>foo, <em>bar</em>, abc</strong></p>");
        }

        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        public void DelayedEmphasisMatch1()
        {
            // '[' char in the middle will delay the ** match to the post-process phase.
            Helpers.ExecuteTest("foo ****ba[r****", "<p>foo <strong><strong>ba[r</strong></strong></p>");
        }

        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        public void DelayedEmphasisMatch2()
        {
            // '[' char in the middle will delay the ** match to the post-process phase.
            Helpers.ExecuteTest("foo ****ba[r**", "<p>foo **<strong>ba[r</strong></p>");
        }

        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        public void DelayedEmphasisMatch3()
        {
            // '[' char in the middle will delay the ** match to the post-process phase.
            Helpers.ExecuteTest("foo **ba[r****", "<p>foo <strong>ba[r</strong>**</p>");
        }

        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        public void DelayedEmphasisMatch4()
        {
            Helpers.ExecuteTest("**[foo* bar", "<p>*<em>[foo</em> bar</p>");
        }
    }
}
