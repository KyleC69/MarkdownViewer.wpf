using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

using Markdig.Extensions.AutoLinks;
using Markdig.Extensions.Tables;
using Markdig.Extensions.TaskLists;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

using MarkdownViewer.Wpf.Core;
using MarkdownViewer.Wpf.Rendering.Blocks;
using MarkdownViewer.Wpf.Rendering.Inlines;
using MarkdownViewer.Wpf.Theming;

using Xunit;

namespace MarkdownViewer.Wpf.Tests;

public sealed class RendererBehaviorTests
{
    [StaFact]
    public void ParagraphRenderer_Render_ReturnsTextBlockWithParagraphText()
    {
        ParagraphRenderer renderer = new();
        ParagraphBlock block = MarkdownTestHelper.ParseFirstBlock<ParagraphBlock>("paragraph text");

        TextBlock result = Assert.IsType<TextBlock>(renderer.Render(block, MarkdownTestHelper.CreateContext()));

        Assert.Equal("paragraph text", MarkdownTestHelper.GetInlineText(result));
    }

    [StaFact]
    public void HeadingRenderer_Render_ReturnsTextBlockWithHeadingText()
    {
        HeadingRenderer renderer = new();
        HeadingBlock block = MarkdownTestHelper.ParseFirstBlock<HeadingBlock>("# Heading");

        TextBlock result = Assert.IsType<TextBlock>(renderer.Render(block, MarkdownTestHelper.CreateContext()));

        Assert.Equal("Heading", MarkdownTestHelper.GetInlineText(result));
    }

    [Fact]
    public void ListItemRenderer_GetMarker_ReturnsBullet_WhenParentIsMissing()
    {
        ListItemBlock item = new(null!);

        Assert.Equal("•", ListItemRenderer.GetMarker(item));
    }

    [Fact]
    public void ListItemRenderer_GetMarker_ReturnsBullet_ForUnorderedList()
    {
        ListItemBlock item = MarkdownTestHelper.FindFirstBlock<ListItemBlock>("- item");

        Assert.Equal("•", ListItemRenderer.GetMarker(item));
    }

    [Fact]
    public void ListItemRenderer_GetMarker_ReturnsSequence_ForOrderedList()
    {
        ListBlock list = MarkdownTestHelper.ParseFirstBlock<ListBlock>("3. first\n4. second");
        ListItemBlock second = Assert.IsType<ListItemBlock>(list[1]);

        Assert.Equal("4.", ListItemRenderer.GetMarker(second));
    }

    [StaFact]
    public void ListRenderer_Render_ReturnsPanelWithRenderedItems()
    {
        ListRenderer renderer = new();
        ListBlock block = MarkdownTestHelper.ParseFirstBlock<ListBlock>("- one\n- two");

        StackPanel result = Assert.IsType<StackPanel>(renderer.Render(block, MarkdownTestHelper.CreateContext()));

        Assert.Equal(2, result.Children.Count);
        Assert.All(result.Children.Cast<UIElement>(), child => Assert.IsType<Grid>(child));
    }

    [StaFact]
    public void ListItemRenderer_Render_ReturnsMarkerAndContentGrid()
    {
        ListItemRenderer renderer = new();
        ListItemBlock block = MarkdownTestHelper.FindFirstBlock<ListItemBlock>("1. item");

        Grid result = Assert.IsType<Grid>(renderer.Render(block, MarkdownTestHelper.CreateContext()));

        TextBlock marker = Assert.IsType<TextBlock>(result.Children[0]);
        StackPanel content = Assert.IsType<StackPanel>(result.Children[1]);
        Assert.Equal("1.", marker.Text);
        Assert.Single(content.Children);
    }

    [Fact]
    public void CodeBlockRenderer_GetLanguage_ReturnsFenceInfo()
    {
        FencedCodeBlock block = MarkdownTestHelper.FindFirstBlock<FencedCodeBlock>("```csharp\nConsole.WriteLine(1);\n```");

        Assert.Equal("csharp", CodeBlockRenderer.GetLanguage(block));
    }

    [Fact]
    public void CodeBlockRenderer_GetLanguage_ReturnsNull_WhenInfoIsMissing()
    {
        CodeBlock block = MarkdownTestHelper.FindFirstBlock<CodeBlock>("    indented code");

        Assert.Null(CodeBlockRenderer.GetLanguage(block));
    }

    [StaFact]
    public void CodeBlockRenderer_CreateCodeBlock_WithLanguage_AddsHeader()
    {
        FencedCodeBlock block = MarkdownTestHelper.FindFirstBlock<FencedCodeBlock>("```csharp\nConsole.WriteLine(1);\n```");

        Border border = CodeBlockRenderer.CreateCodeBlock(block, MarkdownTestHelper.CreateContext(), "csharp");

        Grid grid = Assert.IsType<Grid>(border.Child);
        DockPanel header = Assert.IsType<DockPanel>(grid.Children[0]);
        TextBlock headerText = Assert.IsType<TextBlock>(header.Children[0]);
        Button copyButton = Assert.IsType<Button>(header.Children[1]);
        Assert.Equal("csharp", headerText.Text);
        Assert.Equal("Console.WriteLine(1);", copyButton.Tag);
        Assert.Equal(2, grid.RowDefinitions.Count);
    }

    [StaFact]
    public void CodeBlockRenderer_CreateCodeBlock_WithoutLanguage_OmitsHeader()
    {
        CodeBlock block = MarkdownTestHelper.FindFirstBlock<CodeBlock>("    indented code");

        Border border = CodeBlockRenderer.CreateCodeBlock(block, MarkdownTestHelper.CreateContext(), null);

        Grid grid = Assert.IsType<Grid>(border.Child);
        Assert.Single(grid.RowDefinitions);
        Assert.Single(grid.Children);
        Assert.IsType<ScrollViewer>(grid.Children[0]);
    }

    [StaFact]
    public void FencedCodeBlockRenderer_Render_UsesCodeBlockVisualStructure()
    {
        FencedCodeBlockRenderer renderer = new();
        FencedCodeBlock block = MarkdownTestHelper.FindFirstBlock<FencedCodeBlock>("```json\n{}\n```");

        Border result = Assert.IsType<Border>(renderer.Render(block, MarkdownTestHelper.CreateContext()));

        Assert.IsType<Grid>(result.Child);
    }

    [StaFact]
    public void BlockQuoteRenderer_Render_WrapsContentInBorder()
    {
        BlockQuoteRenderer renderer = new();
        QuoteBlock block = MarkdownTestHelper.ParseFirstBlock<QuoteBlock>("> quoted");

        Border result = Assert.IsType<Border>(renderer.Render(block, MarkdownTestHelper.CreateContext()));

        Assert.IsType<StackPanel>(result.Child);
    }

    [StaFact]
    public void ThematicBreakRenderer_Render_ReturnsHorizontalBorder()
    {
        ThematicBreakRenderer renderer = new();
        ThematicBreakBlock block = MarkdownTestHelper.ParseFirstBlock<ThematicBreakBlock>("---");

        Border result = Assert.IsType<Border>(renderer.Render(block, MarkdownTestHelper.CreateContext()));

        Assert.Equal(1, result.Height);
        Assert.Equal(HorizontalAlignment.Stretch, result.HorizontalAlignment);
    }

    [StaFact]
    public void TableRenderer_Render_BuildsGridForHeaderAndBody()
    {
        TableRenderer renderer = new();
        Markdig.Extensions.Tables.Table table = MarkdownTestHelper.ParseFirstBlock<Markdig.Extensions.Tables.Table>("| A | B |\n| - | - |\n| 1 | 2 |");

        Grid result = Assert.IsType<Grid>(renderer.Render(table, MarkdownTestHelper.CreateContext()));

        Assert.True(result.RowDefinitions.Count >= 2);
        Assert.True(result.ColumnDefinitions.Count >= 2);
        Assert.True(result.Children.Count >= 3);
        Assert.All(result.Children.Cast<UIElement>(), child => Assert.IsType<Border>(child));
    }

    [StaFact]
    public void HtmlBlockRenderer_Render_RendersHtmlContentInsteadOfRawTags()
    {
        HtmlBlockRenderer renderer = new();
        HtmlBlock block = MarkdownTestHelper.ParseFirstBlock<HtmlBlock>("<div>markup</div>");

        TextBlock result = Assert.IsType<TextBlock>(renderer.Render(block, MarkdownTestHelper.CreateContext()));

        Assert.Equal("markup", MarkdownTestHelper.GetInlineText(result));
    }

    [StaFact]
    public void HtmlBlockRenderer_Render_UsesCodeBlockChrome_ForPreCodeLanguageClass()
    {
        HtmlBlockRenderer renderer = new();
        HtmlBlock block = MarkdownTestHelper.ParseFirstBlock<HtmlBlock>("<pre><code class=\"language-csharp\">Console.WriteLine(1);</code></pre>");

        Border result = Assert.IsType<Border>(renderer.Render(block, MarkdownTestHelper.CreateContext()));

        Grid grid = Assert.IsType<Grid>(result.Child);
        DockPanel header = Assert.IsType<DockPanel>(grid.Children[0]);
        TextBlock headerText = Assert.IsType<TextBlock>(header.Children[0]);
        Assert.Equal("csharp", headerText.Text);
    }

    [StaFact]
    public void LiteralInlineRenderer_Render_ReturnsRunWithText()
    {
        LiteralInlineRenderer renderer = new();
        LiteralInline inline = MarkdownTestHelper.FindFirstInline<LiteralInline>("literal");

        Run result = Assert.IsType<Run>(renderer.Render(inline, MarkdownTestHelper.CreateContext()));

        Assert.Equal("literal", result.Text);
    }

    [StaFact]
    public void CodeInlineRenderer_Render_ReturnsRunWithCodeText()
    {
        CodeInlineRenderer renderer = new();
        CodeInline inline = MarkdownTestHelper.FindFirstInline<CodeInline>("`code`");

        Run result = Assert.IsType<Run>(renderer.Render(inline, MarkdownTestHelper.CreateContext()));

        Assert.Equal("code", result.Text);
    }

    [StaFact]
    public void LineBreakInlineRenderer_Render_ReturnsLineBreak()
    {
        LineBreakInlineRenderer renderer = new();
        LineBreakInline inline = MarkdownTestHelper.FindFirstInline<LineBreakInline>("line  \nnext");

        Assert.IsType<LineBreak>(renderer.Render(inline, MarkdownTestHelper.CreateContext()));
    }

    [StaFact]
    public void HtmlInlineRenderer_Render_IgnoresUnsupportedFormattingTag()
    {
        HtmlInlineRenderer renderer = new();
        HtmlInline inline = MarkdownTestHelper.FindFirstInline<HtmlInline>("<span>text</span>");

        Run result = Assert.IsType<Run>(renderer.Render(inline, MarkdownTestHelper.CreateContext()));

        Assert.Equal(string.Empty, result.Text);
    }

    [StaFact]
    public void ParagraphRenderer_Render_ReconstructsInlineHtmlFragment()
    {
        ParagraphRenderer renderer = new();
        ParagraphBlock block = MarkdownTestHelper.ParseFirstBlock<ParagraphBlock>("before <strong>bold</strong> <a href=\"https://example.com\">docs</a> after");

        TextBlock result = Assert.IsType<TextBlock>(renderer.Render(block, MarkdownTestHelper.CreateContext()));

        IReadOnlyList<System.Windows.Documents.Inline> flattened = MarkdownTestHelper.FlattenInlines(result);
        Assert.Contains(flattened, static inline => inline is Bold);
        Hyperlink hyperlink = Assert.IsType<Hyperlink>(flattened.OfType<Hyperlink>().Single());
        Assert.Equal(new Uri("https://example.com"), hyperlink.NavigateUri);
        Assert.Equal("before bold docs after", MarkdownTestHelper.GetInlineText(result));
    }

    [StaFact]
    public void HtmlEntityInlineRenderer_Render_ReturnsDecodedRun()
    {
        HtmlEntityInlineRenderer renderer = new();
        HtmlEntityInline inline = MarkdownTestHelper.FindFirstInline<HtmlEntityInline>("&amp;");

        Run result = Assert.IsType<Run>(renderer.Render(inline, MarkdownTestHelper.CreateContext()));

        Assert.Equal("&", result.Text);
    }

    [StaFact]
    public void AutoLinkInlineRenderer_Render_CreatesHyperlinkForUrl()
    {
        AutoLinkInlineRenderer renderer = new();
        AutolinkInline inline = MarkdownTestHelper.FindFirstInline<AutolinkInline>("<https://example.com>");

        Hyperlink result = Assert.IsType<Hyperlink>(renderer.Render(inline, MarkdownTestHelper.CreateContext()));

        Assert.Equal(new Uri("https://example.com"), result.NavigateUri);
        Assert.Equal("https://example.com", MarkdownTestHelper.GetInlineText(result));
    }

    [StaFact]
    public void AutoLinkInlineRenderer_Render_CreatesMailtoHyperlinkForEmail()
    {
        AutoLinkInlineRenderer renderer = new();
        AutolinkInline inline = MarkdownTestHelper.FindFirstInline<AutolinkInline>("<test@example.com>");

        Hyperlink result = Assert.IsType<Hyperlink>(renderer.Render(inline, MarkdownTestHelper.CreateContext()));

        Assert.Equal(new Uri("mailto:test@example.com"), result.NavigateUri);
    }

    [StaFact]
    public void LinkInlineRenderer_Render_CreatesHyperlinkWithTitleAndText()
    {
        LinkInlineRenderer renderer = new();
        LinkInline inline = MarkdownTestHelper.FindFirstInline<LinkInline>("[docs](https://example.com \"tooltip\")");

        Hyperlink result = Assert.IsType<Hyperlink>(renderer.Render(inline, MarkdownTestHelper.CreateContext()));

        Assert.Equal(new Uri("https://example.com"), result.NavigateUri);
        Assert.Equal("tooltip", result.ToolTip);
        Assert.Equal("docs", MarkdownTestHelper.GetInlineText(result));
    }

    [StaFact]
    public void LinkInlineRenderer_RenderImage_CreatesImageContainer()
    {
        ResourceDictionaryTheme theme = MarkdownTestHelper.CreateTheme((ThemeKeys.ImageStyle, new Style(typeof(Image))));

        InlineUIContainer result = Assert.IsType<InlineUIContainer>(LinkInlineRenderer.RenderImage(null, MarkdownTestHelper.CreateContext(theme: theme)));

        Assert.IsType<Image>(result.Child);
    }

    [StaFact]
    public void LinkInlineRenderer_RenderImage_UsesRegisteredImageResolver()
    {
        TestImageSourceResolver resolver = new();
        IServiceProvider services = MarkdownTestHelper.CreateServiceProvider((typeof(IMarkdownImageSourceResolver), resolver));
        InlineUIContainer result = Assert.IsType<InlineUIContainer>(LinkInlineRenderer.RenderImage("https://example.com/image.png", MarkdownTestHelper.CreateContext(services: services)));

        Image image = Assert.IsType<Image>(result.Child);
        Assert.Same(resolver.ImageSource, image.Source);
        Assert.Equal(new Uri("https://example.com/image.png"), resolver.LastUri);
    }

    [StaFact]
    public void EmphasisInlineRenderer_CreateSpan_ReturnsBold_ForStrongEmphasis()
    {
        EmphasisInline inline = MarkdownTestHelper.FindFirstInline<EmphasisInline>("**strong**");

        Span result = EmphasisInlineRenderer.CreateSpan(inline, new DefaultTheme());

        Assert.IsType<Bold>(result);
    }

    [StaFact]
    public void EmphasisInlineRenderer_CreateSpan_ReturnsItalic_ForSingleEmphasis()
    {
        EmphasisInline inline = MarkdownTestHelper.FindFirstInline<EmphasisInline>("*italic*");

        Span result = EmphasisInlineRenderer.CreateSpan(inline, new DefaultTheme());

        Assert.IsType<Italic>(result);
    }

    [StaFact]
    public void EmphasisInlineRenderer_CreateSpan_AppliesMarkedStyle()
    {
        Style style = new(typeof(Span));
        EmphasisInline inline = MarkdownTestHelper.FindFirstInline<EmphasisInline>("==mark==");

        Span result = EmphasisInlineRenderer.CreateSpan(inline, MarkdownTestHelper.CreateTheme((ThemeKeys.MarkedStyle, style)));

        Assert.Same(style, result.Style);
    }

    [StaFact]
    public void TaskListInlineRenderer_Render_CreatesDisabledCheckBox()
    {
        TaskListInlineRenderer renderer = new();
        TaskList inline = MarkdownTestHelper.FindFirstInline<TaskList>("- [x] done");

        InlineUIContainer result = Assert.IsType<InlineUIContainer>(renderer.Render(inline, MarkdownTestHelper.CreateContext()));
        CheckBox checkBox = Assert.IsType<CheckBox>(result.Child);
        Assert.True(checkBox.IsChecked);
        Assert.False(checkBox.IsEnabled);
    }

    private sealed class TestImageSourceResolver : IMarkdownImageSourceResolver
    {
        public ImageSource ImageSource { get; } = new DrawingImage();

        public Uri? LastUri { get; private set; }

        public ImageSource? ResolveImageSource(Uri uri, IRenderContext context)
        {
            LastUri = uri;
            return ImageSource;
        }
    }
}