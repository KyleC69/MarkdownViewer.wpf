using System.Windows.Controls;

using Markdig.Syntax;

using MarkdownViewer.Wpf.Core;
using MarkdownViewer.Wpf.Theming;

namespace MarkdownViewer.Wpf.Rendering.Blocks;

public sealed class BlockQuoteRenderer : IBlockRenderer<QuoteBlock>
{
    public System.Windows.UIElement Render(QuoteBlock block, IRenderContext context)
    {
        ArgumentNullException.ThrowIfNull(block);
        ArgumentNullException.ThrowIfNull(context);

        Border border = new();
        RenderHelpers.ApplyRole(border, ThemeKeys.BlockQuoteBorderStyle);
        border.Child = RenderHelpers.RenderChildBlocks(block, context);
        return border;
    }
}