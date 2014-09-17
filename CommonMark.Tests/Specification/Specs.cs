using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CommonMark.Tests.Specification
{
    [TestClass]
    public class Specs
    {
        // a---
        // title: CommonMark Spec
        // author:
        // - John MacFarlane
        // version: 1
        // date: 2014-09-06
        // ...
        //
        // # Introduction
        //
        // ## What is Markdown?
        //
        // Markdown is a plain text format for writing structured documents,
        // based on conventions used for indicating formatting in email and
        // usenet posts.  It was developed in 2004 by John Gruber, who wrote
        // the first Markdown-to-HTML converter in perl, and it soon became
        // widely used in websites.  By 2014 there were dozens of
        // implementations in many languages.  Some of them extended basic
        // Markdown syntax with conventions for footnotes, definition lists,
        // tables, and other constructs, and some allowed output not just in
        // HTML but in LaTeX and many other formats.
        //
        // ## Why is a spec needed?
        //
        // John Gruber's [canonical description of Markdown's
        // syntax](http://daringfireball.net/projects/markdown/syntax)
        // does not specify the syntax unambiguously.  Here are some examples of
        // questions it does not answer:
        //
        // 1.  How much indentation is needed for a sublist?  The spec says that
        // continuation paragraphs need to be indented four spaces, but is
        // not fully explicit about sublists.  It is natural to think that
        // they, too, must be indented four spaces, but `Markdown.pl` does
        // not require that.  This is hardly a "corner case," and divergences
        // between implementations on this issue often lead to surprises for
        // users in real documents. (See [this comment by John
        // Gruber](http://article.gmane.org/gmane.text.markdown.general/1997).)
        //
        // 2.  Is a blank line needed before a block quote or header?
        // Most implementations do not require the blank line.  However,
        // this can lead to unexpected results in hard-wrapped text, and
        // also to ambiguities in parsing (note that some implementations
        // put the header inside the blockquote, while others do not).
        // (John Gruber has also spoken [in favor of requiring the blank
        // lines](http://article.gmane.org/gmane.text.markdown.general/2146).)
        //
        // 3.  Is a blank line needed before an indented code block?
        // (`Markdown.pl` requires it, but this is not mentioned in the
        // documentation, and some implementations do not require it.)
        //
        // ``` markdown
        // paragraph
        // code?
        // ```
        //
        // 4.  What is the exact rule for determining when list items get
        // wrapped in `<p>` tags?  Can a list be partially "loose" and partially
        // "tight"?  What should we do with a list like this?
        //
        // ``` markdown
        // 1. one
        //
        // 2. two
        // 3. three
        // ```
        //
        // Or this?
        //
        // ``` markdown
        // 1.  one
        // - a
        //
        // - b
        // 2.  two
        // ```
        //
        // (There are some relevant comments by John Gruber
        // [here](http://article.gmane.org/gmane.text.markdown.general/2554).)
        //
        // 5.  Can list markers be indented?  Can ordered list markers be right-aligned?
        //
        // ``` markdown
        // 8. item 1
        // 9. item 2
        // 10. item 2a
        // ```
        //
        // 6.  Is this one list with a horizontal rule in its second item,
        // or two lists separated by a horizontal rule?
        //
        // ``` markdown
        // * a
        // * * * * *
        // * b
        // ```
        //
        // 7.  When list markers change from numbers to bullets, do we have
        // two lists or one?  (The Markdown syntax description suggests two,
        // but the perl scripts and many other implementations produce one.)
        //
        // ``` markdown
        // 1. fee
        // 2. fie
        // -  foe
        // -  fum
        // ```
        //
        // 8.  What are the precedence rules for the markers of inline structure?
        // For example, is the following a valid link, or does the code span
        // take precedence ?
        //
        // ``` markdown
        // [a backtick (`)](/url) and [another backtick (`)](/url).
        // ```
        //
        // 9.  What are the precedence rules for markers of emphasis and strong
        // emphasis?  For example, how should the following be parsed?
        //
        // ``` markdown
        // *foo *bar* baz*
        // ```
        //
        // 10. What are the precedence rules between block-level and inline-level
        // structure?  For example, how should the following be parsed?
        //
        // ``` markdown
        // - `a long code span can contain a hyphen like this
        // - and it can screw things up`
        // ```
        //
        // 11. Can list items include headers?  (`Markdown.pl` does not allow this,
        // but headers can occur in blockquotes.)
        //
        // ``` markdown
        // - # Heading
        // ```
        //
        // 12. Can link references be defined inside block quotes or list items?
        //
        // ``` markdown
        // > Blockquote [foo].
        // >
        // > [foo]: /url
        // ```
        //
        // 13. If there are multiple definitions for the same reference, which takes
        // precedence?
        //
        // ``` markdown
        // [foo]: /url1
        // [foo]: /url2
        //
        // [foo][]
        // ```
        //
        // In the absence of a spec, early implementers consulted `Markdown.pl`
        // to resolve these ambiguities.  But `Markdown.pl` was quite buggy, and
        // gave manifestly bad results in many cases, so it was not a
        // satisfactory replacement for a spec.
        //
        // Because there is no unambiguous spec, implementations have diverged
        // considerably.  As a result, users are often surprised to find that
        // a document that renders one way on one system (say, a github wiki)
        // renders differently on another (say, converting to docbook using
        // pandoc).  To make matters worse, because nothing in Markdown counts
        // as a "syntax error," the divergence often isn't discovered right away.
        //
        // ## About this document
        //
        // This document attempts to specify Markdown syntax unambiguously.
        // It contains many examples with side-by-side Markdown and
        // HTML.  These are intended to double as conformance tests.  An
        // accompanying script `runtests.pl` can be used to run the tests
        // against any Markdown program:
        //
        // perl runtests.pl spec.txt PROGRAM
        //
        // Since this document describes how Markdown is to be parsed into
        // an abstract syntax tree, it would have made sense to use an abstract
        // representation of the syntax tree instead of HTML.  But HTML is capable
        // of representing the structural distinctions we need to make, and the
        // choice of HTML for the tests makes it possible to run the tests against
        // an implementation without writing an abstract syntax tree renderer.
        //
        // This document is generated from a text file, `spec.txt`, written
        // in Markdown with a small extension for the side-by-side tests.
        // The script `spec2md.pl` can be used to turn `spec.txt` into pandoc
        // Markdown, which can then be converted into other formats.
        //
        // In the examples, the `→` character is used to represent tabs.
        //
        // # Preprocessing
        //
        // A [line](#line) <a id="line"></a>
        // is a sequence of zero or more characters followed by a line
        // ending (CR, LF, or CRLF) or by the end of
        // file.
        //
        // This spec does not specify an encoding; it thinks of lines as composed
        // of characters rather than bytes.  A conforming parser may be limited
        // to a certain encoding.
        //
        // Tabs in lines are expanded to spaces, with a tab stop of 4 characters:
        [TestMethod]
        [TestCategory("Preprocessing")]
        //[Timeout(1000)]
        public void Example001()
        {
            // Example 1
            // Section: Preprocessing
            //
            // The following CommonMark:
            //     →foo→baz→→bim
            //
            // Should be rendered as:
            //     <pre><code>foo baz     bim
            //     </code></pre>

            // Arrange
            var commonMark = Helpers.Normalize("→foo→baz→→bim");
            var expected = Helpers.Normalize("<pre><code>foo baz     bim\n</code></pre>");
            Helpers.Log("Example {0}", 1);
            Helpers.Log("Section: {0}", "Preprocessing");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "→foo→baz→→bim");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        [TestMethod]
        [TestCategory("Preprocessing")]
        //[Timeout(1000)]
        public void Example002()
        {
            // Example 2
            // Section: Preprocessing
            //
            // The following CommonMark:
            //         a→a
            //         ὐ→a
            //
            // Should be rendered as:
            //     <pre><code>a   a
            //     ὐ   a
            //     </code></pre>

            // Arrange
            var commonMark = Helpers.Normalize("    a→a\n    ὐ→a");
            var expected = Helpers.Normalize("<pre><code>a   a\nὐ   a\n</code></pre>");
            Helpers.Log("Example {0}", 2);
            Helpers.Log("Section: {0}", "Preprocessing");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "    a→a\n    ὐ→a");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // Line endings are replaced by newline characters (LF).
        //
        // A line containing no characters, or a line containing only spaces (after
        // tab expansion), is called a [blank line](#blank-line).
        // <a id="blank-line"></a>
        //
        // # Blocks and inlines
        //
        // We can think of a document as a sequence of [blocks](#block)<a
        // id="block"></a>---structural elements like paragraphs, block quotations,
        // lists, headers, rules, and code blocks.  Blocks can contain other
        // blocks, or they can contain [inline](#inline)<a id="inline"></a> content:
        // words, spaces, links, emphasized text, images, and inline code.
        //
        // ## Precedence
        //
        // Indicators of block structure always take precedence over indicators
        // of inline structure.  So, for example, the following is a list with
        // two items, not a list with one item containing a code span:
        [TestMethod]
        [TestCategory("Blocks and inlines - Precedence")]
        //[Timeout(1000)]
        public void Example003()
        {
            // Example 3
            // Section: Blocks and inlines - Precedence
            //
            // The following CommonMark:
            //     - `one
            //     - two`
            //
            // Should be rendered as:
            //     <ul>
            //     <li>`one</li>
            //     <li>two`</li>
            //     </ul>

            // Arrange
            var commonMark = Helpers.Normalize("- `one\n- two`");
            var expected = Helpers.Normalize("<ul>\n<li>`one</li>\n<li>two`</li>\n</ul>");
            Helpers.Log("Example {0}", 3);
            Helpers.Log("Section: {0}", "Blocks and inlines - Precedence");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "- `one\n- two`");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // This means that parsing can proceed in two steps:  first, the block
        // structure of the document can be discerned; second, text lines inside
        // paragraphs, headers, and other block constructs can be parsed for inline
        // structure.  The second step requires information about link reference
        // definitions that will be available only at the end of the first
        // step.  Note that the first step requires processing lines in sequence,
        // but the second can be parallelized, since the inline parsing of
        // one block element does not affect the inline parsing of any other.
        //
        // ## Container blocks and leaf blocks
        //
        // We can divide blocks into two types:
        // [container blocks](#container-block), <a id="container-block"></a>
        // which can contain other blocks, and [leaf blocks](#leaf-block),
        // <a id="leaf-block"></a> which cannot.
        //
        // # Leaf blocks
        //
        // This section describes the different kinds of leaf block that make up a
        // Markdown document.
        //
        // ## Horizontal rules
        //
        // A line consisting of 0-3 spaces of indentation, followed by a sequence
        // of three or more matching `-`, `_`, or `*` characters, each followed
        // optionally by any number of spaces, forms a [horizontal
        // rule](#horizontal-rule). <a id="horizontal-rule"></a>
        [TestMethod]
        [TestCategory("Leaf blocks - Horizontal rules")]
        //[Timeout(1000)]
        public void Example004()
        {
            // Example 4
            // Section: Leaf blocks - Horizontal rules
            //
            // The following CommonMark:
            //     ***
            //     ---
            //     ___
            //
            // Should be rendered as:
            //     <hr />
            //     <hr />
            //     <hr />

            // Arrange
            var commonMark = Helpers.Normalize("***\n---\n___");
            var expected = Helpers.Normalize("<hr />\n<hr />\n<hr />");
            Helpers.Log("Example {0}", 4);
            Helpers.Log("Section: {0}", "Leaf blocks - Horizontal rules");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "***\n---\n___");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // Wrong characters:
        [TestMethod]
        [TestCategory("Leaf blocks - Horizontal rules")]
        //[Timeout(1000)]
        public void Example005()
        {
            // Example 5
            // Section: Leaf blocks - Horizontal rules
            //
            // The following CommonMark:
            //     +++
            //
            // Should be rendered as:
            //     <p>+++</p>

            // Arrange
            var commonMark = Helpers.Normalize("+++");
            var expected = Helpers.Normalize("<p>+++</p>");
            Helpers.Log("Example {0}", 5);
            Helpers.Log("Section: {0}", "Leaf blocks - Horizontal rules");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "+++");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        [TestMethod]
        [TestCategory("Leaf blocks - Horizontal rules")]
        //[Timeout(1000)]
        public void Example006()
        {
            // Example 6
            // Section: Leaf blocks - Horizontal rules
            //
            // The following CommonMark:
            //     ===
            //
            // Should be rendered as:
            //     <p>===</p>

            // Arrange
            var commonMark = Helpers.Normalize("===");
            var expected = Helpers.Normalize("<p>===</p>");
            Helpers.Log("Example {0}", 6);
            Helpers.Log("Section: {0}", "Leaf blocks - Horizontal rules");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "===");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // Not enough characters:
        [TestMethod]
        [TestCategory("Leaf blocks - Horizontal rules")]
        //[Timeout(1000)]
        public void Example007()
        {
            // Example 7
            // Section: Leaf blocks - Horizontal rules
            //
            // The following CommonMark:
            //     --
            //     **
            //     __
            //
            // Should be rendered as:
            //     <p>--
            //     **
            //     __</p>

            // Arrange
            var commonMark = Helpers.Normalize("--\n**\n__");
            var expected = Helpers.Normalize("<p>--\n**\n__</p>");
            Helpers.Log("Example {0}", 7);
            Helpers.Log("Section: {0}", "Leaf blocks - Horizontal rules");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "--\n**\n__");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // One to three spaces indent are allowed:
        [TestMethod]
        [TestCategory("Leaf blocks - Horizontal rules")]
        //[Timeout(1000)]
        public void Example008()
        {
            // Example 8
            // Section: Leaf blocks - Horizontal rules
            //
            // The following CommonMark:
            //      ***
            //       ***
            //        ***
            //
            // Should be rendered as:
            //     <hr />
            //     <hr />
            //     <hr />

            // Arrange
            var commonMark = Helpers.Normalize(" ***\n  ***\n   ***");
            var expected = Helpers.Normalize("<hr />\n<hr />\n<hr />");
            Helpers.Log("Example {0}", 8);
            Helpers.Log("Section: {0}", "Leaf blocks - Horizontal rules");
            Helpers.Log();
            Helpers.LogValue("CommonMark", " ***\n  ***\n   ***");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // Four spaces is too many:
        [TestMethod]
        [TestCategory("Leaf blocks - Horizontal rules")]
        //[Timeout(1000)]
        public void Example009()
        {
            // Example 9
            // Section: Leaf blocks - Horizontal rules
            //
            // The following CommonMark:
            //         ***
            //
            // Should be rendered as:
            //     <pre><code>***
            //     </code></pre>

            // Arrange
            var commonMark = Helpers.Normalize("    ***");
            var expected = Helpers.Normalize("<pre><code>***\n</code></pre>");
            Helpers.Log("Example {0}", 9);
            Helpers.Log("Section: {0}", "Leaf blocks - Horizontal rules");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "    ***");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        [TestMethod]
        [TestCategory("Leaf blocks - Horizontal rules")]
        //[Timeout(1000)]
        public void Example010()
        {
            // Example 10
            // Section: Leaf blocks - Horizontal rules
            //
            // The following CommonMark:
            //     Foo
            //         ***
            //
            // Should be rendered as:
            //     <p>Foo
            //     ***</p>

            // Arrange
            var commonMark = Helpers.Normalize("Foo\n    ***");
            var expected = Helpers.Normalize("<p>Foo\n***</p>");
            Helpers.Log("Example {0}", 10);
            Helpers.Log("Section: {0}", "Leaf blocks - Horizontal rules");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "Foo\n    ***");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // More than three characters may be used:
        [TestMethod]
        [TestCategory("Leaf blocks - Horizontal rules")]
        //[Timeout(1000)]
        public void Example011()
        {
            // Example 11
            // Section: Leaf blocks - Horizontal rules
            //
            // The following CommonMark:
            //     _____________________________________
            //
            // Should be rendered as:
            //     <hr />

            // Arrange
            var commonMark = Helpers.Normalize("_____________________________________");
            var expected = Helpers.Normalize("<hr />");
            Helpers.Log("Example {0}", 11);
            Helpers.Log("Section: {0}", "Leaf blocks - Horizontal rules");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "_____________________________________");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // Spaces are allowed between the characters:
        [TestMethod]
        [TestCategory("Leaf blocks - Horizontal rules")]
        //[Timeout(1000)]
        public void Example012()
        {
            // Example 12
            // Section: Leaf blocks - Horizontal rules
            //
            // The following CommonMark:
            //      - - -
            //
            // Should be rendered as:
            //     <hr />

            // Arrange
            var commonMark = Helpers.Normalize(" - - -");
            var expected = Helpers.Normalize("<hr />");
            Helpers.Log("Example {0}", 12);
            Helpers.Log("Section: {0}", "Leaf blocks - Horizontal rules");
            Helpers.Log();
            Helpers.LogValue("CommonMark", " - - -");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        [TestMethod]
        [TestCategory("Leaf blocks - Horizontal rules")]
        //[Timeout(1000)]
        public void Example013()
        {
            // Example 13
            // Section: Leaf blocks - Horizontal rules
            //
            // The following CommonMark:
            //      **  * ** * ** * **
            //
            // Should be rendered as:
            //     <hr />

            // Arrange
            var commonMark = Helpers.Normalize(" **  * ** * ** * **");
            var expected = Helpers.Normalize("<hr />");
            Helpers.Log("Example {0}", 13);
            Helpers.Log("Section: {0}", "Leaf blocks - Horizontal rules");
            Helpers.Log();
            Helpers.LogValue("CommonMark", " **  * ** * ** * **");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        [TestMethod]
        [TestCategory("Leaf blocks - Horizontal rules")]
        //[Timeout(1000)]
        public void Example014()
        {
            // Example 14
            // Section: Leaf blocks - Horizontal rules
            //
            // The following CommonMark:
            //     -     -      -      -
            //
            // Should be rendered as:
            //     <hr />

            // Arrange
            var commonMark = Helpers.Normalize("-     -      -      -");
            var expected = Helpers.Normalize("<hr />");
            Helpers.Log("Example {0}", 14);
            Helpers.Log("Section: {0}", "Leaf blocks - Horizontal rules");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "-     -      -      -");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // Spaces are allowed at the end:
        [TestMethod]
        [TestCategory("Leaf blocks - Horizontal rules")]
        //[Timeout(1000)]
        public void Example015()
        {
            // Example 15
            // Section: Leaf blocks - Horizontal rules
            //
            // The following CommonMark:
            //     - - - -    
            //
            // Should be rendered as:
            //     <hr />

            // Arrange
            var commonMark = Helpers.Normalize("- - - -    ");
            var expected = Helpers.Normalize("<hr />");
            Helpers.Log("Example {0}", 15);
            Helpers.Log("Section: {0}", "Leaf blocks - Horizontal rules");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "- - - -    ");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // However, no other characters may occur at the end or the
        // beginning:
        [TestMethod]
        [TestCategory("Leaf blocks - Horizontal rules")]
        //[Timeout(1000)]
        public void Example016()
        {
            // Example 16
            // Section: Leaf blocks - Horizontal rules
            //
            // The following CommonMark:
            //     _ _ _ _ a
            //     
            //     a------
            //
            // Should be rendered as:
            //     <p>_ _ _ _ a</p>
            //     <p>a------</p>

            // Arrange
            var commonMark = Helpers.Normalize("_ _ _ _ a\n\na------");
            var expected = Helpers.Normalize("<p>_ _ _ _ a</p>\n<p>a------</p>");
            Helpers.Log("Example {0}", 16);
            Helpers.Log("Section: {0}", "Leaf blocks - Horizontal rules");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "_ _ _ _ a\n\na------");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // It is required that all of the non-space characters be the same.
        // So, this is not a horizontal rule:
        [TestMethod]
        [TestCategory("Leaf blocks - Horizontal rules")]
        //[Timeout(1000)]
        public void Example017()
        {
            // Example 17
            // Section: Leaf blocks - Horizontal rules
            //
            // The following CommonMark:
            //      *-*
            //
            // Should be rendered as:
            //     <p><em>-</em></p>

            // Arrange
            var commonMark = Helpers.Normalize(" *-*");
            var expected = Helpers.Normalize("<p><em>-</em></p>");
            Helpers.Log("Example {0}", 17);
            Helpers.Log("Section: {0}", "Leaf blocks - Horizontal rules");
            Helpers.Log();
            Helpers.LogValue("CommonMark", " *-*");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // Horizontal rules do not need blank lines before or after:
        [TestMethod]
        [TestCategory("Leaf blocks - Horizontal rules")]
        //[Timeout(1000)]
        public void Example018()
        {
            // Example 18
            // Section: Leaf blocks - Horizontal rules
            //
            // The following CommonMark:
            //     - foo
            //     ***
            //     - bar
            //
            // Should be rendered as:
            //     <ul>
            //     <li>foo</li>
            //     </ul>
            //     <hr />
            //     <ul>
            //     <li>bar</li>
            //     </ul>

            // Arrange
            var commonMark = Helpers.Normalize("- foo\n***\n- bar");
            var expected = Helpers.Normalize("<ul>\n<li>foo</li>\n</ul>\n<hr />\n<ul>\n<li>bar</li>\n</ul>");
            Helpers.Log("Example {0}", 18);
            Helpers.Log("Section: {0}", "Leaf blocks - Horizontal rules");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "- foo\n***\n- bar");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // Horizontal rules can interrupt a paragraph:
        [TestMethod]
        [TestCategory("Leaf blocks - Horizontal rules")]
        //[Timeout(1000)]
        public void Example019()
        {
            // Example 19
            // Section: Leaf blocks - Horizontal rules
            //
            // The following CommonMark:
            //     Foo
            //     ***
            //     bar
            //
            // Should be rendered as:
            //     <p>Foo</p>
            //     <hr />
            //     <p>bar</p>

            // Arrange
            var commonMark = Helpers.Normalize("Foo\n***\nbar");
            var expected = Helpers.Normalize("<p>Foo</p>\n<hr />\n<p>bar</p>");
            Helpers.Log("Example {0}", 19);
            Helpers.Log("Section: {0}", "Leaf blocks - Horizontal rules");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "Foo\n***\nbar");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // Note, however, that this is a setext header, not a paragraph followed
        // by a horizontal rule:
        [TestMethod]
        [TestCategory("Leaf blocks - Horizontal rules")]
        //[Timeout(1000)]
        public void Example020()
        {
            // Example 20
            // Section: Leaf blocks - Horizontal rules
            //
            // The following CommonMark:
            //     Foo
            //     ---
            //     bar
            //
            // Should be rendered as:
            //     <h2>Foo</h2>
            //     <p>bar</p>

            // Arrange
            var commonMark = Helpers.Normalize("Foo\n---\nbar");
            var expected = Helpers.Normalize("<h2>Foo</h2>\n<p>bar</p>");
            Helpers.Log("Example {0}", 20);
            Helpers.Log("Section: {0}", "Leaf blocks - Horizontal rules");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "Foo\n---\nbar");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // When both a horizontal rule and a list item are possible
        // interpretations of a line, the horizontal rule is preferred:
        [TestMethod]
        [TestCategory("Leaf blocks - Horizontal rules")]
        //[Timeout(1000)]
        public void Example021()
        {
            // Example 21
            // Section: Leaf blocks - Horizontal rules
            //
            // The following CommonMark:
            //     * Foo
            //     * * *
            //     * Bar
            //
            // Should be rendered as:
            //     <ul>
            //     <li>Foo</li>
            //     </ul>
            //     <hr />
            //     <ul>
            //     <li>Bar</li>
            //     </ul>

            // Arrange
            var commonMark = Helpers.Normalize("* Foo\n* * *\n* Bar");
            var expected = Helpers.Normalize("<ul>\n<li>Foo</li>\n</ul>\n<hr />\n<ul>\n<li>Bar</li>\n</ul>");
            Helpers.Log("Example {0}", 21);
            Helpers.Log("Section: {0}", "Leaf blocks - Horizontal rules");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "* Foo\n* * *\n* Bar");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // If you want a horizontal rule in a list item, use a different bullet:
        [TestMethod]
        [TestCategory("Leaf blocks - Horizontal rules")]
        //[Timeout(1000)]
        public void Example022()
        {
            // Example 22
            // Section: Leaf blocks - Horizontal rules
            //
            // The following CommonMark:
            //     - Foo
            //     - * * *
            //
            // Should be rendered as:
            //     <ul>
            //     <li>Foo</li>
            //     <li><hr /></li>
            //     </ul>

            // Arrange
            var commonMark = Helpers.Normalize("- Foo\n- * * *");
            var expected = Helpers.Normalize("<ul>\n<li>Foo</li>\n<li><hr /></li>\n</ul>");
            Helpers.Log("Example {0}", 22);
            Helpers.Log("Section: {0}", "Leaf blocks - Horizontal rules");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "- Foo\n- * * *");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // ## ATX headers
        //
        // An [ATX header](#atx-header) <a id="atx-header"></a>
        // consists of a string of characters, parsed as inline content, between an
        // opening sequence of 1--6 unescaped `#` characters and an optional
        // closing sequence of any number of `#` characters.  The opening sequence
        // of `#` characters cannot be followed directly by a nonspace character.
        // The closing `#` characters may be followed by spaces only.  The opening
        // `#` character may be indented 0-3 spaces.  The raw contents of the
        // header are stripped of leading and trailing spaces before being parsed
        // as inline content.  The header level is equal to the number of `#`
        // characters in the opening sequence.
        //
        // Simple headers:
        [TestMethod]
        [TestCategory("Leaf blocks - ATX headers")]
        //[Timeout(1000)]
        public void Example023()
        {
            // Example 23
            // Section: Leaf blocks - ATX headers
            //
            // The following CommonMark:
            //     # foo
            //     ## foo
            //     ### foo
            //     #### foo
            //     ##### foo
            //     ###### foo
            //
            // Should be rendered as:
            //     <h1>foo</h1>
            //     <h2>foo</h2>
            //     <h3>foo</h3>
            //     <h4>foo</h4>
            //     <h5>foo</h5>
            //     <h6>foo</h6>

            // Arrange
            var commonMark = Helpers.Normalize("# foo\n## foo\n### foo\n#### foo\n##### foo\n###### foo");
            var expected = Helpers.Normalize("<h1>foo</h1>\n<h2>foo</h2>\n<h3>foo</h3>\n<h4>foo</h4>\n<h5>foo</h5>\n<h6>foo</h6>");
            Helpers.Log("Example {0}", 23);
            Helpers.Log("Section: {0}", "Leaf blocks - ATX headers");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "# foo\n## foo\n### foo\n#### foo\n##### foo\n###### foo");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // More than six `#` characters is not a header:
        [TestMethod]
        [TestCategory("Leaf blocks - ATX headers")]
        //[Timeout(1000)]
        public void Example024()
        {
            // Example 24
            // Section: Leaf blocks - ATX headers
            //
            // The following CommonMark:
            //     ####### foo
            //
            // Should be rendered as:
            //     <p>####### foo</p>

            // Arrange
            var commonMark = Helpers.Normalize("####### foo");
            var expected = Helpers.Normalize("<p>####### foo</p>");
            Helpers.Log("Example {0}", 24);
            Helpers.Log("Section: {0}", "Leaf blocks - ATX headers");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "####### foo");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // A space is required between the `#` characters and the header's
        // contents.  Note that many implementations currently do not require
        // the space.  However, the space was required by the [original ATX
        // implementation](http://www.aaronsw.com/2002/atx/atx.py), and it helps
        // prevent things like the following from being parsed as headers:
        [TestMethod]
        [TestCategory("Leaf blocks - ATX headers")]
        //[Timeout(1000)]
        public void Example025()
        {
            // Example 25
            // Section: Leaf blocks - ATX headers
            //
            // The following CommonMark:
            //     #5 bolt
            //
            // Should be rendered as:
            //     <p>#5 bolt</p>

            // Arrange
            var commonMark = Helpers.Normalize("#5 bolt");
            var expected = Helpers.Normalize("<p>#5 bolt</p>");
            Helpers.Log("Example {0}", 25);
            Helpers.Log("Section: {0}", "Leaf blocks - ATX headers");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "#5 bolt");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // This is not a header, because the first `#` is escaped:
        [TestMethod]
        [TestCategory("Leaf blocks - ATX headers")]
        //[Timeout(1000)]
        public void Example026()
        {
            // Example 26
            // Section: Leaf blocks - ATX headers
            //
            // The following CommonMark:
            //     \## foo
            //
            // Should be rendered as:
            //     <p>## foo</p>

            // Arrange
            var commonMark = Helpers.Normalize("\\## foo");
            var expected = Helpers.Normalize("<p>## foo</p>");
            Helpers.Log("Example {0}", 26);
            Helpers.Log("Section: {0}", "Leaf blocks - ATX headers");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "\\## foo");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // Contents are parsed as inlines:
        [TestMethod]
        [TestCategory("Leaf blocks - ATX headers")]
        //[Timeout(1000)]
        public void Example027()
        {
            // Example 27
            // Section: Leaf blocks - ATX headers
            //
            // The following CommonMark:
            //     # foo *bar* \*baz\*
            //
            // Should be rendered as:
            //     <h1>foo <em>bar</em> *baz*</h1>

            // Arrange
            var commonMark = Helpers.Normalize("# foo *bar* \\*baz\\*");
            var expected = Helpers.Normalize("<h1>foo <em>bar</em> *baz*</h1>");
            Helpers.Log("Example {0}", 27);
            Helpers.Log("Section: {0}", "Leaf blocks - ATX headers");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "# foo *bar* \\*baz\\*");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // Leading and trailing blanks are ignored in parsing inline content:
        [TestMethod]
        [TestCategory("Leaf blocks - ATX headers")]
        //[Timeout(1000)]
        public void Example028()
        {
            // Example 28
            // Section: Leaf blocks - ATX headers
            //
            // The following CommonMark:
            //     #                  foo                     
            //
            // Should be rendered as:
            //     <h1>foo</h1>

            // Arrange
            var commonMark = Helpers.Normalize("#                  foo                     ");
            var expected = Helpers.Normalize("<h1>foo</h1>");
            Helpers.Log("Example {0}", 28);
            Helpers.Log("Section: {0}", "Leaf blocks - ATX headers");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "#                  foo                     ");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // One to three spaces indentation are allowed:
        [TestMethod]
        [TestCategory("Leaf blocks - ATX headers")]
        //[Timeout(1000)]
        public void Example029()
        {
            // Example 29
            // Section: Leaf blocks - ATX headers
            //
            // The following CommonMark:
            //      ### foo
            //       ## foo
            //        # foo
            //
            // Should be rendered as:
            //     <h3>foo</h3>
            //     <h2>foo</h2>
            //     <h1>foo</h1>

            // Arrange
            var commonMark = Helpers.Normalize(" ### foo\n  ## foo\n   # foo");
            var expected = Helpers.Normalize("<h3>foo</h3>\n<h2>foo</h2>\n<h1>foo</h1>");
            Helpers.Log("Example {0}", 29);
            Helpers.Log("Section: {0}", "Leaf blocks - ATX headers");
            Helpers.Log();
            Helpers.LogValue("CommonMark", " ### foo\n  ## foo\n   # foo");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // Four spaces are too much:
        [TestMethod]
        [TestCategory("Leaf blocks - ATX headers")]
        //[Timeout(1000)]
        public void Example030()
        {
            // Example 30
            // Section: Leaf blocks - ATX headers
            //
            // The following CommonMark:
            //         # foo
            //
            // Should be rendered as:
            //     <pre><code># foo
            //     </code></pre>

            // Arrange
            var commonMark = Helpers.Normalize("    # foo");
            var expected = Helpers.Normalize("<pre><code># foo\n</code></pre>");
            Helpers.Log("Example {0}", 30);
            Helpers.Log("Section: {0}", "Leaf blocks - ATX headers");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "    # foo");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        [TestMethod]
        [TestCategory("Leaf blocks - ATX headers")]
        //[Timeout(1000)]
        public void Example031()
        {
            // Example 31
            // Section: Leaf blocks - ATX headers
            //
            // The following CommonMark:
            //     foo
            //         # bar
            //
            // Should be rendered as:
            //     <p>foo
            //     # bar</p>

            // Arrange
            var commonMark = Helpers.Normalize("foo\n    # bar");
            var expected = Helpers.Normalize("<p>foo\n# bar</p>");
            Helpers.Log("Example {0}", 31);
            Helpers.Log("Section: {0}", "Leaf blocks - ATX headers");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "foo\n    # bar");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // A closing sequence of `#` characters is optional:
        [TestMethod]
        [TestCategory("Leaf blocks - ATX headers")]
        //[Timeout(1000)]
        public void Example032()
        {
            // Example 32
            // Section: Leaf blocks - ATX headers
            //
            // The following CommonMark:
            //     ## foo ##
            //       ###   bar    ###
            //
            // Should be rendered as:
            //     <h2>foo</h2>
            //     <h3>bar</h3>

            // Arrange
            var commonMark = Helpers.Normalize("## foo ##\n  ###   bar    ###");
            var expected = Helpers.Normalize("<h2>foo</h2>\n<h3>bar</h3>");
            Helpers.Log("Example {0}", 32);
            Helpers.Log("Section: {0}", "Leaf blocks - ATX headers");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "## foo ##\n  ###   bar    ###");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // It need not be the same length as the opening sequence:
        [TestMethod]
        [TestCategory("Leaf blocks - ATX headers")]
        //[Timeout(1000)]
        public void Example033()
        {
            // Example 33
            // Section: Leaf blocks - ATX headers
            //
            // The following CommonMark:
            //     # foo ##################################
            //     ##### foo ##
            //
            // Should be rendered as:
            //     <h1>foo</h1>
            //     <h5>foo</h5>

            // Arrange
            var commonMark = Helpers.Normalize("# foo ##################################\n##### foo ##");
            var expected = Helpers.Normalize("<h1>foo</h1>\n<h5>foo</h5>");
            Helpers.Log("Example {0}", 33);
            Helpers.Log("Section: {0}", "Leaf blocks - ATX headers");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "# foo ##################################\n##### foo ##");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // Spaces are allowed after the closing sequence:
        [TestMethod]
        [TestCategory("Leaf blocks - ATX headers")]
        //[Timeout(1000)]
        public void Example034()
        {
            // Example 34
            // Section: Leaf blocks - ATX headers
            //
            // The following CommonMark:
            //     ### foo ###     
            //
            // Should be rendered as:
            //     <h3>foo</h3>

            // Arrange
            var commonMark = Helpers.Normalize("### foo ###     ");
            var expected = Helpers.Normalize("<h3>foo</h3>");
            Helpers.Log("Example {0}", 34);
            Helpers.Log("Section: {0}", "Leaf blocks - ATX headers");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "### foo ###     ");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // A sequence of `#` characters with a nonspace character following it
        // is not a closing sequence, but counts as part of the contents of the
        // header:
        [TestMethod]
        [TestCategory("Leaf blocks - ATX headers")]
        //[Timeout(1000)]
        public void Example035()
        {
            // Example 35
            // Section: Leaf blocks - ATX headers
            //
            // The following CommonMark:
            //     ### foo ### b
            //
            // Should be rendered as:
            //     <h3>foo ### b</h3>

            // Arrange
            var commonMark = Helpers.Normalize("### foo ### b");
            var expected = Helpers.Normalize("<h3>foo ### b</h3>");
            Helpers.Log("Example {0}", 35);
            Helpers.Log("Section: {0}", "Leaf blocks - ATX headers");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "### foo ### b");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // Backslash-escaped `#` characters do not count as part
        // of the closing sequence:
        [TestMethod]
        [TestCategory("Leaf blocks - ATX headers")]
        //[Timeout(1000)]
        public void Example036()
        {
            // Example 36
            // Section: Leaf blocks - ATX headers
            //
            // The following CommonMark:
            //     ### foo \###
            //     ## foo \#\##
            //     # foo \#
            //
            // Should be rendered as:
            //     <h3>foo #</h3>
            //     <h2>foo ##</h2>
            //     <h1>foo #</h1>

            // Arrange
            var commonMark = Helpers.Normalize("### foo \\###\n## foo \\#\\##\n# foo \\#");
            var expected = Helpers.Normalize("<h3>foo #</h3>\n<h2>foo ##</h2>\n<h1>foo #</h1>");
            Helpers.Log("Example {0}", 36);
            Helpers.Log("Section: {0}", "Leaf blocks - ATX headers");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "### foo \\###\n## foo \\#\\##\n# foo \\#");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // ATX headers need not be separated from surrounding content by blank
        // lines, and they can interrupt paragraphs:
        [TestMethod]
        [TestCategory("Leaf blocks - ATX headers")]
        //[Timeout(1000)]
        public void Example037()
        {
            // Example 37
            // Section: Leaf blocks - ATX headers
            //
            // The following CommonMark:
            //     ****
            //     ## foo
            //     ****
            //
            // Should be rendered as:
            //     <hr />
            //     <h2>foo</h2>
            //     <hr />

            // Arrange
            var commonMark = Helpers.Normalize("****\n## foo\n****");
            var expected = Helpers.Normalize("<hr />\n<h2>foo</h2>\n<hr />");
            Helpers.Log("Example {0}", 37);
            Helpers.Log("Section: {0}", "Leaf blocks - ATX headers");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "****\n## foo\n****");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        [TestMethod]
        [TestCategory("Leaf blocks - ATX headers")]
        //[Timeout(1000)]
        public void Example038()
        {
            // Example 38
            // Section: Leaf blocks - ATX headers
            //
            // The following CommonMark:
            //     Foo bar
            //     # baz
            //     Bar foo
            //
            // Should be rendered as:
            //     <p>Foo bar</p>
            //     <h1>baz</h1>
            //     <p>Bar foo</p>

            // Arrange
            var commonMark = Helpers.Normalize("Foo bar\n# baz\nBar foo");
            var expected = Helpers.Normalize("<p>Foo bar</p>\n<h1>baz</h1>\n<p>Bar foo</p>");
            Helpers.Log("Example {0}", 38);
            Helpers.Log("Section: {0}", "Leaf blocks - ATX headers");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "Foo bar\n# baz\nBar foo");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // ATX headers can be empty:
        [TestMethod]
        [TestCategory("Leaf blocks - ATX headers")]
        //[Timeout(1000)]
        public void Example039()
        {
            // Example 39
            // Section: Leaf blocks - ATX headers
            //
            // The following CommonMark:
            //     ## 
            //     #
            //     ### ###
            //
            // Should be rendered as:
            //     <h2></h2>
            //     <h1></h1>
            //     <h3></h3>

            // Arrange
            var commonMark = Helpers.Normalize("## \n#\n### ###");
            var expected = Helpers.Normalize("<h2></h2>\n<h1></h1>\n<h3></h3>");
            Helpers.Log("Example {0}", 39);
            Helpers.Log("Section: {0}", "Leaf blocks - ATX headers");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "## \n#\n### ###");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // ## Setext headers
        //
        // A [setext header](#setext-header) <a id="setext-header"></a>
        // consists of a line of text, containing at least one nonspace character,
        // with no more than 3 spaces indentation, followed by a [setext header
        // underline](#setext-header-underline).  A [setext header
        // underline](#setext-header-underline) <a id="setext-header-underline"></a>
        // is a sequence of `=` characters or a sequence of `-` characters, with no
        // more than 3 spaces indentation and any number of trailing
        // spaces.  The header is a level 1 header if `=` characters are used, and
        // a level 2 header if `-` characters are used.  The contents of the header
        // are the result of parsing the first line as Markdown inline content.
        //
        // In general, a setext header need not be preceded or followed by a
        // blank line.  However, it cannot interrupt a paragraph, so when a
        // setext header comes after a paragraph, a blank line is needed between
        // them.
        //
        // Simple examples:
        [TestMethod]
        [TestCategory("Leaf blocks - Setext headers")]
        //[Timeout(1000)]
        public void Example040()
        {
            // Example 40
            // Section: Leaf blocks - Setext headers
            //
            // The following CommonMark:
            //     Foo *bar*
            //     =========
            //     
            //     Foo *bar*
            //     ---------
            //
            // Should be rendered as:
            //     <h1>Foo <em>bar</em></h1>
            //     <h2>Foo <em>bar</em></h2>

            // Arrange
            var commonMark = Helpers.Normalize("Foo *bar*\n=========\n\nFoo *bar*\n---------");
            var expected = Helpers.Normalize("<h1>Foo <em>bar</em></h1>\n<h2>Foo <em>bar</em></h2>");
            Helpers.Log("Example {0}", 40);
            Helpers.Log("Section: {0}", "Leaf blocks - Setext headers");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "Foo *bar*\n=========\n\nFoo *bar*\n---------");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // The underlining can be any length:
        [TestMethod]
        [TestCategory("Leaf blocks - Setext headers")]
        //[Timeout(1000)]
        public void Example041()
        {
            // Example 41
            // Section: Leaf blocks - Setext headers
            //
            // The following CommonMark:
            //     Foo
            //     -------------------------
            //     
            //     Foo
            //     =
            //
            // Should be rendered as:
            //     <h2>Foo</h2>
            //     <h1>Foo</h1>

            // Arrange
            var commonMark = Helpers.Normalize("Foo\n-------------------------\n\nFoo\n=");
            var expected = Helpers.Normalize("<h2>Foo</h2>\n<h1>Foo</h1>");
            Helpers.Log("Example {0}", 41);
            Helpers.Log("Section: {0}", "Leaf blocks - Setext headers");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "Foo\n-------------------------\n\nFoo\n=");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // The header content can be indented up to three spaces, and need
        // not line up with the underlining:
        [TestMethod]
        [TestCategory("Leaf blocks - Setext headers")]
        //[Timeout(1000)]
        public void Example042()
        {
            // Example 42
            // Section: Leaf blocks - Setext headers
            //
            // The following CommonMark:
            //        Foo
            //     ---
            //     
            //       Foo
            //     -----
            //     
            //       Foo
            //       ===
            //
            // Should be rendered as:
            //     <h2>Foo</h2>
            //     <h2>Foo</h2>
            //     <h1>Foo</h1>

            // Arrange
            var commonMark = Helpers.Normalize("   Foo\n---\n\n  Foo\n-----\n\n  Foo\n  ===");
            var expected = Helpers.Normalize("<h2>Foo</h2>\n<h2>Foo</h2>\n<h1>Foo</h1>");
            Helpers.Log("Example {0}", 42);
            Helpers.Log("Section: {0}", "Leaf blocks - Setext headers");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "   Foo\n---\n\n  Foo\n-----\n\n  Foo\n  ===");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // Four spaces indent is too much:
        [TestMethod]
        [TestCategory("Leaf blocks - Setext headers")]
        //[Timeout(1000)]
        public void Example043()
        {
            // Example 43
            // Section: Leaf blocks - Setext headers
            //
            // The following CommonMark:
            //         Foo
            //         ---
            //     
            //         Foo
            //     ---
            //
            // Should be rendered as:
            //     <pre><code>Foo
            //     ---
            //     
            //     Foo
            //     </code></pre>
            //     <hr />

            // Arrange
            var commonMark = Helpers.Normalize("    Foo\n    ---\n\n    Foo\n---");
            var expected = Helpers.Normalize("<pre><code>Foo\n---\n\nFoo\n</code></pre>\n<hr />");
            Helpers.Log("Example {0}", 43);
            Helpers.Log("Section: {0}", "Leaf blocks - Setext headers");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "    Foo\n    ---\n\n    Foo\n---");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // The setext header underline can be indented up to three spaces, and
        // may have trailing spaces:
        [TestMethod]
        [TestCategory("Leaf blocks - Setext headers")]
        //[Timeout(1000)]
        public void Example044()
        {
            // Example 44
            // Section: Leaf blocks - Setext headers
            //
            // The following CommonMark:
            //     Foo
            //        ----      
            //
            // Should be rendered as:
            //     <h2>Foo</h2>

            // Arrange
            var commonMark = Helpers.Normalize("Foo\n   ----      ");
            var expected = Helpers.Normalize("<h2>Foo</h2>");
            Helpers.Log("Example {0}", 44);
            Helpers.Log("Section: {0}", "Leaf blocks - Setext headers");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "Foo\n   ----      ");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // Four spaces is too much:
        [TestMethod]
        [TestCategory("Leaf blocks - Setext headers")]
        //[Timeout(1000)]
        public void Example045()
        {
            // Example 45
            // Section: Leaf blocks - Setext headers
            //
            // The following CommonMark:
            //     Foo
            //          ---
            //
            // Should be rendered as:
            //     <p>Foo
            //     ---</p>

            // Arrange
            var commonMark = Helpers.Normalize("Foo\n     ---");
            var expected = Helpers.Normalize("<p>Foo\n---</p>");
            Helpers.Log("Example {0}", 45);
            Helpers.Log("Section: {0}", "Leaf blocks - Setext headers");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "Foo\n     ---");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // The setext header underline cannot contain internal spaces:
        [TestMethod]
        [TestCategory("Leaf blocks - Setext headers")]
        //[Timeout(1000)]
        public void Example046()
        {
            // Example 46
            // Section: Leaf blocks - Setext headers
            //
            // The following CommonMark:
            //     Foo
            //     = =
            //     
            //     Foo
            //     --- -
            //
            // Should be rendered as:
            //     <p>Foo
            //     = =</p>
            //     <p>Foo</p>
            //     <hr />

            // Arrange
            var commonMark = Helpers.Normalize("Foo\n= =\n\nFoo\n--- -");
            var expected = Helpers.Normalize("<p>Foo\n= =</p>\n<p>Foo</p>\n<hr />");
            Helpers.Log("Example {0}", 46);
            Helpers.Log("Section: {0}", "Leaf blocks - Setext headers");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "Foo\n= =\n\nFoo\n--- -");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // Trailing spaces in the content line do not cause a line break:
        [TestMethod]
        [TestCategory("Leaf blocks - Setext headers")]
        //[Timeout(1000)]
        public void Example047()
        {
            // Example 47
            // Section: Leaf blocks - Setext headers
            //
            // The following CommonMark:
            //     Foo  
            //     -----
            //
            // Should be rendered as:
            //     <h2>Foo</h2>

            // Arrange
            var commonMark = Helpers.Normalize("Foo  \n-----");
            var expected = Helpers.Normalize("<h2>Foo</h2>");
            Helpers.Log("Example {0}", 47);
            Helpers.Log("Section: {0}", "Leaf blocks - Setext headers");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "Foo  \n-----");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // Nor does a backslash at the end:
        [TestMethod]
        [TestCategory("Leaf blocks - Setext headers")]
        //[Timeout(1000)]
        public void Example048()
        {
            // Example 48
            // Section: Leaf blocks - Setext headers
            //
            // The following CommonMark:
            //     Foo\
            //     ----
            //
            // Should be rendered as:
            //     <h2>Foo\</h2>

            // Arrange
            var commonMark = Helpers.Normalize("Foo\\\n----");
            var expected = Helpers.Normalize("<h2>Foo\\</h2>");
            Helpers.Log("Example {0}", 48);
            Helpers.Log("Section: {0}", "Leaf blocks - Setext headers");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "Foo\\\n----");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // Since indicators of block structure take precedence over
        // indicators of inline structure, the following are setext headers:
        [TestMethod]
        [TestCategory("Leaf blocks - Setext headers")]
        //[Timeout(1000)]
        public void Example049()
        {
            // Example 49
            // Section: Leaf blocks - Setext headers
            //
            // The following CommonMark:
            //     `Foo
            //     ----
            //     `
            //     
            //     <a title="a lot
            //     ---
            //     of dashes"/>
            //
            // Should be rendered as:
            //     <h2>`Foo</h2>
            //     <p>`</p>
            //     <h2>&lt;a title=&quot;a lot</h2>
            //     <p>of dashes&quot;/&gt;</p>

            // Arrange
            var commonMark = Helpers.Normalize("`Foo\n----\n`\n\n<a title=\"a lot\n---\nof dashes\"/>");
            var expected = Helpers.Normalize("<h2>`Foo</h2>\n<p>`</p>\n<h2>&lt;a title=&quot;a lot</h2>\n<p>of dashes&quot;/&gt;</p>");
            Helpers.Log("Example {0}", 49);
            Helpers.Log("Section: {0}", "Leaf blocks - Setext headers");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "`Foo\n----\n`\n\n<a title=\"a lot\n---\nof dashes\"/>");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // The setext header underline cannot be a lazy line:
        [TestMethod]
        [TestCategory("Leaf blocks - Setext headers")]
        //[Timeout(1000)]
        public void Example050()
        {
            // Example 50
            // Section: Leaf blocks - Setext headers
            //
            // The following CommonMark:
            //     > Foo
            //     ---
            //
            // Should be rendered as:
            //     <blockquote>
            //     <p>Foo</p>
            //     </blockquote>
            //     <hr />

            // Arrange
            var commonMark = Helpers.Normalize("> Foo\n---");
            var expected = Helpers.Normalize("<blockquote>\n<p>Foo</p>\n</blockquote>\n<hr />");
            Helpers.Log("Example {0}", 50);
            Helpers.Log("Section: {0}", "Leaf blocks - Setext headers");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "> Foo\n---");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // A setext header cannot interrupt a paragraph:
        [TestMethod]
        [TestCategory("Leaf blocks - Setext headers")]
        //[Timeout(1000)]
        public void Example051()
        {
            // Example 51
            // Section: Leaf blocks - Setext headers
            //
            // The following CommonMark:
            //     Foo
            //     Bar
            //     ---
            //     
            //     Foo
            //     Bar
            //     ===
            //
            // Should be rendered as:
            //     <p>Foo
            //     Bar</p>
            //     <hr />
            //     <p>Foo
            //     Bar
            //     ===</p>

            // Arrange
            var commonMark = Helpers.Normalize("Foo\nBar\n---\n\nFoo\nBar\n===");
            var expected = Helpers.Normalize("<p>Foo\nBar</p>\n<hr />\n<p>Foo\nBar\n===</p>");
            Helpers.Log("Example {0}", 51);
            Helpers.Log("Section: {0}", "Leaf blocks - Setext headers");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "Foo\nBar\n---\n\nFoo\nBar\n===");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // But in general a blank line is not required before or after:
        [TestMethod]
        [TestCategory("Leaf blocks - Setext headers")]
        //[Timeout(1000)]
        public void Example052()
        {
            // Example 52
            // Section: Leaf blocks - Setext headers
            //
            // The following CommonMark:
            //     ---
            //     Foo
            //     ---
            //     Bar
            //     ---
            //     Baz
            //
            // Should be rendered as:
            //     <hr />
            //     <h2>Foo</h2>
            //     <h2>Bar</h2>
            //     <p>Baz</p>

            // Arrange
            var commonMark = Helpers.Normalize("---\nFoo\n---\nBar\n---\nBaz");
            var expected = Helpers.Normalize("<hr />\n<h2>Foo</h2>\n<h2>Bar</h2>\n<p>Baz</p>");
            Helpers.Log("Example {0}", 52);
            Helpers.Log("Section: {0}", "Leaf blocks - Setext headers");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "---\nFoo\n---\nBar\n---\nBaz");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // Setext headers cannot be empty:
        [TestMethod]
        [TestCategory("Leaf blocks - Setext headers")]
        //[Timeout(1000)]
        public void Example053()
        {
            // Example 53
            // Section: Leaf blocks - Setext headers
            //
            // The following CommonMark:
            //     ====
            //
            // Should be rendered as:
            //     <p>====</p>

            // Arrange
            var commonMark = Helpers.Normalize("====");
            var expected = Helpers.Normalize("<p>====</p>");
            Helpers.Log("Example {0}", 53);
            Helpers.Log("Section: {0}", "Leaf blocks - Setext headers");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "====");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // ## Indented code blocks
        //
        // An [indented code block](#indented-code-block)
        // <a id="indented-code-block"></a> is composed of one or more
        // [indented chunks](#indented-chunk) separated by blank lines.
        // An [indented chunk](#indented-chunk) <a id="indented-chunk"></a>
        // is a sequence of non-blank lines, each indented four or more
        // spaces.  An indented code block cannot interrupt a paragraph, so
        // if it occurs before or after a paragraph, there must be an
        // intervening blank line.  The contents of the code block are
        // the literal contents of the lines, including trailing newlines,
        // minus four spaces of indentation. An indented code block has no
        // attributes.
        [TestMethod]
        [TestCategory("Leaf blocks - Indented code blocks")]
        //[Timeout(1000)]
        public void Example054()
        {
            // Example 54
            // Section: Leaf blocks - Indented code blocks
            //
            // The following CommonMark:
            //         a simple
            //           indented code block
            //
            // Should be rendered as:
            //     <pre><code>a simple
            //       indented code block
            //     </code></pre>

            // Arrange
            var commonMark = Helpers.Normalize("    a simple\n      indented code block");
            var expected = Helpers.Normalize("<pre><code>a simple\n  indented code block\n</code></pre>");
            Helpers.Log("Example {0}", 54);
            Helpers.Log("Section: {0}", "Leaf blocks - Indented code blocks");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "    a simple\n      indented code block");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // The contents are literal text, and do not get parsed as Markdown:
        [TestMethod]
        [TestCategory("Leaf blocks - Indented code blocks")]
        //[Timeout(1000)]
        public void Example055()
        {
            // Example 55
            // Section: Leaf blocks - Indented code blocks
            //
            // The following CommonMark:
            //         <a/>
            //         *hi*
            //     
            //         - one
            //
            // Should be rendered as:
            //     <pre><code>&lt;a/&gt;
            //     *hi*
            //     
            //     - one
            //     </code></pre>

            // Arrange
            var commonMark = Helpers.Normalize("    <a/>\n    *hi*\n\n    - one");
            var expected = Helpers.Normalize("<pre><code>&lt;a/&gt;\n*hi*\n\n- one\n</code></pre>");
            Helpers.Log("Example {0}", 55);
            Helpers.Log("Section: {0}", "Leaf blocks - Indented code blocks");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "    <a/>\n    *hi*\n\n    - one");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // Here we have three chunks separated by blank lines:
        [TestMethod]
        [TestCategory("Leaf blocks - Indented code blocks")]
        //[Timeout(1000)]
        public void Example056()
        {
            // Example 56
            // Section: Leaf blocks - Indented code blocks
            //
            // The following CommonMark:
            //         chunk1
            //     
            //         chunk2
            //       
            //      
            //      
            //         chunk3
            //
            // Should be rendered as:
            //     <pre><code>chunk1
            //     
            //     chunk2
            //     
            //     
            //     
            //     chunk3
            //     </code></pre>

            // Arrange
            var commonMark = Helpers.Normalize("    chunk1\n\n    chunk2\n  \n \n \n    chunk3");
            var expected = Helpers.Normalize("<pre><code>chunk1\n\nchunk2\n\n\n\nchunk3\n</code></pre>");
            Helpers.Log("Example {0}", 56);
            Helpers.Log("Section: {0}", "Leaf blocks - Indented code blocks");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "    chunk1\n\n    chunk2\n  \n \n \n    chunk3");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // Any initial spaces beyond four will be included in the content, even
        // in interior blank lines:
        [TestMethod]
        [TestCategory("Leaf blocks - Indented code blocks")]
        //[Timeout(1000)]
        public void Example057()
        {
            // Example 57
            // Section: Leaf blocks - Indented code blocks
            //
            // The following CommonMark:
            //         chunk1
            //           
            //           chunk2
            //
            // Should be rendered as:
            //     <pre><code>chunk1
            //       
            //       chunk2
            //     </code></pre>

            // Arrange
            var commonMark = Helpers.Normalize("    chunk1\n      \n      chunk2");
            var expected = Helpers.Normalize("<pre><code>chunk1\n  \n  chunk2\n</code></pre>");
            Helpers.Log("Example {0}", 57);
            Helpers.Log("Section: {0}", "Leaf blocks - Indented code blocks");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "    chunk1\n      \n      chunk2");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // An indented code block cannot interrupt a paragraph.  (This
        // allows hanging indents and the like.)
        [TestMethod]
        [TestCategory("Leaf blocks - Indented code blocks")]
        //[Timeout(1000)]
        public void Example058()
        {
            // Example 58
            // Section: Leaf blocks - Indented code blocks
            //
            // The following CommonMark:
            //     Foo
            //         bar
            //     
            //
            // Should be rendered as:
            //     <p>Foo
            //     bar</p>

            // Arrange
            var commonMark = Helpers.Normalize("Foo\n    bar\n");
            var expected = Helpers.Normalize("<p>Foo\nbar</p>");
            Helpers.Log("Example {0}", 58);
            Helpers.Log("Section: {0}", "Leaf blocks - Indented code blocks");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "Foo\n    bar\n");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // However, any non-blank line with fewer than four leading spaces ends
        // the code block immediately.  So a paragraph may occur immediately
        // after indented code:
        [TestMethod]
        [TestCategory("Leaf blocks - Indented code blocks")]
        //[Timeout(1000)]
        public void Example059()
        {
            // Example 59
            // Section: Leaf blocks - Indented code blocks
            //
            // The following CommonMark:
            //         foo
            //     bar
            //
            // Should be rendered as:
            //     <pre><code>foo
            //     </code></pre>
            //     <p>bar</p>

            // Arrange
            var commonMark = Helpers.Normalize("    foo\nbar");
            var expected = Helpers.Normalize("<pre><code>foo\n</code></pre>\n<p>bar</p>");
            Helpers.Log("Example {0}", 59);
            Helpers.Log("Section: {0}", "Leaf blocks - Indented code blocks");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "    foo\nbar");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // And indented code can occur immediately before and after other kinds of
        // blocks:
        [TestMethod]
        [TestCategory("Leaf blocks - Indented code blocks")]
        //[Timeout(1000)]
        public void Example060()
        {
            // Example 60
            // Section: Leaf blocks - Indented code blocks
            //
            // The following CommonMark:
            //     # Header
            //         foo
            //     Header
            //     ------
            //         foo
            //     ----
            //
            // Should be rendered as:
            //     <h1>Header</h1>
            //     <pre><code>foo
            //     </code></pre>
            //     <h2>Header</h2>
            //     <pre><code>foo
            //     </code></pre>
            //     <hr />

            // Arrange
            var commonMark = Helpers.Normalize("# Header\n    foo\nHeader\n------\n    foo\n----");
            var expected = Helpers.Normalize("<h1>Header</h1>\n<pre><code>foo\n</code></pre>\n<h2>Header</h2>\n<pre><code>foo\n</code></pre>\n<hr />");
            Helpers.Log("Example {0}", 60);
            Helpers.Log("Section: {0}", "Leaf blocks - Indented code blocks");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "# Header\n    foo\nHeader\n------\n    foo\n----");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // The first line can be indented more than four spaces:
        [TestMethod]
        [TestCategory("Leaf blocks - Indented code blocks")]
        //[Timeout(1000)]
        public void Example061()
        {
            // Example 61
            // Section: Leaf blocks - Indented code blocks
            //
            // The following CommonMark:
            //             foo
            //         bar
            //
            // Should be rendered as:
            //     <pre><code>    foo
            //     bar
            //     </code></pre>

            // Arrange
            var commonMark = Helpers.Normalize("        foo\n    bar");
            var expected = Helpers.Normalize("<pre><code>    foo\nbar\n</code></pre>");
            Helpers.Log("Example {0}", 61);
            Helpers.Log("Section: {0}", "Leaf blocks - Indented code blocks");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "        foo\n    bar");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // Blank lines preceding or following an indented code block
        // are not included in it:
        [TestMethod]
        [TestCategory("Leaf blocks - Indented code blocks")]
        //[Timeout(1000)]
        public void Example062()
        {
            // Example 62
            // Section: Leaf blocks - Indented code blocks
            //
            // The following CommonMark:
            //         
            //         foo
            //         
            //     
            //
            // Should be rendered as:
            //     <pre><code>foo
            //     </code></pre>

            // Arrange
            var commonMark = Helpers.Normalize("    \n    foo\n    \n");
            var expected = Helpers.Normalize("<pre><code>foo\n</code></pre>");
            Helpers.Log("Example {0}", 62);
            Helpers.Log("Section: {0}", "Leaf blocks - Indented code blocks");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "    \n    foo\n    \n");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // Trailing spaces are included in the code block's content:
        [TestMethod]
        [TestCategory("Leaf blocks - Indented code blocks")]
        //[Timeout(1000)]
        public void Example063()
        {
            // Example 63
            // Section: Leaf blocks - Indented code blocks
            //
            // The following CommonMark:
            //         foo  
            //
            // Should be rendered as:
            //     <pre><code>foo  
            //     </code></pre>

            // Arrange
            var commonMark = Helpers.Normalize("    foo  ");
            var expected = Helpers.Normalize("<pre><code>foo  \n</code></pre>");
            Helpers.Log("Example {0}", 63);
            Helpers.Log("Section: {0}", "Leaf blocks - Indented code blocks");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "    foo  ");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // ## Fenced code blocks
        //
        // A [code fence](#code-fence) <a id="code-fence"></a> is a sequence
        // of at least three consecutive backtick characters (`` ` ``) or
        // tildes (`~`).  (Tildes and backticks cannot be mixed.)
        // A [fenced code block](#fenced-code-block) <a id="fenced-code-block"></a>
        // begins with a code fence, indented no more than three spaces.
        //
        // The line with the opening code fence may optionally contain some text
        // following the code fence; this is trimmed of leading and trailing
        // spaces and called the [info string](#info-string).
        // <a id="info-string"></a> The info string may not contain any backtick
        // characters.  (The reason for this restriction is that otherwise
        // some inline code would be incorrectly interpreted as the
        // beginning of a fenced code block.)
        //
        // The content of the code block consists of all subsequent lines, until
        // a closing [code fence](#code-fence) of the same type as the code block
        // began with (backticks or tildes), and with at least as many backticks
        // or tildes as the opening code fence.  If the leading code fence is
        // indented N spaces, then up to N spaces of indentation are removed from
        // each line of the content (if present).  (If a content line is not
        // indented, it is preserved unchanged.  If it is indented less than N
        // spaces, all of the indentation is removed.)
        //
        // The closing code fence may be indented up to three spaces, and may be
        // followed only by spaces, which are ignored.  If the end of the
        // containing block (or document) is reached and no closing code fence
        // has been found, the code block contains all of the lines after the
        // opening code fence until the end of the containing block (or
        // document).  (An alternative spec would require backtracking in the
        // event that a closing code fence is not found.  But this makes parsing
        // much less efficient, and there seems to be no real down side to the
        // behavior described here.)
        //
        // A fenced code block may interrupt a paragraph, and does not require
        // a blank line either before or after.
        //
        // The content of a code fence is treated as literal text, not parsed
        // as inlines.  The first word of the info string is typically used to
        // specify the language of the code sample, and rendered in the `class`
        // attribute of the `pre` tag.  However, this spec does not mandate any
        // particular treatment of the info string.
        //
        // Here is a simple example with backticks:
        [TestMethod]
        [TestCategory("Leaf blocks - Fenced code blocks")]
        //[Timeout(1000)]
        public void Example064()
        {
            // Example 64
            // Section: Leaf blocks - Fenced code blocks
            //
            // The following CommonMark:
            //     ```
            //     <
            //      >
            //     ```
            //
            // Should be rendered as:
            //     <pre><code>&lt;
            //      &gt;
            //     </code></pre>

            // Arrange
            var commonMark = Helpers.Normalize("```\n<\n >\n```");
            var expected = Helpers.Normalize("<pre><code>&lt;\n &gt;\n</code></pre>");
            Helpers.Log("Example {0}", 64);
            Helpers.Log("Section: {0}", "Leaf blocks - Fenced code blocks");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "```\n<\n >\n```");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // With tildes:
        [TestMethod]
        [TestCategory("Leaf blocks - Fenced code blocks")]
        //[Timeout(1000)]
        public void Example065()
        {
            // Example 65
            // Section: Leaf blocks - Fenced code blocks
            //
            // The following CommonMark:
            //     ~~~
            //     <
            //      >
            //     ~~~
            //
            // Should be rendered as:
            //     <pre><code>&lt;
            //      &gt;
            //     </code></pre>

            // Arrange
            var commonMark = Helpers.Normalize("~~~\n<\n >\n~~~");
            var expected = Helpers.Normalize("<pre><code>&lt;\n &gt;\n</code></pre>");
            Helpers.Log("Example {0}", 65);
            Helpers.Log("Section: {0}", "Leaf blocks - Fenced code blocks");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "~~~\n<\n >\n~~~");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // The closing code fence must use the same character as the opening
        // fence:
        [TestMethod]
        [TestCategory("Leaf blocks - Fenced code blocks")]
        //[Timeout(1000)]
        public void Example066()
        {
            // Example 66
            // Section: Leaf blocks - Fenced code blocks
            //
            // The following CommonMark:
            //     ```
            //     aaa
            //     ~~~
            //     ```
            //
            // Should be rendered as:
            //     <pre><code>aaa
            //     ~~~
            //     </code></pre>

            // Arrange
            var commonMark = Helpers.Normalize("```\naaa\n~~~\n```");
            var expected = Helpers.Normalize("<pre><code>aaa\n~~~\n</code></pre>");
            Helpers.Log("Example {0}", 66);
            Helpers.Log("Section: {0}", "Leaf blocks - Fenced code blocks");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "```\naaa\n~~~\n```");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        [TestMethod]
        [TestCategory("Leaf blocks - Fenced code blocks")]
        //[Timeout(1000)]
        public void Example067()
        {
            // Example 67
            // Section: Leaf blocks - Fenced code blocks
            //
            // The following CommonMark:
            //     ~~~
            //     aaa
            //     ```
            //     ~~~
            //
            // Should be rendered as:
            //     <pre><code>aaa
            //     ```
            //     </code></pre>

            // Arrange
            var commonMark = Helpers.Normalize("~~~\naaa\n```\n~~~");
            var expected = Helpers.Normalize("<pre><code>aaa\n```\n</code></pre>");
            Helpers.Log("Example {0}", 67);
            Helpers.Log("Section: {0}", "Leaf blocks - Fenced code blocks");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "~~~\naaa\n```\n~~~");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // The closing code fence must be at least as long as the opening fence:
        [TestMethod]
        [TestCategory("Leaf blocks - Fenced code blocks")]
        //[Timeout(1000)]
        public void Example068()
        {
            // Example 68
            // Section: Leaf blocks - Fenced code blocks
            //
            // The following CommonMark:
            //     ````
            //     aaa
            //     ```
            //     ``````
            //
            // Should be rendered as:
            //     <pre><code>aaa
            //     ```
            //     </code></pre>

            // Arrange
            var commonMark = Helpers.Normalize("````\naaa\n```\n``````");
            var expected = Helpers.Normalize("<pre><code>aaa\n```\n</code></pre>");
            Helpers.Log("Example {0}", 68);
            Helpers.Log("Section: {0}", "Leaf blocks - Fenced code blocks");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "````\naaa\n```\n``````");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        [TestMethod]
        [TestCategory("Leaf blocks - Fenced code blocks")]
        //[Timeout(1000)]
        public void Example069()
        {
            // Example 69
            // Section: Leaf blocks - Fenced code blocks
            //
            // The following CommonMark:
            //     ~~~~
            //     aaa
            //     ~~~
            //     ~~~~
            //
            // Should be rendered as:
            //     <pre><code>aaa
            //     ~~~
            //     </code></pre>

            // Arrange
            var commonMark = Helpers.Normalize("~~~~\naaa\n~~~\n~~~~");
            var expected = Helpers.Normalize("<pre><code>aaa\n~~~\n</code></pre>");
            Helpers.Log("Example {0}", 69);
            Helpers.Log("Section: {0}", "Leaf blocks - Fenced code blocks");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "~~~~\naaa\n~~~\n~~~~");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // Unclosed code blocks are closed by the end of the document:
        [TestMethod]
        [TestCategory("Leaf blocks - Fenced code blocks")]
        //[Timeout(1000)]
        public void Example070()
        {
            // Example 70
            // Section: Leaf blocks - Fenced code blocks
            //
            // The following CommonMark:
            //     ```
            //
            // Should be rendered as:
            //     <pre><code></code></pre>

            // Arrange
            var commonMark = Helpers.Normalize("```");
            var expected = Helpers.Normalize("<pre><code></code></pre>");
            Helpers.Log("Example {0}", 70);
            Helpers.Log("Section: {0}", "Leaf blocks - Fenced code blocks");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "```");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        [TestMethod]
        [TestCategory("Leaf blocks - Fenced code blocks")]
        //[Timeout(1000)]
        public void Example071()
        {
            // Example 71
            // Section: Leaf blocks - Fenced code blocks
            //
            // The following CommonMark:
            //     `````
            //     
            //     ```
            //     aaa
            //
            // Should be rendered as:
            //     <pre><code>
            //     ```
            //     aaa
            //     </code></pre>

            // Arrange
            var commonMark = Helpers.Normalize("`````\n\n```\naaa");
            var expected = Helpers.Normalize("<pre><code>\n```\naaa\n</code></pre>");
            Helpers.Log("Example {0}", 71);
            Helpers.Log("Section: {0}", "Leaf blocks - Fenced code blocks");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "`````\n\n```\naaa");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // A code block can have all empty lines as its content:
        [TestMethod]
        [TestCategory("Leaf blocks - Fenced code blocks")]
        //[Timeout(1000)]
        public void Example072()
        {
            // Example 72
            // Section: Leaf blocks - Fenced code blocks
            //
            // The following CommonMark:
            //     ```
            //     
            //       
            //     ```
            //
            // Should be rendered as:
            //     <pre><code>
            //       
            //     </code></pre>

            // Arrange
            var commonMark = Helpers.Normalize("```\n\n  \n```");
            var expected = Helpers.Normalize("<pre><code>\n  \n</code></pre>");
            Helpers.Log("Example {0}", 72);
            Helpers.Log("Section: {0}", "Leaf blocks - Fenced code blocks");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "```\n\n  \n```");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // A code block can be empty:
        [TestMethod]
        [TestCategory("Leaf blocks - Fenced code blocks")]
        //[Timeout(1000)]
        public void Example073()
        {
            // Example 73
            // Section: Leaf blocks - Fenced code blocks
            //
            // The following CommonMark:
            //     ```
            //     ```
            //
            // Should be rendered as:
            //     <pre><code></code></pre>

            // Arrange
            var commonMark = Helpers.Normalize("```\n```");
            var expected = Helpers.Normalize("<pre><code></code></pre>");
            Helpers.Log("Example {0}", 73);
            Helpers.Log("Section: {0}", "Leaf blocks - Fenced code blocks");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "```\n```");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // Fences can be indented.  If the opening fence is indented,
        // content lines will have equivalent opening indentation removed,
        // if present:
        [TestMethod]
        [TestCategory("Leaf blocks - Fenced code blocks")]
        //[Timeout(1000)]
        public void Example074()
        {
            // Example 74
            // Section: Leaf blocks - Fenced code blocks
            //
            // The following CommonMark:
            //      ```
            //      aaa
            //     aaa
            //     ```
            //
            // Should be rendered as:
            //     <pre><code>aaa
            //     aaa
            //     </code></pre>

            // Arrange
            var commonMark = Helpers.Normalize(" ```\n aaa\naaa\n```");
            var expected = Helpers.Normalize("<pre><code>aaa\naaa\n</code></pre>");
            Helpers.Log("Example {0}", 74);
            Helpers.Log("Section: {0}", "Leaf blocks - Fenced code blocks");
            Helpers.Log();
            Helpers.LogValue("CommonMark", " ```\n aaa\naaa\n```");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        [TestMethod]
        [TestCategory("Leaf blocks - Fenced code blocks")]
        //[Timeout(1000)]
        public void Example075()
        {
            // Example 75
            // Section: Leaf blocks - Fenced code blocks
            //
            // The following CommonMark:
            //       ```
            //     aaa
            //       aaa
            //     aaa
            //       ```
            //
            // Should be rendered as:
            //     <pre><code>aaa
            //     aaa
            //     aaa
            //     </code></pre>

            // Arrange
            var commonMark = Helpers.Normalize("  ```\naaa\n  aaa\naaa\n  ```");
            var expected = Helpers.Normalize("<pre><code>aaa\naaa\naaa\n</code></pre>");
            Helpers.Log("Example {0}", 75);
            Helpers.Log("Section: {0}", "Leaf blocks - Fenced code blocks");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "  ```\naaa\n  aaa\naaa\n  ```");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        [TestMethod]
        [TestCategory("Leaf blocks - Fenced code blocks")]
        //[Timeout(1000)]
        public void Example076()
        {
            // Example 76
            // Section: Leaf blocks - Fenced code blocks
            //
            // The following CommonMark:
            //        ```
            //        aaa
            //         aaa
            //       aaa
            //        ```
            //
            // Should be rendered as:
            //     <pre><code>aaa
            //      aaa
            //     aaa
            //     </code></pre>

            // Arrange
            var commonMark = Helpers.Normalize("   ```\n   aaa\n    aaa\n  aaa\n   ```");
            var expected = Helpers.Normalize("<pre><code>aaa\n aaa\naaa\n</code></pre>");
            Helpers.Log("Example {0}", 76);
            Helpers.Log("Section: {0}", "Leaf blocks - Fenced code blocks");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "   ```\n   aaa\n    aaa\n  aaa\n   ```");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // Four spaces indentation produces an indented code block:
        [TestMethod]
        [TestCategory("Leaf blocks - Fenced code blocks")]
        //[Timeout(1000)]
        public void Example077()
        {
            // Example 77
            // Section: Leaf blocks - Fenced code blocks
            //
            // The following CommonMark:
            //         ```
            //         aaa
            //         ```
            //
            // Should be rendered as:
            //     <pre><code>```
            //     aaa
            //     ```
            //     </code></pre>

            // Arrange
            var commonMark = Helpers.Normalize("    ```\n    aaa\n    ```");
            var expected = Helpers.Normalize("<pre><code>```\naaa\n```\n</code></pre>");
            Helpers.Log("Example {0}", 77);
            Helpers.Log("Section: {0}", "Leaf blocks - Fenced code blocks");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "    ```\n    aaa\n    ```");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // Code fences (opening and closing) cannot contain internal spaces:
        [TestMethod]
        [TestCategory("Leaf blocks - Fenced code blocks")]
        //[Timeout(1000)]
        public void Example078()
        {
            // Example 78
            // Section: Leaf blocks - Fenced code blocks
            //
            // The following CommonMark:
            //     ``` ```
            //     aaa
            //
            // Should be rendered as:
            //     <p><code></code>
            //     aaa</p>

            // Arrange
            var commonMark = Helpers.Normalize("``` ```\naaa");
            var expected = Helpers.Normalize("<p><code></code>\naaa</p>");
            Helpers.Log("Example {0}", 78);
            Helpers.Log("Section: {0}", "Leaf blocks - Fenced code blocks");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "``` ```\naaa");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        [TestMethod]
        [TestCategory("Leaf blocks - Fenced code blocks")]
        //[Timeout(1000)]
        public void Example079()
        {
            // Example 79
            // Section: Leaf blocks - Fenced code blocks
            //
            // The following CommonMark:
            //     ~~~~~~
            //     aaa
            //     ~~~ ~~
            //
            // Should be rendered as:
            //     <pre><code>aaa
            //     ~~~ ~~
            //     </code></pre>

            // Arrange
            var commonMark = Helpers.Normalize("~~~~~~\naaa\n~~~ ~~");
            var expected = Helpers.Normalize("<pre><code>aaa\n~~~ ~~\n</code></pre>");
            Helpers.Log("Example {0}", 79);
            Helpers.Log("Section: {0}", "Leaf blocks - Fenced code blocks");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "~~~~~~\naaa\n~~~ ~~");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // Fenced code blocks can interrupt paragraphs, and can be followed
        // directly by paragraphs, without a blank line between:
        [TestMethod]
        [TestCategory("Leaf blocks - Fenced code blocks")]
        //[Timeout(1000)]
        public void Example080()
        {
            // Example 80
            // Section: Leaf blocks - Fenced code blocks
            //
            // The following CommonMark:
            //     foo
            //     ```
            //     bar
            //     ```
            //     baz
            //
            // Should be rendered as:
            //     <p>foo</p>
            //     <pre><code>bar
            //     </code></pre>
            //     <p>baz</p>

            // Arrange
            var commonMark = Helpers.Normalize("foo\n```\nbar\n```\nbaz");
            var expected = Helpers.Normalize("<p>foo</p>\n<pre><code>bar\n</code></pre>\n<p>baz</p>");
            Helpers.Log("Example {0}", 80);
            Helpers.Log("Section: {0}", "Leaf blocks - Fenced code blocks");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "foo\n```\nbar\n```\nbaz");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // Other blocks can also occur before and after fenced code blocks
        // without an intervening blank line:
        [TestMethod]
        [TestCategory("Leaf blocks - Fenced code blocks")]
        //[Timeout(1000)]
        public void Example081()
        {
            // Example 81
            // Section: Leaf blocks - Fenced code blocks
            //
            // The following CommonMark:
            //     foo
            //     ---
            //     ~~~
            //     bar
            //     ~~~
            //     # baz
            //
            // Should be rendered as:
            //     <h2>foo</h2>
            //     <pre><code>bar
            //     </code></pre>
            //     <h1>baz</h1>

            // Arrange
            var commonMark = Helpers.Normalize("foo\n---\n~~~\nbar\n~~~\n# baz");
            var expected = Helpers.Normalize("<h2>foo</h2>\n<pre><code>bar\n</code></pre>\n<h1>baz</h1>");
            Helpers.Log("Example {0}", 81);
            Helpers.Log("Section: {0}", "Leaf blocks - Fenced code blocks");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "foo\n---\n~~~\nbar\n~~~\n# baz");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // An [info string](#info-string) can be provided after the opening code fence.
        // Opening and closing spaces will be stripped, and the first word, prefixed
        // with `language-`, is used as the value for the `class` attribute of the
        // `code` element within the enclosing `pre` element.
        [TestMethod]
        [TestCategory("Leaf blocks - Fenced code blocks")]
        //[Timeout(1000)]
        public void Example082()
        {
            // Example 82
            // Section: Leaf blocks - Fenced code blocks
            //
            // The following CommonMark:
            //     ```ruby
            //     def foo(x)
            //       return 3
            //     end
            //     ```
            //
            // Should be rendered as:
            //     <pre><code class="language-ruby">def foo(x)
            //       return 3
            //     end
            //     </code></pre>

            // Arrange
            var commonMark = Helpers.Normalize("```ruby\ndef foo(x)\n  return 3\nend\n```");
            var expected = Helpers.Normalize("<pre><code class=\"language-ruby\">def foo(x)\n  return 3\nend\n</code></pre>");
            Helpers.Log("Example {0}", 82);
            Helpers.Log("Section: {0}", "Leaf blocks - Fenced code blocks");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "```ruby\ndef foo(x)\n  return 3\nend\n```");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        [TestMethod]
        [TestCategory("Leaf blocks - Fenced code blocks")]
        //[Timeout(1000)]
        public void Example083()
        {
            // Example 83
            // Section: Leaf blocks - Fenced code blocks
            //
            // The following CommonMark:
            //     ~~~~    ruby startline=3 $%@#$
            //     def foo(x)
            //       return 3
            //     end
            //     ~~~~~~~
            //
            // Should be rendered as:
            //     <pre><code class="language-ruby">def foo(x)
            //       return 3
            //     end
            //     </code></pre>

            // Arrange
            var commonMark = Helpers.Normalize("~~~~    ruby startline=3 $%@#$\ndef foo(x)\n  return 3\nend\n~~~~~~~");
            var expected = Helpers.Normalize("<pre><code class=\"language-ruby\">def foo(x)\n  return 3\nend\n</code></pre>");
            Helpers.Log("Example {0}", 83);
            Helpers.Log("Section: {0}", "Leaf blocks - Fenced code blocks");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "~~~~    ruby startline=3 $%@#$\ndef foo(x)\n  return 3\nend\n~~~~~~~");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        [TestMethod]
        [TestCategory("Leaf blocks - Fenced code blocks")]
        //[Timeout(1000)]
        public void Example084()
        {
            // Example 84
            // Section: Leaf blocks - Fenced code blocks
            //
            // The following CommonMark:
            //     ````;
            //     ````
            //
            // Should be rendered as:
            //     <pre><code class="language-;"></code></pre>

            // Arrange
            var commonMark = Helpers.Normalize("````;\n````");
            var expected = Helpers.Normalize("<pre><code class=\"language-;\"></code></pre>");
            Helpers.Log("Example {0}", 84);
            Helpers.Log("Section: {0}", "Leaf blocks - Fenced code blocks");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "````;\n````");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // Info strings for backtick code blocks cannot contain backticks:
        [TestMethod]
        [TestCategory("Leaf blocks - Fenced code blocks")]
        //[Timeout(1000)]
        public void Example085()
        {
            // Example 85
            // Section: Leaf blocks - Fenced code blocks
            //
            // The following CommonMark:
            //     ``` aa ```
            //     foo
            //
            // Should be rendered as:
            //     <p><code>aa</code>
            //     foo</p>

            // Arrange
            var commonMark = Helpers.Normalize("``` aa ```\nfoo");
            var expected = Helpers.Normalize("<p><code>aa</code>\nfoo</p>");
            Helpers.Log("Example {0}", 85);
            Helpers.Log("Section: {0}", "Leaf blocks - Fenced code blocks");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "``` aa ```\nfoo");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // Closing code fences cannot have info strings:
        [TestMethod]
        [TestCategory("Leaf blocks - Fenced code blocks")]
        //[Timeout(1000)]
        public void Example086()
        {
            // Example 86
            // Section: Leaf blocks - Fenced code blocks
            //
            // The following CommonMark:
            //     ```
            //     ``` aaa
            //     ```
            //
            // Should be rendered as:
            //     <pre><code>``` aaa
            //     </code></pre>

            // Arrange
            var commonMark = Helpers.Normalize("```\n``` aaa\n```");
            var expected = Helpers.Normalize("<pre><code>``` aaa\n</code></pre>");
            Helpers.Log("Example {0}", 86);
            Helpers.Log("Section: {0}", "Leaf blocks - Fenced code blocks");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "```\n``` aaa\n```");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // ## HTML blocks
        //
        // An [HTML block tag](#html-block-tag) <a id="html-block-tag"></a> is
        // an [open tag](#open-tag) or [closing tag](#closing-tag) whose tag
        // name is one of the following (case-insensitive):
        // `article`, `header`, `aside`, `hgroup`, `blockquote`, `hr`, `iframe`,
        // `body`, `li`, `map`, `button`, `object`, `canvas`, `ol`, `caption`,
        // `output`, `col`, `p`, `colgroup`, `pre`, `dd`, `progress`, `div`,
        // `section`, `dl`, `table`, `td`, `dt`, `tbody`, `embed`, `textarea`,
        // `fieldset`, `tfoot`, `figcaption`, `th`, `figure`, `thead`, `footer`,
        // `footer`, `tr`, `form`, `ul`, `h1`, `h2`, `h3`, `h4`, `h5`, `h6`,
        // `video`, `script`, `style`.
        //
        // An [HTML block](#html-block) <a id="html-block"></a> begins with an
        // [HTML block tag](#html-block-tag), [HTML comment](#html-comment),
        // [processing instruction](#processing-instruction),
        // [declaration](#declaration), or [CDATA section](#cdata-section).
        // It ends when a [blank line](#blank-line) or the end of the
        // input is encountered.  The initial line may be indented up to three
        // spaces, and subsequent lines may have any indentation.  The contents
        // of the HTML block are interpreted as raw HTML, and will not be escaped
        // in HTML output.
        //
        // Some simple examples:
        [TestMethod]
        [TestCategory("Leaf blocks - HTML blocks")]
        //[Timeout(1000)]
        public void Example087()
        {
            // Example 87
            // Section: Leaf blocks - HTML blocks
            //
            // The following CommonMark:
            //     <table>
            //       <tr>
            //         <td>
            //                hi
            //         </td>
            //       </tr>
            //     </table>
            //     
            //     okay.
            //
            // Should be rendered as:
            //     <table>
            //       <tr>
            //         <td>
            //                hi
            //         </td>
            //       </tr>
            //     </table>
            //     <p>okay.</p>

            // Arrange
            var commonMark = Helpers.Normalize("<table>\n  <tr>\n    <td>\n           hi\n    </td>\n  </tr>\n</table>\n\nokay.");
            var expected = Helpers.Normalize("<table>\n  <tr>\n    <td>\n           hi\n    </td>\n  </tr>\n</table>\n<p>okay.</p>");
            Helpers.Log("Example {0}", 87);
            Helpers.Log("Section: {0}", "Leaf blocks - HTML blocks");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "<table>\n  <tr>\n    <td>\n           hi\n    </td>\n  </tr>\n</table>\n\nokay.");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        [TestMethod]
        [TestCategory("Leaf blocks - HTML blocks")]
        //[Timeout(1000)]
        public void Example088()
        {
            // Example 88
            // Section: Leaf blocks - HTML blocks
            //
            // The following CommonMark:
            //      <div>
            //       *hello*
            //              <foo><a>
            //
            // Should be rendered as:
            //      <div>
            //       *hello*
            //              <foo><a>

            // Arrange
            var commonMark = Helpers.Normalize(" <div>\n  *hello*\n         <foo><a>");
            var expected = Helpers.Normalize(" <div>\n  *hello*\n         <foo><a>");
            Helpers.Log("Example {0}", 88);
            Helpers.Log("Section: {0}", "Leaf blocks - HTML blocks");
            Helpers.Log();
            Helpers.LogValue("CommonMark", " <div>\n  *hello*\n         <foo><a>");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // Here we have two code blocks with a Markdown paragraph between them:
        [TestMethod]
        [TestCategory("Leaf blocks - HTML blocks")]
        //[Timeout(1000)]
        public void Example089()
        {
            // Example 89
            // Section: Leaf blocks - HTML blocks
            //
            // The following CommonMark:
            //     <DIV CLASS="foo">
            //     
            //     *Markdown*
            //     
            //     </DIV>
            //
            // Should be rendered as:
            //     <DIV CLASS="foo">
            //     <p><em>Markdown</em></p>
            //     </DIV>

            // Arrange
            var commonMark = Helpers.Normalize("<DIV CLASS=\"foo\">\n\n*Markdown*\n\n</DIV>");
            var expected = Helpers.Normalize("<DIV CLASS=\"foo\">\n<p><em>Markdown</em></p>\n</DIV>");
            Helpers.Log("Example {0}", 89);
            Helpers.Log("Section: {0}", "Leaf blocks - HTML blocks");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "<DIV CLASS=\"foo\">\n\n*Markdown*\n\n</DIV>");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // In the following example, what looks like a Markdown code block
        // is actually part of the HTML block, which continues until a blank
        // line or the end of the document is reached:
        [TestMethod]
        [TestCategory("Leaf blocks - HTML blocks")]
        //[Timeout(1000)]
        public void Example090()
        {
            // Example 90
            // Section: Leaf blocks - HTML blocks
            //
            // The following CommonMark:
            //     <div></div>
            //     ``` c
            //     int x = 33;
            //     ```
            //
            // Should be rendered as:
            //     <div></div>
            //     ``` c
            //     int x = 33;
            //     ```

            // Arrange
            var commonMark = Helpers.Normalize("<div></div>\n``` c\nint x = 33;\n```");
            var expected = Helpers.Normalize("<div></div>\n``` c\nint x = 33;\n```");
            Helpers.Log("Example {0}", 90);
            Helpers.Log("Section: {0}", "Leaf blocks - HTML blocks");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "<div></div>\n``` c\nint x = 33;\n```");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // A comment:
        [TestMethod]
        [TestCategory("Leaf blocks - HTML blocks")]
        //[Timeout(1000)]
        public void Example091()
        {
            // Example 91
            // Section: Leaf blocks - HTML blocks
            //
            // The following CommonMark:
            //     <!-- Foo
            //     bar
            //        baz -->
            //
            // Should be rendered as:
            //     <!-- Foo
            //     bar
            //        baz -->

            // Arrange
            var commonMark = Helpers.Normalize("<!-- Foo\nbar\n   baz -->");
            var expected = Helpers.Normalize("<!-- Foo\nbar\n   baz -->");
            Helpers.Log("Example {0}", 91);
            Helpers.Log("Section: {0}", "Leaf blocks - HTML blocks");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "<!-- Foo\nbar\n   baz -->");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // A processing instruction:
        [TestMethod]
        [TestCategory("Leaf blocks - HTML blocks")]
        //[Timeout(1000)]
        public void Example092()
        {
            // Example 92
            // Section: Leaf blocks - HTML blocks
            //
            // The following CommonMark:
            //     <?php
            //       echo 'foo'
            //     ?>
            //
            // Should be rendered as:
            //     <?php
            //       echo 'foo'
            //     ?>

            // Arrange
            var commonMark = Helpers.Normalize("<?php\n  echo 'foo'\n?>");
            var expected = Helpers.Normalize("<?php\n  echo 'foo'\n?>");
            Helpers.Log("Example {0}", 92);
            Helpers.Log("Section: {0}", "Leaf blocks - HTML blocks");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "<?php\n  echo 'foo'\n?>");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // CDATA:
        [TestMethod]
        [TestCategory("Leaf blocks - HTML blocks")]
        //[Timeout(1000)]
        public void Example093()
        {
            // Example 93
            // Section: Leaf blocks - HTML blocks
            //
            // The following CommonMark:
            //     <![CDATA[
            //     function matchwo(a,b)
            //     {
            //     if (a < b && a < 0) then
            //       {
            //       return 1;
            //       }
            //     else
            //       {
            //       return 0;
            //       }
            //     }
            //     ]]>
            //
            // Should be rendered as:
            //     <![CDATA[
            //     function matchwo(a,b)
            //     {
            //     if (a < b && a < 0) then
            //       {
            //       return 1;
            //       }
            //     else
            //       {
            //       return 0;
            //       }
            //     }
            //     ]]>

            // Arrange
            var commonMark = Helpers.Normalize("<![CDATA[\nfunction matchwo(a,b)\n{\nif (a < b && a < 0) then\n  {\n  return 1;\n  }\nelse\n  {\n  return 0;\n  }\n}\n]]>");
            var expected = Helpers.Normalize("<![CDATA[\nfunction matchwo(a,b)\n{\nif (a < b && a < 0) then\n  {\n  return 1;\n  }\nelse\n  {\n  return 0;\n  }\n}\n]]>");
            Helpers.Log("Example {0}", 93);
            Helpers.Log("Section: {0}", "Leaf blocks - HTML blocks");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "<![CDATA[\nfunction matchwo(a,b)\n{\nif (a < b && a < 0) then\n  {\n  return 1;\n  }\nelse\n  {\n  return 0;\n  }\n}\n]]>");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // The opening tag can be indented 1-3 spaces, but not 4:
        [TestMethod]
        [TestCategory("Leaf blocks - HTML blocks")]
        //[Timeout(1000)]
        public void Example094()
        {
            // Example 94
            // Section: Leaf blocks - HTML blocks
            //
            // The following CommonMark:
            //       <!-- foo -->
            //     
            //         <!-- foo -->
            //
            // Should be rendered as:
            //       <!-- foo -->
            //     <pre><code>&lt;!-- foo --&gt;
            //     </code></pre>

            // Arrange
            var commonMark = Helpers.Normalize("  <!-- foo -->\n\n    <!-- foo -->");
            var expected = Helpers.Normalize("  <!-- foo -->\n<pre><code>&lt;!-- foo --&gt;\n</code></pre>");
            Helpers.Log("Example {0}", 94);
            Helpers.Log("Section: {0}", "Leaf blocks - HTML blocks");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "  <!-- foo -->\n\n    <!-- foo -->");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // An HTML block can interrupt a paragraph, and need not be preceded
        // by a blank line.
        [TestMethod]
        [TestCategory("Leaf blocks - HTML blocks")]
        //[Timeout(1000)]
        public void Example095()
        {
            // Example 95
            // Section: Leaf blocks - HTML blocks
            //
            // The following CommonMark:
            //     Foo
            //     <div>
            //     bar
            //     </div>
            //
            // Should be rendered as:
            //     <p>Foo</p>
            //     <div>
            //     bar
            //     </div>

            // Arrange
            var commonMark = Helpers.Normalize("Foo\n<div>\nbar\n</div>");
            var expected = Helpers.Normalize("<p>Foo</p>\n<div>\nbar\n</div>");
            Helpers.Log("Example {0}", 95);
            Helpers.Log("Section: {0}", "Leaf blocks - HTML blocks");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "Foo\n<div>\nbar\n</div>");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // However, a following blank line is always needed, except at the end of
        // a document:
        [TestMethod]
        [TestCategory("Leaf blocks - HTML blocks")]
        //[Timeout(1000)]
        public void Example096()
        {
            // Example 96
            // Section: Leaf blocks - HTML blocks
            //
            // The following CommonMark:
            //     <div>
            //     bar
            //     </div>
            //     *foo*
            //
            // Should be rendered as:
            //     <div>
            //     bar
            //     </div>
            //     *foo*

            // Arrange
            var commonMark = Helpers.Normalize("<div>\nbar\n</div>\n*foo*");
            var expected = Helpers.Normalize("<div>\nbar\n</div>\n*foo*");
            Helpers.Log("Example {0}", 96);
            Helpers.Log("Section: {0}", "Leaf blocks - HTML blocks");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "<div>\nbar\n</div>\n*foo*");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // An incomplete HTML block tag may also start an HTML block:
        [TestMethod]
        [TestCategory("Leaf blocks - HTML blocks")]
        //[Timeout(1000)]
        public void Example097()
        {
            // Example 97
            // Section: Leaf blocks - HTML blocks
            //
            // The following CommonMark:
            //     <div class
            //     foo
            //
            // Should be rendered as:
            //     <div class
            //     foo

            // Arrange
            var commonMark = Helpers.Normalize("<div class\nfoo");
            var expected = Helpers.Normalize("<div class\nfoo");
            Helpers.Log("Example {0}", 97);
            Helpers.Log("Section: {0}", "Leaf blocks - HTML blocks");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "<div class\nfoo");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // This rule differs from John Gruber's original Markdown syntax
        // specification, which says:
        //
        // > The only restrictions are that block-level HTML elements —
        // > e.g. `<div>`, `<table>`, `<pre>`, `<p>`, etc. — must be separated from
        // > surrounding content by blank lines, and the start and end tags of the
        // > block should not be indented with tabs or spaces.
        //
        // In some ways Gruber's rule is more restrictive than the one given
        // here:
        //
        // - It requires that an HTML block be preceded by a blank line.
        // - It does not allow the start tag to be indented.
        // - It requires a matching end tag, which it also does not allow to
        // be indented.
        //
        // Indeed, most Markdown implementations, including some of Gruber's
        // own perl implementations, do not impose these restrictions.
        //
        // There is one respect, however, in which Gruber's rule is more liberal
        // than the one given here, since it allows blank lines to occur inside
        // an HTML block.  There are two reasons for disallowing them here.
        // First, it removes the need to parse balanced tags, which is
        // expensive and can require backtracking from the end of the document
        // if no matching end tag is found. Second, it provides a very simple
        // and flexible way of including Markdown content inside HTML tags:
        // simply separate the Markdown from the HTML using blank lines:
        [TestMethod]
        [TestCategory("Leaf blocks - HTML blocks")]
        //[Timeout(1000)]
        public void Example098()
        {
            // Example 98
            // Section: Leaf blocks - HTML blocks
            //
            // The following CommonMark:
            //     <div>
            //     
            //     *Emphasized* text.
            //     
            //     </div>
            //
            // Should be rendered as:
            //     <div>
            //     <p><em>Emphasized</em> text.</p>
            //     </div>

            // Arrange
            var commonMark = Helpers.Normalize("<div>\n\n*Emphasized* text.\n\n</div>");
            var expected = Helpers.Normalize("<div>\n<p><em>Emphasized</em> text.</p>\n</div>");
            Helpers.Log("Example {0}", 98);
            Helpers.Log("Section: {0}", "Leaf blocks - HTML blocks");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "<div>\n\n*Emphasized* text.\n\n</div>");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // Compare:
        [TestMethod]
        [TestCategory("Leaf blocks - HTML blocks")]
        //[Timeout(1000)]
        public void Example099()
        {
            // Example 99
            // Section: Leaf blocks - HTML blocks
            //
            // The following CommonMark:
            //     <div>
            //     *Emphasized* text.
            //     </div>
            //
            // Should be rendered as:
            //     <div>
            //     *Emphasized* text.
            //     </div>

            // Arrange
            var commonMark = Helpers.Normalize("<div>\n*Emphasized* text.\n</div>");
            var expected = Helpers.Normalize("<div>\n*Emphasized* text.\n</div>");
            Helpers.Log("Example {0}", 99);
            Helpers.Log("Section: {0}", "Leaf blocks - HTML blocks");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "<div>\n*Emphasized* text.\n</div>");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // Some Markdown implementations have adopted a convention of
        // interpreting content inside tags as text if the open tag has
        // the attribute `markdown=1`.  The rule given above seems a simpler and
        // more elegant way of achieving the same expressive power, which is also
        // much simpler to parse.
        //
        // The main potential drawback is that one can no longer paste HTML
        // blocks into Markdown documents with 100% reliability.  However,
        // *in most cases* this will work fine, because the blank lines in
        // HTML are usually followed by HTML block tags.  For example:
        [TestMethod]
        [TestCategory("Leaf blocks - HTML blocks")]
        //[Timeout(1000)]
        public void Example100()
        {
            // Example 100
            // Section: Leaf blocks - HTML blocks
            //
            // The following CommonMark:
            //     <table>
            //     
            //     <tr>
            //     
            //     <td>
            //     Hi
            //     </td>
            //     
            //     </tr>
            //     
            //     </table>
            //
            // Should be rendered as:
            //     <table>
            //     <tr>
            //     <td>
            //     Hi
            //     </td>
            //     </tr>
            //     </table>

            // Arrange
            var commonMark = Helpers.Normalize("<table>\n\n<tr>\n\n<td>\nHi\n</td>\n\n</tr>\n\n</table>");
            var expected = Helpers.Normalize("<table>\n<tr>\n<td>\nHi\n</td>\n</tr>\n</table>");
            Helpers.Log("Example {0}", 100);
            Helpers.Log("Section: {0}", "Leaf blocks - HTML blocks");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "<table>\n\n<tr>\n\n<td>\nHi\n</td>\n\n</tr>\n\n</table>");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // Moreover, blank lines are usually not necessary and can be
        // deleted.  The exception is inside `<pre>` tags; here, one can
        // replace the blank lines with `&#10;` entities.
        //
        // So there is no important loss of expressive power with the new rule.
        //
        // ## Link reference definitions
        //
        // A [link reference definition](#link-reference-definition)
        // <a id="link-reference-definition"></a> consists of a [link
        // label](#link-label), indented up to three spaces, followed
        // by a colon (`:`), optional blank space (including up to one
        // newline), a [link destination](#link-destination), optional
        // blank space (including up to one newline), and an optional [link
        // title](#link-title), which if it is present must be separated
        // from the [link destination](#link-destination) by whitespace.
        // No further non-space characters may occur on the line.
        //
        // A [link reference-definition](#link-reference-definition)
        // does not correspond to a structural element of a document.  Instead, it
        // defines a label which can be used in [reference links](#reference-link)
        // and reference-style [images](#image) elsewhere in the document.  [Link
        // reference definitions] can come either before or after the links that use
        // them.
        [TestMethod]
        [TestCategory("Leaf blocks - Link reference definitions")]
        //[Timeout(1000)]
        public void Example101()
        {
            // Example 101
            // Section: Leaf blocks - Link reference definitions
            //
            // The following CommonMark:
            //     [foo]: /url "title"
            //     
            //     [foo]
            //
            // Should be rendered as:
            //     <p><a href="/url" title="title">foo</a></p>

            // Arrange
            var commonMark = Helpers.Normalize("[foo]: /url \"title\"\n\n[foo]");
            var expected = Helpers.Normalize("<p><a href=\"/url\" title=\"title\">foo</a></p>");
            Helpers.Log("Example {0}", 101);
            Helpers.Log("Section: {0}", "Leaf blocks - Link reference definitions");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "[foo]: /url \"title\"\n\n[foo]");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        [TestMethod]
        [TestCategory("Leaf blocks - Link reference definitions")]
        //[Timeout(1000)]
        public void Example102()
        {
            // Example 102
            // Section: Leaf blocks - Link reference definitions
            //
            // The following CommonMark:
            //        [foo]: 
            //           /url  
            //                'the title'  
            //     
            //     [foo]
            //
            // Should be rendered as:
            //     <p><a href="/url" title="the title">foo</a></p>

            // Arrange
            var commonMark = Helpers.Normalize("   [foo]: \n      /url  \n           'the title'  \n\n[foo]");
            var expected = Helpers.Normalize("<p><a href=\"/url\" title=\"the title\">foo</a></p>");
            Helpers.Log("Example {0}", 102);
            Helpers.Log("Section: {0}", "Leaf blocks - Link reference definitions");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "   [foo]: \n      /url  \n           'the title'  \n\n[foo]");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        [TestMethod]
        [TestCategory("Leaf blocks - Link reference definitions")]
        //[Timeout(1000)]
        public void Example103()
        {
            // Example 103
            // Section: Leaf blocks - Link reference definitions
            //
            // The following CommonMark:
            //     [Foo*bar\]]:my_(url) 'title (with parens)'
            //     
            //     [Foo*bar\]]
            //
            // Should be rendered as:
            //     <p><a href="my_(url)" title="title (with parens)">Foo*bar]</a></p>

            // Arrange
            var commonMark = Helpers.Normalize("[Foo*bar\\]]:my_(url) 'title (with parens)'\n\n[Foo*bar\\]]");
            var expected = Helpers.Normalize("<p><a href=\"my_(url)\" title=\"title (with parens)\">Foo*bar]</a></p>");
            Helpers.Log("Example {0}", 103);
            Helpers.Log("Section: {0}", "Leaf blocks - Link reference definitions");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "[Foo*bar\\]]:my_(url) 'title (with parens)'\n\n[Foo*bar\\]]");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        [TestMethod]
        [TestCategory("Leaf blocks - Link reference definitions")]
        //[Timeout(1000)]
        public void Example104()
        {
            // Example 104
            // Section: Leaf blocks - Link reference definitions
            //
            // The following CommonMark:
            //     [Foo bar]:
            //     <my url>
            //     'title'
            //     
            //     [Foo bar]
            //
            // Should be rendered as:
            //     <p><a href="my url" title="title">Foo bar</a></p>

            // Arrange
            var commonMark = Helpers.Normalize("[Foo bar]:\n<my url>\n'title'\n\n[Foo bar]");
            var expected = Helpers.Normalize("<p><a href=\"my url\" title=\"title\">Foo bar</a></p>");
            Helpers.Log("Example {0}", 104);
            Helpers.Log("Section: {0}", "Leaf blocks - Link reference definitions");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "[Foo bar]:\n<my url>\n'title'\n\n[Foo bar]");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // The title may be omitted:
        [TestMethod]
        [TestCategory("Leaf blocks - Link reference definitions")]
        //[Timeout(1000)]
        public void Example105()
        {
            // Example 105
            // Section: Leaf blocks - Link reference definitions
            //
            // The following CommonMark:
            //     [foo]:
            //     /url
            //     
            //     [foo]
            //
            // Should be rendered as:
            //     <p><a href="/url">foo</a></p>

            // Arrange
            var commonMark = Helpers.Normalize("[foo]:\n/url\n\n[foo]");
            var expected = Helpers.Normalize("<p><a href=\"/url\">foo</a></p>");
            Helpers.Log("Example {0}", 105);
            Helpers.Log("Section: {0}", "Leaf blocks - Link reference definitions");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "[foo]:\n/url\n\n[foo]");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // The link destination may not be omitted:
        [TestMethod]
        [TestCategory("Leaf blocks - Link reference definitions")]
        //[Timeout(1000)]
        public void Example106()
        {
            // Example 106
            // Section: Leaf blocks - Link reference definitions
            //
            // The following CommonMark:
            //     [foo]:
            //     
            //     [foo]
            //
            // Should be rendered as:
            //     <p>[foo]:</p>
            //     <p>[foo]</p>

            // Arrange
            var commonMark = Helpers.Normalize("[foo]:\n\n[foo]");
            var expected = Helpers.Normalize("<p>[foo]:</p>\n<p>[foo]</p>");
            Helpers.Log("Example {0}", 106);
            Helpers.Log("Section: {0}", "Leaf blocks - Link reference definitions");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "[foo]:\n\n[foo]");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // A link can come before its corresponding definition:
        [TestMethod]
        [TestCategory("Leaf blocks - Link reference definitions")]
        //[Timeout(1000)]
        public void Example107()
        {
            // Example 107
            // Section: Leaf blocks - Link reference definitions
            //
            // The following CommonMark:
            //     [foo]
            //     
            //     [foo]: url
            //
            // Should be rendered as:
            //     <p><a href="url">foo</a></p>

            // Arrange
            var commonMark = Helpers.Normalize("[foo]\n\n[foo]: url");
            var expected = Helpers.Normalize("<p><a href=\"url\">foo</a></p>");
            Helpers.Log("Example {0}", 107);
            Helpers.Log("Section: {0}", "Leaf blocks - Link reference definitions");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "[foo]\n\n[foo]: url");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // If there are several matching definitions, the first one takes
        // precedence:
        [TestMethod]
        [TestCategory("Leaf blocks - Link reference definitions")]
        //[Timeout(1000)]
        public void Example108()
        {
            // Example 108
            // Section: Leaf blocks - Link reference definitions
            //
            // The following CommonMark:
            //     [foo]
            //     
            //     [foo]: first
            //     [foo]: second
            //
            // Should be rendered as:
            //     <p><a href="first">foo</a></p>

            // Arrange
            var commonMark = Helpers.Normalize("[foo]\n\n[foo]: first\n[foo]: second");
            var expected = Helpers.Normalize("<p><a href=\"first\">foo</a></p>");
            Helpers.Log("Example {0}", 108);
            Helpers.Log("Section: {0}", "Leaf blocks - Link reference definitions");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "[foo]\n\n[foo]: first\n[foo]: second");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // As noted in the section on [Links], matching of labels is
        // case-insensitive (see [matches](#matches)).
        [TestMethod]
        [TestCategory("Leaf blocks - Link reference definitions")]
        //[Timeout(1000)]
        public void Example109()
        {
            // Example 109
            // Section: Leaf blocks - Link reference definitions
            //
            // The following CommonMark:
            //     [FOO]: /url
            //     
            //     [Foo]
            //
            // Should be rendered as:
            //     <p><a href="/url">Foo</a></p>

            // Arrange
            var commonMark = Helpers.Normalize("[FOO]: /url\n\n[Foo]");
            var expected = Helpers.Normalize("<p><a href=\"/url\">Foo</a></p>");
            Helpers.Log("Example {0}", 109);
            Helpers.Log("Section: {0}", "Leaf blocks - Link reference definitions");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "[FOO]: /url\n\n[Foo]");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        [TestMethod]
        [TestCategory("Leaf blocks - Link reference definitions")]
        //[Timeout(1000)]
        public void Example110()
        {
            // Example 110
            // Section: Leaf blocks - Link reference definitions
            //
            // The following CommonMark:
            //     [ΑΓΩ]: /φου
            //     
            //     [αγω]
            //
            // Should be rendered as:
            //     <p><a href="/φου">αγω</a></p>

            // Arrange
            var commonMark = Helpers.Normalize("[ΑΓΩ]: /φου\n\n[αγω]");
            var expected = Helpers.Normalize("<p><a href=\"/φου\">αγω</a></p>");
            Helpers.Log("Example {0}", 110);
            Helpers.Log("Section: {0}", "Leaf blocks - Link reference definitions");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "[ΑΓΩ]: /φου\n\n[αγω]");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // Here is a link reference definition with no corresponding link.
        // It contributes nothing to the document.
        [TestMethod]
        [TestCategory("Leaf blocks - Link reference definitions")]
        //[Timeout(1000)]
        public void Example111()
        {
            // Example 111
            // Section: Leaf blocks - Link reference definitions
            //
            // The following CommonMark:
            //     [foo]: /url
            //
            // Should be rendered as:
            //     

            // Arrange
            var commonMark = Helpers.Normalize("[foo]: /url");
            var expected = Helpers.Normalize("");
            Helpers.Log("Example {0}", 111);
            Helpers.Log("Section: {0}", "Leaf blocks - Link reference definitions");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "[foo]: /url");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // This is not a link reference definition, because there are
        // non-space characters after the title:
        [TestMethod]
        [TestCategory("Leaf blocks - Link reference definitions")]
        //[Timeout(1000)]
        public void Example112()
        {
            // Example 112
            // Section: Leaf blocks - Link reference definitions
            //
            // The following CommonMark:
            //     [foo]: /url "title" ok
            //
            // Should be rendered as:
            //     <p>[foo]: /url &quot;title&quot; ok</p>

            // Arrange
            var commonMark = Helpers.Normalize("[foo]: /url \"title\" ok");
            var expected = Helpers.Normalize("<p>[foo]: /url &quot;title&quot; ok</p>");
            Helpers.Log("Example {0}", 112);
            Helpers.Log("Section: {0}", "Leaf blocks - Link reference definitions");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "[foo]: /url \"title\" ok");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // This is not a link reference definition, because it is indented
        // four spaces:
        [TestMethod]
        [TestCategory("Leaf blocks - Link reference definitions")]
        //[Timeout(1000)]
        public void Example113()
        {
            // Example 113
            // Section: Leaf blocks - Link reference definitions
            //
            // The following CommonMark:
            //         [foo]: /url "title"
            //     
            //     [foo]
            //
            // Should be rendered as:
            //     <pre><code>[foo]: /url &quot;title&quot;
            //     </code></pre>
            //     <p>[foo]</p>

            // Arrange
            var commonMark = Helpers.Normalize("    [foo]: /url \"title\"\n\n[foo]");
            var expected = Helpers.Normalize("<pre><code>[foo]: /url &quot;title&quot;\n</code></pre>\n<p>[foo]</p>");
            Helpers.Log("Example {0}", 113);
            Helpers.Log("Section: {0}", "Leaf blocks - Link reference definitions");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "    [foo]: /url \"title\"\n\n[foo]");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // This is not a link reference definition, because it occurs inside
        // a code block:
        [TestMethod]
        [TestCategory("Leaf blocks - Link reference definitions")]
        //[Timeout(1000)]
        public void Example114()
        {
            // Example 114
            // Section: Leaf blocks - Link reference definitions
            //
            // The following CommonMark:
            //     ```
            //     [foo]: /url
            //     ```
            //     
            //     [foo]
            //
            // Should be rendered as:
            //     <pre><code>[foo]: /url
            //     </code></pre>
            //     <p>[foo]</p>

            // Arrange
            var commonMark = Helpers.Normalize("```\n[foo]: /url\n```\n\n[foo]");
            var expected = Helpers.Normalize("<pre><code>[foo]: /url\n</code></pre>\n<p>[foo]</p>");
            Helpers.Log("Example {0}", 114);
            Helpers.Log("Section: {0}", "Leaf blocks - Link reference definitions");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "```\n[foo]: /url\n```\n\n[foo]");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // A [link reference definition](#link-reference-definition) cannot
        // interrupt a paragraph.
        [TestMethod]
        [TestCategory("Leaf blocks - Link reference definitions")]
        //[Timeout(1000)]
        public void Example115()
        {
            // Example 115
            // Section: Leaf blocks - Link reference definitions
            //
            // The following CommonMark:
            //     Foo
            //     [bar]: /baz
            //     
            //     [bar]
            //
            // Should be rendered as:
            //     <p>Foo
            //     [bar]: /baz</p>
            //     <p>[bar]</p>

            // Arrange
            var commonMark = Helpers.Normalize("Foo\n[bar]: /baz\n\n[bar]");
            var expected = Helpers.Normalize("<p>Foo\n[bar]: /baz</p>\n<p>[bar]</p>");
            Helpers.Log("Example {0}", 115);
            Helpers.Log("Section: {0}", "Leaf blocks - Link reference definitions");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "Foo\n[bar]: /baz\n\n[bar]");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // However, it can directly follow other block elements, such as headers
        // and horizontal rules, and it need not be followed by a blank line.
        [TestMethod]
        [TestCategory("Leaf blocks - Link reference definitions")]
        //[Timeout(1000)]
        public void Example116()
        {
            // Example 116
            // Section: Leaf blocks - Link reference definitions
            //
            // The following CommonMark:
            //     # [Foo]
            //     [foo]: /url
            //     > bar
            //
            // Should be rendered as:
            //     <h1><a href="/url">Foo</a></h1>
            //     <blockquote>
            //     <p>bar</p>
            //     </blockquote>

            // Arrange
            var commonMark = Helpers.Normalize("# [Foo]\n[foo]: /url\n> bar");
            var expected = Helpers.Normalize("<h1><a href=\"/url\">Foo</a></h1>\n<blockquote>\n<p>bar</p>\n</blockquote>");
            Helpers.Log("Example {0}", 116);
            Helpers.Log("Section: {0}", "Leaf blocks - Link reference definitions");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "# [Foo]\n[foo]: /url\n> bar");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // Several [link references](#link-reference) can occur one after another,
        // without intervening blank lines.
        [TestMethod]
        [TestCategory("Leaf blocks - Link reference definitions")]
        //[Timeout(1000)]
        public void Example117()
        {
            // Example 117
            // Section: Leaf blocks - Link reference definitions
            //
            // The following CommonMark:
            //     [foo]: /foo-url "foo"
            //     [bar]: /bar-url
            //       "bar"
            //     [baz]: /baz-url
            //     
            //     [foo],
            //     [bar],
            //     [baz]
            //
            // Should be rendered as:
            //     <p><a href="/foo-url" title="foo">foo</a>,
            //     <a href="/bar-url" title="bar">bar</a>,
            //     <a href="/baz-url">baz</a></p>

            // Arrange
            var commonMark = Helpers.Normalize("[foo]: /foo-url \"foo\"\n[bar]: /bar-url\n  \"bar\"\n[baz]: /baz-url\n\n[foo],\n[bar],\n[baz]");
            var expected = Helpers.Normalize("<p><a href=\"/foo-url\" title=\"foo\">foo</a>,\n<a href=\"/bar-url\" title=\"bar\">bar</a>,\n<a href=\"/baz-url\">baz</a></p>");
            Helpers.Log("Example {0}", 117);
            Helpers.Log("Section: {0}", "Leaf blocks - Link reference definitions");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "[foo]: /foo-url \"foo\"\n[bar]: /bar-url\n  \"bar\"\n[baz]: /baz-url\n\n[foo],\n[bar],\n[baz]");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // [Link reference definitions](#link-reference-definition) can occur
        // inside block containers, like lists and block quotations.  They
        // affect the entire document, not just the container in which they
        // are defined:
        [TestMethod]
        [TestCategory("Leaf blocks - Link reference definitions")]
        //[Timeout(1000)]
        public void Example118()
        {
            // Example 118
            // Section: Leaf blocks - Link reference definitions
            //
            // The following CommonMark:
            //     [foo]
            //     
            //     > [foo]: /url
            //
            // Should be rendered as:
            //     <p><a href="/url">foo</a></p>
            //     <blockquote>
            //     </blockquote>

            // Arrange
            var commonMark = Helpers.Normalize("[foo]\n\n> [foo]: /url");
            var expected = Helpers.Normalize("<p><a href=\"/url\">foo</a></p>\n<blockquote>\n</blockquote>");
            Helpers.Log("Example {0}", 118);
            Helpers.Log("Section: {0}", "Leaf blocks - Link reference definitions");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "[foo]\n\n> [foo]: /url");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // ## Paragraphs
        //
        // A sequence of non-blank lines that cannot be interpreted as other
        // kinds of blocks forms a [paragraph](#paragraph).<a id="paragraph"></a>
        // The contents of the paragraph are the result of parsing the
        // paragraph's raw content as inlines.  The paragraph's raw content
        // is formed by concatenating the lines and removing initial and final
        // spaces.
        //
        // A simple example with two paragraphs:
        [TestMethod]
        [TestCategory("Leaf blocks - Paragraphs")]
        //[Timeout(1000)]
        public void Example119()
        {
            // Example 119
            // Section: Leaf blocks - Paragraphs
            //
            // The following CommonMark:
            //     aaa
            //     
            //     bbb
            //
            // Should be rendered as:
            //     <p>aaa</p>
            //     <p>bbb</p>

            // Arrange
            var commonMark = Helpers.Normalize("aaa\n\nbbb");
            var expected = Helpers.Normalize("<p>aaa</p>\n<p>bbb</p>");
            Helpers.Log("Example {0}", 119);
            Helpers.Log("Section: {0}", "Leaf blocks - Paragraphs");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "aaa\n\nbbb");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // Paragraphs can contain multiple lines, but no blank lines:
        [TestMethod]
        [TestCategory("Leaf blocks - Paragraphs")]
        //[Timeout(1000)]
        public void Example120()
        {
            // Example 120
            // Section: Leaf blocks - Paragraphs
            //
            // The following CommonMark:
            //     aaa
            //     bbb
            //     
            //     ccc
            //     ddd
            //
            // Should be rendered as:
            //     <p>aaa
            //     bbb</p>
            //     <p>ccc
            //     ddd</p>

            // Arrange
            var commonMark = Helpers.Normalize("aaa\nbbb\n\nccc\nddd");
            var expected = Helpers.Normalize("<p>aaa\nbbb</p>\n<p>ccc\nddd</p>");
            Helpers.Log("Example {0}", 120);
            Helpers.Log("Section: {0}", "Leaf blocks - Paragraphs");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "aaa\nbbb\n\nccc\nddd");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // Multiple blank lines between paragraph have no effect:
        [TestMethod]
        [TestCategory("Leaf blocks - Paragraphs")]
        //[Timeout(1000)]
        public void Example121()
        {
            // Example 121
            // Section: Leaf blocks - Paragraphs
            //
            // The following CommonMark:
            //     aaa
            //     
            //     
            //     bbb
            //
            // Should be rendered as:
            //     <p>aaa</p>
            //     <p>bbb</p>

            // Arrange
            var commonMark = Helpers.Normalize("aaa\n\n\nbbb");
            var expected = Helpers.Normalize("<p>aaa</p>\n<p>bbb</p>");
            Helpers.Log("Example {0}", 121);
            Helpers.Log("Section: {0}", "Leaf blocks - Paragraphs");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "aaa\n\n\nbbb");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // Leading spaces are skipped:
        [TestMethod]
        [TestCategory("Leaf blocks - Paragraphs")]
        //[Timeout(1000)]
        public void Example122()
        {
            // Example 122
            // Section: Leaf blocks - Paragraphs
            //
            // The following CommonMark:
            //       aaa
            //      bbb
            //
            // Should be rendered as:
            //     <p>aaa
            //     bbb</p>

            // Arrange
            var commonMark = Helpers.Normalize("  aaa\n bbb");
            var expected = Helpers.Normalize("<p>aaa\nbbb</p>");
            Helpers.Log("Example {0}", 122);
            Helpers.Log("Section: {0}", "Leaf blocks - Paragraphs");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "  aaa\n bbb");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // Lines after the first may be indented any amount, since indented
        // code blocks cannot interrupt paragraphs.
        [TestMethod]
        [TestCategory("Leaf blocks - Paragraphs")]
        //[Timeout(1000)]
        public void Example123()
        {
            // Example 123
            // Section: Leaf blocks - Paragraphs
            //
            // The following CommonMark:
            //     aaa
            //                  bbb
            //                                            ccc
            //
            // Should be rendered as:
            //     <p>aaa
            //     bbb
            //     ccc</p>

            // Arrange
            var commonMark = Helpers.Normalize("aaa\n             bbb\n                                       ccc");
            var expected = Helpers.Normalize("<p>aaa\nbbb\nccc</p>");
            Helpers.Log("Example {0}", 123);
            Helpers.Log("Section: {0}", "Leaf blocks - Paragraphs");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "aaa\n             bbb\n                                       ccc");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // However, the first line may be indented at most three spaces,
        // or an indented code block will be triggered:
        [TestMethod]
        [TestCategory("Leaf blocks - Paragraphs")]
        //[Timeout(1000)]
        public void Example124()
        {
            // Example 124
            // Section: Leaf blocks - Paragraphs
            //
            // The following CommonMark:
            //        aaa
            //     bbb
            //
            // Should be rendered as:
            //     <p>aaa
            //     bbb</p>

            // Arrange
            var commonMark = Helpers.Normalize("   aaa\nbbb");
            var expected = Helpers.Normalize("<p>aaa\nbbb</p>");
            Helpers.Log("Example {0}", 124);
            Helpers.Log("Section: {0}", "Leaf blocks - Paragraphs");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "   aaa\nbbb");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        [TestMethod]
        [TestCategory("Leaf blocks - Paragraphs")]
        //[Timeout(1000)]
        public void Example125()
        {
            // Example 125
            // Section: Leaf blocks - Paragraphs
            //
            // The following CommonMark:
            //         aaa
            //     bbb
            //
            // Should be rendered as:
            //     <pre><code>aaa
            //     </code></pre>
            //     <p>bbb</p>

            // Arrange
            var commonMark = Helpers.Normalize("    aaa\nbbb");
            var expected = Helpers.Normalize("<pre><code>aaa\n</code></pre>\n<p>bbb</p>");
            Helpers.Log("Example {0}", 125);
            Helpers.Log("Section: {0}", "Leaf blocks - Paragraphs");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "    aaa\nbbb");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // Final spaces are stripped before inline parsing, so a paragraph
        // that ends with two or more spaces will not end with a hard line
        // break:
        [TestMethod]
        [TestCategory("Leaf blocks - Paragraphs")]
        //[Timeout(1000)]
        public void Example126()
        {
            // Example 126
            // Section: Leaf blocks - Paragraphs
            //
            // The following CommonMark:
            //     aaa     
            //     bbb     
            //
            // Should be rendered as:
            //     <p>aaa<br />
            //     bbb</p>

            // Arrange
            var commonMark = Helpers.Normalize("aaa     \nbbb     ");
            var expected = Helpers.Normalize("<p>aaa<br />\nbbb</p>");
            Helpers.Log("Example {0}", 126);
            Helpers.Log("Section: {0}", "Leaf blocks - Paragraphs");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "aaa     \nbbb     ");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // ## Blank lines
        //
        // [Blank lines](#blank-line) between block-level elements are ignored,
        // except for the role they play in determining whether a [list](#list)
        // is [tight](#tight) or [loose](#loose).
        //
        // Blank lines at the beginning and end of the document are also ignored.
        [TestMethod]
        [TestCategory("Leaf blocks - Blank lines")]
        //[Timeout(1000)]
        public void Example127()
        {
            // Example 127
            // Section: Leaf blocks - Blank lines
            //
            // The following CommonMark:
            //       
            //     
            //     aaa
            //       
            //     
            //     # aaa
            //     
            //       
            //
            // Should be rendered as:
            //     <p>aaa</p>
            //     <h1>aaa</h1>

            // Arrange
            var commonMark = Helpers.Normalize("  \n\naaa\n  \n\n# aaa\n\n  ");
            var expected = Helpers.Normalize("<p>aaa</p>\n<h1>aaa</h1>");
            Helpers.Log("Example {0}", 127);
            Helpers.Log("Section: {0}", "Leaf blocks - Blank lines");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "  \n\naaa\n  \n\n# aaa\n\n  ");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // # Container blocks
        //
        // A [container block](#container-block) is a block that has other
        // blocks as its contents.  There are two basic kinds of container blocks:
        // [block quotes](#block-quote) and [list items](#list-item).
        // [Lists](#list) are meta-containers for [list items](#list-item).
        //
        // We define the syntax for container blocks recursively.  The general
        // form of the definition is:
        //
        // > If X is a sequence of blocks, then the result of
        // > transforming X in such-and-such a way is a container of type Y
        // > with these blocks as its content.
        //
        // So, we explain what counts as a block quote or list item by
        // explaining how these can be *generated* from their contents.
        // This should suffice to define the syntax, although it does not
        // give a recipe for *parsing* these constructions.  (A recipe is
        // provided below in the section entitled [A parsing strategy].)
        //
        // ## Block quotes
        //
        // A [block quote marker](#block-quote-marker) <a id="block-quote-marker"></a>
        // consists of 0-3 spaces of initial indent, plus (a) the character `>` together
        // with a following space, or (b) a single character `>` not followed by a space.
        //
        // The following rules define [block quotes](#block-quote):
        // <a id="block-quote"></a>
        //
        // 1.  **Basic case.**  If a string of lines *Ls* constitute a sequence
        // of blocks *Bs*, then the result of appending a [block quote marker]
        // to the beginning of each line in *Ls* is a [block quote](#block-quote)
        // containing *Bs*.
        //
        // 2.  **Laziness.**  If a string of lines *Ls* constitute a [block
        // quote](#block-quote) with contents *Bs*, then the result of deleting
        // the initial [block quote marker](#block-quote-marker) from one or
        // more lines in which the next non-space character after the [block
        // quote marker](#block-quote-marker) is [paragraph continuation
        // text](#paragraph-continuation-text) is a block quote with *Bs* as
        // its content.  <a id="paragraph-continuation-text"></a>
        // [Paragraph continuation text](#paragraph-continuation-text) is text
        // that will be parsed as part of the content of a paragraph, but does
        // not occur at the beginning of the paragraph.
        //
        // 3.  **Consecutiveness.**  A document cannot contain two [block
        // quotes](#block-quote) in a row unless there is a [blank
        // line](#blank-line) between them.
        //
        // Nothing else counts as a [block quote](#block-quote).
        //
        // Here is a simple example:
        [TestMethod]
        [TestCategory("Container blocks - Block quotes")]
        //[Timeout(1000)]
        public void Example128()
        {
            // Example 128
            // Section: Container blocks - Block quotes
            //
            // The following CommonMark:
            //     > # Foo
            //     > bar
            //     > baz
            //
            // Should be rendered as:
            //     <blockquote>
            //     <h1>Foo</h1>
            //     <p>bar
            //     baz</p>
            //     </blockquote>

            // Arrange
            var commonMark = Helpers.Normalize("> # Foo\n> bar\n> baz");
            var expected = Helpers.Normalize("<blockquote>\n<h1>Foo</h1>\n<p>bar\nbaz</p>\n</blockquote>");
            Helpers.Log("Example {0}", 128);
            Helpers.Log("Section: {0}", "Container blocks - Block quotes");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "> # Foo\n> bar\n> baz");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // The spaces after the `>` characters can be omitted:
        [TestMethod]
        [TestCategory("Container blocks - Block quotes")]
        //[Timeout(1000)]
        public void Example129()
        {
            // Example 129
            // Section: Container blocks - Block quotes
            //
            // The following CommonMark:
            //     ># Foo
            //     >bar
            //     > baz
            //
            // Should be rendered as:
            //     <blockquote>
            //     <h1>Foo</h1>
            //     <p>bar
            //     baz</p>
            //     </blockquote>

            // Arrange
            var commonMark = Helpers.Normalize("># Foo\n>bar\n> baz");
            var expected = Helpers.Normalize("<blockquote>\n<h1>Foo</h1>\n<p>bar\nbaz</p>\n</blockquote>");
            Helpers.Log("Example {0}", 129);
            Helpers.Log("Section: {0}", "Container blocks - Block quotes");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "># Foo\n>bar\n> baz");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // The `>` characters can be indented 1-3 spaces:
        [TestMethod]
        [TestCategory("Container blocks - Block quotes")]
        //[Timeout(1000)]
        public void Example130()
        {
            // Example 130
            // Section: Container blocks - Block quotes
            //
            // The following CommonMark:
            //        > # Foo
            //        > bar
            //      > baz
            //
            // Should be rendered as:
            //     <blockquote>
            //     <h1>Foo</h1>
            //     <p>bar
            //     baz</p>
            //     </blockquote>

            // Arrange
            var commonMark = Helpers.Normalize("   > # Foo\n   > bar\n > baz");
            var expected = Helpers.Normalize("<blockquote>\n<h1>Foo</h1>\n<p>bar\nbaz</p>\n</blockquote>");
            Helpers.Log("Example {0}", 130);
            Helpers.Log("Section: {0}", "Container blocks - Block quotes");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "   > # Foo\n   > bar\n > baz");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // Four spaces gives us a code block:
        [TestMethod]
        [TestCategory("Container blocks - Block quotes")]
        //[Timeout(1000)]
        public void Example131()
        {
            // Example 131
            // Section: Container blocks - Block quotes
            //
            // The following CommonMark:
            //         > # Foo
            //         > bar
            //         > baz
            //
            // Should be rendered as:
            //     <pre><code>&gt; # Foo
            //     &gt; bar
            //     &gt; baz
            //     </code></pre>

            // Arrange
            var commonMark = Helpers.Normalize("    > # Foo\n    > bar\n    > baz");
            var expected = Helpers.Normalize("<pre><code>&gt; # Foo\n&gt; bar\n&gt; baz\n</code></pre>");
            Helpers.Log("Example {0}", 131);
            Helpers.Log("Section: {0}", "Container blocks - Block quotes");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "    > # Foo\n    > bar\n    > baz");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // The Laziness clause allows us to omit the `>` before a
        // paragraph continuation line:
        [TestMethod]
        [TestCategory("Container blocks - Block quotes")]
        //[Timeout(1000)]
        public void Example132()
        {
            // Example 132
            // Section: Container blocks - Block quotes
            //
            // The following CommonMark:
            //     > # Foo
            //     > bar
            //     baz
            //
            // Should be rendered as:
            //     <blockquote>
            //     <h1>Foo</h1>
            //     <p>bar
            //     baz</p>
            //     </blockquote>

            // Arrange
            var commonMark = Helpers.Normalize("> # Foo\n> bar\nbaz");
            var expected = Helpers.Normalize("<blockquote>\n<h1>Foo</h1>\n<p>bar\nbaz</p>\n</blockquote>");
            Helpers.Log("Example {0}", 132);
            Helpers.Log("Section: {0}", "Container blocks - Block quotes");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "> # Foo\n> bar\nbaz");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // A block quote can contain some lazy and some non-lazy
        // continuation lines:
        [TestMethod]
        [TestCategory("Container blocks - Block quotes")]
        //[Timeout(1000)]
        public void Example133()
        {
            // Example 133
            // Section: Container blocks - Block quotes
            //
            // The following CommonMark:
            //     > bar
            //     baz
            //     > foo
            //
            // Should be rendered as:
            //     <blockquote>
            //     <p>bar
            //     baz
            //     foo</p>
            //     </blockquote>

            // Arrange
            var commonMark = Helpers.Normalize("> bar\nbaz\n> foo");
            var expected = Helpers.Normalize("<blockquote>\n<p>bar\nbaz\nfoo</p>\n</blockquote>");
            Helpers.Log("Example {0}", 133);
            Helpers.Log("Section: {0}", "Container blocks - Block quotes");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "> bar\nbaz\n> foo");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // Laziness only applies to lines that are continuations of
        // paragraphs. Lines containing characters or indentation that indicate
        // block structure cannot be lazy.
        [TestMethod]
        [TestCategory("Container blocks - Block quotes")]
        //[Timeout(1000)]
        public void Example134()
        {
            // Example 134
            // Section: Container blocks - Block quotes
            //
            // The following CommonMark:
            //     > foo
            //     ---
            //
            // Should be rendered as:
            //     <blockquote>
            //     <p>foo</p>
            //     </blockquote>
            //     <hr />

            // Arrange
            var commonMark = Helpers.Normalize("> foo\n---");
            var expected = Helpers.Normalize("<blockquote>\n<p>foo</p>\n</blockquote>\n<hr />");
            Helpers.Log("Example {0}", 134);
            Helpers.Log("Section: {0}", "Container blocks - Block quotes");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "> foo\n---");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        [TestMethod]
        [TestCategory("Container blocks - Block quotes")]
        //[Timeout(1000)]
        public void Example135()
        {
            // Example 135
            // Section: Container blocks - Block quotes
            //
            // The following CommonMark:
            //     > - foo
            //     - bar
            //
            // Should be rendered as:
            //     <blockquote>
            //     <ul>
            //     <li>foo</li>
            //     </ul>
            //     </blockquote>
            //     <ul>
            //     <li>bar</li>
            //     </ul>

            // Arrange
            var commonMark = Helpers.Normalize("> - foo\n- bar");
            var expected = Helpers.Normalize("<blockquote>\n<ul>\n<li>foo</li>\n</ul>\n</blockquote>\n<ul>\n<li>bar</li>\n</ul>");
            Helpers.Log("Example {0}", 135);
            Helpers.Log("Section: {0}", "Container blocks - Block quotes");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "> - foo\n- bar");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        [TestMethod]
        [TestCategory("Container blocks - Block quotes")]
        //[Timeout(1000)]
        public void Example136()
        {
            // Example 136
            // Section: Container blocks - Block quotes
            //
            // The following CommonMark:
            //     >     foo
            //         bar
            //
            // Should be rendered as:
            //     <blockquote>
            //     <pre><code>foo
            //     </code></pre>
            //     </blockquote>
            //     <pre><code>bar
            //     </code></pre>

            // Arrange
            var commonMark = Helpers.Normalize(">     foo\n    bar");
            var expected = Helpers.Normalize("<blockquote>\n<pre><code>foo\n</code></pre>\n</blockquote>\n<pre><code>bar\n</code></pre>");
            Helpers.Log("Example {0}", 136);
            Helpers.Log("Section: {0}", "Container blocks - Block quotes");
            Helpers.Log();
            Helpers.LogValue("CommonMark", ">     foo\n    bar");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        [TestMethod]
        [TestCategory("Container blocks - Block quotes")]
        //[Timeout(1000)]
        public void Example137()
        {
            // Example 137
            // Section: Container blocks - Block quotes
            //
            // The following CommonMark:
            //     > ```
            //     foo
            //     ```
            //
            // Should be rendered as:
            //     <blockquote>
            //     <pre><code></code></pre>
            //     </blockquote>
            //     <p>foo</p>
            //     <pre><code></code></pre>

            // Arrange
            var commonMark = Helpers.Normalize("> ```\nfoo\n```");
            var expected = Helpers.Normalize("<blockquote>\n<pre><code></code></pre>\n</blockquote>\n<p>foo</p>\n<pre><code></code></pre>");
            Helpers.Log("Example {0}", 137);
            Helpers.Log("Section: {0}", "Container blocks - Block quotes");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "> ```\nfoo\n```");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // A block quote can be empty:
        [TestMethod]
        [TestCategory("Container blocks - Block quotes")]
        //[Timeout(1000)]
        public void Example138()
        {
            // Example 138
            // Section: Container blocks - Block quotes
            //
            // The following CommonMark:
            //     >
            //
            // Should be rendered as:
            //     <blockquote>
            //     </blockquote>

            // Arrange
            var commonMark = Helpers.Normalize(">");
            var expected = Helpers.Normalize("<blockquote>\n</blockquote>");
            Helpers.Log("Example {0}", 138);
            Helpers.Log("Section: {0}", "Container blocks - Block quotes");
            Helpers.Log();
            Helpers.LogValue("CommonMark", ">");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        [TestMethod]
        [TestCategory("Container blocks - Block quotes")]
        //[Timeout(1000)]
        public void Example139()
        {
            // Example 139
            // Section: Container blocks - Block quotes
            //
            // The following CommonMark:
            //     >
            //     >  
            //     > 
            //
            // Should be rendered as:
            //     <blockquote>
            //     </blockquote>

            // Arrange
            var commonMark = Helpers.Normalize(">\n>  \n> ");
            var expected = Helpers.Normalize("<blockquote>\n</blockquote>");
            Helpers.Log("Example {0}", 139);
            Helpers.Log("Section: {0}", "Container blocks - Block quotes");
            Helpers.Log();
            Helpers.LogValue("CommonMark", ">\n>  \n> ");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // A block quote can have initial or final blank lines:
        [TestMethod]
        [TestCategory("Container blocks - Block quotes")]
        //[Timeout(1000)]
        public void Example140()
        {
            // Example 140
            // Section: Container blocks - Block quotes
            //
            // The following CommonMark:
            //     >
            //     > foo
            //     >  
            //
            // Should be rendered as:
            //     <blockquote>
            //     <p>foo</p>
            //     </blockquote>

            // Arrange
            var commonMark = Helpers.Normalize(">\n> foo\n>  ");
            var expected = Helpers.Normalize("<blockquote>\n<p>foo</p>\n</blockquote>");
            Helpers.Log("Example {0}", 140);
            Helpers.Log("Section: {0}", "Container blocks - Block quotes");
            Helpers.Log();
            Helpers.LogValue("CommonMark", ">\n> foo\n>  ");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // A blank line always separates block quotes:
        [TestMethod]
        [TestCategory("Container blocks - Block quotes")]
        //[Timeout(1000)]
        public void Example141()
        {
            // Example 141
            // Section: Container blocks - Block quotes
            //
            // The following CommonMark:
            //     > foo
            //     
            //     > bar
            //
            // Should be rendered as:
            //     <blockquote>
            //     <p>foo</p>
            //     </blockquote>
            //     <blockquote>
            //     <p>bar</p>
            //     </blockquote>

            // Arrange
            var commonMark = Helpers.Normalize("> foo\n\n> bar");
            var expected = Helpers.Normalize("<blockquote>\n<p>foo</p>\n</blockquote>\n<blockquote>\n<p>bar</p>\n</blockquote>");
            Helpers.Log("Example {0}", 141);
            Helpers.Log("Section: {0}", "Container blocks - Block quotes");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "> foo\n\n> bar");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // (Most current Markdown implementations, including John Gruber's
        // original `Markdown.pl`, will parse this example as a single block quote
        // with two paragraphs.  But it seems better to allow the author to decide
        // whether two block quotes or one are wanted.)
        //
        // Consecutiveness means that if we put these block quotes together,
        // we get a single block quote:
        [TestMethod]
        [TestCategory("Container blocks - Block quotes")]
        //[Timeout(1000)]
        public void Example142()
        {
            // Example 142
            // Section: Container blocks - Block quotes
            //
            // The following CommonMark:
            //     > foo
            //     > bar
            //
            // Should be rendered as:
            //     <blockquote>
            //     <p>foo
            //     bar</p>
            //     </blockquote>

            // Arrange
            var commonMark = Helpers.Normalize("> foo\n> bar");
            var expected = Helpers.Normalize("<blockquote>\n<p>foo\nbar</p>\n</blockquote>");
            Helpers.Log("Example {0}", 142);
            Helpers.Log("Section: {0}", "Container blocks - Block quotes");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "> foo\n> bar");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // To get a block quote with two paragraphs, use:
        [TestMethod]
        [TestCategory("Container blocks - Block quotes")]
        //[Timeout(1000)]
        public void Example143()
        {
            // Example 143
            // Section: Container blocks - Block quotes
            //
            // The following CommonMark:
            //     > foo
            //     >
            //     > bar
            //
            // Should be rendered as:
            //     <blockquote>
            //     <p>foo</p>
            //     <p>bar</p>
            //     </blockquote>

            // Arrange
            var commonMark = Helpers.Normalize("> foo\n>\n> bar");
            var expected = Helpers.Normalize("<blockquote>\n<p>foo</p>\n<p>bar</p>\n</blockquote>");
            Helpers.Log("Example {0}", 143);
            Helpers.Log("Section: {0}", "Container blocks - Block quotes");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "> foo\n>\n> bar");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // Block quotes can interrupt paragraphs:
        [TestMethod]
        [TestCategory("Container blocks - Block quotes")]
        //[Timeout(1000)]
        public void Example144()
        {
            // Example 144
            // Section: Container blocks - Block quotes
            //
            // The following CommonMark:
            //     foo
            //     > bar
            //
            // Should be rendered as:
            //     <p>foo</p>
            //     <blockquote>
            //     <p>bar</p>
            //     </blockquote>

            // Arrange
            var commonMark = Helpers.Normalize("foo\n> bar");
            var expected = Helpers.Normalize("<p>foo</p>\n<blockquote>\n<p>bar</p>\n</blockquote>");
            Helpers.Log("Example {0}", 144);
            Helpers.Log("Section: {0}", "Container blocks - Block quotes");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "foo\n> bar");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // In general, blank lines are not needed before or after block
        // quotes:
        [TestMethod]
        [TestCategory("Container blocks - Block quotes")]
        //[Timeout(1000)]
        public void Example145()
        {
            // Example 145
            // Section: Container blocks - Block quotes
            //
            // The following CommonMark:
            //     > aaa
            //     ***
            //     > bbb
            //
            // Should be rendered as:
            //     <blockquote>
            //     <p>aaa</p>
            //     </blockquote>
            //     <hr />
            //     <blockquote>
            //     <p>bbb</p>
            //     </blockquote>

            // Arrange
            var commonMark = Helpers.Normalize("> aaa\n***\n> bbb");
            var expected = Helpers.Normalize("<blockquote>\n<p>aaa</p>\n</blockquote>\n<hr />\n<blockquote>\n<p>bbb</p>\n</blockquote>");
            Helpers.Log("Example {0}", 145);
            Helpers.Log("Section: {0}", "Container blocks - Block quotes");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "> aaa\n***\n> bbb");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // However, because of laziness, a blank line is needed between
        // a block quote and a following paragraph:
        [TestMethod]
        [TestCategory("Container blocks - Block quotes")]
        //[Timeout(1000)]
        public void Example146()
        {
            // Example 146
            // Section: Container blocks - Block quotes
            //
            // The following CommonMark:
            //     > bar
            //     baz
            //
            // Should be rendered as:
            //     <blockquote>
            //     <p>bar
            //     baz</p>
            //     </blockquote>

            // Arrange
            var commonMark = Helpers.Normalize("> bar\nbaz");
            var expected = Helpers.Normalize("<blockquote>\n<p>bar\nbaz</p>\n</blockquote>");
            Helpers.Log("Example {0}", 146);
            Helpers.Log("Section: {0}", "Container blocks - Block quotes");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "> bar\nbaz");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        [TestMethod]
        [TestCategory("Container blocks - Block quotes")]
        //[Timeout(1000)]
        public void Example147()
        {
            // Example 147
            // Section: Container blocks - Block quotes
            //
            // The following CommonMark:
            //     > bar
            //     
            //     baz
            //
            // Should be rendered as:
            //     <blockquote>
            //     <p>bar</p>
            //     </blockquote>
            //     <p>baz</p>

            // Arrange
            var commonMark = Helpers.Normalize("> bar\n\nbaz");
            var expected = Helpers.Normalize("<blockquote>\n<p>bar</p>\n</blockquote>\n<p>baz</p>");
            Helpers.Log("Example {0}", 147);
            Helpers.Log("Section: {0}", "Container blocks - Block quotes");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "> bar\n\nbaz");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        [TestMethod]
        [TestCategory("Container blocks - Block quotes")]
        //[Timeout(1000)]
        public void Example148()
        {
            // Example 148
            // Section: Container blocks - Block quotes
            //
            // The following CommonMark:
            //     > bar
            //     >
            //     baz
            //
            // Should be rendered as:
            //     <blockquote>
            //     <p>bar</p>
            //     </blockquote>
            //     <p>baz</p>

            // Arrange
            var commonMark = Helpers.Normalize("> bar\n>\nbaz");
            var expected = Helpers.Normalize("<blockquote>\n<p>bar</p>\n</blockquote>\n<p>baz</p>");
            Helpers.Log("Example {0}", 148);
            Helpers.Log("Section: {0}", "Container blocks - Block quotes");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "> bar\n>\nbaz");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // It is a consequence of the Laziness rule that any number
        // of initial `>`s may be omitted on a continuation line of a
        // nested block quote:
        [TestMethod]
        [TestCategory("Container blocks - Block quotes")]
        //[Timeout(1000)]
        public void Example149()
        {
            // Example 149
            // Section: Container blocks - Block quotes
            //
            // The following CommonMark:
            //     > > > foo
            //     bar
            //
            // Should be rendered as:
            //     <blockquote>
            //     <blockquote>
            //     <blockquote>
            //     <p>foo
            //     bar</p>
            //     </blockquote>
            //     </blockquote>
            //     </blockquote>

            // Arrange
            var commonMark = Helpers.Normalize("> > > foo\nbar");
            var expected = Helpers.Normalize("<blockquote>\n<blockquote>\n<blockquote>\n<p>foo\nbar</p>\n</blockquote>\n</blockquote>\n</blockquote>");
            Helpers.Log("Example {0}", 149);
            Helpers.Log("Section: {0}", "Container blocks - Block quotes");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "> > > foo\nbar");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        [TestMethod]
        [TestCategory("Container blocks - Block quotes")]
        //[Timeout(1000)]
        public void Example150()
        {
            // Example 150
            // Section: Container blocks - Block quotes
            //
            // The following CommonMark:
            //     >>> foo
            //     > bar
            //     >>baz
            //
            // Should be rendered as:
            //     <blockquote>
            //     <blockquote>
            //     <blockquote>
            //     <p>foo
            //     bar
            //     baz</p>
            //     </blockquote>
            //     </blockquote>
            //     </blockquote>

            // Arrange
            var commonMark = Helpers.Normalize(">>> foo\n> bar\n>>baz");
            var expected = Helpers.Normalize("<blockquote>\n<blockquote>\n<blockquote>\n<p>foo\nbar\nbaz</p>\n</blockquote>\n</blockquote>\n</blockquote>");
            Helpers.Log("Example {0}", 150);
            Helpers.Log("Section: {0}", "Container blocks - Block quotes");
            Helpers.Log();
            Helpers.LogValue("CommonMark", ">>> foo\n> bar\n>>baz");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // When including an indented code block in a block quote,
        // remember that the [block quote marker](#block-quote-marker) includes
        // both the `>` and a following space.  So *five spaces* are needed after
        // the `>`:
        [TestMethod]
        [TestCategory("Container blocks - Block quotes")]
        //[Timeout(1000)]
        public void Example151()
        {
            // Example 151
            // Section: Container blocks - Block quotes
            //
            // The following CommonMark:
            //     >     code
            //     
            //     >    not code
            //
            // Should be rendered as:
            //     <blockquote>
            //     <pre><code>code
            //     </code></pre>
            //     </blockquote>
            //     <blockquote>
            //     <p>not code</p>
            //     </blockquote>

            // Arrange
            var commonMark = Helpers.Normalize(">     code\n\n>    not code");
            var expected = Helpers.Normalize("<blockquote>\n<pre><code>code\n</code></pre>\n</blockquote>\n<blockquote>\n<p>not code</p>\n</blockquote>");
            Helpers.Log("Example {0}", 151);
            Helpers.Log("Section: {0}", "Container blocks - Block quotes");
            Helpers.Log();
            Helpers.LogValue("CommonMark", ">     code\n\n>    not code");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // ## List items
        //
        // A [list marker](#list-marker) <a id="list-marker"></a> is a
        // [bullet list marker](#bullet-list-marker) or an [ordered list
        // marker](#ordered-list-marker).
        //
        // A [bullet list marker](#bullet-list-marker) <a id="bullet-list-marker"></a>
        // is a `-`, `+`, or `*` character.
        //
        // An [ordered list marker](#ordered-list-marker) <a id="ordered-list-marker"></a>
        // is a sequence of one of more digits (`0-9`), followed by either a
        // `.` character or a `)` character.
        //
        // The following rules define [list items](#list-item):
        //
        // 1.  **Basic case.**  If a sequence of lines *Ls* constitute a sequence of
        // blocks *Bs* starting with a non-space character and not separated
        // from each other by more than one blank line, and *M* is a list
        // marker *M* of width *W* followed by 0 < *N* < 5 spaces, then the result
        // of prepending *M* and the following spaces to the first line of
        // *Ls*, and indenting subsequent lines of *Ls* by *W + N* spaces, is a
        // list item with *Bs* as its contents.  The type of the list item
        // (bullet or ordered) is determined by the type of its list marker.
        // If the list item is ordered, then it is also assigned a start
        // number, based on the ordered list marker.
        //
        // For example, let *Ls* be the lines
        [TestMethod]
        [TestCategory("Container blocks - List items")]
        //[Timeout(1000)]
        public void Example152()
        {
            // Example 152
            // Section: Container blocks - List items
            //
            // The following CommonMark:
            //     A paragraph
            //     with two lines.
            //     
            //         indented code
            //     
            //     > A block quote.
            //
            // Should be rendered as:
            //     <p>A paragraph
            //     with two lines.</p>
            //     <pre><code>indented code
            //     </code></pre>
            //     <blockquote>
            //     <p>A block quote.</p>
            //     </blockquote>

            // Arrange
            var commonMark = Helpers.Normalize("A paragraph\nwith two lines.\n\n    indented code\n\n> A block quote.");
            var expected = Helpers.Normalize("<p>A paragraph\nwith two lines.</p>\n<pre><code>indented code\n</code></pre>\n<blockquote>\n<p>A block quote.</p>\n</blockquote>");
            Helpers.Log("Example {0}", 152);
            Helpers.Log("Section: {0}", "Container blocks - List items");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "A paragraph\nwith two lines.\n\n    indented code\n\n> A block quote.");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // And let *M* be the marker `1.`, and *N* = 2.  Then rule #1 says
        // that the following is an ordered list item with start number 1,
        // and the same contents as *Ls*:
        [TestMethod]
        [TestCategory("Container blocks - List items")]
        //[Timeout(1000)]
        public void Example153()
        {
            // Example 153
            // Section: Container blocks - List items
            //
            // The following CommonMark:
            //     1.  A paragraph
            //         with two lines.
            //     
            //             indented code
            //     
            //         > A block quote.
            //
            // Should be rendered as:
            //     <ol>
            //     <li><p>A paragraph
            //     with two lines.</p>
            //     <pre><code>indented code
            //     </code></pre>
            //     <blockquote>
            //     <p>A block quote.</p>
            //     </blockquote></li>
            //     </ol>

            // Arrange
            var commonMark = Helpers.Normalize("1.  A paragraph\n    with two lines.\n\n        indented code\n\n    > A block quote.");
            var expected = Helpers.Normalize("<ol>\n<li><p>A paragraph\nwith two lines.</p>\n<pre><code>indented code\n</code></pre>\n<blockquote>\n<p>A block quote.</p>\n</blockquote></li>\n</ol>");
            Helpers.Log("Example {0}", 153);
            Helpers.Log("Section: {0}", "Container blocks - List items");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "1.  A paragraph\n    with two lines.\n\n        indented code\n\n    > A block quote.");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // The most important thing to notice is that the position of
        // the text after the list marker determines how much indentation
        // is needed in subsequent blocks in the list item.  If the list
        // marker takes up two spaces, and there are three spaces between
        // the list marker and the next nonspace character, then blocks
        // must be indented five spaces in order to fall under the list
        // item.
        //
        // Here are some examples showing how far content must be indented to be
        // put under the list item:
        [TestMethod]
        [TestCategory("Container blocks - List items")]
        //[Timeout(1000)]
        public void Example154()
        {
            // Example 154
            // Section: Container blocks - List items
            //
            // The following CommonMark:
            //     - one
            //     
            //      two
            //
            // Should be rendered as:
            //     <ul>
            //     <li>one</li>
            //     </ul>
            //     <p>two</p>

            // Arrange
            var commonMark = Helpers.Normalize("- one\n\n two");
            var expected = Helpers.Normalize("<ul>\n<li>one</li>\n</ul>\n<p>two</p>");
            Helpers.Log("Example {0}", 154);
            Helpers.Log("Section: {0}", "Container blocks - List items");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "- one\n\n two");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        [TestMethod]
        [TestCategory("Container blocks - List items")]
        //[Timeout(1000)]
        public void Example155()
        {
            // Example 155
            // Section: Container blocks - List items
            //
            // The following CommonMark:
            //     - one
            //     
            //       two
            //
            // Should be rendered as:
            //     <ul>
            //     <li><p>one</p>
            //     <p>two</p></li>
            //     </ul>

            // Arrange
            var commonMark = Helpers.Normalize("- one\n\n  two");
            var expected = Helpers.Normalize("<ul>\n<li><p>one</p>\n<p>two</p></li>\n</ul>");
            Helpers.Log("Example {0}", 155);
            Helpers.Log("Section: {0}", "Container blocks - List items");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "- one\n\n  two");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        [TestMethod]
        [TestCategory("Container blocks - List items")]
        //[Timeout(1000)]
        public void Example156()
        {
            // Example 156
            // Section: Container blocks - List items
            //
            // The following CommonMark:
            //      -    one
            //     
            //          two
            //
            // Should be rendered as:
            //     <ul>
            //     <li>one</li>
            //     </ul>
            //     <pre><code> two
            //     </code></pre>

            // Arrange
            var commonMark = Helpers.Normalize(" -    one\n\n     two");
            var expected = Helpers.Normalize("<ul>\n<li>one</li>\n</ul>\n<pre><code> two\n</code></pre>");
            Helpers.Log("Example {0}", 156);
            Helpers.Log("Section: {0}", "Container blocks - List items");
            Helpers.Log();
            Helpers.LogValue("CommonMark", " -    one\n\n     two");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        [TestMethod]
        [TestCategory("Container blocks - List items")]
        //[Timeout(1000)]
        public void Example157()
        {
            // Example 157
            // Section: Container blocks - List items
            //
            // The following CommonMark:
            //      -    one
            //     
            //           two
            //
            // Should be rendered as:
            //     <ul>
            //     <li><p>one</p>
            //     <p>two</p></li>
            //     </ul>

            // Arrange
            var commonMark = Helpers.Normalize(" -    one\n\n      two");
            var expected = Helpers.Normalize("<ul>\n<li><p>one</p>\n<p>two</p></li>\n</ul>");
            Helpers.Log("Example {0}", 157);
            Helpers.Log("Section: {0}", "Container blocks - List items");
            Helpers.Log();
            Helpers.LogValue("CommonMark", " -    one\n\n      two");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // It is tempting to think of this in terms of columns:  the continuation
        // blocks must be indented at least to the column of the first nonspace
        // character after the list marker.  However, that is not quite right.
        // The spaces after the list marker determine how much relative indentation
        // is needed.  Which column this indentation reaches will depend on
        // how the list item is embedded in other constructions, as shown by
        // this example:
        [TestMethod]
        [TestCategory("Container blocks - List items")]
        //[Timeout(1000)]
        public void Example158()
        {
            // Example 158
            // Section: Container blocks - List items
            //
            // The following CommonMark:
            //        > > 1.  one
            //     >>
            //     >>     two
            //
            // Should be rendered as:
            //     <blockquote>
            //     <blockquote>
            //     <ol>
            //     <li><p>one</p>
            //     <p>two</p></li>
            //     </ol>
            //     </blockquote>
            //     </blockquote>

            // Arrange
            var commonMark = Helpers.Normalize("   > > 1.  one\n>>\n>>     two");
            var expected = Helpers.Normalize("<blockquote>\n<blockquote>\n<ol>\n<li><p>one</p>\n<p>two</p></li>\n</ol>\n</blockquote>\n</blockquote>");
            Helpers.Log("Example {0}", 158);
            Helpers.Log("Section: {0}", "Container blocks - List items");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "   > > 1.  one\n>>\n>>     two");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // Here `two` occurs in the same column as the list marker `1.`,
        // but is actually contained in the list item, because there is
        // sufficent indentation after the last containing blockquote marker.
        //
        // The converse is also possible.  In the following example, the word `two`
        // occurs far to the right of the initial text of the list item, `one`, but
        // it is not considered part of the list item, because it is not indented
        // far enough past the blockquote marker:
        [TestMethod]
        [TestCategory("Container blocks - List items")]
        //[Timeout(1000)]
        public void Example159()
        {
            // Example 159
            // Section: Container blocks - List items
            //
            // The following CommonMark:
            //     >>- one
            //     >>
            //       >  > two
            //
            // Should be rendered as:
            //     <blockquote>
            //     <blockquote>
            //     <ul>
            //     <li>one</li>
            //     </ul>
            //     <p>two</p>
            //     </blockquote>
            //     </blockquote>

            // Arrange
            var commonMark = Helpers.Normalize(">>- one\n>>\n  >  > two");
            var expected = Helpers.Normalize("<blockquote>\n<blockquote>\n<ul>\n<li>one</li>\n</ul>\n<p>two</p>\n</blockquote>\n</blockquote>");
            Helpers.Log("Example {0}", 159);
            Helpers.Log("Section: {0}", "Container blocks - List items");
            Helpers.Log();
            Helpers.LogValue("CommonMark", ">>- one\n>>\n  >  > two");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // A list item may not contain blocks that are separated by more than
        // one blank line.  Thus, two blank lines will end a list, unless the
        // two blanks are contained in a [fenced code block](#fenced-code-block).
        [TestMethod]
        [TestCategory("Container blocks - List items")]
        //[Timeout(1000)]
        public void Example160()
        {
            // Example 160
            // Section: Container blocks - List items
            //
            // The following CommonMark:
            //     - foo
            //     
            //       bar
            //     
            //     - foo
            //     
            //     
            //       bar
            //     
            //     - ```
            //       foo
            //     
            //     
            //       bar
            //       ```
            //
            // Should be rendered as:
            //     <ul>
            //     <li><p>foo</p>
            //     <p>bar</p></li>
            //     <li><p>foo</p></li>
            //     </ul>
            //     <p>bar</p>
            //     <ul>
            //     <li><pre><code>foo
            //     
            //     
            //     bar
            //     </code></pre></li>
            //     </ul>

            // Arrange
            var commonMark = Helpers.Normalize("- foo\n\n  bar\n\n- foo\n\n\n  bar\n\n- ```\n  foo\n\n\n  bar\n  ```");
            var expected = Helpers.Normalize("<ul>\n<li><p>foo</p>\n<p>bar</p></li>\n<li><p>foo</p></li>\n</ul>\n<p>bar</p>\n<ul>\n<li><pre><code>foo\n\n\nbar\n</code></pre></li>\n</ul>");
            Helpers.Log("Example {0}", 160);
            Helpers.Log("Section: {0}", "Container blocks - List items");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "- foo\n\n  bar\n\n- foo\n\n\n  bar\n\n- ```\n  foo\n\n\n  bar\n  ```");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // A list item may contain any kind of block:
        [TestMethod]
        [TestCategory("Container blocks - List items")]
        //[Timeout(1000)]
        public void Example161()
        {
            // Example 161
            // Section: Container blocks - List items
            //
            // The following CommonMark:
            //     1.  foo
            //     
            //         ```
            //         bar
            //         ```
            //     
            //         baz
            //     
            //         > bam
            //
            // Should be rendered as:
            //     <ol>
            //     <li><p>foo</p>
            //     <pre><code>bar
            //     </code></pre>
            //     <p>baz</p>
            //     <blockquote>
            //     <p>bam</p>
            //     </blockquote></li>
            //     </ol>

            // Arrange
            var commonMark = Helpers.Normalize("1.  foo\n\n    ```\n    bar\n    ```\n\n    baz\n\n    > bam");
            var expected = Helpers.Normalize("<ol>\n<li><p>foo</p>\n<pre><code>bar\n</code></pre>\n<p>baz</p>\n<blockquote>\n<p>bam</p>\n</blockquote></li>\n</ol>");
            Helpers.Log("Example {0}", 161);
            Helpers.Log("Section: {0}", "Container blocks - List items");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "1.  foo\n\n    ```\n    bar\n    ```\n\n    baz\n\n    > bam");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // 2.  **Item starting with indented code.**  If a sequence of lines *Ls*
        // constitute a sequence of blocks *Bs* starting with an indented code
        // block and not separated from each other by more than one blank line,
        // and *M* is a list marker *M* of width *W* followed by
        // one space, then the result of prepending *M* and the following
        // space to the first line of *Ls*, and indenting subsequent lines of
        // *Ls* by *W + 1* spaces, is a list item with *Bs* as its contents.
        // If a line is empty, then it need not be indented.  The type of the
        // list item (bullet or ordered) is determined by the type of its list
        // marker.  If the list item is ordered, then it is also assigned a
        // start number, based on the ordered list marker.
        //
        // An indented code block will have to be indented four spaces beyond
        // the edge of the region where text will be included in the list item.
        // In the following case that is 6 spaces:
        [TestMethod]
        [TestCategory("Container blocks - List items")]
        //[Timeout(1000)]
        public void Example162()
        {
            // Example 162
            // Section: Container blocks - List items
            //
            // The following CommonMark:
            //     - foo
            //     
            //           bar
            //
            // Should be rendered as:
            //     <ul>
            //     <li><p>foo</p>
            //     <pre><code>bar
            //     </code></pre></li>
            //     </ul>

            // Arrange
            var commonMark = Helpers.Normalize("- foo\n\n      bar");
            var expected = Helpers.Normalize("<ul>\n<li><p>foo</p>\n<pre><code>bar\n</code></pre></li>\n</ul>");
            Helpers.Log("Example {0}", 162);
            Helpers.Log("Section: {0}", "Container blocks - List items");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "- foo\n\n      bar");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // And in this case it is 11 spaces:
        [TestMethod]
        [TestCategory("Container blocks - List items")]
        //[Timeout(1000)]
        public void Example163()
        {
            // Example 163
            // Section: Container blocks - List items
            //
            // The following CommonMark:
            //       10.  foo
            //     
            //                bar
            //
            // Should be rendered as:
            //     <ol start="10">
            //     <li><p>foo</p>
            //     <pre><code>bar
            //     </code></pre></li>
            //     </ol>

            // Arrange
            var commonMark = Helpers.Normalize("  10.  foo\n\n           bar");
            var expected = Helpers.Normalize("<ol start=\"10\">\n<li><p>foo</p>\n<pre><code>bar\n</code></pre></li>\n</ol>");
            Helpers.Log("Example {0}", 163);
            Helpers.Log("Section: {0}", "Container blocks - List items");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "  10.  foo\n\n           bar");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // If the *first* block in the list item is an indented code block,
        // then by rule #2, the contents must be indented *one* space after the
        // list marker:
        [TestMethod]
        [TestCategory("Container blocks - List items")]
        //[Timeout(1000)]
        public void Example164()
        {
            // Example 164
            // Section: Container blocks - List items
            //
            // The following CommonMark:
            //         indented code
            //     
            //     paragraph
            //     
            //         more code
            //
            // Should be rendered as:
            //     <pre><code>indented code
            //     </code></pre>
            //     <p>paragraph</p>
            //     <pre><code>more code
            //     </code></pre>

            // Arrange
            var commonMark = Helpers.Normalize("    indented code\n\nparagraph\n\n    more code");
            var expected = Helpers.Normalize("<pre><code>indented code\n</code></pre>\n<p>paragraph</p>\n<pre><code>more code\n</code></pre>");
            Helpers.Log("Example {0}", 164);
            Helpers.Log("Section: {0}", "Container blocks - List items");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "    indented code\n\nparagraph\n\n    more code");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        [TestMethod]
        [TestCategory("Container blocks - List items")]
        //[Timeout(1000)]
        public void Example165()
        {
            // Example 165
            // Section: Container blocks - List items
            //
            // The following CommonMark:
            //     1.     indented code
            //     
            //        paragraph
            //     
            //            more code
            //
            // Should be rendered as:
            //     <ol>
            //     <li><pre><code>indented code
            //     </code></pre>
            //     <p>paragraph</p>
            //     <pre><code>more code
            //     </code></pre></li>
            //     </ol>

            // Arrange
            var commonMark = Helpers.Normalize("1.     indented code\n\n   paragraph\n\n       more code");
            var expected = Helpers.Normalize("<ol>\n<li><pre><code>indented code\n</code></pre>\n<p>paragraph</p>\n<pre><code>more code\n</code></pre></li>\n</ol>");
            Helpers.Log("Example {0}", 165);
            Helpers.Log("Section: {0}", "Container blocks - List items");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "1.     indented code\n\n   paragraph\n\n       more code");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // Note that an additional space indent is interpreted as space
        // inside the code block:
        [TestMethod]
        [TestCategory("Container blocks - List items")]
        //[Timeout(1000)]
        public void Example166()
        {
            // Example 166
            // Section: Container blocks - List items
            //
            // The following CommonMark:
            //     1.      indented code
            //     
            //        paragraph
            //     
            //            more code
            //
            // Should be rendered as:
            //     <ol>
            //     <li><pre><code> indented code
            //     </code></pre>
            //     <p>paragraph</p>
            //     <pre><code>more code
            //     </code></pre></li>
            //     </ol>

            // Arrange
            var commonMark = Helpers.Normalize("1.      indented code\n\n   paragraph\n\n       more code");
            var expected = Helpers.Normalize("<ol>\n<li><pre><code> indented code\n</code></pre>\n<p>paragraph</p>\n<pre><code>more code\n</code></pre></li>\n</ol>");
            Helpers.Log("Example {0}", 166);
            Helpers.Log("Section: {0}", "Container blocks - List items");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "1.      indented code\n\n   paragraph\n\n       more code");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // Note that rules #1 and #2 only apply to two cases:  (a) cases
        // in which the lines to be included in a list item begin with a nonspace
        // character, and (b) cases in which they begin with an indented code
        // block.  In a case like the following, where the first block begins with
        // a three-space indent, the rules do not allow us to form a list item by
        // indenting the whole thing and prepending a list marker:
        [TestMethod]
        [TestCategory("Container blocks - List items")]
        //[Timeout(1000)]
        public void Example167()
        {
            // Example 167
            // Section: Container blocks - List items
            //
            // The following CommonMark:
            //        foo
            //     
            //     bar
            //
            // Should be rendered as:
            //     <p>foo</p>
            //     <p>bar</p>

            // Arrange
            var commonMark = Helpers.Normalize("   foo\n\nbar");
            var expected = Helpers.Normalize("<p>foo</p>\n<p>bar</p>");
            Helpers.Log("Example {0}", 167);
            Helpers.Log("Section: {0}", "Container blocks - List items");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "   foo\n\nbar");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        [TestMethod]
        [TestCategory("Container blocks - List items")]
        //[Timeout(1000)]
        public void Example168()
        {
            // Example 168
            // Section: Container blocks - List items
            //
            // The following CommonMark:
            //     -    foo
            //     
            //       bar
            //
            // Should be rendered as:
            //     <ul>
            //     <li>foo</li>
            //     </ul>
            //     <p>bar</p>

            // Arrange
            var commonMark = Helpers.Normalize("-    foo\n\n  bar");
            var expected = Helpers.Normalize("<ul>\n<li>foo</li>\n</ul>\n<p>bar</p>");
            Helpers.Log("Example {0}", 168);
            Helpers.Log("Section: {0}", "Container blocks - List items");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "-    foo\n\n  bar");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // This is not a significant restriction, because when a block begins
        // with 1-3 spaces indent, the indentation can always be removed without
        // a change in interpretation, allowing rule #1 to be applied.  So, in
        // the above case:
        [TestMethod]
        [TestCategory("Container blocks - List items")]
        //[Timeout(1000)]
        public void Example169()
        {
            // Example 169
            // Section: Container blocks - List items
            //
            // The following CommonMark:
            //     -  foo
            //     
            //        bar
            //
            // Should be rendered as:
            //     <ul>
            //     <li><p>foo</p>
            //     <p>bar</p></li>
            //     </ul>

            // Arrange
            var commonMark = Helpers.Normalize("-  foo\n\n   bar");
            var expected = Helpers.Normalize("<ul>\n<li><p>foo</p>\n<p>bar</p></li>\n</ul>");
            Helpers.Log("Example {0}", 169);
            Helpers.Log("Section: {0}", "Container blocks - List items");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "-  foo\n\n   bar");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // 3.  **Indentation.**  If a sequence of lines *Ls* constitutes a list item
        // according to rule #1 or #2, then the result of indenting each line
        // of *L* by 1-3 spaces (the same for each line) also constitutes a
        // list item with the same contents and attributes.  If a line is
        // empty, then it need not be indented.
        //
        // Indented one space:
        [TestMethod]
        [TestCategory("Container blocks - List items")]
        //[Timeout(1000)]
        public void Example170()
        {
            // Example 170
            // Section: Container blocks - List items
            //
            // The following CommonMark:
            //      1.  A paragraph
            //          with two lines.
            //     
            //              indented code
            //     
            //          > A block quote.
            //
            // Should be rendered as:
            //     <ol>
            //     <li><p>A paragraph
            //     with two lines.</p>
            //     <pre><code>indented code
            //     </code></pre>
            //     <blockquote>
            //     <p>A block quote.</p>
            //     </blockquote></li>
            //     </ol>

            // Arrange
            var commonMark = Helpers.Normalize(" 1.  A paragraph\n     with two lines.\n\n         indented code\n\n     > A block quote.");
            var expected = Helpers.Normalize("<ol>\n<li><p>A paragraph\nwith two lines.</p>\n<pre><code>indented code\n</code></pre>\n<blockquote>\n<p>A block quote.</p>\n</blockquote></li>\n</ol>");
            Helpers.Log("Example {0}", 170);
            Helpers.Log("Section: {0}", "Container blocks - List items");
            Helpers.Log();
            Helpers.LogValue("CommonMark", " 1.  A paragraph\n     with two lines.\n\n         indented code\n\n     > A block quote.");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // Indented two spaces:
        [TestMethod]
        [TestCategory("Container blocks - List items")]
        //[Timeout(1000)]
        public void Example171()
        {
            // Example 171
            // Section: Container blocks - List items
            //
            // The following CommonMark:
            //       1.  A paragraph
            //           with two lines.
            //     
            //               indented code
            //     
            //           > A block quote.
            //
            // Should be rendered as:
            //     <ol>
            //     <li><p>A paragraph
            //     with two lines.</p>
            //     <pre><code>indented code
            //     </code></pre>
            //     <blockquote>
            //     <p>A block quote.</p>
            //     </blockquote></li>
            //     </ol>

            // Arrange
            var commonMark = Helpers.Normalize("  1.  A paragraph\n      with two lines.\n\n          indented code\n\n      > A block quote.");
            var expected = Helpers.Normalize("<ol>\n<li><p>A paragraph\nwith two lines.</p>\n<pre><code>indented code\n</code></pre>\n<blockquote>\n<p>A block quote.</p>\n</blockquote></li>\n</ol>");
            Helpers.Log("Example {0}", 171);
            Helpers.Log("Section: {0}", "Container blocks - List items");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "  1.  A paragraph\n      with two lines.\n\n          indented code\n\n      > A block quote.");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // Indented three spaces:
        [TestMethod]
        [TestCategory("Container blocks - List items")]
        //[Timeout(1000)]
        public void Example172()
        {
            // Example 172
            // Section: Container blocks - List items
            //
            // The following CommonMark:
            //        1.  A paragraph
            //            with two lines.
            //     
            //                indented code
            //     
            //            > A block quote.
            //
            // Should be rendered as:
            //     <ol>
            //     <li><p>A paragraph
            //     with two lines.</p>
            //     <pre><code>indented code
            //     </code></pre>
            //     <blockquote>
            //     <p>A block quote.</p>
            //     </blockquote></li>
            //     </ol>

            // Arrange
            var commonMark = Helpers.Normalize("   1.  A paragraph\n       with two lines.\n\n           indented code\n\n       > A block quote.");
            var expected = Helpers.Normalize("<ol>\n<li><p>A paragraph\nwith two lines.</p>\n<pre><code>indented code\n</code></pre>\n<blockquote>\n<p>A block quote.</p>\n</blockquote></li>\n</ol>");
            Helpers.Log("Example {0}", 172);
            Helpers.Log("Section: {0}", "Container blocks - List items");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "   1.  A paragraph\n       with two lines.\n\n           indented code\n\n       > A block quote.");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // Four spaces indent gives a code block:
        [TestMethod]
        [TestCategory("Container blocks - List items")]
        //[Timeout(1000)]
        public void Example173()
        {
            // Example 173
            // Section: Container blocks - List items
            //
            // The following CommonMark:
            //         1.  A paragraph
            //             with two lines.
            //     
            //                 indented code
            //     
            //             > A block quote.
            //
            // Should be rendered as:
            //     <pre><code>1.  A paragraph
            //         with two lines.
            //     
            //             indented code
            //     
            //         &gt; A block quote.
            //     </code></pre>

            // Arrange
            var commonMark = Helpers.Normalize("    1.  A paragraph\n        with two lines.\n\n            indented code\n\n        > A block quote.");
            var expected = Helpers.Normalize("<pre><code>1.  A paragraph\n    with two lines.\n\n        indented code\n\n    &gt; A block quote.\n</code></pre>");
            Helpers.Log("Example {0}", 173);
            Helpers.Log("Section: {0}", "Container blocks - List items");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "    1.  A paragraph\n        with two lines.\n\n            indented code\n\n        > A block quote.");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // 4.  **Laziness.**  If a string of lines *Ls* constitute a [list
        // item](#list-item) with contents *Bs*, then the result of deleting
        // some or all of the indentation from one or more lines in which the
        // next non-space character after the indentation is
        // [paragraph continuation text](#paragraph-continuation-text) is a
        // list item with the same contents and attributes.
        //
        // Here is an example with lazy continuation lines:
        [TestMethod]
        [TestCategory("Container blocks - List items")]
        //[Timeout(1000)]
        public void Example174()
        {
            // Example 174
            // Section: Container blocks - List items
            //
            // The following CommonMark:
            //       1.  A paragraph
            //     with two lines.
            //     
            //               indented code
            //     
            //           > A block quote.
            //
            // Should be rendered as:
            //     <ol>
            //     <li><p>A paragraph
            //     with two lines.</p>
            //     <pre><code>indented code
            //     </code></pre>
            //     <blockquote>
            //     <p>A block quote.</p>
            //     </blockquote></li>
            //     </ol>

            // Arrange
            var commonMark = Helpers.Normalize("  1.  A paragraph\nwith two lines.\n\n          indented code\n\n      > A block quote.");
            var expected = Helpers.Normalize("<ol>\n<li><p>A paragraph\nwith two lines.</p>\n<pre><code>indented code\n</code></pre>\n<blockquote>\n<p>A block quote.</p>\n</blockquote></li>\n</ol>");
            Helpers.Log("Example {0}", 174);
            Helpers.Log("Section: {0}", "Container blocks - List items");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "  1.  A paragraph\nwith two lines.\n\n          indented code\n\n      > A block quote.");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // Indentation can be partially deleted:
        [TestMethod]
        [TestCategory("Container blocks - List items")]
        //[Timeout(1000)]
        public void Example175()
        {
            // Example 175
            // Section: Container blocks - List items
            //
            // The following CommonMark:
            //       1.  A paragraph
            //         with two lines.
            //
            // Should be rendered as:
            //     <ol>
            //     <li>A paragraph
            //     with two lines.</li>
            //     </ol>

            // Arrange
            var commonMark = Helpers.Normalize("  1.  A paragraph\n    with two lines.");
            var expected = Helpers.Normalize("<ol>\n<li>A paragraph\nwith two lines.</li>\n</ol>");
            Helpers.Log("Example {0}", 175);
            Helpers.Log("Section: {0}", "Container blocks - List items");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "  1.  A paragraph\n    with two lines.");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // These examples show how laziness can work in nested structures:
        [TestMethod]
        [TestCategory("Container blocks - List items")]
        //[Timeout(1000)]
        public void Example176()
        {
            // Example 176
            // Section: Container blocks - List items
            //
            // The following CommonMark:
            //     > 1. > Blockquote
            //     continued here.
            //
            // Should be rendered as:
            //     <blockquote>
            //     <ol>
            //     <li><blockquote>
            //     <p>Blockquote
            //     continued here.</p>
            //     </blockquote></li>
            //     </ol>
            //     </blockquote>

            // Arrange
            var commonMark = Helpers.Normalize("> 1. > Blockquote\ncontinued here.");
            var expected = Helpers.Normalize("<blockquote>\n<ol>\n<li><blockquote>\n<p>Blockquote\ncontinued here.</p>\n</blockquote></li>\n</ol>\n</blockquote>");
            Helpers.Log("Example {0}", 176);
            Helpers.Log("Section: {0}", "Container blocks - List items");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "> 1. > Blockquote\ncontinued here.");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        [TestMethod]
        [TestCategory("Container blocks - List items")]
        //[Timeout(1000)]
        public void Example177()
        {
            // Example 177
            // Section: Container blocks - List items
            //
            // The following CommonMark:
            //     > 1. > Blockquote
            //     > continued here.
            //
            // Should be rendered as:
            //     <blockquote>
            //     <ol>
            //     <li><blockquote>
            //     <p>Blockquote
            //     continued here.</p>
            //     </blockquote></li>
            //     </ol>
            //     </blockquote>

            // Arrange
            var commonMark = Helpers.Normalize("> 1. > Blockquote\n> continued here.");
            var expected = Helpers.Normalize("<blockquote>\n<ol>\n<li><blockquote>\n<p>Blockquote\ncontinued here.</p>\n</blockquote></li>\n</ol>\n</blockquote>");
            Helpers.Log("Example {0}", 177);
            Helpers.Log("Section: {0}", "Container blocks - List items");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "> 1. > Blockquote\n> continued here.");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // 5.  **That's all.** Nothing that is not counted as a list item by rules
        // #1--4 counts as a [list item](#list-item).
        //
        // The rules for sublists follow from the general rules above.  A sublist
        // must be indented the same number of spaces a paragraph would need to be
        // in order to be included in the list item.
        //
        // So, in this case we need two spaces indent:
        [TestMethod]
        [TestCategory("Container blocks - List items")]
        //[Timeout(1000)]
        public void Example178()
        {
            // Example 178
            // Section: Container blocks - List items
            //
            // The following CommonMark:
            //     - foo
            //       - bar
            //         - baz
            //
            // Should be rendered as:
            //     <ul>
            //     <li>foo
            //     <ul>
            //     <li>bar
            //     <ul>
            //     <li>baz</li>
            //     </ul></li>
            //     </ul></li>
            //     </ul>

            // Arrange
            var commonMark = Helpers.Normalize("- foo\n  - bar\n    - baz");
            var expected = Helpers.Normalize("<ul>\n<li>foo\n<ul>\n<li>bar\n<ul>\n<li>baz</li>\n</ul></li>\n</ul></li>\n</ul>");
            Helpers.Log("Example {0}", 178);
            Helpers.Log("Section: {0}", "Container blocks - List items");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "- foo\n  - bar\n    - baz");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // One is not enough:
        [TestMethod]
        [TestCategory("Container blocks - List items")]
        //[Timeout(1000)]
        public void Example179()
        {
            // Example 179
            // Section: Container blocks - List items
            //
            // The following CommonMark:
            //     - foo
            //      - bar
            //       - baz
            //
            // Should be rendered as:
            //     <ul>
            //     <li>foo</li>
            //     <li>bar</li>
            //     <li>baz</li>
            //     </ul>

            // Arrange
            var commonMark = Helpers.Normalize("- foo\n - bar\n  - baz");
            var expected = Helpers.Normalize("<ul>\n<li>foo</li>\n<li>bar</li>\n<li>baz</li>\n</ul>");
            Helpers.Log("Example {0}", 179);
            Helpers.Log("Section: {0}", "Container blocks - List items");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "- foo\n - bar\n  - baz");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // Here we need four, because the list marker is wider:
        [TestMethod]
        [TestCategory("Container blocks - List items")]
        //[Timeout(1000)]
        public void Example180()
        {
            // Example 180
            // Section: Container blocks - List items
            //
            // The following CommonMark:
            //     10) foo
            //         - bar
            //
            // Should be rendered as:
            //     <ol start="10">
            //     <li>foo
            //     <ul>
            //     <li>bar</li>
            //     </ul></li>
            //     </ol>

            // Arrange
            var commonMark = Helpers.Normalize("10) foo\n    - bar");
            var expected = Helpers.Normalize("<ol start=\"10\">\n<li>foo\n<ul>\n<li>bar</li>\n</ul></li>\n</ol>");
            Helpers.Log("Example {0}", 180);
            Helpers.Log("Section: {0}", "Container blocks - List items");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "10) foo\n    - bar");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // Three is not enough:
        [TestMethod]
        [TestCategory("Container blocks - List items")]
        //[Timeout(1000)]
        public void Example181()
        {
            // Example 181
            // Section: Container blocks - List items
            //
            // The following CommonMark:
            //     10) foo
            //        - bar
            //
            // Should be rendered as:
            //     <ol start="10">
            //     <li>foo</li>
            //     </ol>
            //     <ul>
            //     <li>bar</li>
            //     </ul>

            // Arrange
            var commonMark = Helpers.Normalize("10) foo\n   - bar");
            var expected = Helpers.Normalize("<ol start=\"10\">\n<li>foo</li>\n</ol>\n<ul>\n<li>bar</li>\n</ul>");
            Helpers.Log("Example {0}", 181);
            Helpers.Log("Section: {0}", "Container blocks - List items");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "10) foo\n   - bar");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // A list may be the first block in a list item:
        [TestMethod]
        [TestCategory("Container blocks - List items")]
        //[Timeout(1000)]
        public void Example182()
        {
            // Example 182
            // Section: Container blocks - List items
            //
            // The following CommonMark:
            //     - - foo
            //
            // Should be rendered as:
            //     <ul>
            //     <li><ul>
            //     <li>foo</li>
            //     </ul></li>
            //     </ul>

            // Arrange
            var commonMark = Helpers.Normalize("- - foo");
            var expected = Helpers.Normalize("<ul>\n<li><ul>\n<li>foo</li>\n</ul></li>\n</ul>");
            Helpers.Log("Example {0}", 182);
            Helpers.Log("Section: {0}", "Container blocks - List items");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "- - foo");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        [TestMethod]
        [TestCategory("Container blocks - List items")]
        //[Timeout(1000)]
        public void Example183()
        {
            // Example 183
            // Section: Container blocks - List items
            //
            // The following CommonMark:
            //     1. - 2. foo
            //
            // Should be rendered as:
            //     <ol>
            //     <li><ul>
            //     <li><ol start="2">
            //     <li>foo</li>
            //     </ol></li>
            //     </ul></li>
            //     </ol>

            // Arrange
            var commonMark = Helpers.Normalize("1. - 2. foo");
            var expected = Helpers.Normalize("<ol>\n<li><ul>\n<li><ol start=\"2\">\n<li>foo</li>\n</ol></li>\n</ul></li>\n</ol>");
            Helpers.Log("Example {0}", 183);
            Helpers.Log("Section: {0}", "Container blocks - List items");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "1. - 2. foo");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // A list item may be empty:
        [TestMethod]
        [TestCategory("Container blocks - List items")]
        //[Timeout(1000)]
        public void Example184()
        {
            // Example 184
            // Section: Container blocks - List items
            //
            // The following CommonMark:
            //     - foo
            //     -
            //     - bar
            //
            // Should be rendered as:
            //     <ul>
            //     <li>foo</li>
            //     <li></li>
            //     <li>bar</li>
            //     </ul>

            // Arrange
            var commonMark = Helpers.Normalize("- foo\n-\n- bar");
            var expected = Helpers.Normalize("<ul>\n<li>foo</li>\n<li></li>\n<li>bar</li>\n</ul>");
            Helpers.Log("Example {0}", 184);
            Helpers.Log("Section: {0}", "Container blocks - List items");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "- foo\n-\n- bar");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        [TestMethod]
        [TestCategory("Container blocks - List items")]
        //[Timeout(1000)]
        public void Example185()
        {
            // Example 185
            // Section: Container blocks - List items
            //
            // The following CommonMark:
            //     -
            //
            // Should be rendered as:
            //     <ul>
            //     <li></li>
            //     </ul>

            // Arrange
            var commonMark = Helpers.Normalize("-");
            var expected = Helpers.Normalize("<ul>\n<li></li>\n</ul>");
            Helpers.Log("Example {0}", 185);
            Helpers.Log("Section: {0}", "Container blocks - List items");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "-");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // ### Motivation
        //
        // John Gruber's Markdown spec says the following about list items:
        //
        // 1. "List markers typically start at the left margin, but may be indented
        // by up to three spaces. List markers must be followed by one or more
        // spaces or a tab."
        //
        // 2. "To make lists look nice, you can wrap items with hanging indents....
        // But if you don't want to, you don't have to."
        //
        // 3. "List items may consist of multiple paragraphs. Each subsequent
        // paragraph in a list item must be indented by either 4 spaces or one
        // tab."
        //
        // 4. "It looks nice if you indent every line of the subsequent paragraphs,
        // but here again, Markdown will allow you to be lazy."
        //
        // 5. "To put a blockquote within a list item, the blockquote's `>`
        // delimiters need to be indented."
        //
        // 6. "To put a code block within a list item, the code block needs to be
        // indented twice — 8 spaces or two tabs."
        //
        // These rules specify that a paragraph under a list item must be indented
        // four spaces (presumably, from the left margin, rather than the start of
        // the list marker, but this is not said), and that code under a list item
        // must be indented eight spaces instead of the usual four.  They also say
        // that a block quote must be indented, but not by how much; however, the
        // example given has four spaces indentation.  Although nothing is said
        // about other kinds of block-level content, it is certainly reasonable to
        // infer that *all* block elements under a list item, including other
        // lists, must be indented four spaces.  This principle has been called the
        // *four-space rule*.
        //
        // The four-space rule is clear and principled, and if the reference
        // implementation `Markdown.pl` had followed it, it probably would have
        // become the standard.  However, `Markdown.pl` allowed paragraphs and
        // sublists to start with only two spaces indentation, at least on the
        // outer level.  Worse, its behavior was inconsistent: a sublist of an
        // outer-level list needed two spaces indentation, but a sublist of this
        // sublist needed three spaces.  It is not surprising, then, that different
        // implementations of Markdown have developed very different rules for
        // determining what comes under a list item.  (Pandoc and python-Markdown,
        // for example, stuck with Gruber's syntax description and the four-space
        // rule, while discount, redcarpet, marked, PHP Markdown, and others
        // followed `Markdown.pl`'s behavior more closely.)
        //
        // Unfortunately, given the divergences between implementations, there
        // is no way to give a spec for list items that will be guaranteed not
        // to break any existing documents.  However, the spec given here should
        // correctly handle lists formatted with either the four-space rule or
        // the more forgiving `Markdown.pl` behavior, provided they are laid out
        // in a way that is natural for a human to read.
        //
        // The strategy here is to let the width and indentation of the list marker
        // determine the indentation necessary for blocks to fall under the list
        // item, rather than having a fixed and arbitrary number.  The writer can
        // think of the body of the list item as a unit which gets indented to the
        // right enough to fit the list marker (and any indentation on the list
        // marker).  (The laziness rule, #4, then allows continuation lines to be
        // unindented if needed.)
        //
        // This rule is superior, we claim, to any rule requiring a fixed level of
        // indentation from the margin.  The four-space rule is clear but
        // unnatural. It is quite unintuitive that
        //
        // ``` markdown
        // - foo
        //
        // bar
        //
        // - baz
        // ```
        //
        // should be parsed as two lists with an intervening paragraph,
        //
        // ``` html
        // <ul>
        // <li>foo</li>
        // </ul>
        // <p>bar</p>
        // <ul>
        // <li>baz</li>
        // </ul>
        // ```
        //
        // as the four-space rule demands, rather than a single list,
        //
        // ``` html
        // <ul>
        // <li><p>foo</p>
        // <p>bar</p>
        // <ul>
        // <li>baz</li>
        // </ul></li>
        // </ul>
        // ```
        //
        // The choice of four spaces is arbitrary.  It can be learned, but it is
        // not likely to be guessed, and it trips up beginners regularly.
        //
        // Would it help to adopt a two-space rule?  The problem is that such
        // a rule, together with the rule allowing 1--3 spaces indentation of the
        // initial list marker, allows text that is indented *less than* the
        // original list marker to be included in the list item. For example,
        // `Markdown.pl` parses
        //
        // ``` markdown
        // - one
        //
        // two
        // ```
        //
        // as a single list item, with `two` a continuation paragraph:
        //
        // ``` html
        // <ul>
        // <li><p>one</p>
        // <p>two</p></li>
        // </ul>
        // ```
        //
        // and similarly
        //
        // ``` markdown
        // >   - one
        // >
        // >  two
        // ```
        //
        // as
        //
        // ``` html
        // <blockquote>
        // <ul>
        // <li><p>one</p>
        // <p>two</p></li>
        // </ul>
        // </blockquote>
        // ```
        //
        // This is extremely unintuitive.
        //
        // Rather than requiring a fixed indent from the margin, we could require
        // a fixed indent (say, two spaces, or even one space) from the list marker (which
        // may itself be indented).  This proposal would remove the last anomaly
        // discussed.  Unlike the spec presented above, it would count the following
        // as a list item with a subparagraph, even though the paragraph `bar`
        // is not indented as far as the first paragraph `foo`:
        //
        // ``` markdown
        // 10. foo
        //
        // bar
        // ```
        //
        // Arguably this text does read like a list item with `bar` as a subparagraph,
        // which may count in favor of the proposal.  However, on this proposal indented
        // code would have to be indented six spaces after the list marker.  And this
        // would break a lot of existing Markdown, which has the pattern:
        //
        // ``` markdown
        // 1.  foo
        //
        // indented code
        // ```
        //
        // where the code is indented eight spaces.  The spec above, by contrast, will
        // parse this text as expected, since the code block's indentation is measured
        // from the beginning of `foo`.
        //
        // The one case that needs special treatment is a list item that *starts*
        // with indented code.  How much indentation is required in that case, since
        // we don't have a "first paragraph" to measure from?  Rule #2 simply stipulates
        // that in such cases, we require one space indentation from the list marker
        // (and then the normal four spaces for the indented code).  This will match the
        // four-space rule in cases where the list marker plus its initial indentation
        // takes four spaces (a common case), but diverge in other cases.
        //
        // ## Lists
        //
        // A [list](#list) <a id="list"></a> is a sequence of one or more
        // list items [of the same type](#of-the-same-type).  The list items
        // may be separated by single [blank lines](#blank-line), but two
        // blank lines end all containing lists.
        //
        // Two list items are [of the same type](#of-the-same-type)
        // <a id="of-the-same-type"></a> if they begin with a [list
        // marker](#list-marker) of the same type.  Two list markers are of the
        // same type if (a) they are bullet list markers using the same character
        // (`-`, `+`, or `*`) or (b) they are ordered list numbers with the same
        // delimiter (either `.` or `)`).
        //
        // A list is an [ordered list](#ordered-list) <a id="ordered-list"></a>
        // if its constituent list items begin with
        // [ordered list markers](#ordered-list-marker), and a [bullet
        // list](#bullet-list) <a id="bullet-list"></a> if its constituent list
        // items begin with [bullet list markers](#bullet-list-marker).
        //
        // The [start number](#start-number) <a id="start-number"></a>
        // of an [ordered list](#ordered-list) is determined by the list number of
        // its initial list item.  The numbers of subsequent list items are
        // disregarded.
        //
        // A list is [loose](#loose) if it any of its constituent list items are
        // separated by blank lines, or if any of its constituent list items
        // directly contain two block-level elements with a blank line between
        // them.  Otherwise a list is [tight](#tight).  (The difference in HTML output
        // is that paragraphs in a loose with are wrapped in `<p>` tags, while
        // paragraphs in a tight list are not.)
        //
        // Changing the bullet or ordered list delimiter starts a new list:
        [TestMethod]
        [TestCategory("Container blocks - Lists")]
        //[Timeout(1000)]
        public void Example186()
        {
            // Example 186
            // Section: Container blocks - Lists
            //
            // The following CommonMark:
            //     - foo
            //     - bar
            //     + baz
            //
            // Should be rendered as:
            //     <ul>
            //     <li>foo</li>
            //     <li>bar</li>
            //     </ul>
            //     <ul>
            //     <li>baz</li>
            //     </ul>

            // Arrange
            var commonMark = Helpers.Normalize("- foo\n- bar\n+ baz");
            var expected = Helpers.Normalize("<ul>\n<li>foo</li>\n<li>bar</li>\n</ul>\n<ul>\n<li>baz</li>\n</ul>");
            Helpers.Log("Example {0}", 186);
            Helpers.Log("Section: {0}", "Container blocks - Lists");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "- foo\n- bar\n+ baz");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        [TestMethod]
        [TestCategory("Container blocks - Lists")]
        //[Timeout(1000)]
        public void Example187()
        {
            // Example 187
            // Section: Container blocks - Lists
            //
            // The following CommonMark:
            //     1. foo
            //     2. bar
            //     3) baz
            //
            // Should be rendered as:
            //     <ol>
            //     <li>foo</li>
            //     <li>bar</li>
            //     </ol>
            //     <ol start="3">
            //     <li>baz</li>
            //     </ol>

            // Arrange
            var commonMark = Helpers.Normalize("1. foo\n2. bar\n3) baz");
            var expected = Helpers.Normalize("<ol>\n<li>foo</li>\n<li>bar</li>\n</ol>\n<ol start=\"3\">\n<li>baz</li>\n</ol>");
            Helpers.Log("Example {0}", 187);
            Helpers.Log("Section: {0}", "Container blocks - Lists");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "1. foo\n2. bar\n3) baz");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // There can be blank lines between items, but two blank lines end
        // a list:
        [TestMethod]
        [TestCategory("Container blocks - Lists")]
        //[Timeout(1000)]
        public void Example188()
        {
            // Example 188
            // Section: Container blocks - Lists
            //
            // The following CommonMark:
            //     - foo
            //     
            //     - bar
            //     
            //     
            //     - baz
            //
            // Should be rendered as:
            //     <ul>
            //     <li><p>foo</p></li>
            //     <li><p>bar</p></li>
            //     </ul>
            //     <ul>
            //     <li>baz</li>
            //     </ul>

            // Arrange
            var commonMark = Helpers.Normalize("- foo\n\n- bar\n\n\n- baz");
            var expected = Helpers.Normalize("<ul>\n<li><p>foo</p></li>\n<li><p>bar</p></li>\n</ul>\n<ul>\n<li>baz</li>\n</ul>");
            Helpers.Log("Example {0}", 188);
            Helpers.Log("Section: {0}", "Container blocks - Lists");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "- foo\n\n- bar\n\n\n- baz");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // As illustrated above in the section on [list items](#list-item),
        // two blank lines between blocks *within* a list item will also end a
        // list:
        [TestMethod]
        [TestCategory("Container blocks - Lists")]
        //[Timeout(1000)]
        public void Example189()
        {
            // Example 189
            // Section: Container blocks - Lists
            //
            // The following CommonMark:
            //     - foo
            //     
            //     
            //       bar
            //     - baz
            //
            // Should be rendered as:
            //     <ul>
            //     <li>foo</li>
            //     </ul>
            //     <p>bar</p>
            //     <ul>
            //     <li>baz</li>
            //     </ul>

            // Arrange
            var commonMark = Helpers.Normalize("- foo\n\n\n  bar\n- baz");
            var expected = Helpers.Normalize("<ul>\n<li>foo</li>\n</ul>\n<p>bar</p>\n<ul>\n<li>baz</li>\n</ul>");
            Helpers.Log("Example {0}", 189);
            Helpers.Log("Section: {0}", "Container blocks - Lists");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "- foo\n\n\n  bar\n- baz");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // Indeed, two blank lines will end *all* containing lists:
        [TestMethod]
        [TestCategory("Container blocks - Lists")]
        //[Timeout(1000)]
        public void Example190()
        {
            // Example 190
            // Section: Container blocks - Lists
            //
            // The following CommonMark:
            //     - foo
            //       - bar
            //         - baz
            //     
            //     
            //           bim
            //
            // Should be rendered as:
            //     <ul>
            //     <li>foo
            //     <ul>
            //     <li>bar
            //     <ul>
            //     <li>baz</li>
            //     </ul></li>
            //     </ul></li>
            //     </ul>
            //     <pre><code>  bim
            //     </code></pre>

            // Arrange
            var commonMark = Helpers.Normalize("- foo\n  - bar\n    - baz\n\n\n      bim");
            var expected = Helpers.Normalize("<ul>\n<li>foo\n<ul>\n<li>bar\n<ul>\n<li>baz</li>\n</ul></li>\n</ul></li>\n</ul>\n<pre><code>  bim\n</code></pre>");
            Helpers.Log("Example {0}", 190);
            Helpers.Log("Section: {0}", "Container blocks - Lists");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "- foo\n  - bar\n    - baz\n\n\n      bim");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // Thus, two blank lines can be used to separate consecutive lists of
        // the same type, or to separate a list from an indented code block
        // that would otherwise be parsed as a subparagraph of the final list
        // item:
        [TestMethod]
        [TestCategory("Container blocks - Lists")]
        //[Timeout(1000)]
        public void Example191()
        {
            // Example 191
            // Section: Container blocks - Lists
            //
            // The following CommonMark:
            //     - foo
            //     - bar
            //     
            //     
            //     - baz
            //     - bim
            //
            // Should be rendered as:
            //     <ul>
            //     <li>foo</li>
            //     <li>bar</li>
            //     </ul>
            //     <ul>
            //     <li>baz</li>
            //     <li>bim</li>
            //     </ul>

            // Arrange
            var commonMark = Helpers.Normalize("- foo\n- bar\n\n\n- baz\n- bim");
            var expected = Helpers.Normalize("<ul>\n<li>foo</li>\n<li>bar</li>\n</ul>\n<ul>\n<li>baz</li>\n<li>bim</li>\n</ul>");
            Helpers.Log("Example {0}", 191);
            Helpers.Log("Section: {0}", "Container blocks - Lists");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "- foo\n- bar\n\n\n- baz\n- bim");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        [TestMethod]
        [TestCategory("Container blocks - Lists")]
        //[Timeout(1000)]
        public void Example192()
        {
            // Example 192
            // Section: Container blocks - Lists
            //
            // The following CommonMark:
            //     -   foo
            //     
            //         notcode
            //     
            //     -   foo
            //     
            //     
            //         code
            //
            // Should be rendered as:
            //     <ul>
            //     <li><p>foo</p>
            //     <p>notcode</p></li>
            //     <li><p>foo</p></li>
            //     </ul>
            //     <pre><code>code
            //     </code></pre>

            // Arrange
            var commonMark = Helpers.Normalize("-   foo\n\n    notcode\n\n-   foo\n\n\n    code");
            var expected = Helpers.Normalize("<ul>\n<li><p>foo</p>\n<p>notcode</p></li>\n<li><p>foo</p></li>\n</ul>\n<pre><code>code\n</code></pre>");
            Helpers.Log("Example {0}", 192);
            Helpers.Log("Section: {0}", "Container blocks - Lists");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "-   foo\n\n    notcode\n\n-   foo\n\n\n    code");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // List items need not be indented to the same level.  The following
        // list items will be treated as items at the same list level,
        // since none is indented enough to belong to the previous list
        // item:
        [TestMethod]
        [TestCategory("Container blocks - Lists")]
        //[Timeout(1000)]
        public void Example193()
        {
            // Example 193
            // Section: Container blocks - Lists
            //
            // The following CommonMark:
            //     - a
            //      - b
            //       - c
            //        - d
            //       - e
            //      - f
            //     - g
            //
            // Should be rendered as:
            //     <ul>
            //     <li>a</li>
            //     <li>b</li>
            //     <li>c</li>
            //     <li>d</li>
            //     <li>e</li>
            //     <li>f</li>
            //     <li>g</li>
            //     </ul>

            // Arrange
            var commonMark = Helpers.Normalize("- a\n - b\n  - c\n   - d\n  - e\n - f\n- g");
            var expected = Helpers.Normalize("<ul>\n<li>a</li>\n<li>b</li>\n<li>c</li>\n<li>d</li>\n<li>e</li>\n<li>f</li>\n<li>g</li>\n</ul>");
            Helpers.Log("Example {0}", 193);
            Helpers.Log("Section: {0}", "Container blocks - Lists");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "- a\n - b\n  - c\n   - d\n  - e\n - f\n- g");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // This is a loose list, because there is a blank line between
        // two of the list items:
        [TestMethod]
        [TestCategory("Container blocks - Lists")]
        //[Timeout(1000)]
        public void Example194()
        {
            // Example 194
            // Section: Container blocks - Lists
            //
            // The following CommonMark:
            //     - a
            //     - b
            //     
            //     - c
            //
            // Should be rendered as:
            //     <ul>
            //     <li><p>a</p></li>
            //     <li><p>b</p></li>
            //     <li><p>c</p></li>
            //     </ul>

            // Arrange
            var commonMark = Helpers.Normalize("- a\n- b\n\n- c");
            var expected = Helpers.Normalize("<ul>\n<li><p>a</p></li>\n<li><p>b</p></li>\n<li><p>c</p></li>\n</ul>");
            Helpers.Log("Example {0}", 194);
            Helpers.Log("Section: {0}", "Container blocks - Lists");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "- a\n- b\n\n- c");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // So is this, with a empty second item:
        [TestMethod]
        [TestCategory("Container blocks - Lists")]
        //[Timeout(1000)]
        public void Example195()
        {
            // Example 195
            // Section: Container blocks - Lists
            //
            // The following CommonMark:
            //     * a
            //     *
            //     
            //     * c
            //
            // Should be rendered as:
            //     <ul>
            //     <li><p>a</p></li>
            //     <li></li>
            //     <li><p>c</p></li>
            //     </ul>

            // Arrange
            var commonMark = Helpers.Normalize("* a\n*\n\n* c");
            var expected = Helpers.Normalize("<ul>\n<li><p>a</p></li>\n<li></li>\n<li><p>c</p></li>\n</ul>");
            Helpers.Log("Example {0}", 195);
            Helpers.Log("Section: {0}", "Container blocks - Lists");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "* a\n*\n\n* c");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // These are loose lists, even though there is no space between the items,
        // because one of the items directly contains two block-level elements
        // with a blank line between them:
        [TestMethod]
        [TestCategory("Container blocks - Lists")]
        //[Timeout(1000)]
        public void Example196()
        {
            // Example 196
            // Section: Container blocks - Lists
            //
            // The following CommonMark:
            //     - a
            //     - b
            //     
            //       c
            //     - d
            //
            // Should be rendered as:
            //     <ul>
            //     <li><p>a</p></li>
            //     <li><p>b</p>
            //     <p>c</p></li>
            //     <li><p>d</p></li>
            //     </ul>

            // Arrange
            var commonMark = Helpers.Normalize("- a\n- b\n\n  c\n- d");
            var expected = Helpers.Normalize("<ul>\n<li><p>a</p></li>\n<li><p>b</p>\n<p>c</p></li>\n<li><p>d</p></li>\n</ul>");
            Helpers.Log("Example {0}", 196);
            Helpers.Log("Section: {0}", "Container blocks - Lists");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "- a\n- b\n\n  c\n- d");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        [TestMethod]
        [TestCategory("Container blocks - Lists")]
        //[Timeout(1000)]
        public void Example197()
        {
            // Example 197
            // Section: Container blocks - Lists
            //
            // The following CommonMark:
            //     - a
            //     - b
            //     
            //       [ref]: /url
            //     - d
            //
            // Should be rendered as:
            //     <ul>
            //     <li><p>a</p></li>
            //     <li><p>b</p></li>
            //     <li><p>d</p></li>
            //     </ul>

            // Arrange
            var commonMark = Helpers.Normalize("- a\n- b\n\n  [ref]: /url\n- d");
            var expected = Helpers.Normalize("<ul>\n<li><p>a</p></li>\n<li><p>b</p></li>\n<li><p>d</p></li>\n</ul>");
            Helpers.Log("Example {0}", 197);
            Helpers.Log("Section: {0}", "Container blocks - Lists");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "- a\n- b\n\n  [ref]: /url\n- d");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // This is a tight list, because the blank lines are in a code block:
        [TestMethod]
        [TestCategory("Container blocks - Lists")]
        //[Timeout(1000)]
        public void Example198()
        {
            // Example 198
            // Section: Container blocks - Lists
            //
            // The following CommonMark:
            //     - a
            //     - ```
            //       b
            //     
            //     
            //       ```
            //     - c
            //
            // Should be rendered as:
            //     <ul>
            //     <li>a</li>
            //     <li><pre><code>b
            //     
            //     
            //     </code></pre></li>
            //     <li>c</li>
            //     </ul>

            // Arrange
            var commonMark = Helpers.Normalize("- a\n- ```\n  b\n\n\n  ```\n- c");
            var expected = Helpers.Normalize("<ul>\n<li>a</li>\n<li><pre><code>b\n\n\n</code></pre></li>\n<li>c</li>\n</ul>");
            Helpers.Log("Example {0}", 198);
            Helpers.Log("Section: {0}", "Container blocks - Lists");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "- a\n- ```\n  b\n\n\n  ```\n- c");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // This is a tight list, because the blank line is between two
        // paragraphs of a sublist.  So the inner list is loose while
        // the other list is tight:
        [TestMethod]
        [TestCategory("Container blocks - Lists")]
        //[Timeout(1000)]
        public void Example199()
        {
            // Example 199
            // Section: Container blocks - Lists
            //
            // The following CommonMark:
            //     - a
            //       - b
            //     
            //         c
            //     - d
            //
            // Should be rendered as:
            //     <ul>
            //     <li>a
            //     <ul>
            //     <li><p>b</p>
            //     <p>c</p></li>
            //     </ul></li>
            //     <li>d</li>
            //     </ul>

            // Arrange
            var commonMark = Helpers.Normalize("- a\n  - b\n\n    c\n- d");
            var expected = Helpers.Normalize("<ul>\n<li>a\n<ul>\n<li><p>b</p>\n<p>c</p></li>\n</ul></li>\n<li>d</li>\n</ul>");
            Helpers.Log("Example {0}", 199);
            Helpers.Log("Section: {0}", "Container blocks - Lists");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "- a\n  - b\n\n    c\n- d");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // This is a tight list, because the blank line is inside the
        // block quote:
        [TestMethod]
        [TestCategory("Container blocks - Lists")]
        //[Timeout(1000)]
        public void Example200()
        {
            // Example 200
            // Section: Container blocks - Lists
            //
            // The following CommonMark:
            //     * a
            //       > b
            //       >
            //     * c
            //
            // Should be rendered as:
            //     <ul>
            //     <li>a
            //     <blockquote>
            //     <p>b</p>
            //     </blockquote></li>
            //     <li>c</li>
            //     </ul>

            // Arrange
            var commonMark = Helpers.Normalize("* a\n  > b\n  >\n* c");
            var expected = Helpers.Normalize("<ul>\n<li>a\n<blockquote>\n<p>b</p>\n</blockquote></li>\n<li>c</li>\n</ul>");
            Helpers.Log("Example {0}", 200);
            Helpers.Log("Section: {0}", "Container blocks - Lists");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "* a\n  > b\n  >\n* c");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // This list is tight, because the consecutive block elements
        // are not separated by blank lines:
        [TestMethod]
        [TestCategory("Container blocks - Lists")]
        //[Timeout(1000)]
        public void Example201()
        {
            // Example 201
            // Section: Container blocks - Lists
            //
            // The following CommonMark:
            //     - a
            //       > b
            //       ```
            //       c
            //       ```
            //     - d
            //
            // Should be rendered as:
            //     <ul>
            //     <li>a
            //     <blockquote>
            //     <p>b</p>
            //     </blockquote>
            //     <pre><code>c
            //     </code></pre></li>
            //     <li>d</li>
            //     </ul>

            // Arrange
            var commonMark = Helpers.Normalize("- a\n  > b\n  ```\n  c\n  ```\n- d");
            var expected = Helpers.Normalize("<ul>\n<li>a\n<blockquote>\n<p>b</p>\n</blockquote>\n<pre><code>c\n</code></pre></li>\n<li>d</li>\n</ul>");
            Helpers.Log("Example {0}", 201);
            Helpers.Log("Section: {0}", "Container blocks - Lists");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "- a\n  > b\n  ```\n  c\n  ```\n- d");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // A single-paragraph list is tight:
        [TestMethod]
        [TestCategory("Container blocks - Lists")]
        //[Timeout(1000)]
        public void Example202()
        {
            // Example 202
            // Section: Container blocks - Lists
            //
            // The following CommonMark:
            //     - a
            //
            // Should be rendered as:
            //     <ul>
            //     <li>a</li>
            //     </ul>

            // Arrange
            var commonMark = Helpers.Normalize("- a");
            var expected = Helpers.Normalize("<ul>\n<li>a</li>\n</ul>");
            Helpers.Log("Example {0}", 202);
            Helpers.Log("Section: {0}", "Container blocks - Lists");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "- a");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        [TestMethod]
        [TestCategory("Container blocks - Lists")]
        //[Timeout(1000)]
        public void Example203()
        {
            // Example 203
            // Section: Container blocks - Lists
            //
            // The following CommonMark:
            //     - a
            //       - b
            //
            // Should be rendered as:
            //     <ul>
            //     <li>a
            //     <ul>
            //     <li>b</li>
            //     </ul></li>
            //     </ul>

            // Arrange
            var commonMark = Helpers.Normalize("- a\n  - b");
            var expected = Helpers.Normalize("<ul>\n<li>a\n<ul>\n<li>b</li>\n</ul></li>\n</ul>");
            Helpers.Log("Example {0}", 203);
            Helpers.Log("Section: {0}", "Container blocks - Lists");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "- a\n  - b");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // Here the outer list is loose, the inner list tight:
        [TestMethod]
        [TestCategory("Container blocks - Lists")]
        //[Timeout(1000)]
        public void Example204()
        {
            // Example 204
            // Section: Container blocks - Lists
            //
            // The following CommonMark:
            //     * foo
            //       * bar
            //     
            //       baz
            //
            // Should be rendered as:
            //     <ul>
            //     <li><p>foo</p>
            //     <ul>
            //     <li>bar</li>
            //     </ul>
            //     <p>baz</p></li>
            //     </ul>

            // Arrange
            var commonMark = Helpers.Normalize("* foo\n  * bar\n\n  baz");
            var expected = Helpers.Normalize("<ul>\n<li><p>foo</p>\n<ul>\n<li>bar</li>\n</ul>\n<p>baz</p></li>\n</ul>");
            Helpers.Log("Example {0}", 204);
            Helpers.Log("Section: {0}", "Container blocks - Lists");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "* foo\n  * bar\n\n  baz");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        [TestMethod]
        [TestCategory("Container blocks - Lists")]
        //[Timeout(1000)]
        public void Example205()
        {
            // Example 205
            // Section: Container blocks - Lists
            //
            // The following CommonMark:
            //     - a
            //       - b
            //       - c
            //     
            //     - d
            //       - e
            //       - f
            //
            // Should be rendered as:
            //     <ul>
            //     <li><p>a</p>
            //     <ul>
            //     <li>b</li>
            //     <li>c</li>
            //     </ul></li>
            //     <li><p>d</p>
            //     <ul>
            //     <li>e</li>
            //     <li>f</li>
            //     </ul></li>
            //     </ul>

            // Arrange
            var commonMark = Helpers.Normalize("- a\n  - b\n  - c\n\n- d\n  - e\n  - f");
            var expected = Helpers.Normalize("<ul>\n<li><p>a</p>\n<ul>\n<li>b</li>\n<li>c</li>\n</ul></li>\n<li><p>d</p>\n<ul>\n<li>e</li>\n<li>f</li>\n</ul></li>\n</ul>");
            Helpers.Log("Example {0}", 205);
            Helpers.Log("Section: {0}", "Container blocks - Lists");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "- a\n  - b\n  - c\n\n- d\n  - e\n  - f");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // # Inlines
        //
        // Inlines are parsed sequentially from the beginning of the character
        // stream to the end (left to right, in left-to-right languages).
        // Thus, for example, in
        [TestMethod]
        [TestCategory("Inlines")]
        //[Timeout(1000)]
        public void Example206()
        {
            // Example 206
            // Section: Inlines
            //
            // The following CommonMark:
            //     `hi`lo`
            //
            // Should be rendered as:
            //     <p><code>hi</code>lo`</p>

            // Arrange
            var commonMark = Helpers.Normalize("`hi`lo`");
            var expected = Helpers.Normalize("<p><code>hi</code>lo`</p>");
            Helpers.Log("Example {0}", 206);
            Helpers.Log("Section: {0}", "Inlines");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "`hi`lo`");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // `hi` is parsed as code, leaving the backtick at the end as a literal
        // backtick.
        //
        // ## Backslash escapes
        //
        // Any ASCII punctuation character may be backslash-escaped:
        [TestMethod]
        [TestCategory("Inlines - Backslash escapes")]
        //[Timeout(1000)]
        public void Example207()
        {
            // Example 207
            // Section: Inlines - Backslash escapes
            //
            // The following CommonMark:
            //     \!\"\#\$\%\&\'\(\)\*\+\,\-\.\/\:\;\<\=\>\?\@\[\\\]\^\_\`\{\|\}\~
            //
            // Should be rendered as:
            //     <p>!&quot;#$%&amp;'()*+,-./:;&lt;=&gt;?@[\]^_`{|}~</p>

            // Arrange
            var commonMark = Helpers.Normalize("\\!\\\"\\#\\$\\%\\&\\'\\(\\)\\*\\+\\,\\-\\.\\/\\:\\;\\<\\=\\>\\?\\@\\[\\\\\\]\\^\\_\\`\\{\\|\\}\\~");
            var expected = Helpers.Normalize("<p>!&quot;#$%&amp;'()*+,-./:;&lt;=&gt;?@[\\]^_`{|}~</p>");
            Helpers.Log("Example {0}", 207);
            Helpers.Log("Section: {0}", "Inlines - Backslash escapes");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "\\!\\\"\\#\\$\\%\\&\\'\\(\\)\\*\\+\\,\\-\\.\\/\\:\\;\\<\\=\\>\\?\\@\\[\\\\\\]\\^\\_\\`\\{\\|\\}\\~");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // Backslashes before other characters are treated as literal
        // backslashes:
        [TestMethod]
        [TestCategory("Inlines - Backslash escapes")]
        //[Timeout(1000)]
        public void Example208()
        {
            // Example 208
            // Section: Inlines - Backslash escapes
            //
            // The following CommonMark:
            //     \→\A\a\ \3\φ\«
            //
            // Should be rendered as:
            //     <p>\   \A\a\ \3\φ\«</p>

            // Arrange
            var commonMark = Helpers.Normalize("\\→\\A\\a\\ \\3\\φ\\«");
            var expected = Helpers.Normalize("<p>\\   \\A\\a\\ \\3\\φ\\«</p>");
            Helpers.Log("Example {0}", 208);
            Helpers.Log("Section: {0}", "Inlines - Backslash escapes");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "\\→\\A\\a\\ \\3\\φ\\«");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // Escaped characters are treated as regular characters and do
        // not have their usual Markdown meanings:
        [TestMethod]
        [TestCategory("Inlines - Backslash escapes")]
        //[Timeout(1000)]
        public void Example209()
        {
            // Example 209
            // Section: Inlines - Backslash escapes
            //
            // The following CommonMark:
            //     \*not emphasized*
            //     \<br/> not a tag
            //     \[not a link](/foo)
            //     \`not code`
            //     1\. not a list
            //     \* not a list
            //     \# not a header
            //     \[foo]: /url "not a reference"
            //
            // Should be rendered as:
            //     <p>*not emphasized*
            //     &lt;br/&gt; not a tag
            //     [not a link](/foo)
            //     `not code`
            //     1. not a list
            //     * not a list
            //     # not a header
            //     [foo]: /url &quot;not a reference&quot;</p>

            // Arrange
            var commonMark = Helpers.Normalize("\\*not emphasized*\n\\<br/> not a tag\n\\[not a link](/foo)\n\\`not code`\n1\\. not a list\n\\* not a list\n\\# not a header\n\\[foo]: /url \"not a reference\"");
            var expected = Helpers.Normalize("<p>*not emphasized*\n&lt;br/&gt; not a tag\n[not a link](/foo)\n`not code`\n1. not a list\n* not a list\n# not a header\n[foo]: /url &quot;not a reference&quot;</p>");
            Helpers.Log("Example {0}", 209);
            Helpers.Log("Section: {0}", "Inlines - Backslash escapes");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "\\*not emphasized*\n\\<br/> not a tag\n\\[not a link](/foo)\n\\`not code`\n1\\. not a list\n\\* not a list\n\\# not a header\n\\[foo]: /url \"not a reference\"");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // If a backslash is itself escaped, the following character is not:
        [TestMethod]
        [TestCategory("Inlines - Backslash escapes")]
        //[Timeout(1000)]
        public void Example210()
        {
            // Example 210
            // Section: Inlines - Backslash escapes
            //
            // The following CommonMark:
            //     \\*emphasis*
            //
            // Should be rendered as:
            //     <p>\<em>emphasis</em></p>

            // Arrange
            var commonMark = Helpers.Normalize("\\\\*emphasis*");
            var expected = Helpers.Normalize("<p>\\<em>emphasis</em></p>");
            Helpers.Log("Example {0}", 210);
            Helpers.Log("Section: {0}", "Inlines - Backslash escapes");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "\\\\*emphasis*");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // A backslash at the end of the line is a hard line break:
        [TestMethod]
        [TestCategory("Inlines - Backslash escapes")]
        //[Timeout(1000)]
        public void Example211()
        {
            // Example 211
            // Section: Inlines - Backslash escapes
            //
            // The following CommonMark:
            //     foo\
            //     bar
            //
            // Should be rendered as:
            //     <p>foo<br />
            //     bar</p>

            // Arrange
            var commonMark = Helpers.Normalize("foo\\\nbar");
            var expected = Helpers.Normalize("<p>foo<br />\nbar</p>");
            Helpers.Log("Example {0}", 211);
            Helpers.Log("Section: {0}", "Inlines - Backslash escapes");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "foo\\\nbar");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // Backslash escapes do not work in code blocks, code spans, autolinks, or
        // raw HTML:
        [TestMethod]
        [TestCategory("Inlines - Backslash escapes")]
        //[Timeout(1000)]
        public void Example212()
        {
            // Example 212
            // Section: Inlines - Backslash escapes
            //
            // The following CommonMark:
            //     `` \[\` ``
            //
            // Should be rendered as:
            //     <p><code>\[\`</code></p>

            // Arrange
            var commonMark = Helpers.Normalize("`` \\[\\` ``");
            var expected = Helpers.Normalize("<p><code>\\[\\`</code></p>");
            Helpers.Log("Example {0}", 212);
            Helpers.Log("Section: {0}", "Inlines - Backslash escapes");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "`` \\[\\` ``");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        [TestMethod]
        [TestCategory("Inlines - Backslash escapes")]
        //[Timeout(1000)]
        public void Example213()
        {
            // Example 213
            // Section: Inlines - Backslash escapes
            //
            // The following CommonMark:
            //         \[\]
            //
            // Should be rendered as:
            //     <pre><code>\[\]
            //     </code></pre>

            // Arrange
            var commonMark = Helpers.Normalize("    \\[\\]");
            var expected = Helpers.Normalize("<pre><code>\\[\\]\n</code></pre>");
            Helpers.Log("Example {0}", 213);
            Helpers.Log("Section: {0}", "Inlines - Backslash escapes");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "    \\[\\]");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        [TestMethod]
        [TestCategory("Inlines - Backslash escapes")]
        //[Timeout(1000)]
        public void Example214()
        {
            // Example 214
            // Section: Inlines - Backslash escapes
            //
            // The following CommonMark:
            //     ~~~
            //     \[\]
            //     ~~~
            //
            // Should be rendered as:
            //     <pre><code>\[\]
            //     </code></pre>

            // Arrange
            var commonMark = Helpers.Normalize("~~~\n\\[\\]\n~~~");
            var expected = Helpers.Normalize("<pre><code>\\[\\]\n</code></pre>");
            Helpers.Log("Example {0}", 214);
            Helpers.Log("Section: {0}", "Inlines - Backslash escapes");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "~~~\n\\[\\]\n~~~");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        [TestMethod]
        [TestCategory("Inlines - Backslash escapes")]
        //[Timeout(1000)]
        public void Example215()
        {
            // Example 215
            // Section: Inlines - Backslash escapes
            //
            // The following CommonMark:
            //     <http://google.com?find=\*>
            //
            // Should be rendered as:
            //     <p><a href="http://google.com?find=\*">http://google.com?find=\*</a></p>

            // Arrange
            var commonMark = Helpers.Normalize("<http://google.com?find=\\*>");
            var expected = Helpers.Normalize("<p><a href=\"http://google.com?find=\\*\">http://google.com?find=\\*</a></p>");
            Helpers.Log("Example {0}", 215);
            Helpers.Log("Section: {0}", "Inlines - Backslash escapes");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "<http://google.com?find=\\*>");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        [TestMethod]
        [TestCategory("Inlines - Backslash escapes")]
        //[Timeout(1000)]
        public void Example216()
        {
            // Example 216
            // Section: Inlines - Backslash escapes
            //
            // The following CommonMark:
            //     <a href="/bar\/)">
            //
            // Should be rendered as:
            //     <p><a href="/bar\/)"></p>

            // Arrange
            var commonMark = Helpers.Normalize("<a href=\"/bar\\/)\">");
            var expected = Helpers.Normalize("<p><a href=\"/bar\\/)\"></p>");
            Helpers.Log("Example {0}", 216);
            Helpers.Log("Section: {0}", "Inlines - Backslash escapes");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "<a href=\"/bar\\/)\">");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // But they work in all other contexts, including URLs and link titles,
        // link references, and info strings in [fenced code
        // blocks](#fenced-code-block):
        [TestMethod]
        [TestCategory("Inlines - Backslash escapes")]
        //[Timeout(1000)]
        public void Example217()
        {
            // Example 217
            // Section: Inlines - Backslash escapes
            //
            // The following CommonMark:
            //     [foo](/bar\* "ti\*tle")
            //
            // Should be rendered as:
            //     <p><a href="/bar*" title="ti*tle">foo</a></p>

            // Arrange
            var commonMark = Helpers.Normalize("[foo](/bar\\* \"ti\\*tle\")");
            var expected = Helpers.Normalize("<p><a href=\"/bar*\" title=\"ti*tle\">foo</a></p>");
            Helpers.Log("Example {0}", 217);
            Helpers.Log("Section: {0}", "Inlines - Backslash escapes");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "[foo](/bar\\* \"ti\\*tle\")");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        [TestMethod]
        [TestCategory("Inlines - Backslash escapes")]
        //[Timeout(1000)]
        public void Example218()
        {
            // Example 218
            // Section: Inlines - Backslash escapes
            //
            // The following CommonMark:
            //     [foo]
            //     
            //     [foo]: /bar\* "ti\*tle"
            //
            // Should be rendered as:
            //     <p><a href="/bar*" title="ti*tle">foo</a></p>

            // Arrange
            var commonMark = Helpers.Normalize("[foo]\n\n[foo]: /bar\\* \"ti\\*tle\"");
            var expected = Helpers.Normalize("<p><a href=\"/bar*\" title=\"ti*tle\">foo</a></p>");
            Helpers.Log("Example {0}", 218);
            Helpers.Log("Section: {0}", "Inlines - Backslash escapes");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "[foo]\n\n[foo]: /bar\\* \"ti\\*tle\"");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        [TestMethod]
        [TestCategory("Inlines - Backslash escapes")]
        //[Timeout(1000)]
        public void Example219()
        {
            // Example 219
            // Section: Inlines - Backslash escapes
            //
            // The following CommonMark:
            //     ``` foo\+bar
            //     foo
            //     ```
            //
            // Should be rendered as:
            //     <pre><code class="language-foo+bar">foo
            //     </code></pre>

            // Arrange
            var commonMark = Helpers.Normalize("``` foo\\+bar\nfoo\n```");
            var expected = Helpers.Normalize("<pre><code class=\"language-foo+bar\">foo\n</code></pre>");
            Helpers.Log("Example {0}", 219);
            Helpers.Log("Section: {0}", "Inlines - Backslash escapes");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "``` foo\\+bar\nfoo\n```");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // ## Entities
        //
        // Entities are parsed as entities, not as literal text, in all contexts
        // except code spans and code blocks. Three kinds of entities are recognized.
        //
        // [Named entities](#name-entities) <a id="named-entities"></a> consist of `&`
        // + a string of 2-32 alphanumerics beginning with a letter + `;`.
        [TestMethod]
        [TestCategory("Inlines - Entities")]
        //[Timeout(1000)]
        public void Example220()
        {
            // Example 220
            // Section: Inlines - Entities
            //
            // The following CommonMark:
            //     &nbsp; &amp; &copy; &AElig; &Dcaron; &frac34; &HilbertSpace; &DifferentialD; &ClockwiseContourIntegral;
            //
            // Should be rendered as:
            //     <p>&nbsp; &amp; &copy; &AElig; &Dcaron; &frac34; &HilbertSpace; &DifferentialD; &ClockwiseContourIntegral;</p>

            // Arrange
            var commonMark = Helpers.Normalize("&nbsp; &amp; &copy; &AElig; &Dcaron; &frac34; &HilbertSpace; &DifferentialD; &ClockwiseContourIntegral;");
            var expected = Helpers.Normalize("<p>&nbsp; &amp; &copy; &AElig; &Dcaron; &frac34; &HilbertSpace; &DifferentialD; &ClockwiseContourIntegral;</p>");
            Helpers.Log("Example {0}", 220);
            Helpers.Log("Section: {0}", "Inlines - Entities");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "&nbsp; &amp; &copy; &AElig; &Dcaron; &frac34; &HilbertSpace; &DifferentialD; &ClockwiseContourIntegral;");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // [Decimal entities](#decimal-entities) <a id="decimal-entities"></a>
        // consist of `&#` + a string of 1--8 arabic digits + `;`.
        [TestMethod]
        [TestCategory("Inlines - Entities")]
        //[Timeout(1000)]
        public void Example221()
        {
            // Example 221
            // Section: Inlines - Entities
            //
            // The following CommonMark:
            //     &#1; &#35; &#1234; &#992; &#98765432;
            //
            // Should be rendered as:
            //     <p>&#1; &#35; &#1234; &#992; &#98765432;</p>

            // Arrange
            var commonMark = Helpers.Normalize("&#1; &#35; &#1234; &#992; &#98765432;");
            var expected = Helpers.Normalize("<p>&#1; &#35; &#1234; &#992; &#98765432;</p>");
            Helpers.Log("Example {0}", 221);
            Helpers.Log("Section: {0}", "Inlines - Entities");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "&#1; &#35; &#1234; &#992; &#98765432;");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // [Hexadecimal entities](#hexadecimal-entities) <a id="hexadecimal-entities"></a>
        // consist of `&#` + either `X` or `x` + a string of 1-8 hexadecimal digits
        // + `;`.
        [TestMethod]
        [TestCategory("Inlines - Entities")]
        //[Timeout(1000)]
        public void Example222()
        {
            // Example 222
            // Section: Inlines - Entities
            //
            // The following CommonMark:
            //     &#x1; &#X22; &#XD06; &#xcab;
            //
            // Should be rendered as:
            //     <p>&#x1; &#X22; &#XD06; &#xcab;</p>

            // Arrange
            var commonMark = Helpers.Normalize("&#x1; &#X22; &#XD06; &#xcab;");
            var expected = Helpers.Normalize("<p>&#x1; &#X22; &#XD06; &#xcab;</p>");
            Helpers.Log("Example {0}", 222);
            Helpers.Log("Section: {0}", "Inlines - Entities");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "&#x1; &#X22; &#XD06; &#xcab;");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // Here are some nonentities:
        [TestMethod]
        [TestCategory("Inlines - Entities")]
        //[Timeout(1000)]
        public void Example223()
        {
            // Example 223
            // Section: Inlines - Entities
            //
            // The following CommonMark:
            //     &nbsp &x; &#; &#x; &#123456789; &ThisIsWayTooLongToBeAnEntityIsntIt; &hi?;
            //
            // Should be rendered as:
            //     <p>&amp;nbsp &amp;x; &amp;#; &amp;#x; &amp;#123456789; &amp;ThisIsWayTooLongToBeAnEntityIsntIt; &amp;hi?;</p>

            // Arrange
            var commonMark = Helpers.Normalize("&nbsp &x; &#; &#x; &#123456789; &ThisIsWayTooLongToBeAnEntityIsntIt; &hi?;");
            var expected = Helpers.Normalize("<p>&amp;nbsp &amp;x; &amp;#; &amp;#x; &amp;#123456789; &amp;ThisIsWayTooLongToBeAnEntityIsntIt; &amp;hi?;</p>");
            Helpers.Log("Example {0}", 223);
            Helpers.Log("Section: {0}", "Inlines - Entities");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "&nbsp &x; &#; &#x; &#123456789; &ThisIsWayTooLongToBeAnEntityIsntIt; &hi?;");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // Although HTML5 does accept some entities without a trailing semicolon
        // (such as `&copy`), these are not recognized as entities here:
        [TestMethod]
        [TestCategory("Inlines - Entities")]
        //[Timeout(1000)]
        public void Example224()
        {
            // Example 224
            // Section: Inlines - Entities
            //
            // The following CommonMark:
            //     &copy
            //
            // Should be rendered as:
            //     <p>&amp;copy</p>

            // Arrange
            var commonMark = Helpers.Normalize("&copy");
            var expected = Helpers.Normalize("<p>&amp;copy</p>");
            Helpers.Log("Example {0}", 224);
            Helpers.Log("Section: {0}", "Inlines - Entities");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "&copy");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // On the other hand, many strings that are not on the list of HTML5
        // named entities are recognized as entities here:
        [TestMethod]
        [TestCategory("Inlines - Entities")]
        //[Timeout(1000)]
        public void Example225()
        {
            // Example 225
            // Section: Inlines - Entities
            //
            // The following CommonMark:
            //     &MadeUpEntity;
            //
            // Should be rendered as:
            //     <p>&MadeUpEntity;</p>

            // Arrange
            var commonMark = Helpers.Normalize("&MadeUpEntity;");
            var expected = Helpers.Normalize("<p>&MadeUpEntity;</p>");
            Helpers.Log("Example {0}", 225);
            Helpers.Log("Section: {0}", "Inlines - Entities");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "&MadeUpEntity;");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // Entities are recognized in any context besides code spans or
        // code blocks, including raw HTML, URLs, [link titles](#link-title), and
        // [fenced code block](#fenced-code-block) info strings:
        [TestMethod]
        [TestCategory("Inlines - Entities")]
        //[Timeout(1000)]
        public void Example226()
        {
            // Example 226
            // Section: Inlines - Entities
            //
            // The following CommonMark:
            //     <a href="&ouml;&ouml;.html">
            //
            // Should be rendered as:
            //     <p><a href="&ouml;&ouml;.html"></p>

            // Arrange
            var commonMark = Helpers.Normalize("<a href=\"&ouml;&ouml;.html\">");
            var expected = Helpers.Normalize("<p><a href=\"&ouml;&ouml;.html\"></p>");
            Helpers.Log("Example {0}", 226);
            Helpers.Log("Section: {0}", "Inlines - Entities");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "<a href=\"&ouml;&ouml;.html\">");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        [TestMethod]
        [TestCategory("Inlines - Entities")]
        //[Timeout(1000)]
        public void Example227()
        {
            // Example 227
            // Section: Inlines - Entities
            //
            // The following CommonMark:
            //     [foo](/f&ouml;&ouml; "f&ouml;&ouml;")
            //
            // Should be rendered as:
            //     <p><a href="/f&ouml;&ouml;" title="f&ouml;&ouml;">foo</a></p>

            // Arrange
            var commonMark = Helpers.Normalize("[foo](/f&ouml;&ouml; \"f&ouml;&ouml;\")");
            var expected = Helpers.Normalize("<p><a href=\"/f&ouml;&ouml;\" title=\"f&ouml;&ouml;\">foo</a></p>");
            Helpers.Log("Example {0}", 227);
            Helpers.Log("Section: {0}", "Inlines - Entities");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "[foo](/f&ouml;&ouml; \"f&ouml;&ouml;\")");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        [TestMethod]
        [TestCategory("Inlines - Entities")]
        //[Timeout(1000)]
        public void Example228()
        {
            // Example 228
            // Section: Inlines - Entities
            //
            // The following CommonMark:
            //     [foo]
            //     
            //     [foo]: /f&ouml;&ouml; "f&ouml;&ouml;"
            //
            // Should be rendered as:
            //     <p><a href="/f&ouml;&ouml;" title="f&ouml;&ouml;">foo</a></p>

            // Arrange
            var commonMark = Helpers.Normalize("[foo]\n\n[foo]: /f&ouml;&ouml; \"f&ouml;&ouml;\"");
            var expected = Helpers.Normalize("<p><a href=\"/f&ouml;&ouml;\" title=\"f&ouml;&ouml;\">foo</a></p>");
            Helpers.Log("Example {0}", 228);
            Helpers.Log("Section: {0}", "Inlines - Entities");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "[foo]\n\n[foo]: /f&ouml;&ouml; \"f&ouml;&ouml;\"");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        [TestMethod]
        [TestCategory("Inlines - Entities")]
        //[Timeout(1000)]
        public void Example229()
        {
            // Example 229
            // Section: Inlines - Entities
            //
            // The following CommonMark:
            //     ``` f&ouml;&ouml;
            //     foo
            //     ```
            //
            // Should be rendered as:
            //     <pre><code class="language-f&ouml;&ouml;">foo
            //     </code></pre>

            // Arrange
            var commonMark = Helpers.Normalize("``` f&ouml;&ouml;\nfoo\n```");
            var expected = Helpers.Normalize("<pre><code class=\"language-f&ouml;&ouml;\">foo\n</code></pre>");
            Helpers.Log("Example {0}", 229);
            Helpers.Log("Section: {0}", "Inlines - Entities");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "``` f&ouml;&ouml;\nfoo\n```");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // Entities are treated as literal text in code spans and code blocks:
        [TestMethod]
        [TestCategory("Inlines - Entities")]
        //[Timeout(1000)]
        public void Example230()
        {
            // Example 230
            // Section: Inlines - Entities
            //
            // The following CommonMark:
            //     `f&ouml;&ouml;`
            //
            // Should be rendered as:
            //     <p><code>f&amp;ouml;&amp;ouml;</code></p>

            // Arrange
            var commonMark = Helpers.Normalize("`f&ouml;&ouml;`");
            var expected = Helpers.Normalize("<p><code>f&amp;ouml;&amp;ouml;</code></p>");
            Helpers.Log("Example {0}", 230);
            Helpers.Log("Section: {0}", "Inlines - Entities");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "`f&ouml;&ouml;`");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        [TestMethod]
        [TestCategory("Inlines - Entities")]
        //[Timeout(1000)]
        public void Example231()
        {
            // Example 231
            // Section: Inlines - Entities
            //
            // The following CommonMark:
            //         f&ouml;f&ouml;
            //
            // Should be rendered as:
            //     <pre><code>f&amp;ouml;f&amp;ouml;
            //     </code></pre>

            // Arrange
            var commonMark = Helpers.Normalize("    f&ouml;f&ouml;");
            var expected = Helpers.Normalize("<pre><code>f&amp;ouml;f&amp;ouml;\n</code></pre>");
            Helpers.Log("Example {0}", 231);
            Helpers.Log("Section: {0}", "Inlines - Entities");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "    f&ouml;f&ouml;");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // ## Code span
        //
        // A [backtick string](#backtick-string) <a id="backtick-string"></a>
        // is a string of one or more backtick characters (`` ` ``) that is neither
        // preceded nor followed by a backtick.
        //
        // A code span begins with a backtick string and ends with a backtick
        // string of equal length.  The contents of the code span are the
        // characters between the two backtick strings, with leading and trailing
        // spaces and newlines removed, and consecutive spaces and newlines
        // collapsed to single spaces.
        //
        // This is a simple code span:
        [TestMethod]
        [TestCategory("Inlines - Code span")]
        //[Timeout(1000)]
        public void Example232()
        {
            // Example 232
            // Section: Inlines - Code span
            //
            // The following CommonMark:
            //     `foo`
            //
            // Should be rendered as:
            //     <p><code>foo</code></p>

            // Arrange
            var commonMark = Helpers.Normalize("`foo`");
            var expected = Helpers.Normalize("<p><code>foo</code></p>");
            Helpers.Log("Example {0}", 232);
            Helpers.Log("Section: {0}", "Inlines - Code span");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "`foo`");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // Here two backticks are used, because the code contains a backtick.
        // This example also illustrates stripping of leading and trailing spaces:
        [TestMethod]
        [TestCategory("Inlines - Code span")]
        //[Timeout(1000)]
        public void Example233()
        {
            // Example 233
            // Section: Inlines - Code span
            //
            // The following CommonMark:
            //     `` foo ` bar  ``
            //
            // Should be rendered as:
            //     <p><code>foo ` bar</code></p>

            // Arrange
            var commonMark = Helpers.Normalize("`` foo ` bar  ``");
            var expected = Helpers.Normalize("<p><code>foo ` bar</code></p>");
            Helpers.Log("Example {0}", 233);
            Helpers.Log("Section: {0}", "Inlines - Code span");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "`` foo ` bar  ``");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // This example shows the motivation for stripping leading and trailing
        // spaces:
        [TestMethod]
        [TestCategory("Inlines - Code span")]
        //[Timeout(1000)]
        public void Example234()
        {
            // Example 234
            // Section: Inlines - Code span
            //
            // The following CommonMark:
            //     ` `` `
            //
            // Should be rendered as:
            //     <p><code>``</code></p>

            // Arrange
            var commonMark = Helpers.Normalize("` `` `");
            var expected = Helpers.Normalize("<p><code>``</code></p>");
            Helpers.Log("Example {0}", 234);
            Helpers.Log("Section: {0}", "Inlines - Code span");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "` `` `");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // Newlines are treated like spaces:
        [TestMethod]
        [TestCategory("Inlines - Code span")]
        //[Timeout(1000)]
        public void Example235()
        {
            // Example 235
            // Section: Inlines - Code span
            //
            // The following CommonMark:
            //     ``
            //     foo
            //     ``
            //
            // Should be rendered as:
            //     <p><code>foo</code></p>

            // Arrange
            var commonMark = Helpers.Normalize("``\nfoo\n``");
            var expected = Helpers.Normalize("<p><code>foo</code></p>");
            Helpers.Log("Example {0}", 235);
            Helpers.Log("Section: {0}", "Inlines - Code span");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "``\nfoo\n``");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // Interior spaces and newlines are collapsed into single spaces, just
        // as they would be by a browser:
        [TestMethod]
        [TestCategory("Inlines - Code span")]
        //[Timeout(1000)]
        public void Example236()
        {
            // Example 236
            // Section: Inlines - Code span
            //
            // The following CommonMark:
            //     `foo   bar
            //       baz`
            //
            // Should be rendered as:
            //     <p><code>foo bar baz</code></p>

            // Arrange
            var commonMark = Helpers.Normalize("`foo   bar\n  baz`");
            var expected = Helpers.Normalize("<p><code>foo bar baz</code></p>");
            Helpers.Log("Example {0}", 236);
            Helpers.Log("Section: {0}", "Inlines - Code span");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "`foo   bar\n  baz`");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // Q: Why not just leave the spaces, since browsers will collapse them
        // anyway?  A:  Because we might be targeting a non-HTML format, and we
        // shouldn't rely on HTML-specific rendering assumptions.
        //
        // (Existing implementations differ in their treatment of internal
        // spaces and newlines.  Some, including `Markdown.pl` and
        // `showdown`, convert an internal newline into a `<br />` tag.
        // But this makes things difficult for those who like to hard-wrap
        // their paragraphs, since a line break in the midst of a code
        // span will cause an unintended line break in the output.  Others
        // just leave internal spaces as they are, which is fine if only
        // HTML is being targeted.)
        [TestMethod]
        [TestCategory("Inlines - Code span")]
        //[Timeout(1000)]
        public void Example237()
        {
            // Example 237
            // Section: Inlines - Code span
            //
            // The following CommonMark:
            //     `foo `` bar`
            //
            // Should be rendered as:
            //     <p><code>foo `` bar</code></p>

            // Arrange
            var commonMark = Helpers.Normalize("`foo `` bar`");
            var expected = Helpers.Normalize("<p><code>foo `` bar</code></p>");
            Helpers.Log("Example {0}", 237);
            Helpers.Log("Section: {0}", "Inlines - Code span");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "`foo `` bar`");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // Note that backslash escapes do not work in code spans. All backslashes
        // are treated literally:
        [TestMethod]
        [TestCategory("Inlines - Code span")]
        //[Timeout(1000)]
        public void Example238()
        {
            // Example 238
            // Section: Inlines - Code span
            //
            // The following CommonMark:
            //     `foo\`bar`
            //
            // Should be rendered as:
            //     <p><code>foo\</code>bar`</p>

            // Arrange
            var commonMark = Helpers.Normalize("`foo\\`bar`");
            var expected = Helpers.Normalize("<p><code>foo\\</code>bar`</p>");
            Helpers.Log("Example {0}", 238);
            Helpers.Log("Section: {0}", "Inlines - Code span");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "`foo\\`bar`");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // Backslash escapes are never needed, because one can always choose a
        // string of *n* backtick characters as delimiters, where the code does
        // not contain any strings of exactly *n* backtick characters.
        //
        // Code span backticks have higher precedence than any other inline
        // constructs except HTML tags and autolinks.  Thus, for example, this is
        // not parsed as emphasized text, since the second `*` is part of a code
        // span:
        [TestMethod]
        [TestCategory("Inlines - Code span")]
        //[Timeout(1000)]
        public void Example239()
        {
            // Example 239
            // Section: Inlines - Code span
            //
            // The following CommonMark:
            //     *foo`*`
            //
            // Should be rendered as:
            //     <p>*foo<code>*</code></p>

            // Arrange
            var commonMark = Helpers.Normalize("*foo`*`");
            var expected = Helpers.Normalize("<p>*foo<code>*</code></p>");
            Helpers.Log("Example {0}", 239);
            Helpers.Log("Section: {0}", "Inlines - Code span");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "*foo`*`");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // And this is not parsed as a link:
        [TestMethod]
        [TestCategory("Inlines - Code span")]
        //[Timeout(1000)]
        public void Example240()
        {
            // Example 240
            // Section: Inlines - Code span
            //
            // The following CommonMark:
            //     [not a `link](/foo`)
            //
            // Should be rendered as:
            //     <p>[not a <code>link](/foo</code>)</p>

            // Arrange
            var commonMark = Helpers.Normalize("[not a `link](/foo`)");
            var expected = Helpers.Normalize("<p>[not a <code>link](/foo</code>)</p>");
            Helpers.Log("Example {0}", 240);
            Helpers.Log("Section: {0}", "Inlines - Code span");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "[not a `link](/foo`)");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // But this is a link:
        [TestMethod]
        [TestCategory("Inlines - Code span")]
        //[Timeout(1000)]
        public void Example241()
        {
            // Example 241
            // Section: Inlines - Code span
            //
            // The following CommonMark:
            //     <http://foo.bar.`baz>`
            //
            // Should be rendered as:
            //     <p><a href="http://foo.bar.`baz">http://foo.bar.`baz</a>`</p>

            // Arrange
            var commonMark = Helpers.Normalize("<http://foo.bar.`baz>`");
            var expected = Helpers.Normalize("<p><a href=\"http://foo.bar.`baz\">http://foo.bar.`baz</a>`</p>");
            Helpers.Log("Example {0}", 241);
            Helpers.Log("Section: {0}", "Inlines - Code span");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "<http://foo.bar.`baz>`");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // And this is an HTML tag:
        [TestMethod]
        [TestCategory("Inlines - Code span")]
        //[Timeout(1000)]
        public void Example242()
        {
            // Example 242
            // Section: Inlines - Code span
            //
            // The following CommonMark:
            //     <a href="`">`
            //
            // Should be rendered as:
            //     <p><a href="`">`</p>

            // Arrange
            var commonMark = Helpers.Normalize("<a href=\"`\">`");
            var expected = Helpers.Normalize("<p><a href=\"`\">`</p>");
            Helpers.Log("Example {0}", 242);
            Helpers.Log("Section: {0}", "Inlines - Code span");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "<a href=\"`\">`");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // When a backtick string is not closed by a matching backtick string,
        // we just have literal backticks:
        [TestMethod]
        [TestCategory("Inlines - Code span")]
        //[Timeout(1000)]
        public void Example243()
        {
            // Example 243
            // Section: Inlines - Code span
            //
            // The following CommonMark:
            //     ```foo``
            //
            // Should be rendered as:
            //     <p>```foo``</p>

            // Arrange
            var commonMark = Helpers.Normalize("```foo``");
            var expected = Helpers.Normalize("<p>```foo``</p>");
            Helpers.Log("Example {0}", 243);
            Helpers.Log("Section: {0}", "Inlines - Code span");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "```foo``");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        [TestMethod]
        [TestCategory("Inlines - Code span")]
        //[Timeout(1000)]
        public void Example244()
        {
            // Example 244
            // Section: Inlines - Code span
            //
            // The following CommonMark:
            //     `foo
            //
            // Should be rendered as:
            //     <p>`foo</p>

            // Arrange
            var commonMark = Helpers.Normalize("`foo");
            var expected = Helpers.Normalize("<p>`foo</p>");
            Helpers.Log("Example {0}", 244);
            Helpers.Log("Section: {0}", "Inlines - Code span");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "`foo");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // ## Emphasis and strong emphasis
        //
        // John Gruber's original [Markdown syntax
        // description](http://daringfireball.net/projects/markdown/syntax#em) says:
        //
        // > Markdown treats asterisks (`*`) and underscores (`_`) as indicators of
        // > emphasis. Text wrapped with one `*` or `_` will be wrapped with an HTML
        // > `<em>` tag; double `*`'s or `_`'s will be wrapped with an HTML `<strong>`
        // > tag.
        //
        // This is enough for most users, but these rules leave much undecided,
        // especially when it comes to nested emphasis.  The original
        // `Markdown.pl` test suite makes it clear that triple `***` and
        // `___` delimiters can be used for strong emphasis, and most
        // implementations have also allowed the following patterns:
        //
        // ``` markdown
        // ***strong emph***
        // ***strong** in emph*
        // ***emph* in strong**
        // **in strong *emph***
        // *in emph **strong***
        // ```
        //
        // The following patterns are less widely supported, but the intent
        // is clear and they are useful (especially in contexts like bibliography
        // entries):
        //
        // ``` markdown
        // *emph *with emph* in it*
        // **strong **with strong** in it**
        // ```
        //
        // Many implementations have also restricted intraword emphasis to
        // the `*` forms, to avoid unwanted emphasis in words containing
        // internal underscores.  (It is best practice to put these in code
        // spans, but users often do not.)
        //
        // ``` markdown
        // internal emphasis: foo*bar*baz
        // no emphasis: foo_bar_baz
        // ```
        //
        // The following rules capture all of these patterns, while allowing
        // for efficient parsing strategies that do not backtrack:
        //
        // 1.  A single `*` character [can open emphasis](#can-open-emphasis)
        // <a id="can-open-emphasis"></a> iff
        //
        // (a) it is not part of a sequence of four or more unescaped `*`s,
        // (b) it is not followed by whitespace, and
        // (c) either it is not followed by a `*` character or it is
        // followed immediately by strong emphasis.
        //
        // 2.  A single `_` character [can open emphasis](#can-open-emphasis) iff
        //
        // (a) it is not part of a sequence of four or more unescaped `_`s,
        // (b) it is not followed by whitespace,
        // (c) is is not preceded by an ASCII alphanumeric character, and
        // (d) either it is not followed by a `_` character or it is
        // followed immediately by strong emphasis.
        //
        // 3.  A single `*` character [can close emphasis](#can-close-emphasis)
        // <a id="can-close-emphasis"></a> iff
        //
        // (a) it is not part of a sequence of four or more unescaped `*`s, and
        // (b) it is not preceded by whitespace.
        //
        // 4.  A single `_` character [can close emphasis](#can-close-emphasis) iff
        //
        // (a) it is not part of a sequence of four or more unescaped `_`s,
        // (b) it is not preceded by whitespace, and
        // (c) it is not followed by an ASCII alphanumeric character.
        //
        // 5.  A double `**` [can open strong emphasis](#can-open-strong-emphasis)
        // <a id="can-open-strong-emphasis" ></a> iff
        //
        // (a) it is not part of a sequence of four or more unescaped `*`s,
        // (b) it is not followed by whitespace, and
        // (c) either it is not followed by a `*` character or it is
        // followed immediately by emphasis.
        //
        // 6.  A double `__` [can open strong emphasis](#can-open-strong-emphasis)
        // iff
        //
        // (a) it is not part of a sequence of four or more unescaped `_`s,
        // (b) it is not followed by whitespace, and
        // (c) it is not preceded by an ASCII alphanumeric character, and
        // (d) either it is not followed by a `_` character or it is
        // followed immediately by emphasis.
        //
        // 7.  A double `**` [can close strong emphasis](#can-close-strong-emphasis)
        // <a id="can-close-strong-emphasis" ></a> iff
        //
        // (a) it is not part of a sequence of four or more unescaped `*`s, and
        // (b) it is not preceded by whitespace.
        //
        // 8.  A double `__` [can close strong emphasis](#can-close-strong-emphasis)
        // iff
        //
        // (a) it is not part of a sequence of four or more unescaped `_`s,
        // (b) it is not preceded by whitespace, and
        // (c) it is not followed by an ASCII alphanumeric character.
        //
        // 9.  Emphasis begins with a delimiter that [can open
        // emphasis](#can-open-emphasis) and includes inlines parsed
        // sequentially until a delimiter that [can close
        // emphasis](#can-close-emphasis), and that uses the same
        // character (`_` or `*`) as the opening delimiter, is reached.
        //
        // 10. Strong emphasis begins with a delimiter that [can open strong
        // emphasis](#can-open-strong-emphasis) and includes inlines parsed
        // sequentially until a delimiter that [can close strong
        // emphasis](#can-close-strong-emphasis), and that uses the
        // same character (`_` or `*`) as the opening delimiter, is reached.
        //
        // These rules can be illustrated through a series of examples.
        //
        // Simple emphasis:
        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example245()
        {
            // Example 245
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     *foo bar*
            //
            // Should be rendered as:
            //     <p><em>foo bar</em></p>

            // Arrange
            var commonMark = Helpers.Normalize("*foo bar*");
            var expected = Helpers.Normalize("<p><em>foo bar</em></p>");
            Helpers.Log("Example {0}", 245);
            Helpers.Log("Section: {0}", "Inlines - Emphasis and strong emphasis");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "*foo bar*");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example246()
        {
            // Example 246
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     _foo bar_
            //
            // Should be rendered as:
            //     <p><em>foo bar</em></p>

            // Arrange
            var commonMark = Helpers.Normalize("_foo bar_");
            var expected = Helpers.Normalize("<p><em>foo bar</em></p>");
            Helpers.Log("Example {0}", 246);
            Helpers.Log("Section: {0}", "Inlines - Emphasis and strong emphasis");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "_foo bar_");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // Simple strong emphasis:
        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example247()
        {
            // Example 247
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     **foo bar**
            //
            // Should be rendered as:
            //     <p><strong>foo bar</strong></p>

            // Arrange
            var commonMark = Helpers.Normalize("**foo bar**");
            var expected = Helpers.Normalize("<p><strong>foo bar</strong></p>");
            Helpers.Log("Example {0}", 247);
            Helpers.Log("Section: {0}", "Inlines - Emphasis and strong emphasis");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "**foo bar**");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example248()
        {
            // Example 248
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     __foo bar__
            //
            // Should be rendered as:
            //     <p><strong>foo bar</strong></p>

            // Arrange
            var commonMark = Helpers.Normalize("__foo bar__");
            var expected = Helpers.Normalize("<p><strong>foo bar</strong></p>");
            Helpers.Log("Example {0}", 248);
            Helpers.Log("Section: {0}", "Inlines - Emphasis and strong emphasis");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "__foo bar__");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // Emphasis can continue over line breaks:
        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example249()
        {
            // Example 249
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     *foo
            //     bar*
            //
            // Should be rendered as:
            //     <p><em>foo
            //     bar</em></p>

            // Arrange
            var commonMark = Helpers.Normalize("*foo\nbar*");
            var expected = Helpers.Normalize("<p><em>foo\nbar</em></p>");
            Helpers.Log("Example {0}", 249);
            Helpers.Log("Section: {0}", "Inlines - Emphasis and strong emphasis");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "*foo\nbar*");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example250()
        {
            // Example 250
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     _foo
            //     bar_
            //
            // Should be rendered as:
            //     <p><em>foo
            //     bar</em></p>

            // Arrange
            var commonMark = Helpers.Normalize("_foo\nbar_");
            var expected = Helpers.Normalize("<p><em>foo\nbar</em></p>");
            Helpers.Log("Example {0}", 250);
            Helpers.Log("Section: {0}", "Inlines - Emphasis and strong emphasis");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "_foo\nbar_");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example251()
        {
            // Example 251
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     **foo
            //     bar**
            //
            // Should be rendered as:
            //     <p><strong>foo
            //     bar</strong></p>

            // Arrange
            var commonMark = Helpers.Normalize("**foo\nbar**");
            var expected = Helpers.Normalize("<p><strong>foo\nbar</strong></p>");
            Helpers.Log("Example {0}", 251);
            Helpers.Log("Section: {0}", "Inlines - Emphasis and strong emphasis");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "**foo\nbar**");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example252()
        {
            // Example 252
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     __foo
            //     bar__
            //
            // Should be rendered as:
            //     <p><strong>foo
            //     bar</strong></p>

            // Arrange
            var commonMark = Helpers.Normalize("__foo\nbar__");
            var expected = Helpers.Normalize("<p><strong>foo\nbar</strong></p>");
            Helpers.Log("Example {0}", 252);
            Helpers.Log("Section: {0}", "Inlines - Emphasis and strong emphasis");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "__foo\nbar__");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // Emphasis can contain other inline constructs:
        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example253()
        {
            // Example 253
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     *foo [bar](/url)*
            //
            // Should be rendered as:
            //     <p><em>foo <a href="/url">bar</a></em></p>

            // Arrange
            var commonMark = Helpers.Normalize("*foo [bar](/url)*");
            var expected = Helpers.Normalize("<p><em>foo <a href=\"/url\">bar</a></em></p>");
            Helpers.Log("Example {0}", 253);
            Helpers.Log("Section: {0}", "Inlines - Emphasis and strong emphasis");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "*foo [bar](/url)*");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example254()
        {
            // Example 254
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     _foo [bar](/url)_
            //
            // Should be rendered as:
            //     <p><em>foo <a href="/url">bar</a></em></p>

            // Arrange
            var commonMark = Helpers.Normalize("_foo [bar](/url)_");
            var expected = Helpers.Normalize("<p><em>foo <a href=\"/url\">bar</a></em></p>");
            Helpers.Log("Example {0}", 254);
            Helpers.Log("Section: {0}", "Inlines - Emphasis and strong emphasis");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "_foo [bar](/url)_");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example255()
        {
            // Example 255
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     **foo [bar](/url)**
            //
            // Should be rendered as:
            //     <p><strong>foo <a href="/url">bar</a></strong></p>

            // Arrange
            var commonMark = Helpers.Normalize("**foo [bar](/url)**");
            var expected = Helpers.Normalize("<p><strong>foo <a href=\"/url\">bar</a></strong></p>");
            Helpers.Log("Example {0}", 255);
            Helpers.Log("Section: {0}", "Inlines - Emphasis and strong emphasis");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "**foo [bar](/url)**");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example256()
        {
            // Example 256
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     __foo [bar](/url)__
            //
            // Should be rendered as:
            //     <p><strong>foo <a href="/url">bar</a></strong></p>

            // Arrange
            var commonMark = Helpers.Normalize("__foo [bar](/url)__");
            var expected = Helpers.Normalize("<p><strong>foo <a href=\"/url\">bar</a></strong></p>");
            Helpers.Log("Example {0}", 256);
            Helpers.Log("Section: {0}", "Inlines - Emphasis and strong emphasis");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "__foo [bar](/url)__");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // Symbols contained in other inline constructs will not
        // close emphasis:
        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example257()
        {
            // Example 257
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     *foo [bar*](/url)
            //
            // Should be rendered as:
            //     <p>*foo <a href="/url">bar*</a></p>

            // Arrange
            var commonMark = Helpers.Normalize("*foo [bar*](/url)");
            var expected = Helpers.Normalize("<p>*foo <a href=\"/url\">bar*</a></p>");
            Helpers.Log("Example {0}", 257);
            Helpers.Log("Section: {0}", "Inlines - Emphasis and strong emphasis");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "*foo [bar*](/url)");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example258()
        {
            // Example 258
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     _foo [bar_](/url)
            //
            // Should be rendered as:
            //     <p>_foo <a href="/url">bar_</a></p>

            // Arrange
            var commonMark = Helpers.Normalize("_foo [bar_](/url)");
            var expected = Helpers.Normalize("<p>_foo <a href=\"/url\">bar_</a></p>");
            Helpers.Log("Example {0}", 258);
            Helpers.Log("Section: {0}", "Inlines - Emphasis and strong emphasis");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "_foo [bar_](/url)");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example259()
        {
            // Example 259
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     **<a href="**">
            //
            // Should be rendered as:
            //     <p>**<a href="**"></p>

            // Arrange
            var commonMark = Helpers.Normalize("**<a href=\"**\">");
            var expected = Helpers.Normalize("<p>**<a href=\"**\"></p>");
            Helpers.Log("Example {0}", 259);
            Helpers.Log("Section: {0}", "Inlines - Emphasis and strong emphasis");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "**<a href=\"**\">");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example260()
        {
            // Example 260
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     __<a href="__">
            //
            // Should be rendered as:
            //     <p>__<a href="__"></p>

            // Arrange
            var commonMark = Helpers.Normalize("__<a href=\"__\">");
            var expected = Helpers.Normalize("<p>__<a href=\"__\"></p>");
            Helpers.Log("Example {0}", 260);
            Helpers.Log("Section: {0}", "Inlines - Emphasis and strong emphasis");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "__<a href=\"__\">");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example261()
        {
            // Example 261
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     *a `*`*
            //
            // Should be rendered as:
            //     <p><em>a <code>*</code></em></p>

            // Arrange
            var commonMark = Helpers.Normalize("*a `*`*");
            var expected = Helpers.Normalize("<p><em>a <code>*</code></em></p>");
            Helpers.Log("Example {0}", 261);
            Helpers.Log("Section: {0}", "Inlines - Emphasis and strong emphasis");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "*a `*`*");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example262()
        {
            // Example 262
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     _a `_`_
            //
            // Should be rendered as:
            //     <p><em>a <code>_</code></em></p>

            // Arrange
            var commonMark = Helpers.Normalize("_a `_`_");
            var expected = Helpers.Normalize("<p><em>a <code>_</code></em></p>");
            Helpers.Log("Example {0}", 262);
            Helpers.Log("Section: {0}", "Inlines - Emphasis and strong emphasis");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "_a `_`_");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example263()
        {
            // Example 263
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     **a<http://foo.bar?q=**>
            //
            // Should be rendered as:
            //     <p>**a<a href="http://foo.bar?q=**">http://foo.bar?q=**</a></p>

            // Arrange
            var commonMark = Helpers.Normalize("**a<http://foo.bar?q=**>");
            var expected = Helpers.Normalize("<p>**a<a href=\"http://foo.bar?q=**\">http://foo.bar?q=**</a></p>");
            Helpers.Log("Example {0}", 263);
            Helpers.Log("Section: {0}", "Inlines - Emphasis and strong emphasis");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "**a<http://foo.bar?q=**>");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example264()
        {
            // Example 264
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     __a<http://foo.bar?q=__>
            //
            // Should be rendered as:
            //     <p>__a<a href="http://foo.bar?q=__">http://foo.bar?q=__</a></p>

            // Arrange
            var commonMark = Helpers.Normalize("__a<http://foo.bar?q=__>");
            var expected = Helpers.Normalize("<p>__a<a href=\"http://foo.bar?q=__\">http://foo.bar?q=__</a></p>");
            Helpers.Log("Example {0}", 264);
            Helpers.Log("Section: {0}", "Inlines - Emphasis and strong emphasis");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "__a<http://foo.bar?q=__>");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // This is not emphasis, because the opening delimiter is
        // followed by white space:
        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example265()
        {
            // Example 265
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     and * foo bar*
            //
            // Should be rendered as:
            //     <p>and * foo bar*</p>

            // Arrange
            var commonMark = Helpers.Normalize("and * foo bar*");
            var expected = Helpers.Normalize("<p>and * foo bar*</p>");
            Helpers.Log("Example {0}", 265);
            Helpers.Log("Section: {0}", "Inlines - Emphasis and strong emphasis");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "and * foo bar*");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example266()
        {
            // Example 266
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     _ foo bar_
            //
            // Should be rendered as:
            //     <p>_ foo bar_</p>

            // Arrange
            var commonMark = Helpers.Normalize("_ foo bar_");
            var expected = Helpers.Normalize("<p>_ foo bar_</p>");
            Helpers.Log("Example {0}", 266);
            Helpers.Log("Section: {0}", "Inlines - Emphasis and strong emphasis");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "_ foo bar_");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example267()
        {
            // Example 267
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     and ** foo bar**
            //
            // Should be rendered as:
            //     <p>and ** foo bar**</p>

            // Arrange
            var commonMark = Helpers.Normalize("and ** foo bar**");
            var expected = Helpers.Normalize("<p>and ** foo bar**</p>");
            Helpers.Log("Example {0}", 267);
            Helpers.Log("Section: {0}", "Inlines - Emphasis and strong emphasis");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "and ** foo bar**");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example268()
        {
            // Example 268
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     __ foo bar__
            //
            // Should be rendered as:
            //     <p>__ foo bar__</p>

            // Arrange
            var commonMark = Helpers.Normalize("__ foo bar__");
            var expected = Helpers.Normalize("<p>__ foo bar__</p>");
            Helpers.Log("Example {0}", 268);
            Helpers.Log("Section: {0}", "Inlines - Emphasis and strong emphasis");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "__ foo bar__");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // This is not emphasis, because the closing delimiter is
        // preceded by white space:
        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example269()
        {
            // Example 269
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     and *foo bar *
            //
            // Should be rendered as:
            //     <p>and *foo bar *</p>

            // Arrange
            var commonMark = Helpers.Normalize("and *foo bar *");
            var expected = Helpers.Normalize("<p>and *foo bar *</p>");
            Helpers.Log("Example {0}", 269);
            Helpers.Log("Section: {0}", "Inlines - Emphasis and strong emphasis");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "and *foo bar *");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example270()
        {
            // Example 270
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     and _foo bar _
            //
            // Should be rendered as:
            //     <p>and _foo bar _</p>

            // Arrange
            var commonMark = Helpers.Normalize("and _foo bar _");
            var expected = Helpers.Normalize("<p>and _foo bar _</p>");
            Helpers.Log("Example {0}", 270);
            Helpers.Log("Section: {0}", "Inlines - Emphasis and strong emphasis");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "and _foo bar _");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example271()
        {
            // Example 271
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     and **foo bar **
            //
            // Should be rendered as:
            //     <p>and **foo bar **</p>

            // Arrange
            var commonMark = Helpers.Normalize("and **foo bar **");
            var expected = Helpers.Normalize("<p>and **foo bar **</p>");
            Helpers.Log("Example {0}", 271);
            Helpers.Log("Section: {0}", "Inlines - Emphasis and strong emphasis");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "and **foo bar **");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example272()
        {
            // Example 272
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     and __foo bar __
            //
            // Should be rendered as:
            //     <p>and __foo bar __</p>

            // Arrange
            var commonMark = Helpers.Normalize("and __foo bar __");
            var expected = Helpers.Normalize("<p>and __foo bar __</p>");
            Helpers.Log("Example {0}", 272);
            Helpers.Log("Section: {0}", "Inlines - Emphasis and strong emphasis");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "and __foo bar __");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // The rules imply that a sequence of four or more unescaped `*` or
        // `_` characters will always be parsed as a literal string:
        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example273()
        {
            // Example 273
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     ****hi****
            //
            // Should be rendered as:
            //     <p>****hi****</p>

            // Arrange
            var commonMark = Helpers.Normalize("****hi****");
            var expected = Helpers.Normalize("<p>****hi****</p>");
            Helpers.Log("Example {0}", 273);
            Helpers.Log("Section: {0}", "Inlines - Emphasis and strong emphasis");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "****hi****");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example274()
        {
            // Example 274
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     _____hi_____
            //
            // Should be rendered as:
            //     <p>_____hi_____</p>

            // Arrange
            var commonMark = Helpers.Normalize("_____hi_____");
            var expected = Helpers.Normalize("<p>_____hi_____</p>");
            Helpers.Log("Example {0}", 274);
            Helpers.Log("Section: {0}", "Inlines - Emphasis and strong emphasis");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "_____hi_____");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example275()
        {
            // Example 275
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     Sign here: _________
            //
            // Should be rendered as:
            //     <p>Sign here: _________</p>

            // Arrange
            var commonMark = Helpers.Normalize("Sign here: _________");
            var expected = Helpers.Normalize("<p>Sign here: _________</p>");
            Helpers.Log("Example {0}", 275);
            Helpers.Log("Section: {0}", "Inlines - Emphasis and strong emphasis");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "Sign here: _________");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // The rules also imply that there can be no empty emphasis or strong
        // emphasis:
        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example276()
        {
            // Example 276
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     ** is not an empty emphasis
            //
            // Should be rendered as:
            //     <p>** is not an empty emphasis</p>

            // Arrange
            var commonMark = Helpers.Normalize("** is not an empty emphasis");
            var expected = Helpers.Normalize("<p>** is not an empty emphasis</p>");
            Helpers.Log("Example {0}", 276);
            Helpers.Log("Section: {0}", "Inlines - Emphasis and strong emphasis");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "** is not an empty emphasis");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example277()
        {
            // Example 277
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     **** is not an empty strong emphasis
            //
            // Should be rendered as:
            //     <p>**** is not an empty strong emphasis</p>

            // Arrange
            var commonMark = Helpers.Normalize("**** is not an empty strong emphasis");
            var expected = Helpers.Normalize("<p>**** is not an empty strong emphasis</p>");
            Helpers.Log("Example {0}", 277);
            Helpers.Log("Section: {0}", "Inlines - Emphasis and strong emphasis");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "**** is not an empty strong emphasis");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // To include `*` or `_` in emphasized sections, use backslash escapes
        // or code spans:
        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example278()
        {
            // Example 278
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     *here is a \**
            //
            // Should be rendered as:
            //     <p><em>here is a *</em></p>

            // Arrange
            var commonMark = Helpers.Normalize("*here is a \\**");
            var expected = Helpers.Normalize("<p><em>here is a *</em></p>");
            Helpers.Log("Example {0}", 278);
            Helpers.Log("Section: {0}", "Inlines - Emphasis and strong emphasis");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "*here is a \\**");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example279()
        {
            // Example 279
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     __this is a double underscore (`__`)__
            //
            // Should be rendered as:
            //     <p><strong>this is a double underscore (<code>__</code>)</strong></p>

            // Arrange
            var commonMark = Helpers.Normalize("__this is a double underscore (`__`)__");
            var expected = Helpers.Normalize("<p><strong>this is a double underscore (<code>__</code>)</strong></p>");
            Helpers.Log("Example {0}", 279);
            Helpers.Log("Section: {0}", "Inlines - Emphasis and strong emphasis");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "__this is a double underscore (`__`)__");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // `*` delimiters allow intra-word emphasis; `_` delimiters do not:
        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example280()
        {
            // Example 280
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     foo*bar*baz
            //
            // Should be rendered as:
            //     <p>foo<em>bar</em>baz</p>

            // Arrange
            var commonMark = Helpers.Normalize("foo*bar*baz");
            var expected = Helpers.Normalize("<p>foo<em>bar</em>baz</p>");
            Helpers.Log("Example {0}", 280);
            Helpers.Log("Section: {0}", "Inlines - Emphasis and strong emphasis");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "foo*bar*baz");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example281()
        {
            // Example 281
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     foo_bar_baz
            //
            // Should be rendered as:
            //     <p>foo_bar_baz</p>

            // Arrange
            var commonMark = Helpers.Normalize("foo_bar_baz");
            var expected = Helpers.Normalize("<p>foo_bar_baz</p>");
            Helpers.Log("Example {0}", 281);
            Helpers.Log("Section: {0}", "Inlines - Emphasis and strong emphasis");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "foo_bar_baz");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example282()
        {
            // Example 282
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     foo__bar__baz
            //
            // Should be rendered as:
            //     <p>foo__bar__baz</p>

            // Arrange
            var commonMark = Helpers.Normalize("foo__bar__baz");
            var expected = Helpers.Normalize("<p>foo__bar__baz</p>");
            Helpers.Log("Example {0}", 282);
            Helpers.Log("Section: {0}", "Inlines - Emphasis and strong emphasis");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "foo__bar__baz");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example283()
        {
            // Example 283
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     _foo_bar_baz_
            //
            // Should be rendered as:
            //     <p><em>foo_bar_baz</em></p>

            // Arrange
            var commonMark = Helpers.Normalize("_foo_bar_baz_");
            var expected = Helpers.Normalize("<p><em>foo_bar_baz</em></p>");
            Helpers.Log("Example {0}", 283);
            Helpers.Log("Section: {0}", "Inlines - Emphasis and strong emphasis");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "_foo_bar_baz_");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example284()
        {
            // Example 284
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     11*15*32
            //
            // Should be rendered as:
            //     <p>11<em>15</em>32</p>

            // Arrange
            var commonMark = Helpers.Normalize("11*15*32");
            var expected = Helpers.Normalize("<p>11<em>15</em>32</p>");
            Helpers.Log("Example {0}", 284);
            Helpers.Log("Section: {0}", "Inlines - Emphasis and strong emphasis");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "11*15*32");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example285()
        {
            // Example 285
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     11_15_32
            //
            // Should be rendered as:
            //     <p>11_15_32</p>

            // Arrange
            var commonMark = Helpers.Normalize("11_15_32");
            var expected = Helpers.Normalize("<p>11_15_32</p>");
            Helpers.Log("Example {0}", 285);
            Helpers.Log("Section: {0}", "Inlines - Emphasis and strong emphasis");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "11_15_32");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // Internal underscores will be ignored in underscore-delimited
        // emphasis:
        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example286()
        {
            // Example 286
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     _foo_bar_baz_
            //
            // Should be rendered as:
            //     <p><em>foo_bar_baz</em></p>

            // Arrange
            var commonMark = Helpers.Normalize("_foo_bar_baz_");
            var expected = Helpers.Normalize("<p><em>foo_bar_baz</em></p>");
            Helpers.Log("Example {0}", 286);
            Helpers.Log("Section: {0}", "Inlines - Emphasis and strong emphasis");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "_foo_bar_baz_");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example287()
        {
            // Example 287
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     __foo__bar__baz__
            //
            // Should be rendered as:
            //     <p><strong>foo__bar__baz</strong></p>

            // Arrange
            var commonMark = Helpers.Normalize("__foo__bar__baz__");
            var expected = Helpers.Normalize("<p><strong>foo__bar__baz</strong></p>");
            Helpers.Log("Example {0}", 287);
            Helpers.Log("Section: {0}", "Inlines - Emphasis and strong emphasis");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "__foo__bar__baz__");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // The rules are sufficient for the following nesting patterns:
        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example288()
        {
            // Example 288
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     ***foo bar***
            //
            // Should be rendered as:
            //     <p><strong><em>foo bar</em></strong></p>

            // Arrange
            var commonMark = Helpers.Normalize("***foo bar***");
            var expected = Helpers.Normalize("<p><strong><em>foo bar</em></strong></p>");
            Helpers.Log("Example {0}", 288);
            Helpers.Log("Section: {0}", "Inlines - Emphasis and strong emphasis");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "***foo bar***");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example289()
        {
            // Example 289
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     ___foo bar___
            //
            // Should be rendered as:
            //     <p><strong><em>foo bar</em></strong></p>

            // Arrange
            var commonMark = Helpers.Normalize("___foo bar___");
            var expected = Helpers.Normalize("<p><strong><em>foo bar</em></strong></p>");
            Helpers.Log("Example {0}", 289);
            Helpers.Log("Section: {0}", "Inlines - Emphasis and strong emphasis");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "___foo bar___");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example290()
        {
            // Example 290
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     ***foo** bar*
            //
            // Should be rendered as:
            //     <p><em><strong>foo</strong> bar</em></p>

            // Arrange
            var commonMark = Helpers.Normalize("***foo** bar*");
            var expected = Helpers.Normalize("<p><em><strong>foo</strong> bar</em></p>");
            Helpers.Log("Example {0}", 290);
            Helpers.Log("Section: {0}", "Inlines - Emphasis and strong emphasis");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "***foo** bar*");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example291()
        {
            // Example 291
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     ___foo__ bar_
            //
            // Should be rendered as:
            //     <p><em><strong>foo</strong> bar</em></p>

            // Arrange
            var commonMark = Helpers.Normalize("___foo__ bar_");
            var expected = Helpers.Normalize("<p><em><strong>foo</strong> bar</em></p>");
            Helpers.Log("Example {0}", 291);
            Helpers.Log("Section: {0}", "Inlines - Emphasis and strong emphasis");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "___foo__ bar_");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example292()
        {
            // Example 292
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     ***foo* bar**
            //
            // Should be rendered as:
            //     <p><strong><em>foo</em> bar</strong></p>

            // Arrange
            var commonMark = Helpers.Normalize("***foo* bar**");
            var expected = Helpers.Normalize("<p><strong><em>foo</em> bar</strong></p>");
            Helpers.Log("Example {0}", 292);
            Helpers.Log("Section: {0}", "Inlines - Emphasis and strong emphasis");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "***foo* bar**");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example293()
        {
            // Example 293
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     ___foo_ bar__
            //
            // Should be rendered as:
            //     <p><strong><em>foo</em> bar</strong></p>

            // Arrange
            var commonMark = Helpers.Normalize("___foo_ bar__");
            var expected = Helpers.Normalize("<p><strong><em>foo</em> bar</strong></p>");
            Helpers.Log("Example {0}", 293);
            Helpers.Log("Section: {0}", "Inlines - Emphasis and strong emphasis");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "___foo_ bar__");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example294()
        {
            // Example 294
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     *foo **bar***
            //
            // Should be rendered as:
            //     <p><em>foo <strong>bar</strong></em></p>

            // Arrange
            var commonMark = Helpers.Normalize("*foo **bar***");
            var expected = Helpers.Normalize("<p><em>foo <strong>bar</strong></em></p>");
            Helpers.Log("Example {0}", 294);
            Helpers.Log("Section: {0}", "Inlines - Emphasis and strong emphasis");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "*foo **bar***");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example295()
        {
            // Example 295
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     _foo __bar___
            //
            // Should be rendered as:
            //     <p><em>foo <strong>bar</strong></em></p>

            // Arrange
            var commonMark = Helpers.Normalize("_foo __bar___");
            var expected = Helpers.Normalize("<p><em>foo <strong>bar</strong></em></p>");
            Helpers.Log("Example {0}", 295);
            Helpers.Log("Section: {0}", "Inlines - Emphasis and strong emphasis");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "_foo __bar___");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example296()
        {
            // Example 296
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     **foo *bar***
            //
            // Should be rendered as:
            //     <p><strong>foo <em>bar</em></strong></p>

            // Arrange
            var commonMark = Helpers.Normalize("**foo *bar***");
            var expected = Helpers.Normalize("<p><strong>foo <em>bar</em></strong></p>");
            Helpers.Log("Example {0}", 296);
            Helpers.Log("Section: {0}", "Inlines - Emphasis and strong emphasis");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "**foo *bar***");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example297()
        {
            // Example 297
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     __foo _bar___
            //
            // Should be rendered as:
            //     <p><strong>foo <em>bar</em></strong></p>

            // Arrange
            var commonMark = Helpers.Normalize("__foo _bar___");
            var expected = Helpers.Normalize("<p><strong>foo <em>bar</em></strong></p>");
            Helpers.Log("Example {0}", 297);
            Helpers.Log("Section: {0}", "Inlines - Emphasis and strong emphasis");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "__foo _bar___");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example298()
        {
            // Example 298
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     *foo **bar***
            //
            // Should be rendered as:
            //     <p><em>foo <strong>bar</strong></em></p>

            // Arrange
            var commonMark = Helpers.Normalize("*foo **bar***");
            var expected = Helpers.Normalize("<p><em>foo <strong>bar</strong></em></p>");
            Helpers.Log("Example {0}", 298);
            Helpers.Log("Section: {0}", "Inlines - Emphasis and strong emphasis");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "*foo **bar***");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example299()
        {
            // Example 299
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     _foo __bar___
            //
            // Should be rendered as:
            //     <p><em>foo <strong>bar</strong></em></p>

            // Arrange
            var commonMark = Helpers.Normalize("_foo __bar___");
            var expected = Helpers.Normalize("<p><em>foo <strong>bar</strong></em></p>");
            Helpers.Log("Example {0}", 299);
            Helpers.Log("Section: {0}", "Inlines - Emphasis and strong emphasis");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "_foo __bar___");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example300()
        {
            // Example 300
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     *foo *bar* baz*
            //
            // Should be rendered as:
            //     <p><em>foo <em>bar</em> baz</em></p>

            // Arrange
            var commonMark = Helpers.Normalize("*foo *bar* baz*");
            var expected = Helpers.Normalize("<p><em>foo <em>bar</em> baz</em></p>");
            Helpers.Log("Example {0}", 300);
            Helpers.Log("Section: {0}", "Inlines - Emphasis and strong emphasis");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "*foo *bar* baz*");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example301()
        {
            // Example 301
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     _foo _bar_ baz_
            //
            // Should be rendered as:
            //     <p><em>foo <em>bar</em> baz</em></p>

            // Arrange
            var commonMark = Helpers.Normalize("_foo _bar_ baz_");
            var expected = Helpers.Normalize("<p><em>foo <em>bar</em> baz</em></p>");
            Helpers.Log("Example {0}", 301);
            Helpers.Log("Section: {0}", "Inlines - Emphasis and strong emphasis");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "_foo _bar_ baz_");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example302()
        {
            // Example 302
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     **foo **bar** baz**
            //
            // Should be rendered as:
            //     <p><strong>foo <strong>bar</strong> baz</strong></p>

            // Arrange
            var commonMark = Helpers.Normalize("**foo **bar** baz**");
            var expected = Helpers.Normalize("<p><strong>foo <strong>bar</strong> baz</strong></p>");
            Helpers.Log("Example {0}", 302);
            Helpers.Log("Section: {0}", "Inlines - Emphasis and strong emphasis");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "**foo **bar** baz**");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example303()
        {
            // Example 303
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     __foo __bar__ baz__
            //
            // Should be rendered as:
            //     <p><strong>foo <strong>bar</strong> baz</strong></p>

            // Arrange
            var commonMark = Helpers.Normalize("__foo __bar__ baz__");
            var expected = Helpers.Normalize("<p><strong>foo <strong>bar</strong> baz</strong></p>");
            Helpers.Log("Example {0}", 303);
            Helpers.Log("Section: {0}", "Inlines - Emphasis and strong emphasis");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "__foo __bar__ baz__");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example304()
        {
            // Example 304
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     *foo **bar** baz*
            //
            // Should be rendered as:
            //     <p><em>foo <strong>bar</strong> baz</em></p>

            // Arrange
            var commonMark = Helpers.Normalize("*foo **bar** baz*");
            var expected = Helpers.Normalize("<p><em>foo <strong>bar</strong> baz</em></p>");
            Helpers.Log("Example {0}", 304);
            Helpers.Log("Section: {0}", "Inlines - Emphasis and strong emphasis");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "*foo **bar** baz*");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example305()
        {
            // Example 305
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     _foo __bar__ baz_
            //
            // Should be rendered as:
            //     <p><em>foo <strong>bar</strong> baz</em></p>

            // Arrange
            var commonMark = Helpers.Normalize("_foo __bar__ baz_");
            var expected = Helpers.Normalize("<p><em>foo <strong>bar</strong> baz</em></p>");
            Helpers.Log("Example {0}", 305);
            Helpers.Log("Section: {0}", "Inlines - Emphasis and strong emphasis");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "_foo __bar__ baz_");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example306()
        {
            // Example 306
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     **foo *bar* baz**
            //
            // Should be rendered as:
            //     <p><strong>foo <em>bar</em> baz</strong></p>

            // Arrange
            var commonMark = Helpers.Normalize("**foo *bar* baz**");
            var expected = Helpers.Normalize("<p><strong>foo <em>bar</em> baz</strong></p>");
            Helpers.Log("Example {0}", 306);
            Helpers.Log("Section: {0}", "Inlines - Emphasis and strong emphasis");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "**foo *bar* baz**");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example307()
        {
            // Example 307
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     __foo _bar_ baz__
            //
            // Should be rendered as:
            //     <p><strong>foo <em>bar</em> baz</strong></p>

            // Arrange
            var commonMark = Helpers.Normalize("__foo _bar_ baz__");
            var expected = Helpers.Normalize("<p><strong>foo <em>bar</em> baz</strong></p>");
            Helpers.Log("Example {0}", 307);
            Helpers.Log("Section: {0}", "Inlines - Emphasis and strong emphasis");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "__foo _bar_ baz__");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // Note that you cannot nest emphasis directly inside emphasis
        // using the same delimeter, or strong emphasis directly inside
        // strong emphasis:
        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example308()
        {
            // Example 308
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     **foo**
            //
            // Should be rendered as:
            //     <p><strong>foo</strong></p>

            // Arrange
            var commonMark = Helpers.Normalize("**foo**");
            var expected = Helpers.Normalize("<p><strong>foo</strong></p>");
            Helpers.Log("Example {0}", 308);
            Helpers.Log("Section: {0}", "Inlines - Emphasis and strong emphasis");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "**foo**");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example309()
        {
            // Example 309
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     ****foo****
            //
            // Should be rendered as:
            //     <p>****foo****</p>

            // Arrange
            var commonMark = Helpers.Normalize("****foo****");
            var expected = Helpers.Normalize("<p>****foo****</p>");
            Helpers.Log("Example {0}", 309);
            Helpers.Log("Section: {0}", "Inlines - Emphasis and strong emphasis");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "****foo****");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // For these nestings, you need to switch delimiters:
        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example310()
        {
            // Example 310
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     *_foo_*
            //
            // Should be rendered as:
            //     <p><em><em>foo</em></em></p>

            // Arrange
            var commonMark = Helpers.Normalize("*_foo_*");
            var expected = Helpers.Normalize("<p><em><em>foo</em></em></p>");
            Helpers.Log("Example {0}", 310);
            Helpers.Log("Section: {0}", "Inlines - Emphasis and strong emphasis");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "*_foo_*");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example311()
        {
            // Example 311
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     **__foo__**
            //
            // Should be rendered as:
            //     <p><strong><strong>foo</strong></strong></p>

            // Arrange
            var commonMark = Helpers.Normalize("**__foo__**");
            var expected = Helpers.Normalize("<p><strong><strong>foo</strong></strong></p>");
            Helpers.Log("Example {0}", 311);
            Helpers.Log("Section: {0}", "Inlines - Emphasis and strong emphasis");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "**__foo__**");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // Note that a `*` followed by a `*` can close emphasis, and
        // a `**` followed by a `*` can close strong emphasis (and
        // similarly for `_` and `__`):
        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example312()
        {
            // Example 312
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     *foo**
            //
            // Should be rendered as:
            //     <p><em>foo</em>*</p>

            // Arrange
            var commonMark = Helpers.Normalize("*foo**");
            var expected = Helpers.Normalize("<p><em>foo</em>*</p>");
            Helpers.Log("Example {0}", 312);
            Helpers.Log("Section: {0}", "Inlines - Emphasis and strong emphasis");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "*foo**");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example313()
        {
            // Example 313
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     *foo *bar**
            //
            // Should be rendered as:
            //     <p><em>foo <em>bar</em></em></p>

            // Arrange
            var commonMark = Helpers.Normalize("*foo *bar**");
            var expected = Helpers.Normalize("<p><em>foo <em>bar</em></em></p>");
            Helpers.Log("Example {0}", 313);
            Helpers.Log("Section: {0}", "Inlines - Emphasis and strong emphasis");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "*foo *bar**");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example314()
        {
            // Example 314
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     **foo***
            //
            // Should be rendered as:
            //     <p><strong>foo</strong>*</p>

            // Arrange
            var commonMark = Helpers.Normalize("**foo***");
            var expected = Helpers.Normalize("<p><strong>foo</strong>*</p>");
            Helpers.Log("Example {0}", 314);
            Helpers.Log("Section: {0}", "Inlines - Emphasis and strong emphasis");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "**foo***");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example315()
        {
            // Example 315
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     ***foo* bar***
            //
            // Should be rendered as:
            //     <p><strong><em>foo</em> bar</strong>*</p>

            // Arrange
            var commonMark = Helpers.Normalize("***foo* bar***");
            var expected = Helpers.Normalize("<p><strong><em>foo</em> bar</strong>*</p>");
            Helpers.Log("Example {0}", 315);
            Helpers.Log("Section: {0}", "Inlines - Emphasis and strong emphasis");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "***foo* bar***");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example316()
        {
            // Example 316
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     ***foo** bar***
            //
            // Should be rendered as:
            //     <p><em><strong>foo</strong> bar</em>**</p>

            // Arrange
            var commonMark = Helpers.Normalize("***foo** bar***");
            var expected = Helpers.Normalize("<p><em><strong>foo</strong> bar</em>**</p>");
            Helpers.Log("Example {0}", 316);
            Helpers.Log("Section: {0}", "Inlines - Emphasis and strong emphasis");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "***foo** bar***");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // The following contains no strong emphasis, because the opening
        // delimiter is closed by the first `*` before `bar`:
        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example317()
        {
            // Example 317
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     *foo**bar***
            //
            // Should be rendered as:
            //     <p><em>foo</em><em>bar</em>**</p>

            // Arrange
            var commonMark = Helpers.Normalize("*foo**bar***");
            var expected = Helpers.Normalize("<p><em>foo</em><em>bar</em>**</p>");
            Helpers.Log("Example {0}", 317);
            Helpers.Log("Section: {0}", "Inlines - Emphasis and strong emphasis");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "*foo**bar***");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // However, a string of four or more `****` can never close emphasis:
        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example318()
        {
            // Example 318
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     *foo****
            //
            // Should be rendered as:
            //     <p>*foo****</p>

            // Arrange
            var commonMark = Helpers.Normalize("*foo****");
            var expected = Helpers.Normalize("<p>*foo****</p>");
            Helpers.Log("Example {0}", 318);
            Helpers.Log("Section: {0}", "Inlines - Emphasis and strong emphasis");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "*foo****");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // Note that there are some asymmetries here:
        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example319()
        {
            // Example 319
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     *foo**
            //     
            //     **foo*
            //
            // Should be rendered as:
            //     <p><em>foo</em>*</p>
            //     <p>**foo*</p>

            // Arrange
            var commonMark = Helpers.Normalize("*foo**\n\n**foo*");
            var expected = Helpers.Normalize("<p><em>foo</em>*</p>\n<p>**foo*</p>");
            Helpers.Log("Example {0}", 319);
            Helpers.Log("Section: {0}", "Inlines - Emphasis and strong emphasis");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "*foo**\n\n**foo*");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example320()
        {
            // Example 320
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     *foo *bar**
            //     
            //     **foo* bar*
            //
            // Should be rendered as:
            //     <p><em>foo <em>bar</em></em></p>
            //     <p>**foo* bar*</p>

            // Arrange
            var commonMark = Helpers.Normalize("*foo *bar**\n\n**foo* bar*");
            var expected = Helpers.Normalize("<p><em>foo <em>bar</em></em></p>\n<p>**foo* bar*</p>");
            Helpers.Log("Example {0}", 320);
            Helpers.Log("Section: {0}", "Inlines - Emphasis and strong emphasis");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "*foo *bar**\n\n**foo* bar*");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // More cases with mismatched delimiters:
        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example321()
        {
            // Example 321
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     **foo* bar*
            //
            // Should be rendered as:
            //     <p>**foo* bar*</p>

            // Arrange
            var commonMark = Helpers.Normalize("**foo* bar*");
            var expected = Helpers.Normalize("<p>**foo* bar*</p>");
            Helpers.Log("Example {0}", 321);
            Helpers.Log("Section: {0}", "Inlines - Emphasis and strong emphasis");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "**foo* bar*");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example322()
        {
            // Example 322
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     *bar***
            //
            // Should be rendered as:
            //     <p><em>bar</em>**</p>

            // Arrange
            var commonMark = Helpers.Normalize("*bar***");
            var expected = Helpers.Normalize("<p><em>bar</em>**</p>");
            Helpers.Log("Example {0}", 322);
            Helpers.Log("Section: {0}", "Inlines - Emphasis and strong emphasis");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "*bar***");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example323()
        {
            // Example 323
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     ***foo*
            //
            // Should be rendered as:
            //     <p>***foo*</p>

            // Arrange
            var commonMark = Helpers.Normalize("***foo*");
            var expected = Helpers.Normalize("<p>**<em>foo</em></p>");
            Helpers.Log("Example {0}", 323);
            Helpers.Log("Section: {0}", "Inlines - Emphasis and strong emphasis");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "***foo*");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example324()
        {
            // Example 324
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     **bar***
            //
            // Should be rendered as:
            //     <p><strong>bar</strong>*</p>

            // Arrange
            var commonMark = Helpers.Normalize("**bar***");
            var expected = Helpers.Normalize("<p><strong>bar</strong>*</p>");
            Helpers.Log("Example {0}", 324);
            Helpers.Log("Section: {0}", "Inlines - Emphasis and strong emphasis");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "**bar***");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example325()
        {
            // Example 325
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     ***foo**
            //
            // Should be rendered as:
            //     <p>***foo**</p>

            // Arrange
            var commonMark = Helpers.Normalize("***foo**");
            var expected = Helpers.Normalize("<p>*<strong>foo</strong></p>");
            Helpers.Log("Example {0}", 325);
            Helpers.Log("Section: {0}", "Inlines - Emphasis and strong emphasis");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "***foo**");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example326()
        {
            // Example 326
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     ***foo *bar*
            //
            // Should be rendered as:
            //     <p>***foo <em>bar</em></p>

            // Arrange
            var commonMark = Helpers.Normalize("***foo *bar*");
            var expected = Helpers.Normalize("<p>***foo <em>bar</em></p>");
            Helpers.Log("Example {0}", 326);
            Helpers.Log("Section: {0}", "Inlines - Emphasis and strong emphasis");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "***foo *bar*");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // ## Links
        //
        // A link contains a [link label](#link-label) (the visible text),
        // a [destination](#destination) (the URI that is the link destination),
        // and optionally a [link title](#link-title).  There are two basic kinds
        // of links in Markdown.  In [inline links](#inline-links) the destination
        // and title are given immediately after the label.  In [reference
        // links](#reference-links) the destination and title are defined elsewhere
        // in the document.
        //
        // A [link label](#link-label) <a id="link-label"></a>  consists of
        //
        // - an opening `[`, followed by
        // - zero or more backtick code spans, autolinks, HTML tags, link labels,
        // backslash-escaped ASCII punctuation characters, or non-`]` characters,
        // followed by
        // - a closing `]`.
        //
        // These rules are motivated by the following intuitive ideas:
        //
        // - A link label is a container for inline elements.
        // - The square brackets bind more tightly than emphasis markers,
        // but less tightly than `<>` or `` ` ``.
        // - Link labels may contain material in matching square brackets.
        //
        // A [link destination](#link-destination) <a id="link-destination"></a>
        // consists of either
        //
        // - a sequence of zero or more characters between an opening `<` and a
        // closing `>` that contains no line breaks or unescaped `<` or `>`
        // characters, or
        //
        // - a nonempty sequence of characters that does not include
        // ASCII space or control characters, and includes parentheses
        // only if (a) they are backslash-escaped or (b) they are part of
        // a balanced pair of unescaped parentheses that is not itself
        // inside a balanced pair of unescaped paretheses.
        //
        // A [link title](#link-title) <a id="link-title"></a>  consists of either
        //
        // - a sequence of zero or more characters between straight double-quote
        // characters (`"`), including a `"` character only if it is
        // backslash-escaped, or
        //
        // - a sequence of zero or more characters between straight single-quote
        // characters (`'`), including a `'` character only if it is
        // backslash-escaped, or
        //
        // - a sequence of zero or more characters between matching parentheses
        // (`(...)`), including a `)` character only if it is backslash-escaped.
        //
        // An [inline link](#inline-link) <a id="inline-link"></a>
        // consists of a [link label](#link-label) followed immediately
        // by a left parenthesis `(`, optional whitespace,
        // an optional [link destination](#link-destination),
        // an optional [link title](#link-title) separated from the link
        // destination by whitespace, optional whitespace, and a right
        // parenthesis `)`.  The link's text consists of the label (excluding
        // the enclosing square brackets) parsed as inlines.  The link's
        // URI consists of the link destination, excluding enclosing `<...>` if
        // present, with backslash-escapes in effect as described above.  The
        // link's title consists of the link title, excluding its enclosing
        // delimiters, with backslash-escapes in effect as described above.
        //
        // Here is a simple inline link:
        [TestMethod]
        [TestCategory("Inlines - Links")]
        //[Timeout(1000)]
        public void Example327()
        {
            // Example 327
            // Section: Inlines - Links
            //
            // The following CommonMark:
            //     [link](/uri "title")
            //
            // Should be rendered as:
            //     <p><a href="/uri" title="title">link</a></p>

            // Arrange
            var commonMark = Helpers.Normalize("[link](/uri \"title\")");
            var expected = Helpers.Normalize("<p><a href=\"/uri\" title=\"title\">link</a></p>");
            Helpers.Log("Example {0}", 327);
            Helpers.Log("Section: {0}", "Inlines - Links");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "[link](/uri \"title\")");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // The title may be omitted:
        [TestMethod]
        [TestCategory("Inlines - Links")]
        //[Timeout(1000)]
        public void Example328()
        {
            // Example 328
            // Section: Inlines - Links
            //
            // The following CommonMark:
            //     [link](/uri)
            //
            // Should be rendered as:
            //     <p><a href="/uri">link</a></p>

            // Arrange
            var commonMark = Helpers.Normalize("[link](/uri)");
            var expected = Helpers.Normalize("<p><a href=\"/uri\">link</a></p>");
            Helpers.Log("Example {0}", 328);
            Helpers.Log("Section: {0}", "Inlines - Links");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "[link](/uri)");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // Both the title and the destination may be omitted:
        [TestMethod]
        [TestCategory("Inlines - Links")]
        //[Timeout(1000)]
        public void Example329()
        {
            // Example 329
            // Section: Inlines - Links
            //
            // The following CommonMark:
            //     [link]()
            //
            // Should be rendered as:
            //     <p><a href="">link</a></p>

            // Arrange
            var commonMark = Helpers.Normalize("[link]()");
            var expected = Helpers.Normalize("<p><a href=\"\">link</a></p>");
            Helpers.Log("Example {0}", 329);
            Helpers.Log("Section: {0}", "Inlines - Links");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "[link]()");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        [TestMethod]
        [TestCategory("Inlines - Links")]
        //[Timeout(1000)]
        public void Example330()
        {
            // Example 330
            // Section: Inlines - Links
            //
            // The following CommonMark:
            //     [link](<>)
            //
            // Should be rendered as:
            //     <p><a href="">link</a></p>

            // Arrange
            var commonMark = Helpers.Normalize("[link](<>)");
            var expected = Helpers.Normalize("<p><a href=\"\">link</a></p>");
            Helpers.Log("Example {0}", 330);
            Helpers.Log("Section: {0}", "Inlines - Links");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "[link](<>)");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // If the destination contains spaces, it must be enclosed in pointy
        // braces:
        [TestMethod]
        [TestCategory("Inlines - Links")]
        //[Timeout(1000)]
        public void Example331()
        {
            // Example 331
            // Section: Inlines - Links
            //
            // The following CommonMark:
            //     [link](/my uri)
            //
            // Should be rendered as:
            //     <p>[link](/my uri)</p>

            // Arrange
            var commonMark = Helpers.Normalize("[link](/my uri)");
            var expected = Helpers.Normalize("<p>[link](/my uri)</p>");
            Helpers.Log("Example {0}", 331);
            Helpers.Log("Section: {0}", "Inlines - Links");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "[link](/my uri)");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        [TestMethod]
        [TestCategory("Inlines - Links")]
        //[Timeout(1000)]
        public void Example332()
        {
            // Example 332
            // Section: Inlines - Links
            //
            // The following CommonMark:
            //     [link](</my uri>)
            //
            // Should be rendered as:
            //     <p><a href="/my uri">link</a></p>

            // Arrange
            var commonMark = Helpers.Normalize("[link](</my uri>)");
            var expected = Helpers.Normalize("<p><a href=\"/my uri\">link</a></p>");
            Helpers.Log("Example {0}", 332);
            Helpers.Log("Section: {0}", "Inlines - Links");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "[link](</my uri>)");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // The destination cannot contain line breaks, even with pointy braces:
        [TestMethod]
        [TestCategory("Inlines - Links")]
        //[Timeout(1000)]
        public void Example333()
        {
            // Example 333
            // Section: Inlines - Links
            //
            // The following CommonMark:
            //     [link](foo
            //     bar)
            //
            // Should be rendered as:
            //     <p>[link](foo
            //     bar)</p>

            // Arrange
            var commonMark = Helpers.Normalize("[link](foo\nbar)");
            var expected = Helpers.Normalize("<p>[link](foo\nbar)</p>");
            Helpers.Log("Example {0}", 333);
            Helpers.Log("Section: {0}", "Inlines - Links");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "[link](foo\nbar)");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // One level of balanced parentheses is allowed without escaping:
        [TestMethod]
        [TestCategory("Inlines - Links")]
        //[Timeout(1000)]
        public void Example334()
        {
            // Example 334
            // Section: Inlines - Links
            //
            // The following CommonMark:
            //     [link]((foo)and(bar))
            //
            // Should be rendered as:
            //     <p><a href="(foo)and(bar)">link</a></p>

            // Arrange
            var commonMark = Helpers.Normalize("[link]((foo)and(bar))");
            var expected = Helpers.Normalize("<p><a href=\"(foo)and(bar)\">link</a></p>");
            Helpers.Log("Example {0}", 334);
            Helpers.Log("Section: {0}", "Inlines - Links");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "[link]((foo)and(bar))");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // However, if you have parentheses within parentheses, you need to escape
        // or use the `<...>` form:
        [TestMethod]
        [TestCategory("Inlines - Links")]
        //[Timeout(1000)]
        public void Example335()
        {
            // Example 335
            // Section: Inlines - Links
            //
            // The following CommonMark:
            //     [link](foo(and(bar)))
            //
            // Should be rendered as:
            //     <p>[link](foo(and(bar)))</p>

            // Arrange
            var commonMark = Helpers.Normalize("[link](foo(and(bar)))");
            var expected = Helpers.Normalize("<p>[link](foo(and(bar)))</p>");
            Helpers.Log("Example {0}", 335);
            Helpers.Log("Section: {0}", "Inlines - Links");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "[link](foo(and(bar)))");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        [TestMethod]
        [TestCategory("Inlines - Links")]
        //[Timeout(1000)]
        public void Example336()
        {
            // Example 336
            // Section: Inlines - Links
            //
            // The following CommonMark:
            //     [link](foo(and\(bar\)))
            //
            // Should be rendered as:
            //     <p><a href="foo(and(bar))">link</a></p>

            // Arrange
            var commonMark = Helpers.Normalize("[link](foo(and\\(bar\\)))");
            var expected = Helpers.Normalize("<p><a href=\"foo(and(bar))\">link</a></p>");
            Helpers.Log("Example {0}", 336);
            Helpers.Log("Section: {0}", "Inlines - Links");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "[link](foo(and\\(bar\\)))");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        [TestMethod]
        [TestCategory("Inlines - Links")]
        //[Timeout(1000)]
        public void Example337()
        {
            // Example 337
            // Section: Inlines - Links
            //
            // The following CommonMark:
            //     [link](<foo(and(bar))>)
            //
            // Should be rendered as:
            //     <p><a href="foo(and(bar))">link</a></p>

            // Arrange
            var commonMark = Helpers.Normalize("[link](<foo(and(bar))>)");
            var expected = Helpers.Normalize("<p><a href=\"foo(and(bar))\">link</a></p>");
            Helpers.Log("Example {0}", 337);
            Helpers.Log("Section: {0}", "Inlines - Links");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "[link](<foo(and(bar))>)");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // Parentheses and other symbols can also be escaped, as usual
        // in Markdown:
        [TestMethod]
        [TestCategory("Inlines - Links")]
        //[Timeout(1000)]
        public void Example338()
        {
            // Example 338
            // Section: Inlines - Links
            //
            // The following CommonMark:
            //     [link](foo\)\:)
            //
            // Should be rendered as:
            //     <p><a href="foo):">link</a></p>

            // Arrange
            var commonMark = Helpers.Normalize("[link](foo\\)\\:)");
            var expected = Helpers.Normalize("<p><a href=\"foo):\">link</a></p>");
            Helpers.Log("Example {0}", 338);
            Helpers.Log("Section: {0}", "Inlines - Links");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "[link](foo\\)\\:)");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // URL-escaping and entities should be left alone inside the destination:
        [TestMethod]
        [TestCategory("Inlines - Links")]
        //[Timeout(1000)]
        public void Example339()
        {
            // Example 339
            // Section: Inlines - Links
            //
            // The following CommonMark:
            //     [link](foo%20b&auml;)
            //
            // Should be rendered as:
            //     <p><a href="foo%20b&auml;">link</a></p>

            // Arrange
            var commonMark = Helpers.Normalize("[link](foo%20b&auml;)");
            var expected = Helpers.Normalize("<p><a href=\"foo%20b&auml;\">link</a></p>");
            Helpers.Log("Example {0}", 339);
            Helpers.Log("Section: {0}", "Inlines - Links");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "[link](foo%20b&auml;)");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // Note that, because titles can often be parsed as destinations,
        // if you try to omit the destination and keep the title, you'll
        // get unexpected results:
        [TestMethod]
        [TestCategory("Inlines - Links")]
        //[Timeout(1000)]
        public void Example340()
        {
            // Example 340
            // Section: Inlines - Links
            //
            // The following CommonMark:
            //     [link]("title")
            //
            // Should be rendered as:
            //     <p><a href="&quot;title&quot;">link</a></p>

            // Arrange
            var commonMark = Helpers.Normalize("[link](\"title\")");
            var expected = Helpers.Normalize("<p><a href=\"&quot;title&quot;\">link</a></p>");
            Helpers.Log("Example {0}", 340);
            Helpers.Log("Section: {0}", "Inlines - Links");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "[link](\"title\")");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // Titles may be in single quotes, double quotes, or parentheses:
        [TestMethod]
        [TestCategory("Inlines - Links")]
        //[Timeout(1000)]
        public void Example341()
        {
            // Example 341
            // Section: Inlines - Links
            //
            // The following CommonMark:
            //     [link](/url "title")
            //     [link](/url 'title')
            //     [link](/url (title))
            //
            // Should be rendered as:
            //     <p><a href="/url" title="title">link</a>
            //     <a href="/url" title="title">link</a>
            //     <a href="/url" title="title">link</a></p>

            // Arrange
            var commonMark = Helpers.Normalize("[link](/url \"title\")\n[link](/url 'title')\n[link](/url (title))");
            var expected = Helpers.Normalize("<p><a href=\"/url\" title=\"title\">link</a>\n<a href=\"/url\" title=\"title\">link</a>\n<a href=\"/url\" title=\"title\">link</a></p>");
            Helpers.Log("Example {0}", 341);
            Helpers.Log("Section: {0}", "Inlines - Links");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "[link](/url \"title\")\n[link](/url 'title')\n[link](/url (title))");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // Backslash escapes and entities may be used in titles:
        [TestMethod]
        [TestCategory("Inlines - Links")]
        //[Timeout(1000)]
        public void Example342()
        {
            // Example 342
            // Section: Inlines - Links
            //
            // The following CommonMark:
            //     [link](/url "title \"&quot;")
            //
            // Should be rendered as:
            //     <p><a href="/url" title="title &quot;&quot;">link</a></p>

            // Arrange
            var commonMark = Helpers.Normalize("[link](/url \"title \\\"&quot;\")");
            var expected = Helpers.Normalize("<p><a href=\"/url\" title=\"title &quot;&quot;\">link</a></p>");
            Helpers.Log("Example {0}", 342);
            Helpers.Log("Section: {0}", "Inlines - Links");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "[link](/url \"title \\\"&quot;\")");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // Nested balanced quotes are not allowed without escaping:
        [TestMethod]
        [TestCategory("Inlines - Links")]
        //[Timeout(1000)]
        public void Example343()
        {
            // Example 343
            // Section: Inlines - Links
            //
            // The following CommonMark:
            //     [link](/url "title "and" title")
            //
            // Should be rendered as:
            //     <p>[link](/url &quot;title &quot;and&quot; title&quot;)</p>

            // Arrange
            var commonMark = Helpers.Normalize("[link](/url \"title \"and\" title\")");
            var expected = Helpers.Normalize("<p>[link](/url &quot;title &quot;and&quot; title&quot;)</p>");
            Helpers.Log("Example {0}", 343);
            Helpers.Log("Section: {0}", "Inlines - Links");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "[link](/url \"title \"and\" title\")");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // But it is easy to work around this by using a different quote type:
        [TestMethod]
        [TestCategory("Inlines - Links")]
        //[Timeout(1000)]
        public void Example344()
        {
            // Example 344
            // Section: Inlines - Links
            //
            // The following CommonMark:
            //     [link](/url 'title "and" title')
            //
            // Should be rendered as:
            //     <p><a href="/url" title="title &quot;and&quot; title">link</a></p>

            // Arrange
            var commonMark = Helpers.Normalize("[link](/url 'title \"and\" title')");
            var expected = Helpers.Normalize("<p><a href=\"/url\" title=\"title &quot;and&quot; title\">link</a></p>");
            Helpers.Log("Example {0}", 344);
            Helpers.Log("Section: {0}", "Inlines - Links");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "[link](/url 'title \"and\" title')");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // (Note:  `Markdown.pl` did allow double quotes inside a double-quoted
        // title, and its test suite included a test demonstrating this.
        // But it is hard to see a good rationale for the extra complexity this
        // brings, since there are already many ways---backslash escaping,
        // entities, or using a different quote type for the enclosing title---to
        // write titles containing double quotes.  `Markdown.pl`'s handling of
        // titles has a number of other strange features.  For example, it allows
        // single-quoted titles in inline links, but not reference links.  And, in
        // reference links but not inline links, it allows a title to begin with
        // `"` and end with `)`.  `Markdown.pl` 1.0.1 even allows titles with no closing
        // quotation mark, though 1.0.2b8 does not.  It seems preferable to adopt
        // a simple, rational rule that works the same way in inline links and
        // link reference definitions.)
        //
        // Whitespace is allowed around the destination and title:
        [TestMethod]
        [TestCategory("Inlines - Links")]
        //[Timeout(1000)]
        public void Example345()
        {
            // Example 345
            // Section: Inlines - Links
            //
            // The following CommonMark:
            //     [link](   /uri
            //       "title"  )
            //
            // Should be rendered as:
            //     <p><a href="/uri" title="title">link</a></p>

            // Arrange
            var commonMark = Helpers.Normalize("[link](   /uri\n  \"title\"  )");
            var expected = Helpers.Normalize("<p><a href=\"/uri\" title=\"title\">link</a></p>");
            Helpers.Log("Example {0}", 345);
            Helpers.Log("Section: {0}", "Inlines - Links");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "[link](   /uri\n  \"title\"  )");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // But it is not allowed between the link label and the
        // following parenthesis:
        [TestMethod]
        [TestCategory("Inlines - Links")]
        //[Timeout(1000)]
        public void Example346()
        {
            // Example 346
            // Section: Inlines - Links
            //
            // The following CommonMark:
            //     [link] (/uri)
            //
            // Should be rendered as:
            //     <p>[link] (/uri)</p>

            // Arrange
            var commonMark = Helpers.Normalize("[link] (/uri)");
            var expected = Helpers.Normalize("<p>[link] (/uri)</p>");
            Helpers.Log("Example {0}", 346);
            Helpers.Log("Section: {0}", "Inlines - Links");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "[link] (/uri)");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // Note that this is not a link, because the closing `]` occurs in
        // an HTML tag:
        [TestMethod]
        [TestCategory("Inlines - Links")]
        //[Timeout(1000)]
        public void Example347()
        {
            // Example 347
            // Section: Inlines - Links
            //
            // The following CommonMark:
            //     [foo <bar attr="](baz)">
            //
            // Should be rendered as:
            //     <p>[foo <bar attr="](baz)"></p>

            // Arrange
            var commonMark = Helpers.Normalize("[foo <bar attr=\"](baz)\">");
            var expected = Helpers.Normalize("<p>[foo <bar attr=\"](baz)\"></p>");
            Helpers.Log("Example {0}", 347);
            Helpers.Log("Section: {0}", "Inlines - Links");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "[foo <bar attr=\"](baz)\">");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // There are three kinds of [reference links](#reference-link):
        // <a id="reference-link"></a>
        //
        // A [full reference link](#full-reference-link) <a id="full-reference-link"></a>
        // consists of a [link label](#link-label), optional whitespace, and
        // another [link label](#link-label) that [matches](#matches) a
        // [link reference definition](#link-reference-definition) elsewhere in the
        // document.
        //
        // One label [matches](#matches) <a id="matches"></a>
        // another just in case their normalized forms are equal.  To normalize a
        // label, perform the *unicode case fold* and collapse consecutive internal
        // whitespace to a single space.  If there are multiple matching reference
        // link definitions, the one that comes first in the document is used.  (It
        // is desirable in such cases to emit a warning.)
        //
        // The contents of the first link label are parsed as inlines, which are
        // used as the link's text.  The link's URI and title are provided by the
        // matching [link reference definition](#link-reference-definition).
        //
        // Here is a simple example:
        [TestMethod]
        [TestCategory("Inlines - Links")]
        //[Timeout(1000)]
        public void Example348()
        {
            // Example 348
            // Section: Inlines - Links
            //
            // The following CommonMark:
            //     [foo][bar]
            //     
            //     [bar]: /url "title"
            //
            // Should be rendered as:
            //     <p><a href="/url" title="title">foo</a></p>

            // Arrange
            var commonMark = Helpers.Normalize("[foo][bar]\n\n[bar]: /url \"title\"");
            var expected = Helpers.Normalize("<p><a href=\"/url\" title=\"title\">foo</a></p>");
            Helpers.Log("Example {0}", 348);
            Helpers.Log("Section: {0}", "Inlines - Links");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "[foo][bar]\n\n[bar]: /url \"title\"");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // The first label can contain inline content:
        [TestMethod]
        [TestCategory("Inlines - Links")]
        //[Timeout(1000)]
        public void Example349()
        {
            // Example 349
            // Section: Inlines - Links
            //
            // The following CommonMark:
            //     [*foo\!*][bar]
            //     
            //     [bar]: /url "title"
            //
            // Should be rendered as:
            //     <p><a href="/url" title="title"><em>foo!</em></a></p>

            // Arrange
            var commonMark = Helpers.Normalize("[*foo\\!*][bar]\n\n[bar]: /url \"title\"");
            var expected = Helpers.Normalize("<p><a href=\"/url\" title=\"title\"><em>foo!</em></a></p>");
            Helpers.Log("Example {0}", 349);
            Helpers.Log("Section: {0}", "Inlines - Links");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "[*foo\\!*][bar]\n\n[bar]: /url \"title\"");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // Matching is case-insensitive:
        [TestMethod]
        [TestCategory("Inlines - Links")]
        //[Timeout(1000)]
        public void Example350()
        {
            // Example 350
            // Section: Inlines - Links
            //
            // The following CommonMark:
            //     [foo][BaR]
            //     
            //     [bar]: /url "title"
            //
            // Should be rendered as:
            //     <p><a href="/url" title="title">foo</a></p>

            // Arrange
            var commonMark = Helpers.Normalize("[foo][BaR]\n\n[bar]: /url \"title\"");
            var expected = Helpers.Normalize("<p><a href=\"/url\" title=\"title\">foo</a></p>");
            Helpers.Log("Example {0}", 350);
            Helpers.Log("Section: {0}", "Inlines - Links");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "[foo][BaR]\n\n[bar]: /url \"title\"");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // Unicode case fold is used:
        [TestMethod]
        [TestCategory("Inlines - Links")]
        //[Timeout(1000)]
        public void Example351()
        {
            // Example 351
            // Section: Inlines - Links
            //
            // The following CommonMark:
            //     [Толпой][Толпой] is a Russian word.
            //     
            //     [ТОЛПОЙ]: /url
            //
            // Should be rendered as:
            //     <p><a href="/url">Толпой</a> is a Russian word.</p>

            // Arrange
            var commonMark = Helpers.Normalize("[Толпой][Толпой] is a Russian word.\n\n[ТОЛПОЙ]: /url");
            var expected = Helpers.Normalize("<p><a href=\"/url\">Толпой</a> is a Russian word.</p>");
            Helpers.Log("Example {0}", 351);
            Helpers.Log("Section: {0}", "Inlines - Links");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "[Толпой][Толпой] is a Russian word.\n\n[ТОЛПОЙ]: /url");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // Consecutive internal whitespace is treated as one space for
        // purposes of determining matching:
        [TestMethod]
        [TestCategory("Inlines - Links")]
        //[Timeout(1000)]
        public void Example352()
        {
            // Example 352
            // Section: Inlines - Links
            //
            // The following CommonMark:
            //     [Foo
            //       bar]: /url
            //     
            //     [Baz][Foo bar]
            //
            // Should be rendered as:
            //     <p><a href="/url">Baz</a></p>

            // Arrange
            var commonMark = Helpers.Normalize("[Foo\n  bar]: /url\n\n[Baz][Foo bar]");
            var expected = Helpers.Normalize("<p><a href=\"/url\">Baz</a></p>");
            Helpers.Log("Example {0}", 352);
            Helpers.Log("Section: {0}", "Inlines - Links");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "[Foo\n  bar]: /url\n\n[Baz][Foo bar]");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // There can be whitespace between the two labels:
        [TestMethod]
        [TestCategory("Inlines - Links")]
        //[Timeout(1000)]
        public void Example353()
        {
            // Example 353
            // Section: Inlines - Links
            //
            // The following CommonMark:
            //     [foo] [bar]
            //     
            //     [bar]: /url "title"
            //
            // Should be rendered as:
            //     <p><a href="/url" title="title">foo</a></p>

            // Arrange
            var commonMark = Helpers.Normalize("[foo] [bar]\n\n[bar]: /url \"title\"");
            var expected = Helpers.Normalize("<p><a href=\"/url\" title=\"title\">foo</a></p>");
            Helpers.Log("Example {0}", 353);
            Helpers.Log("Section: {0}", "Inlines - Links");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "[foo] [bar]\n\n[bar]: /url \"title\"");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        [TestMethod]
        [TestCategory("Inlines - Links")]
        //[Timeout(1000)]
        public void Example354()
        {
            // Example 354
            // Section: Inlines - Links
            //
            // The following CommonMark:
            //     [foo]
            //     [bar]
            //     
            //     [bar]: /url "title"
            //
            // Should be rendered as:
            //     <p><a href="/url" title="title">foo</a></p>

            // Arrange
            var commonMark = Helpers.Normalize("[foo]\n[bar]\n\n[bar]: /url \"title\"");
            var expected = Helpers.Normalize("<p><a href=\"/url\" title=\"title\">foo</a></p>");
            Helpers.Log("Example {0}", 354);
            Helpers.Log("Section: {0}", "Inlines - Links");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "[foo]\n[bar]\n\n[bar]: /url \"title\"");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // When there are multiple matching [link reference
        // definitions](#link-reference-definition), the first is used:
        [TestMethod]
        [TestCategory("Inlines - Links")]
        //[Timeout(1000)]
        public void Example355()
        {
            // Example 355
            // Section: Inlines - Links
            //
            // The following CommonMark:
            //     [foo]: /url1
            //     
            //     [foo]: /url2
            //     
            //     [bar][foo]
            //
            // Should be rendered as:
            //     <p><a href="/url1">bar</a></p>

            // Arrange
            var commonMark = Helpers.Normalize("[foo]: /url1\n\n[foo]: /url2\n\n[bar][foo]");
            var expected = Helpers.Normalize("<p><a href=\"/url1\">bar</a></p>");
            Helpers.Log("Example {0}", 355);
            Helpers.Log("Section: {0}", "Inlines - Links");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "[foo]: /url1\n\n[foo]: /url2\n\n[bar][foo]");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // Note that matching is performed on normalized strings, not parsed
        // inline content.  So the following does not match, even though the
        // labels define equivalent inline content:
        [TestMethod]
        [TestCategory("Inlines - Links")]
        //[Timeout(1000)]
        public void Example356()
        {
            // Example 356
            // Section: Inlines - Links
            //
            // The following CommonMark:
            //     [bar][foo\!]
            //     
            //     [foo!]: /url
            //
            // Should be rendered as:
            //     <p>[bar][foo!]</p>

            // Arrange
            var commonMark = Helpers.Normalize("[bar][foo\\!]\n\n[foo!]: /url");
            var expected = Helpers.Normalize("<p>[bar][foo!]</p>");
            Helpers.Log("Example {0}", 356);
            Helpers.Log("Section: {0}", "Inlines - Links");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "[bar][foo\\!]\n\n[foo!]: /url");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // A [collapsed reference link](#collapsed-reference-link)
        // <a id="collapsed-reference-link"></a> consists of a [link
        // label](#link-label) that [matches](#matches) a [link reference
        // definition](#link-reference-definition) elsewhere in the
        // document, optional whitespace, and the string `[]`.  The contents of the
        // first link label are parsed as inlines, which are used as the link's
        // text.  The link's URI and title are provided by the matching reference
        // link definition.  Thus, `[foo][]` is equivalent to `[foo][foo]`.
        [TestMethod]
        [TestCategory("Inlines - Links")]
        //[Timeout(1000)]
        public void Example357()
        {
            // Example 357
            // Section: Inlines - Links
            //
            // The following CommonMark:
            //     [foo][]
            //     
            //     [foo]: /url "title"
            //
            // Should be rendered as:
            //     <p><a href="/url" title="title">foo</a></p>

            // Arrange
            var commonMark = Helpers.Normalize("[foo][]\n\n[foo]: /url \"title\"");
            var expected = Helpers.Normalize("<p><a href=\"/url\" title=\"title\">foo</a></p>");
            Helpers.Log("Example {0}", 357);
            Helpers.Log("Section: {0}", "Inlines - Links");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "[foo][]\n\n[foo]: /url \"title\"");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        [TestMethod]
        [TestCategory("Inlines - Links")]
        //[Timeout(1000)]
        public void Example358()
        {
            // Example 358
            // Section: Inlines - Links
            //
            // The following CommonMark:
            //     [*foo* bar][]
            //     
            //     [*foo* bar]: /url "title"
            //
            // Should be rendered as:
            //     <p><a href="/url" title="title"><em>foo</em> bar</a></p>

            // Arrange
            var commonMark = Helpers.Normalize("[*foo* bar][]\n\n[*foo* bar]: /url \"title\"");
            var expected = Helpers.Normalize("<p><a href=\"/url\" title=\"title\"><em>foo</em> bar</a></p>");
            Helpers.Log("Example {0}", 358);
            Helpers.Log("Section: {0}", "Inlines - Links");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "[*foo* bar][]\n\n[*foo* bar]: /url \"title\"");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // The link labels are case-insensitive:
        [TestMethod]
        [TestCategory("Inlines - Links")]
        //[Timeout(1000)]
        public void Example359()
        {
            // Example 359
            // Section: Inlines - Links
            //
            // The following CommonMark:
            //     [Foo][]
            //     
            //     [foo]: /url "title"
            //
            // Should be rendered as:
            //     <p><a href="/url" title="title">Foo</a></p>

            // Arrange
            var commonMark = Helpers.Normalize("[Foo][]\n\n[foo]: /url \"title\"");
            var expected = Helpers.Normalize("<p><a href=\"/url\" title=\"title\">Foo</a></p>");
            Helpers.Log("Example {0}", 359);
            Helpers.Log("Section: {0}", "Inlines - Links");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "[Foo][]\n\n[foo]: /url \"title\"");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // As with full reference links, whitespace is allowed
        // between the two sets of brackets:
        [TestMethod]
        [TestCategory("Inlines - Links")]
        //[Timeout(1000)]
        public void Example360()
        {
            // Example 360
            // Section: Inlines - Links
            //
            // The following CommonMark:
            //     [foo] 
            //     []
            //     
            //     [foo]: /url "title"
            //
            // Should be rendered as:
            //     <p><a href="/url" title="title">foo</a></p>

            // Arrange
            var commonMark = Helpers.Normalize("[foo] \n[]\n\n[foo]: /url \"title\"");
            var expected = Helpers.Normalize("<p><a href=\"/url\" title=\"title\">foo</a></p>");
            Helpers.Log("Example {0}", 360);
            Helpers.Log("Section: {0}", "Inlines - Links");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "[foo] \n[]\n\n[foo]: /url \"title\"");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // A [shortcut reference link](#shortcut-reference-link)
        // <a id="shortcut-reference-link"></a> consists of a [link
        // label](#link-label) that [matches](#matches) a [link reference
        // definition](#link-reference-definition)  elsewhere in the
        // document and is not followed by `[]` or a link label.
        // The contents of the first link label are parsed as inlines,
        // which are used as the link's text.  the link's URI and title
        // are provided by the matching link reference definition.
        // Thus, `[foo]` is equivalent to `[foo][]`.
        [TestMethod]
        [TestCategory("Inlines - Links")]
        //[Timeout(1000)]
        public void Example361()
        {
            // Example 361
            // Section: Inlines - Links
            //
            // The following CommonMark:
            //     [foo]
            //     
            //     [foo]: /url "title"
            //
            // Should be rendered as:
            //     <p><a href="/url" title="title">foo</a></p>

            // Arrange
            var commonMark = Helpers.Normalize("[foo]\n\n[foo]: /url \"title\"");
            var expected = Helpers.Normalize("<p><a href=\"/url\" title=\"title\">foo</a></p>");
            Helpers.Log("Example {0}", 361);
            Helpers.Log("Section: {0}", "Inlines - Links");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "[foo]\n\n[foo]: /url \"title\"");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        [TestMethod]
        [TestCategory("Inlines - Links")]
        //[Timeout(1000)]
        public void Example362()
        {
            // Example 362
            // Section: Inlines - Links
            //
            // The following CommonMark:
            //     [*foo* bar]
            //     
            //     [*foo* bar]: /url "title"
            //
            // Should be rendered as:
            //     <p><a href="/url" title="title"><em>foo</em> bar</a></p>

            // Arrange
            var commonMark = Helpers.Normalize("[*foo* bar]\n\n[*foo* bar]: /url \"title\"");
            var expected = Helpers.Normalize("<p><a href=\"/url\" title=\"title\"><em>foo</em> bar</a></p>");
            Helpers.Log("Example {0}", 362);
            Helpers.Log("Section: {0}", "Inlines - Links");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "[*foo* bar]\n\n[*foo* bar]: /url \"title\"");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        [TestMethod]
        [TestCategory("Inlines - Links")]
        //[Timeout(1000)]
        public void Example363()
        {
            // Example 363
            // Section: Inlines - Links
            //
            // The following CommonMark:
            //     [[*foo* bar]]
            //     
            //     [*foo* bar]: /url "title"
            //
            // Should be rendered as:
            //     <p>[<a href="/url" title="title"><em>foo</em> bar</a>]</p>

            // Arrange
            var commonMark = Helpers.Normalize("[[*foo* bar]]\n\n[*foo* bar]: /url \"title\"");
            var expected = Helpers.Normalize("<p>[<a href=\"/url\" title=\"title\"><em>foo</em> bar</a>]</p>");
            Helpers.Log("Example {0}", 363);
            Helpers.Log("Section: {0}", "Inlines - Links");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "[[*foo* bar]]\n\n[*foo* bar]: /url \"title\"");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // The link labels are case-insensitive:
        [TestMethod]
        [TestCategory("Inlines - Links")]
        //[Timeout(1000)]
        public void Example364()
        {
            // Example 364
            // Section: Inlines - Links
            //
            // The following CommonMark:
            //     [Foo]
            //     
            //     [foo]: /url "title"
            //
            // Should be rendered as:
            //     <p><a href="/url" title="title">Foo</a></p>

            // Arrange
            var commonMark = Helpers.Normalize("[Foo]\n\n[foo]: /url \"title\"");
            var expected = Helpers.Normalize("<p><a href=\"/url\" title=\"title\">Foo</a></p>");
            Helpers.Log("Example {0}", 364);
            Helpers.Log("Section: {0}", "Inlines - Links");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "[Foo]\n\n[foo]: /url \"title\"");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // If you just want bracketed text, you can backslash-escape the
        // opening bracket to avoid links:
        [TestMethod]
        [TestCategory("Inlines - Links")]
        //[Timeout(1000)]
        public void Example365()
        {
            // Example 365
            // Section: Inlines - Links
            //
            // The following CommonMark:
            //     \[foo]
            //     
            //     [foo]: /url "title"
            //
            // Should be rendered as:
            //     <p>[foo]</p>

            // Arrange
            var commonMark = Helpers.Normalize("\\[foo]\n\n[foo]: /url \"title\"");
            var expected = Helpers.Normalize("<p>[foo]</p>");
            Helpers.Log("Example {0}", 365);
            Helpers.Log("Section: {0}", "Inlines - Links");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "\\[foo]\n\n[foo]: /url \"title\"");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // Note that this is a link, because link labels bind more tightly
        // than emphasis:
        [TestMethod]
        [TestCategory("Inlines - Links")]
        //[Timeout(1000)]
        public void Example366()
        {
            // Example 366
            // Section: Inlines - Links
            //
            // The following CommonMark:
            //     [foo*]: /url
            //     
            //     *[foo*]
            //
            // Should be rendered as:
            //     <p>*<a href="/url">foo*</a></p>

            // Arrange
            var commonMark = Helpers.Normalize("[foo*]: /url\n\n*[foo*]");
            var expected = Helpers.Normalize("<p>*<a href=\"/url\">foo*</a></p>");
            Helpers.Log("Example {0}", 366);
            Helpers.Log("Section: {0}", "Inlines - Links");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "[foo*]: /url\n\n*[foo*]");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // However, this is not, because link labels bind less
        // tightly than code backticks:
        [TestMethod]
        [TestCategory("Inlines - Links")]
        //[Timeout(1000)]
        public void Example367()
        {
            // Example 367
            // Section: Inlines - Links
            //
            // The following CommonMark:
            //     [foo`]: /url
            //     
            //     [foo`]`
            //
            // Should be rendered as:
            //     <p>[foo<code>]</code></p>

            // Arrange
            var commonMark = Helpers.Normalize("[foo`]: /url\n\n[foo`]`");
            var expected = Helpers.Normalize("<p>[foo<code>]</code></p>");
            Helpers.Log("Example {0}", 367);
            Helpers.Log("Section: {0}", "Inlines - Links");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "[foo`]: /url\n\n[foo`]`");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // Link labels can contain matched square brackets:
        [TestMethod]
        [TestCategory("Inlines - Links")]
        //[Timeout(1000)]
        public void Example368()
        {
            // Example 368
            // Section: Inlines - Links
            //
            // The following CommonMark:
            //     [[[foo]]]
            //     
            //     [[[foo]]]: /url
            //
            // Should be rendered as:
            //     <p><a href="/url">[[foo]]</a></p>

            // Arrange
            var commonMark = Helpers.Normalize("[[[foo]]]\n\n[[[foo]]]: /url");
            var expected = Helpers.Normalize("<p><a href=\"/url\">[[foo]]</a></p>");
            Helpers.Log("Example {0}", 368);
            Helpers.Log("Section: {0}", "Inlines - Links");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "[[[foo]]]\n\n[[[foo]]]: /url");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        [TestMethod]
        [TestCategory("Inlines - Links")]
        //[Timeout(1000)]
        public void Example369()
        {
            // Example 369
            // Section: Inlines - Links
            //
            // The following CommonMark:
            //     [[[foo]]]
            //     
            //     [[[foo]]]: /url1
            //     [foo]: /url2
            //
            // Should be rendered as:
            //     <p><a href="/url1">[[foo]]</a></p>

            // Arrange
            var commonMark = Helpers.Normalize("[[[foo]]]\n\n[[[foo]]]: /url1\n[foo]: /url2");
            var expected = Helpers.Normalize("<p><a href=\"/url1\">[[foo]]</a></p>");
            Helpers.Log("Example {0}", 369);
            Helpers.Log("Section: {0}", "Inlines - Links");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "[[[foo]]]\n\n[[[foo]]]: /url1\n[foo]: /url2");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // For non-matching brackets, use backslash escapes:
        [TestMethod]
        [TestCategory("Inlines - Links")]
        //[Timeout(1000)]
        public void Example370()
        {
            // Example 370
            // Section: Inlines - Links
            //
            // The following CommonMark:
            //     [\[foo]
            //     
            //     [\[foo]: /url
            //
            // Should be rendered as:
            //     <p><a href="/url">[foo</a></p>

            // Arrange
            var commonMark = Helpers.Normalize("[\\[foo]\n\n[\\[foo]: /url");
            var expected = Helpers.Normalize("<p><a href=\"/url\">[foo</a></p>");
            Helpers.Log("Example {0}", 370);
            Helpers.Log("Section: {0}", "Inlines - Links");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "[\\[foo]\n\n[\\[foo]: /url");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // Full references take precedence over shortcut references:
        [TestMethod]
        [TestCategory("Inlines - Links")]
        //[Timeout(1000)]
        public void Example371()
        {
            // Example 371
            // Section: Inlines - Links
            //
            // The following CommonMark:
            //     [foo][bar]
            //     
            //     [foo]: /url1
            //     [bar]: /url2
            //
            // Should be rendered as:
            //     <p><a href="/url2">foo</a></p>

            // Arrange
            var commonMark = Helpers.Normalize("[foo][bar]\n\n[foo]: /url1\n[bar]: /url2");
            var expected = Helpers.Normalize("<p><a href=\"/url2\">foo</a></p>");
            Helpers.Log("Example {0}", 371);
            Helpers.Log("Section: {0}", "Inlines - Links");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "[foo][bar]\n\n[foo]: /url1\n[bar]: /url2");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // In the following case `[bar][baz]` is parsed as a reference,
        // `[foo]` as normal text:
        [TestMethod]
        [TestCategory("Inlines - Links")]
        //[Timeout(1000)]
        public void Example372()
        {
            // Example 372
            // Section: Inlines - Links
            //
            // The following CommonMark:
            //     [foo][bar][baz]
            //     
            //     [baz]: /url
            //
            // Should be rendered as:
            //     <p>[foo]<a href="/url">bar</a></p>

            // Arrange
            var commonMark = Helpers.Normalize("[foo][bar][baz]\n\n[baz]: /url");
            var expected = Helpers.Normalize("<p>[foo]<a href=\"/url\">bar</a></p>");
            Helpers.Log("Example {0}", 372);
            Helpers.Log("Section: {0}", "Inlines - Links");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "[foo][bar][baz]\n\n[baz]: /url");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // Here, though, `[foo][bar]` is parsed as a reference, since
        // `[bar]` is defined:
        [TestMethod]
        [TestCategory("Inlines - Links")]
        //[Timeout(1000)]
        public void Example373()
        {
            // Example 373
            // Section: Inlines - Links
            //
            // The following CommonMark:
            //     [foo][bar][baz]
            //     
            //     [baz]: /url1
            //     [bar]: /url2
            //
            // Should be rendered as:
            //     <p><a href="/url2">foo</a><a href="/url1">baz</a></p>

            // Arrange
            var commonMark = Helpers.Normalize("[foo][bar][baz]\n\n[baz]: /url1\n[bar]: /url2");
            var expected = Helpers.Normalize("<p><a href=\"/url2\">foo</a><a href=\"/url1\">baz</a></p>");
            Helpers.Log("Example {0}", 373);
            Helpers.Log("Section: {0}", "Inlines - Links");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "[foo][bar][baz]\n\n[baz]: /url1\n[bar]: /url2");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // Here `[foo]` is not parsed as a shortcut reference, because it
        // is followed by a link label (even though `[bar]` is not defined):
        [TestMethod]
        [TestCategory("Inlines - Links")]
        //[Timeout(1000)]
        public void Example374()
        {
            // Example 374
            // Section: Inlines - Links
            //
            // The following CommonMark:
            //     [foo][bar][baz]
            //     
            //     [baz]: /url1
            //     [foo]: /url2
            //
            // Should be rendered as:
            //     <p>[foo]<a href="/url1">bar</a></p>

            // Arrange
            var commonMark = Helpers.Normalize("[foo][bar][baz]\n\n[baz]: /url1\n[foo]: /url2");
            var expected = Helpers.Normalize("<p>[foo]<a href=\"/url1\">bar</a></p>");
            Helpers.Log("Example {0}", 374);
            Helpers.Log("Section: {0}", "Inlines - Links");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "[foo][bar][baz]\n\n[baz]: /url1\n[foo]: /url2");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // ## Images
        //
        // An (unescaped) exclamation mark (`!`) followed by a reference or
        // inline link will be parsed as an image.  The link label will be
        // used as the image's alt text, and the link title, if any, will
        // be used as the image's title.
        [TestMethod]
        [TestCategory("Inlines - Images")]
        //[Timeout(1000)]
        public void Example375()
        {
            // Example 375
            // Section: Inlines - Images
            //
            // The following CommonMark:
            //     ![foo](/url "title")
            //
            // Should be rendered as:
            //     <p><img src="/url" alt="foo" title="title" /></p>

            // Arrange
            var commonMark = Helpers.Normalize("![foo](/url \"title\")");
            var expected = Helpers.Normalize("<p><img src=\"/url\" alt=\"foo\" title=\"title\" /></p>");
            Helpers.Log("Example {0}", 375);
            Helpers.Log("Section: {0}", "Inlines - Images");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "![foo](/url \"title\")");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        [TestMethod]
        [TestCategory("Inlines - Images")]
        //[Timeout(1000)]
        public void Example376()
        {
            // Example 376
            // Section: Inlines - Images
            //
            // The following CommonMark:
            //     ![foo *bar*]
            //     
            //     [foo *bar*]: train.jpg "train & tracks"
            //
            // Should be rendered as:
            //     <p><img src="train.jpg" alt="foo &lt;em&gt;bar&lt;/em&gt;" title="train &amp; tracks" /></p>

            // Arrange
            var commonMark = Helpers.Normalize("![foo *bar*]\n\n[foo *bar*]: train.jpg \"train & tracks\"");
            var expected = Helpers.Normalize("<p><img src=\"train.jpg\" alt=\"foo &lt;em&gt;bar&lt;/em&gt;\" title=\"train &amp; tracks\" /></p>");
            Helpers.Log("Example {0}", 376);
            Helpers.Log("Section: {0}", "Inlines - Images");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "![foo *bar*]\n\n[foo *bar*]: train.jpg \"train & tracks\"");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        [TestMethod]
        [TestCategory("Inlines - Images")]
        //[Timeout(1000)]
        public void Example377()
        {
            // Example 377
            // Section: Inlines - Images
            //
            // The following CommonMark:
            //     ![foo *bar*][]
            //     
            //     [foo *bar*]: train.jpg "train & tracks"
            //
            // Should be rendered as:
            //     <p><img src="train.jpg" alt="foo &lt;em&gt;bar&lt;/em&gt;" title="train &amp; tracks" /></p>

            // Arrange
            var commonMark = Helpers.Normalize("![foo *bar*][]\n\n[foo *bar*]: train.jpg \"train & tracks\"");
            var expected = Helpers.Normalize("<p><img src=\"train.jpg\" alt=\"foo &lt;em&gt;bar&lt;/em&gt;\" title=\"train &amp; tracks\" /></p>");
            Helpers.Log("Example {0}", 377);
            Helpers.Log("Section: {0}", "Inlines - Images");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "![foo *bar*][]\n\n[foo *bar*]: train.jpg \"train & tracks\"");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        [TestMethod]
        [TestCategory("Inlines - Images")]
        //[Timeout(1000)]
        public void Example378()
        {
            // Example 378
            // Section: Inlines - Images
            //
            // The following CommonMark:
            //     ![foo *bar*][foobar]
            //     
            //     [FOOBAR]: train.jpg "train & tracks"
            //
            // Should be rendered as:
            //     <p><img src="train.jpg" alt="foo &lt;em&gt;bar&lt;/em&gt;" title="train &amp; tracks" /></p>

            // Arrange
            var commonMark = Helpers.Normalize("![foo *bar*][foobar]\n\n[FOOBAR]: train.jpg \"train & tracks\"");
            var expected = Helpers.Normalize("<p><img src=\"train.jpg\" alt=\"foo &lt;em&gt;bar&lt;/em&gt;\" title=\"train &amp; tracks\" /></p>");
            Helpers.Log("Example {0}", 378);
            Helpers.Log("Section: {0}", "Inlines - Images");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "![foo *bar*][foobar]\n\n[FOOBAR]: train.jpg \"train & tracks\"");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        [TestMethod]
        [TestCategory("Inlines - Images")]
        //[Timeout(1000)]
        public void Example379()
        {
            // Example 379
            // Section: Inlines - Images
            //
            // The following CommonMark:
            //     ![foo](train.jpg)
            //
            // Should be rendered as:
            //     <p><img src="train.jpg" alt="foo" /></p>

            // Arrange
            var commonMark = Helpers.Normalize("![foo](train.jpg)");
            var expected = Helpers.Normalize("<p><img src=\"train.jpg\" alt=\"foo\" /></p>");
            Helpers.Log("Example {0}", 379);
            Helpers.Log("Section: {0}", "Inlines - Images");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "![foo](train.jpg)");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        [TestMethod]
        [TestCategory("Inlines - Images")]
        //[Timeout(1000)]
        public void Example380()
        {
            // Example 380
            // Section: Inlines - Images
            //
            // The following CommonMark:
            //     My ![foo bar](/path/to/train.jpg  "title"   )
            //
            // Should be rendered as:
            //     <p>My <img src="/path/to/train.jpg" alt="foo bar" title="title" /></p>

            // Arrange
            var commonMark = Helpers.Normalize("My ![foo bar](/path/to/train.jpg  \"title\"   )");
            var expected = Helpers.Normalize("<p>My <img src=\"/path/to/train.jpg\" alt=\"foo bar\" title=\"title\" /></p>");
            Helpers.Log("Example {0}", 380);
            Helpers.Log("Section: {0}", "Inlines - Images");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "My ![foo bar](/path/to/train.jpg  \"title\"   )");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        [TestMethod]
        [TestCategory("Inlines - Images")]
        //[Timeout(1000)]
        public void Example381()
        {
            // Example 381
            // Section: Inlines - Images
            //
            // The following CommonMark:
            //     ![foo](<url>)
            //
            // Should be rendered as:
            //     <p><img src="url" alt="foo" /></p>

            // Arrange
            var commonMark = Helpers.Normalize("![foo](<url>)");
            var expected = Helpers.Normalize("<p><img src=\"url\" alt=\"foo\" /></p>");
            Helpers.Log("Example {0}", 381);
            Helpers.Log("Section: {0}", "Inlines - Images");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "![foo](<url>)");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        [TestMethod]
        [TestCategory("Inlines - Images")]
        //[Timeout(1000)]
        public void Example382()
        {
            // Example 382
            // Section: Inlines - Images
            //
            // The following CommonMark:
            //     ![](/url)
            //
            // Should be rendered as:
            //     <p><img src="/url" alt="" /></p>

            // Arrange
            var commonMark = Helpers.Normalize("![](/url)");
            var expected = Helpers.Normalize("<p><img src=\"/url\" alt=\"\" /></p>");
            Helpers.Log("Example {0}", 382);
            Helpers.Log("Section: {0}", "Inlines - Images");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "![](/url)");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // Reference-style:
        [TestMethod]
        [TestCategory("Inlines - Images")]
        //[Timeout(1000)]
        public void Example383()
        {
            // Example 383
            // Section: Inlines - Images
            //
            // The following CommonMark:
            //     ![foo] [bar]
            //     
            //     [bar]: /url
            //
            // Should be rendered as:
            //     <p><img src="/url" alt="foo" /></p>

            // Arrange
            var commonMark = Helpers.Normalize("![foo] [bar]\n\n[bar]: /url");
            var expected = Helpers.Normalize("<p><img src=\"/url\" alt=\"foo\" /></p>");
            Helpers.Log("Example {0}", 383);
            Helpers.Log("Section: {0}", "Inlines - Images");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "![foo] [bar]\n\n[bar]: /url");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        [TestMethod]
        [TestCategory("Inlines - Images")]
        //[Timeout(1000)]
        public void Example384()
        {
            // Example 384
            // Section: Inlines - Images
            //
            // The following CommonMark:
            //     ![foo] [bar]
            //     
            //     [BAR]: /url
            //
            // Should be rendered as:
            //     <p><img src="/url" alt="foo" /></p>

            // Arrange
            var commonMark = Helpers.Normalize("![foo] [bar]\n\n[BAR]: /url");
            var expected = Helpers.Normalize("<p><img src=\"/url\" alt=\"foo\" /></p>");
            Helpers.Log("Example {0}", 384);
            Helpers.Log("Section: {0}", "Inlines - Images");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "![foo] [bar]\n\n[BAR]: /url");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // Collapsed:
        [TestMethod]
        [TestCategory("Inlines - Images")]
        //[Timeout(1000)]
        public void Example385()
        {
            // Example 385
            // Section: Inlines - Images
            //
            // The following CommonMark:
            //     ![foo][]
            //     
            //     [foo]: /url "title"
            //
            // Should be rendered as:
            //     <p><img src="/url" alt="foo" title="title" /></p>

            // Arrange
            var commonMark = Helpers.Normalize("![foo][]\n\n[foo]: /url \"title\"");
            var expected = Helpers.Normalize("<p><img src=\"/url\" alt=\"foo\" title=\"title\" /></p>");
            Helpers.Log("Example {0}", 385);
            Helpers.Log("Section: {0}", "Inlines - Images");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "![foo][]\n\n[foo]: /url \"title\"");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        [TestMethod]
        [TestCategory("Inlines - Images")]
        //[Timeout(1000)]
        public void Example386()
        {
            // Example 386
            // Section: Inlines - Images
            //
            // The following CommonMark:
            //     ![*foo* bar][]
            //     
            //     [*foo* bar]: /url "title"
            //
            // Should be rendered as:
            //     <p><img src="/url" alt="&lt;em&gt;foo&lt;/em&gt; bar" title="title" /></p>

            // Arrange
            var commonMark = Helpers.Normalize("![*foo* bar][]\n\n[*foo* bar]: /url \"title\"");
            var expected = Helpers.Normalize("<p><img src=\"/url\" alt=\"&lt;em&gt;foo&lt;/em&gt; bar\" title=\"title\" /></p>");
            Helpers.Log("Example {0}", 386);
            Helpers.Log("Section: {0}", "Inlines - Images");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "![*foo* bar][]\n\n[*foo* bar]: /url \"title\"");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // The labels are case-insensitive:
        [TestMethod]
        [TestCategory("Inlines - Images")]
        //[Timeout(1000)]
        public void Example387()
        {
            // Example 387
            // Section: Inlines - Images
            //
            // The following CommonMark:
            //     ![Foo][]
            //     
            //     [foo]: /url "title"
            //
            // Should be rendered as:
            //     <p><img src="/url" alt="Foo" title="title" /></p>

            // Arrange
            var commonMark = Helpers.Normalize("![Foo][]\n\n[foo]: /url \"title\"");
            var expected = Helpers.Normalize("<p><img src=\"/url\" alt=\"Foo\" title=\"title\" /></p>");
            Helpers.Log("Example {0}", 387);
            Helpers.Log("Section: {0}", "Inlines - Images");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "![Foo][]\n\n[foo]: /url \"title\"");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // As with full reference links, whitespace is allowed
        // between the two sets of brackets:
        [TestMethod]
        [TestCategory("Inlines - Images")]
        //[Timeout(1000)]
        public void Example388()
        {
            // Example 388
            // Section: Inlines - Images
            //
            // The following CommonMark:
            //     ![foo] 
            //     []
            //     
            //     [foo]: /url "title"
            //
            // Should be rendered as:
            //     <p><img src="/url" alt="foo" title="title" /></p>

            // Arrange
            var commonMark = Helpers.Normalize("![foo] \n[]\n\n[foo]: /url \"title\"");
            var expected = Helpers.Normalize("<p><img src=\"/url\" alt=\"foo\" title=\"title\" /></p>");
            Helpers.Log("Example {0}", 388);
            Helpers.Log("Section: {0}", "Inlines - Images");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "![foo] \n[]\n\n[foo]: /url \"title\"");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // Shortcut:
        [TestMethod]
        [TestCategory("Inlines - Images")]
        //[Timeout(1000)]
        public void Example389()
        {
            // Example 389
            // Section: Inlines - Images
            //
            // The following CommonMark:
            //     ![foo]
            //     
            //     [foo]: /url "title"
            //
            // Should be rendered as:
            //     <p><img src="/url" alt="foo" title="title" /></p>

            // Arrange
            var commonMark = Helpers.Normalize("![foo]\n\n[foo]: /url \"title\"");
            var expected = Helpers.Normalize("<p><img src=\"/url\" alt=\"foo\" title=\"title\" /></p>");
            Helpers.Log("Example {0}", 389);
            Helpers.Log("Section: {0}", "Inlines - Images");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "![foo]\n\n[foo]: /url \"title\"");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        [TestMethod]
        [TestCategory("Inlines - Images")]
        //[Timeout(1000)]
        public void Example390()
        {
            // Example 390
            // Section: Inlines - Images
            //
            // The following CommonMark:
            //     ![*foo* bar]
            //     
            //     [*foo* bar]: /url "title"
            //
            // Should be rendered as:
            //     <p><img src="/url" alt="&lt;em&gt;foo&lt;/em&gt; bar" title="title" /></p>

            // Arrange
            var commonMark = Helpers.Normalize("![*foo* bar]\n\n[*foo* bar]: /url \"title\"");
            var expected = Helpers.Normalize("<p><img src=\"/url\" alt=\"&lt;em&gt;foo&lt;/em&gt; bar\" title=\"title\" /></p>");
            Helpers.Log("Example {0}", 390);
            Helpers.Log("Section: {0}", "Inlines - Images");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "![*foo* bar]\n\n[*foo* bar]: /url \"title\"");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        [TestMethod]
        [TestCategory("Inlines - Images")]
        //[Timeout(1000)]
        public void Example391()
        {
            // Example 391
            // Section: Inlines - Images
            //
            // The following CommonMark:
            //     ![[foo]]
            //     
            //     [[foo]]: /url "title"
            //
            // Should be rendered as:
            //     <p><img src="/url" alt="[foo]" title="title" /></p>

            // Arrange
            var commonMark = Helpers.Normalize("![[foo]]\n\n[[foo]]: /url \"title\"");
            var expected = Helpers.Normalize("<p><img src=\"/url\" alt=\"[foo]\" title=\"title\" /></p>");
            Helpers.Log("Example {0}", 391);
            Helpers.Log("Section: {0}", "Inlines - Images");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "![[foo]]\n\n[[foo]]: /url \"title\"");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // The link labels are case-insensitive:
        [TestMethod]
        [TestCategory("Inlines - Images")]
        //[Timeout(1000)]
        public void Example392()
        {
            // Example 392
            // Section: Inlines - Images
            //
            // The following CommonMark:
            //     ![Foo]
            //     
            //     [foo]: /url "title"
            //
            // Should be rendered as:
            //     <p><img src="/url" alt="Foo" title="title" /></p>

            // Arrange
            var commonMark = Helpers.Normalize("![Foo]\n\n[foo]: /url \"title\"");
            var expected = Helpers.Normalize("<p><img src=\"/url\" alt=\"Foo\" title=\"title\" /></p>");
            Helpers.Log("Example {0}", 392);
            Helpers.Log("Section: {0}", "Inlines - Images");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "![Foo]\n\n[foo]: /url \"title\"");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // If you just want bracketed text, you can backslash-escape the
        // opening `!` and `[`:
        [TestMethod]
        [TestCategory("Inlines - Images")]
        //[Timeout(1000)]
        public void Example393()
        {
            // Example 393
            // Section: Inlines - Images
            //
            // The following CommonMark:
            //     \!\[foo]
            //     
            //     [foo]: /url "title"
            //
            // Should be rendered as:
            //     <p>![foo]</p>

            // Arrange
            var commonMark = Helpers.Normalize("\\!\\[foo]\n\n[foo]: /url \"title\"");
            var expected = Helpers.Normalize("<p>![foo]</p>");
            Helpers.Log("Example {0}", 393);
            Helpers.Log("Section: {0}", "Inlines - Images");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "\\!\\[foo]\n\n[foo]: /url \"title\"");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // If you want a link after a literal `!`, backslash-escape the
        // `!`:
        [TestMethod]
        [TestCategory("Inlines - Images")]
        //[Timeout(1000)]
        public void Example394()
        {
            // Example 394
            // Section: Inlines - Images
            //
            // The following CommonMark:
            //     \![foo]
            //     
            //     [foo]: /url "title"
            //
            // Should be rendered as:
            //     <p>!<a href="/url" title="title">foo</a></p>

            // Arrange
            var commonMark = Helpers.Normalize("\\![foo]\n\n[foo]: /url \"title\"");
            var expected = Helpers.Normalize("<p>!<a href=\"/url\" title=\"title\">foo</a></p>");
            Helpers.Log("Example {0}", 394);
            Helpers.Log("Section: {0}", "Inlines - Images");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "\\![foo]\n\n[foo]: /url \"title\"");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // ## Autolinks
        //
        // Autolinks are absolute URIs and email addresses inside `<` and `>`.
        // They are parsed as links, with the URL or email address as the link
        // label.
        //
        // A [URI autolink](#uri-autolink) <a id="uri-autolink"></a>
        // consists of `<`, followed by an [absolute
        // URI](#absolute-uri) not containing `<`, followed by `>`.  It is parsed
        // as a link to the URI, with the URI as the link's label.
        //
        // An [absolute URI](#absolute-uri), <a id="absolute-uri"></a>
        // for these purposes, consists of a [scheme](#scheme) followed by a colon (`:`)
        // followed by zero or more characters other than ASCII whitespace and
        // control characters, `<`, and `>`.  If the URI includes these characters,
        // you must use percent-encoding (e.g. `%20` for a space).
        //
        // The following [schemes](#scheme) <a id="scheme"></a>
        // are recognized (case-insensitive):
        // `coap`, `doi`, `javascript`, `aaa`, `aaas`, `about`, `acap`, `cap`,
        // `cid`, `crid`, `data`, `dav`, `dict`, `dns`, `file`, `ftp`, `geo`, `go`,
        // `gopher`, `h323`, `http`, `https`, `iax`, `icap`, `im`, `imap`, `info`,
        // `ipp`, `iris`, `iris.beep`, `iris.xpc`, `iris.xpcs`, `iris.lwz`, `ldap`,
        // `mailto`, `mid`, `msrp`, `msrps`, `mtqp`, `mupdate`, `news`, `nfs`,
        // `ni`, `nih`, `nntp`, `opaquelocktoken`, `pop`, `pres`, `rtsp`,
        // `service`, `session`, `shttp`, `sieve`, `sip`, `sips`, `sms`, `snmp`,`
        // soap.beep`, `soap.beeps`, `tag`, `tel`, `telnet`, `tftp`, `thismessage`,
        // `tn3270`, `tip`, `tv`, `urn`, `vemmi`, `ws`, `wss`, `xcon`,
        // `xcon-userid`, `xmlrpc.beep`, `xmlrpc.beeps`, `xmpp`, `z39.50r`,
        // `z39.50s`, `adiumxtra`, `afp`, `afs`, `aim`, `apt`,` attachment`, `aw`,
        // `beshare`, `bitcoin`, `bolo`, `callto`, `chrome`,` chrome-extension`,
        // `com-eventbrite-attendee`, `content`, `cvs`,` dlna-playsingle`,
        // `dlna-playcontainer`, `dtn`, `dvb`, `ed2k`, `facetime`, `feed`,
        // `finger`, `fish`, `gg`, `git`, `gizmoproject`, `gtalk`, `hcp`, `icon`,
        // `ipn`, `irc`, `irc6`, `ircs`, `itms`, `jar`, `jms`, `keyparc`, `lastfm`,
        // `ldaps`, `magnet`, `maps`, `market`,` message`, `mms`, `ms-help`,
        // `msnim`, `mumble`, `mvn`, `notes`, `oid`, `palm`, `paparazzi`,
        // `platform`, `proxy`, `psyc`, `query`, `res`, `resource`, `rmi`, `rsync`,
        // `rtmp`, `secondlife`, `sftp`, `sgn`, `skype`, `smb`, `soldat`,
        // `spotify`, `ssh`, `steam`, `svn`, `teamspeak`, `things`, `udp`,
        // `unreal`, `ut2004`, `ventrilo`, `view-source`, `webcal`, `wtai`,
        // `wyciwyg`, `xfire`, `xri`, `ymsgr`.
        //
        // Here are some valid autolinks:
        [TestMethod]
        [TestCategory("Inlines - Autolinks")]
        //[Timeout(1000)]
        public void Example395()
        {
            // Example 395
            // Section: Inlines - Autolinks
            //
            // The following CommonMark:
            //     <http://foo.bar.baz>
            //
            // Should be rendered as:
            //     <p><a href="http://foo.bar.baz">http://foo.bar.baz</a></p>

            // Arrange
            var commonMark = Helpers.Normalize("<http://foo.bar.baz>");
            var expected = Helpers.Normalize("<p><a href=\"http://foo.bar.baz\">http://foo.bar.baz</a></p>");
            Helpers.Log("Example {0}", 395);
            Helpers.Log("Section: {0}", "Inlines - Autolinks");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "<http://foo.bar.baz>");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        [TestMethod]
        [TestCategory("Inlines - Autolinks")]
        //[Timeout(1000)]
        public void Example396()
        {
            // Example 396
            // Section: Inlines - Autolinks
            //
            // The following CommonMark:
            //     <http://foo.bar.baz?q=hello&id=22&boolean>
            //
            // Should be rendered as:
            //     <p><a href="http://foo.bar.baz?q=hello&amp;id=22&amp;boolean">http://foo.bar.baz?q=hello&amp;id=22&amp;boolean</a></p>

            // Arrange
            var commonMark = Helpers.Normalize("<http://foo.bar.baz?q=hello&id=22&boolean>");
            var expected = Helpers.Normalize("<p><a href=\"http://foo.bar.baz?q=hello&amp;id=22&amp;boolean\">http://foo.bar.baz?q=hello&amp;id=22&amp;boolean</a></p>");
            Helpers.Log("Example {0}", 396);
            Helpers.Log("Section: {0}", "Inlines - Autolinks");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "<http://foo.bar.baz?q=hello&id=22&boolean>");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        [TestMethod]
        [TestCategory("Inlines - Autolinks")]
        //[Timeout(1000)]
        public void Example397()
        {
            // Example 397
            // Section: Inlines - Autolinks
            //
            // The following CommonMark:
            //     <irc://foo.bar:2233/baz>
            //
            // Should be rendered as:
            //     <p><a href="irc://foo.bar:2233/baz">irc://foo.bar:2233/baz</a></p>

            // Arrange
            var commonMark = Helpers.Normalize("<irc://foo.bar:2233/baz>");
            var expected = Helpers.Normalize("<p><a href=\"irc://foo.bar:2233/baz\">irc://foo.bar:2233/baz</a></p>");
            Helpers.Log("Example {0}", 397);
            Helpers.Log("Section: {0}", "Inlines - Autolinks");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "<irc://foo.bar:2233/baz>");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // Uppercase is also fine:
        [TestMethod]
        [TestCategory("Inlines - Autolinks")]
        //[Timeout(1000)]
        public void Example398()
        {
            // Example 398
            // Section: Inlines - Autolinks
            //
            // The following CommonMark:
            //     <MAILTO:FOO@BAR.BAZ>
            //
            // Should be rendered as:
            //     <p><a href="MAILTO:FOO@BAR.BAZ">MAILTO:FOO@BAR.BAZ</a></p>

            // Arrange
            var commonMark = Helpers.Normalize("<MAILTO:FOO@BAR.BAZ>");
            var expected = Helpers.Normalize("<p><a href=\"MAILTO:FOO@BAR.BAZ\">MAILTO:FOO@BAR.BAZ</a></p>");
            Helpers.Log("Example {0}", 398);
            Helpers.Log("Section: {0}", "Inlines - Autolinks");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "<MAILTO:FOO@BAR.BAZ>");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // Spaces are not allowed in autolinks:
        [TestMethod]
        [TestCategory("Inlines - Autolinks")]
        //[Timeout(1000)]
        public void Example399()
        {
            // Example 399
            // Section: Inlines - Autolinks
            //
            // The following CommonMark:
            //     <http://foo.bar/baz bim>
            //
            // Should be rendered as:
            //     <p>&lt;http://foo.bar/baz bim&gt;</p>

            // Arrange
            var commonMark = Helpers.Normalize("<http://foo.bar/baz bim>");
            var expected = Helpers.Normalize("<p>&lt;http://foo.bar/baz bim&gt;</p>");
            Helpers.Log("Example {0}", 399);
            Helpers.Log("Section: {0}", "Inlines - Autolinks");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "<http://foo.bar/baz bim>");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // An [email autolink](#email-autolink) <a id="email-autolink"></a>
        // consists of `<`, followed by an [email address](#email-address),
        // followed by `>`.  The link's label is the email address,
        // and the URL is `mailto:` followed by the email address.
        //
        // An [email address](#email-address), <a id="email-address"></a>
        // for these purposes, is anything that matches
        // the [non-normative regex from the HTML5
        // spec](http://www.whatwg.org/specs/web-apps/current-work/multipage/forms.html#e-mail-state-%28type=email%29):
        //
        // /^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?
        // (?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*$/
        //
        // Examples of email autolinks:
        [TestMethod]
        [TestCategory("Inlines - Autolinks")]
        //[Timeout(1000)]
        public void Example400()
        {
            // Example 400
            // Section: Inlines - Autolinks
            //
            // The following CommonMark:
            //     <foo@bar.baz.com>
            //
            // Should be rendered as:
            //     <p><a href="mailto:foo@bar.baz.com">foo@bar.baz.com</a></p>

            // Arrange
            var commonMark = Helpers.Normalize("<foo@bar.baz.com>");
            var expected = Helpers.Normalize("<p><a href=\"mailto:foo@bar.baz.com\">foo@bar.baz.com</a></p>");
            Helpers.Log("Example {0}", 400);
            Helpers.Log("Section: {0}", "Inlines - Autolinks");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "<foo@bar.baz.com>");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        [TestMethod]
        [TestCategory("Inlines - Autolinks")]
        //[Timeout(1000)]
        public void Example401()
        {
            // Example 401
            // Section: Inlines - Autolinks
            //
            // The following CommonMark:
            //     <foo+special@Bar.baz-bar0.com>
            //
            // Should be rendered as:
            //     <p><a href="mailto:foo+special@Bar.baz-bar0.com">foo+special@Bar.baz-bar0.com</a></p>

            // Arrange
            var commonMark = Helpers.Normalize("<foo+special@Bar.baz-bar0.com>");
            var expected = Helpers.Normalize("<p><a href=\"mailto:foo+special@Bar.baz-bar0.com\">foo+special@Bar.baz-bar0.com</a></p>");
            Helpers.Log("Example {0}", 401);
            Helpers.Log("Section: {0}", "Inlines - Autolinks");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "<foo+special@Bar.baz-bar0.com>");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // These are not autolinks:
        [TestMethod]
        [TestCategory("Inlines - Autolinks")]
        //[Timeout(1000)]
        public void Example402()
        {
            // Example 402
            // Section: Inlines - Autolinks
            //
            // The following CommonMark:
            //     <>
            //
            // Should be rendered as:
            //     <p>&lt;&gt;</p>

            // Arrange
            var commonMark = Helpers.Normalize("<>");
            var expected = Helpers.Normalize("<p>&lt;&gt;</p>");
            Helpers.Log("Example {0}", 402);
            Helpers.Log("Section: {0}", "Inlines - Autolinks");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "<>");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        [TestMethod]
        [TestCategory("Inlines - Autolinks")]
        //[Timeout(1000)]
        public void Example403()
        {
            // Example 403
            // Section: Inlines - Autolinks
            //
            // The following CommonMark:
            //     <heck://bing.bong>
            //
            // Should be rendered as:
            //     <p>&lt;heck://bing.bong&gt;</p>

            // Arrange
            var commonMark = Helpers.Normalize("<heck://bing.bong>");
            var expected = Helpers.Normalize("<p>&lt;heck://bing.bong&gt;</p>");
            Helpers.Log("Example {0}", 403);
            Helpers.Log("Section: {0}", "Inlines - Autolinks");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "<heck://bing.bong>");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        [TestMethod]
        [TestCategory("Inlines - Autolinks")]
        //[Timeout(1000)]
        public void Example404()
        {
            // Example 404
            // Section: Inlines - Autolinks
            //
            // The following CommonMark:
            //     < http://foo.bar >
            //
            // Should be rendered as:
            //     <p>&lt; http://foo.bar &gt;</p>

            // Arrange
            var commonMark = Helpers.Normalize("< http://foo.bar >");
            var expected = Helpers.Normalize("<p>&lt; http://foo.bar &gt;</p>");
            Helpers.Log("Example {0}", 404);
            Helpers.Log("Section: {0}", "Inlines - Autolinks");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "< http://foo.bar >");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        [TestMethod]
        [TestCategory("Inlines - Autolinks")]
        //[Timeout(1000)]
        public void Example405()
        {
            // Example 405
            // Section: Inlines - Autolinks
            //
            // The following CommonMark:
            //     <foo.bar.baz>
            //
            // Should be rendered as:
            //     <p>&lt;foo.bar.baz&gt;</p>

            // Arrange
            var commonMark = Helpers.Normalize("<foo.bar.baz>");
            var expected = Helpers.Normalize("<p>&lt;foo.bar.baz&gt;</p>");
            Helpers.Log("Example {0}", 405);
            Helpers.Log("Section: {0}", "Inlines - Autolinks");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "<foo.bar.baz>");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        [TestMethod]
        [TestCategory("Inlines - Autolinks")]
        //[Timeout(1000)]
        public void Example406()
        {
            // Example 406
            // Section: Inlines - Autolinks
            //
            // The following CommonMark:
            //     <localhost:5001/foo>
            //
            // Should be rendered as:
            //     <p>&lt;localhost:5001/foo&gt;</p>

            // Arrange
            var commonMark = Helpers.Normalize("<localhost:5001/foo>");
            var expected = Helpers.Normalize("<p>&lt;localhost:5001/foo&gt;</p>");
            Helpers.Log("Example {0}", 406);
            Helpers.Log("Section: {0}", "Inlines - Autolinks");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "<localhost:5001/foo>");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        [TestMethod]
        [TestCategory("Inlines - Autolinks")]
        //[Timeout(1000)]
        public void Example407()
        {
            // Example 407
            // Section: Inlines - Autolinks
            //
            // The following CommonMark:
            //     http://google.com
            //
            // Should be rendered as:
            //     <p>http://google.com</p>

            // Arrange
            var commonMark = Helpers.Normalize("http://google.com");
            var expected = Helpers.Normalize("<p>http://google.com</p>");
            Helpers.Log("Example {0}", 407);
            Helpers.Log("Section: {0}", "Inlines - Autolinks");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "http://google.com");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        [TestMethod]
        [TestCategory("Inlines - Autolinks")]
        //[Timeout(1000)]
        public void Example408()
        {
            // Example 408
            // Section: Inlines - Autolinks
            //
            // The following CommonMark:
            //     foo@bar.baz.com
            //
            // Should be rendered as:
            //     <p>foo@bar.baz.com</p>

            // Arrange
            var commonMark = Helpers.Normalize("foo@bar.baz.com");
            var expected = Helpers.Normalize("<p>foo@bar.baz.com</p>");
            Helpers.Log("Example {0}", 408);
            Helpers.Log("Section: {0}", "Inlines - Autolinks");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "foo@bar.baz.com");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // ## Raw HTML
        //
        // Text between `<` and `>` that looks like an HTML tag is parsed as a
        // raw HTML tag and will be rendered in HTML without escaping.
        // Tag and attribute names are not limited to current HTML tags,
        // so custom tags (and even, say, DocBook tags) may be used.
        //
        // Here is the grammar for tags:
        //
        // A [tag name](#tag-name) <a id="tag-name"></a> consists of an ASCII letter
        // followed by zero or more ASCII letters or digits.
        //
        // An [attribute](#attribute) <a id="attribute"></a> consists of whitespace,
        // an **attribute name**, and an optional **attribute value
        // specification**.
        //
        // An [attribute name](#attribute-name) <a id="attribute-name"></a>
        // consists of an ASCII letter, `_`, or `:`, followed by zero or more ASCII
        // letters, digits, `_`, `.`, `:`, or `-`.  (Note:  This is the XML
        // specification restricted to ASCII.  HTML5 is laxer.)
        //
        // An [attribute value specification](#attribute-value-specification)
        // <a id="attribute-value-specification"></a> consists of optional whitespace,
        // a `=` character, optional whitespace, and an [attribute
        // value](#attribute-value).
        //
        // An [attribute value](#attribute-value) <a id="attribute-value"></a>
        // consists of an [unquoted attribute value](#unquoted-attribute-value),
        // a [single-quoted attribute value](#single-quoted-attribute-value),
        // or a [double-quoted attribute value](#double-quoted-attribute-value).
        //
        // An [unquoted attribute value](#unquoted-attribute-value)
        // <a id="unquoted-attribute-value"></a> is a nonempty string of characters not
        // including spaces, `"`, `'`, `=`, `<`, `>`, or `` ` ``.
        //
        // A [single-quoted attribute value](#single-quoted-attribute-value)
        // <a id="single-quoted-attribute-value"></a> consists of `'`, zero or more
        // characters not including `'`, and a final `'`.
        //
        // A [double-quoted attribute value](#double-quoted-attribute-value)
        // <a id="double-quoted-attribute-value"></a> consists of `"`, zero or more
        // characters not including `"`, and a final `"`.
        //
        // An [open tag](#open-tag) <a id="open-tag"></a> consists of a `<` character,
        // a [tag name](#tag-name), zero or more [attributes](#attribute),
        // optional whitespace, an optional `/` character, and a `>` character.
        //
        // A [closing tag](#closing-tag) <a id="closing-tag"></a> consists of the
        // string `</`, a [tag name](#tag-name), optional whitespace, and the
        // character `>`.
        //
        // An [HTML comment](#html-comment) <a id="html-comment"></a> consists of the
        // string `<!--`, a string of characters not including the string `--`, and
        // the string `-->`.
        //
        // A [processing instruction](#processing-instruction)
        // <a id="processing-instruction"></a> consists of the string `<?`, a string
        // of characters not including the string `?>`, and the string
        // `?>`.
        //
        // A [declaration](#declaration) <a id="declaration"></a> consists of the
        // string `<!`, a name consisting of one or more uppercase ASCII letters,
        // whitespace, a string of characters not including the character `>`, and
        // the character `>`.
        //
        // A [CDATA section](#cdata-section) <a id="cdata-section"></a> consists of
        // the string `<![CDATA[`, a string of characters not including the string
        // `]]>`, and the string `]]>`.
        //
        // An [HTML tag](#html-tag) <a id="html-tag"></a> consists of an [open
        // tag](#open-tag), a [closing tag](#closing-tag), an [HTML
        // comment](#html-comment), a [processing
        // instruction](#processing-instruction), an [element type
        // declaration](#element-type-declaration), or a [CDATA
        // section](#cdata-section).
        //
        // Here are some simple open tags:
        [TestMethod]
        [TestCategory("Inlines - Raw HTML")]
        //[Timeout(1000)]
        public void Example409()
        {
            // Example 409
            // Section: Inlines - Raw HTML
            //
            // The following CommonMark:
            //     <a><bab><c2c>
            //
            // Should be rendered as:
            //     <p><a><bab><c2c></p>

            // Arrange
            var commonMark = Helpers.Normalize("<a><bab><c2c>");
            var expected = Helpers.Normalize("<p><a><bab><c2c></p>");
            Helpers.Log("Example {0}", 409);
            Helpers.Log("Section: {0}", "Inlines - Raw HTML");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "<a><bab><c2c>");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // Empty elements:
        [TestMethod]
        [TestCategory("Inlines - Raw HTML")]
        //[Timeout(1000)]
        public void Example410()
        {
            // Example 410
            // Section: Inlines - Raw HTML
            //
            // The following CommonMark:
            //     <a/><b2/>
            //
            // Should be rendered as:
            //     <p><a/><b2/></p>

            // Arrange
            var commonMark = Helpers.Normalize("<a/><b2/>");
            var expected = Helpers.Normalize("<p><a/><b2/></p>");
            Helpers.Log("Example {0}", 410);
            Helpers.Log("Section: {0}", "Inlines - Raw HTML");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "<a/><b2/>");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // Whitespace is allowed:
        [TestMethod]
        [TestCategory("Inlines - Raw HTML")]
        //[Timeout(1000)]
        public void Example411()
        {
            // Example 411
            // Section: Inlines - Raw HTML
            //
            // The following CommonMark:
            //     <a  /><b2
            //     data="foo" >
            //
            // Should be rendered as:
            //     <p><a  /><b2
            //     data="foo" ></p>

            // Arrange
            var commonMark = Helpers.Normalize("<a  /><b2\ndata=\"foo\" >");
            var expected = Helpers.Normalize("<p><a  /><b2\ndata=\"foo\" ></p>");
            Helpers.Log("Example {0}", 411);
            Helpers.Log("Section: {0}", "Inlines - Raw HTML");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "<a  /><b2\ndata=\"foo\" >");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // With attributes:
        [TestMethod]
        [TestCategory("Inlines - Raw HTML")]
        //[Timeout(1000)]
        public void Example412()
        {
            // Example 412
            // Section: Inlines - Raw HTML
            //
            // The following CommonMark:
            //     <a foo="bar" bam = 'baz <em>"</em>'
            //     _boolean zoop:33=zoop:33 />
            //
            // Should be rendered as:
            //     <p><a foo="bar" bam = 'baz <em>"</em>'
            //     _boolean zoop:33=zoop:33 /></p>

            // Arrange
            var commonMark = Helpers.Normalize("<a foo=\"bar\" bam = 'baz <em>\"</em>'\n_boolean zoop:33=zoop:33 />");
            var expected = Helpers.Normalize("<p><a foo=\"bar\" bam = 'baz <em>\"</em>'\n_boolean zoop:33=zoop:33 /></p>");
            Helpers.Log("Example {0}", 412);
            Helpers.Log("Section: {0}", "Inlines - Raw HTML");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "<a foo=\"bar\" bam = 'baz <em>\"</em>'\n_boolean zoop:33=zoop:33 />");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // Illegal tag names, not parsed as HTML:
        [TestMethod]
        [TestCategory("Inlines - Raw HTML")]
        //[Timeout(1000)]
        public void Example413()
        {
            // Example 413
            // Section: Inlines - Raw HTML
            //
            // The following CommonMark:
            //     <33> <__>
            //
            // Should be rendered as:
            //     <p>&lt;33&gt; &lt;__&gt;</p>

            // Arrange
            var commonMark = Helpers.Normalize("<33> <__>");
            var expected = Helpers.Normalize("<p>&lt;33&gt; &lt;__&gt;</p>");
            Helpers.Log("Example {0}", 413);
            Helpers.Log("Section: {0}", "Inlines - Raw HTML");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "<33> <__>");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // Illegal attribute names:
        [TestMethod]
        [TestCategory("Inlines - Raw HTML")]
        //[Timeout(1000)]
        public void Example414()
        {
            // Example 414
            // Section: Inlines - Raw HTML
            //
            // The following CommonMark:
            //     <a h*#ref="hi">
            //
            // Should be rendered as:
            //     <p>&lt;a h*#ref=&quot;hi&quot;&gt;</p>

            // Arrange
            var commonMark = Helpers.Normalize("<a h*#ref=\"hi\">");
            var expected = Helpers.Normalize("<p>&lt;a h*#ref=&quot;hi&quot;&gt;</p>");
            Helpers.Log("Example {0}", 414);
            Helpers.Log("Section: {0}", "Inlines - Raw HTML");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "<a h*#ref=\"hi\">");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // Illegal attribute values:
        [TestMethod]
        [TestCategory("Inlines - Raw HTML")]
        //[Timeout(1000)]
        public void Example415()
        {
            // Example 415
            // Section: Inlines - Raw HTML
            //
            // The following CommonMark:
            //     <a href="hi'> <a href=hi'>
            //
            // Should be rendered as:
            //     <p>&lt;a href=&quot;hi'&gt; &lt;a href=hi'&gt;</p>

            // Arrange
            var commonMark = Helpers.Normalize("<a href=\"hi'> <a href=hi'>");
            var expected = Helpers.Normalize("<p>&lt;a href=&quot;hi'&gt; &lt;a href=hi'&gt;</p>");
            Helpers.Log("Example {0}", 415);
            Helpers.Log("Section: {0}", "Inlines - Raw HTML");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "<a href=\"hi'> <a href=hi'>");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // Illegal whitespace:
        [TestMethod]
        [TestCategory("Inlines - Raw HTML")]
        //[Timeout(1000)]
        public void Example416()
        {
            // Example 416
            // Section: Inlines - Raw HTML
            //
            // The following CommonMark:
            //     < a><
            //     foo><bar/ >
            //
            // Should be rendered as:
            //     <p>&lt; a&gt;&lt;
            //     foo&gt;&lt;bar/ &gt;</p>

            // Arrange
            var commonMark = Helpers.Normalize("< a><\nfoo><bar/ >");
            var expected = Helpers.Normalize("<p>&lt; a&gt;&lt;\nfoo&gt;&lt;bar/ &gt;</p>");
            Helpers.Log("Example {0}", 416);
            Helpers.Log("Section: {0}", "Inlines - Raw HTML");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "< a><\nfoo><bar/ >");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // Missing whitespace:
        [TestMethod]
        [TestCategory("Inlines - Raw HTML")]
        //[Timeout(1000)]
        public void Example417()
        {
            // Example 417
            // Section: Inlines - Raw HTML
            //
            // The following CommonMark:
            //     <a href='bar'title=title>
            //
            // Should be rendered as:
            //     <p>&lt;a href='bar'title=title&gt;</p>

            // Arrange
            var commonMark = Helpers.Normalize("<a href='bar'title=title>");
            var expected = Helpers.Normalize("<p>&lt;a href='bar'title=title&gt;</p>");
            Helpers.Log("Example {0}", 417);
            Helpers.Log("Section: {0}", "Inlines - Raw HTML");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "<a href='bar'title=title>");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // Closing tags:
        [TestMethod]
        [TestCategory("Inlines - Raw HTML")]
        //[Timeout(1000)]
        public void Example418()
        {
            // Example 418
            // Section: Inlines - Raw HTML
            //
            // The following CommonMark:
            //     </a>
            //     </foo >
            //
            // Should be rendered as:
            //     <p></a>
            //     </foo ></p>

            // Arrange
            var commonMark = Helpers.Normalize("</a>\n</foo >");
            var expected = Helpers.Normalize("<p></a>\n</foo ></p>");
            Helpers.Log("Example {0}", 418);
            Helpers.Log("Section: {0}", "Inlines - Raw HTML");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "</a>\n</foo >");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // Illegal attributes in closing tag:
        [TestMethod]
        [TestCategory("Inlines - Raw HTML")]
        //[Timeout(1000)]
        public void Example419()
        {
            // Example 419
            // Section: Inlines - Raw HTML
            //
            // The following CommonMark:
            //     </a href="foo">
            //
            // Should be rendered as:
            //     <p>&lt;/a href=&quot;foo&quot;&gt;</p>

            // Arrange
            var commonMark = Helpers.Normalize("</a href=\"foo\">");
            var expected = Helpers.Normalize("<p>&lt;/a href=&quot;foo&quot;&gt;</p>");
            Helpers.Log("Example {0}", 419);
            Helpers.Log("Section: {0}", "Inlines - Raw HTML");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "</a href=\"foo\">");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // Comments:
        [TestMethod]
        [TestCategory("Inlines - Raw HTML")]
        //[Timeout(1000)]
        public void Example420()
        {
            // Example 420
            // Section: Inlines - Raw HTML
            //
            // The following CommonMark:
            //     foo <!-- this is a
            //     comment - with hyphen -->
            //
            // Should be rendered as:
            //     <p>foo <!-- this is a
            //     comment - with hyphen --></p>

            // Arrange
            var commonMark = Helpers.Normalize("foo <!-- this is a\ncomment - with hyphen -->");
            var expected = Helpers.Normalize("<p>foo <!-- this is a\ncomment - with hyphen --></p>");
            Helpers.Log("Example {0}", 420);
            Helpers.Log("Section: {0}", "Inlines - Raw HTML");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "foo <!-- this is a\ncomment - with hyphen -->");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        [TestMethod]
        [TestCategory("Inlines - Raw HTML")]
        //[Timeout(1000)]
        public void Example421()
        {
            // Example 421
            // Section: Inlines - Raw HTML
            //
            // The following CommonMark:
            //     foo <!-- not a comment -- two hyphens -->
            //
            // Should be rendered as:
            //     <p>foo &lt;!-- not a comment -- two hyphens --&gt;</p>

            // Arrange
            var commonMark = Helpers.Normalize("foo <!-- not a comment -- two hyphens -->");
            var expected = Helpers.Normalize("<p>foo &lt;!-- not a comment -- two hyphens --&gt;</p>");
            Helpers.Log("Example {0}", 421);
            Helpers.Log("Section: {0}", "Inlines - Raw HTML");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "foo <!-- not a comment -- two hyphens -->");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // Processing instructions:
        [TestMethod]
        [TestCategory("Inlines - Raw HTML")]
        //[Timeout(1000)]
        public void Example422()
        {
            // Example 422
            // Section: Inlines - Raw HTML
            //
            // The following CommonMark:
            //     foo <?php echo $a; ?>
            //
            // Should be rendered as:
            //     <p>foo <?php echo $a; ?></p>

            // Arrange
            var commonMark = Helpers.Normalize("foo <?php echo $a; ?>");
            var expected = Helpers.Normalize("<p>foo <?php echo $a; ?></p>");
            Helpers.Log("Example {0}", 422);
            Helpers.Log("Section: {0}", "Inlines - Raw HTML");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "foo <?php echo $a; ?>");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // Declarations:
        [TestMethod]
        [TestCategory("Inlines - Raw HTML")]
        //[Timeout(1000)]
        public void Example423()
        {
            // Example 423
            // Section: Inlines - Raw HTML
            //
            // The following CommonMark:
            //     foo <!ELEMENT br EMPTY>
            //
            // Should be rendered as:
            //     <p>foo <!ELEMENT br EMPTY></p>

            // Arrange
            var commonMark = Helpers.Normalize("foo <!ELEMENT br EMPTY>");
            var expected = Helpers.Normalize("<p>foo <!ELEMENT br EMPTY></p>");
            Helpers.Log("Example {0}", 423);
            Helpers.Log("Section: {0}", "Inlines - Raw HTML");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "foo <!ELEMENT br EMPTY>");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // CDATA sections:
        [TestMethod]
        [TestCategory("Inlines - Raw HTML")]
        //[Timeout(1000)]
        public void Example424()
        {
            // Example 424
            // Section: Inlines - Raw HTML
            //
            // The following CommonMark:
            //     foo <![CDATA[>&<]]>
            //
            // Should be rendered as:
            //     <p>foo <![CDATA[>&<]]></p>

            // Arrange
            var commonMark = Helpers.Normalize("foo <![CDATA[>&<]]>");
            var expected = Helpers.Normalize("<p>foo <![CDATA[>&<]]></p>");
            Helpers.Log("Example {0}", 424);
            Helpers.Log("Section: {0}", "Inlines - Raw HTML");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "foo <![CDATA[>&<]]>");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // Entities are preserved in HTML attributes:
        [TestMethod]
        [TestCategory("Inlines - Raw HTML")]
        //[Timeout(1000)]
        public void Example425()
        {
            // Example 425
            // Section: Inlines - Raw HTML
            //
            // The following CommonMark:
            //     <a href="&ouml;">
            //
            // Should be rendered as:
            //     <p><a href="&ouml;"></p>

            // Arrange
            var commonMark = Helpers.Normalize("<a href=\"&ouml;\">");
            var expected = Helpers.Normalize("<p><a href=\"&ouml;\"></p>");
            Helpers.Log("Example {0}", 425);
            Helpers.Log("Section: {0}", "Inlines - Raw HTML");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "<a href=\"&ouml;\">");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // Backslash escapes do not work in HTML attributes:
        [TestMethod]
        [TestCategory("Inlines - Raw HTML")]
        //[Timeout(1000)]
        public void Example426()
        {
            // Example 426
            // Section: Inlines - Raw HTML
            //
            // The following CommonMark:
            //     <a href="\*">
            //
            // Should be rendered as:
            //     <p><a href="\*"></p>

            // Arrange
            var commonMark = Helpers.Normalize("<a href=\"\\*\">");
            var expected = Helpers.Normalize("<p><a href=\"\\*\"></p>");
            Helpers.Log("Example {0}", 426);
            Helpers.Log("Section: {0}", "Inlines - Raw HTML");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "<a href=\"\\*\">");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        [TestMethod]
        [TestCategory("Inlines - Raw HTML")]
        //[Timeout(1000)]
        public void Example427()
        {
            // Example 427
            // Section: Inlines - Raw HTML
            //
            // The following CommonMark:
            //     <a href="\"">
            //
            // Should be rendered as:
            //     <p>&lt;a href=&quot;&quot;&quot;&gt;</p>

            // Arrange
            var commonMark = Helpers.Normalize("<a href=\"\\\"\">");
            var expected = Helpers.Normalize("<p>&lt;a href=&quot;&quot;&quot;&gt;</p>");
            Helpers.Log("Example {0}", 427);
            Helpers.Log("Section: {0}", "Inlines - Raw HTML");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "<a href=\"\\\"\">");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // ## Hard line breaks
        //
        // A line break (not in a code span or HTML tag) that is preceded
        // by two or more spaces is parsed as a linebreak (rendered
        // in HTML as a `<br />` tag):
        [TestMethod]
        [TestCategory("Inlines - Hard line breaks")]
        //[Timeout(1000)]
        public void Example428()
        {
            // Example 428
            // Section: Inlines - Hard line breaks
            //
            // The following CommonMark:
            //     foo  
            //     baz
            //
            // Should be rendered as:
            //     <p>foo<br />
            //     baz</p>

            // Arrange
            var commonMark = Helpers.Normalize("foo  \nbaz");
            var expected = Helpers.Normalize("<p>foo<br />\nbaz</p>");
            Helpers.Log("Example {0}", 428);
            Helpers.Log("Section: {0}", "Inlines - Hard line breaks");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "foo  \nbaz");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // For a more visible alternative, a backslash before the newline may be
        // used instead of two spaces:
        [TestMethod]
        [TestCategory("Inlines - Hard line breaks")]
        //[Timeout(1000)]
        public void Example429()
        {
            // Example 429
            // Section: Inlines - Hard line breaks
            //
            // The following CommonMark:
            //     foo\
            //     baz
            //
            // Should be rendered as:
            //     <p>foo<br />
            //     baz</p>

            // Arrange
            var commonMark = Helpers.Normalize("foo\\\nbaz");
            var expected = Helpers.Normalize("<p>foo<br />\nbaz</p>");
            Helpers.Log("Example {0}", 429);
            Helpers.Log("Section: {0}", "Inlines - Hard line breaks");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "foo\\\nbaz");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // More than two spaces can be used:
        [TestMethod]
        [TestCategory("Inlines - Hard line breaks")]
        //[Timeout(1000)]
        public void Example430()
        {
            // Example 430
            // Section: Inlines - Hard line breaks
            //
            // The following CommonMark:
            //     foo       
            //     baz
            //
            // Should be rendered as:
            //     <p>foo<br />
            //     baz</p>

            // Arrange
            var commonMark = Helpers.Normalize("foo       \nbaz");
            var expected = Helpers.Normalize("<p>foo<br />\nbaz</p>");
            Helpers.Log("Example {0}", 430);
            Helpers.Log("Section: {0}", "Inlines - Hard line breaks");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "foo       \nbaz");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // Leading spaces at the beginning of the next line are ignored:
        [TestMethod]
        [TestCategory("Inlines - Hard line breaks")]
        //[Timeout(1000)]
        public void Example431()
        {
            // Example 431
            // Section: Inlines - Hard line breaks
            //
            // The following CommonMark:
            //     foo  
            //          bar
            //
            // Should be rendered as:
            //     <p>foo<br />
            //     bar</p>

            // Arrange
            var commonMark = Helpers.Normalize("foo  \n     bar");
            var expected = Helpers.Normalize("<p>foo<br />\nbar</p>");
            Helpers.Log("Example {0}", 431);
            Helpers.Log("Section: {0}", "Inlines - Hard line breaks");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "foo  \n     bar");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        [TestMethod]
        [TestCategory("Inlines - Hard line breaks")]
        //[Timeout(1000)]
        public void Example432()
        {
            // Example 432
            // Section: Inlines - Hard line breaks
            //
            // The following CommonMark:
            //     foo\
            //          bar
            //
            // Should be rendered as:
            //     <p>foo<br />
            //     bar</p>

            // Arrange
            var commonMark = Helpers.Normalize("foo\\\n     bar");
            var expected = Helpers.Normalize("<p>foo<br />\nbar</p>");
            Helpers.Log("Example {0}", 432);
            Helpers.Log("Section: {0}", "Inlines - Hard line breaks");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "foo\\\n     bar");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // Line breaks can occur inside emphasis, links, and other constructs
        // that allow inline content:
        [TestMethod]
        [TestCategory("Inlines - Hard line breaks")]
        //[Timeout(1000)]
        public void Example433()
        {
            // Example 433
            // Section: Inlines - Hard line breaks
            //
            // The following CommonMark:
            //     *foo  
            //     bar*
            //
            // Should be rendered as:
            //     <p><em>foo<br />
            //     bar</em></p>

            // Arrange
            var commonMark = Helpers.Normalize("*foo  \nbar*");
            var expected = Helpers.Normalize("<p><em>foo<br />\nbar</em></p>");
            Helpers.Log("Example {0}", 433);
            Helpers.Log("Section: {0}", "Inlines - Hard line breaks");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "*foo  \nbar*");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        [TestMethod]
        [TestCategory("Inlines - Hard line breaks")]
        //[Timeout(1000)]
        public void Example434()
        {
            // Example 434
            // Section: Inlines - Hard line breaks
            //
            // The following CommonMark:
            //     *foo\
            //     bar*
            //
            // Should be rendered as:
            //     <p><em>foo<br />
            //     bar</em></p>

            // Arrange
            var commonMark = Helpers.Normalize("*foo\\\nbar*");
            var expected = Helpers.Normalize("<p><em>foo<br />\nbar</em></p>");
            Helpers.Log("Example {0}", 434);
            Helpers.Log("Section: {0}", "Inlines - Hard line breaks");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "*foo\\\nbar*");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // Line breaks do not occur inside code spans
        [TestMethod]
        [TestCategory("Inlines - Hard line breaks")]
        //[Timeout(1000)]
        public void Example435()
        {
            // Example 435
            // Section: Inlines - Hard line breaks
            //
            // The following CommonMark:
            //     `code  
            //     span`
            //
            // Should be rendered as:
            //     <p><code>code span</code></p>

            // Arrange
            var commonMark = Helpers.Normalize("`code  \nspan`");
            var expected = Helpers.Normalize("<p><code>code span</code></p>");
            Helpers.Log("Example {0}", 435);
            Helpers.Log("Section: {0}", "Inlines - Hard line breaks");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "`code  \nspan`");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        [TestMethod]
        [TestCategory("Inlines - Hard line breaks")]
        //[Timeout(1000)]
        public void Example436()
        {
            // Example 436
            // Section: Inlines - Hard line breaks
            //
            // The following CommonMark:
            //     `code\
            //     span`
            //
            // Should be rendered as:
            //     <p><code>code\ span</code></p>

            // Arrange
            var commonMark = Helpers.Normalize("`code\\\nspan`");
            var expected = Helpers.Normalize("<p><code>code\\ span</code></p>");
            Helpers.Log("Example {0}", 436);
            Helpers.Log("Section: {0}", "Inlines - Hard line breaks");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "`code\\\nspan`");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // or HTML tags:
        [TestMethod]
        [TestCategory("Inlines - Hard line breaks")]
        //[Timeout(1000)]
        public void Example437()
        {
            // Example 437
            // Section: Inlines - Hard line breaks
            //
            // The following CommonMark:
            //     <a href="foo  
            //     bar">
            //
            // Should be rendered as:
            //     <p><a href="foo  
            //     bar"></p>

            // Arrange
            var commonMark = Helpers.Normalize("<a href=\"foo  \nbar\">");
            var expected = Helpers.Normalize("<p><a href=\"foo  \nbar\"></p>");
            Helpers.Log("Example {0}", 437);
            Helpers.Log("Section: {0}", "Inlines - Hard line breaks");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "<a href=\"foo  \nbar\">");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        [TestMethod]
        [TestCategory("Inlines - Hard line breaks")]
        //[Timeout(1000)]
        public void Example438()
        {
            // Example 438
            // Section: Inlines - Hard line breaks
            //
            // The following CommonMark:
            //     <a href="foo\
            //     bar">
            //
            // Should be rendered as:
            //     <p><a href="foo\
            //     bar"></p>

            // Arrange
            var commonMark = Helpers.Normalize("<a href=\"foo\\\nbar\">");
            var expected = Helpers.Normalize("<p><a href=\"foo\\\nbar\"></p>");
            Helpers.Log("Example {0}", 438);
            Helpers.Log("Section: {0}", "Inlines - Hard line breaks");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "<a href=\"foo\\\nbar\">");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // ## Soft line breaks
        //
        // A regular line break (not in a code span or HTML tag) that is not
        // preceded by two or more spaces is parsed as a softbreak.  (A
        // softbreak may be rendered in HTML either as a newline or as a space.
        // The result will be the same in browsers. In the examples here, a
        // newline will be used.)
        [TestMethod]
        [TestCategory("Inlines - Soft line breaks")]
        //[Timeout(1000)]
        public void Example439()
        {
            // Example 439
            // Section: Inlines - Soft line breaks
            //
            // The following CommonMark:
            //     foo
            //     baz
            //
            // Should be rendered as:
            //     <p>foo
            //     baz</p>

            // Arrange
            var commonMark = Helpers.Normalize("foo\nbaz");
            var expected = Helpers.Normalize("<p>foo\nbaz</p>");
            Helpers.Log("Example {0}", 439);
            Helpers.Log("Section: {0}", "Inlines - Soft line breaks");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "foo\nbaz");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // Spaces at the end of the line and beginning of the next line are
        // removed:
        [TestMethod]
        [TestCategory("Inlines - Soft line breaks")]
        //[Timeout(1000)]
        public void Example440()
        {
            // Example 440
            // Section: Inlines - Soft line breaks
            //
            // The following CommonMark:
            //     foo 
            //      baz
            //
            // Should be rendered as:
            //     <p>foo
            //     baz</p>

            // Arrange
            var commonMark = Helpers.Normalize("foo \n baz");
            var expected = Helpers.Normalize("<p>foo\nbaz</p>");
            Helpers.Log("Example {0}", 440);
            Helpers.Log("Section: {0}", "Inlines - Soft line breaks");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "foo \n baz");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // A conforming parser may render a soft line break in HTML either as a
        // line break or as a space.
        //
        // A renderer may also provide an option to render soft line breaks
        // as hard line breaks.
        //
        // ## Strings
        //
        // Any characters not given an interpretation by the above rules will
        // be parsed as string content.
        [TestMethod]
        [TestCategory("Inlines - Strings")]
        //[Timeout(1000)]
        public void Example441()
        {
            // Example 441
            // Section: Inlines - Strings
            //
            // The following CommonMark:
            //     hello $.;'there
            //
            // Should be rendered as:
            //     <p>hello $.;'there</p>

            // Arrange
            var commonMark = Helpers.Normalize("hello $.;'there");
            var expected = Helpers.Normalize("<p>hello $.;'there</p>");
            Helpers.Log("Example {0}", 441);
            Helpers.Log("Section: {0}", "Inlines - Strings");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "hello $.;'there");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        [TestMethod]
        [TestCategory("Inlines - Strings")]
        //[Timeout(1000)]
        public void Example442()
        {
            // Example 442
            // Section: Inlines - Strings
            //
            // The following CommonMark:
            //     Foo χρῆν
            //
            // Should be rendered as:
            //     <p>Foo χρῆν</p>

            // Arrange
            var commonMark = Helpers.Normalize("Foo χρῆν");
            var expected = Helpers.Normalize("<p>Foo χρῆν</p>");
            Helpers.Log("Example {0}", 442);
            Helpers.Log("Section: {0}", "Inlines - Strings");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "Foo χρῆν");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // Internal spaces are preserved verbatim:
        [TestMethod]
        [TestCategory("Inlines - Strings")]
        //[Timeout(1000)]
        public void Example443()
        {
            // Example 443
            // Section: Inlines - Strings
            //
            // The following CommonMark:
            //     Multiple     spaces
            //
            // Should be rendered as:
            //     <p>Multiple     spaces</p>

            // Arrange
            var commonMark = Helpers.Normalize("Multiple     spaces");
            var expected = Helpers.Normalize("<p>Multiple     spaces</p>");
            Helpers.Log("Example {0}", 443);
            Helpers.Log("Section: {0}", "Inlines - Strings");
            Helpers.Log();
            Helpers.LogValue("CommonMark", "Multiple     spaces");
            Helpers.LogValue("Expected", expected);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }

        // # Appendix A: A parsing strategy {-}
        //
        // ## Overview {-}
        //
        // Parsing has two phases:
        //
        // 1. In the first phase, lines of input are consumed and the block
        // structure of the document---its division into paragraphs, block quotes,
        // list items, and so on---is constructed.  Text is assigned to these
        // blocks but not parsed. Link reference definitions are parsed and a
        // map of links is constructed.
        //
        // 2. In the second phase, the raw text contents of paragraphs and headers
        // are parsed into sequences of Markdown inline elements (strings,
        // code spans, links, emphasis, and so on), using the map of link
        // references constructed in phase 1.
        //
        // ## The document tree {-}
        //
        // At each point in processing, the document is represented as a tree of
        // **blocks**.  The root of the tree is a `document` block.  The `document`
        // may have any number of other blocks as **children**.  These children
        // may, in turn, have other blocks as children.  The last child of a block
        // is normally considered **open**, meaning that subsequent lines of input
        // can alter its contents.  (Blocks that are not open are **closed**.)
        // Here, for example, is a possible document tree, with the open blocks
        // marked by arrows:
        //
        // ``` tree
        // -> document
        // -> block_quote
        // paragraph
        // "Lorem ipsum dolor\nsit amet."
        // -> list (type=bullet tight=true bullet_char=-)
        // list_item
        // paragraph
        // "Qui *quodsi iracundia*"
        // -> list_item
        // -> paragraph
        // "aliquando id"
        // ```
        //
        // ## How source lines alter the document tree {-}
        //
        // Each line that is processed has an effect on this tree.  The line is
        // analyzed and, depending on its contents, the document may be altered
        // in one or more of the following ways:
        //
        // 1. One or more open blocks may be closed.
        // 2. One or more new blocks may be created as children of the
        // last open block.
        // 3. Text may be added to the last (deepest) open block remaining
        // on the tree.
        //
        // Once a line has been incorporated into the tree in this way,
        // it can be discarded, so input can be read in a stream.
        //
        // We can see how this works by considering how the tree above is
        // generated by four lines of Markdown:
        //
        // ``` markdown
        // > Lorem ipsum dolor
        // sit amet.
        // > - Qui *quodsi iracundia*
        // > - aliquando id
        // ```
        //
        // At the outset, our document model is just
        //
        // ``` tree
        // -> document
        // ```
        //
        // The first line of our text,
        //
        // ``` markdown
        // > Lorem ipsum dolor
        // ```
        //
        // causes a `block_quote` block to be created as a child of our
        // open `document` block, and a `paragraph` block as a child of
        // the `block_quote`.  Then the text is added to the last open
        // block, the `paragraph`:
        //
        // ``` tree
        // -> document
        // -> block_quote
        // -> paragraph
        // "Lorem ipsum dolor"
        // ```
        //
        // The next line,
        //
        // ``` markdown
        // sit amet.
        // ```
        //
        // is a "lazy continuation" of the open `paragraph`, so it gets added
        // to the paragraph's text:
        //
        // ``` tree
        // -> document
        // -> block_quote
        // -> paragraph
        // "Lorem ipsum dolor\nsit amet."
        // ```
        //
        // The third line,
        //
        // ``` markdown
        // > - Qui *quodsi iracundia*
        // ```
        //
        // causes the `paragraph` block to be closed, and a new `list` block
        // opened as a child of the `block_quote`.  A `list_item` is also
        // added as a child of the `list`, and a `paragraph` as a child of
        // the `list_item`.  The text is then added to the new `paragraph`:
        //
        // ``` tree
        // -> document
        // -> block_quote
        // paragraph
        // "Lorem ipsum dolor\nsit amet."
        // -> list (type=bullet tight=true bullet_char=-)
        // -> list_item
        // -> paragraph
        // "Qui *quodsi iracundia*"
        // ```
        //
        // The fourth line,
        //
        // ``` markdown
        // > - aliquando id
        // ```
        //
        // causes the `list_item` (and its child the `paragraph`) to be closed,
        // and a new `list_item` opened up as child of the `list`.  A `paragraph`
        // is added as a child of the new `list_item`, to contain the text.
        // We thus obtain the final tree:
        //
        // ``` tree
        // -> document
        // -> block_quote
        // paragraph
        // "Lorem ipsum dolor\nsit amet."
        // -> list (type=bullet tight=true bullet_char=-)
        // list_item
        // paragraph
        // "Qui *quodsi iracundia*"
        // -> list_item
        // -> paragraph
        // "aliquando id"
        // ```
        //
        // ## From block structure to the final document {-}
        //
        // Once all of the input has been parsed, all open blocks are closed.
        //
        // We then "walk the tree," visiting every node, and parse raw
        // string contents of paragraphs and headers as inlines.  At this
        // point we have seen all the link reference definitions, so we can
        // resolve reference links as we go.
        //
        // ``` tree
        // document
        // block_quote
        // paragraph
        // str "Lorem ipsum dolor"
        // softbreak
        // str "sit amet."
        // list (type=bullet tight=true bullet_char=-)
        // list_item
        // paragraph
        // str "Qui "
        // emph
        // str "quodsi iracundia"
        // list_item
        // paragraph
        // str "aliquando id"
        // ```
        //
        // Notice how the newline in the first paragraph has been parsed as
        // a `softbreak`, and the asterisks in the first list item have become
        // an `emph`.
        //
        // The document can be rendered as HTML, or in any other format, given
        // an appropriate renderer.
    }
}
