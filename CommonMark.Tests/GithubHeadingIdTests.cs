using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CommonMark.Tests
{
    [TestClass]
    public class GitHubHeadingIdTests
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
                    s.AdditionalFeatures |= CommonMarkAdditionalFeatures.GitHubStyleHeadingIds;
                    _settings = s;
                }
                return s;
            }
        }

        private static readonly string _linkContent = @"<svg class=""octicon octicon-link"" aria-hidden=""true"" height=""16"" role=""img"" version=""1.1"" viewBox=""0 0 16 16"" width=""16""><path d=""M4 9h1v1h-1c-1.5 0-3-1.69-3-3.5s1.55-3.5 3-3.5h4c1.45 0 3 1.69 3 3.5 0 1.41-0.91 2.72-2 3.25v-1.16c0.58-0.45 1-1.27 1-2.09 0-1.28-1.02-2.5-2-2.5H4c-0.98 0-2 1.22-2 2.5s1 2.5 2 2.5z m9-3h-1v1h1c1 0 2 1.22 2 2.5s-1.02 2.5-2 2.5H9c-0.98 0-2-1.22-2-2.5 0-0.83 0.42-1.64 1-2.09v-1.16c-1.09 0.53-2 1.84-2 3.25 0 1.81 1.55 3.5 3 3.5h4c1.45 0 3-1.69 3-3.5s-1.5-3.5-3-3.5z""></path></svg>";

        [TestMethod]
        [TestCategory("Container blocks - GitHub heading IDs")]
        public void GitHubHeadingIdsDisabledByDefault()
        {
            Helpers.ExecuteTest("# Test heading\n\nTest paragraph.", "<h1>Test heading</h1>\n<p>Test paragraph.</p>");
        }

        [TestMethod]
        [TestCategory("Container blocks - GitHub heading IDs")]
        public void GitHubHeadingIdsExample1() {
            Helpers.ExecuteTest("# Test heading\n\nTest paragraph.", @"<h1><a id=""user-content-test-heading"" class=""anchor"" href=""#user-content-test-heading"" aria-hidden=""true"">" + _linkContent + "</a>Test heading</h1>" + "\n<p>Test paragraph.</p>", Settings);
        }

        [TestMethod]
        [TestCategory("Container blocks - GitHub heading IDs")]
        public void GitHubHeadingIdsExample2() {
            Helpers.ExecuteTest("# Test [link](/url) heading\n\nTest paragraph.", @"<h1><a id=""user-content-test-link-heading"" class=""anchor"" href=""#user-content-test-link-heading"" aria-hidden=""true"">" + _linkContent + @"</a>Test <a href=""/url"">link</a> heading</h1>" + "\n<p>Test paragraph.</p>", Settings);
        }

        [TestMethod]
        [TestCategory("Container blocks - GitHub heading IDs")]
        public void GitHubHeadingIdsExample3() {
            Helpers.ExecuteTest("# Test [link](/url) heading with punc#t!uation\n\nTest paragraph.", @"<h1><a id=""user-content-test-link-heading-with-punctuation"" class=""anchor"" href=""#user-content-test-link-heading-with-punctuation"" aria-hidden=""true"">" + _linkContent + @"</a>Test <a href=""/url"">link</a> heading with punc#t!uation</h1>" + "\n<p>Test paragraph.</p>", Settings);
        }

        [TestMethod]
        [TestCategory("Container blocks - GitHub heading IDs")]
        public void GitHubHeadingIdsExample4() {
            Helpers.ExecuteTest("# Test complex šņāčÆÿőƕƢɱ heading\n\nTest paragraph.", @"<h1><a id=""user-content-test-complex-šņāčÆÿőƕƢɱ-heading"" class=""anchor"" href=""#user-content-test-complex-šņāčÆÿőƕƢɱ-heading"" aria-hidden=""true"">" + _linkContent + @"</a>Test complex šņāčÆÿőƕƢɱ heading</h1>" + "\n<p>Test paragraph.</p>", Settings);
        }

        [TestMethod]
        [TestCategory("Container blocks - GitHub heading IDs")]
        public void GitHubHeadingIdsExample5() {
            Helpers.ExecuteTest("## # ##  Test heading  spaces\n\nTest paragraph.", @"<h2><a id=""user-content----test-heading--spaces"" class=""anchor"" href=""#user-content----test-heading--spaces"" aria-hidden=""true"">" + _linkContent + @"</a># ##  Test heading  spaces</h2>" + "\n<p>Test paragraph.</p>", Settings);
        }

        [TestMethod]
        [TestCategory("Container blocks - GitHub heading IDs")]
        public void GitHubHeadingIdsExample6() {
            Helpers.ExecuteTest("# 3. Test numbered heading\n\nTest paragraph.", @"<h1><a id=""user-content-3-test-numbered-heading"" class=""anchor"" href=""#user-content-3-test-numbered-heading"" aria-hidden=""true"">" + _linkContent + @"</a>3. Test numbered heading</h1>" + "\n<p>Test paragraph.</p>", Settings);
        }

        [TestMethod]
        [TestCategory("Container blocks - GitHub heading IDs")]
        public void GitHubHeadingIdsExample7() {
            Helpers.ExecuteTest("# 456\n\nTest paragraph after pure number heading.", @"<h1><a id=""user-content-456"" class=""anchor"" href=""#user-content-456"" aria-hidden=""true"">" + _linkContent + @"</a>456</h1>" + "\n<p>Test paragraph after pure number heading.</p>", Settings);
        }

        [TestMethod]
        [TestCategory("Container blocks - GitHub heading IDs")]
        public void GitHubHeadingIdsExample8() {
            Helpers.ExecuteTest("# Test dUpE heading\n\n# Another heading\n\n# Test DuPe heading\n\nTest paragraph.", @"<h1><a id=""user-content-test-dupe-heading"" class=""anchor"" href=""#user-content-test-dupe-heading"" aria-hidden=""true"">" + _linkContent + @"</a>Test dUpE heading</h1>" + "\n" + @"<h1><a id=""user-content-another-heading"" class=""anchor"" href=""#user-content-another-heading"" aria-hidden=""true"">" + _linkContent + @"</a>Another heading</h1>" + "\n" + @"<h1><a id=""user-content-test-dupe-heading-1"" class=""anchor"" href=""#user-content-test-dupe-heading-1"" aria-hidden=""true"">" + _linkContent + @"</a>Test DuPe heading</h1>" + "\n<p>Test paragraph.</p>", Settings);
        }

        [TestMethod]
        [TestCategory("Container blocks - GitHub heading IDs")]
        public void GitHubHeadingIdsExample9() {
            Helpers.ExecuteTest("# Test dupe headings.\n\n# Test dupe headings!\n\n# Test dupe headings\n\n# Test dupe headings.\n\nTest paragraph.", @"<h1><a id=""user-content-test-dupe-headings"" class=""anchor"" href=""#user-content-test-dupe-headings"" aria-hidden=""true"">" + _linkContent + @"</a>Test dupe headings.</h1>" + "\n" + @"<h1><a id=""user-content-test-dupe-headings-1"" class=""anchor"" href=""#user-content-test-dupe-headings-1"" aria-hidden=""true"">" + _linkContent + @"</a>Test dupe headings!</h1>" + "\n" + @"<h1><a id=""user-content-test-dupe-headings-2"" class=""anchor"" href=""#user-content-test-dupe-headings-2"" aria-hidden=""true"">" + _linkContent + @"</a>Test dupe headings</h1>" + "\n" + @"<h1><a id=""user-content-test-dupe-headings-3"" class=""anchor"" href=""#user-content-test-dupe-headings-3"" aria-hidden=""true"">" + _linkContent + @"</a>Test dupe headings.</h1>" + "\n<p>Test paragraph.</p>", Settings);
        }
    }
}
