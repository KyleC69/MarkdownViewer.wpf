using System.Windows;
using System.Windows.Controls;

using Markdig.Syntax;

using MarkdownViewer.Wpf.Controls;
using MarkdownViewer.Wpf.Core;
using MarkdownViewer.Wpf.Rendering.Blocks;

using Xunit;

namespace MarkdownViewer.Wpf.Tests;

public sealed class HeadingRendererTests
{
    private readonly HeadingRenderer renderer = new();
    private readonly RenderContext context = MarkdownTestHelper.CreateContext();

    [StaFact]
    public void Render_ReturnsHeading1TextBlock_ForLevelOneHeading()
    {
        HeadingBlock block = MarkdownTestHelper.ParseFirstBlock<HeadingBlock>("# Hello");

        UIElement result = renderer.Render(block, context);

        Assert.IsType<Heading1TextBlock>(result);
    }

    [StaFact]
    public void Render_ReturnsHeading2TextBlock_ForLevelTwoHeading()
    {
        HeadingBlock block = MarkdownTestHelper.ParseFirstBlock<HeadingBlock>("## Hello");

        UIElement result = renderer.Render(block, context);

        Assert.IsType<Heading2TextBlock>(result);
    }

    [StaFact]
    public void Render_ReturnsHeading3TextBlock_ForLevelThreeHeading()
    {
        HeadingBlock block = MarkdownTestHelper.ParseFirstBlock<HeadingBlock>("### Hello");

        UIElement result = renderer.Render(block, context);

        Assert.IsType<Heading3TextBlock>(result);
    }

    [StaFact]
    public void Render_ReturnsHeading4TextBlock_ForLevelFourHeading()
    {
        HeadingBlock block = MarkdownTestHelper.ParseFirstBlock<HeadingBlock>("#### Hello");

        UIElement result = renderer.Render(block, context);

        Assert.IsType<Heading4TextBlock>(result);
    }

    [StaFact]
    public void Render_ReturnsHeading5TextBlock_ForLevelFiveHeading()
    {
        HeadingBlock block = MarkdownTestHelper.ParseFirstBlock<HeadingBlock>("##### Hello");

        UIElement result = renderer.Render(block, context);

        Assert.IsType<Heading5TextBlock>(result);
    }

    [StaFact]
    public void Render_ReturnsHeading6TextBlock_ForLevelSixHeading()
    {
        HeadingBlock block = MarkdownTestHelper.ParseFirstBlock<HeadingBlock>("###### Hello");

        UIElement result = renderer.Render(block, context);

        Assert.IsType<Heading6TextBlock>(result);
    }

    [StaFact]
    public void Render_AppliesTextWrapping_ForAllHeadingLevels()
    {
        HeadingBlock block = MarkdownTestHelper.ParseFirstBlock<HeadingBlock>("# Heading");

        TextBlock result = Assert.IsAssignableFrom<TextBlock>(renderer.Render(block, context));

        Assert.Equal(TextWrapping.Wrap, result.TextWrapping);
    }

    [StaFact]
    public void Render_IncludesHeadingText_InReturnedTextBlock()
    {
        HeadingBlock block = MarkdownTestHelper.ParseFirstBlock<HeadingBlock>("## My Section");

        TextBlock result = Assert.IsAssignableFrom<TextBlock>(renderer.Render(block, context));

        Assert.Equal("My Section", MarkdownTestHelper.GetInlineText(result));
    }

    [StaFact]
    public void Render_Throws_WhenBlockIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => renderer.Render(null!, context));
    }

    [StaFact]
    public void Render_Throws_WhenContextIsNull()
    {
        HeadingBlock block = MarkdownTestHelper.ParseFirstBlock<HeadingBlock>("# Title");

        Assert.Throws<ArgumentNullException>(() => renderer.Render(block, null!));
    }

    [StaTheory]
    [InlineData("# H1", 30d)]
    [InlineData("## H2", 24d)]
    [InlineData("### H3", 20d)]
    [InlineData("#### H4", 18d)]
    [InlineData("##### H5", 16d)]
    [InlineData("###### H6", 14d)]
    public void Render_AppliesCorrectFontSize_ForEachHeadingLevel(string markdown, double expectedFontSize)
    {
        HeadingBlock block = MarkdownTestHelper.ParseFirstBlock<HeadingBlock>(markdown);

        TextBlock result = Assert.IsAssignableFrom<TextBlock>(renderer.Render(block, context));

        Assert.Equal(expectedFontSize, result.FontSize);
    }

    [StaTheory]
    [InlineData("# H1")]
    [InlineData("## H2")]
    [InlineData("### H3")]
    [InlineData("#### H4")]
    [InlineData("##### H5")]
    [InlineData("###### H6")]
    public void Render_AlwaysProducesTextBlock_ForAnyHeadingLevel(string markdown)
    {
        HeadingBlock block = MarkdownTestHelper.ParseFirstBlock<HeadingBlock>(markdown);

        UIElement result = renderer.Render(block, context);

        Assert.IsAssignableFrom<TextBlock>(result);
    }
}
