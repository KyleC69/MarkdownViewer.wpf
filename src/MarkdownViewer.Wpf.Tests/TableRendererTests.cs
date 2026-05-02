using System.Windows;
using System.Windows.Controls;

using Markdig.Extensions.Tables;

using MarkdownViewer.Wpf.Controls;
using MarkdownViewer.Wpf.Core;
using MarkdownViewer.Wpf.Rendering.Blocks;

using Xunit;

namespace MarkdownViewer.Wpf.Tests;

public sealed class TableRendererTests
{
    private readonly TableRenderer renderer = new();
    private readonly RenderContext context = MarkdownTestHelper.CreateContext();

    private const string SimpleTable =
        "| Col1 | Col2 |\n" +
        "|------|------|\n" +
        "| A    | B    |\n" +
        "| C    | D    |";

    [StaFact]
    public void Render_ReturnsTableGrid()
    {
        Table table = MarkdownTestHelper.ParseFirstBlock<Table>(SimpleTable);

        UIElement result = renderer.Render(table, context);

        Assert.IsType<TableGrid>(result);
    }

    [StaFact]
    public void Render_CreatesCorrectColumnCount_ForTwoColumnTable()
    {
        Table table = MarkdownTestHelper.ParseFirstBlock<Table>(SimpleTable);

        TableGrid grid = Assert.IsType<TableGrid>(renderer.Render(table, context));

        Assert.Equal(2, grid.ColumnDefinitions.Count);
    }

    [StaFact]
    public void Render_CreatesCorrectRowCount_ForHeaderPlusDataRows()
    {
        Table table = MarkdownTestHelper.ParseFirstBlock<Table>(SimpleTable);

        TableGrid grid = Assert.IsType<TableGrid>(renderer.Render(table, context));

        // Header row + 2 data rows = 3
        Assert.Equal(3, grid.RowDefinitions.Count);
    }

    [StaFact]
    public void Render_CreatesCellBordersForEachCell()
    {
        Table table = MarkdownTestHelper.ParseFirstBlock<Table>(SimpleTable);

        TableGrid grid = Assert.IsType<TableGrid>(renderer.Render(table, context));

        // 3 rows × 2 columns = 6 cells
        Assert.Equal(6, grid.Children.Count);
        foreach (UIElement child in grid.Children)
        {
            Assert.IsType<TableCellBorder>(child);
        }
    }

    [StaFact]
    public void Render_Throws_WhenTableIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => renderer.Render(null!, context));
    }

    [StaFact]
    public void Render_Throws_WhenContextIsNull()
    {
        Table table = MarkdownTestHelper.ParseFirstBlock<Table>(SimpleTable);

        Assert.Throws<ArgumentNullException>(() => renderer.Render(table, null!));
    }

    [StaTheory]
    [InlineData("| A |\n|---|\n| B |", 1, 2)]    // 1 col, 2 rows
    [InlineData("| A | B | C |\n|---|---|---|\n| D | E | F |", 3, 2)] // 3 cols, 2 rows
    public void Render_CreatesCorrectDimensions_ForVariousTableSizes(string markdown, int expectedColumns, int expectedRows)
    {
        Table table = MarkdownTestHelper.ParseFirstBlock<Table>(markdown);

        TableGrid grid = Assert.IsType<TableGrid>(renderer.Render(table, context));

        Assert.Equal(expectedColumns, grid.ColumnDefinitions.Count);
        Assert.Equal(expectedRows, grid.RowDefinitions.Count);
    }

    [StaFact]
    public void Render_CellBordersHaveCorrectRowAndColumnAssignments()
    {
        const string markdown =
            "| A | B |\n" +
            "|---|---|\n" +
            "| C | D |";
        Table table = MarkdownTestHelper.ParseFirstBlock<Table>(markdown);

        TableGrid grid = Assert.IsType<TableGrid>(renderer.Render(table, context));

        // First cell: row 0, col 0
        UIElement firstCell = grid.Children[0];
        Assert.Equal(0, Grid.GetRow(firstCell));
        Assert.Equal(0, Grid.GetColumn(firstCell));

        // Second cell: row 0, col 1
        UIElement secondCell = grid.Children[1];
        Assert.Equal(0, Grid.GetRow(secondCell));
        Assert.Equal(1, Grid.GetColumn(secondCell));
    }
}
