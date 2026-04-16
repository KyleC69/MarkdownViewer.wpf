using System.Windows.Controls;

using Markdig.Syntax;

using MarkdownViewer.Wpf.Core;
using MarkdownViewer.Wpf.Rendering.Html;
using MarkdownViewer.Wpf.Theming;

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