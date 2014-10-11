using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CommonMark.Tests
{
    [TestClass]
    public class HtmlTests
    {
        [TestMethod]
        [TestCategory("Inlines - Raw HTML")]
        public void HtmlTagsWithDigits()
        {
            // Arrange
            var commonMark = Helpers.Normalize("foo <h1>asd</h1> bar");
            var expected = Helpers.Normalize("<p>foo <h1>asd</h1> bar</p>");
            Helpers.LogValue("CommonMark", commonMark);
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }
    }
}
