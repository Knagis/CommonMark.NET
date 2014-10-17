using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CommonMark.Tests
{
    [TestClass]
    public class UrlTests
    {
        [TestMethod]
        [TestCategory("Inlines - Links")]
        public void NewlineInUrl()
        {
            // Arrange
            var commonMark = Helpers.Normalize("[foo](\r\n/url)\r\n\r\n[foo](");
            var expected = Helpers.Normalize("<p><a href=\"/url\">foo</a></p>\r\n<p>[foo](</p>");
            Helpers.LogValue("CommonMark", commonMark);
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        [TestMethod]
        [TestCategory("Inlines - Links")]
        public void UTF32CharacterEscape()
        {
            // Arrange
            var commonMark = Helpers.Normalize("[foo](/\uD835\uDD6B\uD835\uDCB5)");
            var expected = Helpers.Normalize("<p><a href=\"/%F0%9D%95%AB%F0%9D%92%B5\">foo</a></p>");
            Helpers.LogValue("CommonMark", commonMark);
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        [TestMethod]
        [TestCategory("Inlines - Links")]
        public void CustomUriResolver()
        {
            // Arrange
            var commonMark = Helpers.Normalize("foo␣**[a](~/temp)**␣[b][c]␣\r\n\r\n[c]:␣~/bar");
            var expected = Helpers.Normalize("<p>foo <strong><a href=\"/app/dir/temp\">a</a></strong> <a href=\"/app/dir/bar\">b</a></p>");
            var settings = CommonMarkSettings.Default.Clone();
            settings.UriResolver = url => url.Replace("~/", "/app/dir/");

            Helpers.LogValue("CommonMark", "foo␣**[a](~/temp)**␣[b][c]␣\r\n\r\n[c]:␣~/bar");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark, settings);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }
    }
}
