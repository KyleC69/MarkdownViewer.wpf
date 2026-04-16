// Build Date: 2026/04/15
// Solution: Markdig.Wpf
// Project:   MarkdownViewer.Wpf
// File:         DefaultTableRenderer.cs
// Author: Kyle L. Crowder
// Build Num: 184533



using Markdig.Extensions.Tables;
using Markdig.Syntax;




namespace MarkdownViewer.Wpf.Rendering.Xaml.Defaults.Blocks
{


    /// <summary>
    ///     Default XAML renderer for markdown tables.
    /// </summary>
    public sealed class DefaultTableRenderer : DefaultXamlBlockRenderer<Table>
    {
        protected override void WriteObject(Abstractions.IXamlRenderContext renderer, Table table)
        {
            renderer.EnsureLine();

            renderer.Write("<Table");
            WriteStyleAttribute(renderer, nameof(Styles.TableStyleKey));
            renderer.WriteLine(">");

            if (table.ColumnDefinitions.Count > 0)
            {
                renderer.WriteLine("<Table.Columns>");

                foreach (TableColumnDefinition? tableColumnDefinition in table.ColumnDefinitions)
                {
                    renderer.Write("<TableColumn Width=\"");
                    if ((tableColumnDefinition?.Width ?? 0) != 0)
                    {
                        renderer.Write(tableColumnDefinition!.Width.ToString()).Write("*");
                    }
                    else
                    {
                        renderer.Write("Auto");
                    }

                    renderer.WriteLine("\" />");
                }

                renderer.WriteLine("</Table.Columns>");
            }

            renderer.WriteLine("<TableRowGroup>");

            foreach (Block? rowObj in table)
            {
                TableRow? row = (TableRow)rowObj;

                renderer.Write("<TableRow");
                if (row.IsHeader)
                {
                    WriteStyleAttribute(renderer, nameof(Styles.TableHeaderStyleKey));
                }

                renderer.WriteLine(">");

                for (var index = 0; index < row.Count; index++)
                {
                    TableCell? cell = (TableCell)row[index];

                    renderer.Write("<TableCell");
                    WriteStyleAttribute(renderer, nameof(Styles.TableCellStyleKey));

                    if (cell.ColumnSpan > 1)
                    {
                        renderer.Write(" ColumnSpan=\"").Write(cell.ColumnSpan.ToString()).Write("\"");
                    }

                    if (cell.RowSpan > 1)
                    {
                        renderer.Write(" RowSpan=\"").Write(cell.RowSpan.ToString()).Write("\"");
                    }

                    if (table.ColumnDefinitions.Count > 0)
                    {
                        var columnIndex = cell.ColumnIndex < 0 || cell.ColumnIndex >= table.ColumnDefinitions.Count ? index : cell.ColumnIndex;
                        columnIndex = columnIndex >= table.ColumnDefinitions.Count ? table.ColumnDefinitions.Count - 1 : columnIndex;
                        var alignment = table.ColumnDefinitions[columnIndex].Alignment;
                        if (alignment.HasValue)
                        {
                            renderer.Write(" TextAlignment=\"");
                            switch (alignment.Value)
                            {
                                case TableColumnAlign.Center:
                                    renderer.Write("Center");
                                    break;
                                case TableColumnAlign.Right:
                                    renderer.Write("Right");
                                    break;
                                default:
                                    renderer.Write("Left");
                                    break;
                            }

                            renderer.Write("\"");
                        }
                    }

                    renderer.WriteLine(">");
                    renderer.WriteChildren(cell);
                    renderer.WriteLine("</TableCell>");
                }

                renderer.WriteLine("</TableRow>");
            }

            renderer.WriteLine("</TableRowGroup>");
            renderer.WriteLine("</Table>");
        }
    }


}