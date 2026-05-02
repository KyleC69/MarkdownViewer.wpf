// Solution: MarkdownViewer.Wpf
// Project:   MarkdownViewer.Wpf.Tests
// File:         CodeInlineRendererTests.cs
// Author: Kyle L. Crowder
// Build Date: 2026/05/02



using System.Windows.Documents;

using Markdig.Syntax.Inlines;

using MarkdownViewer.Wpf.Controls;
using MarkdownViewer.Wpf.Core;
using MarkdownViewer.Wpf.Rendering.Inlines;

using Xunit;




namespace MarkdownViewer.Wpf.Tests;





public sealed class CodeInlineRendererTests
{
    private readonly RenderContext context = MarkdownTestHelper.CreateContext();
    private readonly CodeInlineRenderer renderer = new();








    [Fact]
    public void Render_AppliesMonospaceFont_ToInnerRun()
    {
        CodeInline inline = MarkdownTestHelper.FindFirstInline<CodeInline>("Call `method()` now");

        CodeInlineSpan span = Assert.IsType<CodeInlineSpan>(renderer.Render(inline, context));

        Run run = Assert.IsType<Run>(span.Inlines.FirstInline);
        Assert.NotNull(run.FontFamily);
        Assert.Contains("Consolas", run.FontFamily.Source);
    }








    [Fact]
    public void Render_ContainsRunWithCorrectText()
    {
        CodeInline inline = MarkdownTestHelper.FindFirstInline<CodeInline>("Use `var x = 1;` here");

        CodeInlineSpan span = Assert.IsType<CodeInlineSpan>(renderer.Render(inline, context));

        Run run = Assert.IsType<Run>(span.Inlines.FirstInline);
        Assert.Equal("var x = 1;", run.Text);
    }








    [Theory]
    [InlineData("Use `x` in code", "x")]
    [InlineData("Call `myMethod()` please", "myMethod()")]
    [InlineData("Type `System.Console` here", "System.Console")]
    public void Render_PreservesCodeContent_ForVariousInputs(string markdown, string expectedCode)
    {
        CodeInline inline = MarkdownTestHelper.FindFirstInline<CodeInline>(markdown);

        CodeInlineSpan span = Assert.IsType<CodeInlineSpan>(renderer.Render(inline, context));

        Run run = Assert.IsType<Run>(span.Inlines.FirstInline);
        Assert.Equal(expectedCode, run.Text);
    }








    [Fact]
    public void Render_ReturnsCodeInlineSpan()
    {
        CodeInline inline = MarkdownTestHelper.FindFirstInline<CodeInline>("Use `var x = 1;` here");

        System.Windows.Documents.Inline result = renderer.Render(inline, context);

        Assert.IsType<CodeInlineSpan>(result);
    }








    [Fact]
    public void Render_Throws_WhenContextIsNull()
    {
        CodeInline inline = MarkdownTestHelper.FindFirstInline<CodeInline>("Use `code` here");

        Assert.Throws<ArgumentNullException>(() => renderer.Render(inline, null!));
    }








    [Fact]
    public void Render_Throws_WhenInlineIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => renderer.Render(null!, context));
    }
}