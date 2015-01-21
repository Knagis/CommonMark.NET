using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CommonMark.Tests
{
    [TestClass]
    public class SourcePositionTests
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
                    s.TrackSourcePosition = true;
                    _settings = s;
                }
                return s;
            }
        }

        [TestMethod]
        [TestCategory("SourcePosition")]
        public void SourcePositionComplex()
        {
            var data = "\0\0   **foo**\r\n\t\0bar \n\n> *quoting **is nice***, `right`?";
            var doc = Helpers.ParseDocument(data, Settings);

            var inlinesQ = doc.AsEnumerable()
                .Where(o => o.Inline != null && o.IsOpening)
                .Select(o => o.Inline.SourceLength < 0 ? null : data.Substring(o.Inline.SourcePosition, o.Inline.SourceLength));

            var inlines = new HashSet<string>(inlinesQ, StringComparer.Ordinal);

            var expectedInlines = new[] 
            {
                "**foo**",
                "foo",
                "bar",
                "*quoting **is nice***",
                "**is nice**",
                "`right`",
                "?",
                "\n",
            };

            foreach (var ei in expectedInlines)
            {
                if (!inlines.Contains(ei))
                    Assert.Fail("Inline '{0}' was not parsed with correct source positions.", ei);
            }
        }

        [TestMethod]
        [TestCategory("SourcePosition")]
        public void SourcePositionNestedLists()
        {
            var data = "* ani\0mals\r\n* * *horses*\n* * dogs\n* persons\r* 1. john\n* 2. anna";
            var doc = Helpers.ParseDocument(data, Settings);

            var inlinesQ = doc.AsEnumerable()
                .Where(o => o.Inline != null && o.IsOpening)
                .Select(o => o.Inline.SourceLength < 0 ? null : data.Substring(o.Inline.SourcePosition, o.Inline.SourceLength));

            var inlines = new HashSet<string>(inlinesQ, StringComparer.Ordinal);

            var expectedInlines = new[] 
            {
                "ani\0mals",
                "*horses*",
                "dogs",
                "persons",
                "john",
                "anna",
            };

            foreach (var ei in expectedInlines)
            {
                if (!inlines.Contains(ei))
                    Assert.Fail("Inline '{0}' was not parsed with correct source positions.", ei);
            }
        }

        [TestMethod]
        [TestCategory("SourcePosition")]
        public void SourcePositionTabs()
        {
            foreach (var data in new[] { "h\to", "h\to\tr", "h\to\tr\ts\te\ts" })
            {
                var doc = Helpers.ParseDocument(data, Settings);

                var inline = doc.AsEnumerable().FirstOrDefault(o => o.Inline != null && o.Inline.Tag == Syntax.InlineTag.String);
                Assert.IsNotNull(inline, "Cannot find the string inline in '{0}'", data.Replace('\t', '→'));
                Assert.AreEqual(data.Replace('\t', '→'),
                    data.Substring(inline.Inline.SourcePosition, inline.Inline.SourceLength).Replace('\t', '→'));
            }
        }

        [TestMethod]
        [TestCategory("SourcePosition")]
        public void SourcePositionUnicodeZero()
        {
            var data = "h\0o\0r\0s\0e\0s";
            var doc = Helpers.ParseDocument(data, Settings);

            var inline = doc.AsEnumerable().FirstOrDefault(o => o.Inline != null && o.Inline.Tag == Syntax.InlineTag.String);
            Assert.IsNotNull(inline);
            Assert.AreEqual(data.Replace('\0', '␣'),
                data.Substring(inline.Inline.SourcePosition, inline.Inline.SourceLength).Replace('\0', '␣'));
        }

        [TestMethod]
        [TestCategory("SourcePosition")]
        public void SourcePositionStringNodes()
        {
            var data = "**foo** bar";
            var doc = Helpers.ParseDocument(data, Settings);

            var foo = doc.AsEnumerable().FirstOrDefault(o => o.Inline != null && o.Inline.LiteralContent == "foo");
            var bar = doc.AsEnumerable().FirstOrDefault(o => o.Inline != null && o.Inline.LiteralContent == " bar");

            Assert.IsNotNull(foo);
            Assert.AreEqual("foo", data.Substring(foo.Inline.SourcePosition, foo.Inline.SourceLength));

            Assert.IsNotNull(bar);
            Assert.AreEqual(" bar", data.Substring(bar.Inline.SourcePosition, bar.Inline.SourceLength));
        }

        [TestMethod]
        [TestCategory("SourcePosition")]
        public void SourcePositionLineBreak()
        {
            var data = "a  \nb\\\nc";
            var doc = Helpers.ParseDocument(data, Settings);

            var breaks = doc.AsEnumerable().Where(o => o.Inline != null && o.Inline.Tag == Syntax.InlineTag.LineBreak).ToList();
            Assert.AreEqual(2, breaks.Count);

            Assert.AreEqual("  ↓", data.Substring(breaks[0].Inline.SourcePosition, breaks[0].Inline.SourceLength).Replace("\n", "↓"));
            Assert.AreEqual("\\↓", data.Substring(breaks[1].Inline.SourcePosition, breaks[1].Inline.SourceLength).Replace("\n", "↓"));
        }

        [TestMethod]
        [TestCategory("SourcePosition")]
        public void SourcePositionSoftBreak1()
        {
            var data = "**foo**\0\r\n\0*bar*";
            var doc = Helpers.ParseDocument(data, Settings);

            var inline = doc.AsEnumerable().FirstOrDefault(o => o.Inline != null && o.Inline.Tag == Syntax.InlineTag.SoftBreak);
            Assert.IsNotNull(inline);

            // note that the SoftBreak inline unfortunately cannot include the full value of '\r\n' because
            // the position tracker will not be able to distinguish if the deleted char is '\r' or '\0' and 
            // will include both.
            Assert.AreEqual("\\n", 
                data.Substring(inline.Inline.SourcePosition, inline.Inline.SourceLength)
                    .Replace("\r", "\\r")
                    .Replace("\n", "\\n"));
        }

        [TestMethod]
        [TestCategory("SourcePosition")]
        public void SourcePositionSoftBreak2()
        {
            var data = "**foo**\0\n\0*bar*";
            var doc = Helpers.ParseDocument(data, Settings);

            var inline = doc.AsEnumerable().FirstOrDefault(o => o.Inline != null && o.Inline.Tag == Syntax.InlineTag.SoftBreak);
            Assert.IsNotNull(inline);

            Assert.AreEqual("\\n",
                data.Substring(inline.Inline.SourcePosition, inline.Inline.SourceLength)
                    .Replace("\r", "\\r")
                    .Replace("\n", "\\n"));
        }

        [TestMethod]
        [TestCategory("SourcePosition")]
        public void SourcePositionSoftBreak3()
        {
            var data = "**foo**\0\r\0*bar*";
            var doc = Helpers.ParseDocument(data, Settings);

            var inline = doc.AsEnumerable().FirstOrDefault(o => o.Inline != null && o.Inline.Tag == Syntax.InlineTag.SoftBreak);
            Assert.IsNotNull(inline);

            Assert.AreEqual("\\r",
                data.Substring(inline.Inline.SourcePosition, inline.Inline.SourceLength)
                    .Replace("\r", "\\r")
                    .Replace("\n", "\\n"));
        }

        [TestMethod]
        [TestCategory("SourcePosition")]
        public void SourcePositionEmphasis1()
        {
            var data = "**foo**";
            var doc = Helpers.ParseDocument(data, Settings);

            var inline = doc.AsEnumerable().FirstOrDefault(o => o.Inline != null && o.Inline.Tag == Syntax.InlineTag.Strong);
            Assert.IsNotNull(inline);
            Assert.AreEqual(data, data.Substring(inline.Inline.SourcePosition, inline.Inline.SourceLength));
        }

        [TestMethod]
        [TestCategory("SourcePosition")]
        public void SourcePositionEmphasis2()
        {
            var data = "***foo***";
            var doc = Helpers.ParseDocument(data, Settings);

            var inline = doc.AsEnumerable().FirstOrDefault(o => o.Inline != null && o.Inline.Tag == Syntax.InlineTag.Strong);
            Assert.IsNotNull(inline);
            Assert.AreEqual(data, data.Substring(inline.Inline.SourcePosition, inline.Inline.SourceLength));

            inline = doc.AsEnumerable().FirstOrDefault(o => o.Inline != null && o.Inline.Tag == Syntax.InlineTag.Emphasis);
            Assert.IsNotNull(inline);
            Assert.AreEqual("*foo*", data.Substring(inline.Inline.SourcePosition, inline.Inline.SourceLength));
        }

        [TestMethod]
        [TestCategory("SourcePosition")]
        public void SourcePositionEmphasis3()
        {
            var data = @"**[foo* bar";
            var doc = Helpers.ParseDocument(data, Settings);

            var inline = doc.AsEnumerable().FirstOrDefault(o => o.Inline != null && o.Inline.Tag == Syntax.InlineTag.Emphasis);
            Assert.IsNotNull(inline);
            Assert.AreEqual("*[foo*", data.Substring(inline.Inline.SourcePosition, inline.Inline.SourceLength));

            var str = doc.AsEnumerable().FirstOrDefault(o => o.Inline != null && o.Inline.LiteralContent == "*");
            Assert.IsNotNull(str);
            Assert.AreEqual("*", data.Substring(str.Inline.SourcePosition, str.Inline.SourceLength));
        }

        [TestMethod]
        [TestCategory("SourcePosition")]
        public void SourcePositionEmphasis4()
        {
            var data = @"*[foo*** bar";
            var doc = Helpers.ParseDocument(data, Settings);

            var inline = doc.AsEnumerable().FirstOrDefault(o => o.Inline != null && o.Inline.Tag == Syntax.InlineTag.Emphasis);
            Assert.IsNotNull(inline);
            Assert.AreEqual("*[foo*", data.Substring(inline.Inline.SourcePosition, inline.Inline.SourceLength));

            var str = doc.AsEnumerable().FirstOrDefault(o => o.Inline != null && o.Inline.LiteralContent == "**");
            Assert.IsNotNull(str);
            Assert.AreEqual("**", data.Substring(str.Inline.SourcePosition, str.Inline.SourceLength));
        }

        [TestMethod]
        [TestCategory("SourcePosition")]
        public void SourcePositionEmphasisStringNodes()
        {
            var data = @"**[foo* bar
***double***
*[inlink*
**[inlink2*
****many***
*foo**
*[bar***
**first**
*second*
third";
            data = data.Replace("\r", "");
            var doc = Helpers.ParseDocument(data, Settings);

            var res = doc.AsEnumerable()
                .Where(o => o.Inline != null && o.Inline.Tag == Syntax.InlineTag.String)
                .Select(o => new { o.Inline.LiteralContent, Source = data.Substring(o.Inline.SourcePosition, o.Inline.SourceLength) })
                .Where(o => o.Source != o.LiteralContent)
                .FirstOrDefault();

            Assert.IsNull(res, "String inline '" + (res == null ? string.Empty : res.LiteralContent) + "' has invalid position or length.");
        }

        [TestMethod]
        [TestCategory("SourcePosition")]
        public void SourcePositionBackticksMatch()
        {
            var data = @"``foo``";
            var doc = Helpers.ParseDocument(data, Settings);

            var inline = doc.AsEnumerable().FirstOrDefault(o => o.Inline != null && o.Inline.Tag == Syntax.InlineTag.Code);
            Assert.IsNotNull(inline);
            Assert.AreEqual(data, data.Substring(inline.Inline.SourcePosition, inline.Inline.SourceLength));
        }

        [TestMethod]
        [TestCategory("SourcePosition")]
        public void SourcePositionBackticksNoMatch()
        {
            var data = @"```foo``";
            var doc = Helpers.ParseDocument(data, Settings);

            var inline = doc.AsEnumerable().FirstOrDefault(o => o.Inline != null && o.Inline.LiteralContent == "```");
            Assert.IsNotNull(inline);
            Assert.AreEqual("```", data.Substring(inline.Inline.SourcePosition, inline.Inline.SourceLength));

            inline = doc.AsEnumerable().FirstOrDefault(o => o.Inline != null && o.Inline.LiteralContent == "``");
            Assert.IsNotNull(inline);
            Assert.AreEqual("``", data.Substring(inline.Inline.SourcePosition, inline.Inline.SourceLength));
        }

        [TestMethod]
        [TestCategory("SourcePosition")]
        public void SourcePositionBackslash()
        {
            var data = "a\\b\\*d\\\r\ne\\";
            var doc = Helpers.ParseDocument(data, Settings);

            // backslash before "b" (unescapable char)
            var inline = doc.AsEnumerable().FirstOrDefault(o => o.Inline != null && o.Inline.LiteralContent == "\\");
            Assert.IsNotNull(inline);
            Assert.AreEqual("\\", data.Substring(inline.Inline.SourcePosition, inline.Inline.SourceLength));

            // backslash at the end of the string
            inline = doc.AsEnumerable().Last(o => o.Inline != null && o.Inline.LiteralContent == "\\");
            Assert.IsNotNull(inline);
            Assert.AreEqual("\\", data.Substring(inline.Inline.SourcePosition, inline.Inline.SourceLength));

            // escaped char
            inline = doc.AsEnumerable().FirstOrDefault(o => o.Inline != null && o.Inline.LiteralContent == "*");
            Assert.IsNotNull(inline);
            Assert.AreEqual("\\*", data.Substring(inline.Inline.SourcePosition, inline.Inline.SourceLength));

            // linebreak
            inline = doc.AsEnumerable().FirstOrDefault(o => o.Inline != null && o.Inline.Tag == Syntax.InlineTag.LineBreak);
            Assert.IsNotNull(inline);
            Assert.AreEqual("\\\r\n", data.Substring(inline.Inline.SourcePosition, inline.Inline.SourceLength));
        }

        [TestMethod]
        [TestCategory("SourcePosition")]
        public void SourcePositionEntities()
        {
            var data = "foo &copy; &#1234; &#98765432; &MadeUp; &test";
            var doc = Helpers.ParseDocument(data, Settings);

            var inline = doc.AsEnumerable().FirstOrDefault(o => o.Inline != null && o.Inline.LiteralContent == "©");
            Assert.IsNotNull(inline);
            Assert.AreEqual("&copy;", data.Substring(inline.Inline.SourcePosition, inline.Inline.SourceLength));

            inline = doc.AsEnumerable().FirstOrDefault(o => o.Inline != null && o.Inline.LiteralContent == "Ӓ");
            Assert.IsNotNull(inline);
            Assert.AreEqual("&#1234;", data.Substring(inline.Inline.SourcePosition, inline.Inline.SourceLength));

            inline = doc.AsEnumerable().FirstOrDefault(o => o.Inline != null && o.Inline.LiteralContent == "�");
            Assert.IsNotNull(inline);
            Assert.AreEqual("&#98765432;", data.Substring(inline.Inline.SourcePosition, inline.Inline.SourceLength));

            inline = doc.AsEnumerable().FirstOrDefault(o => o.Inline != null && o.Inline.LiteralContent == "&MadeUp;");
            Assert.IsNotNull(inline);
            Assert.AreEqual("&MadeUp;", data.Substring(inline.Inline.SourcePosition, inline.Inline.SourceLength));

            inline = doc.AsEnumerable().FirstOrDefault(o => o.Inline != null && o.Inline.LiteralContent == "&");
            Assert.IsNotNull(inline);
            Assert.AreEqual("&", data.Substring(inline.Inline.SourcePosition, inline.Inline.SourceLength));
        }

        [TestMethod]
        [TestCategory("SourcePosition")]
        public void SourcePositionAutoLinks()
        {
            var data = "foo <http://google.com> bar <no@spam.org> foo";
            var doc = Helpers.ParseDocument(data, Settings);

            var inline = doc.AsEnumerable().FirstOrDefault(o => o.Inline != null && o.Inline.TargetUrl == "http://google.com");
            Assert.IsNotNull(inline);
            Assert.AreEqual("<http://google.com>", data.Substring(inline.Inline.SourcePosition, inline.Inline.SourceLength));

            inline = doc.AsEnumerable().FirstOrDefault(o => o.Inline != null && o.Inline.Tag == Syntax.InlineTag.String && o.Inline.LiteralContent == "http://google.com");
            Assert.IsNotNull(inline);
            Assert.AreEqual("http://google.com", data.Substring(inline.Inline.SourcePosition, inline.Inline.SourceLength));

            inline = doc.AsEnumerable().FirstOrDefault(o => o.Inline != null && o.Inline.TargetUrl == "mailto:no@spam.org");
            Assert.IsNotNull(inline);
            Assert.AreEqual("<no@spam.org>", data.Substring(inline.Inline.SourcePosition, inline.Inline.SourceLength));

            inline = doc.AsEnumerable().FirstOrDefault(o => o.Inline != null && o.Inline.Tag == Syntax.InlineTag.String && o.Inline.LiteralContent == "no@spam.org");
            Assert.IsNotNull(inline);
            Assert.AreEqual("no@spam.org", data.Substring(inline.Inline.SourcePosition, inline.Inline.SourceLength));
        }

        [TestMethod]
        [TestCategory("SourcePosition")]
        public void SourcePositionInlineRawHtml()
        {
            var data = "foo <strong x=1>bar</strong> x<";
            var doc = Helpers.ParseDocument(data, Settings);

            var inline = doc.AsEnumerable().FirstOrDefault(o => o.Inline != null && o.Inline.LiteralContent == "<strong x=1>");
            Assert.IsNotNull(inline);
            Assert.AreEqual("<strong x=1>", data.Substring(inline.Inline.SourcePosition, inline.Inline.SourceLength));

            inline = doc.AsEnumerable().FirstOrDefault(o => o.Inline != null && o.Inline.LiteralContent == "</strong>");
            Assert.IsNotNull(inline);
            Assert.AreEqual("</strong>", data.Substring(inline.Inline.SourcePosition, inline.Inline.SourceLength));

            inline = doc.AsEnumerable().FirstOrDefault(o => o.Inline != null && o.Inline.LiteralContent == "<");
            Assert.IsNotNull(inline);
            Assert.AreEqual("<", data.Substring(inline.Inline.SourcePosition, inline.Inline.SourceLength));
        }

        [TestMethod]
        [TestCategory("SourcePosition")]
        public void SourcePositionStrikethrough()
        {
            // this method verifies only simple case because most of the code is common with emphasis parser
            // so those tests will apply to strikethrough as well.

            var data = "~~~~foo~~~~ ~";
            var stng = Settings.Clone();
            stng.AdditionalFeatures |= CommonMarkAdditionalFeatures.StrikethroughTilde;
            var doc = Helpers.ParseDocument(data, stng);

            var inline = doc.AsEnumerable().FirstOrDefault(o => o.Inline != null && o.Inline.Tag == Syntax.InlineTag.Strikethrough);
            Assert.IsNotNull(inline);
            Assert.AreEqual("~~~~foo~~~~", data.Substring(inline.Inline.SourcePosition, inline.Inline.SourceLength));

            inline = doc.AsEnumerable().Last(o => o.Inline != null && o.IsOpening && o.Inline.Tag == Syntax.InlineTag.Strikethrough);
            Assert.IsNotNull(inline);
            Assert.AreEqual("~~foo~~", data.Substring(inline.Inline.SourcePosition, inline.Inline.SourceLength));

            inline = doc.AsEnumerable().Last(o => o.Inline != null && o.IsOpening && o.Inline.LiteralContent == "~");
            Assert.IsNotNull(inline);
            Assert.AreEqual("~", data.Substring(inline.Inline.SourcePosition, inline.Inline.SourceLength));
        }

        [TestMethod]
        [TestCategory("SourcePosition")]
        public void SourcePositionImage()
        {
            var data = "![bar](/url) !foo";
            
            var doc = Helpers.ParseDocument(data, Settings);

            var inline = doc.AsEnumerable().FirstOrDefault(o => o.Inline != null && o.Inline.Tag == Syntax.InlineTag.Image);
            Assert.IsNotNull(inline);
            Assert.AreEqual("![bar](/url)", data.Substring(inline.Inline.SourcePosition, inline.Inline.SourceLength));

            inline = doc.AsEnumerable().FirstOrDefault(o => o.Inline != null && o.Inline.LiteralContent == "!");
            Assert.IsNotNull(inline);
            Assert.AreEqual("!", data.Substring(inline.Inline.SourcePosition, inline.Inline.SourceLength));
        }

        [TestMethod]
        [TestCategory("SourcePosition")]
        public void SourcePositionLink1()
        {
            var data = "[bar](/url)";

            var doc = Helpers.ParseDocument(data, Settings);

            var inline = doc.AsEnumerable().FirstOrDefault(o => o.Inline != null && o.Inline.Tag == Syntax.InlineTag.Link);
            Assert.IsNotNull(inline);
            Assert.AreEqual(data, data.Substring(inline.Inline.SourcePosition, inline.Inline.SourceLength));
        }

        [TestMethod]
        [TestCategory("SourcePosition")]
        public void SourcePositionLink2()
        {
            var data = "[bar]\n\n[bar]: /url";

            var doc = Helpers.ParseDocument(data, Settings);

            var inline = doc.AsEnumerable().FirstOrDefault(o => o.Inline != null && o.Inline.Tag == Syntax.InlineTag.Link);
            Assert.IsNotNull(inline);
            Assert.AreEqual("[bar]", data.Substring(inline.Inline.SourcePosition, inline.Inline.SourceLength));
        }

        [TestMethod]
        [TestCategory("SourcePosition")]
        public void SourcePositionLink3()
        {
            var data = "[foo][bar]\n\n[bar]: /url";

            var doc = Helpers.ParseDocument(data, Settings);

            var inline = doc.AsEnumerable().FirstOrDefault(o => o.Inline != null && o.Inline.Tag == Syntax.InlineTag.Link);
            Assert.IsNotNull(inline);
            Assert.AreEqual("[foo][bar]", data.Substring(inline.Inline.SourcePosition, inline.Inline.SourceLength));
        }

        [TestMethod]
        [TestCategory("SourcePosition")]
        public void SourcePositionLinkInvalid()
        {
            var data = "] [foo]\n\n[bar [x](/u)](/u)";

            var doc = Helpers.ParseDocument(data, Settings);

            var cnt = 0;
            foreach (var inline in doc.AsEnumerable().Where(o => o.Inline != null && o.Inline.LiteralContent == "]"))
            {
                cnt++;
                Assert.AreEqual("]", data.Substring(inline.Inline.SourcePosition, inline.Inline.SourceLength));
            }

            Assert.AreEqual(3, cnt);
        }
    }
}
