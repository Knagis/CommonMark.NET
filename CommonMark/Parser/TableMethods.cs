using System.Collections.Generic;
using System.Text;
using CommonMark.Syntax;

namespace CommonMark.Parser
{
	internal static class TableMethods
	{
		static List<string> ParseTableLine(StringPart part, StringBuilder sb)
		{
			string line = part.Source.Substring(part.StartIndex, part.Length);
			line = line.TrimEnd('\n');

			var ret = new List<string>();

			var i = 0;

			if (i < line.Length && line[i] == '|') i++;

			while (i < line.Length && char.IsWhiteSpace(line[i])) i++;

			for (; i < line.Length; i++)
			{
				var c = line[i];
				if (c == '\\')
				{
					i++;
					if (i < line.Length && line[i] == '|')
					{
						sb.Append(line[i]);
						continue;
					}
					i--;
				}

				if (c == '|')
				{
					ret.Add(sb.ToString());
					Utilities.ClearStringBuilder(sb);
				}
				else
				{
					sb.Append(c);
				}
			}

			if (sb.Length != 0)
			{
				ret.Add(sb.ToString());
				Utilities.ClearStringBuilder(sb);
			}

			return ret;
		}

		static void MakeTableCells(Block row, StringBuilder sb)
		{
			var offset = 0;

			var parts = row.StringContent.RetrieveParts();
			foreach (var part in parts.Array)
			{
				if (part.Length <= 0)
					continue;

				string asStr = part.Source.Substring(part.StartIndex, part.Length);

				for (var i = 0; i < asStr.Length; i++)
				{
					var c = asStr[i];

					if (c == '|')
					{
						var text = sb.ToString();
						Utilities.ClearStringBuilder(sb);

						if (text.Length > 0)
						{
							int length = text.Length;
							string trimmedText = text.TrimStart();
							var leadingWhiteSpace = length - trimmedText.Length;
							trimmedText = trimmedText.TrimEnd();
							var trailingWhiteSpace = length - leadingWhiteSpace - text.Length;

							var cell = new Block(BlockTag.TableCell, row.SourcePosition + offset + leadingWhiteSpace);
							cell.SourceLastPosition = cell.SourcePosition + trimmedText.Length;
							cell.StringContent = new StringContent();
							cell.StringContent.Append(trimmedText, 0, trimmedText.Length);

							if (row.LastChild == null)
							{
								row.FirstChild = row.LastChild = cell;
							}
							else
							{
								row.LastChild.NextSibling = cell;
								row.LastChild = cell;
							}

							cell.IsOpen = false;
						}

						offset += text.Length;

						// skip the |
						offset++;
						continue;
					}

					if (c == '\\')
					{
						sb.Append(c);
						if (i + 1 < asStr.Length)
						{
							if (Utilities.IsEscapableSymbol(asStr[i + 1]))
								sb.Append(asStr[i + 1]);
						}
						i++;
					}
					else
					{
						sb.Append(c);
					}
				}
			}

			if (sb.Length > 0)
			{
				var text = sb.ToString();
				Utilities.ClearStringBuilder(sb);

				if (text.Length > 0)
				{
					var leadingWhiteSpace = 0;
					while (leadingWhiteSpace < text.Length && char.IsWhiteSpace(text[leadingWhiteSpace])) leadingWhiteSpace++;
					var trailingWhiteSpace = 0;
					while (trailingWhiteSpace < text.Length && char.IsWhiteSpace(text[text.Length - trailingWhiteSpace - 1])) trailingWhiteSpace++;

					if (text.Length - leadingWhiteSpace - trailingWhiteSpace > 0)
					{
						var cell = new Block(BlockTag.TableCell, row.SourcePosition + offset + leadingWhiteSpace);
						cell.SourceLastPosition = cell.SourcePosition + text.Length - trailingWhiteSpace - leadingWhiteSpace;
						cell.StringContent = new StringContent();
						cell.StringContent.Append(text, leadingWhiteSpace, text.Length - leadingWhiteSpace - trailingWhiteSpace);

						if (row.LastChild == null)
						{
							row.FirstChild = row.LastChild = cell;
						}
						else
						{
							row.LastChild.NextSibling = cell;
							row.LastChild = cell;
						}

						cell.IsOpen = false;
					}
				}
			}
		}

		static void MakeTableRows(Block table, StringBuilder sb)
		{
			var parts = table.StringContent.RetrieveParts();
			var offset = 0;

			for (var i = 0; i < parts.Array.Length; i++)
			{
				var line = parts.Array[i];
				if (line.Length <= 0)
					continue;

				var lineLength = line.Length;
				string actualLine = line.Source.Substring(line.StartIndex, line.Length);

				// skip the header row
				if (i != 1 && !string.IsNullOrEmpty(actualLine) && actualLine != " ")
				{
					var rowStartsInDocument = table.SourcePosition + offset;
					var row = new Block(BlockTag.TableRow, rowStartsInDocument);
					row.SourceLastPosition = rowStartsInDocument + lineLength;

					row.StringContent = new StringContent();
					row.StringContent.Append(actualLine, 0, actualLine.Length);

					if (table.LastChild == null)
					{
						table.FirstChild = row;
						table.LastChild = row;
					}
					else
					{
						table.LastChild.NextSibling = row;
						table.LastChild = row;
					}

					MakeTableCells(row, sb);
					row.IsOpen = false;
				}

				offset += lineLength;
			}
		}

		internal static bool TryMakeTable(Block b, LineInfo line, CommonMarkSettings settings)
		{
			if ((settings.AdditionalFeatures & CommonMarkAdditionalFeatures.GithubStyleTables) == 0) return false;

			var parts = b.StringContent.RetrieveParts().Array;

			if (parts.Length < 2) return false;

			var sb = new StringBuilder();

			var columnsPart = parts[0];
			var columnsLine = ParseTableLine(columnsPart, sb);
			if (columnsLine.Count == 1) return false;

			var headersPart = parts[1];
			var headerLine = ParseTableLine(headersPart, sb);
			if (headerLine.Count == 1) return false;

			TableHeaderAlignment[] headerAlignment = new TableHeaderAlignment[headerLine.Count];

			for (int hl = 0; hl < headerLine.Count; hl++)
			{
				var headerPart = headerLine[hl];
				var trimmed = headerPart.Trim();
				if (trimmed.Length < 3) return false;

				var validateFrom = 0;
				var startsWithColon = trimmed[validateFrom] == ':';
				if (startsWithColon) validateFrom++;

				var validateTo = trimmed.Length - 1;
				var endsWithColon = trimmed[validateTo] == ':';
				if (endsWithColon) validateTo--;

				for (var i = validateFrom; i <= validateTo; i++)
				{
					// don't check for escapes, they don't count in header
					if (trimmed[i] != '-') return false;
				}

				if (!startsWithColon && !endsWithColon)
				{
					headerAlignment[hl] = TableHeaderAlignment.None;
					continue;
				}

				if (startsWithColon && endsWithColon)
				{
					headerAlignment[hl] = TableHeaderAlignment.Center;
					continue;
				}

				if (startsWithColon)
				{
					headerAlignment[hl] = TableHeaderAlignment.Left;
				}

				if (endsWithColon)
				{
					headerAlignment[hl] = TableHeaderAlignment.Right;
				}
			}

			if (columnsLine.Count < 2) return false;
			if (headerLine.Count < columnsLine.Count) return false;

			var lastTableLine = 1;

			// it's a table!
			List<StringPart> tableParts = new List<StringPart> { columnsPart, headersPart };
			var takingCharsForTable = columnsPart.Length + headersPart.Length;
			for (var i = 2; i < parts.Length; i++)
			{
				var hasPipe = false;
				var part = parts[i];

				if (part.Length <= 0)
					continue;

				string strLine = part.Source.Substring(part.StartIndex, part.Length);

				int indexOfPipe = strLine.IndexOf('|');
				hasPipe = indexOfPipe == 0;

				while (!hasPipe)
				{
					if (indexOfPipe > 0 && strLine[indexOfPipe - 1] == '\\')
					{
						indexOfPipe = strLine.IndexOf('|', indexOfPipe);
					}
					else if (indexOfPipe > 0)
					{
						hasPipe = true;
						break;
					}
					else
					{
						break;
					}
				}

				if (!hasPipe) break;

				tableParts.Add(part);
				takingCharsForTable += part.Length;
				lastTableLine = i;
			}

			bool hasTrailingParts = false;
			for (var i = lastTableLine + 1; i < parts.Length; i++)
			{
				var part = parts[i];
				if (part.Length <= 0)
					continue;

				hasTrailingParts = true;
				break;
			}

			// No need to break, the whole block is a table now
			if (!hasTrailingParts)
			{
				b.Tag = BlockTag.Table;
				b.TableHeaderAlignments = headerAlignment;

				// create table rows
				MakeTableRows(b, sb);
				return true;
			}

			// get the text of the table separate
			var tableBlockString = b.StringContent.TakeFromStart(takingCharsForTable, trim: true);
			var newBlock = new Block(BlockTag.Paragraph, b.SourcePosition + tableBlockString.Length);

			// create the trailing paragraph, and set it's text and source positions
			var newParagraph = b.Clone();
			newParagraph.StringContent = b.StringContent;
			if (settings.TrackSourcePosition)
			{
				newParagraph.SourcePosition = b.SourcePosition + tableBlockString.Length;
				newParagraph.SourceLastPosition = newParagraph.SourcePosition + (b.SourceLength - tableBlockString.Length);
			}

			// update the text of the table block
			b.Tag = BlockTag.Table;
			b.TableHeaderAlignments = headerAlignment;
			b.StringContent = new StringContent();
			foreach (StringPart part in tableParts)
			{
				b.StringContent.Append(part.Source, part.StartIndex, part.Length);
			}
			if (settings.TrackSourcePosition)
			{
				b.SourceLastPosition = b.SourcePosition + tableBlockString.Length;
			}

			// create table rows
			MakeTableRows(b, sb);

			// put the new paragraph after the table
			newParagraph.NextSibling = b.NextSibling;
			b.NextSibling = newParagraph;

			BlockMethods.Finalize(newParagraph, line, settings);

			return true;
		}
	}
}
