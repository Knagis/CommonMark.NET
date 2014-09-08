using CommonMark.Formatter;
using CommonMark.Parser;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace CommonMark
{
    /// <summary>
    /// Contains methods for parsing and formatting CommonMark data.
    /// </summary>
    public static class CommonMarkConverter
    {
        /// <summary>
        /// Gets the CommonMark assembly version number.
        /// </summary>
        public static Version AssemblyVersion
        {
            get
            {
                return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            }
        }

        /// <summary>
        /// Performs the first stage of the conversion - parses block elements from the source and created the syntax tree.
        /// </summary>
        /// <param name="source">The reader that contains the source data.</param>
        /// <param name="settings">The object containing settings for the parsing process.</param>
        /// <returns>The block element that represents the document.</returns>
        /// <exception cref="ArgumentNullException">when <paramref name="source"/> is <c>null</c></exception>
        /// <exception cref="CommonMarkException">when errors occur during block parsing.</exception>
        /// <exception cref="IOException">when error occur while reading the data.</exception>
        public static Syntax.Block ProcessStage1(TextReader source, CommonMarkSettings settings = null)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            var cur = BlockMethods.make_document();

            int linenum = 1;
            try
            {
                while (source.Peek() != -1)
                {
                    BlockMethods.incorporate_line(source.ReadLine(), linenum, ref cur);
                    linenum++;
                }
            }
            catch(IOException)
            {
                throw;
            }
            catch(CommonMarkException)
            {
                throw;
            }
            catch(Exception ex)
            {
                throw new CommonMarkException("An error occured while parsing line " + linenum.ToString(CultureInfo.InvariantCulture), cur, ex);
            }

            try
            {
                while (cur != cur.top)
                {
                    BlockMethods.finalize(cur, linenum);
                    cur = cur.parent;
                }
            }
            catch (CommonMarkException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new CommonMarkException("An error occured while finalizing open containers.", cur, ex);
            }

            if (cur != cur.top)
                throw new CommonMarkException("Unable to finalize open containers.", cur);

            try
            {
                BlockMethods.finalize(cur, linenum);
            }
            catch(CommonMarkException)
            {
                throw;
            }
            catch(Exception ex)
            {
                throw new CommonMarkException("Unable to finalize document element.", cur, ex);
            }

            return cur;
        }

        /// <summary>
        /// Performs the second stage of the conversion - parses block element contents into inline elements.
        /// </summary>
        /// <param name="document">The top level document element.</param>
        /// <param name="settings">The object containing settings for the parsing process.</param>
        /// <exception cref="ArgumentException">when <paramref name="document"/> does not represent a top level document.</exception>
        /// <exception cref="ArgumentNullException">when <paramref name="document"/> is <c>null</c></exception>
        /// <exception cref="CommonMarkException">when errors occur during inline parsing.</exception>
        public static void ProcessStage2(Syntax.Block document, CommonMarkSettings settings = null)
        {
            if (document == null)
                throw new ArgumentNullException("document");

            if (document.tag != Syntax.BlockTag.document)
                throw new ArgumentException("The block element passed to this method must represent a top level document.", "document");

            try
            {
                BlockMethods.process_inlines(document, document.attributes.refmap);
            }
            catch(CommonMarkException)
            {
                throw;
            }
            catch(Exception ex)
            {
                throw new CommonMarkException("An error occured during inline parsing.", ex);
            }
        }

        /// <summary>
        /// Performs the last stage of the conversion - converts the syntax tree to HTML representation.
        /// </summary>
        /// <param name="document">The top level document element.</param>
        /// <param name="target">The target text writer where the result will be written to.</param>
        /// <param name="settings">The object containing settings for the formatting process.</param>
        /// <exception cref="ArgumentException">when <paramref name="document"/> does not represent a top level document.</exception>
        /// <exception cref="ArgumentNullException">when <paramref name="document"/> or <paramref name="target"/> is <c>null</c></exception>
        /// <exception cref="CommonMarkException">when errors occur during formatting.</exception>
        /// <exception cref="IOException">when error occur while writing the data to the target.</exception>
        public static void ProcessStage3(Syntax.Block document, TextWriter target, CommonMarkSettings settings = null)
        {
            if (document == null)
                throw new ArgumentNullException("document");

            if (target == null)
                throw new ArgumentNullException("target");

            if (document.tag != Syntax.BlockTag.document)
                throw new ArgumentException("The block element passed to this method must represent a top level document.", "document");

            if (settings == null)
                settings = CommonMarkSettings.Default;

            try
            {
                if (settings.OutputFormat == OutputFormat.SyntaxTree)
                {
                    Printer.print_blocks(target, document, 0);
                }
                else
                {
                    HtmlPrinter.blocks_to_html(target, document, false);
                }
            }
            catch (CommonMarkException)
            {
                throw;
            }
            catch(IOException)
            {
                throw;
            }
            catch(Exception ex)
            {
                throw new CommonMarkException("An error occured during formatting of the document.", ex);
            }
        }

        /// <summary>
        /// Converts the given source data and writes the result directly to the target.
        /// </summary>
        /// <param name="source">The reader that contains the source data.</param>
        /// <param name="target">The target text writer where the result will be written to.</param>
        /// <param name="settings">The object containing settings for the parsing and formatting process.</param>
        /// <exception cref="ArgumentNullException">when <paramref name="source"/> or <paramref name="target"/> is <c>null</c></exception>
        /// <exception cref="CommonMarkException">when errors occur during parsing or formatting.</exception>
        /// <exception cref="IOException">when error occur while reading or writing the data.</exception>
        public static void Convert(TextReader source, TextWriter target, CommonMarkSettings settings = null)
        {
            if (settings == null)
                settings = CommonMarkSettings.Default;

            var document = ProcessStage1(source, settings);
            ProcessStage2(document, settings);
            ProcessStage3(document, target, settings);
        }

        /// <summary>
        /// Converts the given source data and returns the result as a string.
        /// </summary>
        /// <param name="source">The source data.</param>
        /// <param name="settings">The object containing settings for the parsing and formatting process.</param>
        /// <exception cref="CommonMarkException">when errors occur during parsing or formatting.</exception>
        /// <returns>The converted data.</returns>
        public static string Convert(string source, CommonMarkSettings settings = null)
        {
            if (source == null)
                return null;

            using (var reader = new System.IO.StringReader(source))
            using (var writer = new System.IO.StringWriter())
            {
                Convert(reader, writer, settings);

                return writer.ToString();
            }
        }
    }
}
