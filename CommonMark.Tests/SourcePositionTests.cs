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
            var data = "* animals\r\n* * *horses*\n* * dogs\n* persons\r* 1. john\n* 2. anna";
            var doc = Helpers.ParseDocument(data, Settings);

            var inlinesQ = doc.AsEnumerable()
                .Where(o => o.Inline != null && o.IsOpening)
                .Select(o => o.Inline.SourceLength < 0 ? null : data.Substring(o.Inline.SourcePosition, o.Inline.SourceLength));

            var inlines = new HashSet<string>(inlinesQ, StringComparer.Ordinal);

            var expectedInlines = new[] 
            {
                "animals",
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
    }
}
