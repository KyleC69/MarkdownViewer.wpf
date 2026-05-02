using System.Windows;
using System.Windows.Controls;

using Markdig.Syntax;

using MarkdownViewer.Wpf.Controls;
using MarkdownViewer.Wpf.Core;
using MarkdownViewer.Wpf.Rendering.Blocks;

using Xunit;

namespace MarkdownViewer.Wpf.Tests;

public sealed class BlockQuoteRendererTests
{
    private readonly BlockQuoteRenderer renderer = new();
    private readonly RenderContext context = MarkdownTestHelper.CreateContext();

    [StaFact]
    public void Render_ReturnsBlockQuoteBorder()
    {
        QuoteBlock block = MarkdownTestHelper.ParseFirstBlock<QuoteBlock>("> quoted text");

        UIElement result = renderer.Render(block, context);

        Assert.IsType<BlockQuoteBorder>(result);
    }

    [StaFact]
    public void Render_AppliesLeftBorderThickness_ToBlockQuote()
    {
        QuoteBlock block = MarkdownTestHelper.ParseFirstBlock<QuoteBlock>("> quoted text");

        BlockQuoteBorder border = Assert.IsType<BlockQuoteBorder>(renderer.Render(block, context));

        Assert.Equal(new Thickness(4, 0, 0, 0), border.BorderThickness);
    }

    [StaFact]
    public void Render_ContainsChildStackPanel_WithNestedContent()
    {
        QuoteBlock block = MarkdownTestHelper.ParseFirstBlock<QuoteBlock>("> quoted text");

        BlockQuoteBorder border = Assert.IsType<BlockQuoteBorder>(renderer.Render(block, context));

        Assert.IsType<StackPanel>(border.Child);
    }

    [StaFact]
    public void Render_NestedStackPanel_ContainsRenderedParagraph()
    {
        QuoteBlock block = MarkdownTestHelper.ParseFirstBlock<QuoteBlock>("> quoted text");

        BlockQuoteBorder border = Assert.IsType<BlockQuoteBorder>(renderer.Render(block, context));
        StackPanel stackPanel = Assert.IsType<StackPanel>(border.Child);

        Assert.Single(stackPanel.Children);
        Assert.IsType<ParagraphTextBlock>(stackPanel.Children[0]);
    }

    [StaFact]
    public void Render_AppliesBottomMargin_ToBlockQuote()
    {
        QuoteBlock block = MarkdownTestHelper.ParseFirstBlock<QuoteBlock>("> text");

        BlockQuoteBorder border = Assert.IsType<BlockQuoteBorder>(renderer.Render(block, context));

        Assert.Equal(12, border.Margin.Bottom);
    }

    [StaFact]
    public void Render_Throws_WhenBlockIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => renderer.Render(null!, context));
    }

    [StaFact]
    public void Render_Throws_WhenContextIsNull()
    {
        QuoteBlock block = MarkdownTestHelper.ParseFirstBlock<QuoteBlock>("> text");

        Assert.Throws<ArgumentNullException>(() => renderer.Render(block, null!));
    }

    [StaTheory]
    [InlineData("> single line")]
    [InlineData("> line one\n> line two")]
    public void Render_AlwaysReturnsBlockQuoteBorder_ForAnyQuoteContent(string markdown)
    {
        QuoteBlock block = MarkdownTestHelper.ParseFirstBlock<QuoteBlock>(markdown);

        UIElement result = renderer.Render(block, context);

        Assert.IsType<BlockQuoteBorder>(result);
    }
}
