// Solution: MarkdownViewer.Wpf
// Project:   MarkdownViewer.Wpf.Tests
// File:         RendererDispatcherTests.cs
// Author: Kyle L. Crowder
// Build Date: 2026/05/02



using Markdig;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

using MarkdownViewer.Wpf.Core;
using MarkdownViewer.Wpf.Diagnostics;

using Xunit;




namespace MarkdownViewer.Wpf.Tests;





public sealed class RendererDispatcherTests
{

    [Fact]
    public void IndexRenderers_BuildsCorrectIndex_ForValidRenderers()
    {
        StubParagraphRenderer renderer = new();
        object[] renderers = [renderer];

        var index = RendererDispatcher.IndexRenderers(renderers, typeof(IBlockRenderer<>));

        Assert.True(index.ContainsKey(typeof(ParagraphBlock)));
        Assert.Same(renderer, index[typeof(ParagraphBlock)]);
    }








    [Theory]
    [InlineData(typeof(ParagraphBlock))]
    public void IndexRenderers_MapsExpectedType_ForEachRenderer(Type expectedKey)
    {
        StubParagraphRenderer renderer = new StubParagraphRenderer();
        var index = RendererDispatcher.IndexRenderers([renderer], typeof(IBlockRenderer<>));

        Assert.True(index.ContainsKey(expectedKey), $"Expected key {expectedKey.Name} not found in index.");
    }








    [Fact]
    public void IndexRenderers_Throws_WhenRendererDoesNotImplementInterface()
    {
        object[] invalidRenderers = [new object()];

        Assert.Throws<InvalidOperationException>(() => RendererDispatcher.IndexRenderers(invalidRenderers, typeof(IBlockRenderer<>)));
    }








    [Fact]
    public void IndexRenderers_Throws_WhenRendererItemIsNull()
    {
        object[] renderersWithNull = [null!];

        Assert.Throws<ArgumentNullException>(() => RendererDispatcher.IndexRenderers(renderersWithNull, typeof(IBlockRenderer<>)));
    }








    [Fact]
    public void IndexRenderers_Throws_WhenRenderersIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => RendererDispatcher.IndexRenderers(null!, typeof(IBlockRenderer<>)));
    }








    [Fact]
    public void RenderBlock_Throws_WhenBlockIsNull()
    {
        RendererDispatcher dispatcher = new RendererDispatcher([], []);
        RenderContext context = MarkdownTestHelper.CreateContext(dispatcher);

        Assert.Throws<ArgumentNullException>(() => dispatcher.RenderBlock(null!, context));
    }








    [Fact]
    public void RenderBlock_Throws_WhenContextIsNull()
    {
        RendererDispatcher dispatcher = new RendererDispatcher([], []);
        MarkdownDocument document = Markdown.Parse("text", MarkdownEngine.CreateDefaultPipeline());
        ParagraphBlock block = Assert.IsType<ParagraphBlock>(document[0]);

        Assert.Throws<ArgumentNullException>(() => dispatcher.RenderBlock(block, null!));
    }








    [Fact]
    public void RenderBlock_Throws_WhenRendererIsMissing()
    {
        RendererDispatcher dispatcher = new RendererDispatcher([], []);
        RenderContext context = MarkdownTestHelper.CreateContext(dispatcher);
        MarkdownDocument document = Markdown.Parse("plain text", MarkdownEngine.CreateDefaultPipeline());
        ParagraphBlock block = Assert.IsType<ParagraphBlock>(document[0]);

        RendererNotFoundException exception = Assert.Throws<RendererNotFoundException>(() => dispatcher.RenderBlock(block, context));

        Assert.Equal(typeof(ParagraphBlock), exception.MarkdownNodeType);
        Assert.Contains(nameof(ParagraphBlock), exception.Message);
    }








    [Fact]
    public void RenderInline_Throws_WhenContextIsNull()
    {
        RendererDispatcher dispatcher = new RendererDispatcher([], []);
        LiteralInline inline = new LiteralInline("hello");

        Assert.Throws<ArgumentNullException>(() => dispatcher.RenderInline(inline, null!));
    }








    [Fact]
    public void RenderInline_Throws_WhenInlineIsNull()
    {
        RendererDispatcher dispatcher = new RendererDispatcher([], []);
        RenderContext context = MarkdownTestHelper.CreateContext(dispatcher);

        Assert.Throws<ArgumentNullException>(() => dispatcher.RenderInline(null!, context));
    }








    [Fact]
    public void RenderInline_Throws_WhenRendererIsMissing()
    {
        RendererDispatcher dispatcher = new RendererDispatcher([], []);
        RenderContext context = MarkdownTestHelper.CreateContext(dispatcher);
        LiteralInline inline = new LiteralInline("hello");

        RendererNotFoundException exception = Assert.Throws<RendererNotFoundException>(() => dispatcher.RenderInline(inline, context));

        Assert.Equal(typeof(LiteralInline), exception.MarkdownNodeType);
        Assert.Contains(nameof(LiteralInline), exception.Message);
    }








    private sealed class StubParagraphRenderer : IBlockRenderer<ParagraphBlock>
    {
        public System.Windows.UIElement Render(ParagraphBlock block, IRenderContext context) =>
                throw new NotImplementedException();
    }
}