// Solution: MarkdownViewer.Wpf
// Project:   MarkdownViewer.Wpf.Tests
// File:         MarkdownEngineTests.cs
// Author: Kyle L. Crowder
// Build Date: 2026/05/02



using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

using MarkdownViewer.Wpf.Controls;
using MarkdownViewer.Wpf.Core;

using Xunit;




namespace MarkdownViewer.Wpf.Tests;





public sealed class MarkdownEngineTests
{

    [StaFact]
    public void Constructor_Throws_WhenDispatcherIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => new MarkdownEngine(MarkdownEngine.CreateDefaultPipeline(), null!, Array.Empty<IPostProcessor>()));
    }








    [StaFact]
    public void Constructor_Throws_WhenPipelineIsNull()
    {
        MarkdownRendererBuilder builder = MarkdownRendererBuilder.CreateDefault();

        Assert.Throws<ArgumentNullException>(() => new MarkdownEngine(null!, builder.BuildDispatcher(), builder.BuildPostProcessors()));
    }








    [StaFact]
    public void Constructor_Throws_WhenPostProcessorsIsNull()
    {
        MarkdownRendererBuilder builder = MarkdownRendererBuilder.CreateDefault();

        Assert.Throws<ArgumentNullException>(() => new MarkdownEngine(MarkdownEngine.CreateDefaultPipeline(), builder.BuildDispatcher(), null!));
    }








    [StaFact]
    public void CreateDefaultPipeline_ReturnsNonNullPipeline()
    {
        Markdig.MarkdownPipeline pipeline = MarkdownEngine.CreateDefaultPipeline();

        Assert.NotNull(pipeline);
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
                _ => string.Empty
        };
    }








    [StaFact]
    public void Render_AppliesThemeResources_ToReturnedPanel()
    {
        MarkdownEngine engine = MarkdownEngine.CreateDefault();
        ResourceDictionary theme = new();
        theme["testKey"] = "testValue";

        MarkdownRootPanel panel = Assert.IsType<MarkdownRootPanel>(engine.Render("text", EmptyServiceProvider.Instance, theme));

        Assert.Equal("testValue", panel.Resources["testKey"]);
    }








    [StaFact]
    public void Render_BuildsExpectedVisualTree_ForHeadingAndParagraph()
    {
        MarkdownEngine engine = MarkdownEngine.CreateDefault();

        UIElement root = engine.Render("# Title\n\nParagraph text", EmptyServiceProvider.Instance);

        MarkdownRootPanel panel = Assert.IsType<MarkdownRootPanel>(root);
        Assert.Equal(2, panel.Children.Count);

        TextBlock heading = Assert.IsAssignableFrom<TextBlock>(panel.Children[0]);
        TextBlock paragraph = Assert.IsAssignableFrom<TextBlock>(panel.Children[1]);

        Assert.Equal("Title", GetInlineText(heading.Inlines));
        Assert.Equal("Paragraph text", GetInlineText(paragraph.Inlines));
    }








    [StaFact]
    public void Render_MergesThemeResourcesMergedDictionaries_IntoScopedResources()
    {
        MarkdownEngine engine = MarkdownEngine.CreateDefault();
        ResourceDictionary inner = new();
        inner["inner"] = "value";
        ResourceDictionary theme = new();
        theme.MergedDictionaries.Add(inner);

        MarkdownRootPanel panel = Assert.IsType<MarkdownRootPanel>(engine.Render("text", EmptyServiceProvider.Instance, theme));

        Assert.Contains(inner, panel.Resources.MergedDictionaries);
    }








    [StaTheory]
    [InlineData("# H1\n\n## H2\n\np", 3)]
    [InlineData("line1\n\nline2\n\nline3", 3)]
    [InlineData("---\n\ntext", 2)]
    public void Render_ProducesCorrectChildCount_ForMultipleBlocks(string markdown, int expectedCount)
    {
        MarkdownEngine engine = MarkdownEngine.CreateDefault();

        MarkdownRootPanel panel = Assert.IsType<MarkdownRootPanel>(engine.Render(markdown, EmptyServiceProvider.Instance));

        Assert.Equal(expectedCount, panel.Children.Count);
    }








    [StaFact]
    public void Render_ReturnsEmptyRoot_ForEmptyMarkdown()
    {
        MarkdownEngine engine = MarkdownEngine.CreateDefault();

        MarkdownRootPanel panel = Assert.IsType<MarkdownRootPanel>(engine.Render(string.Empty, EmptyServiceProvider.Instance));

        Assert.Empty(panel.Children);
    }








    [StaFact]
    public void Render_ReturnsRootWithOneChild_ForSingleBlock()
    {
        MarkdownEngine engine = MarkdownEngine.CreateDefault();

        MarkdownRootPanel panel = Assert.IsType<MarkdownRootPanel>(engine.Render("Hello world", EmptyServiceProvider.Instance));

        Assert.Single(panel.Children);
    }








    [StaFact]
    public void Render_RunsPostProcessors_AfterChildrenAreRendered()
    {
        RecordingPostProcessor postProcessor = new();
        MarkdownRendererBuilder builder = MarkdownRendererBuilder.CreateDefault().AddPostProcessor(postProcessor);
        MarkdownEngine engine = new(MarkdownEngine.CreateDefaultPipeline(), builder.BuildDispatcher(), builder.BuildPostProcessors());

        UIElement root = engine.Render("Paragraph text", EmptyServiceProvider.Instance);

        Assert.True(postProcessor.WasCalled);
        Assert.Same(root, postProcessor.Root);
        Assert.Equal(1, postProcessor.ChildCountWhenProcessed);
    }








    [StaFact]
    public void Render_Throws_WhenMarkdownIsNull()
    {
        MarkdownEngine engine = MarkdownEngine.CreateDefault();

        Assert.Throws<ArgumentNullException>(() => engine.Render(null!, EmptyServiceProvider.Instance));
    }








    [StaFact]
    public void Render_Throws_WhenServicesIsNull()
    {
        MarkdownEngine engine = MarkdownEngine.CreateDefault();

        Assert.Throws<ArgumentNullException>(() => engine.Render("text", null!));
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
            ChildCountWhenProcessed = Assert.IsType<MarkdownRootPanel>(root).Children.Count;
        }
    }
}