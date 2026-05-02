// Solution: MarkdownViewer.Wpf
// Project:   MarkdownViewer.Wpf.Tests
// File:         LinkInlineRendererTests.cs
// Author: Kyle L. Crowder
// Build Date: 2026/05/02



using System.Windows.Controls;
using System.Windows.Documents;

using Markdig.Syntax.Inlines;

using MarkdownViewer.Wpf.Core;
using MarkdownViewer.Wpf.Rendering.Inlines;

using Xunit;




namespace MarkdownViewer.Wpf.Tests;





public sealed class LinkInlineRendererTests
{
    private readonly RenderContext context = MarkdownTestHelper.CreateContext();
    private readonly LinkInlineRenderer renderer = new();








    [StaTheory]
    [InlineData("[a](https://a.com)")]
    [InlineData("[b](https://b.com \"title\")")]
    [InlineData("[c](https://c.com)")]
    public void Render_AlwaysReturnsHyperlink_ForTextLinks(string markdown)
    {
        LinkInline inline = MarkdownTestHelper.FindFirstInline<LinkInline>(markdown);

        System.Windows.Documents.Inline result = renderer.Render(inline, context);

        Assert.IsType<Hyperlink>(result);
    }








    [StaFact]
    public void Render_ContainsChildInlines_WithLinkText()
    {
        LinkInline inline = MarkdownTestHelper.FindFirstInline<LinkInline>("[click here](https://example.com)");

        Hyperlink hyperlink = Assert.IsType<Hyperlink>(renderer.Render(inline, context));

        Assert.NotEmpty(hyperlink.Inlines);
    }








    [StaFact]
    public void Render_DoesNotSetToolTip_WhenLinkHasNoTitle()
    {
        LinkInline inline = MarkdownTestHelper.FindFirstInline<LinkInline>("[link](https://example.com)");

        Hyperlink hyperlink = Assert.IsType<Hyperlink>(renderer.Render(inline, context));

        Assert.Null(hyperlink.ToolTip);
    }








    [StaFact]
    public void Render_ImageContainer_ContainsImageElement()
    {
        LinkInline inline = MarkdownTestHelper.FindFirstInline<LinkInline>("![alt text](https://example.com/image.png)");

        InlineUIContainer container = Assert.IsType<InlineUIContainer>(renderer.Render(inline, context));

        Assert.IsType<Image>(container.Child);
    }








    [StaFact]
    public void Render_ReturnsHyperlink_ForStandardLink()
    {
        LinkInline inline = MarkdownTestHelper.FindFirstInline<LinkInline>("[click here](https://example.com)");

        System.Windows.Documents.Inline result = renderer.Render(inline, context);

        Assert.IsType<Hyperlink>(result);
    }








    [StaFact]
    public void Render_ReturnsInlineUIContainer_ForImageLink()
    {
        LinkInline inline = MarkdownTestHelper.FindFirstInline<LinkInline>("![alt text](https://example.com/image.png)");

        System.Windows.Documents.Inline result = renderer.Render(inline, context);

        Assert.IsType<InlineUIContainer>(result);
    }








    [StaFact]
    public void Render_SetsNavigateUri_ForValidAbsoluteUrl()
    {
        LinkInline inline = MarkdownTestHelper.FindFirstInline<LinkInline>("[link](https://example.com)");

        Hyperlink hyperlink = Assert.IsType<Hyperlink>(renderer.Render(inline, context));

        Assert.Equal(new Uri("https://example.com"), hyperlink.NavigateUri);
    }








    [StaFact]
    public void Render_SetsToolTip_WhenLinkHasTitle()
    {
        LinkInline inline = MarkdownTestHelper.FindFirstInline<LinkInline>("[link](https://example.com \"My Title\")");

        Hyperlink hyperlink = Assert.IsType<Hyperlink>(renderer.Render(inline, context));

        Assert.Equal("My Title", hyperlink.ToolTip);
    }








    [StaFact]
    public void Render_Throws_WhenContextIsNull()
    {
        LinkInline inline = MarkdownTestHelper.FindFirstInline<LinkInline>("[link](https://example.com)");

        Assert.Throws<ArgumentNullException>(() => renderer.Render(inline, null!));
    }








    [StaFact]
    public void Render_Throws_WhenInlineIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => renderer.Render(null!, context));
    }








    [StaFact]
    public void RenderImage_ReturnsInlineUIContainer_WithImageChild()
    {
        System.Windows.Documents.Inline result = LinkInlineRenderer.RenderImage("https://example.com/img.png", context);

        InlineUIContainer container = Assert.IsType<InlineUIContainer>(result);
        Assert.IsType<Image>(container.Child);
    }








    [StaFact]
    public void RenderImage_WithNullUrl_ReturnsInlineUIContainerWithImageHavingNullSource()
    {
        System.Windows.Documents.Inline result = LinkInlineRenderer.RenderImage(null, context);

        InlineUIContainer container = Assert.IsType<InlineUIContainer>(result);
        Image image = Assert.IsType<Image>(container.Child);
        Assert.Null(image.Source);
    }
}