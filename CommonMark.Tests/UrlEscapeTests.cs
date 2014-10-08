using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CommonMark.Tests
{
    [TestClass]
    public class UrlEscapeTests
    {
        [TestMethod]
        [TestCategory("Leaf blocks - Link reference definitions")]
        public void UTF32CharacterEscape()
        {
            // Arrange
            var commonMark = Helpers.Normalize("[foo](/\uD835\uDD6B\uD835\uDCB5)");
            var expected = Helpers.Normalize("<p><a href=\"/%F0%9D%95%AB%F0%9D%92%B5\">foo</a></p>");

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }
    }
}
