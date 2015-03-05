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
        // ---
        // title: CommonMark Spec
        // author: John MacFarlane
        // version: 0.18
        // date: 2015-03-03
        // license: '[CC-BY-SA 4.0](http://creativecommons.org/licenses/by-sa/4.0/)'
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
        // 11. Can list items include section headers?  (`Markdown.pl` does not
        // allow this, but does allow blockquotes to include headers.)
        //
        // ``` markdown
        // - # Heading
        // ```
        //
        // 12. Can list items be empty?
        //
        // ``` markdown
        // * a
        // *
        // * b
        // ```
        //
        // 13. Can link references be defined inside block quotes or list items?
        //
        // ``` markdown
        // > Blockquote [foo].
        // >
        // > [foo]: /url
        // ```
        //
        // 14. If there are multiple definitions for the same reference, which takes
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
        // accompanying script `spec_tests.py` can be used to run the tests
        // against any Markdown program:
        //
        // python test/spec_tests.py --spec spec.txt --program PROGRAM
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
        // The script `tools/makespec.py` can be used to convert `spec.txt` into
        // HTML or CommonMark (which can then be converted into other formats).
        //
        // In the examples, the `→` character is used to represent tabs.
        //
        // # Preliminaries
        //
        // ## Characters and lines
        //
        // Any sequence of [character]s is a valid CommonMark
        // document.
        //
        // A [character](@character) is a unicode code point.
        // This spec does not specify an encoding; it thinks of lines as composed
        // of characters rather than bytes.  A conforming parser may be limited
        // to a certain encoding.
        //
        // A [line](@line) is a sequence of zero or more [character]s
        // followed by a [line ending] or by the end of file.
        //
        // A [line ending](@line-ending) is, depending on the platform, a
        // newline (`U+000A`), carriage return (`U+000D`), or
        // carriage return + newline.
        //
        // For security reasons, a conforming parser must strip or replace the
        // Unicode character `U+0000`.
        //
        // A line containing no characters, or a line containing only spaces
        // (`U+0020`) or tabs (`U+0009`), is called a [blank line](@blank-line).
        //
        // The following definitions of character classes will be used in this spec:
        //
        // A [whitespace character](@whitespace-character) is a space
        // (`U+0020`), tab (`U+0009`), newline (`U+000A`), line tabulation (`U+000B`),
        // form feed (`U+000C`), or carriage return (`U+000D`).
        //
        // [Whitespace](@whitespace) is a sequence of one or more [whitespace
        // character]s.
        //
        // A [unicode whitespace character](@unicode-whitespace-character) is
        // any code point in the unicode `Zs` class, or a tab (`U+0009`),
        // carriage return (`U+000D`), newline (`U+000A`), or form feed
        // (`U+000C`).
        //
        // [Unicode whitespace](@unicode-whitespace) is a sequence of one
        // or more [unicode whitespace character]s.
        //
        // A [non-space character](@non-space-character) is anything but `U+0020`.
        //
        // An [ASCII punctuation character](@ascii-punctuation-character)
        // is `!`, `"`, `#`, `$`, `%`, `&`, `'`, `(`, `)`,
        // `*`, `+`, `,`, `-`, `.`, `/`, `:`, `;`, `<`, `=`, `>`, `?`, `@`,
        // `[`, `\`, `]`, `^`, `_`, `` ` ``, `{`, `|`, `}`, or `~`.
        //
        // A [punctuation character](@punctuation-character) is an [ASCII
        // punctuation character] or anything in
        // the unicode classes `Pc`, `Pd`, `Pe`, `Pf`, `Pi`, `Po`, or `Ps`.
        //
        // ## Tab expansion
        //
        // Tabs in lines are expanded to spaces, with a tab stop of 4 characters:
        [TestMethod]
        [TestCategory("Preliminaries - Tab expansion")]
        //[Timeout(1000)]
        public void Example001()
        {
            // Example 1
            // Section: Preliminaries - Tab expansion
            //
            // The following CommonMark:
            //     →foo→baz→→bim
            //
            // Should be rendered as:
            //     <pre><code>foo baz     bim
            //     </code></pre>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 1, "Preliminaries - Tab expansion");
			Helpers.ExecuteTest("→foo→baz→→bim", "<pre><code>foo baz     bim\n</code></pre>");
        }

        [TestMethod]
        [TestCategory("Preliminaries - Tab expansion")]
        //[Timeout(1000)]
        public void Example002()
        {
            // Example 2
            // Section: Preliminaries - Tab expansion
            //
            // The following CommonMark:
            //         a→a
            //         ὐ→a
            //
            // Should be rendered as:
            //     <pre><code>a   a
            //     ὐ   a
            //     </code></pre>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 2, "Preliminaries - Tab expansion");
			Helpers.ExecuteTest("    a→a\n    ὐ→a", "<pre><code>a   a\nὐ   a\n</code></pre>");
        }

        // # Blocks and inlines
        //
        // We can think of a document as a sequence of
        // [blocks](@block)---structural
        // elements like paragraphs, block quotations,
        // lists, headers, rules, and code blocks.  Blocks can contain other
        // blocks, or they can contain [inline](@inline) content:
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

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 3, "Blocks and inlines - Precedence");
			Helpers.ExecuteTest("- `one\n- two`", "<ul>\n<li>`one</li>\n<li>two`</li>\n</ul>");
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
        // [container block](@container-block)s,
        // which can contain other blocks, and [leaf block](@leaf-block)s,
        // which cannot.
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
        // optionally by any number of spaces, forms a
        // [horizontal rule](@horizontal-rule).
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

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 4, "Leaf blocks - Horizontal rules");
			Helpers.ExecuteTest("***\n---\n___", "<hr />\n<hr />\n<hr />");
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

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 5, "Leaf blocks - Horizontal rules");
			Helpers.ExecuteTest("+++", "<p>+++</p>");
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

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 6, "Leaf blocks - Horizontal rules");
			Helpers.ExecuteTest("===", "<p>===</p>");
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

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 7, "Leaf blocks - Horizontal rules");
			Helpers.ExecuteTest("--\n**\n__", "<p>--\n**\n__</p>");
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

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 8, "Leaf blocks - Horizontal rules");
			Helpers.ExecuteTest(" ***\n  ***\n   ***", "<hr />\n<hr />\n<hr />");
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

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 9, "Leaf blocks - Horizontal rules");
			Helpers.ExecuteTest("    ***", "<pre><code>***\n</code></pre>");
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

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 10, "Leaf blocks - Horizontal rules");
			Helpers.ExecuteTest("Foo\n    ***", "<p>Foo\n***</p>");
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

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 11, "Leaf blocks - Horizontal rules");
			Helpers.ExecuteTest("_____________________________________", "<hr />");
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

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 12, "Leaf blocks - Horizontal rules");
			Helpers.ExecuteTest(" - - -", "<hr />");
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

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 13, "Leaf blocks - Horizontal rules");
			Helpers.ExecuteTest(" **  * ** * ** * **", "<hr />");
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

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 14, "Leaf blocks - Horizontal rules");
			Helpers.ExecuteTest("-     -      -      -", "<hr />");
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

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 15, "Leaf blocks - Horizontal rules");
			Helpers.ExecuteTest("- - - -    ", "<hr />");
        }

        // However, no other characters may occur in the line:
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
            //     ---a---
            //
            // Should be rendered as:
            //     <p>_ _ _ _ a</p>
            //     <p>a------</p>
            //     <p>---a---</p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 16, "Leaf blocks - Horizontal rules");
			Helpers.ExecuteTest("_ _ _ _ a\n\na------\n\n---a---", "<p>_ _ _ _ a</p>\n<p>a------</p>\n<p>---a---</p>");
        }

        // It is required that all of the [non-space character]s be the same.
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

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 17, "Leaf blocks - Horizontal rules");
			Helpers.ExecuteTest(" *-*", "<p><em>-</em></p>");
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

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 18, "Leaf blocks - Horizontal rules");
			Helpers.ExecuteTest("- foo\n***\n- bar", "<ul>\n<li>foo</li>\n</ul>\n<hr />\n<ul>\n<li>bar</li>\n</ul>");
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

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 19, "Leaf blocks - Horizontal rules");
			Helpers.ExecuteTest("Foo\n***\nbar", "<p>Foo</p>\n<hr />\n<p>bar</p>");
        }

        // If a line of dashes that meets the above conditions for being a
        // horizontal rule could also be interpreted as the underline of a [setext
        // header], the interpretation as a
        // [setext header] takes precedence. Thus, for example,
        // this is a setext header, not a paragraph followed by a horizontal rule:
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

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 20, "Leaf blocks - Horizontal rules");
			Helpers.ExecuteTest("Foo\n---\nbar", "<h2>Foo</h2>\n<p>bar</p>");
        }

        // When both a horizontal rule and a list item are possible
        // interpretations of a line, the horizontal rule takes precedence:
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

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 21, "Leaf blocks - Horizontal rules");
			Helpers.ExecuteTest("* Foo\n* * *\n* Bar", "<ul>\n<li>Foo</li>\n</ul>\n<hr />\n<ul>\n<li>Bar</li>\n</ul>");
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
            //     <li>
            //     <hr />
            //     </li>
            //     </ul>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 22, "Leaf blocks - Horizontal rules");
			Helpers.ExecuteTest("- Foo\n- * * *", "<ul>\n<li>Foo</li>\n<li>\n<hr />\n</li>\n</ul>");
        }

        // ## ATX headers
        //
        // An [ATX header](@atx-header)
        // consists of a string of characters, parsed as inline content, between an
        // opening sequence of 1--6 unescaped `#` characters and an optional
        // closing sequence of any number of `#` characters.  The opening sequence
        // of `#` characters cannot be followed directly by a
        // [non-space character].
        // The optional closing sequence of `#`s must be preceded by a space and may be
        // followed by spaces only.  The opening `#` character may be indented 0-3
        // spaces.  The raw contents of the header are stripped of leading and
        // trailing spaces before being parsed as inline content.  The header level
        // is equal to the number of `#` characters in the opening sequence.
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

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 23, "Leaf blocks - ATX headers");
			Helpers.ExecuteTest("# foo\n## foo\n### foo\n#### foo\n##### foo\n###### foo", "<h1>foo</h1>\n<h2>foo</h2>\n<h3>foo</h3>\n<h4>foo</h4>\n<h5>foo</h5>\n<h6>foo</h6>");
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

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 24, "Leaf blocks - ATX headers");
			Helpers.ExecuteTest("####### foo", "<p>####### foo</p>");
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

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 25, "Leaf blocks - ATX headers");
			Helpers.ExecuteTest("#5 bolt", "<p>#5 bolt</p>");
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

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 26, "Leaf blocks - ATX headers");
			Helpers.ExecuteTest("\\## foo", "<p>## foo</p>");
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

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 27, "Leaf blocks - ATX headers");
			Helpers.ExecuteTest("# foo *bar* \\*baz\\*", "<h1>foo <em>bar</em> *baz*</h1>");
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

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 28, "Leaf blocks - ATX headers");
			Helpers.ExecuteTest("#                  foo                     ", "<h1>foo</h1>");
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

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 29, "Leaf blocks - ATX headers");
			Helpers.ExecuteTest(" ### foo\n  ## foo\n   # foo", "<h3>foo</h3>\n<h2>foo</h2>\n<h1>foo</h1>");
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

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 30, "Leaf blocks - ATX headers");
			Helpers.ExecuteTest("    # foo", "<pre><code># foo\n</code></pre>");
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

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 31, "Leaf blocks - ATX headers");
			Helpers.ExecuteTest("foo\n    # bar", "<p>foo\n# bar</p>");
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

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 32, "Leaf blocks - ATX headers");
			Helpers.ExecuteTest("## foo ##\n  ###   bar    ###", "<h2>foo</h2>\n<h3>bar</h3>");
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

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 33, "Leaf blocks - ATX headers");
			Helpers.ExecuteTest("# foo ##################################\n##### foo ##", "<h1>foo</h1>\n<h5>foo</h5>");
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

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 34, "Leaf blocks - ATX headers");
			Helpers.ExecuteTest("### foo ###     ", "<h3>foo</h3>");
        }

        // A sequence of `#` characters with a
        // [non-space character] following it
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

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 35, "Leaf blocks - ATX headers");
			Helpers.ExecuteTest("### foo ### b", "<h3>foo ### b</h3>");
        }

        // The closing sequence must be preceded by a space:
        [TestMethod]
        [TestCategory("Leaf blocks - ATX headers")]
        //[Timeout(1000)]
        public void Example036()
        {
            // Example 36
            // Section: Leaf blocks - ATX headers
            //
            // The following CommonMark:
            //     # foo#
            //
            // Should be rendered as:
            //     <h1>foo#</h1>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 36, "Leaf blocks - ATX headers");
			Helpers.ExecuteTest("# foo#", "<h1>foo#</h1>");
        }

        // Backslash-escaped `#` characters do not count as part
        // of the closing sequence:
        [TestMethod]
        [TestCategory("Leaf blocks - ATX headers")]
        //[Timeout(1000)]
        public void Example037()
        {
            // Example 37
            // Section: Leaf blocks - ATX headers
            //
            // The following CommonMark:
            //     ### foo \###
            //     ## foo #\##
            //     # foo \#
            //
            // Should be rendered as:
            //     <h3>foo ###</h3>
            //     <h2>foo ###</h2>
            //     <h1>foo #</h1>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 37, "Leaf blocks - ATX headers");
			Helpers.ExecuteTest("### foo \\###\n## foo #\\##\n# foo \\#", "<h3>foo ###</h3>\n<h2>foo ###</h2>\n<h1>foo #</h1>");
        }

        // ATX headers need not be separated from surrounding content by blank
        // lines, and they can interrupt paragraphs:
        [TestMethod]
        [TestCategory("Leaf blocks - ATX headers")]
        //[Timeout(1000)]
        public void Example038()
        {
            // Example 38
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

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 38, "Leaf blocks - ATX headers");
			Helpers.ExecuteTest("****\n## foo\n****", "<hr />\n<h2>foo</h2>\n<hr />");
        }

        [TestMethod]
        [TestCategory("Leaf blocks - ATX headers")]
        //[Timeout(1000)]
        public void Example039()
        {
            // Example 39
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

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 39, "Leaf blocks - ATX headers");
			Helpers.ExecuteTest("Foo bar\n# baz\nBar foo", "<p>Foo bar</p>\n<h1>baz</h1>\n<p>Bar foo</p>");
        }

        // ATX headers can be empty:
        [TestMethod]
        [TestCategory("Leaf blocks - ATX headers")]
        //[Timeout(1000)]
        public void Example040()
        {
            // Example 40
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

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 40, "Leaf blocks - ATX headers");
			Helpers.ExecuteTest("## \n#\n### ###", "<h2></h2>\n<h1></h1>\n<h3></h3>");
        }

        // ## Setext headers
        //
        // A [setext header](@setext-header)
        // consists of a line of text, containing at least one
        // [non-space character],
        // with no more than 3 spaces indentation, followed by a [setext header
        // underline].  The line of text must be
        // one that, were it not followed by the setext header underline,
        // would be interpreted as part of a paragraph:  it cannot be a code
        // block, header, blockquote, horizontal rule, or list.
        //
        // A [setext header underline](@setext-header-underline) is a sequence of
        // `=` characters or a sequence of `-` characters, with no more than 3
        // spaces indentation and any number of trailing spaces.  If a line
        // containing a single `-` can be interpreted as an
        // empty [list items], it should be interpreted this way
        // and not as a [setext header underline].
        //
        // The header is a level 1 header if `=` characters are used in the
        // [setext header underline], and a level 2
        // header if `-` characters are used.  The contents of the header are the
        // result of parsing the first line as Markdown inline content.
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
        public void Example041()
        {
            // Example 41
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

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 41, "Leaf blocks - Setext headers");
			Helpers.ExecuteTest("Foo *bar*\n=========\n\nFoo *bar*\n---------", "<h1>Foo <em>bar</em></h1>\n<h2>Foo <em>bar</em></h2>");
        }

        // The underlining can be any length:
        [TestMethod]
        [TestCategory("Leaf blocks - Setext headers")]
        //[Timeout(1000)]
        public void Example042()
        {
            // Example 42
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

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 42, "Leaf blocks - Setext headers");
			Helpers.ExecuteTest("Foo\n-------------------------\n\nFoo\n=", "<h2>Foo</h2>\n<h1>Foo</h1>");
        }

        // The header content can be indented up to three spaces, and need
        // not line up with the underlining:
        [TestMethod]
        [TestCategory("Leaf blocks - Setext headers")]
        //[Timeout(1000)]
        public void Example043()
        {
            // Example 43
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

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 43, "Leaf blocks - Setext headers");
			Helpers.ExecuteTest("   Foo\n---\n\n  Foo\n-----\n\n  Foo\n  ===", "<h2>Foo</h2>\n<h2>Foo</h2>\n<h1>Foo</h1>");
        }

        // Four spaces indent is too much:
        [TestMethod]
        [TestCategory("Leaf blocks - Setext headers")]
        //[Timeout(1000)]
        public void Example044()
        {
            // Example 44
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

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 44, "Leaf blocks - Setext headers");
			Helpers.ExecuteTest("    Foo\n    ---\n\n    Foo\n---", "<pre><code>Foo\n---\n\nFoo\n</code></pre>\n<hr />");
        }

        // The setext header underline can be indented up to three spaces, and
        // may have trailing spaces:
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
            //        ----      
            //
            // Should be rendered as:
            //     <h2>Foo</h2>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 45, "Leaf blocks - Setext headers");
			Helpers.ExecuteTest("Foo\n   ----      ", "<h2>Foo</h2>");
        }

        // Four spaces is too much:
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
            //         ---
            //
            // Should be rendered as:
            //     <p>Foo
            //     ---</p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 46, "Leaf blocks - Setext headers");
			Helpers.ExecuteTest("Foo\n    ---", "<p>Foo\n---</p>");
        }

        // The setext header underline cannot contain internal spaces:
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

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 47, "Leaf blocks - Setext headers");
			Helpers.ExecuteTest("Foo\n= =\n\nFoo\n--- -", "<p>Foo\n= =</p>\n<p>Foo</p>\n<hr />");
        }

        // Trailing spaces in the content line do not cause a line break:
        [TestMethod]
        [TestCategory("Leaf blocks - Setext headers")]
        //[Timeout(1000)]
        public void Example048()
        {
            // Example 48
            // Section: Leaf blocks - Setext headers
            //
            // The following CommonMark:
            //     Foo  
            //     -----
            //
            // Should be rendered as:
            //     <h2>Foo</h2>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 48, "Leaf blocks - Setext headers");
			Helpers.ExecuteTest("Foo  \n-----", "<h2>Foo</h2>");
        }

        // Nor does a backslash at the end:
        [TestMethod]
        [TestCategory("Leaf blocks - Setext headers")]
        //[Timeout(1000)]
        public void Example049()
        {
            // Example 49
            // Section: Leaf blocks - Setext headers
            //
            // The following CommonMark:
            //     Foo\
            //     ----
            //
            // Should be rendered as:
            //     <h2>Foo\</h2>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 49, "Leaf blocks - Setext headers");
			Helpers.ExecuteTest("Foo\\\n----", "<h2>Foo\\</h2>");
        }

        // Since indicators of block structure take precedence over
        // indicators of inline structure, the following are setext headers:
        [TestMethod]
        [TestCategory("Leaf blocks - Setext headers")]
        //[Timeout(1000)]
        public void Example050()
        {
            // Example 50
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

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 50, "Leaf blocks - Setext headers");
			Helpers.ExecuteTest("`Foo\n----\n`\n\n<a title=\"a lot\n---\nof dashes\"/>", "<h2>`Foo</h2>\n<p>`</p>\n<h2>&lt;a title=&quot;a lot</h2>\n<p>of dashes&quot;/&gt;</p>");
        }

        // The setext header underline cannot be a [lazy continuation
        // line] in a list item or block quote:
        [TestMethod]
        [TestCategory("Leaf blocks - Setext headers")]
        //[Timeout(1000)]
        public void Example051()
        {
            // Example 51
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

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 51, "Leaf blocks - Setext headers");
			Helpers.ExecuteTest("> Foo\n---", "<blockquote>\n<p>Foo</p>\n</blockquote>\n<hr />");
        }

        [TestMethod]
        [TestCategory("Leaf blocks - Setext headers")]
        //[Timeout(1000)]
        public void Example052()
        {
            // Example 52
            // Section: Leaf blocks - Setext headers
            //
            // The following CommonMark:
            //     - Foo
            //     ---
            //
            // Should be rendered as:
            //     <ul>
            //     <li>Foo</li>
            //     </ul>
            //     <hr />

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 52, "Leaf blocks - Setext headers");
			Helpers.ExecuteTest("- Foo\n---", "<ul>\n<li>Foo</li>\n</ul>\n<hr />");
        }

        // A setext header cannot interrupt a paragraph:
        [TestMethod]
        [TestCategory("Leaf blocks - Setext headers")]
        //[Timeout(1000)]
        public void Example053()
        {
            // Example 53
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

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 53, "Leaf blocks - Setext headers");
			Helpers.ExecuteTest("Foo\nBar\n---\n\nFoo\nBar\n===", "<p>Foo\nBar</p>\n<hr />\n<p>Foo\nBar\n===</p>");
        }

        // But in general a blank line is not required before or after:
        [TestMethod]
        [TestCategory("Leaf blocks - Setext headers")]
        //[Timeout(1000)]
        public void Example054()
        {
            // Example 54
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

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 54, "Leaf blocks - Setext headers");
			Helpers.ExecuteTest("---\nFoo\n---\nBar\n---\nBaz", "<hr />\n<h2>Foo</h2>\n<h2>Bar</h2>\n<p>Baz</p>");
        }

        // Setext headers cannot be empty:
        [TestMethod]
        [TestCategory("Leaf blocks - Setext headers")]
        //[Timeout(1000)]
        public void Example055()
        {
            // Example 55
            // Section: Leaf blocks - Setext headers
            //
            // The following CommonMark:
            //     ====
            //
            // Should be rendered as:
            //     <p>====</p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 55, "Leaf blocks - Setext headers");
			Helpers.ExecuteTest("====", "<p>====</p>");
        }

        // Setext header text lines must not be interpretable as block
        // constructs other than paragraphs.  So, the line of dashes
        // in these examples gets interpreted as a horizontal rule:
        [TestMethod]
        [TestCategory("Leaf blocks - Setext headers")]
        //[Timeout(1000)]
        public void Example056()
        {
            // Example 56
            // Section: Leaf blocks - Setext headers
            //
            // The following CommonMark:
            //     ---
            //     ---
            //
            // Should be rendered as:
            //     <hr />
            //     <hr />

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 56, "Leaf blocks - Setext headers");
			Helpers.ExecuteTest("---\n---", "<hr />\n<hr />");
        }

        [TestMethod]
        [TestCategory("Leaf blocks - Setext headers")]
        //[Timeout(1000)]
        public void Example057()
        {
            // Example 57
            // Section: Leaf blocks - Setext headers
            //
            // The following CommonMark:
            //     - foo
            //     -----
            //
            // Should be rendered as:
            //     <ul>
            //     <li>foo</li>
            //     </ul>
            //     <hr />

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 57, "Leaf blocks - Setext headers");
			Helpers.ExecuteTest("- foo\n-----", "<ul>\n<li>foo</li>\n</ul>\n<hr />");
        }

        [TestMethod]
        [TestCategory("Leaf blocks - Setext headers")]
        //[Timeout(1000)]
        public void Example058()
        {
            // Example 58
            // Section: Leaf blocks - Setext headers
            //
            // The following CommonMark:
            //         foo
            //     ---
            //
            // Should be rendered as:
            //     <pre><code>foo
            //     </code></pre>
            //     <hr />

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 58, "Leaf blocks - Setext headers");
			Helpers.ExecuteTest("    foo\n---", "<pre><code>foo\n</code></pre>\n<hr />");
        }

        [TestMethod]
        [TestCategory("Leaf blocks - Setext headers")]
        //[Timeout(1000)]
        public void Example059()
        {
            // Example 59
            // Section: Leaf blocks - Setext headers
            //
            // The following CommonMark:
            //     > foo
            //     -----
            //
            // Should be rendered as:
            //     <blockquote>
            //     <p>foo</p>
            //     </blockquote>
            //     <hr />

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 59, "Leaf blocks - Setext headers");
			Helpers.ExecuteTest("> foo\n-----", "<blockquote>\n<p>foo</p>\n</blockquote>\n<hr />");
        }

        // If you want a header with `> foo` as its literal text, you can
        // use backslash escapes:
        [TestMethod]
        [TestCategory("Leaf blocks - Setext headers")]
        //[Timeout(1000)]
        public void Example060()
        {
            // Example 60
            // Section: Leaf blocks - Setext headers
            //
            // The following CommonMark:
            //     \> foo
            //     ------
            //
            // Should be rendered as:
            //     <h2>&gt; foo</h2>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 60, "Leaf blocks - Setext headers");
			Helpers.ExecuteTest("\\> foo\n------", "<h2>&gt; foo</h2>");
        }

        // ## Indented code blocks
        //
        // An [indented code block](@indented-code-block) is composed of one or more
        // [indented chunk]s separated by blank lines.
        // An [indented chunk](@indented-chunk) is a sequence of non-blank lines,
        // each indented four or more spaces. The contents of the code block are
        // the literal contents of the lines, including trailing
        // [line ending]s, minus four spaces of indentation.
        // An indented code block has no [info string].
        //
        // An indented code block cannot interrupt a paragraph, so there must be
        // a blank line between a paragraph and a following indented code block.
        // (A blank line is not needed, however, between a code block and a following
        // paragraph.)
        [TestMethod]
        [TestCategory("Leaf blocks - Indented code blocks")]
        //[Timeout(1000)]
        public void Example061()
        {
            // Example 61
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

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 61, "Leaf blocks - Indented code blocks");
			Helpers.ExecuteTest("    a simple\n      indented code block", "<pre><code>a simple\n  indented code block\n</code></pre>");
        }

        // The contents are literal text, and do not get parsed as Markdown:
        [TestMethod]
        [TestCategory("Leaf blocks - Indented code blocks")]
        //[Timeout(1000)]
        public void Example062()
        {
            // Example 62
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

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 62, "Leaf blocks - Indented code blocks");
			Helpers.ExecuteTest("    <a/>\n    *hi*\n\n    - one", "<pre><code>&lt;a/&gt;\n*hi*\n\n- one\n</code></pre>");
        }

        // Here we have three chunks separated by blank lines:
        [TestMethod]
        [TestCategory("Leaf blocks - Indented code blocks")]
        //[Timeout(1000)]
        public void Example063()
        {
            // Example 63
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

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 63, "Leaf blocks - Indented code blocks");
			Helpers.ExecuteTest("    chunk1\n\n    chunk2\n  \n \n \n    chunk3", "<pre><code>chunk1\n\nchunk2\n\n\n\nchunk3\n</code></pre>");
        }

        // Any initial spaces beyond four will be included in the content, even
        // in interior blank lines:
        [TestMethod]
        [TestCategory("Leaf blocks - Indented code blocks")]
        //[Timeout(1000)]
        public void Example064()
        {
            // Example 64
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

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 64, "Leaf blocks - Indented code blocks");
			Helpers.ExecuteTest("    chunk1\n      \n      chunk2", "<pre><code>chunk1\n  \n  chunk2\n</code></pre>");
        }

        // An indented code block cannot interrupt a paragraph.  (This
        // allows hanging indents and the like.)
        [TestMethod]
        [TestCategory("Leaf blocks - Indented code blocks")]
        //[Timeout(1000)]
        public void Example065()
        {
            // Example 65
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

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 65, "Leaf blocks - Indented code blocks");
			Helpers.ExecuteTest("Foo\n    bar\n", "<p>Foo\nbar</p>");
        }

        // However, any non-blank line with fewer than four leading spaces ends
        // the code block immediately.  So a paragraph may occur immediately
        // after indented code:
        [TestMethod]
        [TestCategory("Leaf blocks - Indented code blocks")]
        //[Timeout(1000)]
        public void Example066()
        {
            // Example 66
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

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 66, "Leaf blocks - Indented code blocks");
			Helpers.ExecuteTest("    foo\nbar", "<pre><code>foo\n</code></pre>\n<p>bar</p>");
        }

        // And indented code can occur immediately before and after other kinds of
        // blocks:
        [TestMethod]
        [TestCategory("Leaf blocks - Indented code blocks")]
        //[Timeout(1000)]
        public void Example067()
        {
            // Example 67
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

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 67, "Leaf blocks - Indented code blocks");
			Helpers.ExecuteTest("# Header\n    foo\nHeader\n------\n    foo\n----", "<h1>Header</h1>\n<pre><code>foo\n</code></pre>\n<h2>Header</h2>\n<pre><code>foo\n</code></pre>\n<hr />");
        }

        // The first line can be indented more than four spaces:
        [TestMethod]
        [TestCategory("Leaf blocks - Indented code blocks")]
        //[Timeout(1000)]
        public void Example068()
        {
            // Example 68
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

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 68, "Leaf blocks - Indented code blocks");
			Helpers.ExecuteTest("        foo\n    bar", "<pre><code>    foo\nbar\n</code></pre>");
        }

        // Blank lines preceding or following an indented code block
        // are not included in it:
        [TestMethod]
        [TestCategory("Leaf blocks - Indented code blocks")]
        //[Timeout(1000)]
        public void Example069()
        {
            // Example 69
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

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 69, "Leaf blocks - Indented code blocks");
			Helpers.ExecuteTest("    \n    foo\n    \n", "<pre><code>foo\n</code></pre>");
        }

        // Trailing spaces are included in the code block's content:
        [TestMethod]
        [TestCategory("Leaf blocks - Indented code blocks")]
        //[Timeout(1000)]
        public void Example070()
        {
            // Example 70
            // Section: Leaf blocks - Indented code blocks
            //
            // The following CommonMark:
            //         foo  
            //
            // Should be rendered as:
            //     <pre><code>foo  
            //     </code></pre>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 70, "Leaf blocks - Indented code blocks");
			Helpers.ExecuteTest("    foo  ", "<pre><code>foo  \n</code></pre>");
        }

        // ## Fenced code blocks
        //
        // A [code fence](@code-fence) is a sequence
        // of at least three consecutive backtick characters (`` ` ``) or
        // tildes (`~`).  (Tildes and backticks cannot be mixed.)
        // A [fenced code block](@fenced-code-block)
        // begins with a code fence, indented no more than three spaces.
        //
        // The line with the opening code fence may optionally contain some text
        // following the code fence; this is trimmed of leading and trailing
        // spaces and called the [info string](@info-string).
        // The [info string] may not contain any backtick
        // characters.  (The reason for this restriction is that otherwise
        // some inline code would be incorrectly interpreted as the
        // beginning of a fenced code block.)
        //
        // The content of the code block consists of all subsequent lines, until
        // a closing [code fence] of the same type as the code block
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
        // as inlines.  The first word of the [info string] is typically used to
        // specify the language of the code sample, and rendered in the `class`
        // attribute of the `code` tag.  However, this spec does not mandate any
        // particular treatment of the [info string].
        //
        // Here is a simple example with backticks:
        [TestMethod]
        [TestCategory("Leaf blocks - Fenced code blocks")]
        //[Timeout(1000)]
        public void Example071()
        {
            // Example 71
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

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 71, "Leaf blocks - Fenced code blocks");
			Helpers.ExecuteTest("```\n<\n >\n```", "<pre><code>&lt;\n &gt;\n</code></pre>");
        }

        // With tildes:
        [TestMethod]
        [TestCategory("Leaf blocks - Fenced code blocks")]
        //[Timeout(1000)]
        public void Example072()
        {
            // Example 72
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

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 72, "Leaf blocks - Fenced code blocks");
			Helpers.ExecuteTest("~~~\n<\n >\n~~~", "<pre><code>&lt;\n &gt;\n</code></pre>");
        }

        // The closing code fence must use the same character as the opening
        // fence:
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
            //     aaa
            //     ~~~
            //     ```
            //
            // Should be rendered as:
            //     <pre><code>aaa
            //     ~~~
            //     </code></pre>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 73, "Leaf blocks - Fenced code blocks");
			Helpers.ExecuteTest("```\naaa\n~~~\n```", "<pre><code>aaa\n~~~\n</code></pre>");
        }

        [TestMethod]
        [TestCategory("Leaf blocks - Fenced code blocks")]
        //[Timeout(1000)]
        public void Example074()
        {
            // Example 74
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

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 74, "Leaf blocks - Fenced code blocks");
			Helpers.ExecuteTest("~~~\naaa\n```\n~~~", "<pre><code>aaa\n```\n</code></pre>");
        }

        // The closing code fence must be at least as long as the opening fence:
        [TestMethod]
        [TestCategory("Leaf blocks - Fenced code blocks")]
        //[Timeout(1000)]
        public void Example075()
        {
            // Example 75
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

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 75, "Leaf blocks - Fenced code blocks");
			Helpers.ExecuteTest("````\naaa\n```\n``````", "<pre><code>aaa\n```\n</code></pre>");
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
            //     ~~~~
            //     aaa
            //     ~~~
            //     ~~~~
            //
            // Should be rendered as:
            //     <pre><code>aaa
            //     ~~~
            //     </code></pre>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 76, "Leaf blocks - Fenced code blocks");
			Helpers.ExecuteTest("~~~~\naaa\n~~~\n~~~~", "<pre><code>aaa\n~~~\n</code></pre>");
        }

        // Unclosed code blocks are closed by the end of the document:
        [TestMethod]
        [TestCategory("Leaf blocks - Fenced code blocks")]
        //[Timeout(1000)]
        public void Example077()
        {
            // Example 77
            // Section: Leaf blocks - Fenced code blocks
            //
            // The following CommonMark:
            //     ```
            //
            // Should be rendered as:
            //     <pre><code></code></pre>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 77, "Leaf blocks - Fenced code blocks");
			Helpers.ExecuteTest("```", "<pre><code></code></pre>");
        }

        [TestMethod]
        [TestCategory("Leaf blocks - Fenced code blocks")]
        //[Timeout(1000)]
        public void Example078()
        {
            // Example 78
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

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 78, "Leaf blocks - Fenced code blocks");
			Helpers.ExecuteTest("`````\n\n```\naaa", "<pre><code>\n```\naaa\n</code></pre>");
        }

        // A code block can have all empty lines as its content:
        [TestMethod]
        [TestCategory("Leaf blocks - Fenced code blocks")]
        //[Timeout(1000)]
        public void Example079()
        {
            // Example 79
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

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 79, "Leaf blocks - Fenced code blocks");
			Helpers.ExecuteTest("```\n\n  \n```", "<pre><code>\n  \n</code></pre>");
        }

        // A code block can be empty:
        [TestMethod]
        [TestCategory("Leaf blocks - Fenced code blocks")]
        //[Timeout(1000)]
        public void Example080()
        {
            // Example 80
            // Section: Leaf blocks - Fenced code blocks
            //
            // The following CommonMark:
            //     ```
            //     ```
            //
            // Should be rendered as:
            //     <pre><code></code></pre>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 80, "Leaf blocks - Fenced code blocks");
			Helpers.ExecuteTest("```\n```", "<pre><code></code></pre>");
        }

        // Fences can be indented.  If the opening fence is indented,
        // content lines will have equivalent opening indentation removed,
        // if present:
        [TestMethod]
        [TestCategory("Leaf blocks - Fenced code blocks")]
        //[Timeout(1000)]
        public void Example081()
        {
            // Example 81
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

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 81, "Leaf blocks - Fenced code blocks");
			Helpers.ExecuteTest(" ```\n aaa\naaa\n```", "<pre><code>aaa\naaa\n</code></pre>");
        }

        [TestMethod]
        [TestCategory("Leaf blocks - Fenced code blocks")]
        //[Timeout(1000)]
        public void Example082()
        {
            // Example 82
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

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 82, "Leaf blocks - Fenced code blocks");
			Helpers.ExecuteTest("  ```\naaa\n  aaa\naaa\n  ```", "<pre><code>aaa\naaa\naaa\n</code></pre>");
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

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 83, "Leaf blocks - Fenced code blocks");
			Helpers.ExecuteTest("   ```\n   aaa\n    aaa\n  aaa\n   ```", "<pre><code>aaa\n aaa\naaa\n</code></pre>");
        }

        // Four spaces indentation produces an indented code block:
        [TestMethod]
        [TestCategory("Leaf blocks - Fenced code blocks")]
        //[Timeout(1000)]
        public void Example084()
        {
            // Example 84
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

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 84, "Leaf blocks - Fenced code blocks");
			Helpers.ExecuteTest("    ```\n    aaa\n    ```", "<pre><code>```\naaa\n```\n</code></pre>");
        }

        // Closing fences may be indented by 0-3 spaces, and their indentation
        // need not match that of the opening fence:
        [TestMethod]
        [TestCategory("Leaf blocks - Fenced code blocks")]
        //[Timeout(1000)]
        public void Example085()
        {
            // Example 85
            // Section: Leaf blocks - Fenced code blocks
            //
            // The following CommonMark:
            //     ```
            //     aaa
            //       ```
            //
            // Should be rendered as:
            //     <pre><code>aaa
            //     </code></pre>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 85, "Leaf blocks - Fenced code blocks");
			Helpers.ExecuteTest("```\naaa\n  ```", "<pre><code>aaa\n</code></pre>");
        }

        [TestMethod]
        [TestCategory("Leaf blocks - Fenced code blocks")]
        //[Timeout(1000)]
        public void Example086()
        {
            // Example 86
            // Section: Leaf blocks - Fenced code blocks
            //
            // The following CommonMark:
            //        ```
            //     aaa
            //       ```
            //
            // Should be rendered as:
            //     <pre><code>aaa
            //     </code></pre>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 86, "Leaf blocks - Fenced code blocks");
			Helpers.ExecuteTest("   ```\naaa\n  ```", "<pre><code>aaa\n</code></pre>");
        }

        // This is not a closing fence, because it is indented 4 spaces:
        [TestMethod]
        [TestCategory("Leaf blocks - Fenced code blocks")]
        //[Timeout(1000)]
        public void Example087()
        {
            // Example 87
            // Section: Leaf blocks - Fenced code blocks
            //
            // The following CommonMark:
            //     ```
            //     aaa
            //         ```
            //
            // Should be rendered as:
            //     <pre><code>aaa
            //         ```
            //     </code></pre>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 87, "Leaf blocks - Fenced code blocks");
			Helpers.ExecuteTest("```\naaa\n    ```", "<pre><code>aaa\n    ```\n</code></pre>");
        }

        // Code fences (opening and closing) cannot contain internal spaces:
        [TestMethod]
        [TestCategory("Leaf blocks - Fenced code blocks")]
        //[Timeout(1000)]
        public void Example088()
        {
            // Example 88
            // Section: Leaf blocks - Fenced code blocks
            //
            // The following CommonMark:
            //     ``` ```
            //     aaa
            //
            // Should be rendered as:
            //     <p><code></code>
            //     aaa</p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 88, "Leaf blocks - Fenced code blocks");
			Helpers.ExecuteTest("``` ```\naaa", "<p><code></code>\naaa</p>");
        }

        [TestMethod]
        [TestCategory("Leaf blocks - Fenced code blocks")]
        //[Timeout(1000)]
        public void Example089()
        {
            // Example 89
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

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 89, "Leaf blocks - Fenced code blocks");
			Helpers.ExecuteTest("~~~~~~\naaa\n~~~ ~~", "<pre><code>aaa\n~~~ ~~\n</code></pre>");
        }

        // Fenced code blocks can interrupt paragraphs, and can be followed
        // directly by paragraphs, without a blank line between:
        [TestMethod]
        [TestCategory("Leaf blocks - Fenced code blocks")]
        //[Timeout(1000)]
        public void Example090()
        {
            // Example 90
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

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 90, "Leaf blocks - Fenced code blocks");
			Helpers.ExecuteTest("foo\n```\nbar\n```\nbaz", "<p>foo</p>\n<pre><code>bar\n</code></pre>\n<p>baz</p>");
        }

        // Other blocks can also occur before and after fenced code blocks
        // without an intervening blank line:
        [TestMethod]
        [TestCategory("Leaf blocks - Fenced code blocks")]
        //[Timeout(1000)]
        public void Example091()
        {
            // Example 91
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

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 91, "Leaf blocks - Fenced code blocks");
			Helpers.ExecuteTest("foo\n---\n~~~\nbar\n~~~\n# baz", "<h2>foo</h2>\n<pre><code>bar\n</code></pre>\n<h1>baz</h1>");
        }

        // An [info string] can be provided after the opening code fence.
        // Opening and closing spaces will be stripped, and the first word, prefixed
        // with `language-`, is used as the value for the `class` attribute of the
        // `code` element within the enclosing `pre` element.
        [TestMethod]
        [TestCategory("Leaf blocks - Fenced code blocks")]
        //[Timeout(1000)]
        public void Example092()
        {
            // Example 92
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

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 92, "Leaf blocks - Fenced code blocks");
			Helpers.ExecuteTest("```ruby\ndef foo(x)\n  return 3\nend\n```", "<pre><code class=\"language-ruby\">def foo(x)\n  return 3\nend\n</code></pre>");
        }

        [TestMethod]
        [TestCategory("Leaf blocks - Fenced code blocks")]
        //[Timeout(1000)]
        public void Example093()
        {
            // Example 93
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

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 93, "Leaf blocks - Fenced code blocks");
			Helpers.ExecuteTest("~~~~    ruby startline=3 $%@#$\ndef foo(x)\n  return 3\nend\n~~~~~~~", "<pre><code class=\"language-ruby\">def foo(x)\n  return 3\nend\n</code></pre>");
        }

        [TestMethod]
        [TestCategory("Leaf blocks - Fenced code blocks")]
        //[Timeout(1000)]
        public void Example094()
        {
            // Example 94
            // Section: Leaf blocks - Fenced code blocks
            //
            // The following CommonMark:
            //     ````;
            //     ````
            //
            // Should be rendered as:
            //     <pre><code class="language-;"></code></pre>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 94, "Leaf blocks - Fenced code blocks");
			Helpers.ExecuteTest("````;\n````", "<pre><code class=\"language-;\"></code></pre>");
        }

        // [Info string]s for backtick code blocks cannot contain backticks:
        [TestMethod]
        [TestCategory("Leaf blocks - Fenced code blocks")]
        //[Timeout(1000)]
        public void Example095()
        {
            // Example 95
            // Section: Leaf blocks - Fenced code blocks
            //
            // The following CommonMark:
            //     ``` aa ```
            //     foo
            //
            // Should be rendered as:
            //     <p><code>aa</code>
            //     foo</p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 95, "Leaf blocks - Fenced code blocks");
			Helpers.ExecuteTest("``` aa ```\nfoo", "<p><code>aa</code>\nfoo</p>");
        }

        // Closing code fences cannot have [info string]s:
        [TestMethod]
        [TestCategory("Leaf blocks - Fenced code blocks")]
        //[Timeout(1000)]
        public void Example096()
        {
            // Example 96
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

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 96, "Leaf blocks - Fenced code blocks");
			Helpers.ExecuteTest("```\n``` aaa\n```", "<pre><code>``` aaa\n</code></pre>");
        }

        // ## HTML blocks
        //
        // An [HTML block tag](@html-block-tag) is
        // an [open tag] or [closing tag] whose tag
        // name is one of the following (case-insensitive):
        // `article`, `header`, `aside`, `hgroup`, `blockquote`, `hr`, `iframe`,
        // `body`, `li`, `map`, `button`, `object`, `canvas`, `ol`, `caption`,
        // `output`, `col`, `p`, `colgroup`, `pre`, `dd`, `progress`, `div`,
        // `section`, `dl`, `table`, `td`, `dt`, `tbody`, `embed`, `textarea`,
        // `fieldset`, `tfoot`, `figcaption`, `th`, `figure`, `thead`, `footer`,
        // `tr`, `form`, `ul`, `h1`, `h2`, `h3`, `h4`, `h5`, `h6`, `video`,
        // `script`, `style`.
        //
        // An [HTML block](@html-block) begins with an
        // [HTML block tag], [HTML comment], [processing instruction],
        // [declaration], or [CDATA section].
        // It ends when a [blank line] or the end of the
        // input is encountered.  The initial line may be indented up to three
        // spaces, and subsequent lines may have any indentation.  The contents
        // of the HTML block are interpreted as raw HTML, and will not be escaped
        // in HTML output.
        //
        // Some simple examples:
        [TestMethod]
        [TestCategory("Leaf blocks - HTML blocks")]
        //[Timeout(1000)]
        public void Example097()
        {
            // Example 97
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

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 97, "Leaf blocks - HTML blocks");
			Helpers.ExecuteTest("<table>\n  <tr>\n    <td>\n           hi\n    </td>\n  </tr>\n</table>\n\nokay.", "<table>\n  <tr>\n    <td>\n           hi\n    </td>\n  </tr>\n</table>\n<p>okay.</p>");
        }

        [TestMethod]
        [TestCategory("Leaf blocks - HTML blocks")]
        //[Timeout(1000)]
        public void Example098()
        {
            // Example 98
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

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 98, "Leaf blocks - HTML blocks");
			Helpers.ExecuteTest(" <div>\n  *hello*\n         <foo><a>", " <div>\n  *hello*\n         <foo><a>");
        }

        // Here we have two HTML blocks with a Markdown paragraph between them:
        [TestMethod]
        [TestCategory("Leaf blocks - HTML blocks")]
        //[Timeout(1000)]
        public void Example099()
        {
            // Example 99
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

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 99, "Leaf blocks - HTML blocks");
			Helpers.ExecuteTest("<DIV CLASS=\"foo\">\n\n*Markdown*\n\n</DIV>", "<DIV CLASS=\"foo\">\n<p><em>Markdown</em></p>\n</DIV>");
        }

        // In the following example, what looks like a Markdown code block
        // is actually part of the HTML block, which continues until a blank
        // line or the end of the document is reached:
        [TestMethod]
        [TestCategory("Leaf blocks - HTML blocks")]
        //[Timeout(1000)]
        public void Example100()
        {
            // Example 100
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

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 100, "Leaf blocks - HTML blocks");
			Helpers.ExecuteTest("<div></div>\n``` c\nint x = 33;\n```", "<div></div>\n``` c\nint x = 33;\n```");
        }

        // A comment:
        [TestMethod]
        [TestCategory("Leaf blocks - HTML blocks")]
        //[Timeout(1000)]
        public void Example101()
        {
            // Example 101
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

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 101, "Leaf blocks - HTML blocks");
			Helpers.ExecuteTest("<!-- Foo\nbar\n   baz -->", "<!-- Foo\nbar\n   baz -->");
        }

        // A processing instruction:
        [TestMethod]
        [TestCategory("Leaf blocks - HTML blocks")]
        //[Timeout(1000)]
        public void Example102()
        {
            // Example 102
            // Section: Leaf blocks - HTML blocks
            //
            // The following CommonMark:
            //     <?php
            //       echo '>';
            //     ?>
            //
            // Should be rendered as:
            //     <?php
            //       echo '>';
            //     ?>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 102, "Leaf blocks - HTML blocks");
			Helpers.ExecuteTest("<?php\n  echo '>';\n?>", "<?php\n  echo '>';\n?>");
        }

        // CDATA:
        [TestMethod]
        [TestCategory("Leaf blocks - HTML blocks")]
        //[Timeout(1000)]
        public void Example103()
        {
            // Example 103
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

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 103, "Leaf blocks - HTML blocks");
			Helpers.ExecuteTest("<![CDATA[\nfunction matchwo(a,b)\n{\nif (a < b && a < 0) then\n  {\n  return 1;\n  }\nelse\n  {\n  return 0;\n  }\n}\n]]>", "<![CDATA[\nfunction matchwo(a,b)\n{\nif (a < b && a < 0) then\n  {\n  return 1;\n  }\nelse\n  {\n  return 0;\n  }\n}\n]]>");
        }

        // The opening tag can be indented 1-3 spaces, but not 4:
        [TestMethod]
        [TestCategory("Leaf blocks - HTML blocks")]
        //[Timeout(1000)]
        public void Example104()
        {
            // Example 104
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

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 104, "Leaf blocks - HTML blocks");
			Helpers.ExecuteTest("  <!-- foo -->\n\n    <!-- foo -->", "  <!-- foo -->\n<pre><code>&lt;!-- foo --&gt;\n</code></pre>");
        }

        // An HTML block can interrupt a paragraph, and need not be preceded
        // by a blank line.
        [TestMethod]
        [TestCategory("Leaf blocks - HTML blocks")]
        //[Timeout(1000)]
        public void Example105()
        {
            // Example 105
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

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 105, "Leaf blocks - HTML blocks");
			Helpers.ExecuteTest("Foo\n<div>\nbar\n</div>", "<p>Foo</p>\n<div>\nbar\n</div>");
        }

        // However, a following blank line is always needed, except at the end of
        // a document:
        [TestMethod]
        [TestCategory("Leaf blocks - HTML blocks")]
        //[Timeout(1000)]
        public void Example106()
        {
            // Example 106
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

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 106, "Leaf blocks - HTML blocks");
			Helpers.ExecuteTest("<div>\nbar\n</div>\n*foo*", "<div>\nbar\n</div>\n*foo*");
        }

        // An incomplete HTML block tag may also start an HTML block:
        [TestMethod]
        [TestCategory("Leaf blocks - HTML blocks")]
        //[Timeout(1000)]
        public void Example107()
        {
            // Example 107
            // Section: Leaf blocks - HTML blocks
            //
            // The following CommonMark:
            //     <div class
            //     foo
            //
            // Should be rendered as:
            //     <div class
            //     foo

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 107, "Leaf blocks - HTML blocks");
			Helpers.ExecuteTest("<div class\nfoo", "<div class\nfoo");
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
        public void Example108()
        {
            // Example 108
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

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 108, "Leaf blocks - HTML blocks");
			Helpers.ExecuteTest("<div>\n\n*Emphasized* text.\n\n</div>", "<div>\n<p><em>Emphasized</em> text.</p>\n</div>");
        }

        // Compare:
        [TestMethod]
        [TestCategory("Leaf blocks - HTML blocks")]
        //[Timeout(1000)]
        public void Example109()
        {
            // Example 109
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

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 109, "Leaf blocks - HTML blocks");
			Helpers.ExecuteTest("<div>\n*Emphasized* text.\n</div>", "<div>\n*Emphasized* text.\n</div>");
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
        public void Example110()
        {
            // Example 110
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

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 110, "Leaf blocks - HTML blocks");
			Helpers.ExecuteTest("<table>\n\n<tr>\n\n<td>\nHi\n</td>\n\n</tr>\n\n</table>", "<table>\n<tr>\n<td>\nHi\n</td>\n</tr>\n</table>");
        }

        // Moreover, blank lines are usually not necessary and can be
        // deleted.  The exception is inside `<pre>` tags; here, one can
        // replace the blank lines with `&#10;` entities.
        //
        // So there is no important loss of expressive power with the new rule.
        //
        // ## Link reference definitions
        //
        // A [link reference definition](@link-reference-definition)
        // consists of a [link label], indented up to three spaces, followed
        // by a colon (`:`), optional [whitespace] (including up to one
        // [line ending]), a [link destination],
        // optional [whitespace] (including up to one
        // [line ending]), and an optional [link
        // title], which if it is present must be separated
        // from the [link destination] by [whitespace].
        // No further [non-space character]s may occur on the line.
        //
        // A [link reference-definition]
        // does not correspond to a structural element of a document.  Instead, it
        // defines a label which can be used in [reference link]s
        // and reference-style [images] elsewhere in the document.  [Link
        // reference definitions] can come either before or after the links that use
        // them.
        [TestMethod]
        [TestCategory("Leaf blocks - Link reference definitions")]
        //[Timeout(1000)]
        public void Example111()
        {
            // Example 111
            // Section: Leaf blocks - Link reference definitions
            //
            // The following CommonMark:
            //     [foo]: /url "title"
            //     
            //     [foo]
            //
            // Should be rendered as:
            //     <p><a href="/url" title="title">foo</a></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 111, "Leaf blocks - Link reference definitions");
			Helpers.ExecuteTest("[foo]: /url \"title\"\n\n[foo]", "<p><a href=\"/url\" title=\"title\">foo</a></p>");
        }

        [TestMethod]
        [TestCategory("Leaf blocks - Link reference definitions")]
        //[Timeout(1000)]
        public void Example112()
        {
            // Example 112
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

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 112, "Leaf blocks - Link reference definitions");
			Helpers.ExecuteTest("   [foo]: \n      /url  \n           'the title'  \n\n[foo]", "<p><a href=\"/url\" title=\"the title\">foo</a></p>");
        }

        [TestMethod]
        [TestCategory("Leaf blocks - Link reference definitions")]
        //[Timeout(1000)]
        public void Example113()
        {
            // Example 113
            // Section: Leaf blocks - Link reference definitions
            //
            // The following CommonMark:
            //     [Foo*bar\]]:my_(url) 'title (with parens)'
            //     
            //     [Foo*bar\]]
            //
            // Should be rendered as:
            //     <p><a href="my_(url)" title="title (with parens)">Foo*bar]</a></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 113, "Leaf blocks - Link reference definitions");
			Helpers.ExecuteTest("[Foo*bar\\]]:my_(url) 'title (with parens)'\n\n[Foo*bar\\]]", "<p><a href=\"my_(url)\" title=\"title (with parens)\">Foo*bar]</a></p>");
        }

        [TestMethod]
        [TestCategory("Leaf blocks - Link reference definitions")]
        //[Timeout(1000)]
        public void Example114()
        {
            // Example 114
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
            //     <p><a href="my%20url" title="title">Foo bar</a></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 114, "Leaf blocks - Link reference definitions");
			Helpers.ExecuteTest("[Foo bar]:\n<my url>\n'title'\n\n[Foo bar]", "<p><a href=\"my%20url\" title=\"title\">Foo bar</a></p>");
        }

        // The title may extend over multiple lines:
        [TestMethod]
        [TestCategory("Leaf blocks - Link reference definitions")]
        //[Timeout(1000)]
        public void Example115()
        {
            // Example 115
            // Section: Leaf blocks - Link reference definitions
            //
            // The following CommonMark:
            //     [foo]: /url '
            //     title
            //     line1
            //     line2
            //     '
            //     
            //     [foo]
            //
            // Should be rendered as:
            //     <p><a href="/url" title="
            //     title
            //     line1
            //     line2
            //     ">foo</a></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 115, "Leaf blocks - Link reference definitions");
			Helpers.ExecuteTest("[foo]: /url '\ntitle\nline1\nline2\n'\n\n[foo]", "<p><a href=\"/url\" title=\"\ntitle\nline1\nline2\n\">foo</a></p>");
        }

        // However, it may not contain a [blank line]:
        [TestMethod]
        [TestCategory("Leaf blocks - Link reference definitions")]
        //[Timeout(1000)]
        public void Example116()
        {
            // Example 116
            // Section: Leaf blocks - Link reference definitions
            //
            // The following CommonMark:
            //     [foo]: /url 'title
            //     
            //     with blank line'
            //     
            //     [foo]
            //
            // Should be rendered as:
            //     <p>[foo]: /url 'title</p>
            //     <p>with blank line'</p>
            //     <p>[foo]</p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 116, "Leaf blocks - Link reference definitions");
			Helpers.ExecuteTest("[foo]: /url 'title\n\nwith blank line'\n\n[foo]", "<p>[foo]: /url 'title</p>\n<p>with blank line'</p>\n<p>[foo]</p>");
        }

        // The title may be omitted:
        [TestMethod]
        [TestCategory("Leaf blocks - Link reference definitions")]
        //[Timeout(1000)]
        public void Example117()
        {
            // Example 117
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

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 117, "Leaf blocks - Link reference definitions");
			Helpers.ExecuteTest("[foo]:\n/url\n\n[foo]", "<p><a href=\"/url\">foo</a></p>");
        }

        // The link destination may not be omitted:
        [TestMethod]
        [TestCategory("Leaf blocks - Link reference definitions")]
        //[Timeout(1000)]
        public void Example118()
        {
            // Example 118
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

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 118, "Leaf blocks - Link reference definitions");
			Helpers.ExecuteTest("[foo]:\n\n[foo]", "<p>[foo]:</p>\n<p>[foo]</p>");
        }

        // A link can come before its corresponding definition:
        [TestMethod]
        [TestCategory("Leaf blocks - Link reference definitions")]
        //[Timeout(1000)]
        public void Example119()
        {
            // Example 119
            // Section: Leaf blocks - Link reference definitions
            //
            // The following CommonMark:
            //     [foo]
            //     
            //     [foo]: url
            //
            // Should be rendered as:
            //     <p><a href="url">foo</a></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 119, "Leaf blocks - Link reference definitions");
			Helpers.ExecuteTest("[foo]\n\n[foo]: url", "<p><a href=\"url\">foo</a></p>");
        }

        // If there are several matching definitions, the first one takes
        // precedence:
        [TestMethod]
        [TestCategory("Leaf blocks - Link reference definitions")]
        //[Timeout(1000)]
        public void Example120()
        {
            // Example 120
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

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 120, "Leaf blocks - Link reference definitions");
			Helpers.ExecuteTest("[foo]\n\n[foo]: first\n[foo]: second", "<p><a href=\"first\">foo</a></p>");
        }

        // As noted in the section on [Links], matching of labels is
        // case-insensitive (see [matches]).
        [TestMethod]
        [TestCategory("Leaf blocks - Link reference definitions")]
        //[Timeout(1000)]
        public void Example121()
        {
            // Example 121
            // Section: Leaf blocks - Link reference definitions
            //
            // The following CommonMark:
            //     [FOO]: /url
            //     
            //     [Foo]
            //
            // Should be rendered as:
            //     <p><a href="/url">Foo</a></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 121, "Leaf blocks - Link reference definitions");
			Helpers.ExecuteTest("[FOO]: /url\n\n[Foo]", "<p><a href=\"/url\">Foo</a></p>");
        }

        [TestMethod]
        [TestCategory("Leaf blocks - Link reference definitions")]
        //[Timeout(1000)]
        public void Example122()
        {
            // Example 122
            // Section: Leaf blocks - Link reference definitions
            //
            // The following CommonMark:
            //     [ΑΓΩ]: /φου
            //     
            //     [αγω]
            //
            // Should be rendered as:
            //     <p><a href="/%CF%86%CE%BF%CF%85">αγω</a></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 122, "Leaf blocks - Link reference definitions");
			Helpers.ExecuteTest("[ΑΓΩ]: /φου\n\n[αγω]", "<p><a href=\"/%CF%86%CE%BF%CF%85\">αγω</a></p>");
        }

        // Here is a link reference definition with no corresponding link.
        // It contributes nothing to the document.
        [TestMethod]
        [TestCategory("Leaf blocks - Link reference definitions")]
        //[Timeout(1000)]
        public void Example123()
        {
            // Example 123
            // Section: Leaf blocks - Link reference definitions
            //
            // The following CommonMark:
            //     [foo]: /url
            //
            // Should be rendered as:
            //     

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 123, "Leaf blocks - Link reference definitions");
			Helpers.ExecuteTest("[foo]: /url", "");
        }

        // Here is another one:
        [TestMethod]
        [TestCategory("Leaf blocks - Link reference definitions")]
        //[Timeout(1000)]
        public void Example124()
        {
            // Example 124
            // Section: Leaf blocks - Link reference definitions
            //
            // The following CommonMark:
            //     [
            //     foo
            //     ]: /url
            //     bar
            //
            // Should be rendered as:
            //     <p>bar</p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 124, "Leaf blocks - Link reference definitions");
			Helpers.ExecuteTest("[\nfoo\n]: /url\nbar", "<p>bar</p>");
        }

        // This is not a link reference definition, because there are
        // [non-space character]s after the title:
        [TestMethod]
        [TestCategory("Leaf blocks - Link reference definitions")]
        //[Timeout(1000)]
        public void Example125()
        {
            // Example 125
            // Section: Leaf blocks - Link reference definitions
            //
            // The following CommonMark:
            //     [foo]: /url "title" ok
            //
            // Should be rendered as:
            //     <p>[foo]: /url &quot;title&quot; ok</p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 125, "Leaf blocks - Link reference definitions");
			Helpers.ExecuteTest("[foo]: /url \"title\" ok", "<p>[foo]: /url &quot;title&quot; ok</p>");
        }

        // This is not a link reference definition, because it is indented
        // four spaces:
        [TestMethod]
        [TestCategory("Leaf blocks - Link reference definitions")]
        //[Timeout(1000)]
        public void Example126()
        {
            // Example 126
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

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 126, "Leaf blocks - Link reference definitions");
			Helpers.ExecuteTest("    [foo]: /url \"title\"\n\n[foo]", "<pre><code>[foo]: /url &quot;title&quot;\n</code></pre>\n<p>[foo]</p>");
        }

        // This is not a link reference definition, because it occurs inside
        // a code block:
        [TestMethod]
        [TestCategory("Leaf blocks - Link reference definitions")]
        //[Timeout(1000)]
        public void Example127()
        {
            // Example 127
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

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 127, "Leaf blocks - Link reference definitions");
			Helpers.ExecuteTest("```\n[foo]: /url\n```\n\n[foo]", "<pre><code>[foo]: /url\n</code></pre>\n<p>[foo]</p>");
        }

        // A [link reference definition] cannot interrupt a paragraph.
        [TestMethod]
        [TestCategory("Leaf blocks - Link reference definitions")]
        //[Timeout(1000)]
        public void Example128()
        {
            // Example 128
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

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 128, "Leaf blocks - Link reference definitions");
			Helpers.ExecuteTest("Foo\n[bar]: /baz\n\n[bar]", "<p>Foo\n[bar]: /baz</p>\n<p>[bar]</p>");
        }

        // However, it can directly follow other block elements, such as headers
        // and horizontal rules, and it need not be followed by a blank line.
        [TestMethod]
        [TestCategory("Leaf blocks - Link reference definitions")]
        //[Timeout(1000)]
        public void Example129()
        {
            // Example 129
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

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 129, "Leaf blocks - Link reference definitions");
			Helpers.ExecuteTest("# [Foo]\n[foo]: /url\n> bar", "<h1><a href=\"/url\">Foo</a></h1>\n<blockquote>\n<p>bar</p>\n</blockquote>");
        }

        // Several [link reference definition]s
        // can occur one after another, without intervening blank lines.
        [TestMethod]
        [TestCategory("Leaf blocks - Link reference definitions")]
        //[Timeout(1000)]
        public void Example130()
        {
            // Example 130
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

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 130, "Leaf blocks - Link reference definitions");
			Helpers.ExecuteTest("[foo]: /foo-url \"foo\"\n[bar]: /bar-url\n  \"bar\"\n[baz]: /baz-url\n\n[foo],\n[bar],\n[baz]", "<p><a href=\"/foo-url\" title=\"foo\">foo</a>,\n<a href=\"/bar-url\" title=\"bar\">bar</a>,\n<a href=\"/baz-url\">baz</a></p>");
        }

        // [Link reference definition]s can occur
        // inside block containers, like lists and block quotations.  They
        // affect the entire document, not just the container in which they
        // are defined:
        [TestMethod]
        [TestCategory("Leaf blocks - Link reference definitions")]
        //[Timeout(1000)]
        public void Example131()
        {
            // Example 131
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

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 131, "Leaf blocks - Link reference definitions");
			Helpers.ExecuteTest("[foo]\n\n> [foo]: /url", "<p><a href=\"/url\">foo</a></p>\n<blockquote>\n</blockquote>");
        }

        // ## Paragraphs
        //
        // A sequence of non-blank lines that cannot be interpreted as other
        // kinds of blocks forms a [paragraph](@paragraph).
        // The contents of the paragraph are the result of parsing the
        // paragraph's raw content as inlines.  The paragraph's raw content
        // is formed by concatenating the lines and removing initial and final
        // [whitespace].
        //
        // A simple example with two paragraphs:
        [TestMethod]
        [TestCategory("Leaf blocks - Paragraphs")]
        //[Timeout(1000)]
        public void Example132()
        {
            // Example 132
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

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 132, "Leaf blocks - Paragraphs");
			Helpers.ExecuteTest("aaa\n\nbbb", "<p>aaa</p>\n<p>bbb</p>");
        }

        // Paragraphs can contain multiple lines, but no blank lines:
        [TestMethod]
        [TestCategory("Leaf blocks - Paragraphs")]
        //[Timeout(1000)]
        public void Example133()
        {
            // Example 133
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

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 133, "Leaf blocks - Paragraphs");
			Helpers.ExecuteTest("aaa\nbbb\n\nccc\nddd", "<p>aaa\nbbb</p>\n<p>ccc\nddd</p>");
        }

        // Multiple blank lines between paragraph have no effect:
        [TestMethod]
        [TestCategory("Leaf blocks - Paragraphs")]
        //[Timeout(1000)]
        public void Example134()
        {
            // Example 134
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

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 134, "Leaf blocks - Paragraphs");
			Helpers.ExecuteTest("aaa\n\n\nbbb", "<p>aaa</p>\n<p>bbb</p>");
        }

        // Leading spaces are skipped:
        [TestMethod]
        [TestCategory("Leaf blocks - Paragraphs")]
        //[Timeout(1000)]
        public void Example135()
        {
            // Example 135
            // Section: Leaf blocks - Paragraphs
            //
            // The following CommonMark:
            //       aaa
            //      bbb
            //
            // Should be rendered as:
            //     <p>aaa
            //     bbb</p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 135, "Leaf blocks - Paragraphs");
			Helpers.ExecuteTest("  aaa\n bbb", "<p>aaa\nbbb</p>");
        }

        // Lines after the first may be indented any amount, since indented
        // code blocks cannot interrupt paragraphs.
        [TestMethod]
        [TestCategory("Leaf blocks - Paragraphs")]
        //[Timeout(1000)]
        public void Example136()
        {
            // Example 136
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

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 136, "Leaf blocks - Paragraphs");
			Helpers.ExecuteTest("aaa\n             bbb\n                                       ccc", "<p>aaa\nbbb\nccc</p>");
        }

        // However, the first line may be indented at most three spaces,
        // or an indented code block will be triggered:
        [TestMethod]
        [TestCategory("Leaf blocks - Paragraphs")]
        //[Timeout(1000)]
        public void Example137()
        {
            // Example 137
            // Section: Leaf blocks - Paragraphs
            //
            // The following CommonMark:
            //        aaa
            //     bbb
            //
            // Should be rendered as:
            //     <p>aaa
            //     bbb</p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 137, "Leaf blocks - Paragraphs");
			Helpers.ExecuteTest("   aaa\nbbb", "<p>aaa\nbbb</p>");
        }

        [TestMethod]
        [TestCategory("Leaf blocks - Paragraphs")]
        //[Timeout(1000)]
        public void Example138()
        {
            // Example 138
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

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 138, "Leaf blocks - Paragraphs");
			Helpers.ExecuteTest("    aaa\nbbb", "<pre><code>aaa\n</code></pre>\n<p>bbb</p>");
        }

        // Final spaces are stripped before inline parsing, so a paragraph
        // that ends with two or more spaces will not end with a [hard line
        // break]:
        [TestMethod]
        [TestCategory("Leaf blocks - Paragraphs")]
        //[Timeout(1000)]
        public void Example139()
        {
            // Example 139
            // Section: Leaf blocks - Paragraphs
            //
            // The following CommonMark:
            //     aaa     
            //     bbb     
            //
            // Should be rendered as:
            //     <p>aaa<br />
            //     bbb</p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 139, "Leaf blocks - Paragraphs");
			Helpers.ExecuteTest("aaa     \nbbb     ", "<p>aaa<br />\nbbb</p>");
        }

        // ## Blank lines
        //
        // [Blank line]s between block-level elements are ignored,
        // except for the role they play in determining whether a [list]
        // is [tight] or [loose].
        //
        // Blank lines at the beginning and end of the document are also ignored.
        [TestMethod]
        [TestCategory("Leaf blocks - Blank lines")]
        //[Timeout(1000)]
        public void Example140()
        {
            // Example 140
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

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 140, "Leaf blocks - Blank lines");
			Helpers.ExecuteTest("  \n\naaa\n  \n\n# aaa\n\n  ", "<p>aaa</p>\n<h1>aaa</h1>");
        }

        // # Container blocks
        //
        // A [container block] is a block that has other
        // blocks as its contents.  There are two basic kinds of container blocks:
        // [block quotes] and [list items].
        // [Lists] are meta-containers for [list items].
        //
        // We define the syntax for container blocks recursively.  The general
        // form of the definition is:
        //
        // > If X is a sequence of blocks, then the result of
        // > transforming X in such-and-such a way is a container of type Y
        // > with these blocks as its content.
        //
        // So, we explain what counts as a block quote or list item by explaining
        // how these can be *generated* from their contents. This should suffice
        // to define the syntax, although it does not give a recipe for *parsing*
        // these constructions.  (A recipe is provided below in the section entitled
        // [A parsing strategy](#appendix-a-a-parsing-strategy).)
        //
        // ## Block quotes
        //
        // A [block quote marker](@block-quote-marker)
        // consists of 0-3 spaces of initial indent, plus (a) the character `>` together
        // with a following space, or (b) a single character `>` not followed by a space.
        //
        // The following rules define [block quotes]:
        //
        // 1.  **Basic case.**  If a string of lines *Ls* constitute a sequence
        // of blocks *Bs*, then the result of prepending a [block quote
        // marker] to the beginning of each line in *Ls*
        // is a [block quote](#block-quotes) containing *Bs*.
        //
        // 2.  **Laziness.**  If a string of lines *Ls* constitute a [block
        // quote](#block-quotes) with contents *Bs*, then the result of deleting
        // the initial [block quote marker] from one or
        // more lines in which the next [non-space character] after the [block
        // quote marker] is [paragraph continuation
        // text] is a block quote with *Bs* as its content.
        // [Paragraph continuation text](@paragraph-continuation-text) is text
        // that will be parsed as part of the content of a paragraph, but does
        // not occur at the beginning of the paragraph.
        //
        // 3.  **Consecutiveness.**  A document cannot contain two [block
        // quotes] in a row unless there is a [blank line] between them.
        //
        // Nothing else counts as a [block quote](#block-quotes).
        //
        // Here is a simple example:
        [TestMethod]
        [TestCategory("Container blocks - Block quotes")]
        //[Timeout(1000)]
        public void Example141()
        {
            // Example 141
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

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 141, "Container blocks - Block quotes");
			Helpers.ExecuteTest("> # Foo\n> bar\n> baz", "<blockquote>\n<h1>Foo</h1>\n<p>bar\nbaz</p>\n</blockquote>");
        }

        // The spaces after the `>` characters can be omitted:
        [TestMethod]
        [TestCategory("Container blocks - Block quotes")]
        //[Timeout(1000)]
        public void Example142()
        {
            // Example 142
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

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 142, "Container blocks - Block quotes");
			Helpers.ExecuteTest("># Foo\n>bar\n> baz", "<blockquote>\n<h1>Foo</h1>\n<p>bar\nbaz</p>\n</blockquote>");
        }

        // The `>` characters can be indented 1-3 spaces:
        [TestMethod]
        [TestCategory("Container blocks - Block quotes")]
        //[Timeout(1000)]
        public void Example143()
        {
            // Example 143
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

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 143, "Container blocks - Block quotes");
			Helpers.ExecuteTest("   > # Foo\n   > bar\n > baz", "<blockquote>\n<h1>Foo</h1>\n<p>bar\nbaz</p>\n</blockquote>");
        }

        // Four spaces gives us a code block:
        [TestMethod]
        [TestCategory("Container blocks - Block quotes")]
        //[Timeout(1000)]
        public void Example144()
        {
            // Example 144
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

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 144, "Container blocks - Block quotes");
			Helpers.ExecuteTest("    > # Foo\n    > bar\n    > baz", "<pre><code>&gt; # Foo\n&gt; bar\n&gt; baz\n</code></pre>");
        }

        // The Laziness clause allows us to omit the `>` before a
        // paragraph continuation line:
        [TestMethod]
        [TestCategory("Container blocks - Block quotes")]
        //[Timeout(1000)]
        public void Example145()
        {
            // Example 145
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

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 145, "Container blocks - Block quotes");
			Helpers.ExecuteTest("> # Foo\n> bar\nbaz", "<blockquote>\n<h1>Foo</h1>\n<p>bar\nbaz</p>\n</blockquote>");
        }

        // A block quote can contain some lazy and some non-lazy
        // continuation lines:
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
            //     > foo
            //
            // Should be rendered as:
            //     <blockquote>
            //     <p>bar
            //     baz
            //     foo</p>
            //     </blockquote>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 146, "Container blocks - Block quotes");
			Helpers.ExecuteTest("> bar\nbaz\n> foo", "<blockquote>\n<p>bar\nbaz\nfoo</p>\n</blockquote>");
        }

        // Laziness only applies to lines that are continuations of
        // paragraphs. Lines containing characters or indentation that indicate
        // block structure cannot be lazy.
        [TestMethod]
        [TestCategory("Container blocks - Block quotes")]
        //[Timeout(1000)]
        public void Example147()
        {
            // Example 147
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

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 147, "Container blocks - Block quotes");
			Helpers.ExecuteTest("> foo\n---", "<blockquote>\n<p>foo</p>\n</blockquote>\n<hr />");
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

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 148, "Container blocks - Block quotes");
			Helpers.ExecuteTest("> - foo\n- bar", "<blockquote>\n<ul>\n<li>foo</li>\n</ul>\n</blockquote>\n<ul>\n<li>bar</li>\n</ul>");
        }

        [TestMethod]
        [TestCategory("Container blocks - Block quotes")]
        //[Timeout(1000)]
        public void Example149()
        {
            // Example 149
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

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 149, "Container blocks - Block quotes");
			Helpers.ExecuteTest(">     foo\n    bar", "<blockquote>\n<pre><code>foo\n</code></pre>\n</blockquote>\n<pre><code>bar\n</code></pre>");
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

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 150, "Container blocks - Block quotes");
			Helpers.ExecuteTest("> ```\nfoo\n```", "<blockquote>\n<pre><code></code></pre>\n</blockquote>\n<p>foo</p>\n<pre><code></code></pre>");
        }

        // A block quote can be empty:
        [TestMethod]
        [TestCategory("Container blocks - Block quotes")]
        //[Timeout(1000)]
        public void Example151()
        {
            // Example 151
            // Section: Container blocks - Block quotes
            //
            // The following CommonMark:
            //     >
            //
            // Should be rendered as:
            //     <blockquote>
            //     </blockquote>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 151, "Container blocks - Block quotes");
			Helpers.ExecuteTest(">", "<blockquote>\n</blockquote>");
        }

        [TestMethod]
        [TestCategory("Container blocks - Block quotes")]
        //[Timeout(1000)]
        public void Example152()
        {
            // Example 152
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

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 152, "Container blocks - Block quotes");
			Helpers.ExecuteTest(">\n>  \n> ", "<blockquote>\n</blockquote>");
        }

        // A block quote can have initial or final blank lines:
        [TestMethod]
        [TestCategory("Container blocks - Block quotes")]
        //[Timeout(1000)]
        public void Example153()
        {
            // Example 153
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

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 153, "Container blocks - Block quotes");
			Helpers.ExecuteTest(">\n> foo\n>  ", "<blockquote>\n<p>foo</p>\n</blockquote>");
        }

        // A blank line always separates block quotes:
        [TestMethod]
        [TestCategory("Container blocks - Block quotes")]
        //[Timeout(1000)]
        public void Example154()
        {
            // Example 154
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

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 154, "Container blocks - Block quotes");
			Helpers.ExecuteTest("> foo\n\n> bar", "<blockquote>\n<p>foo</p>\n</blockquote>\n<blockquote>\n<p>bar</p>\n</blockquote>");
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
        public void Example155()
        {
            // Example 155
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

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 155, "Container blocks - Block quotes");
			Helpers.ExecuteTest("> foo\n> bar", "<blockquote>\n<p>foo\nbar</p>\n</blockquote>");
        }

        // To get a block quote with two paragraphs, use:
        [TestMethod]
        [TestCategory("Container blocks - Block quotes")]
        //[Timeout(1000)]
        public void Example156()
        {
            // Example 156
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

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 156, "Container blocks - Block quotes");
			Helpers.ExecuteTest("> foo\n>\n> bar", "<blockquote>\n<p>foo</p>\n<p>bar</p>\n</blockquote>");
        }

        // Block quotes can interrupt paragraphs:
        [TestMethod]
        [TestCategory("Container blocks - Block quotes")]
        //[Timeout(1000)]
        public void Example157()
        {
            // Example 157
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

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 157, "Container blocks - Block quotes");
			Helpers.ExecuteTest("foo\n> bar", "<p>foo</p>\n<blockquote>\n<p>bar</p>\n</blockquote>");
        }

        // In general, blank lines are not needed before or after block
        // quotes:
        [TestMethod]
        [TestCategory("Container blocks - Block quotes")]
        //[Timeout(1000)]
        public void Example158()
        {
            // Example 158
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

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 158, "Container blocks - Block quotes");
			Helpers.ExecuteTest("> aaa\n***\n> bbb", "<blockquote>\n<p>aaa</p>\n</blockquote>\n<hr />\n<blockquote>\n<p>bbb</p>\n</blockquote>");
        }

        // However, because of laziness, a blank line is needed between
        // a block quote and a following paragraph:
        [TestMethod]
        [TestCategory("Container blocks - Block quotes")]
        //[Timeout(1000)]
        public void Example159()
        {
            // Example 159
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

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 159, "Container blocks - Block quotes");
			Helpers.ExecuteTest("> bar\nbaz", "<blockquote>\n<p>bar\nbaz</p>\n</blockquote>");
        }

        [TestMethod]
        [TestCategory("Container blocks - Block quotes")]
        //[Timeout(1000)]
        public void Example160()
        {
            // Example 160
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

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 160, "Container blocks - Block quotes");
			Helpers.ExecuteTest("> bar\n\nbaz", "<blockquote>\n<p>bar</p>\n</blockquote>\n<p>baz</p>");
        }

        [TestMethod]
        [TestCategory("Container blocks - Block quotes")]
        //[Timeout(1000)]
        public void Example161()
        {
            // Example 161
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

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 161, "Container blocks - Block quotes");
			Helpers.ExecuteTest("> bar\n>\nbaz", "<blockquote>\n<p>bar</p>\n</blockquote>\n<p>baz</p>");
        }

        // It is a consequence of the Laziness rule that any number
        // of initial `>`s may be omitted on a continuation line of a
        // nested block quote:
        [TestMethod]
        [TestCategory("Container blocks - Block quotes")]
        //[Timeout(1000)]
        public void Example162()
        {
            // Example 162
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

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 162, "Container blocks - Block quotes");
			Helpers.ExecuteTest("> > > foo\nbar", "<blockquote>\n<blockquote>\n<blockquote>\n<p>foo\nbar</p>\n</blockquote>\n</blockquote>\n</blockquote>");
        }

        [TestMethod]
        [TestCategory("Container blocks - Block quotes")]
        //[Timeout(1000)]
        public void Example163()
        {
            // Example 163
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

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 163, "Container blocks - Block quotes");
			Helpers.ExecuteTest(">>> foo\n> bar\n>>baz", "<blockquote>\n<blockquote>\n<blockquote>\n<p>foo\nbar\nbaz</p>\n</blockquote>\n</blockquote>\n</blockquote>");
        }

        // When including an indented code block in a block quote,
        // remember that the [block quote marker] includes
        // both the `>` and a following space.  So *five spaces* are needed after
        // the `>`:
        [TestMethod]
        [TestCategory("Container blocks - Block quotes")]
        //[Timeout(1000)]
        public void Example164()
        {
            // Example 164
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

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 164, "Container blocks - Block quotes");
			Helpers.ExecuteTest(">     code\n\n>    not code", "<blockquote>\n<pre><code>code\n</code></pre>\n</blockquote>\n<blockquote>\n<p>not code</p>\n</blockquote>");
        }

        // ## List items
        //
        // A [list marker](@list-marker) is a
        // [bullet list marker] or an [ordered list marker].
        //
        // A [bullet list marker](@bullet-list-marker)
        // is a `-`, `+`, or `*` character.
        //
        // An [ordered list marker](@ordered-list-marker)
        // is a sequence of one of more digits (`0-9`), followed by either a
        // `.` character or a `)` character.
        //
        // The following rules define [list items]:
        //
        // 1.  **Basic case.**  If a sequence of lines *Ls* constitute a sequence of
        // blocks *Bs* starting with a [non-space character] and not separated
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
        public void Example165()
        {
            // Example 165
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

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 165, "Container blocks - List items");
			Helpers.ExecuteTest("A paragraph\nwith two lines.\n\n    indented code\n\n> A block quote.", "<p>A paragraph\nwith two lines.</p>\n<pre><code>indented code\n</code></pre>\n<blockquote>\n<p>A block quote.</p>\n</blockquote>");
        }

        // And let *M* be the marker `1.`, and *N* = 2.  Then rule #1 says
        // that the following is an ordered list item with start number 1,
        // and the same contents as *Ls*:
        [TestMethod]
        [TestCategory("Container blocks - List items")]
        //[Timeout(1000)]
        public void Example166()
        {
            // Example 166
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
            //     <li>
            //     <p>A paragraph
            //     with two lines.</p>
            //     <pre><code>indented code
            //     </code></pre>
            //     <blockquote>
            //     <p>A block quote.</p>
            //     </blockquote>
            //     </li>
            //     </ol>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 166, "Container blocks - List items");
			Helpers.ExecuteTest("1.  A paragraph\n    with two lines.\n\n        indented code\n\n    > A block quote.", "<ol>\n<li>\n<p>A paragraph\nwith two lines.</p>\n<pre><code>indented code\n</code></pre>\n<blockquote>\n<p>A block quote.</p>\n</blockquote>\n</li>\n</ol>");
        }

        // The most important thing to notice is that the position of
        // the text after the list marker determines how much indentation
        // is needed in subsequent blocks in the list item.  If the list
        // marker takes up two spaces, and there are three spaces between
        // the list marker and the next [non-space character], then blocks
        // must be indented five spaces in order to fall under the list
        // item.
        //
        // Here are some examples showing how far content must be indented to be
        // put under the list item:
        [TestMethod]
        [TestCategory("Container blocks - List items")]
        //[Timeout(1000)]
        public void Example167()
        {
            // Example 167
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

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 167, "Container blocks - List items");
			Helpers.ExecuteTest("- one\n\n two", "<ul>\n<li>one</li>\n</ul>\n<p>two</p>");
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
            //     - one
            //     
            //       two
            //
            // Should be rendered as:
            //     <ul>
            //     <li>
            //     <p>one</p>
            //     <p>two</p>
            //     </li>
            //     </ul>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 168, "Container blocks - List items");
			Helpers.ExecuteTest("- one\n\n  two", "<ul>\n<li>\n<p>one</p>\n<p>two</p>\n</li>\n</ul>");
        }

        [TestMethod]
        [TestCategory("Container blocks - List items")]
        //[Timeout(1000)]
        public void Example169()
        {
            // Example 169
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

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 169, "Container blocks - List items");
			Helpers.ExecuteTest(" -    one\n\n     two", "<ul>\n<li>one</li>\n</ul>\n<pre><code> two\n</code></pre>");
        }

        [TestMethod]
        [TestCategory("Container blocks - List items")]
        //[Timeout(1000)]
        public void Example170()
        {
            // Example 170
            // Section: Container blocks - List items
            //
            // The following CommonMark:
            //      -    one
            //     
            //           two
            //
            // Should be rendered as:
            //     <ul>
            //     <li>
            //     <p>one</p>
            //     <p>two</p>
            //     </li>
            //     </ul>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 170, "Container blocks - List items");
			Helpers.ExecuteTest(" -    one\n\n      two", "<ul>\n<li>\n<p>one</p>\n<p>two</p>\n</li>\n</ul>");
        }

        // It is tempting to think of this in terms of columns:  the continuation
        // blocks must be indented at least to the column of the first
        // [non-space character] after the list marker. However, that is not quite right.
        // The spaces after the list marker determine how much relative indentation
        // is needed.  Which column this indentation reaches will depend on
        // how the list item is embedded in other constructions, as shown by
        // this example:
        [TestMethod]
        [TestCategory("Container blocks - List items")]
        //[Timeout(1000)]
        public void Example171()
        {
            // Example 171
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
            //     <li>
            //     <p>one</p>
            //     <p>two</p>
            //     </li>
            //     </ol>
            //     </blockquote>
            //     </blockquote>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 171, "Container blocks - List items");
			Helpers.ExecuteTest("   > > 1.  one\n>>\n>>     two", "<blockquote>\n<blockquote>\n<ol>\n<li>\n<p>one</p>\n<p>two</p>\n</li>\n</ol>\n</blockquote>\n</blockquote>");
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
        public void Example172()
        {
            // Example 172
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

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 172, "Container blocks - List items");
			Helpers.ExecuteTest(">>- one\n>>\n  >  > two", "<blockquote>\n<blockquote>\n<ul>\n<li>one</li>\n</ul>\n<p>two</p>\n</blockquote>\n</blockquote>");
        }

        // Note that at least one space is needed between the list marker and
        // any following content, so these are not list items:
        [TestMethod]
        [TestCategory("Container blocks - List items")]
        //[Timeout(1000)]
        public void Example173()
        {
            // Example 173
            // Section: Container blocks - List items
            //
            // The following CommonMark:
            //     -one
            //     
            //     2.two
            //
            // Should be rendered as:
            //     <p>-one</p>
            //     <p>2.two</p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 173, "Container blocks - List items");
			Helpers.ExecuteTest("-one\n\n2.two", "<p>-one</p>\n<p>2.two</p>");
        }

        // A list item may not contain blocks that are separated by more than
        // one blank line.  Thus, two blank lines will end a list, unless the
        // two blanks are contained in a [fenced code block].
        [TestMethod]
        [TestCategory("Container blocks - List items")]
        //[Timeout(1000)]
        public void Example174()
        {
            // Example 174
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
            //     - baz
            //     
            //       + ```
            //         foo
            //     
            //     
            //         bar
            //         ```
            //
            // Should be rendered as:
            //     <ul>
            //     <li>
            //     <p>foo</p>
            //     <p>bar</p>
            //     </li>
            //     <li>
            //     <p>foo</p>
            //     </li>
            //     </ul>
            //     <p>bar</p>
            //     <ul>
            //     <li>
            //     <pre><code>foo
            //     
            //     
            //     bar
            //     </code></pre>
            //     </li>
            //     <li>
            //     <p>baz</p>
            //     <ul>
            //     <li>
            //     <pre><code>foo
            //     
            //     
            //     bar
            //     </code></pre>
            //     </li>
            //     </ul>
            //     </li>
            //     </ul>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 174, "Container blocks - List items");
			Helpers.ExecuteTest("- foo\n\n  bar\n\n- foo\n\n\n  bar\n\n- ```\n  foo\n\n\n  bar\n  ```\n\n- baz\n\n  + ```\n    foo\n\n\n    bar\n    ```", "<ul>\n<li>\n<p>foo</p>\n<p>bar</p>\n</li>\n<li>\n<p>foo</p>\n</li>\n</ul>\n<p>bar</p>\n<ul>\n<li>\n<pre><code>foo\n\n\nbar\n</code></pre>\n</li>\n<li>\n<p>baz</p>\n<ul>\n<li>\n<pre><code>foo\n\n\nbar\n</code></pre>\n</li>\n</ul>\n</li>\n</ul>");
        }

        // A list item may contain any kind of block:
        [TestMethod]
        [TestCategory("Container blocks - List items")]
        //[Timeout(1000)]
        public void Example175()
        {
            // Example 175
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
            //     <li>
            //     <p>foo</p>
            //     <pre><code>bar
            //     </code></pre>
            //     <p>baz</p>
            //     <blockquote>
            //     <p>bam</p>
            //     </blockquote>
            //     </li>
            //     </ol>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 175, "Container blocks - List items");
			Helpers.ExecuteTest("1.  foo\n\n    ```\n    bar\n    ```\n\n    baz\n\n    > bam", "<ol>\n<li>\n<p>foo</p>\n<pre><code>bar\n</code></pre>\n<p>baz</p>\n<blockquote>\n<p>bam</p>\n</blockquote>\n</li>\n</ol>");
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
        public void Example176()
        {
            // Example 176
            // Section: Container blocks - List items
            //
            // The following CommonMark:
            //     - foo
            //     
            //           bar
            //
            // Should be rendered as:
            //     <ul>
            //     <li>
            //     <p>foo</p>
            //     <pre><code>bar
            //     </code></pre>
            //     </li>
            //     </ul>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 176, "Container blocks - List items");
			Helpers.ExecuteTest("- foo\n\n      bar", "<ul>\n<li>\n<p>foo</p>\n<pre><code>bar\n</code></pre>\n</li>\n</ul>");
        }

        // And in this case it is 11 spaces:
        [TestMethod]
        [TestCategory("Container blocks - List items")]
        //[Timeout(1000)]
        public void Example177()
        {
            // Example 177
            // Section: Container blocks - List items
            //
            // The following CommonMark:
            //       10.  foo
            //     
            //                bar
            //
            // Should be rendered as:
            //     <ol start="10">
            //     <li>
            //     <p>foo</p>
            //     <pre><code>bar
            //     </code></pre>
            //     </li>
            //     </ol>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 177, "Container blocks - List items");
			Helpers.ExecuteTest("  10.  foo\n\n           bar", "<ol start=\"10\">\n<li>\n<p>foo</p>\n<pre><code>bar\n</code></pre>\n</li>\n</ol>");
        }

        // If the *first* block in the list item is an indented code block,
        // then by rule #2, the contents must be indented *one* space after the
        // list marker:
        [TestMethod]
        [TestCategory("Container blocks - List items")]
        //[Timeout(1000)]
        public void Example178()
        {
            // Example 178
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

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 178, "Container blocks - List items");
			Helpers.ExecuteTest("    indented code\n\nparagraph\n\n    more code", "<pre><code>indented code\n</code></pre>\n<p>paragraph</p>\n<pre><code>more code\n</code></pre>");
        }

        [TestMethod]
        [TestCategory("Container blocks - List items")]
        //[Timeout(1000)]
        public void Example179()
        {
            // Example 179
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
            //     <li>
            //     <pre><code>indented code
            //     </code></pre>
            //     <p>paragraph</p>
            //     <pre><code>more code
            //     </code></pre>
            //     </li>
            //     </ol>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 179, "Container blocks - List items");
			Helpers.ExecuteTest("1.     indented code\n\n   paragraph\n\n       more code", "<ol>\n<li>\n<pre><code>indented code\n</code></pre>\n<p>paragraph</p>\n<pre><code>more code\n</code></pre>\n</li>\n</ol>");
        }

        // Note that an additional space indent is interpreted as space
        // inside the code block:
        [TestMethod]
        [TestCategory("Container blocks - List items")]
        //[Timeout(1000)]
        public void Example180()
        {
            // Example 180
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
            //     <li>
            //     <pre><code> indented code
            //     </code></pre>
            //     <p>paragraph</p>
            //     <pre><code>more code
            //     </code></pre>
            //     </li>
            //     </ol>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 180, "Container blocks - List items");
			Helpers.ExecuteTest("1.      indented code\n\n   paragraph\n\n       more code", "<ol>\n<li>\n<pre><code> indented code\n</code></pre>\n<p>paragraph</p>\n<pre><code>more code\n</code></pre>\n</li>\n</ol>");
        }

        // Note that rules #1 and #2 only apply to two cases:  (a) cases
        // in which the lines to be included in a list item begin with a
        // [non-space character], and (b) cases in which
        // they begin with an indented code
        // block.  In a case like the following, where the first block begins with
        // a three-space indent, the rules do not allow us to form a list item by
        // indenting the whole thing and prepending a list marker:
        [TestMethod]
        [TestCategory("Container blocks - List items")]
        //[Timeout(1000)]
        public void Example181()
        {
            // Example 181
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

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 181, "Container blocks - List items");
			Helpers.ExecuteTest("   foo\n\nbar", "<p>foo</p>\n<p>bar</p>");
        }

        [TestMethod]
        [TestCategory("Container blocks - List items")]
        //[Timeout(1000)]
        public void Example182()
        {
            // Example 182
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

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 182, "Container blocks - List items");
			Helpers.ExecuteTest("-    foo\n\n  bar", "<ul>\n<li>foo</li>\n</ul>\n<p>bar</p>");
        }

        // This is not a significant restriction, because when a block begins
        // with 1-3 spaces indent, the indentation can always be removed without
        // a change in interpretation, allowing rule #1 to be applied.  So, in
        // the above case:
        [TestMethod]
        [TestCategory("Container blocks - List items")]
        //[Timeout(1000)]
        public void Example183()
        {
            // Example 183
            // Section: Container blocks - List items
            //
            // The following CommonMark:
            //     -  foo
            //     
            //        bar
            //
            // Should be rendered as:
            //     <ul>
            //     <li>
            //     <p>foo</p>
            //     <p>bar</p>
            //     </li>
            //     </ul>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 183, "Container blocks - List items");
			Helpers.ExecuteTest("-  foo\n\n   bar", "<ul>\n<li>\n<p>foo</p>\n<p>bar</p>\n</li>\n</ul>");
        }

        // 3.  **Item starting with a blank line.**  If a sequence of lines *Ls*
        // starting with a single [blank line] constitute a (possibly empty)
        // sequence of blocks *Bs*, not separated from each other by more than
        // one blank line, and *M* is a list marker *M* of width *W*,
        // then the result of prepending *M* to the first line of *Ls*, and
        // indenting subsequent lines of *Ls* by *W + 1* spaces, is a list
        // item with *Bs* as its contents.
        // If a line is empty, then it need not be indented.  The type of the
        // list item (bullet or ordered) is determined by the type of its list
        // marker.  If the list item is ordered, then it is also assigned a
        // start number, based on the ordered list marker.
        //
        // Here are some list items that start with a blank line but are not empty:
        [TestMethod]
        [TestCategory("Container blocks - List items")]
        //[Timeout(1000)]
        public void Example184()
        {
            // Example 184
            // Section: Container blocks - List items
            //
            // The following CommonMark:
            //     -
            //       foo
            //     -
            //       ```
            //       bar
            //       ```
            //     -
            //           baz
            //
            // Should be rendered as:
            //     <ul>
            //     <li>foo</li>
            //     <li>
            //     <pre><code>bar
            //     </code></pre>
            //     </li>
            //     <li>
            //     <pre><code>baz
            //     </code></pre>
            //     </li>
            //     </ul>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 184, "Container blocks - List items");
			Helpers.ExecuteTest("-\n  foo\n-\n  ```\n  bar\n  ```\n-\n      baz", "<ul>\n<li>foo</li>\n<li>\n<pre><code>bar\n</code></pre>\n</li>\n<li>\n<pre><code>baz\n</code></pre>\n</li>\n</ul>");
        }

        // Here is an empty bullet list item:
        [TestMethod]
        [TestCategory("Container blocks - List items")]
        //[Timeout(1000)]
        public void Example185()
        {
            // Example 185
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

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 185, "Container blocks - List items");
			Helpers.ExecuteTest("- foo\n-\n- bar", "<ul>\n<li>foo</li>\n<li></li>\n<li>bar</li>\n</ul>");
        }

        // It does not matter whether there are spaces following the [list marker]:
        [TestMethod]
        [TestCategory("Container blocks - List items")]
        //[Timeout(1000)]
        public void Example186()
        {
            // Example 186
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

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 186, "Container blocks - List items");
			Helpers.ExecuteTest("- foo\n-   \n- bar", "<ul>\n<li>foo</li>\n<li></li>\n<li>bar</li>\n</ul>");
        }

        // Here is an empty ordered list item:
        [TestMethod]
        [TestCategory("Container blocks - List items")]
        //[Timeout(1000)]
        public void Example187()
        {
            // Example 187
            // Section: Container blocks - List items
            //
            // The following CommonMark:
            //     1. foo
            //     2.
            //     3. bar
            //
            // Should be rendered as:
            //     <ol>
            //     <li>foo</li>
            //     <li></li>
            //     <li>bar</li>
            //     </ol>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 187, "Container blocks - List items");
			Helpers.ExecuteTest("1. foo\n2.\n3. bar", "<ol>\n<li>foo</li>\n<li></li>\n<li>bar</li>\n</ol>");
        }

        // A list may start or end with an empty list item:
        [TestMethod]
        [TestCategory("Container blocks - List items")]
        //[Timeout(1000)]
        public void Example188()
        {
            // Example 188
            // Section: Container blocks - List items
            //
            // The following CommonMark:
            //     *
            //
            // Should be rendered as:
            //     <ul>
            //     <li></li>
            //     </ul>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 188, "Container blocks - List items");
			Helpers.ExecuteTest("*", "<ul>\n<li></li>\n</ul>");
        }

        // 4.  **Indentation.**  If a sequence of lines *Ls* constitutes a list item
        // according to rule #1, #2, or #3, then the result of indenting each line
        // of *L* by 1-3 spaces (the same for each line) also constitutes a
        // list item with the same contents and attributes.  If a line is
        // empty, then it need not be indented.
        //
        // Indented one space:
        [TestMethod]
        [TestCategory("Container blocks - List items")]
        //[Timeout(1000)]
        public void Example189()
        {
            // Example 189
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
            //     <li>
            //     <p>A paragraph
            //     with two lines.</p>
            //     <pre><code>indented code
            //     </code></pre>
            //     <blockquote>
            //     <p>A block quote.</p>
            //     </blockquote>
            //     </li>
            //     </ol>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 189, "Container blocks - List items");
			Helpers.ExecuteTest(" 1.  A paragraph\n     with two lines.\n\n         indented code\n\n     > A block quote.", "<ol>\n<li>\n<p>A paragraph\nwith two lines.</p>\n<pre><code>indented code\n</code></pre>\n<blockquote>\n<p>A block quote.</p>\n</blockquote>\n</li>\n</ol>");
        }

        // Indented two spaces:
        [TestMethod]
        [TestCategory("Container blocks - List items")]
        //[Timeout(1000)]
        public void Example190()
        {
            // Example 190
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
            //     <li>
            //     <p>A paragraph
            //     with two lines.</p>
            //     <pre><code>indented code
            //     </code></pre>
            //     <blockquote>
            //     <p>A block quote.</p>
            //     </blockquote>
            //     </li>
            //     </ol>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 190, "Container blocks - List items");
			Helpers.ExecuteTest("  1.  A paragraph\n      with two lines.\n\n          indented code\n\n      > A block quote.", "<ol>\n<li>\n<p>A paragraph\nwith two lines.</p>\n<pre><code>indented code\n</code></pre>\n<blockquote>\n<p>A block quote.</p>\n</blockquote>\n</li>\n</ol>");
        }

        // Indented three spaces:
        [TestMethod]
        [TestCategory("Container blocks - List items")]
        //[Timeout(1000)]
        public void Example191()
        {
            // Example 191
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
            //     <li>
            //     <p>A paragraph
            //     with two lines.</p>
            //     <pre><code>indented code
            //     </code></pre>
            //     <blockquote>
            //     <p>A block quote.</p>
            //     </blockquote>
            //     </li>
            //     </ol>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 191, "Container blocks - List items");
			Helpers.ExecuteTest("   1.  A paragraph\n       with two lines.\n\n           indented code\n\n       > A block quote.", "<ol>\n<li>\n<p>A paragraph\nwith two lines.</p>\n<pre><code>indented code\n</code></pre>\n<blockquote>\n<p>A block quote.</p>\n</blockquote>\n</li>\n</ol>");
        }

        // Four spaces indent gives a code block:
        [TestMethod]
        [TestCategory("Container blocks - List items")]
        //[Timeout(1000)]
        public void Example192()
        {
            // Example 192
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

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 192, "Container blocks - List items");
			Helpers.ExecuteTest("    1.  A paragraph\n        with two lines.\n\n            indented code\n\n        > A block quote.", "<pre><code>1.  A paragraph\n    with two lines.\n\n        indented code\n\n    &gt; A block quote.\n</code></pre>");
        }

        // 5.  **Laziness.**  If a string of lines *Ls* constitute a [list
        // item](#list-items) with contents *Bs*, then the result of deleting
        // some or all of the indentation from one or more lines in which the
        // next [non-space character] after the indentation is
        // [paragraph continuation text] is a
        // list item with the same contents and attributes.  The unindented
        // lines are called
        // [lazy continuation line](@lazy-continuation-line)s.
        //
        // Here is an example with [lazy continuation line]s:
        [TestMethod]
        [TestCategory("Container blocks - List items")]
        //[Timeout(1000)]
        public void Example193()
        {
            // Example 193
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
            //     <li>
            //     <p>A paragraph
            //     with two lines.</p>
            //     <pre><code>indented code
            //     </code></pre>
            //     <blockquote>
            //     <p>A block quote.</p>
            //     </blockquote>
            //     </li>
            //     </ol>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 193, "Container blocks - List items");
			Helpers.ExecuteTest("  1.  A paragraph\nwith two lines.\n\n          indented code\n\n      > A block quote.", "<ol>\n<li>\n<p>A paragraph\nwith two lines.</p>\n<pre><code>indented code\n</code></pre>\n<blockquote>\n<p>A block quote.</p>\n</blockquote>\n</li>\n</ol>");
        }

        // Indentation can be partially deleted:
        [TestMethod]
        [TestCategory("Container blocks - List items")]
        //[Timeout(1000)]
        public void Example194()
        {
            // Example 194
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

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 194, "Container blocks - List items");
			Helpers.ExecuteTest("  1.  A paragraph\n    with two lines.", "<ol>\n<li>A paragraph\nwith two lines.</li>\n</ol>");
        }

        // These examples show how laziness can work in nested structures:
        [TestMethod]
        [TestCategory("Container blocks - List items")]
        //[Timeout(1000)]
        public void Example195()
        {
            // Example 195
            // Section: Container blocks - List items
            //
            // The following CommonMark:
            //     > 1. > Blockquote
            //     continued here.
            //
            // Should be rendered as:
            //     <blockquote>
            //     <ol>
            //     <li>
            //     <blockquote>
            //     <p>Blockquote
            //     continued here.</p>
            //     </blockquote>
            //     </li>
            //     </ol>
            //     </blockquote>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 195, "Container blocks - List items");
			Helpers.ExecuteTest("> 1. > Blockquote\ncontinued here.", "<blockquote>\n<ol>\n<li>\n<blockquote>\n<p>Blockquote\ncontinued here.</p>\n</blockquote>\n</li>\n</ol>\n</blockquote>");
        }

        [TestMethod]
        [TestCategory("Container blocks - List items")]
        //[Timeout(1000)]
        public void Example196()
        {
            // Example 196
            // Section: Container blocks - List items
            //
            // The following CommonMark:
            //     > 1. > Blockquote
            //     > continued here.
            //
            // Should be rendered as:
            //     <blockquote>
            //     <ol>
            //     <li>
            //     <blockquote>
            //     <p>Blockquote
            //     continued here.</p>
            //     </blockquote>
            //     </li>
            //     </ol>
            //     </blockquote>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 196, "Container blocks - List items");
			Helpers.ExecuteTest("> 1. > Blockquote\n> continued here.", "<blockquote>\n<ol>\n<li>\n<blockquote>\n<p>Blockquote\ncontinued here.</p>\n</blockquote>\n</li>\n</ol>\n</blockquote>");
        }

        // 6.  **That's all.** Nothing that is not counted as a list item by rules
        // #1--5 counts as a [list item](#list-items).
        //
        // The rules for sublists follow from the general rules above.  A sublist
        // must be indented the same number of spaces a paragraph would need to be
        // in order to be included in the list item.
        //
        // So, in this case we need two spaces indent:
        [TestMethod]
        [TestCategory("Container blocks - List items")]
        //[Timeout(1000)]
        public void Example197()
        {
            // Example 197
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
            //     </ul>
            //     </li>
            //     </ul>
            //     </li>
            //     </ul>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 197, "Container blocks - List items");
			Helpers.ExecuteTest("- foo\n  - bar\n    - baz", "<ul>\n<li>foo\n<ul>\n<li>bar\n<ul>\n<li>baz</li>\n</ul>\n</li>\n</ul>\n</li>\n</ul>");
        }

        // One is not enough:
        [TestMethod]
        [TestCategory("Container blocks - List items")]
        //[Timeout(1000)]
        public void Example198()
        {
            // Example 198
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

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 198, "Container blocks - List items");
			Helpers.ExecuteTest("- foo\n - bar\n  - baz", "<ul>\n<li>foo</li>\n<li>bar</li>\n<li>baz</li>\n</ul>");
        }

        // Here we need four, because the list marker is wider:
        [TestMethod]
        [TestCategory("Container blocks - List items")]
        //[Timeout(1000)]
        public void Example199()
        {
            // Example 199
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
            //     </ul>
            //     </li>
            //     </ol>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 199, "Container blocks - List items");
			Helpers.ExecuteTest("10) foo\n    - bar", "<ol start=\"10\">\n<li>foo\n<ul>\n<li>bar</li>\n</ul>\n</li>\n</ol>");
        }

        // Three is not enough:
        [TestMethod]
        [TestCategory("Container blocks - List items")]
        //[Timeout(1000)]
        public void Example200()
        {
            // Example 200
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

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 200, "Container blocks - List items");
			Helpers.ExecuteTest("10) foo\n   - bar", "<ol start=\"10\">\n<li>foo</li>\n</ol>\n<ul>\n<li>bar</li>\n</ul>");
        }

        // A list may be the first block in a list item:
        [TestMethod]
        [TestCategory("Container blocks - List items")]
        //[Timeout(1000)]
        public void Example201()
        {
            // Example 201
            // Section: Container blocks - List items
            //
            // The following CommonMark:
            //     - - foo
            //
            // Should be rendered as:
            //     <ul>
            //     <li>
            //     <ul>
            //     <li>foo</li>
            //     </ul>
            //     </li>
            //     </ul>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 201, "Container blocks - List items");
			Helpers.ExecuteTest("- - foo", "<ul>\n<li>\n<ul>\n<li>foo</li>\n</ul>\n</li>\n</ul>");
        }

        [TestMethod]
        [TestCategory("Container blocks - List items")]
        //[Timeout(1000)]
        public void Example202()
        {
            // Example 202
            // Section: Container blocks - List items
            //
            // The following CommonMark:
            //     1. - 2. foo
            //
            // Should be rendered as:
            //     <ol>
            //     <li>
            //     <ul>
            //     <li>
            //     <ol start="2">
            //     <li>foo</li>
            //     </ol>
            //     </li>
            //     </ul>
            //     </li>
            //     </ol>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 202, "Container blocks - List items");
			Helpers.ExecuteTest("1. - 2. foo", "<ol>\n<li>\n<ul>\n<li>\n<ol start=\"2\">\n<li>foo</li>\n</ol>\n</li>\n</ul>\n</li>\n</ol>");
        }

        // A list item can contain a header:
        [TestMethod]
        [TestCategory("Container blocks - List items")]
        //[Timeout(1000)]
        public void Example203()
        {
            // Example 203
            // Section: Container blocks - List items
            //
            // The following CommonMark:
            //     - # Foo
            //     - Bar
            //       ---
            //       baz
            //
            // Should be rendered as:
            //     <ul>
            //     <li>
            //     <h1>Foo</h1>
            //     </li>
            //     <li>
            //     <h2>Bar</h2>
            //     baz</li>
            //     </ul>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 203, "Container blocks - List items");
			Helpers.ExecuteTest("- # Foo\n- Bar\n  ---\n  baz", "<ul>\n<li>\n<h1>Foo</h1>\n</li>\n<li>\n<h2>Bar</h2>\nbaz</li>\n</ul>");
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
        // marker).  (The laziness rule, #5, then allows continuation lines to be
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
        // <li>
        // <p>foo</p>
        // <p>bar</p>
        // <ul>
        // <li>baz</li>
        // </ul>
        // </li>
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
        // <li>
        // <p>one</p>
        // <p>two</p>
        // </li>
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
        // <li>
        // <p>one</p>
        // <p>two</p>
        // </li>
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
        // A [list](@list) is a sequence of one or more
        // list items [of the same type].  The list items
        // may be separated by single [blank lines], but two
        // blank lines end all containing lists.
        //
        // Two list items are [of the same type](@of-the-same-type)
        // if they begin with a [list marker] of the same type.
        // Two list markers are of the
        // same type if (a) they are bullet list markers using the same character
        // (`-`, `+`, or `*`) or (b) they are ordered list numbers with the same
        // delimiter (either `.` or `)`).
        //
        // A list is an [ordered list](@ordered-list)
        // if its constituent list items begin with
        // [ordered list marker]s, and a
        // [bullet list](@bullet-list) if its constituent list
        // items begin with [bullet list marker]s.
        //
        // The [start number](@start-number)
        // of an [ordered list] is determined by the list number of
        // its initial list item.  The numbers of subsequent list items are
        // disregarded.
        //
        // A list is [loose](@loose) if any of its constituent
        // list items are separated by blank lines, or if any of its constituent
        // list items directly contain two block-level elements with a blank line
        // between them.  Otherwise a list is [tight](@tight).
        // (The difference in HTML output is that paragraphs in a loose list are
        // wrapped in `<p>` tags, while paragraphs in a tight list are not.)
        //
        // Changing the bullet or ordered list delimiter starts a new list:
        [TestMethod]
        [TestCategory("Container blocks - Lists")]
        //[Timeout(1000)]
        public void Example204()
        {
            // Example 204
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

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 204, "Container blocks - Lists");
			Helpers.ExecuteTest("- foo\n- bar\n+ baz", "<ul>\n<li>foo</li>\n<li>bar</li>\n</ul>\n<ul>\n<li>baz</li>\n</ul>");
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

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 205, "Container blocks - Lists");
			Helpers.ExecuteTest("1. foo\n2. bar\n3) baz", "<ol>\n<li>foo</li>\n<li>bar</li>\n</ol>\n<ol start=\"3\">\n<li>baz</li>\n</ol>");
        }

        // In CommonMark, a list can interrupt a paragraph. That is,
        // no blank line is needed to separate a paragraph from a following
        // list:
        [TestMethod]
        [TestCategory("Container blocks - Lists")]
        //[Timeout(1000)]
        public void Example206()
        {
            // Example 206
            // Section: Container blocks - Lists
            //
            // The following CommonMark:
            //     Foo
            //     - bar
            //     - baz
            //
            // Should be rendered as:
            //     <p>Foo</p>
            //     <ul>
            //     <li>bar</li>
            //     <li>baz</li>
            //     </ul>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 206, "Container blocks - Lists");
			Helpers.ExecuteTest("Foo\n- bar\n- baz", "<p>Foo</p>\n<ul>\n<li>bar</li>\n<li>baz</li>\n</ul>");
        }

        // `Markdown.pl` does not allow this, through fear of triggering a list
        // via a numeral in a hard-wrapped line:
        [TestMethod]
        [TestCategory("Container blocks - Lists")]
        //[Timeout(1000)]
        public void Example207()
        {
            // Example 207
            // Section: Container blocks - Lists
            //
            // The following CommonMark:
            //     The number of windows in my house is
            //     14.  The number of doors is 6.
            //
            // Should be rendered as:
            //     <p>The number of windows in my house is</p>
            //     <ol start="14">
            //     <li>The number of doors is 6.</li>
            //     </ol>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 207, "Container blocks - Lists");
			Helpers.ExecuteTest("The number of windows in my house is\n14.  The number of doors is 6.", "<p>The number of windows in my house is</p>\n<ol start=\"14\">\n<li>The number of doors is 6.</li>\n</ol>");
        }

        // Oddly, `Markdown.pl` *does* allow a blockquote to interrupt a paragraph,
        // even though the same considerations might apply.  We think that the two
        // cases should be treated the same.  Here are two reasons for allowing
        // lists to interrupt paragraphs:
        //
        // First, it is natural and not uncommon for people to start lists without
        // blank lines:
        //
        // I need to buy
        // - new shoes
        // - a coat
        // - a plane ticket
        //
        // Second, we are attracted to a
        //
        // > [principle of uniformity](@principle-of-uniformity):
        // > if a chunk of text has a certain
        // > meaning, it will continue to have the same meaning when put into a
        // > container block (such as a list item or blockquote).
        //
        // (Indeed, the spec for [list items] and [block quotes] presupposes
        // this principle.) This principle implies that if
        //
        // * I need to buy
        // - new shoes
        // - a coat
        // - a plane ticket
        //
        // is a list item containing a paragraph followed by a nested sublist,
        // as all Markdown implementations agree it is (though the paragraph
        // may be rendered without `<p>` tags, since the list is "tight"),
        // then
        //
        // I need to buy
        // - new shoes
        // - a coat
        // - a plane ticket
        //
        // by itself should be a paragraph followed by a nested sublist.
        //
        // Our adherence to the [principle of uniformity]
        // thus inclines us to think that there are two coherent packages:
        //
        // 1.  Require blank lines before *all* lists and blockquotes,
        // including lists that occur as sublists inside other list items.
        //
        // 2.  Require blank lines in none of these places.
        //
        // [reStructuredText](http://docutils.sourceforge.net/rst.html) takes
        // the first approach, for which there is much to be said.  But the second
        // seems more consistent with established practice with Markdown.
        //
        // There can be blank lines between items, but two blank lines end
        // a list:
        [TestMethod]
        [TestCategory("Container blocks - Lists")]
        //[Timeout(1000)]
        public void Example208()
        {
            // Example 208
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
            //     <li>
            //     <p>foo</p>
            //     </li>
            //     <li>
            //     <p>bar</p>
            //     </li>
            //     </ul>
            //     <ul>
            //     <li>baz</li>
            //     </ul>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 208, "Container blocks - Lists");
			Helpers.ExecuteTest("- foo\n\n- bar\n\n\n- baz", "<ul>\n<li>\n<p>foo</p>\n</li>\n<li>\n<p>bar</p>\n</li>\n</ul>\n<ul>\n<li>baz</li>\n</ul>");
        }

        // As illustrated above in the section on [list items],
        // two blank lines between blocks *within* a list item will also end a
        // list:
        [TestMethod]
        [TestCategory("Container blocks - Lists")]
        //[Timeout(1000)]
        public void Example209()
        {
            // Example 209
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

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 209, "Container blocks - Lists");
			Helpers.ExecuteTest("- foo\n\n\n  bar\n- baz", "<ul>\n<li>foo</li>\n</ul>\n<p>bar</p>\n<ul>\n<li>baz</li>\n</ul>");
        }

        // Indeed, two blank lines will end *all* containing lists:
        [TestMethod]
        [TestCategory("Container blocks - Lists")]
        //[Timeout(1000)]
        public void Example210()
        {
            // Example 210
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
            //     </ul>
            //     </li>
            //     </ul>
            //     </li>
            //     </ul>
            //     <pre><code>  bim
            //     </code></pre>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 210, "Container blocks - Lists");
			Helpers.ExecuteTest("- foo\n  - bar\n    - baz\n\n\n      bim", "<ul>\n<li>foo\n<ul>\n<li>bar\n<ul>\n<li>baz</li>\n</ul>\n</li>\n</ul>\n</li>\n</ul>\n<pre><code>  bim\n</code></pre>");
        }

        // Thus, two blank lines can be used to separate consecutive lists of
        // the same type, or to separate a list from an indented code block
        // that would otherwise be parsed as a subparagraph of the final list
        // item:
        [TestMethod]
        [TestCategory("Container blocks - Lists")]
        //[Timeout(1000)]
        public void Example211()
        {
            // Example 211
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

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 211, "Container blocks - Lists");
			Helpers.ExecuteTest("- foo\n- bar\n\n\n- baz\n- bim", "<ul>\n<li>foo</li>\n<li>bar</li>\n</ul>\n<ul>\n<li>baz</li>\n<li>bim</li>\n</ul>");
        }

        [TestMethod]
        [TestCategory("Container blocks - Lists")]
        //[Timeout(1000)]
        public void Example212()
        {
            // Example 212
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
            //     <li>
            //     <p>foo</p>
            //     <p>notcode</p>
            //     </li>
            //     <li>
            //     <p>foo</p>
            //     </li>
            //     </ul>
            //     <pre><code>code
            //     </code></pre>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 212, "Container blocks - Lists");
			Helpers.ExecuteTest("-   foo\n\n    notcode\n\n-   foo\n\n\n    code", "<ul>\n<li>\n<p>foo</p>\n<p>notcode</p>\n</li>\n<li>\n<p>foo</p>\n</li>\n</ul>\n<pre><code>code\n</code></pre>");
        }

        // List items need not be indented to the same level.  The following
        // list items will be treated as items at the same list level,
        // since none is indented enough to belong to the previous list
        // item:
        [TestMethod]
        [TestCategory("Container blocks - Lists")]
        //[Timeout(1000)]
        public void Example213()
        {
            // Example 213
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

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 213, "Container blocks - Lists");
			Helpers.ExecuteTest("- a\n - b\n  - c\n   - d\n  - e\n - f\n- g", "<ul>\n<li>a</li>\n<li>b</li>\n<li>c</li>\n<li>d</li>\n<li>e</li>\n<li>f</li>\n<li>g</li>\n</ul>");
        }

        // This is a loose list, because there is a blank line between
        // two of the list items:
        [TestMethod]
        [TestCategory("Container blocks - Lists")]
        //[Timeout(1000)]
        public void Example214()
        {
            // Example 214
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
            //     <li>
            //     <p>a</p>
            //     </li>
            //     <li>
            //     <p>b</p>
            //     </li>
            //     <li>
            //     <p>c</p>
            //     </li>
            //     </ul>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 214, "Container blocks - Lists");
			Helpers.ExecuteTest("- a\n- b\n\n- c", "<ul>\n<li>\n<p>a</p>\n</li>\n<li>\n<p>b</p>\n</li>\n<li>\n<p>c</p>\n</li>\n</ul>");
        }

        // So is this, with a empty second item:
        [TestMethod]
        [TestCategory("Container blocks - Lists")]
        //[Timeout(1000)]
        public void Example215()
        {
            // Example 215
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
            //     <li>
            //     <p>a</p>
            //     </li>
            //     <li></li>
            //     <li>
            //     <p>c</p>
            //     </li>
            //     </ul>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 215, "Container blocks - Lists");
			Helpers.ExecuteTest("* a\n*\n\n* c", "<ul>\n<li>\n<p>a</p>\n</li>\n<li></li>\n<li>\n<p>c</p>\n</li>\n</ul>");
        }

        // These are loose lists, even though there is no space between the items,
        // because one of the items directly contains two block-level elements
        // with a blank line between them:
        [TestMethod]
        [TestCategory("Container blocks - Lists")]
        //[Timeout(1000)]
        public void Example216()
        {
            // Example 216
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
            //     <li>
            //     <p>a</p>
            //     </li>
            //     <li>
            //     <p>b</p>
            //     <p>c</p>
            //     </li>
            //     <li>
            //     <p>d</p>
            //     </li>
            //     </ul>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 216, "Container blocks - Lists");
			Helpers.ExecuteTest("- a\n- b\n\n  c\n- d", "<ul>\n<li>\n<p>a</p>\n</li>\n<li>\n<p>b</p>\n<p>c</p>\n</li>\n<li>\n<p>d</p>\n</li>\n</ul>");
        }

        [TestMethod]
        [TestCategory("Container blocks - Lists")]
        //[Timeout(1000)]
        public void Example217()
        {
            // Example 217
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
            //     <li>
            //     <p>a</p>
            //     </li>
            //     <li>
            //     <p>b</p>
            //     </li>
            //     <li>
            //     <p>d</p>
            //     </li>
            //     </ul>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 217, "Container blocks - Lists");
			Helpers.ExecuteTest("- a\n- b\n\n  [ref]: /url\n- d", "<ul>\n<li>\n<p>a</p>\n</li>\n<li>\n<p>b</p>\n</li>\n<li>\n<p>d</p>\n</li>\n</ul>");
        }

        // This is a tight list, because the blank lines are in a code block:
        [TestMethod]
        [TestCategory("Container blocks - Lists")]
        //[Timeout(1000)]
        public void Example218()
        {
            // Example 218
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
            //     <li>
            //     <pre><code>b
            //     
            //     
            //     </code></pre>
            //     </li>
            //     <li>c</li>
            //     </ul>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 218, "Container blocks - Lists");
			Helpers.ExecuteTest("- a\n- ```\n  b\n\n\n  ```\n- c", "<ul>\n<li>a</li>\n<li>\n<pre><code>b\n\n\n</code></pre>\n</li>\n<li>c</li>\n</ul>");
        }

        // This is a tight list, because the blank line is between two
        // paragraphs of a sublist.  So the sublist is loose while
        // the outer list is tight:
        [TestMethod]
        [TestCategory("Container blocks - Lists")]
        //[Timeout(1000)]
        public void Example219()
        {
            // Example 219
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
            //     <li>
            //     <p>b</p>
            //     <p>c</p>
            //     </li>
            //     </ul>
            //     </li>
            //     <li>d</li>
            //     </ul>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 219, "Container blocks - Lists");
			Helpers.ExecuteTest("- a\n  - b\n\n    c\n- d", "<ul>\n<li>a\n<ul>\n<li>\n<p>b</p>\n<p>c</p>\n</li>\n</ul>\n</li>\n<li>d</li>\n</ul>");
        }

        // This is a tight list, because the blank line is inside the
        // block quote:
        [TestMethod]
        [TestCategory("Container blocks - Lists")]
        //[Timeout(1000)]
        public void Example220()
        {
            // Example 220
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
            //     </blockquote>
            //     </li>
            //     <li>c</li>
            //     </ul>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 220, "Container blocks - Lists");
			Helpers.ExecuteTest("* a\n  > b\n  >\n* c", "<ul>\n<li>a\n<blockquote>\n<p>b</p>\n</blockquote>\n</li>\n<li>c</li>\n</ul>");
        }

        // This list is tight, because the consecutive block elements
        // are not separated by blank lines:
        [TestMethod]
        [TestCategory("Container blocks - Lists")]
        //[Timeout(1000)]
        public void Example221()
        {
            // Example 221
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
            //     </code></pre>
            //     </li>
            //     <li>d</li>
            //     </ul>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 221, "Container blocks - Lists");
			Helpers.ExecuteTest("- a\n  > b\n  ```\n  c\n  ```\n- d", "<ul>\n<li>a\n<blockquote>\n<p>b</p>\n</blockquote>\n<pre><code>c\n</code></pre>\n</li>\n<li>d</li>\n</ul>");
        }

        // A single-paragraph list is tight:
        [TestMethod]
        [TestCategory("Container blocks - Lists")]
        //[Timeout(1000)]
        public void Example222()
        {
            // Example 222
            // Section: Container blocks - Lists
            //
            // The following CommonMark:
            //     - a
            //
            // Should be rendered as:
            //     <ul>
            //     <li>a</li>
            //     </ul>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 222, "Container blocks - Lists");
			Helpers.ExecuteTest("- a", "<ul>\n<li>a</li>\n</ul>");
        }

        [TestMethod]
        [TestCategory("Container blocks - Lists")]
        //[Timeout(1000)]
        public void Example223()
        {
            // Example 223
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
            //     </ul>
            //     </li>
            //     </ul>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 223, "Container blocks - Lists");
			Helpers.ExecuteTest("- a\n  - b", "<ul>\n<li>a\n<ul>\n<li>b</li>\n</ul>\n</li>\n</ul>");
        }

        // This list is loose, because of the blank line between the
        // two block elements in the list item:
        [TestMethod]
        [TestCategory("Container blocks - Lists")]
        //[Timeout(1000)]
        public void Example224()
        {
            // Example 224
            // Section: Container blocks - Lists
            //
            // The following CommonMark:
            //     1. ```
            //        foo
            //        ```
            //     
            //        bar
            //
            // Should be rendered as:
            //     <ol>
            //     <li>
            //     <pre><code>foo
            //     </code></pre>
            //     <p>bar</p>
            //     </li>
            //     </ol>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 224, "Container blocks - Lists");
			Helpers.ExecuteTest("1. ```\n   foo\n   ```\n\n   bar", "<ol>\n<li>\n<pre><code>foo\n</code></pre>\n<p>bar</p>\n</li>\n</ol>");
        }

        // Here the outer list is loose, the inner list tight:
        [TestMethod]
        [TestCategory("Container blocks - Lists")]
        //[Timeout(1000)]
        public void Example225()
        {
            // Example 225
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
            //     <li>
            //     <p>foo</p>
            //     <ul>
            //     <li>bar</li>
            //     </ul>
            //     <p>baz</p>
            //     </li>
            //     </ul>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 225, "Container blocks - Lists");
			Helpers.ExecuteTest("* foo\n  * bar\n\n  baz", "<ul>\n<li>\n<p>foo</p>\n<ul>\n<li>bar</li>\n</ul>\n<p>baz</p>\n</li>\n</ul>");
        }

        [TestMethod]
        [TestCategory("Container blocks - Lists")]
        //[Timeout(1000)]
        public void Example226()
        {
            // Example 226
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
            //     <li>
            //     <p>a</p>
            //     <ul>
            //     <li>b</li>
            //     <li>c</li>
            //     </ul>
            //     </li>
            //     <li>
            //     <p>d</p>
            //     <ul>
            //     <li>e</li>
            //     <li>f</li>
            //     </ul>
            //     </li>
            //     </ul>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 226, "Container blocks - Lists");
			Helpers.ExecuteTest("- a\n  - b\n  - c\n\n- d\n  - e\n  - f", "<ul>\n<li>\n<p>a</p>\n<ul>\n<li>b</li>\n<li>c</li>\n</ul>\n</li>\n<li>\n<p>d</p>\n<ul>\n<li>e</li>\n<li>f</li>\n</ul>\n</li>\n</ul>");
        }

        // # Inlines
        //
        // Inlines are parsed sequentially from the beginning of the character
        // stream to the end (left to right, in left-to-right languages).
        // Thus, for example, in
        [TestMethod]
        [TestCategory("Inlines")]
        //[Timeout(1000)]
        public void Example227()
        {
            // Example 227
            // Section: Inlines
            //
            // The following CommonMark:
            //     `hi`lo`
            //
            // Should be rendered as:
            //     <p><code>hi</code>lo`</p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 227, "Inlines");
			Helpers.ExecuteTest("`hi`lo`", "<p><code>hi</code>lo`</p>");
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
        public void Example228()
        {
            // Example 228
            // Section: Inlines - Backslash escapes
            //
            // The following CommonMark:
            //     \!\"\#\$\%\&\'\(\)\*\+\,\-\.\/\:\;\<\=\>\?\@\[\\\]\^\_\`\{\|\}\~
            //
            // Should be rendered as:
            //     <p>!&quot;#$%&amp;'()*+,-./:;&lt;=&gt;?@[\]^_`{|}~</p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 228, "Inlines - Backslash escapes");
			Helpers.ExecuteTest("\\!\\\"\\#\\$\\%\\&\\'\\(\\)\\*\\+\\,\\-\\.\\/\\:\\;\\<\\=\\>\\?\\@\\[\\\\\\]\\^\\_\\`\\{\\|\\}\\~", "<p>!&quot;#$%&amp;'()*+,-./:;&lt;=&gt;?@[\\]^_`{|}~</p>");
        }

        // Backslashes before other characters are treated as literal
        // backslashes:
        [TestMethod]
        [TestCategory("Inlines - Backslash escapes")]
        //[Timeout(1000)]
        public void Example229()
        {
            // Example 229
            // Section: Inlines - Backslash escapes
            //
            // The following CommonMark:
            //     \→\A\a\ \3\φ\«
            //
            // Should be rendered as:
            //     <p>\   \A\a\ \3\φ\«</p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 229, "Inlines - Backslash escapes");
			Helpers.ExecuteTest("\\→\\A\\a\\ \\3\\φ\\«", "<p>\\   \\A\\a\\ \\3\\φ\\«</p>");
        }

        // Escaped characters are treated as regular characters and do
        // not have their usual Markdown meanings:
        [TestMethod]
        [TestCategory("Inlines - Backslash escapes")]
        //[Timeout(1000)]
        public void Example230()
        {
            // Example 230
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

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 230, "Inlines - Backslash escapes");
			Helpers.ExecuteTest("\\*not emphasized*\n\\<br/> not a tag\n\\[not a link](/foo)\n\\`not code`\n1\\. not a list\n\\* not a list\n\\# not a header\n\\[foo]: /url \"not a reference\"", "<p>*not emphasized*\n&lt;br/&gt; not a tag\n[not a link](/foo)\n`not code`\n1. not a list\n* not a list\n# not a header\n[foo]: /url &quot;not a reference&quot;</p>");
        }

        // If a backslash is itself escaped, the following character is not:
        [TestMethod]
        [TestCategory("Inlines - Backslash escapes")]
        //[Timeout(1000)]
        public void Example231()
        {
            // Example 231
            // Section: Inlines - Backslash escapes
            //
            // The following CommonMark:
            //     \\*emphasis*
            //
            // Should be rendered as:
            //     <p>\<em>emphasis</em></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 231, "Inlines - Backslash escapes");
			Helpers.ExecuteTest("\\\\*emphasis*", "<p>\\<em>emphasis</em></p>");
        }

        // A backslash at the end of the line is a [hard line break]:
        [TestMethod]
        [TestCategory("Inlines - Backslash escapes")]
        //[Timeout(1000)]
        public void Example232()
        {
            // Example 232
            // Section: Inlines - Backslash escapes
            //
            // The following CommonMark:
            //     foo\
            //     bar
            //
            // Should be rendered as:
            //     <p>foo<br />
            //     bar</p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 232, "Inlines - Backslash escapes");
			Helpers.ExecuteTest("foo\\\nbar", "<p>foo<br />\nbar</p>");
        }

        // Backslash escapes do not work in code blocks, code spans, autolinks, or
        // raw HTML:
        [TestMethod]
        [TestCategory("Inlines - Backslash escapes")]
        //[Timeout(1000)]
        public void Example233()
        {
            // Example 233
            // Section: Inlines - Backslash escapes
            //
            // The following CommonMark:
            //     `` \[\` ``
            //
            // Should be rendered as:
            //     <p><code>\[\`</code></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 233, "Inlines - Backslash escapes");
			Helpers.ExecuteTest("`` \\[\\` ``", "<p><code>\\[\\`</code></p>");
        }

        [TestMethod]
        [TestCategory("Inlines - Backslash escapes")]
        //[Timeout(1000)]
        public void Example234()
        {
            // Example 234
            // Section: Inlines - Backslash escapes
            //
            // The following CommonMark:
            //         \[\]
            //
            // Should be rendered as:
            //     <pre><code>\[\]
            //     </code></pre>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 234, "Inlines - Backslash escapes");
			Helpers.ExecuteTest("    \\[\\]", "<pre><code>\\[\\]\n</code></pre>");
        }

        [TestMethod]
        [TestCategory("Inlines - Backslash escapes")]
        //[Timeout(1000)]
        public void Example235()
        {
            // Example 235
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

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 235, "Inlines - Backslash escapes");
			Helpers.ExecuteTest("~~~\n\\[\\]\n~~~", "<pre><code>\\[\\]\n</code></pre>");
        }

        [TestMethod]
        [TestCategory("Inlines - Backslash escapes")]
        //[Timeout(1000)]
        public void Example236()
        {
            // Example 236
            // Section: Inlines - Backslash escapes
            //
            // The following CommonMark:
            //     <http://example.com?find=\*>
            //
            // Should be rendered as:
            //     <p><a href="http://example.com?find=%5C*">http://example.com?find=\*</a></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 236, "Inlines - Backslash escapes");
			Helpers.ExecuteTest("<http://example.com?find=\\*>", "<p><a href=\"http://example.com?find=%5C*\">http://example.com?find=\\*</a></p>");
        }

        [TestMethod]
        [TestCategory("Inlines - Backslash escapes")]
        //[Timeout(1000)]
        public void Example237()
        {
            // Example 237
            // Section: Inlines - Backslash escapes
            //
            // The following CommonMark:
            //     <a href="/bar\/)">
            //
            // Should be rendered as:
            //     <p><a href="/bar\/)"></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 237, "Inlines - Backslash escapes");
			Helpers.ExecuteTest("<a href=\"/bar\\/)\">", "<p><a href=\"/bar\\/)\"></p>");
        }

        // But they work in all other contexts, including URLs and link titles,
        // link references, and [info string]s in [fenced code block]s:
        [TestMethod]
        [TestCategory("Inlines - Backslash escapes")]
        //[Timeout(1000)]
        public void Example238()
        {
            // Example 238
            // Section: Inlines - Backslash escapes
            //
            // The following CommonMark:
            //     [foo](/bar\* "ti\*tle")
            //
            // Should be rendered as:
            //     <p><a href="/bar*" title="ti*tle">foo</a></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 238, "Inlines - Backslash escapes");
			Helpers.ExecuteTest("[foo](/bar\\* \"ti\\*tle\")", "<p><a href=\"/bar*\" title=\"ti*tle\">foo</a></p>");
        }

        [TestMethod]
        [TestCategory("Inlines - Backslash escapes")]
        //[Timeout(1000)]
        public void Example239()
        {
            // Example 239
            // Section: Inlines - Backslash escapes
            //
            // The following CommonMark:
            //     [foo]
            //     
            //     [foo]: /bar\* "ti\*tle"
            //
            // Should be rendered as:
            //     <p><a href="/bar*" title="ti*tle">foo</a></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 239, "Inlines - Backslash escapes");
			Helpers.ExecuteTest("[foo]\n\n[foo]: /bar\\* \"ti\\*tle\"", "<p><a href=\"/bar*\" title=\"ti*tle\">foo</a></p>");
        }

        [TestMethod]
        [TestCategory("Inlines - Backslash escapes")]
        //[Timeout(1000)]
        public void Example240()
        {
            // Example 240
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

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 240, "Inlines - Backslash escapes");
			Helpers.ExecuteTest("``` foo\\+bar\nfoo\n```", "<pre><code class=\"language-foo+bar\">foo\n</code></pre>");
        }

        // ## Entities
        //
        // With the goal of making this standard as HTML-agnostic as possible, all
        // valid HTML entities (except in code blocks and code spans)
        // are recognized as such and converted into unicode characters before
        // they are stored in the AST. This means that renderers to formats other
        // than HTML need not be HTML-entity aware.  HTML renderers may either escape
        // unicode characters as entities or leave them as they are.  (However,
        // `"`, `&`, `<`, and `>` must always be rendered as entities.)
        //
        // [Named entities](@name-entities) consist of `&`
        // + any of the valid HTML5 entity names + `;`. The
        // [following document](https://html.spec.whatwg.org/multipage/entities.json)
        // is used as an authoritative source of the valid entity names and their
        // corresponding codepoints.
        [TestMethod]
        [TestCategory("Inlines - Entities")]
        //[Timeout(1000)]
        public void Example241()
        {
            // Example 241
            // Section: Inlines - Entities
            //
            // The following CommonMark:
            //     &nbsp; &amp; &copy; &AElig; &Dcaron; &frac34; &HilbertSpace; &DifferentialD; &ClockwiseContourIntegral;
            //
            // Should be rendered as:
            //     <p>  &amp; © Æ Ď ¾ ℋ ⅆ ∲</p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 241, "Inlines - Entities");
			Helpers.ExecuteTest("&nbsp; &amp; &copy; &AElig; &Dcaron; &frac34; &HilbertSpace; &DifferentialD; &ClockwiseContourIntegral;", "<p>  &amp; © Æ Ď ¾ ℋ ⅆ ∲</p>");
        }

        // [Decimal entities](@decimal-entities)
        // consist of `&#` + a string of 1--8 arabic digits + `;`. Again, these
        // entities need to be recognised and tranformed into their corresponding
        // UTF8 codepoints. Invalid Unicode codepoints will be written as the
        // "unknown codepoint" character (`0xFFFD`)
        [TestMethod]
        [TestCategory("Inlines - Entities")]
        //[Timeout(1000)]
        public void Example242()
        {
            // Example 242
            // Section: Inlines - Entities
            //
            // The following CommonMark:
            //     &#35; &#1234; &#992; &#98765432;
            //
            // Should be rendered as:
            //     <p># Ӓ Ϡ �</p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 242, "Inlines - Entities");
			Helpers.ExecuteTest("&#35; &#1234; &#992; &#98765432;", "<p># Ӓ Ϡ �</p>");
        }

        // [Hexadecimal entities](@hexadecimal-entities)
        // consist of `&#` + either `X` or `x` + a string of 1-8 hexadecimal digits
        // + `;`. They will also be parsed and turned into their corresponding UTF8 values in the AST.
        [TestMethod]
        [TestCategory("Inlines - Entities")]
        //[Timeout(1000)]
        public void Example243()
        {
            // Example 243
            // Section: Inlines - Entities
            //
            // The following CommonMark:
            //     &#X22; &#XD06; &#xcab;
            //
            // Should be rendered as:
            //     <p>&quot; ആ ಫ</p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 243, "Inlines - Entities");
			Helpers.ExecuteTest("&#X22; &#XD06; &#xcab;", "<p>&quot; ആ ಫ</p>");
        }

        // Here are some nonentities:
        [TestMethod]
        [TestCategory("Inlines - Entities")]
        //[Timeout(1000)]
        public void Example244()
        {
            // Example 244
            // Section: Inlines - Entities
            //
            // The following CommonMark:
            //     &nbsp &x; &#; &#x; &ThisIsWayTooLongToBeAnEntityIsntIt; &hi?;
            //
            // Should be rendered as:
            //     <p>&amp;nbsp &amp;x; &amp;#; &amp;#x; &amp;ThisIsWayTooLongToBeAnEntityIsntIt; &amp;hi?;</p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 244, "Inlines - Entities");
			Helpers.ExecuteTest("&nbsp &x; &#; &#x; &ThisIsWayTooLongToBeAnEntityIsntIt; &hi?;", "<p>&amp;nbsp &amp;x; &amp;#; &amp;#x; &amp;ThisIsWayTooLongToBeAnEntityIsntIt; &amp;hi?;</p>");
        }

        // Although HTML5 does accept some entities without a trailing semicolon
        // (such as `&copy`), these are not recognized as entities here, because it
        // makes the grammar too ambiguous:
        [TestMethod]
        [TestCategory("Inlines - Entities")]
        //[Timeout(1000)]
        public void Example245()
        {
            // Example 245
            // Section: Inlines - Entities
            //
            // The following CommonMark:
            //     &copy
            //
            // Should be rendered as:
            //     <p>&amp;copy</p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 245, "Inlines - Entities");
			Helpers.ExecuteTest("&copy", "<p>&amp;copy</p>");
        }

        // Strings that are not on the list of HTML5 named entities are not
        // recognized as entities either:
        [TestMethod]
        [TestCategory("Inlines - Entities")]
        //[Timeout(1000)]
        public void Example246()
        {
            // Example 246
            // Section: Inlines - Entities
            //
            // The following CommonMark:
            //     &MadeUpEntity;
            //
            // Should be rendered as:
            //     <p>&amp;MadeUpEntity;</p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 246, "Inlines - Entities");
			Helpers.ExecuteTest("&MadeUpEntity;", "<p>&amp;MadeUpEntity;</p>");
        }

        // Entities are recognized in any context besides code spans or
        // code blocks, including raw HTML, URLs, [link title]s, and
        // [fenced code block] [info string]s:
        [TestMethod]
        [TestCategory("Inlines - Entities")]
        //[Timeout(1000)]
        public void Example247()
        {
            // Example 247
            // Section: Inlines - Entities
            //
            // The following CommonMark:
            //     <a href="&ouml;&ouml;.html">
            //
            // Should be rendered as:
            //     <p><a href="&ouml;&ouml;.html"></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 247, "Inlines - Entities");
			Helpers.ExecuteTest("<a href=\"&ouml;&ouml;.html\">", "<p><a href=\"&ouml;&ouml;.html\"></p>");
        }

        [TestMethod]
        [TestCategory("Inlines - Entities")]
        //[Timeout(1000)]
        public void Example248()
        {
            // Example 248
            // Section: Inlines - Entities
            //
            // The following CommonMark:
            //     [foo](/f&ouml;&ouml; "f&ouml;&ouml;")
            //
            // Should be rendered as:
            //     <p><a href="/f%C3%B6%C3%B6" title="föö">foo</a></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 248, "Inlines - Entities");
			Helpers.ExecuteTest("[foo](/f&ouml;&ouml; \"f&ouml;&ouml;\")", "<p><a href=\"/f%C3%B6%C3%B6\" title=\"föö\">foo</a></p>");
        }

        [TestMethod]
        [TestCategory("Inlines - Entities")]
        //[Timeout(1000)]
        public void Example249()
        {
            // Example 249
            // Section: Inlines - Entities
            //
            // The following CommonMark:
            //     [foo]
            //     
            //     [foo]: /f&ouml;&ouml; "f&ouml;&ouml;"
            //
            // Should be rendered as:
            //     <p><a href="/f%C3%B6%C3%B6" title="föö">foo</a></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 249, "Inlines - Entities");
			Helpers.ExecuteTest("[foo]\n\n[foo]: /f&ouml;&ouml; \"f&ouml;&ouml;\"", "<p><a href=\"/f%C3%B6%C3%B6\" title=\"föö\">foo</a></p>");
        }

        [TestMethod]
        [TestCategory("Inlines - Entities")]
        //[Timeout(1000)]
        public void Example250()
        {
            // Example 250
            // Section: Inlines - Entities
            //
            // The following CommonMark:
            //     ``` f&ouml;&ouml;
            //     foo
            //     ```
            //
            // Should be rendered as:
            //     <pre><code class="language-föö">foo
            //     </code></pre>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 250, "Inlines - Entities");
			Helpers.ExecuteTest("``` f&ouml;&ouml;\nfoo\n```", "<pre><code class=\"language-föö\">foo\n</code></pre>");
        }

        // Entities are treated as literal text in code spans and code blocks:
        [TestMethod]
        [TestCategory("Inlines - Entities")]
        //[Timeout(1000)]
        public void Example251()
        {
            // Example 251
            // Section: Inlines - Entities
            //
            // The following CommonMark:
            //     `f&ouml;&ouml;`
            //
            // Should be rendered as:
            //     <p><code>f&amp;ouml;&amp;ouml;</code></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 251, "Inlines - Entities");
			Helpers.ExecuteTest("`f&ouml;&ouml;`", "<p><code>f&amp;ouml;&amp;ouml;</code></p>");
        }

        [TestMethod]
        [TestCategory("Inlines - Entities")]
        //[Timeout(1000)]
        public void Example252()
        {
            // Example 252
            // Section: Inlines - Entities
            //
            // The following CommonMark:
            //         f&ouml;f&ouml;
            //
            // Should be rendered as:
            //     <pre><code>f&amp;ouml;f&amp;ouml;
            //     </code></pre>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 252, "Inlines - Entities");
			Helpers.ExecuteTest("    f&ouml;f&ouml;", "<pre><code>f&amp;ouml;f&amp;ouml;\n</code></pre>");
        }

        // ## Code spans
        //
        // A [backtick string](@backtick-string)
        // is a string of one or more backtick characters (`` ` ``) that is neither
        // preceded nor followed by a backtick.
        //
        // A [code span](@code-span) begins with a backtick string and ends with
        // a backtick string of equal length.  The contents of the code span are
        // the characters between the two backtick strings, with leading and
        // trailing spaces and [line ending]s removed, and
        // [whitespace] collapsed to single spaces.
        //
        // This is a simple code span:
        [TestMethod]
        [TestCategory("Inlines - Code spans")]
        //[Timeout(1000)]
        public void Example253()
        {
            // Example 253
            // Section: Inlines - Code spans
            //
            // The following CommonMark:
            //     `foo`
            //
            // Should be rendered as:
            //     <p><code>foo</code></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 253, "Inlines - Code spans");
			Helpers.ExecuteTest("`foo`", "<p><code>foo</code></p>");
        }

        // Here two backticks are used, because the code contains a backtick.
        // This example also illustrates stripping of leading and trailing spaces:
        [TestMethod]
        [TestCategory("Inlines - Code spans")]
        //[Timeout(1000)]
        public void Example254()
        {
            // Example 254
            // Section: Inlines - Code spans
            //
            // The following CommonMark:
            //     `` foo ` bar  ``
            //
            // Should be rendered as:
            //     <p><code>foo ` bar</code></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 254, "Inlines - Code spans");
			Helpers.ExecuteTest("`` foo ` bar  ``", "<p><code>foo ` bar</code></p>");
        }

        // This example shows the motivation for stripping leading and trailing
        // spaces:
        [TestMethod]
        [TestCategory("Inlines - Code spans")]
        //[Timeout(1000)]
        public void Example255()
        {
            // Example 255
            // Section: Inlines - Code spans
            //
            // The following CommonMark:
            //     ` `` `
            //
            // Should be rendered as:
            //     <p><code>``</code></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 255, "Inlines - Code spans");
			Helpers.ExecuteTest("` `` `", "<p><code>``</code></p>");
        }

        // [Line ending]s are treated like spaces:
        [TestMethod]
        [TestCategory("Inlines - Code spans")]
        //[Timeout(1000)]
        public void Example256()
        {
            // Example 256
            // Section: Inlines - Code spans
            //
            // The following CommonMark:
            //     ``
            //     foo
            //     ``
            //
            // Should be rendered as:
            //     <p><code>foo</code></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 256, "Inlines - Code spans");
			Helpers.ExecuteTest("``\nfoo\n``", "<p><code>foo</code></p>");
        }

        // Interior spaces and [line ending]s are collapsed into
        // single spaces, just as they would be by a browser:
        [TestMethod]
        [TestCategory("Inlines - Code spans")]
        //[Timeout(1000)]
        public void Example257()
        {
            // Example 257
            // Section: Inlines - Code spans
            //
            // The following CommonMark:
            //     `foo   bar
            //       baz`
            //
            // Should be rendered as:
            //     <p><code>foo bar baz</code></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 257, "Inlines - Code spans");
			Helpers.ExecuteTest("`foo   bar\n  baz`", "<p><code>foo bar baz</code></p>");
        }

        // Q: Why not just leave the spaces, since browsers will collapse them
        // anyway?  A:  Because we might be targeting a non-HTML format, and we
        // shouldn't rely on HTML-specific rendering assumptions.
        //
        // (Existing implementations differ in their treatment of internal
        // spaces and [line ending]s.  Some, including `Markdown.pl` and
        // `showdown`, convert an internal [line ending] into a
        // `<br />` tag.  But this makes things difficult for those who like to
        // hard-wrap their paragraphs, since a line break in the midst of a code
        // span will cause an unintended line break in the output.  Others just
        // leave internal spaces as they are, which is fine if only HTML is being
        // targeted.)
        [TestMethod]
        [TestCategory("Inlines - Code spans")]
        //[Timeout(1000)]
        public void Example258()
        {
            // Example 258
            // Section: Inlines - Code spans
            //
            // The following CommonMark:
            //     `foo `` bar`
            //
            // Should be rendered as:
            //     <p><code>foo `` bar</code></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 258, "Inlines - Code spans");
			Helpers.ExecuteTest("`foo `` bar`", "<p><code>foo `` bar</code></p>");
        }

        // Note that backslash escapes do not work in code spans. All backslashes
        // are treated literally:
        [TestMethod]
        [TestCategory("Inlines - Code spans")]
        //[Timeout(1000)]
        public void Example259()
        {
            // Example 259
            // Section: Inlines - Code spans
            //
            // The following CommonMark:
            //     `foo\`bar`
            //
            // Should be rendered as:
            //     <p><code>foo\</code>bar`</p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 259, "Inlines - Code spans");
			Helpers.ExecuteTest("`foo\\`bar`", "<p><code>foo\\</code>bar`</p>");
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
        [TestCategory("Inlines - Code spans")]
        //[Timeout(1000)]
        public void Example260()
        {
            // Example 260
            // Section: Inlines - Code spans
            //
            // The following CommonMark:
            //     *foo`*`
            //
            // Should be rendered as:
            //     <p>*foo<code>*</code></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 260, "Inlines - Code spans");
			Helpers.ExecuteTest("*foo`*`", "<p>*foo<code>*</code></p>");
        }

        // And this is not parsed as a link:
        [TestMethod]
        [TestCategory("Inlines - Code spans")]
        //[Timeout(1000)]
        public void Example261()
        {
            // Example 261
            // Section: Inlines - Code spans
            //
            // The following CommonMark:
            //     [not a `link](/foo`)
            //
            // Should be rendered as:
            //     <p>[not a <code>link](/foo</code>)</p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 261, "Inlines - Code spans");
			Helpers.ExecuteTest("[not a `link](/foo`)", "<p>[not a <code>link](/foo</code>)</p>");
        }

        // Code spans, HTML tags, and autolinks have the same precedence.
        // Thus, this is code:
        [TestMethod]
        [TestCategory("Inlines - Code spans")]
        //[Timeout(1000)]
        public void Example262()
        {
            // Example 262
            // Section: Inlines - Code spans
            //
            // The following CommonMark:
            //     `<a href="`">`
            //
            // Should be rendered as:
            //     <p><code>&lt;a href=&quot;</code>&quot;&gt;`</p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 262, "Inlines - Code spans");
			Helpers.ExecuteTest("`<a href=\"`\">`", "<p><code>&lt;a href=&quot;</code>&quot;&gt;`</p>");
        }

        // But this is an HTML tag:
        [TestMethod]
        [TestCategory("Inlines - Code spans")]
        //[Timeout(1000)]
        public void Example263()
        {
            // Example 263
            // Section: Inlines - Code spans
            //
            // The following CommonMark:
            //     <a href="`">`
            //
            // Should be rendered as:
            //     <p><a href="`">`</p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 263, "Inlines - Code spans");
			Helpers.ExecuteTest("<a href=\"`\">`", "<p><a href=\"`\">`</p>");
        }

        // And this is code:
        [TestMethod]
        [TestCategory("Inlines - Code spans")]
        //[Timeout(1000)]
        public void Example264()
        {
            // Example 264
            // Section: Inlines - Code spans
            //
            // The following CommonMark:
            //     `<http://foo.bar.`baz>`
            //
            // Should be rendered as:
            //     <p><code>&lt;http://foo.bar.</code>baz&gt;`</p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 264, "Inlines - Code spans");
			Helpers.ExecuteTest("`<http://foo.bar.`baz>`", "<p><code>&lt;http://foo.bar.</code>baz&gt;`</p>");
        }

        // But this is an autolink:
        [TestMethod]
        [TestCategory("Inlines - Code spans")]
        //[Timeout(1000)]
        public void Example265()
        {
            // Example 265
            // Section: Inlines - Code spans
            //
            // The following CommonMark:
            //     <http://foo.bar.`baz>`
            //
            // Should be rendered as:
            //     <p><a href="http://foo.bar.%60baz">http://foo.bar.`baz</a>`</p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 265, "Inlines - Code spans");
			Helpers.ExecuteTest("<http://foo.bar.`baz>`", "<p><a href=\"http://foo.bar.%60baz\">http://foo.bar.`baz</a>`</p>");
        }

        // When a backtick string is not closed by a matching backtick string,
        // we just have literal backticks:
        [TestMethod]
        [TestCategory("Inlines - Code spans")]
        //[Timeout(1000)]
        public void Example266()
        {
            // Example 266
            // Section: Inlines - Code spans
            //
            // The following CommonMark:
            //     ```foo``
            //
            // Should be rendered as:
            //     <p>```foo``</p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 266, "Inlines - Code spans");
			Helpers.ExecuteTest("```foo``", "<p>```foo``</p>");
        }

        [TestMethod]
        [TestCategory("Inlines - Code spans")]
        //[Timeout(1000)]
        public void Example267()
        {
            // Example 267
            // Section: Inlines - Code spans
            //
            // The following CommonMark:
            //     `foo
            //
            // Should be rendered as:
            //     <p>`foo</p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 267, "Inlines - Code spans");
			Helpers.ExecuteTest("`foo", "<p>`foo</p>");
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
        // The rules given below capture all of these patterns, while allowing
        // for efficient parsing strategies that do not backtrack.
        //
        // First, some definitions.  A [delimiter run](@delimiter-run) is either
        // a sequence of one or more `*` characters that is not preceded or
        // followed by a `*` character, or a sequence of one or more `_`
        // characters that is not preceded or followed by a `_` character.
        //
        // A [left-flanking delimiter run](@left-flanking-delimiter-run) is
        // a [delimiter run] that is (a) not followed by [unicode whitespace],
        // and (b) either not followed by a [punctuation character], or
        // preceded by [unicode whitespace] or a [punctuation character] or
        // the beginning of a line.
        //
        // A [right-flanking delimiter run](@right-flanking-delimiter-run) is
        // a [delimiter run] that is (a) not preceded by [unicode whitespace],
        // and (b) either not preceded by a [punctuation character], or
        // followed by [unicode whitespace] or a [punctuation character] or
        // the end of a line.
        //
        // Here are some examples of delimiter runs.
        //
        // - left-flanking but not right-flanking:
        //
        // ```
        // ***abc
        // _abc
        // **"abc"
        // _"abc"
        // ```
        //
        // - right-flanking but not left-flanking:
        //
        // ```
        // abc***
        // abc_
        // "abc"**
        // _"abc"
        // ```
        //
        // - Both right and right-flanking:
        //
        // ```
        // abc***def
        // "abc"_"def"
        // ```
        //
        // - Neither right nor right-flanking:
        //
        // ```
        // abc *** def
        // a _ b
        // ```
        //
        // (The idea of distinguishing left-flanking and right-flanking
        // delimiter runs based on the character before and the character
        // after comes from Roopesh Chander's
        // [vfmd](http://www.vfmd.org/vfmd-spec/specification/#procedure-for-identifying-emphasis-tags).
        // vfmd uses the terminology "emphasis indicator string" instead of "delimiter
        // run," and its rules for distinguishing left- and right-flanking runs
        // are a bit more complex than the ones given here.)
        //
        // The following rules define emphasis and strong emphasis:
        //
        // 1.  A single `*` character [can open emphasis](@can-open-emphasis)
        // iff it is part of a [left-flanking delimiter run].
        //
        // 2.  A single `_` character [can open emphasis] iff
        // it is part of a [left-flanking delimiter run]
        // and not part of a [right-flanking delimiter run].
        //
        // 3.  A single `*` character [can close emphasis](@can-close-emphasis)
        // iff it is part of a [right-flanking delimiter run].
        //
        // 4.  A single `_` character [can close emphasis]
        // iff it is part of a [right-flanking delimiter run]
        // and not part of a [left-flanking delimiter run].
        //
        // 5.  A double `**` [can open strong emphasis](@can-open-strong-emphasis)
        // iff it is part of a [left-flanking delimiter run].
        //
        // 6.  A double `__` [can open strong emphasis]
        // iff it is part of a [left-flanking delimiter run]
        // and not part of a [right-flanking delimiter run].
        //
        // 7.  A double `**` [can close strong emphasis](@can-close-strong-emphasis)
        // iff it is part of a [right-flanking delimiter run].
        //
        // 8.  A double `__` [can close strong emphasis]
        // iff it is part of a [right-flanking delimiter run]
        // and not part of a [left-flanking delimiter run].
        //
        // 9.  Emphasis begins with a delimiter that [can open emphasis] and ends
        // with a delimiter that [can close emphasis], and that uses the same
        // character (`_` or `*`) as the opening delimiter.  There must
        // be a nonempty sequence of inlines between the open delimiter
        // and the closing delimiter; these form the contents of the emphasis
        // inline.
        //
        // 10. Strong emphasis begins with a delimiter that
        // [can open strong emphasis] and ends with a delimiter that
        // [can close strong emphasis], and that uses the same character
        // (`_` or `*`) as the opening delimiter.
        // There must be a nonempty sequence of inlines between the open
        // delimiter and the closing delimiter; these form the contents of
        // the strong emphasis inline.
        //
        // 11. A literal `*` character cannot occur at the beginning or end of
        // `*`-delimited emphasis or `**`-delimited strong emphasis, unless it
        // is backslash-escaped.
        //
        // 12. A literal `_` character cannot occur at the beginning or end of
        // `_`-delimited emphasis or `__`-delimited strong emphasis, unless it
        // is backslash-escaped.
        //
        // Where rules 1--12 above are compatible with multiple parsings,
        // the following principles resolve ambiguity:
        //
        // 13. The number of nestings should be minimized. Thus, for example,
        // an interpretation `<strong>...</strong>` is always preferred to
        // `<em><em>...</em></em>`.
        //
        // 14. An interpretation `<strong><em>...</em></strong>` is always
        // preferred to `<em><strong>..</strong></em>`.
        //
        // 15. When two potential emphasis or strong emphasis spans overlap,
        // so that the second begins before the first ends and ends after
        // the first ends, the first takes precedence. Thus, for example,
        // `*foo _bar* baz_` is parsed as `<em>foo _bar</em> baz_` rather
        // than `*foo <em>bar* baz</em>`.  For the same reason,
        // `**foo*bar**` is parsed as `<em><em>foo</em>bar</em>*`
        // rather than `<strong>foo*bar</strong>`.
        //
        // 16. When there are two potential emphasis or strong emphasis spans
        // with the same closing delimiter, the shorter one (the one that
        // opens later) takes precedence. Thus, for example,
        // `**foo **bar baz**` is parsed as `**foo <strong>bar baz</strong>`
        // rather than `<strong>foo **bar baz</strong>`.
        //
        // 17. Inline code spans, links, images, and HTML tags group more tightly
        // than emphasis.  So, when there is a choice between an interpretation
        // that contains one of these elements and one that does not, the
        // former always wins.  Thus, for example, `*[foo*](bar)` is
        // parsed as `*<a href="bar">foo*</a>` rather than as
        // `<em>[foo</em>](bar)`.
        //
        // These rules can be illustrated through a series of examples.
        //
        // Rule 1:
        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example268()
        {
            // Example 268
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     *foo bar*
            //
            // Should be rendered as:
            //     <p><em>foo bar</em></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 268, "Inlines - Emphasis and strong emphasis");
			Helpers.ExecuteTest("*foo bar*", "<p><em>foo bar</em></p>");
        }

        // This is not emphasis, because the opening `*` is followed by
        // whitespace, and hence not part of a [left-flanking delimiter run]:
        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example269()
        {
            // Example 269
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     a * foo bar*
            //
            // Should be rendered as:
            //     <p>a * foo bar*</p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 269, "Inlines - Emphasis and strong emphasis");
			Helpers.ExecuteTest("a * foo bar*", "<p>a * foo bar*</p>");
        }

        // This is not emphasis, because the opening `*` is preceded
        // by an alphanumeric and followed by punctuation, and hence
        // not part of a [left-flanking delimiter run]:
        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example270()
        {
            // Example 270
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     a*"foo"*
            //
            // Should be rendered as:
            //     <p>a*&quot;foo&quot;*</p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 270, "Inlines - Emphasis and strong emphasis");
			Helpers.ExecuteTest("a*\"foo\"*", "<p>a*&quot;foo&quot;*</p>");
        }

        // Unicode nonbreaking spaces count as whitespace, too:
        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example271()
        {
            // Example 271
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     * a *
            //
            // Should be rendered as:
            //     <p>* a *</p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 271, "Inlines - Emphasis and strong emphasis");
			Helpers.ExecuteTest("* a *", "<p>* a *</p>");
        }

        // Intraword emphasis with `*` is permitted:
        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example272()
        {
            // Example 272
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     foo*bar*
            //
            // Should be rendered as:
            //     <p>foo<em>bar</em></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 272, "Inlines - Emphasis and strong emphasis");
			Helpers.ExecuteTest("foo*bar*", "<p>foo<em>bar</em></p>");
        }

        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example273()
        {
            // Example 273
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     5*6*78
            //
            // Should be rendered as:
            //     <p>5<em>6</em>78</p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 273, "Inlines - Emphasis and strong emphasis");
			Helpers.ExecuteTest("5*6*78", "<p>5<em>6</em>78</p>");
        }

        // Rule 2:
        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example274()
        {
            // Example 274
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     _foo bar_
            //
            // Should be rendered as:
            //     <p><em>foo bar</em></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 274, "Inlines - Emphasis and strong emphasis");
			Helpers.ExecuteTest("_foo bar_", "<p><em>foo bar</em></p>");
        }

        // This is not emphasis, because the opening `_` is followed by
        // whitespace:
        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example275()
        {
            // Example 275
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     _ foo bar_
            //
            // Should be rendered as:
            //     <p>_ foo bar_</p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 275, "Inlines - Emphasis and strong emphasis");
			Helpers.ExecuteTest("_ foo bar_", "<p>_ foo bar_</p>");
        }

        // This is not emphasis, because the opening `_` is preceded
        // by an alphanumeric and followed by punctuation:
        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example276()
        {
            // Example 276
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     a_"foo"_
            //
            // Should be rendered as:
            //     <p>a_&quot;foo&quot;_</p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 276, "Inlines - Emphasis and strong emphasis");
			Helpers.ExecuteTest("a_\"foo\"_", "<p>a_&quot;foo&quot;_</p>");
        }

        // Emphasis with `_` is not allowed inside words:
        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example277()
        {
            // Example 277
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     foo_bar_
            //
            // Should be rendered as:
            //     <p>foo_bar_</p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 277, "Inlines - Emphasis and strong emphasis");
			Helpers.ExecuteTest("foo_bar_", "<p>foo_bar_</p>");
        }

        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example278()
        {
            // Example 278
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     5_6_78
            //
            // Should be rendered as:
            //     <p>5_6_78</p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 278, "Inlines - Emphasis and strong emphasis");
			Helpers.ExecuteTest("5_6_78", "<p>5_6_78</p>");
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
            //     пристаням_стремятся_
            //
            // Should be rendered as:
            //     <p>пристаням_стремятся_</p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 279, "Inlines - Emphasis and strong emphasis");
			Helpers.ExecuteTest("пристаням_стремятся_", "<p>пристаням_стремятся_</p>");
        }

        // Here `_` does not generate emphasis, because the first delimiter run
        // is right-flanking and the second left-flanking:
        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example280()
        {
            // Example 280
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     aa_"bb"_cc
            //
            // Should be rendered as:
            //     <p>aa_&quot;bb&quot;_cc</p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 280, "Inlines - Emphasis and strong emphasis");
			Helpers.ExecuteTest("aa_\"bb\"_cc", "<p>aa_&quot;bb&quot;_cc</p>");
        }

        // Here there is no emphasis, because the delimiter runs are
        // both left- and right-flanking:
        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example281()
        {
            // Example 281
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     "aa"_"bb"_"cc"
            //
            // Should be rendered as:
            //     <p>&quot;aa&quot;_&quot;bb&quot;_&quot;cc&quot;</p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 281, "Inlines - Emphasis and strong emphasis");
			Helpers.ExecuteTest("\"aa\"_\"bb\"_\"cc\"", "<p>&quot;aa&quot;_&quot;bb&quot;_&quot;cc&quot;</p>");
        }

        // Rule 3:
        //
        // This is not emphasis, because the closing delimiter does
        // not match the opening delimiter:
        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example282()
        {
            // Example 282
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     _foo*
            //
            // Should be rendered as:
            //     <p>_foo*</p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 282, "Inlines - Emphasis and strong emphasis");
			Helpers.ExecuteTest("_foo*", "<p>_foo*</p>");
        }

        // This is not emphasis, because the closing `*` is preceded by
        // whitespace:
        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example283()
        {
            // Example 283
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     *foo bar *
            //
            // Should be rendered as:
            //     <p>*foo bar *</p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 283, "Inlines - Emphasis and strong emphasis");
			Helpers.ExecuteTest("*foo bar *", "<p>*foo bar *</p>");
        }

        // A newline also counts as whitespace:
        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example284()
        {
            // Example 284
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     *foo bar
            //     *
            //
            // Should be rendered as:
            //     <p>*foo bar</p>
            //     <ul>
            //     <li></li>
            //     </ul>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 284, "Inlines - Emphasis and strong emphasis");
			Helpers.ExecuteTest("*foo bar\n*", "<p>*foo bar</p>\n<ul>\n<li></li>\n</ul>");
        }

        // This is not emphasis, because the second `*` is
        // preceded by punctuation and followed by an alphanumeric
        // (hence it is not part of a [right-flanking delimiter run]:
        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example285()
        {
            // Example 285
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     *(*foo)
            //
            // Should be rendered as:
            //     <p>*(*foo)</p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 285, "Inlines - Emphasis and strong emphasis");
			Helpers.ExecuteTest("*(*foo)", "<p>*(*foo)</p>");
        }

        // The point of this restriction is more easily appreciated
        // with this example:
        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example286()
        {
            // Example 286
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     *(*foo*)*
            //
            // Should be rendered as:
            //     <p><em>(<em>foo</em>)</em></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 286, "Inlines - Emphasis and strong emphasis");
			Helpers.ExecuteTest("*(*foo*)*", "<p><em>(<em>foo</em>)</em></p>");
        }

        // Intraword emphasis with `*` is allowed:
        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example287()
        {
            // Example 287
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     *foo*bar
            //
            // Should be rendered as:
            //     <p><em>foo</em>bar</p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 287, "Inlines - Emphasis and strong emphasis");
			Helpers.ExecuteTest("*foo*bar", "<p><em>foo</em>bar</p>");
        }

        // Rule 4:
        //
        // This is not emphasis, because the closing `_` is preceded by
        // whitespace:
        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example288()
        {
            // Example 288
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     _foo bar _
            //
            // Should be rendered as:
            //     <p>_foo bar _</p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 288, "Inlines - Emphasis and strong emphasis");
			Helpers.ExecuteTest("_foo bar _", "<p>_foo bar _</p>");
        }

        // This is not emphasis, because the second `_` is
        // preceded by punctuation and followed by an alphanumeric:
        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example289()
        {
            // Example 289
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     _(_foo)
            //
            // Should be rendered as:
            //     <p>_(_foo)</p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 289, "Inlines - Emphasis and strong emphasis");
			Helpers.ExecuteTest("_(_foo)", "<p>_(_foo)</p>");
        }

        // This is emphasis within emphasis:
        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example290()
        {
            // Example 290
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     _(_foo_)_
            //
            // Should be rendered as:
            //     <p><em>(<em>foo</em>)</em></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 290, "Inlines - Emphasis and strong emphasis");
			Helpers.ExecuteTest("_(_foo_)_", "<p><em>(<em>foo</em>)</em></p>");
        }

        // Intraword emphasis is disallowed for `_`:
        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example291()
        {
            // Example 291
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     _foo_bar
            //
            // Should be rendered as:
            //     <p>_foo_bar</p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 291, "Inlines - Emphasis and strong emphasis");
			Helpers.ExecuteTest("_foo_bar", "<p>_foo_bar</p>");
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
            //     _пристаням_стремятся
            //
            // Should be rendered as:
            //     <p>_пристаням_стремятся</p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 292, "Inlines - Emphasis and strong emphasis");
			Helpers.ExecuteTest("_пристаням_стремятся", "<p>_пристаням_стремятся</p>");
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
            //     _foo_bar_baz_
            //
            // Should be rendered as:
            //     <p><em>foo_bar_baz</em></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 293, "Inlines - Emphasis and strong emphasis");
			Helpers.ExecuteTest("_foo_bar_baz_", "<p><em>foo_bar_baz</em></p>");
        }

        // Rule 5:
        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example294()
        {
            // Example 294
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     **foo bar**
            //
            // Should be rendered as:
            //     <p><strong>foo bar</strong></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 294, "Inlines - Emphasis and strong emphasis");
			Helpers.ExecuteTest("**foo bar**", "<p><strong>foo bar</strong></p>");
        }

        // This is not strong emphasis, because the opening delimiter is
        // followed by whitespace:
        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example295()
        {
            // Example 295
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     ** foo bar**
            //
            // Should be rendered as:
            //     <p>** foo bar**</p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 295, "Inlines - Emphasis and strong emphasis");
			Helpers.ExecuteTest("** foo bar**", "<p>** foo bar**</p>");
        }

        // This is not strong emphasis, because the opening `**` is preceded
        // by an alphanumeric and followed by punctuation, and hence
        // not part of a [left-flanking delimiter run]:
        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example296()
        {
            // Example 296
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     a**"foo"**
            //
            // Should be rendered as:
            //     <p>a**&quot;foo&quot;**</p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 296, "Inlines - Emphasis and strong emphasis");
			Helpers.ExecuteTest("a**\"foo\"**", "<p>a**&quot;foo&quot;**</p>");
        }

        // Intraword strong emphasis with `**` is permitted:
        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example297()
        {
            // Example 297
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     foo**bar**
            //
            // Should be rendered as:
            //     <p>foo<strong>bar</strong></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 297, "Inlines - Emphasis and strong emphasis");
			Helpers.ExecuteTest("foo**bar**", "<p>foo<strong>bar</strong></p>");
        }

        // Rule 6:
        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example298()
        {
            // Example 298
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     __foo bar__
            //
            // Should be rendered as:
            //     <p><strong>foo bar</strong></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 298, "Inlines - Emphasis and strong emphasis");
			Helpers.ExecuteTest("__foo bar__", "<p><strong>foo bar</strong></p>");
        }

        // This is not strong emphasis, because the opening delimiter is
        // followed by whitespace:
        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example299()
        {
            // Example 299
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     __ foo bar__
            //
            // Should be rendered as:
            //     <p>__ foo bar__</p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 299, "Inlines - Emphasis and strong emphasis");
			Helpers.ExecuteTest("__ foo bar__", "<p>__ foo bar__</p>");
        }

        // A newline counts as whitespace:
        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example300()
        {
            // Example 300
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     __
            //     foo bar__
            //
            // Should be rendered as:
            //     <p>__
            //     foo bar__</p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 300, "Inlines - Emphasis and strong emphasis");
			Helpers.ExecuteTest("__\nfoo bar__", "<p>__\nfoo bar__</p>");
        }

        // This is not strong emphasis, because the opening `__` is preceded
        // by an alphanumeric and followed by punctuation:
        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example301()
        {
            // Example 301
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     a__"foo"__
            //
            // Should be rendered as:
            //     <p>a__&quot;foo&quot;__</p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 301, "Inlines - Emphasis and strong emphasis");
			Helpers.ExecuteTest("a__\"foo\"__", "<p>a__&quot;foo&quot;__</p>");
        }

        // Intraword strong emphasis is forbidden with `__`:
        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example302()
        {
            // Example 302
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     foo__bar__
            //
            // Should be rendered as:
            //     <p>foo__bar__</p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 302, "Inlines - Emphasis and strong emphasis");
			Helpers.ExecuteTest("foo__bar__", "<p>foo__bar__</p>");
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
            //     5__6__78
            //
            // Should be rendered as:
            //     <p>5__6__78</p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 303, "Inlines - Emphasis and strong emphasis");
			Helpers.ExecuteTest("5__6__78", "<p>5__6__78</p>");
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
            //     пристаням__стремятся__
            //
            // Should be rendered as:
            //     <p>пристаням__стремятся__</p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 304, "Inlines - Emphasis and strong emphasis");
			Helpers.ExecuteTest("пристаням__стремятся__", "<p>пристаням__стремятся__</p>");
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
            //     __foo, __bar__, baz__
            //
            // Should be rendered as:
            //     <p><strong>foo, <strong>bar</strong>, baz</strong></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 305, "Inlines - Emphasis and strong emphasis");
			Helpers.ExecuteTest("__foo, __bar__, baz__", "<p><strong>foo, <strong>bar</strong>, baz</strong></p>");
        }

        // Rule 7:
        //
        // This is not strong emphasis, because the closing delimiter is preceded
        // by whitespace:
        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example306()
        {
            // Example 306
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     **foo bar **
            //
            // Should be rendered as:
            //     <p>**foo bar **</p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 306, "Inlines - Emphasis and strong emphasis");
			Helpers.ExecuteTest("**foo bar **", "<p>**foo bar **</p>");
        }

        // (Nor can it be interpreted as an emphasized `*foo bar *`, because of
        // Rule 11.)
        //
        // This is not strong emphasis, because the second `**` is
        // preceded by punctuation and followed by an alphanumeric:
        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example307()
        {
            // Example 307
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     **(**foo)
            //
            // Should be rendered as:
            //     <p>**(**foo)</p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 307, "Inlines - Emphasis and strong emphasis");
			Helpers.ExecuteTest("**(**foo)", "<p>**(**foo)</p>");
        }

        // The point of this restriction is more easily appreciated
        // with these examples:
        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example308()
        {
            // Example 308
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     *(**foo**)*
            //
            // Should be rendered as:
            //     <p><em>(<strong>foo</strong>)</em></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 308, "Inlines - Emphasis and strong emphasis");
			Helpers.ExecuteTest("*(**foo**)*", "<p><em>(<strong>foo</strong>)</em></p>");
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
            //     **Gomphocarpus (*Gomphocarpus physocarpus*, syn.
            //     *Asclepias physocarpa*)**
            //
            // Should be rendered as:
            //     <p><strong>Gomphocarpus (<em>Gomphocarpus physocarpus</em>, syn.
            //     <em>Asclepias physocarpa</em>)</strong></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 309, "Inlines - Emphasis and strong emphasis");
			Helpers.ExecuteTest("**Gomphocarpus (*Gomphocarpus physocarpus*, syn.\n*Asclepias physocarpa*)**", "<p><strong>Gomphocarpus (<em>Gomphocarpus physocarpus</em>, syn.\n<em>Asclepias physocarpa</em>)</strong></p>");
        }

        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example310()
        {
            // Example 310
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     **foo "*bar*" foo**
            //
            // Should be rendered as:
            //     <p><strong>foo &quot;<em>bar</em>&quot; foo</strong></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 310, "Inlines - Emphasis and strong emphasis");
			Helpers.ExecuteTest("**foo \"*bar*\" foo**", "<p><strong>foo &quot;<em>bar</em>&quot; foo</strong></p>");
        }

        // Intraword emphasis:
        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example311()
        {
            // Example 311
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     **foo**bar
            //
            // Should be rendered as:
            //     <p><strong>foo</strong>bar</p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 311, "Inlines - Emphasis and strong emphasis");
			Helpers.ExecuteTest("**foo**bar", "<p><strong>foo</strong>bar</p>");
        }

        // Rule 8:
        //
        // This is not strong emphasis, because the closing delimiter is
        // preceded by whitespace:
        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example312()
        {
            // Example 312
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     __foo bar __
            //
            // Should be rendered as:
            //     <p>__foo bar __</p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 312, "Inlines - Emphasis and strong emphasis");
			Helpers.ExecuteTest("__foo bar __", "<p>__foo bar __</p>");
        }

        // This is not strong emphasis, because the second `__` is
        // preceded by punctuation and followed by an alphanumeric:
        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example313()
        {
            // Example 313
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     __(__foo)
            //
            // Should be rendered as:
            //     <p>__(__foo)</p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 313, "Inlines - Emphasis and strong emphasis");
			Helpers.ExecuteTest("__(__foo)", "<p>__(__foo)</p>");
        }

        // The point of this restriction is more easily appreciated
        // with this example:
        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example314()
        {
            // Example 314
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     _(__foo__)_
            //
            // Should be rendered as:
            //     <p><em>(<strong>foo</strong>)</em></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 314, "Inlines - Emphasis and strong emphasis");
			Helpers.ExecuteTest("_(__foo__)_", "<p><em>(<strong>foo</strong>)</em></p>");
        }

        // Intraword strong emphasis is forbidden with `__`:
        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example315()
        {
            // Example 315
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     __foo__bar
            //
            // Should be rendered as:
            //     <p>__foo__bar</p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 315, "Inlines - Emphasis and strong emphasis");
			Helpers.ExecuteTest("__foo__bar", "<p>__foo__bar</p>");
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
            //     __пристаням__стремятся
            //
            // Should be rendered as:
            //     <p>__пристаням__стремятся</p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 316, "Inlines - Emphasis and strong emphasis");
			Helpers.ExecuteTest("__пристаням__стремятся", "<p>__пристаням__стремятся</p>");
        }

        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example317()
        {
            // Example 317
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     __foo__bar__baz__
            //
            // Should be rendered as:
            //     <p><strong>foo__bar__baz</strong></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 317, "Inlines - Emphasis and strong emphasis");
			Helpers.ExecuteTest("__foo__bar__baz__", "<p><strong>foo__bar__baz</strong></p>");
        }

        // Rule 9:
        //
        // Any nonempty sequence of inline elements can be the contents of an
        // emphasized span.
        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example318()
        {
            // Example 318
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     *foo [bar](/url)*
            //
            // Should be rendered as:
            //     <p><em>foo <a href="/url">bar</a></em></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 318, "Inlines - Emphasis and strong emphasis");
			Helpers.ExecuteTest("*foo [bar](/url)*", "<p><em>foo <a href=\"/url\">bar</a></em></p>");
        }

        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example319()
        {
            // Example 319
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     *foo
            //     bar*
            //
            // Should be rendered as:
            //     <p><em>foo
            //     bar</em></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 319, "Inlines - Emphasis and strong emphasis");
			Helpers.ExecuteTest("*foo\nbar*", "<p><em>foo\nbar</em></p>");
        }

        // In particular, emphasis and strong emphasis can be nested
        // inside emphasis:
        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example320()
        {
            // Example 320
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     _foo __bar__ baz_
            //
            // Should be rendered as:
            //     <p><em>foo <strong>bar</strong> baz</em></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 320, "Inlines - Emphasis and strong emphasis");
			Helpers.ExecuteTest("_foo __bar__ baz_", "<p><em>foo <strong>bar</strong> baz</em></p>");
        }

        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example321()
        {
            // Example 321
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     _foo _bar_ baz_
            //
            // Should be rendered as:
            //     <p><em>foo <em>bar</em> baz</em></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 321, "Inlines - Emphasis and strong emphasis");
			Helpers.ExecuteTest("_foo _bar_ baz_", "<p><em>foo <em>bar</em> baz</em></p>");
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
            //     __foo_ bar_
            //
            // Should be rendered as:
            //     <p><em><em>foo</em> bar</em></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 322, "Inlines - Emphasis and strong emphasis");
			Helpers.ExecuteTest("__foo_ bar_", "<p><em><em>foo</em> bar</em></p>");
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
            //     *foo *bar**
            //
            // Should be rendered as:
            //     <p><em>foo <em>bar</em></em></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 323, "Inlines - Emphasis and strong emphasis");
			Helpers.ExecuteTest("*foo *bar**", "<p><em>foo <em>bar</em></em></p>");
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
            //     *foo **bar** baz*
            //
            // Should be rendered as:
            //     <p><em>foo <strong>bar</strong> baz</em></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 324, "Inlines - Emphasis and strong emphasis");
			Helpers.ExecuteTest("*foo **bar** baz*", "<p><em>foo <strong>bar</strong> baz</em></p>");
        }

        // But note:
        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example325()
        {
            // Example 325
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     *foo**bar**baz*
            //
            // Should be rendered as:
            //     <p><em>foo</em><em>bar</em><em>baz</em></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 325, "Inlines - Emphasis and strong emphasis");
			Helpers.ExecuteTest("*foo**bar**baz*", "<p><em>foo</em><em>bar</em><em>baz</em></p>");
        }

        // The difference is that in the preceding case, the internal delimiters
        // [can close emphasis], while in the cases with spaces, they cannot.
        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example326()
        {
            // Example 326
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     ***foo** bar*
            //
            // Should be rendered as:
            //     <p><em><strong>foo</strong> bar</em></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 326, "Inlines - Emphasis and strong emphasis");
			Helpers.ExecuteTest("***foo** bar*", "<p><em><strong>foo</strong> bar</em></p>");
        }

        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example327()
        {
            // Example 327
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     *foo **bar***
            //
            // Should be rendered as:
            //     <p><em>foo <strong>bar</strong></em></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 327, "Inlines - Emphasis and strong emphasis");
			Helpers.ExecuteTest("*foo **bar***", "<p><em>foo <strong>bar</strong></em></p>");
        }

        // Note, however, that in the following case we get no strong
        // emphasis, because the opening delimiter is closed by the first
        // `*` before `bar`:
        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example328()
        {
            // Example 328
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     *foo**bar***
            //
            // Should be rendered as:
            //     <p><em>foo</em><em>bar</em>**</p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 328, "Inlines - Emphasis and strong emphasis");
			Helpers.ExecuteTest("*foo**bar***", "<p><em>foo</em><em>bar</em>**</p>");
        }

        // Indefinite levels of nesting are possible:
        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example329()
        {
            // Example 329
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     *foo **bar *baz* bim** bop*
            //
            // Should be rendered as:
            //     <p><em>foo <strong>bar <em>baz</em> bim</strong> bop</em></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 329, "Inlines - Emphasis and strong emphasis");
			Helpers.ExecuteTest("*foo **bar *baz* bim** bop*", "<p><em>foo <strong>bar <em>baz</em> bim</strong> bop</em></p>");
        }

        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example330()
        {
            // Example 330
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     *foo [*bar*](/url)*
            //
            // Should be rendered as:
            //     <p><em>foo <a href="/url"><em>bar</em></a></em></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 330, "Inlines - Emphasis and strong emphasis");
			Helpers.ExecuteTest("*foo [*bar*](/url)*", "<p><em>foo <a href=\"/url\"><em>bar</em></a></em></p>");
        }

        // There can be no empty emphasis or strong emphasis:
        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example331()
        {
            // Example 331
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     ** is not an empty emphasis
            //
            // Should be rendered as:
            //     <p>** is not an empty emphasis</p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 331, "Inlines - Emphasis and strong emphasis");
			Helpers.ExecuteTest("** is not an empty emphasis", "<p>** is not an empty emphasis</p>");
        }

        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example332()
        {
            // Example 332
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     **** is not an empty strong emphasis
            //
            // Should be rendered as:
            //     <p>**** is not an empty strong emphasis</p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 332, "Inlines - Emphasis and strong emphasis");
			Helpers.ExecuteTest("**** is not an empty strong emphasis", "<p>**** is not an empty strong emphasis</p>");
        }

        // Rule 10:
        //
        // Any nonempty sequence of inline elements can be the contents of an
        // strongly emphasized span.
        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example333()
        {
            // Example 333
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     **foo [bar](/url)**
            //
            // Should be rendered as:
            //     <p><strong>foo <a href="/url">bar</a></strong></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 333, "Inlines - Emphasis and strong emphasis");
			Helpers.ExecuteTest("**foo [bar](/url)**", "<p><strong>foo <a href=\"/url\">bar</a></strong></p>");
        }

        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example334()
        {
            // Example 334
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     **foo
            //     bar**
            //
            // Should be rendered as:
            //     <p><strong>foo
            //     bar</strong></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 334, "Inlines - Emphasis and strong emphasis");
			Helpers.ExecuteTest("**foo\nbar**", "<p><strong>foo\nbar</strong></p>");
        }

        // In particular, emphasis and strong emphasis can be nested
        // inside strong emphasis:
        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example335()
        {
            // Example 335
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     __foo _bar_ baz__
            //
            // Should be rendered as:
            //     <p><strong>foo <em>bar</em> baz</strong></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 335, "Inlines - Emphasis and strong emphasis");
			Helpers.ExecuteTest("__foo _bar_ baz__", "<p><strong>foo <em>bar</em> baz</strong></p>");
        }

        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example336()
        {
            // Example 336
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     __foo __bar__ baz__
            //
            // Should be rendered as:
            //     <p><strong>foo <strong>bar</strong> baz</strong></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 336, "Inlines - Emphasis and strong emphasis");
			Helpers.ExecuteTest("__foo __bar__ baz__", "<p><strong>foo <strong>bar</strong> baz</strong></p>");
        }

        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example337()
        {
            // Example 337
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     ____foo__ bar__
            //
            // Should be rendered as:
            //     <p><strong><strong>foo</strong> bar</strong></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 337, "Inlines - Emphasis and strong emphasis");
			Helpers.ExecuteTest("____foo__ bar__", "<p><strong><strong>foo</strong> bar</strong></p>");
        }

        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example338()
        {
            // Example 338
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     **foo **bar****
            //
            // Should be rendered as:
            //     <p><strong>foo <strong>bar</strong></strong></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 338, "Inlines - Emphasis and strong emphasis");
			Helpers.ExecuteTest("**foo **bar****", "<p><strong>foo <strong>bar</strong></strong></p>");
        }

        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example339()
        {
            // Example 339
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     **foo *bar* baz**
            //
            // Should be rendered as:
            //     <p><strong>foo <em>bar</em> baz</strong></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 339, "Inlines - Emphasis and strong emphasis");
			Helpers.ExecuteTest("**foo *bar* baz**", "<p><strong>foo <em>bar</em> baz</strong></p>");
        }

        // But note:
        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example340()
        {
            // Example 340
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     **foo*bar*baz**
            //
            // Should be rendered as:
            //     <p><em><em>foo</em>bar</em>baz**</p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 340, "Inlines - Emphasis and strong emphasis");
			Helpers.ExecuteTest("**foo*bar*baz**", "<p><em><em>foo</em>bar</em>baz**</p>");
        }

        // The difference is that in the preceding case, the internal delimiters
        // [can close emphasis], while in the cases with spaces, they cannot.
        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example341()
        {
            // Example 341
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     ***foo* bar**
            //
            // Should be rendered as:
            //     <p><strong><em>foo</em> bar</strong></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 341, "Inlines - Emphasis and strong emphasis");
			Helpers.ExecuteTest("***foo* bar**", "<p><strong><em>foo</em> bar</strong></p>");
        }

        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example342()
        {
            // Example 342
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     **foo *bar***
            //
            // Should be rendered as:
            //     <p><strong>foo <em>bar</em></strong></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 342, "Inlines - Emphasis and strong emphasis");
			Helpers.ExecuteTest("**foo *bar***", "<p><strong>foo <em>bar</em></strong></p>");
        }

        // Indefinite levels of nesting are possible:
        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example343()
        {
            // Example 343
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     **foo *bar **baz**
            //     bim* bop**
            //
            // Should be rendered as:
            //     <p><strong>foo <em>bar <strong>baz</strong>
            //     bim</em> bop</strong></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 343, "Inlines - Emphasis and strong emphasis");
			Helpers.ExecuteTest("**foo *bar **baz**\nbim* bop**", "<p><strong>foo <em>bar <strong>baz</strong>\nbim</em> bop</strong></p>");
        }

        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example344()
        {
            // Example 344
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     **foo [*bar*](/url)**
            //
            // Should be rendered as:
            //     <p><strong>foo <a href="/url"><em>bar</em></a></strong></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 344, "Inlines - Emphasis and strong emphasis");
			Helpers.ExecuteTest("**foo [*bar*](/url)**", "<p><strong>foo <a href=\"/url\"><em>bar</em></a></strong></p>");
        }

        // There can be no empty emphasis or strong emphasis:
        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example345()
        {
            // Example 345
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     __ is not an empty emphasis
            //
            // Should be rendered as:
            //     <p>__ is not an empty emphasis</p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 345, "Inlines - Emphasis and strong emphasis");
			Helpers.ExecuteTest("__ is not an empty emphasis", "<p>__ is not an empty emphasis</p>");
        }

        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example346()
        {
            // Example 346
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     ____ is not an empty strong emphasis
            //
            // Should be rendered as:
            //     <p>____ is not an empty strong emphasis</p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 346, "Inlines - Emphasis and strong emphasis");
			Helpers.ExecuteTest("____ is not an empty strong emphasis", "<p>____ is not an empty strong emphasis</p>");
        }

        // Rule 11:
        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example347()
        {
            // Example 347
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     foo ***
            //
            // Should be rendered as:
            //     <p>foo ***</p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 347, "Inlines - Emphasis and strong emphasis");
			Helpers.ExecuteTest("foo ***", "<p>foo ***</p>");
        }

        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example348()
        {
            // Example 348
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     foo *\**
            //
            // Should be rendered as:
            //     <p>foo <em>*</em></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 348, "Inlines - Emphasis and strong emphasis");
			Helpers.ExecuteTest("foo *\\**", "<p>foo <em>*</em></p>");
        }

        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example349()
        {
            // Example 349
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     foo *_*
            //
            // Should be rendered as:
            //     <p>foo <em>_</em></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 349, "Inlines - Emphasis and strong emphasis");
			Helpers.ExecuteTest("foo *_*", "<p>foo <em>_</em></p>");
        }

        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example350()
        {
            // Example 350
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     foo *****
            //
            // Should be rendered as:
            //     <p>foo *****</p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 350, "Inlines - Emphasis and strong emphasis");
			Helpers.ExecuteTest("foo *****", "<p>foo *****</p>");
        }

        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example351()
        {
            // Example 351
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     foo **\***
            //
            // Should be rendered as:
            //     <p>foo <strong>*</strong></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 351, "Inlines - Emphasis and strong emphasis");
			Helpers.ExecuteTest("foo **\\***", "<p>foo <strong>*</strong></p>");
        }

        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example352()
        {
            // Example 352
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     foo **_**
            //
            // Should be rendered as:
            //     <p>foo <strong>_</strong></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 352, "Inlines - Emphasis and strong emphasis");
			Helpers.ExecuteTest("foo **_**", "<p>foo <strong>_</strong></p>");
        }

        // Note that when delimiters do not match evenly, Rule 11 determines
        // that the excess literal `*` characters will appear outside of the
        // emphasis, rather than inside it:
        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example353()
        {
            // Example 353
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     **foo*
            //
            // Should be rendered as:
            //     <p>*<em>foo</em></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 353, "Inlines - Emphasis and strong emphasis");
			Helpers.ExecuteTest("**foo*", "<p>*<em>foo</em></p>");
        }

        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example354()
        {
            // Example 354
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     *foo**
            //
            // Should be rendered as:
            //     <p><em>foo</em>*</p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 354, "Inlines - Emphasis and strong emphasis");
			Helpers.ExecuteTest("*foo**", "<p><em>foo</em>*</p>");
        }

        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example355()
        {
            // Example 355
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     ***foo**
            //
            // Should be rendered as:
            //     <p>*<strong>foo</strong></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 355, "Inlines - Emphasis and strong emphasis");
			Helpers.ExecuteTest("***foo**", "<p>*<strong>foo</strong></p>");
        }

        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example356()
        {
            // Example 356
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     ****foo*
            //
            // Should be rendered as:
            //     <p>***<em>foo</em></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 356, "Inlines - Emphasis and strong emphasis");
			Helpers.ExecuteTest("****foo*", "<p>***<em>foo</em></p>");
        }

        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example357()
        {
            // Example 357
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     **foo***
            //
            // Should be rendered as:
            //     <p><strong>foo</strong>*</p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 357, "Inlines - Emphasis and strong emphasis");
			Helpers.ExecuteTest("**foo***", "<p><strong>foo</strong>*</p>");
        }

        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example358()
        {
            // Example 358
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     *foo****
            //
            // Should be rendered as:
            //     <p><em>foo</em>***</p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 358, "Inlines - Emphasis and strong emphasis");
			Helpers.ExecuteTest("*foo****", "<p><em>foo</em>***</p>");
        }

        // Rule 12:
        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example359()
        {
            // Example 359
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     foo ___
            //
            // Should be rendered as:
            //     <p>foo ___</p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 359, "Inlines - Emphasis and strong emphasis");
			Helpers.ExecuteTest("foo ___", "<p>foo ___</p>");
        }

        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example360()
        {
            // Example 360
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     foo _\__
            //
            // Should be rendered as:
            //     <p>foo <em>_</em></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 360, "Inlines - Emphasis and strong emphasis");
			Helpers.ExecuteTest("foo _\\__", "<p>foo <em>_</em></p>");
        }

        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example361()
        {
            // Example 361
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     foo _*_
            //
            // Should be rendered as:
            //     <p>foo <em>*</em></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 361, "Inlines - Emphasis and strong emphasis");
			Helpers.ExecuteTest("foo _*_", "<p>foo <em>*</em></p>");
        }

        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example362()
        {
            // Example 362
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     foo _____
            //
            // Should be rendered as:
            //     <p>foo _____</p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 362, "Inlines - Emphasis and strong emphasis");
			Helpers.ExecuteTest("foo _____", "<p>foo _____</p>");
        }

        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example363()
        {
            // Example 363
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     foo __\___
            //
            // Should be rendered as:
            //     <p>foo <strong>_</strong></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 363, "Inlines - Emphasis and strong emphasis");
			Helpers.ExecuteTest("foo __\\___", "<p>foo <strong>_</strong></p>");
        }

        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example364()
        {
            // Example 364
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     foo __*__
            //
            // Should be rendered as:
            //     <p>foo <strong>*</strong></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 364, "Inlines - Emphasis and strong emphasis");
			Helpers.ExecuteTest("foo __*__", "<p>foo <strong>*</strong></p>");
        }

        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example365()
        {
            // Example 365
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     __foo_
            //
            // Should be rendered as:
            //     <p>_<em>foo</em></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 365, "Inlines - Emphasis and strong emphasis");
			Helpers.ExecuteTest("__foo_", "<p>_<em>foo</em></p>");
        }

        // Note that when delimiters do not match evenly, Rule 12 determines
        // that the excess literal `_` characters will appear outside of the
        // emphasis, rather than inside it:
        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example366()
        {
            // Example 366
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     _foo__
            //
            // Should be rendered as:
            //     <p><em>foo</em>_</p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 366, "Inlines - Emphasis and strong emphasis");
			Helpers.ExecuteTest("_foo__", "<p><em>foo</em>_</p>");
        }

        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example367()
        {
            // Example 367
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     ___foo__
            //
            // Should be rendered as:
            //     <p>_<strong>foo</strong></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 367, "Inlines - Emphasis and strong emphasis");
			Helpers.ExecuteTest("___foo__", "<p>_<strong>foo</strong></p>");
        }

        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example368()
        {
            // Example 368
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     ____foo_
            //
            // Should be rendered as:
            //     <p>___<em>foo</em></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 368, "Inlines - Emphasis and strong emphasis");
			Helpers.ExecuteTest("____foo_", "<p>___<em>foo</em></p>");
        }

        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example369()
        {
            // Example 369
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     __foo___
            //
            // Should be rendered as:
            //     <p><strong>foo</strong>_</p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 369, "Inlines - Emphasis and strong emphasis");
			Helpers.ExecuteTest("__foo___", "<p><strong>foo</strong>_</p>");
        }

        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example370()
        {
            // Example 370
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     _foo____
            //
            // Should be rendered as:
            //     <p><em>foo</em>___</p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 370, "Inlines - Emphasis and strong emphasis");
			Helpers.ExecuteTest("_foo____", "<p><em>foo</em>___</p>");
        }

        // Rule 13 implies that if you want emphasis nested directly inside
        // emphasis, you must use different delimiters:
        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example371()
        {
            // Example 371
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     **foo**
            //
            // Should be rendered as:
            //     <p><strong>foo</strong></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 371, "Inlines - Emphasis and strong emphasis");
			Helpers.ExecuteTest("**foo**", "<p><strong>foo</strong></p>");
        }

        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example372()
        {
            // Example 372
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     *_foo_*
            //
            // Should be rendered as:
            //     <p><em><em>foo</em></em></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 372, "Inlines - Emphasis and strong emphasis");
			Helpers.ExecuteTest("*_foo_*", "<p><em><em>foo</em></em></p>");
        }

        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example373()
        {
            // Example 373
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     __foo__
            //
            // Should be rendered as:
            //     <p><strong>foo</strong></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 373, "Inlines - Emphasis and strong emphasis");
			Helpers.ExecuteTest("__foo__", "<p><strong>foo</strong></p>");
        }

        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example374()
        {
            // Example 374
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     _*foo*_
            //
            // Should be rendered as:
            //     <p><em><em>foo</em></em></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 374, "Inlines - Emphasis and strong emphasis");
			Helpers.ExecuteTest("_*foo*_", "<p><em><em>foo</em></em></p>");
        }

        // However, strong emphasis within strong emphasis is possible without
        // switching delimiters:
        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example375()
        {
            // Example 375
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     ****foo****
            //
            // Should be rendered as:
            //     <p><strong><strong>foo</strong></strong></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 375, "Inlines - Emphasis and strong emphasis");
			Helpers.ExecuteTest("****foo****", "<p><strong><strong>foo</strong></strong></p>");
        }

        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example376()
        {
            // Example 376
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     ____foo____
            //
            // Should be rendered as:
            //     <p><strong><strong>foo</strong></strong></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 376, "Inlines - Emphasis and strong emphasis");
			Helpers.ExecuteTest("____foo____", "<p><strong><strong>foo</strong></strong></p>");
        }

        // Rule 13 can be applied to arbitrarily long sequences of
        // delimiters:
        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example377()
        {
            // Example 377
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     ******foo******
            //
            // Should be rendered as:
            //     <p><strong><strong><strong>foo</strong></strong></strong></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 377, "Inlines - Emphasis and strong emphasis");
			Helpers.ExecuteTest("******foo******", "<p><strong><strong><strong>foo</strong></strong></strong></p>");
        }

        // Rule 14:
        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example378()
        {
            // Example 378
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     ***foo***
            //
            // Should be rendered as:
            //     <p><strong><em>foo</em></strong></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 378, "Inlines - Emphasis and strong emphasis");
			Helpers.ExecuteTest("***foo***", "<p><strong><em>foo</em></strong></p>");
        }

        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example379()
        {
            // Example 379
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     _____foo_____
            //
            // Should be rendered as:
            //     <p><strong><strong><em>foo</em></strong></strong></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 379, "Inlines - Emphasis and strong emphasis");
			Helpers.ExecuteTest("_____foo_____", "<p><strong><strong><em>foo</em></strong></strong></p>");
        }

        // Rule 15:
        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example380()
        {
            // Example 380
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     *foo _bar* baz_
            //
            // Should be rendered as:
            //     <p><em>foo _bar</em> baz_</p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 380, "Inlines - Emphasis and strong emphasis");
			Helpers.ExecuteTest("*foo _bar* baz_", "<p><em>foo _bar</em> baz_</p>");
        }

        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example381()
        {
            // Example 381
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     **foo*bar**
            //
            // Should be rendered as:
            //     <p><em><em>foo</em>bar</em>*</p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 381, "Inlines - Emphasis and strong emphasis");
			Helpers.ExecuteTest("**foo*bar**", "<p><em><em>foo</em>bar</em>*</p>");
        }

        // Rule 16:
        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example382()
        {
            // Example 382
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     **foo **bar baz**
            //
            // Should be rendered as:
            //     <p>**foo <strong>bar baz</strong></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 382, "Inlines - Emphasis and strong emphasis");
			Helpers.ExecuteTest("**foo **bar baz**", "<p>**foo <strong>bar baz</strong></p>");
        }

        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example383()
        {
            // Example 383
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     *foo *bar baz*
            //
            // Should be rendered as:
            //     <p>*foo <em>bar baz</em></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 383, "Inlines - Emphasis and strong emphasis");
			Helpers.ExecuteTest("*foo *bar baz*", "<p>*foo <em>bar baz</em></p>");
        }

        // Rule 17:
        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example384()
        {
            // Example 384
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     *[bar*](/url)
            //
            // Should be rendered as:
            //     <p>*<a href="/url">bar*</a></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 384, "Inlines - Emphasis and strong emphasis");
			Helpers.ExecuteTest("*[bar*](/url)", "<p>*<a href=\"/url\">bar*</a></p>");
        }

        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example385()
        {
            // Example 385
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     _foo [bar_](/url)
            //
            // Should be rendered as:
            //     <p>_foo <a href="/url">bar_</a></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 385, "Inlines - Emphasis and strong emphasis");
			Helpers.ExecuteTest("_foo [bar_](/url)", "<p>_foo <a href=\"/url\">bar_</a></p>");
        }

        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example386()
        {
            // Example 386
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     *<img src="foo" title="*"/>
            //
            // Should be rendered as:
            //     <p>*<img src="foo" title="*"/></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 386, "Inlines - Emphasis and strong emphasis");
			Helpers.ExecuteTest("*<img src=\"foo\" title=\"*\"/>", "<p>*<img src=\"foo\" title=\"*\"/></p>");
        }

        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example387()
        {
            // Example 387
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     **<a href="**">
            //
            // Should be rendered as:
            //     <p>**<a href="**"></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 387, "Inlines - Emphasis and strong emphasis");
			Helpers.ExecuteTest("**<a href=\"**\">", "<p>**<a href=\"**\"></p>");
        }

        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example388()
        {
            // Example 388
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     __<a href="__">
            //
            // Should be rendered as:
            //     <p>__<a href="__"></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 388, "Inlines - Emphasis and strong emphasis");
			Helpers.ExecuteTest("__<a href=\"__\">", "<p>__<a href=\"__\"></p>");
        }

        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example389()
        {
            // Example 389
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     *a `*`*
            //
            // Should be rendered as:
            //     <p><em>a <code>*</code></em></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 389, "Inlines - Emphasis and strong emphasis");
			Helpers.ExecuteTest("*a `*`*", "<p><em>a <code>*</code></em></p>");
        }

        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example390()
        {
            // Example 390
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     _a `_`_
            //
            // Should be rendered as:
            //     <p><em>a <code>_</code></em></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 390, "Inlines - Emphasis and strong emphasis");
			Helpers.ExecuteTest("_a `_`_", "<p><em>a <code>_</code></em></p>");
        }

        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example391()
        {
            // Example 391
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     **a<http://foo.bar/?q=**>
            //
            // Should be rendered as:
            //     <p>**a<a href="http://foo.bar/?q=**">http://foo.bar/?q=**</a></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 391, "Inlines - Emphasis and strong emphasis");
			Helpers.ExecuteTest("**a<http://foo.bar/?q=**>", "<p>**a<a href=\"http://foo.bar/?q=**\">http://foo.bar/?q=**</a></p>");
        }

        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        //[Timeout(1000)]
        public void Example392()
        {
            // Example 392
            // Section: Inlines - Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     __a<http://foo.bar/?q=__>
            //
            // Should be rendered as:
            //     <p>__a<a href="http://foo.bar/?q=__">http://foo.bar/?q=__</a></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 392, "Inlines - Emphasis and strong emphasis");
			Helpers.ExecuteTest("__a<http://foo.bar/?q=__>", "<p>__a<a href=\"http://foo.bar/?q=__\">http://foo.bar/?q=__</a></p>");
        }

        // ## Links
        //
        // A link contains [link text] (the visible text), a [link destination]
        // (the URI that is the link destination), and optionally a [link title].
        // There are two basic kinds of links in Markdown.  In [inline link]s the
        // destination and title are given immediately after the link text.  In
        // [reference link]s the destination and title are defined elsewhere in
        // the document.
        //
        // A [link text](@link-text) consists of a sequence of zero or more
        // inline elements enclosed by square brackets (`[` and `]`).  The
        // following rules apply:
        //
        // - Links may not contain other links, at any level of nesting.
        //
        // - Brackets are allowed in the [link text] only if (a) they
        // are backslash-escaped or (b) they appear as a matched pair of brackets,
        // with an open bracket `[`, a sequence of zero or more inlines, and
        // a close bracket `]`.
        //
        // - Backtick [code span]s, [autolink]s, and raw [HTML tag]s bind more tightly
        // than the brackets in link text.  Thus, for example,
        // `` [foo`]` `` could not be a link text, since the second `]`
        // is part of a code span.
        //
        // - The brackets in link text bind more tightly than markers for
        // [emphasis and strong emphasis]. Thus, for example, `*[foo*](url)` is a link.
        //
        // A [link destination](@link-destination) consists of either
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
        // A [link title](@link-title)  consists of either
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
        // Although [link title]s may span multiple lines, they may not contain
        // a [blank line].
        //
        // An [inline link](@inline-link) consists of a [link text] followed immediately
        // by a left parenthesis `(`, optional [whitespace], an optional
        // [link destination], an optional [link title] separated from the link
        // destination by [whitespace], optional [whitespace], and a right
        // parenthesis `)`. The link's text consists of the inlines contained
        // in the [link text] (excluding the enclosing square brackets).
        // The link's URI consists of the link destination, excluding enclosing
        // `<...>` if present, with backslash-escapes in effect as described
        // above.  The link's title consists of the link title, excluding its
        // enclosing delimiters, with backslash-escapes in effect as described
        // above.
        //
        // Here is a simple inline link:
        [TestMethod]
        [TestCategory("Inlines - Links")]
        //[Timeout(1000)]
        public void Example393()
        {
            // Example 393
            // Section: Inlines - Links
            //
            // The following CommonMark:
            //     [link](/uri "title")
            //
            // Should be rendered as:
            //     <p><a href="/uri" title="title">link</a></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 393, "Inlines - Links");
			Helpers.ExecuteTest("[link](/uri \"title\")", "<p><a href=\"/uri\" title=\"title\">link</a></p>");
        }

        // The title may be omitted:
        [TestMethod]
        [TestCategory("Inlines - Links")]
        //[Timeout(1000)]
        public void Example394()
        {
            // Example 394
            // Section: Inlines - Links
            //
            // The following CommonMark:
            //     [link](/uri)
            //
            // Should be rendered as:
            //     <p><a href="/uri">link</a></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 394, "Inlines - Links");
			Helpers.ExecuteTest("[link](/uri)", "<p><a href=\"/uri\">link</a></p>");
        }

        // Both the title and the destination may be omitted:
        [TestMethod]
        [TestCategory("Inlines - Links")]
        //[Timeout(1000)]
        public void Example395()
        {
            // Example 395
            // Section: Inlines - Links
            //
            // The following CommonMark:
            //     [link]()
            //
            // Should be rendered as:
            //     <p><a href="">link</a></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 395, "Inlines - Links");
			Helpers.ExecuteTest("[link]()", "<p><a href=\"\">link</a></p>");
        }

        [TestMethod]
        [TestCategory("Inlines - Links")]
        //[Timeout(1000)]
        public void Example396()
        {
            // Example 396
            // Section: Inlines - Links
            //
            // The following CommonMark:
            //     [link](<>)
            //
            // Should be rendered as:
            //     <p><a href="">link</a></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 396, "Inlines - Links");
			Helpers.ExecuteTest("[link](<>)", "<p><a href=\"\">link</a></p>");
        }

        // If the destination contains spaces, it must be enclosed in pointy
        // braces:
        [TestMethod]
        [TestCategory("Inlines - Links")]
        //[Timeout(1000)]
        public void Example397()
        {
            // Example 397
            // Section: Inlines - Links
            //
            // The following CommonMark:
            //     [link](/my uri)
            //
            // Should be rendered as:
            //     <p>[link](/my uri)</p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 397, "Inlines - Links");
			Helpers.ExecuteTest("[link](/my uri)", "<p>[link](/my uri)</p>");
        }

        [TestMethod]
        [TestCategory("Inlines - Links")]
        //[Timeout(1000)]
        public void Example398()
        {
            // Example 398
            // Section: Inlines - Links
            //
            // The following CommonMark:
            //     [link](</my uri>)
            //
            // Should be rendered as:
            //     <p><a href="/my%20uri">link</a></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 398, "Inlines - Links");
			Helpers.ExecuteTest("[link](</my uri>)", "<p><a href=\"/my%20uri\">link</a></p>");
        }

        // The destination cannot contain line breaks, even with pointy braces:
        [TestMethod]
        [TestCategory("Inlines - Links")]
        //[Timeout(1000)]
        public void Example399()
        {
            // Example 399
            // Section: Inlines - Links
            //
            // The following CommonMark:
            //     [link](foo
            //     bar)
            //
            // Should be rendered as:
            //     <p>[link](foo
            //     bar)</p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 399, "Inlines - Links");
			Helpers.ExecuteTest("[link](foo\nbar)", "<p>[link](foo\nbar)</p>");
        }

        [TestMethod]
        [TestCategory("Inlines - Links")]
        //[Timeout(1000)]
        public void Example400()
        {
            // Example 400
            // Section: Inlines - Links
            //
            // The following CommonMark:
            //     [link](<foo
            //     bar>)
            //
            // Should be rendered as:
            //     <p>[link](<foo
            //     bar>)</p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 400, "Inlines - Links");
			Helpers.ExecuteTest("[link](<foo\nbar>)", "<p>[link](<foo\nbar>)</p>");
        }

        // One level of balanced parentheses is allowed without escaping:
        [TestMethod]
        [TestCategory("Inlines - Links")]
        //[Timeout(1000)]
        public void Example401()
        {
            // Example 401
            // Section: Inlines - Links
            //
            // The following CommonMark:
            //     [link]((foo)and(bar))
            //
            // Should be rendered as:
            //     <p><a href="(foo)and(bar)">link</a></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 401, "Inlines - Links");
			Helpers.ExecuteTest("[link]((foo)and(bar))", "<p><a href=\"(foo)and(bar)\">link</a></p>");
        }

        // However, if you have parentheses within parentheses, you need to escape
        // or use the `<...>` form:
        [TestMethod]
        [TestCategory("Inlines - Links")]
        //[Timeout(1000)]
        public void Example402()
        {
            // Example 402
            // Section: Inlines - Links
            //
            // The following CommonMark:
            //     [link](foo(and(bar)))
            //
            // Should be rendered as:
            //     <p>[link](foo(and(bar)))</p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 402, "Inlines - Links");
			Helpers.ExecuteTest("[link](foo(and(bar)))", "<p>[link](foo(and(bar)))</p>");
        }

        [TestMethod]
        [TestCategory("Inlines - Links")]
        //[Timeout(1000)]
        public void Example403()
        {
            // Example 403
            // Section: Inlines - Links
            //
            // The following CommonMark:
            //     [link](foo(and\(bar\)))
            //
            // Should be rendered as:
            //     <p><a href="foo(and(bar))">link</a></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 403, "Inlines - Links");
			Helpers.ExecuteTest("[link](foo(and\\(bar\\)))", "<p><a href=\"foo(and(bar))\">link</a></p>");
        }

        [TestMethod]
        [TestCategory("Inlines - Links")]
        //[Timeout(1000)]
        public void Example404()
        {
            // Example 404
            // Section: Inlines - Links
            //
            // The following CommonMark:
            //     [link](<foo(and(bar))>)
            //
            // Should be rendered as:
            //     <p><a href="foo(and(bar))">link</a></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 404, "Inlines - Links");
			Helpers.ExecuteTest("[link](<foo(and(bar))>)", "<p><a href=\"foo(and(bar))\">link</a></p>");
        }

        // Parentheses and other symbols can also be escaped, as usual
        // in Markdown:
        [TestMethod]
        [TestCategory("Inlines - Links")]
        //[Timeout(1000)]
        public void Example405()
        {
            // Example 405
            // Section: Inlines - Links
            //
            // The following CommonMark:
            //     [link](foo\)\:)
            //
            // Should be rendered as:
            //     <p><a href="foo):">link</a></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 405, "Inlines - Links");
			Helpers.ExecuteTest("[link](foo\\)\\:)", "<p><a href=\"foo):\">link</a></p>");
        }

        // URL-escaping should be left alone inside the destination, as all
        // URL-escaped characters are also valid URL characters. HTML entities in
        // the destination will be parsed into their UTF-8 codepoints, as usual, and
        // optionally URL-escaped when written as HTML.
        [TestMethod]
        [TestCategory("Inlines - Links")]
        //[Timeout(1000)]
        public void Example406()
        {
            // Example 406
            // Section: Inlines - Links
            //
            // The following CommonMark:
            //     [link](foo%20b&auml;)
            //
            // Should be rendered as:
            //     <p><a href="foo%20b%C3%A4">link</a></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 406, "Inlines - Links");
			Helpers.ExecuteTest("[link](foo%20b&auml;)", "<p><a href=\"foo%20b%C3%A4\">link</a></p>");
        }

        // Note that, because titles can often be parsed as destinations,
        // if you try to omit the destination and keep the title, you'll
        // get unexpected results:
        [TestMethod]
        [TestCategory("Inlines - Links")]
        //[Timeout(1000)]
        public void Example407()
        {
            // Example 407
            // Section: Inlines - Links
            //
            // The following CommonMark:
            //     [link]("title")
            //
            // Should be rendered as:
            //     <p><a href="%22title%22">link</a></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 407, "Inlines - Links");
			Helpers.ExecuteTest("[link](\"title\")", "<p><a href=\"%22title%22\">link</a></p>");
        }

        // Titles may be in single quotes, double quotes, or parentheses:
        [TestMethod]
        [TestCategory("Inlines - Links")]
        //[Timeout(1000)]
        public void Example408()
        {
            // Example 408
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

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 408, "Inlines - Links");
			Helpers.ExecuteTest("[link](/url \"title\")\n[link](/url 'title')\n[link](/url (title))", "<p><a href=\"/url\" title=\"title\">link</a>\n<a href=\"/url\" title=\"title\">link</a>\n<a href=\"/url\" title=\"title\">link</a></p>");
        }

        // Backslash escapes and entities may be used in titles:
        [TestMethod]
        [TestCategory("Inlines - Links")]
        //[Timeout(1000)]
        public void Example409()
        {
            // Example 409
            // Section: Inlines - Links
            //
            // The following CommonMark:
            //     [link](/url "title \"&quot;")
            //
            // Should be rendered as:
            //     <p><a href="/url" title="title &quot;&quot;">link</a></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 409, "Inlines - Links");
			Helpers.ExecuteTest("[link](/url \"title \\\"&quot;\")", "<p><a href=\"/url\" title=\"title &quot;&quot;\">link</a></p>");
        }

        // Nested balanced quotes are not allowed without escaping:
        [TestMethod]
        [TestCategory("Inlines - Links")]
        //[Timeout(1000)]
        public void Example410()
        {
            // Example 410
            // Section: Inlines - Links
            //
            // The following CommonMark:
            //     [link](/url "title "and" title")
            //
            // Should be rendered as:
            //     <p>[link](/url &quot;title &quot;and&quot; title&quot;)</p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 410, "Inlines - Links");
			Helpers.ExecuteTest("[link](/url \"title \"and\" title\")", "<p>[link](/url &quot;title &quot;and&quot; title&quot;)</p>");
        }

        // But it is easy to work around this by using a different quote type:
        [TestMethod]
        [TestCategory("Inlines - Links")]
        //[Timeout(1000)]
        public void Example411()
        {
            // Example 411
            // Section: Inlines - Links
            //
            // The following CommonMark:
            //     [link](/url 'title "and" title')
            //
            // Should be rendered as:
            //     <p><a href="/url" title="title &quot;and&quot; title">link</a></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 411, "Inlines - Links");
			Helpers.ExecuteTest("[link](/url 'title \"and\" title')", "<p><a href=\"/url\" title=\"title &quot;and&quot; title\">link</a></p>");
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
        // [Whitespace] is allowed around the destination and title:
        [TestMethod]
        [TestCategory("Inlines - Links")]
        //[Timeout(1000)]
        public void Example412()
        {
            // Example 412
            // Section: Inlines - Links
            //
            // The following CommonMark:
            //     [link](   /uri
            //       "title"  )
            //
            // Should be rendered as:
            //     <p><a href="/uri" title="title">link</a></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 412, "Inlines - Links");
			Helpers.ExecuteTest("[link](   /uri\n  \"title\"  )", "<p><a href=\"/uri\" title=\"title\">link</a></p>");
        }

        // But it is not allowed between the link text and the
        // following parenthesis:
        [TestMethod]
        [TestCategory("Inlines - Links")]
        //[Timeout(1000)]
        public void Example413()
        {
            // Example 413
            // Section: Inlines - Links
            //
            // The following CommonMark:
            //     [link] (/uri)
            //
            // Should be rendered as:
            //     <p>[link] (/uri)</p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 413, "Inlines - Links");
			Helpers.ExecuteTest("[link] (/uri)", "<p>[link] (/uri)</p>");
        }

        // The link text may contain balanced brackets, but not unbalanced ones,
        // unless they are escaped:
        [TestMethod]
        [TestCategory("Inlines - Links")]
        //[Timeout(1000)]
        public void Example414()
        {
            // Example 414
            // Section: Inlines - Links
            //
            // The following CommonMark:
            //     [link [foo [bar]]](/uri)
            //
            // Should be rendered as:
            //     <p><a href="/uri">link [foo [bar]]</a></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 414, "Inlines - Links");
			Helpers.ExecuteTest("[link [foo [bar]]](/uri)", "<p><a href=\"/uri\">link [foo [bar]]</a></p>");
        }

        [TestMethod]
        [TestCategory("Inlines - Links")]
        //[Timeout(1000)]
        public void Example415()
        {
            // Example 415
            // Section: Inlines - Links
            //
            // The following CommonMark:
            //     [link] bar](/uri)
            //
            // Should be rendered as:
            //     <p>[link] bar](/uri)</p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 415, "Inlines - Links");
			Helpers.ExecuteTest("[link] bar](/uri)", "<p>[link] bar](/uri)</p>");
        }

        [TestMethod]
        [TestCategory("Inlines - Links")]
        //[Timeout(1000)]
        public void Example416()
        {
            // Example 416
            // Section: Inlines - Links
            //
            // The following CommonMark:
            //     [link [bar](/uri)
            //
            // Should be rendered as:
            //     <p>[link <a href="/uri">bar</a></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 416, "Inlines - Links");
			Helpers.ExecuteTest("[link [bar](/uri)", "<p>[link <a href=\"/uri\">bar</a></p>");
        }

        [TestMethod]
        [TestCategory("Inlines - Links")]
        //[Timeout(1000)]
        public void Example417()
        {
            // Example 417
            // Section: Inlines - Links
            //
            // The following CommonMark:
            //     [link \[bar](/uri)
            //
            // Should be rendered as:
            //     <p><a href="/uri">link [bar</a></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 417, "Inlines - Links");
			Helpers.ExecuteTest("[link \\[bar](/uri)", "<p><a href=\"/uri\">link [bar</a></p>");
        }

        // The link text may contain inline content:
        [TestMethod]
        [TestCategory("Inlines - Links")]
        //[Timeout(1000)]
        public void Example418()
        {
            // Example 418
            // Section: Inlines - Links
            //
            // The following CommonMark:
            //     [link *foo **bar** `#`*](/uri)
            //
            // Should be rendered as:
            //     <p><a href="/uri">link <em>foo <strong>bar</strong> <code>#</code></em></a></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 418, "Inlines - Links");
			Helpers.ExecuteTest("[link *foo **bar** `#`*](/uri)", "<p><a href=\"/uri\">link <em>foo <strong>bar</strong> <code>#</code></em></a></p>");
        }

        [TestMethod]
        [TestCategory("Inlines - Links")]
        //[Timeout(1000)]
        public void Example419()
        {
            // Example 419
            // Section: Inlines - Links
            //
            // The following CommonMark:
            //     [![moon](moon.jpg)](/uri)
            //
            // Should be rendered as:
            //     <p><a href="/uri"><img src="moon.jpg" alt="moon" /></a></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 419, "Inlines - Links");
			Helpers.ExecuteTest("[![moon](moon.jpg)](/uri)", "<p><a href=\"/uri\"><img src=\"moon.jpg\" alt=\"moon\" /></a></p>");
        }

        // However, links may not contain other links, at any level of nesting.
        [TestMethod]
        [TestCategory("Inlines - Links")]
        //[Timeout(1000)]
        public void Example420()
        {
            // Example 420
            // Section: Inlines - Links
            //
            // The following CommonMark:
            //     [foo [bar](/uri)](/uri)
            //
            // Should be rendered as:
            //     <p>[foo <a href="/uri">bar</a>](/uri)</p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 420, "Inlines - Links");
			Helpers.ExecuteTest("[foo [bar](/uri)](/uri)", "<p>[foo <a href=\"/uri\">bar</a>](/uri)</p>");
        }

        [TestMethod]
        [TestCategory("Inlines - Links")]
        //[Timeout(1000)]
        public void Example421()
        {
            // Example 421
            // Section: Inlines - Links
            //
            // The following CommonMark:
            //     [foo *[bar [baz](/uri)](/uri)*](/uri)
            //
            // Should be rendered as:
            //     <p>[foo <em>[bar <a href="/uri">baz</a>](/uri)</em>](/uri)</p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 421, "Inlines - Links");
			Helpers.ExecuteTest("[foo *[bar [baz](/uri)](/uri)*](/uri)", "<p>[foo <em>[bar <a href=\"/uri\">baz</a>](/uri)</em>](/uri)</p>");
        }

        [TestMethod]
        [TestCategory("Inlines - Links")]
        //[Timeout(1000)]
        public void Example422()
        {
            // Example 422
            // Section: Inlines - Links
            //
            // The following CommonMark:
            //     ![[[foo](uri1)](uri2)](uri3)
            //
            // Should be rendered as:
            //     <p><img src="uri3" alt="[foo](uri2)" /></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 422, "Inlines - Links");
			Helpers.ExecuteTest("![[[foo](uri1)](uri2)](uri3)", "<p><img src=\"uri3\" alt=\"[foo](uri2)\" /></p>");
        }

        // These cases illustrate the precedence of link text grouping over
        // emphasis grouping:
        [TestMethod]
        [TestCategory("Inlines - Links")]
        //[Timeout(1000)]
        public void Example423()
        {
            // Example 423
            // Section: Inlines - Links
            //
            // The following CommonMark:
            //     *[foo*](/uri)
            //
            // Should be rendered as:
            //     <p>*<a href="/uri">foo*</a></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 423, "Inlines - Links");
			Helpers.ExecuteTest("*[foo*](/uri)", "<p>*<a href=\"/uri\">foo*</a></p>");
        }

        [TestMethod]
        [TestCategory("Inlines - Links")]
        //[Timeout(1000)]
        public void Example424()
        {
            // Example 424
            // Section: Inlines - Links
            //
            // The following CommonMark:
            //     [foo *bar](baz*)
            //
            // Should be rendered as:
            //     <p><a href="baz*">foo *bar</a></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 424, "Inlines - Links");
			Helpers.ExecuteTest("[foo *bar](baz*)", "<p><a href=\"baz*\">foo *bar</a></p>");
        }

        // Note that brackets that *aren't* part of links do not take
        // precedence:
        [TestMethod]
        [TestCategory("Inlines - Links")]
        //[Timeout(1000)]
        public void Example425()
        {
            // Example 425
            // Section: Inlines - Links
            //
            // The following CommonMark:
            //     *foo [bar* baz]
            //
            // Should be rendered as:
            //     <p><em>foo [bar</em> baz]</p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 425, "Inlines - Links");
			Helpers.ExecuteTest("*foo [bar* baz]", "<p><em>foo [bar</em> baz]</p>");
        }

        // These cases illustrate the precedence of HTML tags, code spans,
        // and autolinks over link grouping:
        [TestMethod]
        [TestCategory("Inlines - Links")]
        //[Timeout(1000)]
        public void Example426()
        {
            // Example 426
            // Section: Inlines - Links
            //
            // The following CommonMark:
            //     [foo <bar attr="](baz)">
            //
            // Should be rendered as:
            //     <p>[foo <bar attr="](baz)"></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 426, "Inlines - Links");
			Helpers.ExecuteTest("[foo <bar attr=\"](baz)\">", "<p>[foo <bar attr=\"](baz)\"></p>");
        }

        [TestMethod]
        [TestCategory("Inlines - Links")]
        //[Timeout(1000)]
        public void Example427()
        {
            // Example 427
            // Section: Inlines - Links
            //
            // The following CommonMark:
            //     [foo`](/uri)`
            //
            // Should be rendered as:
            //     <p>[foo<code>](/uri)</code></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 427, "Inlines - Links");
			Helpers.ExecuteTest("[foo`](/uri)`", "<p>[foo<code>](/uri)</code></p>");
        }

        [TestMethod]
        [TestCategory("Inlines - Links")]
        //[Timeout(1000)]
        public void Example428()
        {
            // Example 428
            // Section: Inlines - Links
            //
            // The following CommonMark:
            //     [foo<http://example.com/?search=](uri)>
            //
            // Should be rendered as:
            //     <p>[foo<a href="http://example.com/?search=%5D(uri)">http://example.com/?search=](uri)</a></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 428, "Inlines - Links");
			Helpers.ExecuteTest("[foo<http://example.com/?search=](uri)>", "<p>[foo<a href=\"http://example.com/?search=%5D(uri)\">http://example.com/?search=](uri)</a></p>");
        }

        // There are three kinds of [reference link](@reference-link)s:
        // [full](#full-reference-link), [collapsed](#collapsed-reference-link),
        // and [shortcut](#shortcut-reference-link).
        //
        // A [full reference link](@full-reference-link)
        // consists of a [link text], optional [whitespace], and a [link label]
        // that [matches] a [link reference definition] elsewhere in the document.
        //
        // A [link label](@link-label)  begins with a left bracket (`[`) and ends
        // with the first right bracket (`]`) that is not backslash-escaped.
        // Unescaped square bracket characters are not allowed in
        // [link label]s.  A link label can have at most 999
        // characters inside the square brackets.
        //
        // One label [matches](@matches)
        // another just in case their normalized forms are equal.  To normalize a
        // label, perform the *unicode case fold* and collapse consecutive internal
        // [whitespace] to a single space.  If there are multiple
        // matching reference link definitions, the one that comes first in the
        // document is used.  (It is desirable in such cases to emit a warning.)
        //
        // The contents of the first link label are parsed as inlines, which are
        // used as the link's text.  The link's URI and title are provided by the
        // matching [link reference definition].
        //
        // Here is a simple example:
        [TestMethod]
        [TestCategory("Inlines - Links")]
        //[Timeout(1000)]
        public void Example429()
        {
            // Example 429
            // Section: Inlines - Links
            //
            // The following CommonMark:
            //     [foo][bar]
            //     
            //     [bar]: /url "title"
            //
            // Should be rendered as:
            //     <p><a href="/url" title="title">foo</a></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 429, "Inlines - Links");
			Helpers.ExecuteTest("[foo][bar]\n\n[bar]: /url \"title\"", "<p><a href=\"/url\" title=\"title\">foo</a></p>");
        }

        // The rules for the [link text] are the same as with
        // [inline link]s.  Thus:
        //
        // The link text may contain balanced brackets, but not unbalanced ones,
        // unless they are escaped:
        [TestMethod]
        [TestCategory("Inlines - Links")]
        //[Timeout(1000)]
        public void Example430()
        {
            // Example 430
            // Section: Inlines - Links
            //
            // The following CommonMark:
            //     [link [foo [bar]]][ref]
            //     
            //     [ref]: /uri
            //
            // Should be rendered as:
            //     <p><a href="/uri">link [foo [bar]]</a></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 430, "Inlines - Links");
			Helpers.ExecuteTest("[link [foo [bar]]][ref]\n\n[ref]: /uri", "<p><a href=\"/uri\">link [foo [bar]]</a></p>");
        }

        [TestMethod]
        [TestCategory("Inlines - Links")]
        //[Timeout(1000)]
        public void Example431()
        {
            // Example 431
            // Section: Inlines - Links
            //
            // The following CommonMark:
            //     [link \[bar][ref]
            //     
            //     [ref]: /uri
            //
            // Should be rendered as:
            //     <p><a href="/uri">link [bar</a></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 431, "Inlines - Links");
			Helpers.ExecuteTest("[link \\[bar][ref]\n\n[ref]: /uri", "<p><a href=\"/uri\">link [bar</a></p>");
        }

        // The link text may contain inline content:
        [TestMethod]
        [TestCategory("Inlines - Links")]
        //[Timeout(1000)]
        public void Example432()
        {
            // Example 432
            // Section: Inlines - Links
            //
            // The following CommonMark:
            //     [link *foo **bar** `#`*][ref]
            //     
            //     [ref]: /uri
            //
            // Should be rendered as:
            //     <p><a href="/uri">link <em>foo <strong>bar</strong> <code>#</code></em></a></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 432, "Inlines - Links");
			Helpers.ExecuteTest("[link *foo **bar** `#`*][ref]\n\n[ref]: /uri", "<p><a href=\"/uri\">link <em>foo <strong>bar</strong> <code>#</code></em></a></p>");
        }

        [TestMethod]
        [TestCategory("Inlines - Links")]
        //[Timeout(1000)]
        public void Example433()
        {
            // Example 433
            // Section: Inlines - Links
            //
            // The following CommonMark:
            //     [![moon](moon.jpg)][ref]
            //     
            //     [ref]: /uri
            //
            // Should be rendered as:
            //     <p><a href="/uri"><img src="moon.jpg" alt="moon" /></a></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 433, "Inlines - Links");
			Helpers.ExecuteTest("[![moon](moon.jpg)][ref]\n\n[ref]: /uri", "<p><a href=\"/uri\"><img src=\"moon.jpg\" alt=\"moon\" /></a></p>");
        }

        // However, links may not contain other links, at any level of nesting.
        [TestMethod]
        [TestCategory("Inlines - Links")]
        //[Timeout(1000)]
        public void Example434()
        {
            // Example 434
            // Section: Inlines - Links
            //
            // The following CommonMark:
            //     [foo [bar](/uri)][ref]
            //     
            //     [ref]: /uri
            //
            // Should be rendered as:
            //     <p>[foo <a href="/uri">bar</a>]<a href="/uri">ref</a></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 434, "Inlines - Links");
			Helpers.ExecuteTest("[foo [bar](/uri)][ref]\n\n[ref]: /uri", "<p>[foo <a href=\"/uri\">bar</a>]<a href=\"/uri\">ref</a></p>");
        }

        [TestMethod]
        [TestCategory("Inlines - Links")]
        //[Timeout(1000)]
        public void Example435()
        {
            // Example 435
            // Section: Inlines - Links
            //
            // The following CommonMark:
            //     [foo *bar [baz][ref]*][ref]
            //     
            //     [ref]: /uri
            //
            // Should be rendered as:
            //     <p>[foo <em>bar <a href="/uri">baz</a></em>]<a href="/uri">ref</a></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 435, "Inlines - Links");
			Helpers.ExecuteTest("[foo *bar [baz][ref]*][ref]\n\n[ref]: /uri", "<p>[foo <em>bar <a href=\"/uri\">baz</a></em>]<a href=\"/uri\">ref</a></p>");
        }

        // (In the examples above, we have two [shortcut reference link]s
        // instead of one [full reference link].)
        //
        // The following cases illustrate the precedence of link text grouping over
        // emphasis grouping:
        [TestMethod]
        [TestCategory("Inlines - Links")]
        //[Timeout(1000)]
        public void Example436()
        {
            // Example 436
            // Section: Inlines - Links
            //
            // The following CommonMark:
            //     *[foo*][ref]
            //     
            //     [ref]: /uri
            //
            // Should be rendered as:
            //     <p>*<a href="/uri">foo*</a></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 436, "Inlines - Links");
			Helpers.ExecuteTest("*[foo*][ref]\n\n[ref]: /uri", "<p>*<a href=\"/uri\">foo*</a></p>");
        }

        [TestMethod]
        [TestCategory("Inlines - Links")]
        //[Timeout(1000)]
        public void Example437()
        {
            // Example 437
            // Section: Inlines - Links
            //
            // The following CommonMark:
            //     [foo *bar][ref]
            //     
            //     [ref]: /uri
            //
            // Should be rendered as:
            //     <p><a href="/uri">foo *bar</a></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 437, "Inlines - Links");
			Helpers.ExecuteTest("[foo *bar][ref]\n\n[ref]: /uri", "<p><a href=\"/uri\">foo *bar</a></p>");
        }

        // These cases illustrate the precedence of HTML tags, code spans,
        // and autolinks over link grouping:
        [TestMethod]
        [TestCategory("Inlines - Links")]
        //[Timeout(1000)]
        public void Example438()
        {
            // Example 438
            // Section: Inlines - Links
            //
            // The following CommonMark:
            //     [foo <bar attr="][ref]">
            //     
            //     [ref]: /uri
            //
            // Should be rendered as:
            //     <p>[foo <bar attr="][ref]"></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 438, "Inlines - Links");
			Helpers.ExecuteTest("[foo <bar attr=\"][ref]\">\n\n[ref]: /uri", "<p>[foo <bar attr=\"][ref]\"></p>");
        }

        [TestMethod]
        [TestCategory("Inlines - Links")]
        //[Timeout(1000)]
        public void Example439()
        {
            // Example 439
            // Section: Inlines - Links
            //
            // The following CommonMark:
            //     [foo`][ref]`
            //     
            //     [ref]: /uri
            //
            // Should be rendered as:
            //     <p>[foo<code>][ref]</code></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 439, "Inlines - Links");
			Helpers.ExecuteTest("[foo`][ref]`\n\n[ref]: /uri", "<p>[foo<code>][ref]</code></p>");
        }

        [TestMethod]
        [TestCategory("Inlines - Links")]
        //[Timeout(1000)]
        public void Example440()
        {
            // Example 440
            // Section: Inlines - Links
            //
            // The following CommonMark:
            //     [foo<http://example.com/?search=][ref]>
            //     
            //     [ref]: /uri
            //
            // Should be rendered as:
            //     <p>[foo<a href="http://example.com/?search=%5D%5Bref%5D">http://example.com/?search=][ref]</a></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 440, "Inlines - Links");
			Helpers.ExecuteTest("[foo<http://example.com/?search=][ref]>\n\n[ref]: /uri", "<p>[foo<a href=\"http://example.com/?search=%5D%5Bref%5D\">http://example.com/?search=][ref]</a></p>");
        }

        // Matching is case-insensitive:
        [TestMethod]
        [TestCategory("Inlines - Links")]
        //[Timeout(1000)]
        public void Example441()
        {
            // Example 441
            // Section: Inlines - Links
            //
            // The following CommonMark:
            //     [foo][BaR]
            //     
            //     [bar]: /url "title"
            //
            // Should be rendered as:
            //     <p><a href="/url" title="title">foo</a></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 441, "Inlines - Links");
			Helpers.ExecuteTest("[foo][BaR]\n\n[bar]: /url \"title\"", "<p><a href=\"/url\" title=\"title\">foo</a></p>");
        }

        // Unicode case fold is used:
        [TestMethod]
        [TestCategory("Inlines - Links")]
        //[Timeout(1000)]
        public void Example442()
        {
            // Example 442
            // Section: Inlines - Links
            //
            // The following CommonMark:
            //     [Толпой][Толпой] is a Russian word.
            //     
            //     [ТОЛПОЙ]: /url
            //
            // Should be rendered as:
            //     <p><a href="/url">Толпой</a> is a Russian word.</p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 442, "Inlines - Links");
			Helpers.ExecuteTest("[Толпой][Толпой] is a Russian word.\n\n[ТОЛПОЙ]: /url", "<p><a href=\"/url\">Толпой</a> is a Russian word.</p>");
        }

        // Consecutive internal [whitespace] is treated as one space for
        // purposes of determining matching:
        [TestMethod]
        [TestCategory("Inlines - Links")]
        //[Timeout(1000)]
        public void Example443()
        {
            // Example 443
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

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 443, "Inlines - Links");
			Helpers.ExecuteTest("[Foo\n  bar]: /url\n\n[Baz][Foo bar]", "<p><a href=\"/url\">Baz</a></p>");
        }

        // There can be [whitespace] between the [link text] and the [link label]:
        [TestMethod]
        [TestCategory("Inlines - Links")]
        //[Timeout(1000)]
        public void Example444()
        {
            // Example 444
            // Section: Inlines - Links
            //
            // The following CommonMark:
            //     [foo] [bar]
            //     
            //     [bar]: /url "title"
            //
            // Should be rendered as:
            //     <p><a href="/url" title="title">foo</a></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 444, "Inlines - Links");
			Helpers.ExecuteTest("[foo] [bar]\n\n[bar]: /url \"title\"", "<p><a href=\"/url\" title=\"title\">foo</a></p>");
        }

        [TestMethod]
        [TestCategory("Inlines - Links")]
        //[Timeout(1000)]
        public void Example445()
        {
            // Example 445
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

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 445, "Inlines - Links");
			Helpers.ExecuteTest("[foo]\n[bar]\n\n[bar]: /url \"title\"", "<p><a href=\"/url\" title=\"title\">foo</a></p>");
        }

        // When there are multiple matching [link reference definition]s,
        // the first is used:
        [TestMethod]
        [TestCategory("Inlines - Links")]
        //[Timeout(1000)]
        public void Example446()
        {
            // Example 446
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

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 446, "Inlines - Links");
			Helpers.ExecuteTest("[foo]: /url1\n\n[foo]: /url2\n\n[bar][foo]", "<p><a href=\"/url1\">bar</a></p>");
        }

        // Note that matching is performed on normalized strings, not parsed
        // inline content.  So the following does not match, even though the
        // labels define equivalent inline content:
        [TestMethod]
        [TestCategory("Inlines - Links")]
        //[Timeout(1000)]
        public void Example447()
        {
            // Example 447
            // Section: Inlines - Links
            //
            // The following CommonMark:
            //     [bar][foo\!]
            //     
            //     [foo!]: /url
            //
            // Should be rendered as:
            //     <p>[bar][foo!]</p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 447, "Inlines - Links");
			Helpers.ExecuteTest("[bar][foo\\!]\n\n[foo!]: /url", "<p>[bar][foo!]</p>");
        }

        // [Link label]s cannot contain brackets, unless they are
        // backslash-escaped:
        [TestMethod]
        [TestCategory("Inlines - Links")]
        //[Timeout(1000)]
        public void Example448()
        {
            // Example 448
            // Section: Inlines - Links
            //
            // The following CommonMark:
            //     [foo][ref[]
            //     
            //     [ref[]: /uri
            //
            // Should be rendered as:
            //     <p>[foo][ref[]</p>
            //     <p>[ref[]: /uri</p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 448, "Inlines - Links");
			Helpers.ExecuteTest("[foo][ref[]\n\n[ref[]: /uri", "<p>[foo][ref[]</p>\n<p>[ref[]: /uri</p>");
        }

        [TestMethod]
        [TestCategory("Inlines - Links")]
        //[Timeout(1000)]
        public void Example449()
        {
            // Example 449
            // Section: Inlines - Links
            //
            // The following CommonMark:
            //     [foo][ref[bar]]
            //     
            //     [ref[bar]]: /uri
            //
            // Should be rendered as:
            //     <p>[foo][ref[bar]]</p>
            //     <p>[ref[bar]]: /uri</p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 449, "Inlines - Links");
			Helpers.ExecuteTest("[foo][ref[bar]]\n\n[ref[bar]]: /uri", "<p>[foo][ref[bar]]</p>\n<p>[ref[bar]]: /uri</p>");
        }

        [TestMethod]
        [TestCategory("Inlines - Links")]
        //[Timeout(1000)]
        public void Example450()
        {
            // Example 450
            // Section: Inlines - Links
            //
            // The following CommonMark:
            //     [[[foo]]]
            //     
            //     [[[foo]]]: /url
            //
            // Should be rendered as:
            //     <p>[[[foo]]]</p>
            //     <p>[[[foo]]]: /url</p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 450, "Inlines - Links");
			Helpers.ExecuteTest("[[[foo]]]\n\n[[[foo]]]: /url", "<p>[[[foo]]]</p>\n<p>[[[foo]]]: /url</p>");
        }

        [TestMethod]
        [TestCategory("Inlines - Links")]
        //[Timeout(1000)]
        public void Example451()
        {
            // Example 451
            // Section: Inlines - Links
            //
            // The following CommonMark:
            //     [foo][ref\[]
            //     
            //     [ref\[]: /uri
            //
            // Should be rendered as:
            //     <p><a href="/uri">foo</a></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 451, "Inlines - Links");
			Helpers.ExecuteTest("[foo][ref\\[]\n\n[ref\\[]: /uri", "<p><a href=\"/uri\">foo</a></p>");
        }

        // A [collapsed reference link](@collapsed-reference-link)
        // consists of a [link label] that [matches] a
        // [link reference definition] elsewhere in the
        // document, optional [whitespace], and the string `[]`.
        // The contents of the first link label are parsed as inlines,
        // which are used as the link's text.  The link's URI and title are
        // provided by the matching reference link definition.  Thus,
        // `[foo][]` is equivalent to `[foo][foo]`.
        [TestMethod]
        [TestCategory("Inlines - Links")]
        //[Timeout(1000)]
        public void Example452()
        {
            // Example 452
            // Section: Inlines - Links
            //
            // The following CommonMark:
            //     [foo][]
            //     
            //     [foo]: /url "title"
            //
            // Should be rendered as:
            //     <p><a href="/url" title="title">foo</a></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 452, "Inlines - Links");
			Helpers.ExecuteTest("[foo][]\n\n[foo]: /url \"title\"", "<p><a href=\"/url\" title=\"title\">foo</a></p>");
        }

        [TestMethod]
        [TestCategory("Inlines - Links")]
        //[Timeout(1000)]
        public void Example453()
        {
            // Example 453
            // Section: Inlines - Links
            //
            // The following CommonMark:
            //     [*foo* bar][]
            //     
            //     [*foo* bar]: /url "title"
            //
            // Should be rendered as:
            //     <p><a href="/url" title="title"><em>foo</em> bar</a></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 453, "Inlines - Links");
			Helpers.ExecuteTest("[*foo* bar][]\n\n[*foo* bar]: /url \"title\"", "<p><a href=\"/url\" title=\"title\"><em>foo</em> bar</a></p>");
        }

        // The link labels are case-insensitive:
        [TestMethod]
        [TestCategory("Inlines - Links")]
        //[Timeout(1000)]
        public void Example454()
        {
            // Example 454
            // Section: Inlines - Links
            //
            // The following CommonMark:
            //     [Foo][]
            //     
            //     [foo]: /url "title"
            //
            // Should be rendered as:
            //     <p><a href="/url" title="title">Foo</a></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 454, "Inlines - Links");
			Helpers.ExecuteTest("[Foo][]\n\n[foo]: /url \"title\"", "<p><a href=\"/url\" title=\"title\">Foo</a></p>");
        }

        // As with full reference links, [whitespace] is allowed
        // between the two sets of brackets:
        [TestMethod]
        [TestCategory("Inlines - Links")]
        //[Timeout(1000)]
        public void Example455()
        {
            // Example 455
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

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 455, "Inlines - Links");
			Helpers.ExecuteTest("[foo] \n[]\n\n[foo]: /url \"title\"", "<p><a href=\"/url\" title=\"title\">foo</a></p>");
        }

        // A [shortcut reference link](@shortcut-reference-link)
        // consists of a [link label] that [matches] a
        // [link reference definition] elsewhere in the
        // document and is not followed by `[]` or a link label.
        // The contents of the first link label are parsed as inlines,
        // which are used as the link's text.  the link's URI and title
        // are provided by the matching link reference definition.
        // Thus, `[foo]` is equivalent to `[foo][]`.
        [TestMethod]
        [TestCategory("Inlines - Links")]
        //[Timeout(1000)]
        public void Example456()
        {
            // Example 456
            // Section: Inlines - Links
            //
            // The following CommonMark:
            //     [foo]
            //     
            //     [foo]: /url "title"
            //
            // Should be rendered as:
            //     <p><a href="/url" title="title">foo</a></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 456, "Inlines - Links");
			Helpers.ExecuteTest("[foo]\n\n[foo]: /url \"title\"", "<p><a href=\"/url\" title=\"title\">foo</a></p>");
        }

        [TestMethod]
        [TestCategory("Inlines - Links")]
        //[Timeout(1000)]
        public void Example457()
        {
            // Example 457
            // Section: Inlines - Links
            //
            // The following CommonMark:
            //     [*foo* bar]
            //     
            //     [*foo* bar]: /url "title"
            //
            // Should be rendered as:
            //     <p><a href="/url" title="title"><em>foo</em> bar</a></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 457, "Inlines - Links");
			Helpers.ExecuteTest("[*foo* bar]\n\n[*foo* bar]: /url \"title\"", "<p><a href=\"/url\" title=\"title\"><em>foo</em> bar</a></p>");
        }

        [TestMethod]
        [TestCategory("Inlines - Links")]
        //[Timeout(1000)]
        public void Example458()
        {
            // Example 458
            // Section: Inlines - Links
            //
            // The following CommonMark:
            //     [[*foo* bar]]
            //     
            //     [*foo* bar]: /url "title"
            //
            // Should be rendered as:
            //     <p>[<a href="/url" title="title"><em>foo</em> bar</a>]</p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 458, "Inlines - Links");
			Helpers.ExecuteTest("[[*foo* bar]]\n\n[*foo* bar]: /url \"title\"", "<p>[<a href=\"/url\" title=\"title\"><em>foo</em> bar</a>]</p>");
        }

        [TestMethod]
        [TestCategory("Inlines - Links")]
        //[Timeout(1000)]
        public void Example459()
        {
            // Example 459
            // Section: Inlines - Links
            //
            // The following CommonMark:
            //     [[bar [foo]
            //     
            //     [foo]: /url
            //
            // Should be rendered as:
            //     <p>[[bar <a href="/url">foo</a></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 459, "Inlines - Links");
			Helpers.ExecuteTest("[[bar [foo]\n\n[foo]: /url", "<p>[[bar <a href=\"/url\">foo</a></p>");
        }

        // The link labels are case-insensitive:
        [TestMethod]
        [TestCategory("Inlines - Links")]
        //[Timeout(1000)]
        public void Example460()
        {
            // Example 460
            // Section: Inlines - Links
            //
            // The following CommonMark:
            //     [Foo]
            //     
            //     [foo]: /url "title"
            //
            // Should be rendered as:
            //     <p><a href="/url" title="title">Foo</a></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 460, "Inlines - Links");
			Helpers.ExecuteTest("[Foo]\n\n[foo]: /url \"title\"", "<p><a href=\"/url\" title=\"title\">Foo</a></p>");
        }

        // A space after the link text should be preserved:
        [TestMethod]
        [TestCategory("Inlines - Links")]
        //[Timeout(1000)]
        public void Example461()
        {
            // Example 461
            // Section: Inlines - Links
            //
            // The following CommonMark:
            //     [foo] bar
            //     
            //     [foo]: /url
            //
            // Should be rendered as:
            //     <p><a href="/url">foo</a> bar</p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 461, "Inlines - Links");
			Helpers.ExecuteTest("[foo] bar\n\n[foo]: /url", "<p><a href=\"/url\">foo</a> bar</p>");
        }

        // If you just want bracketed text, you can backslash-escape the
        // opening bracket to avoid links:
        [TestMethod]
        [TestCategory("Inlines - Links")]
        //[Timeout(1000)]
        public void Example462()
        {
            // Example 462
            // Section: Inlines - Links
            //
            // The following CommonMark:
            //     \[foo]
            //     
            //     [foo]: /url "title"
            //
            // Should be rendered as:
            //     <p>[foo]</p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 462, "Inlines - Links");
			Helpers.ExecuteTest("\\[foo]\n\n[foo]: /url \"title\"", "<p>[foo]</p>");
        }

        // Note that this is a link, because a link label ends with the first
        // following closing bracket:
        [TestMethod]
        [TestCategory("Inlines - Links")]
        //[Timeout(1000)]
        public void Example463()
        {
            // Example 463
            // Section: Inlines - Links
            //
            // The following CommonMark:
            //     [foo*]: /url
            //     
            //     *[foo*]
            //
            // Should be rendered as:
            //     <p>*<a href="/url">foo*</a></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 463, "Inlines - Links");
			Helpers.ExecuteTest("[foo*]: /url\n\n*[foo*]", "<p>*<a href=\"/url\">foo*</a></p>");
        }

        // Full references take precedence over shortcut references:
        [TestMethod]
        [TestCategory("Inlines - Links")]
        //[Timeout(1000)]
        public void Example464()
        {
            // Example 464
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

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 464, "Inlines - Links");
			Helpers.ExecuteTest("[foo][bar]\n\n[foo]: /url1\n[bar]: /url2", "<p><a href=\"/url2\">foo</a></p>");
        }

        // In the following case `[bar][baz]` is parsed as a reference,
        // `[foo]` as normal text:
        [TestMethod]
        [TestCategory("Inlines - Links")]
        //[Timeout(1000)]
        public void Example465()
        {
            // Example 465
            // Section: Inlines - Links
            //
            // The following CommonMark:
            //     [foo][bar][baz]
            //     
            //     [baz]: /url
            //
            // Should be rendered as:
            //     <p>[foo]<a href="/url">bar</a></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 465, "Inlines - Links");
			Helpers.ExecuteTest("[foo][bar][baz]\n\n[baz]: /url", "<p>[foo]<a href=\"/url\">bar</a></p>");
        }

        // Here, though, `[foo][bar]` is parsed as a reference, since
        // `[bar]` is defined:
        [TestMethod]
        [TestCategory("Inlines - Links")]
        //[Timeout(1000)]
        public void Example466()
        {
            // Example 466
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

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 466, "Inlines - Links");
			Helpers.ExecuteTest("[foo][bar][baz]\n\n[baz]: /url1\n[bar]: /url2", "<p><a href=\"/url2\">foo</a><a href=\"/url1\">baz</a></p>");
        }

        // Here `[foo]` is not parsed as a shortcut reference, because it
        // is followed by a link label (even though `[bar]` is not defined):
        [TestMethod]
        [TestCategory("Inlines - Links")]
        //[Timeout(1000)]
        public void Example467()
        {
            // Example 467
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

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 467, "Inlines - Links");
			Helpers.ExecuteTest("[foo][bar][baz]\n\n[baz]: /url1\n[foo]: /url2", "<p>[foo]<a href=\"/url1\">bar</a></p>");
        }

        // ## Images
        //
        // Syntax for images is like the syntax for links, with one
        // difference. Instead of [link text], we have an
        // [image description](@image-description).  The rules for this are the
        // same as for [link text], except that (a) an
        // image description starts with `![` rather than `[`, and
        // (b) an image description may contain links.
        // An image description has inline elements
        // as its contents.  When an image is rendered to HTML,
        // this is standardly used as the image's `alt` attribute.
        [TestMethod]
        [TestCategory("Inlines - Images")]
        //[Timeout(1000)]
        public void Example468()
        {
            // Example 468
            // Section: Inlines - Images
            //
            // The following CommonMark:
            //     ![foo](/url "title")
            //
            // Should be rendered as:
            //     <p><img src="/url" alt="foo" title="title" /></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 468, "Inlines - Images");
			Helpers.ExecuteTest("![foo](/url \"title\")", "<p><img src=\"/url\" alt=\"foo\" title=\"title\" /></p>");
        }

        [TestMethod]
        [TestCategory("Inlines - Images")]
        //[Timeout(1000)]
        public void Example469()
        {
            // Example 469
            // Section: Inlines - Images
            //
            // The following CommonMark:
            //     ![foo *bar*]
            //     
            //     [foo *bar*]: train.jpg "train & tracks"
            //
            // Should be rendered as:
            //     <p><img src="train.jpg" alt="foo bar" title="train &amp; tracks" /></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 469, "Inlines - Images");
			Helpers.ExecuteTest("![foo *bar*]\n\n[foo *bar*]: train.jpg \"train & tracks\"", "<p><img src=\"train.jpg\" alt=\"foo bar\" title=\"train &amp; tracks\" /></p>");
        }

        [TestMethod]
        [TestCategory("Inlines - Images")]
        //[Timeout(1000)]
        public void Example470()
        {
            // Example 470
            // Section: Inlines - Images
            //
            // The following CommonMark:
            //     ![foo ![bar](/url)](/url2)
            //
            // Should be rendered as:
            //     <p><img src="/url2" alt="foo bar" /></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 470, "Inlines - Images");
			Helpers.ExecuteTest("![foo ![bar](/url)](/url2)", "<p><img src=\"/url2\" alt=\"foo bar\" /></p>");
        }

        [TestMethod]
        [TestCategory("Inlines - Images")]
        //[Timeout(1000)]
        public void Example471()
        {
            // Example 471
            // Section: Inlines - Images
            //
            // The following CommonMark:
            //     ![foo [bar](/url)](/url2)
            //
            // Should be rendered as:
            //     <p><img src="/url2" alt="foo bar" /></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 471, "Inlines - Images");
			Helpers.ExecuteTest("![foo [bar](/url)](/url2)", "<p><img src=\"/url2\" alt=\"foo bar\" /></p>");
        }

        // Though this spec is concerned with parsing, not rendering, it is
        // recommended that in rendering to HTML, only the plain string content
        // of the [image description] be used.  Note that in
        // the above example, the alt attribute's value is `foo bar`, not `foo
        // [bar](/url)` or `foo <a href="/url">bar</a>`.  Only the plain string
        // content is rendered, without formatting.
        [TestMethod]
        [TestCategory("Inlines - Images")]
        //[Timeout(1000)]
        public void Example472()
        {
            // Example 472
            // Section: Inlines - Images
            //
            // The following CommonMark:
            //     ![foo *bar*][]
            //     
            //     [foo *bar*]: train.jpg "train & tracks"
            //
            // Should be rendered as:
            //     <p><img src="train.jpg" alt="foo bar" title="train &amp; tracks" /></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 472, "Inlines - Images");
			Helpers.ExecuteTest("![foo *bar*][]\n\n[foo *bar*]: train.jpg \"train & tracks\"", "<p><img src=\"train.jpg\" alt=\"foo bar\" title=\"train &amp; tracks\" /></p>");
        }

        [TestMethod]
        [TestCategory("Inlines - Images")]
        //[Timeout(1000)]
        public void Example473()
        {
            // Example 473
            // Section: Inlines - Images
            //
            // The following CommonMark:
            //     ![foo *bar*][foobar]
            //     
            //     [FOOBAR]: train.jpg "train & tracks"
            //
            // Should be rendered as:
            //     <p><img src="train.jpg" alt="foo bar" title="train &amp; tracks" /></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 473, "Inlines - Images");
			Helpers.ExecuteTest("![foo *bar*][foobar]\n\n[FOOBAR]: train.jpg \"train & tracks\"", "<p><img src=\"train.jpg\" alt=\"foo bar\" title=\"train &amp; tracks\" /></p>");
        }

        [TestMethod]
        [TestCategory("Inlines - Images")]
        //[Timeout(1000)]
        public void Example474()
        {
            // Example 474
            // Section: Inlines - Images
            //
            // The following CommonMark:
            //     ![foo](train.jpg)
            //
            // Should be rendered as:
            //     <p><img src="train.jpg" alt="foo" /></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 474, "Inlines - Images");
			Helpers.ExecuteTest("![foo](train.jpg)", "<p><img src=\"train.jpg\" alt=\"foo\" /></p>");
        }

        [TestMethod]
        [TestCategory("Inlines - Images")]
        //[Timeout(1000)]
        public void Example475()
        {
            // Example 475
            // Section: Inlines - Images
            //
            // The following CommonMark:
            //     My ![foo bar](/path/to/train.jpg  "title"   )
            //
            // Should be rendered as:
            //     <p>My <img src="/path/to/train.jpg" alt="foo bar" title="title" /></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 475, "Inlines - Images");
			Helpers.ExecuteTest("My ![foo bar](/path/to/train.jpg  \"title\"   )", "<p>My <img src=\"/path/to/train.jpg\" alt=\"foo bar\" title=\"title\" /></p>");
        }

        [TestMethod]
        [TestCategory("Inlines - Images")]
        //[Timeout(1000)]
        public void Example476()
        {
            // Example 476
            // Section: Inlines - Images
            //
            // The following CommonMark:
            //     ![foo](<url>)
            //
            // Should be rendered as:
            //     <p><img src="url" alt="foo" /></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 476, "Inlines - Images");
			Helpers.ExecuteTest("![foo](<url>)", "<p><img src=\"url\" alt=\"foo\" /></p>");
        }

        [TestMethod]
        [TestCategory("Inlines - Images")]
        //[Timeout(1000)]
        public void Example477()
        {
            // Example 477
            // Section: Inlines - Images
            //
            // The following CommonMark:
            //     ![](/url)
            //
            // Should be rendered as:
            //     <p><img src="/url" alt="" /></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 477, "Inlines - Images");
			Helpers.ExecuteTest("![](/url)", "<p><img src=\"/url\" alt=\"\" /></p>");
        }

        // Reference-style:
        [TestMethod]
        [TestCategory("Inlines - Images")]
        //[Timeout(1000)]
        public void Example478()
        {
            // Example 478
            // Section: Inlines - Images
            //
            // The following CommonMark:
            //     ![foo] [bar]
            //     
            //     [bar]: /url
            //
            // Should be rendered as:
            //     <p><img src="/url" alt="foo" /></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 478, "Inlines - Images");
			Helpers.ExecuteTest("![foo] [bar]\n\n[bar]: /url", "<p><img src=\"/url\" alt=\"foo\" /></p>");
        }

        [TestMethod]
        [TestCategory("Inlines - Images")]
        //[Timeout(1000)]
        public void Example479()
        {
            // Example 479
            // Section: Inlines - Images
            //
            // The following CommonMark:
            //     ![foo] [bar]
            //     
            //     [BAR]: /url
            //
            // Should be rendered as:
            //     <p><img src="/url" alt="foo" /></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 479, "Inlines - Images");
			Helpers.ExecuteTest("![foo] [bar]\n\n[BAR]: /url", "<p><img src=\"/url\" alt=\"foo\" /></p>");
        }

        // Collapsed:
        [TestMethod]
        [TestCategory("Inlines - Images")]
        //[Timeout(1000)]
        public void Example480()
        {
            // Example 480
            // Section: Inlines - Images
            //
            // The following CommonMark:
            //     ![foo][]
            //     
            //     [foo]: /url "title"
            //
            // Should be rendered as:
            //     <p><img src="/url" alt="foo" title="title" /></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 480, "Inlines - Images");
			Helpers.ExecuteTest("![foo][]\n\n[foo]: /url \"title\"", "<p><img src=\"/url\" alt=\"foo\" title=\"title\" /></p>");
        }

        [TestMethod]
        [TestCategory("Inlines - Images")]
        //[Timeout(1000)]
        public void Example481()
        {
            // Example 481
            // Section: Inlines - Images
            //
            // The following CommonMark:
            //     ![*foo* bar][]
            //     
            //     [*foo* bar]: /url "title"
            //
            // Should be rendered as:
            //     <p><img src="/url" alt="foo bar" title="title" /></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 481, "Inlines - Images");
			Helpers.ExecuteTest("![*foo* bar][]\n\n[*foo* bar]: /url \"title\"", "<p><img src=\"/url\" alt=\"foo bar\" title=\"title\" /></p>");
        }

        // The labels are case-insensitive:
        [TestMethod]
        [TestCategory("Inlines - Images")]
        //[Timeout(1000)]
        public void Example482()
        {
            // Example 482
            // Section: Inlines - Images
            //
            // The following CommonMark:
            //     ![Foo][]
            //     
            //     [foo]: /url "title"
            //
            // Should be rendered as:
            //     <p><img src="/url" alt="Foo" title="title" /></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 482, "Inlines - Images");
			Helpers.ExecuteTest("![Foo][]\n\n[foo]: /url \"title\"", "<p><img src=\"/url\" alt=\"Foo\" title=\"title\" /></p>");
        }

        // As with full reference links, [whitespace] is allowed
        // between the two sets of brackets:
        [TestMethod]
        [TestCategory("Inlines - Images")]
        //[Timeout(1000)]
        public void Example483()
        {
            // Example 483
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

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 483, "Inlines - Images");
			Helpers.ExecuteTest("![foo] \n[]\n\n[foo]: /url \"title\"", "<p><img src=\"/url\" alt=\"foo\" title=\"title\" /></p>");
        }

        // Shortcut:
        [TestMethod]
        [TestCategory("Inlines - Images")]
        //[Timeout(1000)]
        public void Example484()
        {
            // Example 484
            // Section: Inlines - Images
            //
            // The following CommonMark:
            //     ![foo]
            //     
            //     [foo]: /url "title"
            //
            // Should be rendered as:
            //     <p><img src="/url" alt="foo" title="title" /></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 484, "Inlines - Images");
			Helpers.ExecuteTest("![foo]\n\n[foo]: /url \"title\"", "<p><img src=\"/url\" alt=\"foo\" title=\"title\" /></p>");
        }

        [TestMethod]
        [TestCategory("Inlines - Images")]
        //[Timeout(1000)]
        public void Example485()
        {
            // Example 485
            // Section: Inlines - Images
            //
            // The following CommonMark:
            //     ![*foo* bar]
            //     
            //     [*foo* bar]: /url "title"
            //
            // Should be rendered as:
            //     <p><img src="/url" alt="foo bar" title="title" /></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 485, "Inlines - Images");
			Helpers.ExecuteTest("![*foo* bar]\n\n[*foo* bar]: /url \"title\"", "<p><img src=\"/url\" alt=\"foo bar\" title=\"title\" /></p>");
        }

        // Note that link labels cannot contain unescaped brackets:
        [TestMethod]
        [TestCategory("Inlines - Images")]
        //[Timeout(1000)]
        public void Example486()
        {
            // Example 486
            // Section: Inlines - Images
            //
            // The following CommonMark:
            //     ![[foo]]
            //     
            //     [[foo]]: /url "title"
            //
            // Should be rendered as:
            //     <p>![[foo]]</p>
            //     <p>[[foo]]: /url &quot;title&quot;</p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 486, "Inlines - Images");
			Helpers.ExecuteTest("![[foo]]\n\n[[foo]]: /url \"title\"", "<p>![[foo]]</p>\n<p>[[foo]]: /url &quot;title&quot;</p>");
        }

        // The link labels are case-insensitive:
        [TestMethod]
        [TestCategory("Inlines - Images")]
        //[Timeout(1000)]
        public void Example487()
        {
            // Example 487
            // Section: Inlines - Images
            //
            // The following CommonMark:
            //     ![Foo]
            //     
            //     [foo]: /url "title"
            //
            // Should be rendered as:
            //     <p><img src="/url" alt="Foo" title="title" /></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 487, "Inlines - Images");
			Helpers.ExecuteTest("![Foo]\n\n[foo]: /url \"title\"", "<p><img src=\"/url\" alt=\"Foo\" title=\"title\" /></p>");
        }

        // If you just want bracketed text, you can backslash-escape the
        // opening `!` and `[`:
        [TestMethod]
        [TestCategory("Inlines - Images")]
        //[Timeout(1000)]
        public void Example488()
        {
            // Example 488
            // Section: Inlines - Images
            //
            // The following CommonMark:
            //     \!\[foo]
            //     
            //     [foo]: /url "title"
            //
            // Should be rendered as:
            //     <p>![foo]</p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 488, "Inlines - Images");
			Helpers.ExecuteTest("\\!\\[foo]\n\n[foo]: /url \"title\"", "<p>![foo]</p>");
        }

        // If you want a link after a literal `!`, backslash-escape the
        // `!`:
        [TestMethod]
        [TestCategory("Inlines - Images")]
        //[Timeout(1000)]
        public void Example489()
        {
            // Example 489
            // Section: Inlines - Images
            //
            // The following CommonMark:
            //     \![foo]
            //     
            //     [foo]: /url "title"
            //
            // Should be rendered as:
            //     <p>!<a href="/url" title="title">foo</a></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 489, "Inlines - Images");
			Helpers.ExecuteTest("\\![foo]\n\n[foo]: /url \"title\"", "<p>!<a href=\"/url\" title=\"title\">foo</a></p>");
        }

        // ## Autolinks
        //
        // [Autolink](@autolink)s are absolute URIs and email addresses inside
        // `<` and `>`. They are parsed as links, with the URL or email address
        // as the link label.
        //
        // A [URI autolink](@uri-autolink) consists of `<`, followed by an
        // [absolute URI] not containing `<`, followed by `>`.  It is parsed as
        // a link to the URI, with the URI as the link's label.
        //
        // An [absolute URI](@absolute-uri),
        // for these purposes, consists of a [scheme] followed by a colon (`:`)
        // followed by zero or more characters other than ASCII
        // [whitespace] and control characters, `<`, and `>`.  If
        // the URI includes these characters, you must use percent-encoding
        // (e.g. `%20` for a space).
        //
        // The following [schemes](@scheme)
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
        public void Example490()
        {
            // Example 490
            // Section: Inlines - Autolinks
            //
            // The following CommonMark:
            //     <http://foo.bar.baz>
            //
            // Should be rendered as:
            //     <p><a href="http://foo.bar.baz">http://foo.bar.baz</a></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 490, "Inlines - Autolinks");
			Helpers.ExecuteTest("<http://foo.bar.baz>", "<p><a href=\"http://foo.bar.baz\">http://foo.bar.baz</a></p>");
        }

        [TestMethod]
        [TestCategory("Inlines - Autolinks")]
        //[Timeout(1000)]
        public void Example491()
        {
            // Example 491
            // Section: Inlines - Autolinks
            //
            // The following CommonMark:
            //     <http://foo.bar.baz/test?q=hello&id=22&boolean>
            //
            // Should be rendered as:
            //     <p><a href="http://foo.bar.baz/test?q=hello&amp;id=22&amp;boolean">http://foo.bar.baz/test?q=hello&amp;id=22&amp;boolean</a></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 491, "Inlines - Autolinks");
			Helpers.ExecuteTest("<http://foo.bar.baz/test?q=hello&id=22&boolean>", "<p><a href=\"http://foo.bar.baz/test?q=hello&amp;id=22&amp;boolean\">http://foo.bar.baz/test?q=hello&amp;id=22&amp;boolean</a></p>");
        }

        [TestMethod]
        [TestCategory("Inlines - Autolinks")]
        //[Timeout(1000)]
        public void Example492()
        {
            // Example 492
            // Section: Inlines - Autolinks
            //
            // The following CommonMark:
            //     <irc://foo.bar:2233/baz>
            //
            // Should be rendered as:
            //     <p><a href="irc://foo.bar:2233/baz">irc://foo.bar:2233/baz</a></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 492, "Inlines - Autolinks");
			Helpers.ExecuteTest("<irc://foo.bar:2233/baz>", "<p><a href=\"irc://foo.bar:2233/baz\">irc://foo.bar:2233/baz</a></p>");
        }

        // Uppercase is also fine:
        [TestMethod]
        [TestCategory("Inlines - Autolinks")]
        //[Timeout(1000)]
        public void Example493()
        {
            // Example 493
            // Section: Inlines - Autolinks
            //
            // The following CommonMark:
            //     <MAILTO:FOO@BAR.BAZ>
            //
            // Should be rendered as:
            //     <p><a href="MAILTO:FOO@BAR.BAZ">MAILTO:FOO@BAR.BAZ</a></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 493, "Inlines - Autolinks");
			Helpers.ExecuteTest("<MAILTO:FOO@BAR.BAZ>", "<p><a href=\"MAILTO:FOO@BAR.BAZ\">MAILTO:FOO@BAR.BAZ</a></p>");
        }

        // Spaces are not allowed in autolinks:
        [TestMethod]
        [TestCategory("Inlines - Autolinks")]
        //[Timeout(1000)]
        public void Example494()
        {
            // Example 494
            // Section: Inlines - Autolinks
            //
            // The following CommonMark:
            //     <http://foo.bar/baz bim>
            //
            // Should be rendered as:
            //     <p>&lt;http://foo.bar/baz bim&gt;</p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 494, "Inlines - Autolinks");
			Helpers.ExecuteTest("<http://foo.bar/baz bim>", "<p>&lt;http://foo.bar/baz bim&gt;</p>");
        }

        // Backslash-escapes do not work inside autolinks:
        [TestMethod]
        [TestCategory("Inlines - Autolinks")]
        //[Timeout(1000)]
        public void Example495()
        {
            // Example 495
            // Section: Inlines - Autolinks
            //
            // The following CommonMark:
            //     <http://example.com/\[\>
            //
            // Should be rendered as:
            //     <p><a href="http://example.com/%5C%5B%5C">http://example.com/\[\</a></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 495, "Inlines - Autolinks");
			Helpers.ExecuteTest("<http://example.com/\\[\\>", "<p><a href=\"http://example.com/%5C%5B%5C\">http://example.com/\\[\\</a></p>");
        }

        // An [email autolink](@email-autolink)
        // consists of `<`, followed by an [email address],
        // followed by `>`.  The link's label is the email address,
        // and the URL is `mailto:` followed by the email address.
        //
        // An [email address](@email-address),
        // for these purposes, is anything that matches
        // the [non-normative regex from the HTML5
        // spec](https://html.spec.whatwg.org/multipage/forms.html#e-mail-state-(type=email)):
        //
        // /^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?
        // (?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*$/
        //
        // Examples of email autolinks:
        [TestMethod]
        [TestCategory("Inlines - Autolinks")]
        //[Timeout(1000)]
        public void Example496()
        {
            // Example 496
            // Section: Inlines - Autolinks
            //
            // The following CommonMark:
            //     <foo@bar.example.com>
            //
            // Should be rendered as:
            //     <p><a href="mailto:foo@bar.example.com">foo@bar.example.com</a></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 496, "Inlines - Autolinks");
			Helpers.ExecuteTest("<foo@bar.example.com>", "<p><a href=\"mailto:foo@bar.example.com\">foo@bar.example.com</a></p>");
        }

        [TestMethod]
        [TestCategory("Inlines - Autolinks")]
        //[Timeout(1000)]
        public void Example497()
        {
            // Example 497
            // Section: Inlines - Autolinks
            //
            // The following CommonMark:
            //     <foo+special@Bar.baz-bar0.com>
            //
            // Should be rendered as:
            //     <p><a href="mailto:foo+special@Bar.baz-bar0.com">foo+special@Bar.baz-bar0.com</a></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 497, "Inlines - Autolinks");
			Helpers.ExecuteTest("<foo+special@Bar.baz-bar0.com>", "<p><a href=\"mailto:foo+special@Bar.baz-bar0.com\">foo+special@Bar.baz-bar0.com</a></p>");
        }

        // Backslash-escapes do not work inside email autolinks:
        [TestMethod]
        [TestCategory("Inlines - Autolinks")]
        //[Timeout(1000)]
        public void Example498()
        {
            // Example 498
            // Section: Inlines - Autolinks
            //
            // The following CommonMark:
            //     <foo\+@bar.example.com>
            //
            // Should be rendered as:
            //     <p>&lt;foo+@bar.example.com&gt;</p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 498, "Inlines - Autolinks");
			Helpers.ExecuteTest("<foo\\+@bar.example.com>", "<p>&lt;foo+@bar.example.com&gt;</p>");
        }

        // These are not autolinks:
        [TestMethod]
        [TestCategory("Inlines - Autolinks")]
        //[Timeout(1000)]
        public void Example499()
        {
            // Example 499
            // Section: Inlines - Autolinks
            //
            // The following CommonMark:
            //     <>
            //
            // Should be rendered as:
            //     <p>&lt;&gt;</p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 499, "Inlines - Autolinks");
			Helpers.ExecuteTest("<>", "<p>&lt;&gt;</p>");
        }

        [TestMethod]
        [TestCategory("Inlines - Autolinks")]
        //[Timeout(1000)]
        public void Example500()
        {
            // Example 500
            // Section: Inlines - Autolinks
            //
            // The following CommonMark:
            //     <heck://bing.bong>
            //
            // Should be rendered as:
            //     <p>&lt;heck://bing.bong&gt;</p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 500, "Inlines - Autolinks");
			Helpers.ExecuteTest("<heck://bing.bong>", "<p>&lt;heck://bing.bong&gt;</p>");
        }

        [TestMethod]
        [TestCategory("Inlines - Autolinks")]
        //[Timeout(1000)]
        public void Example501()
        {
            // Example 501
            // Section: Inlines - Autolinks
            //
            // The following CommonMark:
            //     < http://foo.bar >
            //
            // Should be rendered as:
            //     <p>&lt; http://foo.bar &gt;</p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 501, "Inlines - Autolinks");
			Helpers.ExecuteTest("< http://foo.bar >", "<p>&lt; http://foo.bar &gt;</p>");
        }

        [TestMethod]
        [TestCategory("Inlines - Autolinks")]
        //[Timeout(1000)]
        public void Example502()
        {
            // Example 502
            // Section: Inlines - Autolinks
            //
            // The following CommonMark:
            //     <foo.bar.baz>
            //
            // Should be rendered as:
            //     <p>&lt;foo.bar.baz&gt;</p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 502, "Inlines - Autolinks");
			Helpers.ExecuteTest("<foo.bar.baz>", "<p>&lt;foo.bar.baz&gt;</p>");
        }

        [TestMethod]
        [TestCategory("Inlines - Autolinks")]
        //[Timeout(1000)]
        public void Example503()
        {
            // Example 503
            // Section: Inlines - Autolinks
            //
            // The following CommonMark:
            //     <localhost:5001/foo>
            //
            // Should be rendered as:
            //     <p>&lt;localhost:5001/foo&gt;</p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 503, "Inlines - Autolinks");
			Helpers.ExecuteTest("<localhost:5001/foo>", "<p>&lt;localhost:5001/foo&gt;</p>");
        }

        [TestMethod]
        [TestCategory("Inlines - Autolinks")]
        //[Timeout(1000)]
        public void Example504()
        {
            // Example 504
            // Section: Inlines - Autolinks
            //
            // The following CommonMark:
            //     http://example.com
            //
            // Should be rendered as:
            //     <p>http://example.com</p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 504, "Inlines - Autolinks");
			Helpers.ExecuteTest("http://example.com", "<p>http://example.com</p>");
        }

        [TestMethod]
        [TestCategory("Inlines - Autolinks")]
        //[Timeout(1000)]
        public void Example505()
        {
            // Example 505
            // Section: Inlines - Autolinks
            //
            // The following CommonMark:
            //     foo@bar.example.com
            //
            // Should be rendered as:
            //     <p>foo@bar.example.com</p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 505, "Inlines - Autolinks");
			Helpers.ExecuteTest("foo@bar.example.com", "<p>foo@bar.example.com</p>");
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
        // A [tag name](@tag-name) consists of an ASCII letter
        // followed by zero or more ASCII letters or digits.
        //
        // An [attribute](@attribute) consists of [whitespace],
        // an [attribute name], and an optional
        // [attribute value specification].
        //
        // An [attribute name](@attribute-name)
        // consists of an ASCII letter, `_`, or `:`, followed by zero or more ASCII
        // letters, digits, `_`, `.`, `:`, or `-`.  (Note:  This is the XML
        // specification restricted to ASCII.  HTML5 is laxer.)
        //
        // An [attribute value specification](@attribute-value-specification)
        // consists of optional [whitespace],
        // a `=` character, optional [whitespace], and an [attribute
        // value].
        //
        // An [attribute value](@attribute-value)
        // consists of an [unquoted attribute value],
        // a [single-quoted attribute value], or a [double-quoted attribute value].
        //
        // An [unquoted attribute value](@unquoted-attribute-value)
        // is a nonempty string of characters not
        // including spaces, `"`, `'`, `=`, `<`, `>`, or `` ` ``.
        //
        // A [single-quoted attribute value](@single-quoted-attribute-value)
        // consists of `'`, zero or more
        // characters not including `'`, and a final `'`.
        //
        // A [double-quoted attribute value](@double-quoted-attribute-value)
        // consists of `"`, zero or more
        // characters not including `"`, and a final `"`.
        //
        // An [open tag](@open-tag) consists of a `<` character, a [tag name],
        // zero or more [attributes], optional [whitespace], an optional `/`
        // character, and a `>` character.
        //
        // A [closing tag](@closing-tag) consists of the string `</`, a
        // [tag name], optional [whitespace], and the character `>`.
        //
        // An [HTML comment](@html-comment) consists of `<!--` + *text* + `-->`,
        // where *text* does not start with `>` or `->`, does not end with `-`,
        // and does not contain `--`.  (See the
        // [HTML5 spec](http://www.w3.org/TR/html5/syntax.html#comments).)
        //
        // A [processing instruction](@processing-instruction)
        // consists of the string `<?`, a string
        // of characters not including the string `?>`, and the string
        // `?>`.
        //
        // A [declaration](@declaration) consists of the
        // string `<!`, a name consisting of one or more uppercase ASCII letters,
        // [whitespace], a string of characters not including the
        // character `>`, and the character `>`.
        //
        // A [CDATA section](@cdata-section) consists of
        // the string `<![CDATA[`, a string of characters not including the string
        // `]]>`, and the string `]]>`.
        //
        // An [HTML tag](@html-tag) consists of an [open tag], a [closing tag],
        // an [HTML comment], a [processing instruction], a [declaration],
        // or a [CDATA section].
        //
        // Here are some simple open tags:
        [TestMethod]
        [TestCategory("Inlines - Raw HTML")]
        //[Timeout(1000)]
        public void Example506()
        {
            // Example 506
            // Section: Inlines - Raw HTML
            //
            // The following CommonMark:
            //     <a><bab><c2c>
            //
            // Should be rendered as:
            //     <p><a><bab><c2c></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 506, "Inlines - Raw HTML");
			Helpers.ExecuteTest("<a><bab><c2c>", "<p><a><bab><c2c></p>");
        }

        // Empty elements:
        [TestMethod]
        [TestCategory("Inlines - Raw HTML")]
        //[Timeout(1000)]
        public void Example507()
        {
            // Example 507
            // Section: Inlines - Raw HTML
            //
            // The following CommonMark:
            //     <a/><b2/>
            //
            // Should be rendered as:
            //     <p><a/><b2/></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 507, "Inlines - Raw HTML");
			Helpers.ExecuteTest("<a/><b2/>", "<p><a/><b2/></p>");
        }

        // [Whitespace] is allowed:
        [TestMethod]
        [TestCategory("Inlines - Raw HTML")]
        //[Timeout(1000)]
        public void Example508()
        {
            // Example 508
            // Section: Inlines - Raw HTML
            //
            // The following CommonMark:
            //     <a  /><b2
            //     data="foo" >
            //
            // Should be rendered as:
            //     <p><a  /><b2
            //     data="foo" ></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 508, "Inlines - Raw HTML");
			Helpers.ExecuteTest("<a  /><b2\ndata=\"foo\" >", "<p><a  /><b2\ndata=\"foo\" ></p>");
        }

        // With attributes:
        [TestMethod]
        [TestCategory("Inlines - Raw HTML")]
        //[Timeout(1000)]
        public void Example509()
        {
            // Example 509
            // Section: Inlines - Raw HTML
            //
            // The following CommonMark:
            //     <a foo="bar" bam = 'baz <em>"</em>'
            //     _boolean zoop:33=zoop:33 />
            //
            // Should be rendered as:
            //     <p><a foo="bar" bam = 'baz <em>"</em>'
            //     _boolean zoop:33=zoop:33 /></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 509, "Inlines - Raw HTML");
			Helpers.ExecuteTest("<a foo=\"bar\" bam = 'baz <em>\"</em>'\n_boolean zoop:33=zoop:33 />", "<p><a foo=\"bar\" bam = 'baz <em>\"</em>'\n_boolean zoop:33=zoop:33 /></p>");
        }

        // Illegal tag names, not parsed as HTML:
        [TestMethod]
        [TestCategory("Inlines - Raw HTML")]
        //[Timeout(1000)]
        public void Example510()
        {
            // Example 510
            // Section: Inlines - Raw HTML
            //
            // The following CommonMark:
            //     <33> <__>
            //
            // Should be rendered as:
            //     <p>&lt;33&gt; &lt;__&gt;</p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 510, "Inlines - Raw HTML");
			Helpers.ExecuteTest("<33> <__>", "<p>&lt;33&gt; &lt;__&gt;</p>");
        }

        // Illegal attribute names:
        [TestMethod]
        [TestCategory("Inlines - Raw HTML")]
        //[Timeout(1000)]
        public void Example511()
        {
            // Example 511
            // Section: Inlines - Raw HTML
            //
            // The following CommonMark:
            //     <a h*#ref="hi">
            //
            // Should be rendered as:
            //     <p>&lt;a h*#ref=&quot;hi&quot;&gt;</p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 511, "Inlines - Raw HTML");
			Helpers.ExecuteTest("<a h*#ref=\"hi\">", "<p>&lt;a h*#ref=&quot;hi&quot;&gt;</p>");
        }

        // Illegal attribute values:
        [TestMethod]
        [TestCategory("Inlines - Raw HTML")]
        //[Timeout(1000)]
        public void Example512()
        {
            // Example 512
            // Section: Inlines - Raw HTML
            //
            // The following CommonMark:
            //     <a href="hi'> <a href=hi'>
            //
            // Should be rendered as:
            //     <p>&lt;a href=&quot;hi'&gt; &lt;a href=hi'&gt;</p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 512, "Inlines - Raw HTML");
			Helpers.ExecuteTest("<a href=\"hi'> <a href=hi'>", "<p>&lt;a href=&quot;hi'&gt; &lt;a href=hi'&gt;</p>");
        }

        // Illegal [whitespace]:
        [TestMethod]
        [TestCategory("Inlines - Raw HTML")]
        //[Timeout(1000)]
        public void Example513()
        {
            // Example 513
            // Section: Inlines - Raw HTML
            //
            // The following CommonMark:
            //     < a><
            //     foo><bar/ >
            //
            // Should be rendered as:
            //     <p>&lt; a&gt;&lt;
            //     foo&gt;&lt;bar/ &gt;</p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 513, "Inlines - Raw HTML");
			Helpers.ExecuteTest("< a><\nfoo><bar/ >", "<p>&lt; a&gt;&lt;\nfoo&gt;&lt;bar/ &gt;</p>");
        }

        // Missing [whitespace]:
        [TestMethod]
        [TestCategory("Inlines - Raw HTML")]
        //[Timeout(1000)]
        public void Example514()
        {
            // Example 514
            // Section: Inlines - Raw HTML
            //
            // The following CommonMark:
            //     <a href='bar'title=title>
            //
            // Should be rendered as:
            //     <p>&lt;a href='bar'title=title&gt;</p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 514, "Inlines - Raw HTML");
			Helpers.ExecuteTest("<a href='bar'title=title>", "<p>&lt;a href='bar'title=title&gt;</p>");
        }

        // Closing tags:
        [TestMethod]
        [TestCategory("Inlines - Raw HTML")]
        //[Timeout(1000)]
        public void Example515()
        {
            // Example 515
            // Section: Inlines - Raw HTML
            //
            // The following CommonMark:
            //     </a>
            //     </foo >
            //
            // Should be rendered as:
            //     <p></a>
            //     </foo ></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 515, "Inlines - Raw HTML");
			Helpers.ExecuteTest("</a>\n</foo >", "<p></a>\n</foo ></p>");
        }

        // Illegal attributes in closing tag:
        [TestMethod]
        [TestCategory("Inlines - Raw HTML")]
        //[Timeout(1000)]
        public void Example516()
        {
            // Example 516
            // Section: Inlines - Raw HTML
            //
            // The following CommonMark:
            //     </a href="foo">
            //
            // Should be rendered as:
            //     <p>&lt;/a href=&quot;foo&quot;&gt;</p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 516, "Inlines - Raw HTML");
			Helpers.ExecuteTest("</a href=\"foo\">", "<p>&lt;/a href=&quot;foo&quot;&gt;</p>");
        }

        // Comments:
        [TestMethod]
        [TestCategory("Inlines - Raw HTML")]
        //[Timeout(1000)]
        public void Example517()
        {
            // Example 517
            // Section: Inlines - Raw HTML
            //
            // The following CommonMark:
            //     foo <!-- this is a
            //     comment - with hyphen -->
            //
            // Should be rendered as:
            //     <p>foo <!-- this is a
            //     comment - with hyphen --></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 517, "Inlines - Raw HTML");
			Helpers.ExecuteTest("foo <!-- this is a\ncomment - with hyphen -->", "<p>foo <!-- this is a\ncomment - with hyphen --></p>");
        }

        [TestMethod]
        [TestCategory("Inlines - Raw HTML")]
        //[Timeout(1000)]
        public void Example518()
        {
            // Example 518
            // Section: Inlines - Raw HTML
            //
            // The following CommonMark:
            //     foo <!-- not a comment -- two hyphens -->
            //
            // Should be rendered as:
            //     <p>foo &lt;!-- not a comment -- two hyphens --&gt;</p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 518, "Inlines - Raw HTML");
			Helpers.ExecuteTest("foo <!-- not a comment -- two hyphens -->", "<p>foo &lt;!-- not a comment -- two hyphens --&gt;</p>");
        }

        // Not comments:
        [TestMethod]
        [TestCategory("Inlines - Raw HTML")]
        //[Timeout(1000)]
        public void Example519()
        {
            // Example 519
            // Section: Inlines - Raw HTML
            //
            // The following CommonMark:
            //     foo <!--> foo -->
            //     
            //     foo <!-- foo--->
            //
            // Should be rendered as:
            //     <p>foo &lt;!--&gt; foo --&gt;</p>
            //     <p>foo &lt;!-- foo---&gt;</p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 519, "Inlines - Raw HTML");
			Helpers.ExecuteTest("foo <!--> foo -->\n\nfoo <!-- foo--->", "<p>foo &lt;!--&gt; foo --&gt;</p>\n<p>foo &lt;!-- foo---&gt;</p>");
        }

        // Processing instructions:
        [TestMethod]
        [TestCategory("Inlines - Raw HTML")]
        //[Timeout(1000)]
        public void Example520()
        {
            // Example 520
            // Section: Inlines - Raw HTML
            //
            // The following CommonMark:
            //     foo <?php echo $a; ?>
            //
            // Should be rendered as:
            //     <p>foo <?php echo $a; ?></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 520, "Inlines - Raw HTML");
			Helpers.ExecuteTest("foo <?php echo $a; ?>", "<p>foo <?php echo $a; ?></p>");
        }

        // Declarations:
        [TestMethod]
        [TestCategory("Inlines - Raw HTML")]
        //[Timeout(1000)]
        public void Example521()
        {
            // Example 521
            // Section: Inlines - Raw HTML
            //
            // The following CommonMark:
            //     foo <!ELEMENT br EMPTY>
            //
            // Should be rendered as:
            //     <p>foo <!ELEMENT br EMPTY></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 521, "Inlines - Raw HTML");
			Helpers.ExecuteTest("foo <!ELEMENT br EMPTY>", "<p>foo <!ELEMENT br EMPTY></p>");
        }

        // CDATA sections:
        [TestMethod]
        [TestCategory("Inlines - Raw HTML")]
        //[Timeout(1000)]
        public void Example522()
        {
            // Example 522
            // Section: Inlines - Raw HTML
            //
            // The following CommonMark:
            //     foo <![CDATA[>&<]]>
            //
            // Should be rendered as:
            //     <p>foo <![CDATA[>&<]]></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 522, "Inlines - Raw HTML");
			Helpers.ExecuteTest("foo <![CDATA[>&<]]>", "<p>foo <![CDATA[>&<]]></p>");
        }

        // Entities are preserved in HTML attributes:
        [TestMethod]
        [TestCategory("Inlines - Raw HTML")]
        //[Timeout(1000)]
        public void Example523()
        {
            // Example 523
            // Section: Inlines - Raw HTML
            //
            // The following CommonMark:
            //     <a href="&ouml;">
            //
            // Should be rendered as:
            //     <p><a href="&ouml;"></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 523, "Inlines - Raw HTML");
			Helpers.ExecuteTest("<a href=\"&ouml;\">", "<p><a href=\"&ouml;\"></p>");
        }

        // Backslash escapes do not work in HTML attributes:
        [TestMethod]
        [TestCategory("Inlines - Raw HTML")]
        //[Timeout(1000)]
        public void Example524()
        {
            // Example 524
            // Section: Inlines - Raw HTML
            //
            // The following CommonMark:
            //     <a href="\*">
            //
            // Should be rendered as:
            //     <p><a href="\*"></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 524, "Inlines - Raw HTML");
			Helpers.ExecuteTest("<a href=\"\\*\">", "<p><a href=\"\\*\"></p>");
        }

        [TestMethod]
        [TestCategory("Inlines - Raw HTML")]
        //[Timeout(1000)]
        public void Example525()
        {
            // Example 525
            // Section: Inlines - Raw HTML
            //
            // The following CommonMark:
            //     <a href="\"">
            //
            // Should be rendered as:
            //     <p>&lt;a href=&quot;&quot;&quot;&gt;</p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 525, "Inlines - Raw HTML");
			Helpers.ExecuteTest("<a href=\"\\\"\">", "<p>&lt;a href=&quot;&quot;&quot;&gt;</p>");
        }

        // ## Hard line breaks
        //
        // A line break (not in a code span or HTML tag) that is preceded
        // by two or more spaces and does not occur at the end of a block
        // is parsed as a [hard line break](@hard-line-break) (rendered
        // in HTML as a `<br />` tag):
        [TestMethod]
        [TestCategory("Inlines - Hard line breaks")]
        //[Timeout(1000)]
        public void Example526()
        {
            // Example 526
            // Section: Inlines - Hard line breaks
            //
            // The following CommonMark:
            //     foo  
            //     baz
            //
            // Should be rendered as:
            //     <p>foo<br />
            //     baz</p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 526, "Inlines - Hard line breaks");
			Helpers.ExecuteTest("foo  \nbaz", "<p>foo<br />\nbaz</p>");
        }

        // For a more visible alternative, a backslash before the
        // [line ending] may be used instead of two spaces:
        [TestMethod]
        [TestCategory("Inlines - Hard line breaks")]
        //[Timeout(1000)]
        public void Example527()
        {
            // Example 527
            // Section: Inlines - Hard line breaks
            //
            // The following CommonMark:
            //     foo\
            //     baz
            //
            // Should be rendered as:
            //     <p>foo<br />
            //     baz</p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 527, "Inlines - Hard line breaks");
			Helpers.ExecuteTest("foo\\\nbaz", "<p>foo<br />\nbaz</p>");
        }

        // More than two spaces can be used:
        [TestMethod]
        [TestCategory("Inlines - Hard line breaks")]
        //[Timeout(1000)]
        public void Example528()
        {
            // Example 528
            // Section: Inlines - Hard line breaks
            //
            // The following CommonMark:
            //     foo       
            //     baz
            //
            // Should be rendered as:
            //     <p>foo<br />
            //     baz</p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 528, "Inlines - Hard line breaks");
			Helpers.ExecuteTest("foo       \nbaz", "<p>foo<br />\nbaz</p>");
        }

        // Leading spaces at the beginning of the next line are ignored:
        [TestMethod]
        [TestCategory("Inlines - Hard line breaks")]
        //[Timeout(1000)]
        public void Example529()
        {
            // Example 529
            // Section: Inlines - Hard line breaks
            //
            // The following CommonMark:
            //     foo  
            //          bar
            //
            // Should be rendered as:
            //     <p>foo<br />
            //     bar</p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 529, "Inlines - Hard line breaks");
			Helpers.ExecuteTest("foo  \n     bar", "<p>foo<br />\nbar</p>");
        }

        [TestMethod]
        [TestCategory("Inlines - Hard line breaks")]
        //[Timeout(1000)]
        public void Example530()
        {
            // Example 530
            // Section: Inlines - Hard line breaks
            //
            // The following CommonMark:
            //     foo\
            //          bar
            //
            // Should be rendered as:
            //     <p>foo<br />
            //     bar</p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 530, "Inlines - Hard line breaks");
			Helpers.ExecuteTest("foo\\\n     bar", "<p>foo<br />\nbar</p>");
        }

        // Line breaks can occur inside emphasis, links, and other constructs
        // that allow inline content:
        [TestMethod]
        [TestCategory("Inlines - Hard line breaks")]
        //[Timeout(1000)]
        public void Example531()
        {
            // Example 531
            // Section: Inlines - Hard line breaks
            //
            // The following CommonMark:
            //     *foo  
            //     bar*
            //
            // Should be rendered as:
            //     <p><em>foo<br />
            //     bar</em></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 531, "Inlines - Hard line breaks");
			Helpers.ExecuteTest("*foo  \nbar*", "<p><em>foo<br />\nbar</em></p>");
        }

        [TestMethod]
        [TestCategory("Inlines - Hard line breaks")]
        //[Timeout(1000)]
        public void Example532()
        {
            // Example 532
            // Section: Inlines - Hard line breaks
            //
            // The following CommonMark:
            //     *foo\
            //     bar*
            //
            // Should be rendered as:
            //     <p><em>foo<br />
            //     bar</em></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 532, "Inlines - Hard line breaks");
			Helpers.ExecuteTest("*foo\\\nbar*", "<p><em>foo<br />\nbar</em></p>");
        }

        // Line breaks do not occur inside code spans
        [TestMethod]
        [TestCategory("Inlines - Hard line breaks")]
        //[Timeout(1000)]
        public void Example533()
        {
            // Example 533
            // Section: Inlines - Hard line breaks
            //
            // The following CommonMark:
            //     `code  
            //     span`
            //
            // Should be rendered as:
            //     <p><code>code span</code></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 533, "Inlines - Hard line breaks");
			Helpers.ExecuteTest("`code  \nspan`", "<p><code>code span</code></p>");
        }

        [TestMethod]
        [TestCategory("Inlines - Hard line breaks")]
        //[Timeout(1000)]
        public void Example534()
        {
            // Example 534
            // Section: Inlines - Hard line breaks
            //
            // The following CommonMark:
            //     `code\
            //     span`
            //
            // Should be rendered as:
            //     <p><code>code\ span</code></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 534, "Inlines - Hard line breaks");
			Helpers.ExecuteTest("`code\\\nspan`", "<p><code>code\\ span</code></p>");
        }

        // or HTML tags:
        [TestMethod]
        [TestCategory("Inlines - Hard line breaks")]
        //[Timeout(1000)]
        public void Example535()
        {
            // Example 535
            // Section: Inlines - Hard line breaks
            //
            // The following CommonMark:
            //     <a href="foo  
            //     bar">
            //
            // Should be rendered as:
            //     <p><a href="foo  
            //     bar"></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 535, "Inlines - Hard line breaks");
			Helpers.ExecuteTest("<a href=\"foo  \nbar\">", "<p><a href=\"foo  \nbar\"></p>");
        }

        [TestMethod]
        [TestCategory("Inlines - Hard line breaks")]
        //[Timeout(1000)]
        public void Example536()
        {
            // Example 536
            // Section: Inlines - Hard line breaks
            //
            // The following CommonMark:
            //     <a href="foo\
            //     bar">
            //
            // Should be rendered as:
            //     <p><a href="foo\
            //     bar"></p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 536, "Inlines - Hard line breaks");
			Helpers.ExecuteTest("<a href=\"foo\\\nbar\">", "<p><a href=\"foo\\\nbar\"></p>");
        }

        // Hard line breaks are for separating inline content within a block.
        // Neither syntax for hard line breaks works at the end of a paragraph or
        // other block element:
        [TestMethod]
        [TestCategory("Inlines - Hard line breaks")]
        //[Timeout(1000)]
        public void Example537()
        {
            // Example 537
            // Section: Inlines - Hard line breaks
            //
            // The following CommonMark:
            //     foo\
            //
            // Should be rendered as:
            //     <p>foo\</p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 537, "Inlines - Hard line breaks");
			Helpers.ExecuteTest("foo\\", "<p>foo\\</p>");
        }

        [TestMethod]
        [TestCategory("Inlines - Hard line breaks")]
        //[Timeout(1000)]
        public void Example538()
        {
            // Example 538
            // Section: Inlines - Hard line breaks
            //
            // The following CommonMark:
            //     foo  
            //
            // Should be rendered as:
            //     <p>foo</p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 538, "Inlines - Hard line breaks");
			Helpers.ExecuteTest("foo  ", "<p>foo</p>");
        }

        [TestMethod]
        [TestCategory("Inlines - Hard line breaks")]
        //[Timeout(1000)]
        public void Example539()
        {
            // Example 539
            // Section: Inlines - Hard line breaks
            //
            // The following CommonMark:
            //     ### foo\
            //
            // Should be rendered as:
            //     <h3>foo\</h3>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 539, "Inlines - Hard line breaks");
			Helpers.ExecuteTest("### foo\\", "<h3>foo\\</h3>");
        }

        [TestMethod]
        [TestCategory("Inlines - Hard line breaks")]
        //[Timeout(1000)]
        public void Example540()
        {
            // Example 540
            // Section: Inlines - Hard line breaks
            //
            // The following CommonMark:
            //     ### foo  
            //
            // Should be rendered as:
            //     <h3>foo</h3>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 540, "Inlines - Hard line breaks");
			Helpers.ExecuteTest("### foo  ", "<h3>foo</h3>");
        }

        // ## Soft line breaks
        //
        // A regular line break (not in a code span or HTML tag) that is not
        // preceded by two or more spaces is parsed as a softbreak.  (A
        // softbreak may be rendered in HTML either as a
        // [line ending] or as a space. The result will be the same
        // in browsers. In the examples here, a [line ending] will be used.)
        [TestMethod]
        [TestCategory("Inlines - Soft line breaks")]
        //[Timeout(1000)]
        public void Example541()
        {
            // Example 541
            // Section: Inlines - Soft line breaks
            //
            // The following CommonMark:
            //     foo
            //     baz
            //
            // Should be rendered as:
            //     <p>foo
            //     baz</p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 541, "Inlines - Soft line breaks");
			Helpers.ExecuteTest("foo\nbaz", "<p>foo\nbaz</p>");
        }

        // Spaces at the end of the line and beginning of the next line are
        // removed:
        [TestMethod]
        [TestCategory("Inlines - Soft line breaks")]
        //[Timeout(1000)]
        public void Example542()
        {
            // Example 542
            // Section: Inlines - Soft line breaks
            //
            // The following CommonMark:
            //     foo 
            //      baz
            //
            // Should be rendered as:
            //     <p>foo
            //     baz</p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 542, "Inlines - Soft line breaks");
			Helpers.ExecuteTest("foo \n baz", "<p>foo\nbaz</p>");
        }

        // A conforming parser may render a soft line break in HTML either as a
        // line break or as a space.
        //
        // A renderer may also provide an option to render soft line breaks
        // as hard line breaks.
        //
        // ## Textual content
        //
        // Any characters not given an interpretation by the above rules will
        // be parsed as plain textual content.
        [TestMethod]
        [TestCategory("Inlines - Textual content")]
        //[Timeout(1000)]
        public void Example543()
        {
            // Example 543
            // Section: Inlines - Textual content
            //
            // The following CommonMark:
            //     hello $.;'there
            //
            // Should be rendered as:
            //     <p>hello $.;'there</p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 543, "Inlines - Textual content");
			Helpers.ExecuteTest("hello $.;'there", "<p>hello $.;'there</p>");
        }

        [TestMethod]
        [TestCategory("Inlines - Textual content")]
        //[Timeout(1000)]
        public void Example544()
        {
            // Example 544
            // Section: Inlines - Textual content
            //
            // The following CommonMark:
            //     Foo χρῆν
            //
            // Should be rendered as:
            //     <p>Foo χρῆν</p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 544, "Inlines - Textual content");
			Helpers.ExecuteTest("Foo χρῆν", "<p>Foo χρῆν</p>");
        }

        // Internal spaces are preserved verbatim:
        [TestMethod]
        [TestCategory("Inlines - Textual content")]
        //[Timeout(1000)]
        public void Example545()
        {
            // Example 545
            // Section: Inlines - Textual content
            //
            // The following CommonMark:
            //     Multiple     spaces
            //
            // Should be rendered as:
            //     <p>Multiple     spaces</p>

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 545, "Inlines - Textual content");
			Helpers.ExecuteTest("Multiple     spaces", "<p>Multiple     spaces</p>");
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
        // Notice how the [line ending] in the first paragraph has
        // been parsed as a `softbreak`, and the asterisks in the first list item
        // have become an `emph`.
        //
        // The document can be rendered as HTML, or in any other format, given
        // an appropriate renderer.
    }
}
