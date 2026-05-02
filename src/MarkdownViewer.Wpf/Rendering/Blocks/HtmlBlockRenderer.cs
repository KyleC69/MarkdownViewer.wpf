// Solution: MarkdownViewer.Wpf
// Project:   MarkdownViewer.Wpf
// File:         HtmlBlockRenderer.cs
// Author: Kyle L. Crowder
// Build Date: 2026/05/02



using Markdig.Syntax;

using MarkdownViewer.Wpf.Core;
using MarkdownViewer.Wpf.Rendering.Html;




namespace MarkdownViewer.Wpf.Rendering.Blocks;





public sealed class HtmlBlockRenderer : IBlockRenderer<HtmlBlock>
{
    public System.Windows.UIElement Render(HtmlBlock block, IRenderContext context)
    {
        ArgumentNullException.ThrowIfNull(block);
        ArgumentNullException.ThrowIfNull(context);

        return HtmlWpfRenderer.RenderBlock(RenderHelpers.GetLiteral(block), context);
    }
}