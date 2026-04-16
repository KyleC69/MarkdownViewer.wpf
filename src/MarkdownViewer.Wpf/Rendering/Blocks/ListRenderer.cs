using System.Windows.Controls;

using Markdig.Syntax;

using MarkdownViewer.Wpf.Core;
using MarkdownViewer.Wpf.Theming;

namespace MarkdownViewer.Wpf.Rendering.Blocks;

public sealed class ListRenderer : IBlockRenderer<ListBlock>
{
    public System.Windows.UIElement Render(ListBlock block, IRenderContext context)
    {
        ArgumentNullException.ThrowIfNull(block);
        ArgumentNullException.ThrowIfNull(context);

        StackPanel panel = new()
        {
            Orientation = Orientation.Vertical,
        };

        RenderHelpers.ApplyRole(panel, ThemeKeys.ListStyle);

        foreach (Block child in block)
        {
            panel.Children.Add(context.RenderBlock(child));
        }

        return panel;
    }
}