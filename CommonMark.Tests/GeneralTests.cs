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
            Helpers.ExecuteTest("\u0000*foo*\0", "<p><em>foo</em></p>");
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
