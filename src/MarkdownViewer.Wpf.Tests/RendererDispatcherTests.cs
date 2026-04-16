using Markdig;
using Markdig.Syntax;

using MarkdownViewer.Wpf.Core;
using MarkdownViewer.Wpf.Diagnostics;
using MarkdownViewer.Wpf.Theming;

using Xunit;

namespace MarkdownViewer.Wpf.Tests;

public sealed class RendererDispatcherTests
{
    [Fact]
    public void RenderBlock_Throws_WhenRendererIsMissing()
    {
        RendererDispatcher dispatcher = new([], []);
        RenderContext context = new(dispatcher, new FakeTheme(), EmptyServiceProvider.Instance);
        MarkdownDocument document = Markdig.Markdown.Parse("plain text", MarkdownEngine.CreateDefaultPipeline());
        ParagraphBlock block = Assert.IsType<ParagraphBlock>(document[0]);

        RendererNotFoundException exception = Assert.Throws<RendererNotFoundException>(() => dispatcher.RenderBlock(block, context));

        Assert.Equal(typeof(ParagraphBlock), exception.MarkdownNodeType);
        Assert.Contains(nameof(ParagraphBlock), exception.Message);
    }
}