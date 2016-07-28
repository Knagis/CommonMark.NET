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
            Assert.AreEqual("<table><thead><tr><th>First Header</th><th>Second Header</th></tr></thead><tbody><tr><td>Content Cell</td><td>Content Cell</td></tr><tr><td>Content Cell</td><td>Content Cell</td></tr></tbody></table>\r\n", html);

            var firstChild = ast.FirstChild;
            Assert.AreEqual(BlockTag.Table, firstChild.Tag);
            Assert.AreEqual(markdown, markdown.Substring(firstChild.SourcePosition, firstChild.SourceLength));
            Assert.IsNotNull(firstChild.TableHeaderAlignments);
            Assert.AreEqual(2, firstChild.TableHeaderAlignments.Count);
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
        public void SplitTable()
        {
            var markdown =
@"First Header  | Second Header
------------- | -------------
Content Cell1  | Content Cell2
Content Cell3  | Content Cell4
Hello world
";
            markdown = markdown.Replace("\r\n", "\n");

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
            Assert.AreEqual("<table><thead><tr><th>First Header</th><th>Second Header</th></tr></thead><tbody><tr><td>Content Cell1</td><td>Content Cell2</td></tr><tr><td>Content Cell3</td><td>Content Cell4</td></tr></tbody></table>\r\n<p>Hello world</p>\r\n\r\n", html);

            var firstChild = ast.FirstChild;
            var secondChild = firstChild.NextSibling;
            Assert.AreEqual(BlockTag.Table, firstChild.Tag);
            var firstMarkdown = markdown.Substring(firstChild.SourcePosition, firstChild.SourceLength);
            var shouldMatch = @"First Header  | Second Header
------------- | -------------
Content Cell1  | Content Cell2
Content Cell3  | Content Cell4
";
            shouldMatch = shouldMatch.Replace("\r\n", "\n");

            Assert.AreEqual(shouldMatch, firstMarkdown);
            Assert.IsNotNull(firstChild.TableHeaderAlignments);
            Assert.AreEqual(2, firstChild.TableHeaderAlignments.Count);
            Assert.AreEqual(TableHeaderAlignment.None, firstChild.TableHeaderAlignments[0]);
            Assert.AreEqual(TableHeaderAlignment.None, firstChild.TableHeaderAlignments[1]);

            var headerRow = firstChild.FirstChild;
            Assert.AreEqual(BlockTag.TableRow, headerRow.Tag);
            Assert.AreEqual("First Header  | Second Header\n", markdown.Substring(headerRow.SourcePosition, headerRow.SourceLength));

            var headerCell1 = headerRow.FirstChild;
            Assert.AreEqual(BlockTag.TableCell, headerCell1.Tag);
            Assert.AreEqual("First Header", markdown.Substring(headerCell1.SourcePosition, headerCell1.SourceLength));

            var headerCell2 = headerCell1.NextSibling;
            Assert.AreEqual(BlockTag.TableCell, headerCell2.Tag);
            Assert.AreEqual("Second Header", markdown.Substring(headerCell2.SourcePosition, headerCell2.SourceLength));
            Assert.IsNull(headerCell2.NextSibling);

            var firstRow = headerRow.NextSibling;
            Assert.AreEqual(BlockTag.TableRow, firstRow.Tag);
            Assert.AreEqual("Content Cell1  | Content Cell2\n", markdown.Substring(firstRow.SourcePosition, firstRow.SourceLength));

            var firstRowCell1 = firstRow.FirstChild;
            Assert.AreEqual(BlockTag.TableCell, firstRowCell1.Tag);
            Assert.AreEqual("Content Cell1", markdown.Substring(firstRowCell1.SourcePosition, firstRowCell1.SourceLength));

            var firstRowCell2 = firstRowCell1.NextSibling;
            Assert.AreEqual(BlockTag.TableCell, firstRowCell2.Tag);
            Assert.AreEqual("Content Cell2", markdown.Substring(firstRowCell2.SourcePosition, firstRowCell2.SourceLength));
            Assert.IsNull(firstRowCell2.NextSibling);

            var secondRow = firstRow.NextSibling;
            Assert.AreEqual(BlockTag.TableRow, secondRow.Tag);
            Assert.AreEqual("Content Cell3  | Content Cell4\n", markdown.Substring(secondRow.SourcePosition, secondRow.SourceLength));
            Assert.IsNull(secondRow.NextSibling);

            var secondRowCell1 = secondRow.FirstChild;
            Assert.AreEqual(BlockTag.TableCell, secondRowCell1.Tag);
            Assert.AreEqual("Content Cell3", markdown.Substring(secondRowCell1.SourcePosition, secondRowCell1.SourceLength));

            var secondRowCell2 = secondRowCell1.NextSibling;
            Assert.AreEqual(BlockTag.TableCell, secondRowCell2.Tag);
            Assert.AreEqual("Content Cell4", markdown.Substring(secondRowCell2.SourcePosition, secondRowCell2.SourceLength));
            Assert.IsNull(secondRowCell2.NextSibling);

            Assert.AreEqual(BlockTag.Paragraph, secondChild.Tag);
            var secondMarkdown = markdown.Substring(secondChild.SourcePosition, secondChild.SourceLength);
            Assert.AreEqual("Hello world\n", secondMarkdown);
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
            Assert.AreEqual("<p>Nope nope.</p>\r\n<table><thead><tr><th>First Header</th><th>Second Header</th></tr></thead><tbody><tr><td>Content Cell</td><td>Content Cell</td></tr><tr><td>Content Cell</td><td>Content Cell</td></tr></tbody></table>\r\n<p>Hello world</p>\r\n\r\n", html);

            Assert.AreEqual(BlockTag.Paragraph, ast.FirstChild.Tag);
            Assert.AreEqual(BlockTag.Table, ast.FirstChild.NextSibling.Tag);
            Assert.AreEqual(BlockTag.Paragraph, ast.FirstChild.NextSibling.NextSibling.Tag);
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
            Assert.AreEqual("<table><thead><tr><th>Name</th><th>Description</th></tr></thead><tbody><tr><td>Help</td><td><strong>Display the</strong> <a href=\"/help\">help</a> window.</td></tr><tr><td>Close</td><td><em>Closes</em> a window</td></tr></tbody></table>\r\n", html);
        }

        [TestMethod]
        public void TableWithExtraPipes()
        {
            var markdown = "| First Header  | Second Header |\n| ------------- | ------------- |\n| cell #11  | cell #12  |\n| cell #21  | cell #22  |\n";

            var ast =
                CommonMarkConverter.Parse(
                    markdown,
                    ReadSettings
                );

            var firstChild = ast.FirstChild;
            Assert.AreEqual(BlockTag.Table, firstChild.Tag);
            Assert.AreEqual(markdown, markdown.Substring(firstChild.SourcePosition, firstChild.SourceLength));

            var headerRow = firstChild.FirstChild;
            Assert.AreEqual(BlockTag.TableRow, headerRow.Tag);
            Assert.AreEqual("| First Header  | Second Header |\n", markdown.Substring(headerRow.SourcePosition, headerRow.SourceLength));

            var headerCell1 = headerRow.FirstChild;
            Assert.AreEqual(BlockTag.TableCell, headerCell1.Tag);
            Assert.AreEqual("First Header", markdown.Substring(headerCell1.SourcePosition, headerCell1.SourceLength));

            var headerCell2 = headerCell1.NextSibling;
            Assert.AreEqual(BlockTag.TableCell, headerCell2.Tag);
            Assert.AreEqual("Second Header", markdown.Substring(headerCell2.SourcePosition, headerCell2.SourceLength));
            Assert.IsNull(headerCell2.NextSibling);

            var firstRow = headerRow.NextSibling;
            Assert.AreEqual(BlockTag.TableRow, firstRow.Tag);
            Assert.AreEqual("| cell #11  | cell #12  |\n", markdown.Substring(firstRow.SourcePosition, firstRow.SourceLength));

            var firstRowCell1 = firstRow.FirstChild;
            Assert.AreEqual(BlockTag.TableCell, firstRowCell1.Tag);
            Assert.AreEqual("cell #11", markdown.Substring(firstRowCell1.SourcePosition, firstRowCell1.SourceLength));

            var firstRowCell2 = firstRowCell1.NextSibling;
            Assert.AreEqual(BlockTag.TableCell, firstRowCell2.Tag);
            Assert.AreEqual("cell #12", markdown.Substring(firstRowCell2.SourcePosition, firstRowCell2.SourceLength));
            Assert.IsNull(firstRowCell2.NextSibling);

            var secondRow = firstRow.NextSibling;
            Assert.AreEqual(BlockTag.TableRow, secondRow.Tag);
            Assert.AreEqual("| cell #21  | cell #22  |\n", markdown.Substring(secondRow.SourcePosition, secondRow.SourceLength));
            Assert.IsNull(secondRow.NextSibling);

            var secondRowCell1 = secondRow.FirstChild;
            Assert.AreEqual(BlockTag.TableCell, secondRowCell1.Tag);
            Assert.AreEqual("cell #21", markdown.Substring(secondRowCell1.SourcePosition, secondRowCell1.SourceLength));

            var secondRowCell2 = secondRowCell1.NextSibling;
            Assert.AreEqual(BlockTag.TableCell, secondRowCell2.Tag);
            Assert.AreEqual("cell #22", markdown.Substring(secondRowCell2.SourcePosition, secondRowCell2.SourceLength));
            Assert.IsNull(secondRowCell2.NextSibling);
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
            Assert.AreEqual("<table><thead><tr><th>First Header</th><th>Second Header</th></tr></thead><tbody><tr><td>11</td><td></td></tr><tr><td>21</td><td>22</td></tr></tbody></table>\r\n", html);
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
            Assert.AreEqual(4, table.TableHeaderAlignments.Count);
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
            Assert.AreEqual("<table><thead><tr><th>H1</th><th align=\"left\">H2</th><th align=\"right\">H3</th><th align=\"center\">H4</th></tr></thead><tbody><tr><td>1</td><td align=\"left\">2</td><td align=\"right\">3</td><td align=\"center\">4</td></tr></tbody></table>\r\n", html);
        }
    }
}