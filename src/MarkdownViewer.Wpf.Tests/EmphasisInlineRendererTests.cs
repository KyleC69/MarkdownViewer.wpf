using System.Windows;
using System.Windows.Documents;

using Markdig.Syntax.Inlines;

using MarkdownViewer.Wpf.Controls;
using MarkdownViewer.Wpf.Core;
using MarkdownViewer.Wpf.Rendering.Inlines;

using Xunit;

namespace MarkdownViewer.Wpf.Tests;

public sealed class EmphasisInlineRendererTests
{
    private readonly EmphasisInlineRenderer renderer = new();
    private readonly RenderContext context = MarkdownTestHelper.CreateContext();

    [StaFact]
    public void Render_ReturnsBold_ForDoubleAsterisk()
    {
        EmphasisInline inline = MarkdownTestHelper.FindFirstInline<EmphasisInline>("**bold text**");

        System.Windows.Documents.Inline result = renderer.Render(inline, context);

        Assert.IsType<Bold>(result);
    }

    [StaFact]
    public void Render_ReturnsBold_ForDoubleUnderscore()
    {
        EmphasisInline inline = MarkdownTestHelper.FindFirstInline<EmphasisInline>("__bold text__");

        System.Windows.Documents.Inline result = renderer.Render(inline, context);

        Assert.IsType<Bold>(result);
    }

    [StaFact]
    public void Render_ReturnsItalic_ForSingleAsterisk()
    {
        EmphasisInline inline = MarkdownTestHelper.FindFirstInline<EmphasisInline>("*italic text*");

        System.Windows.Documents.Inline result = renderer.Render(inline, context);

        Assert.IsType<Italic>(result);
    }

    [StaFact]
    public void Render_ReturnsItalic_ForSingleUnderscore()
    {
        EmphasisInline inline = MarkdownTestHelper.FindFirstInline<EmphasisInline>("_italic text_");

        System.Windows.Documents.Inline result = renderer.Render(inline, context);

        Assert.IsType<Italic>(result);
    }

    [StaFact]
    public void Render_ReturnsStrikeThroughSpan_ForDoubleTilde()
    {
        EmphasisInline inline = MarkdownTestHelper.FindFirstInline<EmphasisInline>("~~strikethrough~~");

        System.Windows.Documents.Inline result = renderer.Render(inline, context);

        StrikeThroughSpan span = Assert.IsType<StrikeThroughSpan>(result);
        Assert.Equal(TextDecorations.Strikethrough, span.TextDecorations);
    }

    [StaFact]
    public void Render_ReturnsSubscriptSpan_ForSingleTilde()
    {
        EmphasisInline inline = MarkdownTestHelper.FindFirstInline<EmphasisInline>("~sub~");

        System.Windows.Documents.Inline result = renderer.Render(inline, context);

        Assert.IsType<SubscriptSpan>(result);
    }

    [StaFact]
    public void Render_ReturnsSuperscriptSpan_ForCaret()
    {
        EmphasisInline inline = MarkdownTestHelper.FindFirstInline<EmphasisInline>("^sup^");

        System.Windows.Documents.Inline result = renderer.Render(inline, context);

        Assert.IsType<SuperscriptSpan>(result);
    }

    [StaFact]
    public void Render_ReturnsInsertedSpan_ForDoublePlus()
    {
        EmphasisInline inline = MarkdownTestHelper.FindFirstInline<EmphasisInline>("++inserted++");

        System.Windows.Documents.Inline result = renderer.Render(inline, context);

        InsertedSpan span = Assert.IsType<InsertedSpan>(result);
        Assert.Equal(TextDecorations.Underline, span.TextDecorations);
    }

    [StaFact]
    public void Render_ReturnsMarkedSpan_ForDoubleEquals()
    {
        EmphasisInline inline = MarkdownTestHelper.FindFirstInline<EmphasisInline>("==marked==");

        System.Windows.Documents.Inline result = renderer.Render(inline, context);

        Assert.IsType<MarkedSpan>(result);
    }

    [StaFact]
    public void Render_ContainsChildInlines_FromEmphasisContent()
    {
        EmphasisInline inline = MarkdownTestHelper.FindFirstInline<EmphasisInline>("**hello world**");

        Span result = Assert.IsAssignableFrom<Span>(renderer.Render(inline, context));

        Assert.NotEmpty(result.Inlines);
    }

    [StaFact]
    public void Render_Throws_WhenInlineIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => renderer.Render(null!, context));
    }

    [StaFact]
    public void Render_Throws_WhenContextIsNull()
    {
        EmphasisInline inline = MarkdownTestHelper.FindFirstInline<EmphasisInline>("**text**");

        Assert.Throws<ArgumentNullException>(() => renderer.Render(inline, null!));
    }

    [StaTheory]
    [InlineData("**bold**")]
    [InlineData("*italic*")]
    [InlineData("~~strikethrough~~")]
    [InlineData("~subscript~")]
    [InlineData("^superscript^")]
    [InlineData("++inserted++")]
    [InlineData("==marked==")]
    public void Render_AlwaysReturnsSpan_ForAnyEmphasisType(string markdown)
    {
        EmphasisInline inline = MarkdownTestHelper.FindFirstInline<EmphasisInline>(markdown);

        System.Windows.Documents.Inline result = renderer.Render(inline, context);

        Assert.IsAssignableFrom<Span>(result);
    }
}
