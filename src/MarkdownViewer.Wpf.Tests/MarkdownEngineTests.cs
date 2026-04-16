using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

using MarkdownViewer.Wpf.Core;
using MarkdownViewer.Wpf.Theming;

using Xunit;

namespace MarkdownViewer.Wpf.Tests;

public sealed class MarkdownEngineTests
{
    [StaFact]
    public void Render_BuildsExpectedVisualTree_ForHeadingAndParagraph()
    {
        MarkdownEngine engine = MarkdownEngine.CreateDefault();

        UIElement root = engine.Render("# Title\n\nParagraph text", EmptyServiceProvider.Instance, new DefaultTheme());

        StackPanel panel = Assert.IsType<StackPanel>(root);
        Assert.Equal(2, panel.Children.Count);

        TextBlock heading = Assert.IsType<TextBlock>(panel.Children[0]);
        TextBlock paragraph = Assert.IsType<TextBlock>(panel.Children[1]);

        Assert.Equal("Title", GetInlineText(heading.Inlines));
        Assert.Equal("Paragraph text", GetInlineText(paragraph.Inlines));
    }

    [StaFact]
    public void Render_RunsPostProcessors_AfterChildrenAreRendered()
    {
        RecordingPostProcessor postProcessor = new();
        MarkdownRendererBuilder builder = MarkdownRendererBuilder.CreateDefault()
            .AddPostProcessor(postProcessor);
        MarkdownEngine engine = new(MarkdownEngine.CreateDefaultPipeline(), builder.BuildDispatcher(), builder.BuildPostProcessors());

        UIElement root = engine.Render("Paragraph text", EmptyServiceProvider.Instance, new DefaultTheme());

        Assert.True(postProcessor.WasCalled);
        Assert.Same(root, postProcessor.Root);
        Assert.Equal(1, postProcessor.ChildCountWhenProcessed);
    }

    private static string GetInlineText(InlineCollection inlines)
    {
        return string.Concat(inlines.Select(GetInlineText));
    }

    private static string GetInlineText(Inline inline)
    {
        return inline switch
        {
            Run run => run.Text,
            Span span => string.Concat(span.Inlines.Select(GetInlineText)),
            LineBreak => Environment.NewLine,
            _ => string.Empty,
        };
    }

    private sealed class RecordingPostProcessor : IPostProcessor
    {
        public int ChildCountWhenProcessed { get; private set; }

        public UIElement? Root { get; private set; }

        public bool WasCalled { get; private set; }

        public void Process(UIElement root, IRenderContext context)
        {
            WasCalled = true;
            Root = root;
            ChildCountWhenProcessed = Assert.IsType<StackPanel>(root).Children.Count;
        }
    }
}