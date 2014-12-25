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
            // Arrange
            var commonMark = Helpers.Normalize("\u0000*foo*\0");
            var expected = Helpers.Normalize("<p><em>foo</em></p>");
            Helpers.LogValue("CommonMark", commonMark);
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Assert.AreEqual(0, commonMark[0]);
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }
    }
}
