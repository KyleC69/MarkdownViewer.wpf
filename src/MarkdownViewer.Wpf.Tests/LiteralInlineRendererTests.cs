using System.Windows.Documents;

using Markdig.Syntax.Inlines;

using MarkdownViewer.Wpf.Core;
using MarkdownViewer.Wpf.Rendering.Inlines;

using Xunit;

namespace MarkdownViewer.Wpf.Tests;

public sealed class LiteralInlineRendererTests
{
    private readonly LiteralInlineRenderer renderer = new();
    private readonly RenderContext context = MarkdownTestHelper.CreateContext();

    [Fact]
    public void Render_ReturnsRun_WithCorrectText()
    {
        LiteralInline inline = MarkdownTestHelper.FindFirstInline<LiteralInline>("Hello world");

        System.Windows.Documents.Inline result = renderer.Render(inline, context);

        Run run = Assert.IsType<Run>(result);
        Assert.Equal("Hello world", run.Text);
    }

    [Fact]
    public void Render_ReturnsRun_WithEmptyText_ForEmptyLiteral()
    {
        // Empty literal - a paragraph with only whitespace collapses, so test via direct instantiation
        LiteralInline inline = new(string.Empty);

        System.Windows.Documents.Inline result = renderer.Render(inline, context);

        Run run = Assert.IsType<Run>(result);
        Assert.Equal(string.Empty, run.Text);
    }

    [Fact]
    public void Render_Throws_WhenInlineIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => renderer.Render(null!, context));
    }

    [Fact]
    public void Render_Throws_WhenContextIsNull()
    {
        LiteralInline inline = new("text");

        Assert.Throws<ArgumentNullException>(() => renderer.Render(inline, null!));
    }

    [Theory]
    [InlineData("Hello")]
    [InlineData("A longer sentence with spaces")]
    [InlineData("Text with punctuation, and more!")]
    public void Render_AlwaysReturnsRun_PreservingOriginalText(string text)
    {
        LiteralInline inline = new(text);

        System.Windows.Documents.Inline result = renderer.Render(inline, context);

        Run run = Assert.IsType<Run>(result);
        Assert.Equal(text, run.Text);
    }
}
