using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CommonMark.Tests
{
    [TestClass]
    public class PlaceholderTests
    {
        private const string TEST_CATEGORY = "Inlines - Placeholder";

        private static CommonMarkSettings _settings;
        private static CommonMarkSettings Settings
        {
            get
            {
                var s = _settings;
                if (s == null)
                {
                    s = CommonMarkSettings.Default.Clone();
                    s.AdditionalFeatures |= CommonMarkAdditionalFeatures.PlaceholderBracket;
                    _settings = s;
                }
                return s;
            }
        }

        private static Formatters.HtmlFormatter CreatePrintPlaceholdersWithColonDollarHtmlFormatter(System.IO.TextWriter target, CommonMarkSettings stngs)
        {
            return new Formatters.HtmlFormatter(target, stngs)
            {
                PlaceholderResolver = placeholder => ":" + placeholder + "$"
            };
        }

        private static Formatters.HtmlFormatter CreatePrintPlaceholdersWithColonDollarExcludesBHtmlFormatter(System.IO.TextWriter target, CommonMarkSettings stngs)
        {
            return new Formatters.HtmlFormatter(target, stngs)
            {
                PlaceholderResolver = placeholder => placeholder.Contains("b") ? null : ":" + placeholder + "$"
            };
        }

        [TestMethod]
        [TestCategory(TEST_CATEGORY)]
        public void PlaceholdersDisabledByDefault()
        {
            Helpers.ExecuteTest("foo [bar]", "<p>foo [bar]</p>");
        }

        [TestMethod]
        [TestCategory(TEST_CATEGORY)]
        public void PlaceholdersExample1()
        {
            Helpers.ExecuteTest("foo [bar]", "<p>foo :bar$</p>", Settings, CreatePrintPlaceholdersWithColonDollarHtmlFormatter);
        }

        [TestMethod]
        [TestCategory(TEST_CATEGORY)]
        public void PlaceholdersExample2()
        {
            Helpers.ExecuteTest("foo [[bar]", "<p>foo [:bar$</p>", Settings, CreatePrintPlaceholdersWithColonDollarHtmlFormatter);
        }

        [TestMethod]
        [TestCategory(TEST_CATEGORY)]
        public void PlaceholdersExample3()
        {
            Helpers.ExecuteTest("foo [bar]]", "<p>foo :bar$]</p>", Settings, CreatePrintPlaceholdersWithColonDollarHtmlFormatter);
        }

        [TestMethod]
        [TestCategory(TEST_CATEGORY)]
        public void PlaceholdersExample4()
        {
            Helpers.ExecuteTest("foo [ba[r]]", "<p>foo :ba[r]$</p>", Settings, CreatePrintPlaceholdersWithColonDollarHtmlFormatter);
        }

        [TestMethod]
        [TestCategory(TEST_CATEGORY)]
        public void PlaceholdersExample5()
        {
            Helpers.ExecuteTest("f[oo] [ba]r", "<p>f:oo$ [ba]r</p>", Settings, CreatePrintPlaceholdersWithColonDollarExcludesBHtmlFormatter);
        }

        [TestMethod]
        [TestCategory(TEST_CATEGORY)]
        public void PlaceholdersExample6()
        {
            Helpers.ExecuteTest("f[oo] [*b*a]r", "<p>f:oo$ :*b*a$r</p>", Settings, CreatePrintPlaceholdersWithColonDollarHtmlFormatter);
        }

        [TestMethod]
        [TestCategory(TEST_CATEGORY)]
        public void PlaceholdersExample7()
        {
            Helpers.ExecuteTest("f[oo] [*b*a]r", "<p>f:oo$ [<em>b</em>a]r</p>", Settings, CreatePrintPlaceholdersWithColonDollarExcludesBHtmlFormatter);
        }

        [TestMethod]
        [TestCategory(TEST_CATEGORY)]
        public void PlaceholdersExample8()
        {
            Helpers.ExecuteTest("foo **[bar]**", "<p>foo <strong>:bar$</strong></p>", Settings, CreatePrintPlaceholdersWithColonDollarHtmlFormatter);
        }

        [TestMethod]
        [TestCategory(TEST_CATEGORY)]
        public void PlaceholdersExample9()
        {
            Helpers.ExecuteTest("fo[o [ba]r](/url)", "<p>fo<a href=\"/url\">o :ba$r</a></p>", Settings, CreatePrintPlaceholdersWithColonDollarHtmlFormatter);
        }
    }
}
