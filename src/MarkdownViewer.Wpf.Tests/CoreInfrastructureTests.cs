using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

using Markdig.Syntax;
using Markdig.Syntax.Inlines;

using MarkdownViewer.Wpf.Core;
using MarkdownViewer.Wpf.Diagnostics;
using MarkdownViewer.Wpf.Theming;

using Xunit;

namespace MarkdownViewer.Wpf.Tests;

public sealed class CoreInfrastructureTests
{
    [Fact]
    public void EmptyServiceProvider_GetService_ReturnsNull()
    {
        Assert.Null(EmptyServiceProvider.Instance.GetService(typeof(string)));
    }

    [StaFact]
    public void MarkdownRendererBuilder_AddBlockRenderer_InstanceRendererIsUsed()
    {
        TestParagraphRenderer renderer = new(() => new Border { Tag = "instance" });
        MarkdownRendererBuilder builder = new MarkdownRendererBuilder()
            .AddBlockRenderer<ParagraphBlock>(renderer);
        RendererDispatcher dispatcher = builder.BuildDispatcher();
        ParagraphBlock block = MarkdownTestHelper.ParseFirstBlock<ParagraphBlock>("paragraph");

        Border result = Assert.IsType<Border>(dispatcher.RenderBlock(block, MarkdownTestHelper.CreateContext(dispatcher)));

        Assert.Equal("instance", result.Tag);
    }

    [StaFact]
    public void MarkdownRendererBuilder_AddBlockRenderer_GenericRendererIsUsed()
    {
        MarkdownRendererBuilder builder = new MarkdownRendererBuilder()
            .AddBlockRenderer<ParagraphBlock, ParameterlessParagraphRenderer>();
        RendererDispatcher dispatcher = builder.BuildDispatcher();
        ParagraphBlock block = MarkdownTestHelper.ParseFirstBlock<ParagraphBlock>("paragraph");

        Border result = Assert.IsType<Border>(dispatcher.RenderBlock(block, MarkdownTestHelper.CreateContext(dispatcher)));

        Assert.Equal("generic", result.Tag);
    }

    [StaFact]
    public void MarkdownRendererBuilder_AddInlineRenderer_InstanceRendererIsUsed()
    {
        TestLiteralRenderer renderer = new("instance");
        MarkdownRendererBuilder builder = new MarkdownRendererBuilder()
            .AddInlineRenderer<LiteralInline>(renderer);
        RendererDispatcher dispatcher = builder.BuildDispatcher();
        LiteralInline inline = MarkdownTestHelper.FindFirstInline<LiteralInline>("paragraph");

        Run result = Assert.IsType<Run>(dispatcher.RenderInline(inline, MarkdownTestHelper.CreateContext(dispatcher)));

        Assert.Equal("instance", result.Text);
    }

    [StaFact]
    public void MarkdownRendererBuilder_AddInlineRenderer_GenericRendererIsUsed()
    {
        MarkdownRendererBuilder builder = new MarkdownRendererBuilder()
            .AddInlineRenderer<LiteralInline, ParameterlessLiteralRenderer>();
        RendererDispatcher dispatcher = builder.BuildDispatcher();
        LiteralInline inline = MarkdownTestHelper.FindFirstInline<LiteralInline>("paragraph");

        Run result = Assert.IsType<Run>(dispatcher.RenderInline(inline, MarkdownTestHelper.CreateContext(dispatcher)));

        Assert.Equal("generic", result.Text);
    }

    [Fact]
    public void MarkdownRendererBuilder_AddPostProcessor_BuildPostProcessors_PreservesOrder()
    {
        RecordingPostProcessor first = new("first");
        RecordingPostProcessor second = new("second");
        MarkdownRendererBuilder builder = new MarkdownRendererBuilder()
            .AddPostProcessor(first)
            .AddPostProcessor(second);

        IReadOnlyList<IPostProcessor> processors = builder.BuildPostProcessors();

        Assert.Collection(
            processors,
            item => Assert.Same(first, item),
            item => Assert.Same(second, item));
    }

    [StaFact]
    public void RenderContext_AddPostProcessor_AppendsProcessor()
    {
        RenderContext context = MarkdownTestHelper.CreateContext();
        RecordingPostProcessor processor = new("added");

        context.AddPostProcessor(processor);

        Assert.Same(processor, Assert.Single(context.PostProcessors));
    }

    [StaFact]
    public void RenderContext_RenderBlock_DelegatesToDispatcher()
    {
        RendererDispatcher dispatcher = new([new TestParagraphRenderer(() => new Border { Tag = "block" })], []);
        RenderContext context = MarkdownTestHelper.CreateContext(dispatcher);
        ParagraphBlock block = MarkdownTestHelper.ParseFirstBlock<ParagraphBlock>("paragraph");

        Border result = Assert.IsType<Border>(context.RenderBlock(block));

        Assert.Equal("block", result.Tag);
    }

    [StaFact]
    public void RenderContext_RenderInline_DelegatesToDispatcher()
    {
        RendererDispatcher dispatcher = new([], [new TestLiteralRenderer("inline")]);
        RenderContext context = MarkdownTestHelper.CreateContext(dispatcher);
        LiteralInline inline = MarkdownTestHelper.FindFirstInline<LiteralInline>("paragraph");

        Run result = Assert.IsType<Run>(context.RenderInline(inline));

        Assert.Equal("inline", result.Text);
    }

    [Fact]
    public void RendererDispatcher_RenderInline_Throws_WhenRendererIsMissing()
    {
        RendererDispatcher dispatcher = new([], []);
        RenderContext context = MarkdownTestHelper.CreateContext(dispatcher);
        LiteralInline inline = MarkdownTestHelper.FindFirstInline<LiteralInline>("paragraph");

        RendererNotFoundException exception = Assert.Throws<RendererNotFoundException>(() => dispatcher.RenderInline(inline, context));

        Assert.Equal(typeof(LiteralInline), exception.MarkdownNodeType);
    }

    [Fact]
    public void RendererDispatcher_IndexRenderers_Throws_WhenRendererDoesNotImplementExpectedInterface()
    {
        InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() => RendererDispatcher.IndexRenderers([new object()], typeof(IBlockRenderer<>)));

        Assert.Contains("IBlockRenderer", exception.Message);
    }

    [StaFact]
    public void RenderHelpers_AppendInlines_RendersContainerChildren()
    {
        ParagraphBlock block = MarkdownTestHelper.ParseFirstBlock<ParagraphBlock>("**bold** text");
        TextBlock textBlock = new();

        RenderHelpers.AppendInlines(textBlock.Inlines, block.Inline, MarkdownTestHelper.CreateContext());

        Assert.Equal(2, textBlock.Inlines.Count);
        Assert.IsType<Bold>(textBlock.Inlines.FirstInline);
    }

    [StaFact]
    public void RenderHelpers_AppendInlines_IgnoresNullContainer()
    {
        TextBlock textBlock = new();

        RenderHelpers.AppendInlines(textBlock.Inlines, null, MarkdownTestHelper.CreateContext());

        Assert.Empty(textBlock.Inlines);
    }

    [StaFact]
    public void RenderHelpers_RenderChildBlocks_RendersChildrenAndAppliesStyle()
    {
        Style panelStyle = new(typeof(StackPanel));
        ResourceDictionaryTheme theme = MarkdownTestHelper.CreateTheme(("panelStyle", panelStyle));
        QuoteBlock quoteBlock = MarkdownTestHelper.ParseFirstBlock<QuoteBlock>("> one\n>\n> two");

        StackPanel result = RenderHelpers.RenderChildBlocks(quoteBlock, MarkdownTestHelper.CreateContext(theme: theme), "panelStyle");

        Assert.Same(panelStyle, result.Style);
        Assert.Equal(2, result.Children.Count);
    }

    [StaFact]
    public void RenderHelpers_CreateTextBlock_WrapsAndAppliesStyle()
    {
        Style style = new(typeof(TextBlock));
        ResourceDictionaryTheme theme = MarkdownTestHelper.CreateTheme((ThemeKeys.ParagraphStyle, style));

        TextBlock result = RenderHelpers.CreateTextBlock(MarkdownTestHelper.CreateContext(theme: theme), ThemeKeys.ParagraphStyle);

        Assert.Equal(TextWrapping.Wrap, result.TextWrapping);
        Assert.Same(style, result.Style);
    }

    [StaFact]
    public void RenderHelpers_TryApplyStyle_FrameworkElement_SetsStyleWhenFound()
    {
        Style style = new(typeof(Border));
        Border border = new();

        RenderHelpers.TryApplyStyle(border, MarkdownTestHelper.CreateTheme(("style", style)), "style");

        Assert.Same(style, border.Style);
    }

    [StaFact]
    public void RenderHelpers_TryApplyStyle_TextElement_SetsStyleWhenFound()
    {
        Style style = new(typeof(Span));
        Span span = new();

        RenderHelpers.TryApplyStyle(span, MarkdownTestHelper.CreateTheme(("style", style)), "style");

        Assert.Same(style, span.Style);
    }

    [StaFact]
    public void RenderHelpers_GetLiteral_JoinsMarkdownLines()
    {
        FencedCodeBlock block = MarkdownTestHelper.FindFirstBlock<FencedCodeBlock>("```txt\nline1\nline2\n```");

        Assert.Equal($"line1{Environment.NewLine}line2", RenderHelpers.GetLiteral(block));
    }

    [StaFact]
    public void RenderHelpers_CreateDebugLabel_ReturnsConfiguredVisual()
    {
        Style borderStyle = new(typeof(Border));
        Style textStyle = new(typeof(TextBlock));
        ResourceDictionaryTheme theme = MarkdownTestHelper.CreateTheme(
            (ThemeKeys.DiagnosticsLabelBorderStyle, borderStyle),
            (ThemeKeys.DiagnosticsLabelTextStyle, textStyle));

        Border border = RenderHelpers.CreateDebugLabel("debug", theme);
        TextBlock text = Assert.IsType<TextBlock>(border.Child);

        Assert.Equal("debug", text.Text);
        Assert.Same(borderStyle, border.Style);
        Assert.Same(textStyle, text.Style);
    }

    public sealed class TestParagraphRenderer(Func<UIElement> createElement) : IBlockRenderer<ParagraphBlock>
    {
        public UIElement Render(ParagraphBlock block, IRenderContext context) => createElement();
    }

    public sealed class ParameterlessParagraphRenderer : IBlockRenderer<ParagraphBlock>
    {
        public UIElement Render(ParagraphBlock block, IRenderContext context) => new Border { Tag = "generic" };
    }

    public sealed class TestLiteralRenderer(string text) : IInlineRenderer<LiteralInline>
    {
        public System.Windows.Documents.Inline Render(LiteralInline inline, IRenderContext context) => new Run(text);
    }

    public sealed class ParameterlessLiteralRenderer : IInlineRenderer<LiteralInline>
    {
        public System.Windows.Documents.Inline Render(LiteralInline inline, IRenderContext context) => new Run("generic");
    }

    private sealed class RecordingPostProcessor(string name) : IPostProcessor
    {
        public string Name { get; } = name;

        public void Process(UIElement root, IRenderContext context)
        {
        }
    }
}