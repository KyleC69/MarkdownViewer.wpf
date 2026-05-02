using System.Windows;
using System.Windows.Controls;

using Markdig.Syntax;

using MarkdownViewer.Wpf.Controls;
using MarkdownViewer.Wpf.Core;
using MarkdownViewer.Wpf.Rendering.Blocks;

using Xunit;

namespace MarkdownViewer.Wpf.Tests;

public sealed class ParagraphRendererTests
{
    private readonly ParagraphRenderer renderer = new();
    private readonly RenderContext context = MarkdownTestHelper.CreateContext();

    [StaFact]
    public void Render_ReturnsParagraphTextBlock()
    {
        ParagraphBlock block = MarkdownTestHelper.ParseFirstBlock<ParagraphBlock>("Hello world");

        UIElement result = renderer.Render(block, context);

        Assert.IsType<ParagraphTextBlock>(result);
    }

    [StaFact]
    public void Render_IncludesTextContent_InReturnedTextBlock()
    {
        ParagraphBlock block = MarkdownTestHelper.ParseFirstBlock<ParagraphBlock>("Hello world");

        TextBlock result = Assert.IsType<ParagraphTextBlock>(renderer.Render(block, context));

        Assert.Equal("Hello world", MarkdownTestHelper.GetInlineText(result));
    }

    [StaFact]
    public void Render_AppliesTextWrapping_ToParagraph()
    {
        ParagraphBlock block = MarkdownTestHelper.ParseFirstBlock<ParagraphBlock>("Some paragraph");

        TextBlock result = Assert.IsType<ParagraphTextBlock>(renderer.Render(block, context));

        Assert.Equal(TextWrapping.Wrap, result.TextWrapping);
    }

    [StaFact]
    public void Render_AppliesBottomMargin_ToParagraph()
    {
        ParagraphBlock block = MarkdownTestHelper.ParseFirstBlock<ParagraphBlock>("Some paragraph");

        TextBlock result = Assert.IsType<ParagraphTextBlock>(renderer.Render(block, context));

        Assert.Equal(12, result.Margin.Bottom);
    }

    [StaFact]
    public void Render_Throws_WhenBlockIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => renderer.Render(null!, context));
    }

    [StaFact]
    public void Render_Throws_WhenContextIsNull()
    {
        ParagraphBlock block = MarkdownTestHelper.ParseFirstBlock<ParagraphBlock>("text");

        Assert.Throws<ArgumentNullException>(() => renderer.Render(block, null!));
    }

    [StaTheory]
    [InlineData("Simple text")]
    [InlineData("A longer paragraph with more words")]
    [InlineData("A paragraph with **bold** text")]
    public void Render_AlwaysReturnsParagraphTextBlock_ForAnyParagraphContent(string markdown)
    {
        ParagraphBlock block = MarkdownTestHelper.ParseFirstBlock<ParagraphBlock>(markdown);

        UIElement result = renderer.Render(block, context);

        Assert.IsType<ParagraphTextBlock>(result);
    }
}
