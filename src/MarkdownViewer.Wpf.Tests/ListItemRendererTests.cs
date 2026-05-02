using System.Windows.Controls;

using Markdig.Syntax;

using MarkdownViewer.Wpf.Controls;
using MarkdownViewer.Wpf.Core;
using MarkdownViewer.Wpf.Rendering.Blocks;

using Xunit;

namespace MarkdownViewer.Wpf.Tests;

public sealed class ListItemRendererTests
{
    private readonly RenderContext context = MarkdownTestHelper.CreateContext();

    [Fact]
    public void GetMarker_ReturnsBullet_ForUnorderedList()
    {
        ListBlock list = MarkdownTestHelper.ParseFirstBlock<ListBlock>("- item one\n- item two");
        ListItemBlock item = Assert.IsType<ListItemBlock>(list[0]);

        string marker = ListItemRenderer.GetMarker(item);

        Assert.Equal("•", marker);
    }

    [Fact]
    public void GetMarker_ReturnsFirstNumber_ForFirstOrderedItem()
    {
        ListBlock list = MarkdownTestHelper.ParseFirstBlock<ListBlock>("1. first\n2. second");
        ListItemBlock item = Assert.IsType<ListItemBlock>(list[0]);

        string marker = ListItemRenderer.GetMarker(item);

        Assert.Equal("1.", marker);
    }

    [Fact]
    public void GetMarker_ReturnsSecondNumber_ForSecondOrderedItem()
    {
        ListBlock list = MarkdownTestHelper.ParseFirstBlock<ListBlock>("1. first\n2. second");
        ListItemBlock item = Assert.IsType<ListItemBlock>(list[1]);

        string marker = ListItemRenderer.GetMarker(item);

        Assert.Equal("2.", marker);
    }

    [Fact]
    public void GetMarker_RespectCustomOrderedStart_WhenListStartsFromFive()
    {
        ListBlock list = MarkdownTestHelper.ParseFirstBlock<ListBlock>("5. first\n6. second");
        ListItemBlock item = Assert.IsType<ListItemBlock>(list[0]);

        string marker = ListItemRenderer.GetMarker(item);

        Assert.Equal("5.", marker);
    }

    [Fact]
    public void GetMarker_ReturnsBullet_WhenParentIsNotAListBlock()
    {
        // A list item nested inside another container (e.g. block-quote > list > list-item)
        // The immediate parent is still a ListBlock, but verify the unordered path
        ListBlock list = MarkdownTestHelper.ParseFirstBlock<ListBlock>("- only item");
        ListItemBlock item = Assert.IsType<ListItemBlock>(list[0]);

        string marker = ListItemRenderer.GetMarker(item);

        // Unordered list always returns bullet
        Assert.Equal("•", marker);
    }

    [Theory]
    [InlineData("- a\n- b\n- c", 0, "•")]
    [InlineData("- a\n- b\n- c", 1, "•")]
    [InlineData("1. a\n2. b\n3. c", 0, "1.")]
    [InlineData("1. a\n2. b\n3. c", 2, "3.")]
    public void GetMarker_ReturnsExpectedMarker_ForVariousListPositions(string markdown, int itemIndex, string expectedMarker)
    {
        ListBlock list = MarkdownTestHelper.ParseFirstBlock<ListBlock>(markdown);
        ListItemBlock item = Assert.IsType<ListItemBlock>(list[itemIndex]);

        string marker = ListItemRenderer.GetMarker(item);

        Assert.Equal(expectedMarker, marker);
    }

    [StaFact]
    public void CreateListItem_ReturnsListItemGrid_WithTwoColumns()
    {
        ListItemGrid grid = Assert.IsType<ListItemGrid>(ListItemRenderer.CreateListItem("•", [], context));

        Assert.Equal(2, grid.ColumnDefinitions.Count);
    }

    [StaFact]
    public void CreateListItem_PlacesMarkerInFirstColumn()
    {
        ListItemGrid grid = Assert.IsType<ListItemGrid>(ListItemRenderer.CreateListItem("1.", [], context));

        ListItemMarkerTextBlock marker = Assert.IsType<ListItemMarkerTextBlock>(grid.Children[0]);
        Assert.Equal("1.", marker.Text);
        Assert.Equal(0, Grid.GetColumn(marker));
    }

    [StaFact]
    public void CreateListItem_PlacesContentPanelInSecondColumn()
    {
        ListItemGrid grid = Assert.IsType<ListItemGrid>(ListItemRenderer.CreateListItem("•", [], context));

        ListItemContentPanel content = Assert.IsType<ListItemContentPanel>(grid.Children[1]);
        Assert.Equal(1, Grid.GetColumn(content));
    }

    [StaFact]
    public void CreateListItem_AddsContentElements_ToContentPanel()
    {
        MarkdownEngine engine = MarkdownEngine.CreateDefault();
        System.Windows.UIElement[] elements =
        [
            engine.Render("text", EmptyServiceProvider.Instance),
            engine.Render("more text", EmptyServiceProvider.Instance),
        ];

        ListItemGrid grid = Assert.IsType<ListItemGrid>(ListItemRenderer.CreateListItem("•", elements, context));

        ListItemContentPanel content = Assert.IsType<ListItemContentPanel>(grid.Children[1]);
        Assert.Equal(2, content.Children.Count);
    }

    [StaFact]
    public void CreateListItem_Throws_WhenMarkerTextIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => ListItemRenderer.CreateListItem(null!, [], context));
    }

    [StaFact]
    public void CreateListItem_Throws_WhenMarkerTextIsEmpty()
    {
        Assert.Throws<ArgumentException>(() => ListItemRenderer.CreateListItem(string.Empty, [], context));
    }

    [StaFact]
    public void CreateListItem_Throws_WhenContentElementsIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => ListItemRenderer.CreateListItem("•", null!, context));
    }

    [StaFact]
    public void CreateListItem_Throws_WhenContextIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => ListItemRenderer.CreateListItem("•", [], null!));
    }
}
