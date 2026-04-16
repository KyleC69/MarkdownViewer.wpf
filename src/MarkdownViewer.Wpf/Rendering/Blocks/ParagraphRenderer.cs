using System.Windows.Controls;

using Markdig.Syntax;

using MarkdownViewer.Wpf.Core;
using MarkdownViewer.Wpf.Theming;

namespace MarkdownViewer.Wpf.Rendering.Blocks;

public sealed class ParagraphRenderer : IBlockRenderer<ParagraphBlock>
{
    public System.Windows.UIElement Render(ParagraphBlock block, IRenderContext context)
    {
        ArgumentNullException.ThrowIfNull(block);
        ArgumentNullException.ThrowIfNull(context);

        TextBlock textBlock = RenderHelpers.CreateTextBlock(context, ThemeKeys.ParagraphStyle);
        RenderHelpers.AppendInlines(textBlock.Inlines, block.Inline, context);
        return textBlock;
    }
}