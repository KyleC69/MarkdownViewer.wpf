// Solution: MarkdownViewer.Wpf
// Project:   MarkdownViewer.Wpf
// File:         TableRenderer.cs
// Author: Kyle L. Crowder
// Build Date: 2026/05/02



using System.Windows;
using System.Windows.Controls;

using Markdig.Extensions.Tables;

using MarkdownViewer.Wpf.Controls;
using MarkdownViewer.Wpf.Core;




namespace MarkdownViewer.Wpf.Rendering.Blocks;





public sealed class TableRenderer : IBlockRenderer<Table>
{
    public UIElement Render(Table table, IRenderContext context)
    {
        ArgumentNullException.ThrowIfNull(table);
        ArgumentNullException.ThrowIfNull(context);

        TableGrid grid = new();

        var columnCount = table.ColumnDefinitions.Count;
        if (columnCount == 0)
        {
            TableRow? firstRow = table.OfType<TableRow>().FirstOrDefault();
            columnCount = firstRow?.Count ?? 0;
        }

        for (var columnIndex = 0; columnIndex < columnCount; columnIndex++) grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

        var rowIndex = 0;
        foreach (TableRow row in table)
        {
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            for (var cellIndex = 0; cellIndex < row.Count; cellIndex++)
            {
                TableCell cell = (TableCell)row[cellIndex];
                TableCellBorder border = new() { BorderThickness = new Thickness(1), BorderBrush = SystemColors.ControlDarkBrush, Padding = new Thickness(8, 4, 8, 4) };
                border.Child = RenderHelpers.RenderChildBlocks(cell, context);
                Grid.SetRow(border, rowIndex);
                Grid.SetColumn(border, Math.Min(cellIndex, Math.Max(0, columnCount - 1)));

                if (cell.ColumnSpan > 1)
                {
                    Grid.SetColumnSpan(border, cell.ColumnSpan);
                }

                if (cell.RowSpan > 1)
                {
                    Grid.SetRowSpan(border, cell.RowSpan);
                }

                grid.Children.Add(border);
            }

            rowIndex++;
        }

        return grid;
    }
}