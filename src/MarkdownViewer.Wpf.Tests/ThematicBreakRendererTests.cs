// Solution: MarkdownViewer.Wpf
// Project:   MarkdownViewer.Wpf.Tests
// File:         ThematicBreakRendererTests.cs
// Author: Kyle L. Crowder
// Build Date: 2026/05/02



using System.Windows;

using Markdig.Syntax;

using MarkdownViewer.Wpf.Controls;
using MarkdownViewer.Wpf.Core;
using MarkdownViewer.Wpf.Rendering.Blocks;

using Xunit;




namespace MarkdownViewer.Wpf.Tests;





public sealed class ThematicBreakRendererTests
{
    private readonly RenderContext context = MarkdownTestHelper.CreateContext();
    private readonly ThematicBreakRenderer renderer = new();








    [StaTheory]
    [InlineData("---")]
    [InlineData("***")]
    [InlineData("___")]
    public void Render_AlwaysReturnsThematicBreakBorder_ForAnyBreakSyntax(string markdown)
    {
        ThematicBreakBlock block = MarkdownTestHelper.ParseFirstBlock<ThematicBreakBlock>(markdown);

        UIElement result = renderer.Render(block, context);

        Assert.IsType<ThematicBreakBorder>(result);
    }








    [StaFact]
    public void Render_AppliesHeightOfOne()
    {
        ThematicBreakBlock block = MarkdownTestHelper.ParseFirstBlock<ThematicBreakBlock>("---");

        ThematicBreakBorder border = Assert.IsType<ThematicBreakBorder>(renderer.Render(block, context));

        Assert.Equal(1, border.Height);
    }








    [StaFact]
    public void Render_AppliesTopAndBottomMargin()
    {
        ThematicBreakBlock block = MarkdownTestHelper.ParseFirstBlock<ThematicBreakBlock>("---");

        ThematicBreakBorder border = Assert.IsType<ThematicBreakBorder>(renderer.Render(block, context));

        Assert.Equal(4, border.Margin.Top);
        Assert.Equal(16, border.Margin.Bottom);
    }








    [StaFact]
    public void Render_ReturnsThematicBreakBorder()
    {
        ThematicBreakBlock block = MarkdownTestHelper.ParseFirstBlock<ThematicBreakBlock>("---");

        UIElement result = renderer.Render(block, context);

        Assert.IsType<ThematicBreakBorder>(result);
    }








    [StaFact]
    public void Render_StretchesHorizontalAlignment()
    {
        ThematicBreakBlock block = MarkdownTestHelper.ParseFirstBlock<ThematicBreakBlock>("---");

        ThematicBreakBorder border = Assert.IsType<ThematicBreakBorder>(renderer.Render(block, context));

        Assert.Equal(HorizontalAlignment.Stretch, border.HorizontalAlignment);
    }








    [StaFact]
    public void Render_Throws_WhenBlockIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => renderer.Render(null!, context));
    }








    [StaFact]
    public void Render_Throws_WhenContextIsNull()
    {
        ThematicBreakBlock block = MarkdownTestHelper.ParseFirstBlock<ThematicBreakBlock>("---");

        Assert.Throws<ArgumentNullException>(() => renderer.Render(block, null!));
    }
}