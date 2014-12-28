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
            Helpers.ExecuteTest("[foo](\r\n/url)\r\n\r\n[foo](", "<p><a href=\"/url\">foo</a></p>\r\n<p>[foo](</p>");
        }

        [TestMethod]
        [TestCategory("Inlines - Links")]
        public void UTF32CharacterEscape()
        {
            Helpers.ExecuteTest("[foo](/\uD835\uDD6B\uD835\uDCB5)", "<p><a href=\"/%F0%9D%95%AB%F0%9D%92%B5\">foo</a></p>");
        }

        [TestMethod]
        [TestCategory("Inlines - Links")]
        public void CustomUriResolver()
        {
            var settings = CommonMarkSettings.Default.Clone();
            settings.UriResolver = url => url.Replace("~/", "/app/dir/");
            Helpers.ExecuteTest("foo␣**[a](~/temp)**␣[b][c]␣\r\n\r\n[c]:␣~/bar", "<p>foo <strong><a href=\"/app/dir/temp\">a</a></strong> <a href=\"/app/dir/bar\">b</a></p>", settings);
        }

        [TestMethod]
        [TestCategory("Inlines - Links")]
        public void CustomUriResolverException()
        {
            var settings = CommonMarkSettings.Default.Clone();
            settings.UriResolver = url => { throw new ArgumentNullException(); };

            var gotException = false;
            try
            {
                Helpers.ExecuteTest("foo␣**[a](~/temp)**␣[b][c]␣\r\n\r\n[c]:␣~/bar", "<p>foo <strong><a href=\"/app/dir/temp\">a</a></strong> <a href=\"/app/dir/bar\">b</a></p>", settings);
            }
            catch(CommonMarkException)
            {
                gotException = true;
            }

            Assert.IsTrue(gotException, "A required exception was not thrown.");
        }
    }
}
