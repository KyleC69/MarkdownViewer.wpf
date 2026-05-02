using System.Windows;
using System.Windows.Controls;

using Markdig.Syntax;

using MarkdownViewer.Wpf.Controls;
using MarkdownViewer.Wpf.Core;

namespace MarkdownViewer.Wpf.Rendering.Blocks;

public sealed class ParagraphRenderer : IBlockRenderer<ParagraphBlock>
{
    public System.Windows.UIElement Render(ParagraphBlock block, IRenderContext context)
    {
        ArgumentNullException.ThrowIfNull(block);
        ArgumentNullException.ThrowIfNull(context);

        ParagraphTextBlock textBlock = new()
        {
            TextWrapping = TextWrapping.Wrap,
            Margin = new Thickness(0, 0, 0, 12),
        };
        RenderHelpers.AppendInlines(textBlock.Inlines, block.Inline, context);
        return textBlock;
    }
}