using CommonMark.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace CommonMark.Tests
{
    [TestClass]
    public class TableTests
    {
        static CommonMarkSettings ReadSettings;
        static CommonMarkSettings WriteSettings;

        static TableTests()
        {
            ReadSettings = CommonMarkSettings.Default.Clone();
            ReadSettings.AdditionalFeatures = CommonMarkAdditionalFeatures.GithubStyleTables;
            ReadSettings.TrackSourcePosition = true;

            WriteSettings = CommonMarkSettings.Default.Clone();
            WriteSettings.AdditionalFeatures = CommonMarkAdditionalFeatures.GithubStyleTables;
        }

        [TestMethod]
        public void SimpleTable()
        {
            var markdown = "First Header | Second Header\n------------- | -------------\nContent Cell | Content Cell\nContent Cell | Content Cell\n";

            var ast =
                CommonMarkConverter.Parse(
                    markdown,
                    ReadSettings
                );

            string html;
            using (var str = new StringWriter())
            {
                CommonMarkConverter.ProcessStage3(ast, str, WriteSettings);
                html = str.ToString();
            }
            Assert.AreEqual("<table><thead><tr><th>First Header</th><th>Second Header</th></tr></thead><tbody><tr><td>Content Cell</td><td>Content Cell</td></tr><tr><td>Content Cell</td><td>Content Cell</td></tr></tbody></table>", html);

            var firstChild = ast.FirstChild;
            Assert.AreEqual(BlockTag.Table, firstChild.Tag);
            Assert.AreEqual(markdown, markdown.Substring(firstChild.SourcePosition, firstChild.SourceLength));
            Assert.IsNotNull(firstChild.TableHeaderAlignments);
            Assert.AreEqual(2, firstChild.TableHeaderAlignments.Length);
            Assert.AreEqual(TableHeaderAlignment.None, firstChild.TableHeaderAlignments[0]);
            Assert.AreEqual(TableHeaderAlignment.None, firstChild.TableHeaderAlignments[1]);

            var headerRow = firstChild.FirstChild;
            Assert.AreEqual(BlockTag.TableRow, headerRow.Tag);
            Assert.AreEqual("First Header | Second Header\n", markdown.Substring(headerRow.SourcePosition, headerRow.SourceLength));

            var headerCell1 = headerRow.FirstChild;
            Assert.AreEqual(BlockTag.TableCell, headerCell1.Tag);
            Assert.AreEqual("First Header", markdown.Substring(headerCell1.SourcePosition, headerCell1.SourceLength));

            var headerCell2 = headerCell1.NextSibling;
            Assert.AreEqual(BlockTag.TableCell, headerCell2.Tag);
            Assert.AreEqual("Second Header", markdown.Substring(headerCell2.SourcePosition, headerCell2.SourceLength));
            Assert.IsNull(headerCell2.NextSibling);

            var firstRow = headerRow.NextSibling;
            Assert.AreEqual(BlockTag.TableRow, firstRow.Tag);
            Assert.AreEqual("Content Cell | Content Cell\n", markdown.Substring(firstRow.SourcePosition, firstRow.SourceLength));

            var firstRowCell1 = firstRow.FirstChild;
            Assert.AreEqual(BlockTag.TableCell, firstRowCell1.Tag);
            Assert.AreEqual("Content Cell", markdown.Substring(firstRowCell1.SourcePosition, firstRowCell1.SourceLength));

            var firstRowCell2 = firstRowCell1.NextSibling;
            Assert.AreEqual(BlockTag.TableCell, firstRowCell2.Tag);
            Assert.AreEqual("Content Cell", markdown.Substring(firstRowCell2.SourcePosition, firstRowCell2.SourceLength));
            Assert.IsNull(firstRowCell2.NextSibling);

            var secondRow = firstRow.NextSibling;
            Assert.AreEqual(BlockTag.TableRow, secondRow.Tag);
            Assert.AreEqual("Content Cell | Content Cell\n", markdown.Substring(secondRow.SourcePosition, secondRow.SourceLength));
            Assert.IsNull(secondRow.NextSibling);

            var secondRowCell1 = secondRow.FirstChild;
            Assert.AreEqual(BlockTag.TableCell, secondRowCell1.Tag);
            Assert.AreEqual("Content Cell", markdown.Substring(secondRowCell1.SourcePosition, secondRowCell1.SourceLength));

            var secondRowCell2 = secondRowCell1.NextSibling;
            Assert.AreEqual(BlockTag.TableCell, secondRowCell2.Tag);
            Assert.AreEqual("Content Cell", markdown.Substring(secondRowCell2.SourcePosition, secondRowCell2.SourceLength));
            Assert.IsNull(secondRowCell2.NextSibling);
        }

        [TestMethod]
        public void WrappedTable()
        {
            var markdown =
@"Nope nope.

First Header  | Second Header
------------- | -------------
Content Cell  | Content Cell
Content Cell  | Content Cell
Hello world
";

            var ast =
                CommonMarkConverter.Parse(
                    markdown,
                    ReadSettings
                );

            string html;
            using (var str = new StringWriter())
            {
                CommonMarkConverter.ProcessStage3(ast, str, WriteSettings);
                html = str.ToString();
            }
			string expected = "<p>Nope nope.</p>\r\n<table><thead><tr><th>First Header</th><th>Second Header</th></tr></thead><tbody><tr><td>Content Cell</td><td>Content Cell</td></tr><tr><td>Content Cell</td><td>Content Cell</td></tr><tr><td>Hello world</td><td></td></tr></tbody></table>";

			Assert.AreEqual(expected, html);

            Assert.AreEqual(BlockTag.Paragraph, ast.FirstChild.Tag);
            Assert.AreEqual(BlockTag.Table, ast.FirstChild.NextSibling.Tag);
        }

        [TestMethod]
        public void TableWithInlines()
        {
            var markdown =
@" Name | Description          
 ------------- | ----------- 
 Help      | **Display the** [help](/help) window.
 Close     | _Closes_ a window     ";

            var ast =
                CommonMarkConverter.Parse(
                    markdown,
                    ReadSettings
                );
            string html;
            using (var str = new StringWriter())
            {
                CommonMarkConverter.ProcessStage3(ast, str, WriteSettings);
                html = str.ToString();
            }
            Assert.AreEqual("<table><thead><tr><th>Name</th><th>Description</th></tr></thead><tbody><tr><td>Help</td><td><strong>Display the</strong> <a href=\"/help\">help</a> window.</td></tr><tr><td>Close</td><td><em>Closes</em> a window</td></tr></tbody></table>", html);
        }

        [TestMethod]
        public void TableCellMismatch()
        {
            var markdown =
@"| First Header  | Second Header |
| ------------- | ------------- |
| 11  |
| 21  | 22  | 23
";

            var ast = CommonMarkConverter.Parse(markdown, ReadSettings);
            string html;
            using (var str = new StringWriter())
            {
                CommonMarkConverter.ProcessStage3(ast, str, WriteSettings);
                html = str.ToString();
            }
            Assert.AreEqual("<table><thead><tr><th>First Header</th><th>Second Header</th></tr></thead><tbody><tr><td>11</td><td></td></tr><tr><td>21</td><td>22</td></tr></tbody></table>", html);
        }

        [TestMethod]
        public void TableAlignment()
        {
            var markdown =
@"| H1  | H2 | H3 |      H4
 ---    | :--   | ---:|   :-: |
|1|2|3|4|
";

            var ast = CommonMarkConverter.Parse(markdown, ReadSettings);
            var table = ast.FirstChild;
            Assert.AreEqual(BlockTag.Table, table.Tag);
            Assert.AreEqual(4, table.TableHeaderAlignments.Length);
            Assert.AreEqual(TableHeaderAlignment.None, table.TableHeaderAlignments[0]);
            Assert.AreEqual(TableHeaderAlignment.Left, table.TableHeaderAlignments[1]);
            Assert.AreEqual(TableHeaderAlignment.Right, table.TableHeaderAlignments[2]);
            Assert.AreEqual(TableHeaderAlignment.Center, table.TableHeaderAlignments[3]);
            string html;
            using (var str = new StringWriter())
            {
                CommonMarkConverter.ProcessStage3(ast, str, WriteSettings);
                html = str.ToString();
            }
            Assert.AreEqual("<table><thead><tr><th>H1</th><th align=\"left\">H2</th><th align=\"right\">H3</th><th align=\"center\">H4</th></tr></thead><tbody><tr><td>1</td><td align=\"left\">2</td><td align=\"right\">3</td><td align=\"center\">4</td></tr></tbody></table>", html);
        }


		[TestMethod]
        public void Example189()
        {
            var markdown = @"| foo | bar |
| --- | --- |
| baz | bim | ";

            var ast = CommonMarkConverter.Parse(markdown, ReadSettings);

            string html;
            using (var str = new StringWriter())
            {
                CommonMarkConverter.ProcessStage3(ast, str, WriteSettings);
                html = str.ToString();
            }
            Assert.AreEqual("<table><thead><tr><th>foo</th><th>bar</th></tr></thead><tbody><tr><td>baz</td><td>bim</td></tr></tbody></table>", html);
        }

		[TestMethod]
        public void Example190()
        {
            var markdown = @"| abc | defghi |
:-: | -----------:
bar | baz";

            var ast = CommonMarkConverter.Parse(markdown, ReadSettings);

            string html;
            using (var str = new StringWriter())
            {
                CommonMarkConverter.ProcessStage3(ast, str, WriteSettings);
                html = str.ToString();
            }
            Assert.AreEqual("<table><thead><tr><th align=\"center\">abc</th><th align=\"right\">defghi</th></tr></thead><tbody><tr><td align=\"center\">bar</td><td align=\"right\">baz</td></tr></tbody></table>", html);
        }

		[TestMethod]
        public void Example191()
        {
            var markdown = @"| f\|oo  |
| ------ |
| b `|` az |
| b **|** im |";

            var ast = CommonMarkConverter.Parse(markdown, ReadSettings);

            string html;
            using (var str = new StringWriter())
            {
                CommonMarkConverter.ProcessStage3(ast, str, WriteSettings);
                html = str.ToString();
            }
            Assert.AreEqual("<table><thead><tr><th>f|oo</th></tr></thead><tbody><tr><td>b <code>|</code> az</td></tr><tr><td>b <strong>|</strong> im</td></tr></tbody></table>", html);
        }

		[TestMethod]
        public void Example192()
        {
            var markdown = @"| abc | def |
| --- | --- |
| bar | baz |
> bar";

            var ast = CommonMarkConverter.Parse(markdown, ReadSettings);

            string html;
            using (var str = new StringWriter())
            {
                CommonMarkConverter.ProcessStage3(ast, str, WriteSettings);
                html = str.ToString();
            }

			string expected = @"<table><thead><tr><th>abc</th><th>def</th></tr></thead><tbody><tr><td>bar</td><td>baz</td></tr></tbody></table>
<blockquote>
<p>bar</p>
</blockquote>
";

			Assert.AreEqual(expected, html);
        }

		[TestMethod]
        public void Example193()
        {
            var markdown = @"| abc | def |
| --- | --- |
| bar | baz |
bar

bar";

            var ast = CommonMarkConverter.Parse(markdown, ReadSettings);

            string html;
            using (var str = new StringWriter())
            {
                CommonMarkConverter.ProcessStage3(ast, str, WriteSettings);
                html = str.ToString();
            }

			string expected = @"<table><thead><tr><th>abc</th><th>def</th></tr></thead><tbody><tr><td>bar</td><td>baz</td></tr><tr><td>bar</td><td></td></tr></tbody></table>
<p>bar</p>
";


			Assert.AreEqual(expected, html);
        }

		[TestMethod]
		public void Example194()
		{
			var markdown = @"| abc | def |
| --- |
| bar |";

			var ast = CommonMarkConverter.Parse(markdown, ReadSettings);

			string html;
			using (var str = new StringWriter())
			{
				CommonMarkConverter.ProcessStage3(ast, str, WriteSettings);
				html = str.ToString();
			}
			Assert.AreEqual(@"<p>| abc | def |
| --- |
| bar |</p>
", html);
		}

		[TestMethod]
        public void Example195()
        {
            var markdown = @"| abc | def |
| --- | --- |
| bar |
| bar | baz | boo |";

            var ast = CommonMarkConverter.Parse(markdown, ReadSettings);

            string html;
            using (var str = new StringWriter())
            {
                CommonMarkConverter.ProcessStage3(ast, str, WriteSettings);
                html = str.ToString();
            }
            Assert.AreEqual("<table><thead><tr><th>abc</th><th>def</th></tr></thead><tbody><tr><td>bar</td><td></td></tr><tr><td>bar</td><td>baz</td></tr></tbody></table>", html);
        }
    }
}