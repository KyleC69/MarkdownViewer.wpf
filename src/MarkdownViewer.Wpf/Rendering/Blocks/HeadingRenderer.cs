using System.Windows.Controls;

using Markdig.Syntax;

using MarkdownViewer.Wpf.Core;
using MarkdownViewer.Wpf.Theming;

namespace MarkdownViewer.Wpf.Rendering.Blocks;

public sealed class HeadingRenderer : IBlockRenderer<HeadingBlock>
{
    public System.Windows.UIElement Render(HeadingBlock block, IRenderContext context)
    {
        ArgumentNullException.ThrowIfNull(block);
        ArgumentNullException.ThrowIfNull(context);

        TextBlock textBlock = RenderHelpers.CreateTextBlock(context, ThemeKeys.GetHeadingStyle(block.Level));
        RenderHelpers.AppendInlines(textBlock.Inlines, block.Inline, context);
        return textBlock;
    }
}