using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CommonMark.Tests
{
    [TestClass]
    public class GeneralTests
    {
        /// <summary>
        /// Verifies that the U+0000 characters are removed from the source data.
        /// </summary>
        [TestMethod]
        [TestCategory("Security")]
        public void TestZeroCharRemoval()
        {
            Helpers.ExecuteTest("\u0000*foo*\0", "<p>\uFFFD<em>foo</em>\uFFFD</p>");
        }

        [TestMethod]
        [TestCategory("Other")]
        public void EmptyString()
        {
            Helpers.ExecuteTest("", "");
        }

        [TestMethod]
        [TestCategory("Other")]
        public void TrailingNewline()
        {
            var result = CommonMarkConverter.Convert("foo");
            Assert.AreEqual("<p>foo</p>" + Environment.NewLine, result);
        }

        /// <summary>
        /// Verifies that the U+0000 characters are removed from the source data.
        /// </summary>
        [TestMethod]
        [TestCategory("Other")]
        public void Version()
        {
            var version = CommonMarkConverter.Version;

            Assert.AreEqual(0, version.Major, "The version number is incorrect: {0}", version);
            Assert.IsTrue(version.Minor > 5, "The version number is incorrect: {0}", version);
        }

        [TestMethod]
        [TestCategory("CommonMarkConverter")]
        public void ConvertShortcutMethod()
        {
            var expected = "<p><strong>foo</strong></p>";
            var result = CommonMarkConverter.Convert("**foo**");

            // Assert
            Helpers.LogValue("Expected", expected);
            Helpers.LogValue("Actual", result);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(result));
        }
    }
}
