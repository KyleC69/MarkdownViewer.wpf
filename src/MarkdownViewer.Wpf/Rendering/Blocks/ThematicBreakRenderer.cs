using System.Windows;
using System.Windows.Controls;

using Markdig.Syntax;

using MarkdownViewer.Wpf.Core;
using MarkdownViewer.Wpf.Theming;

namespace MarkdownViewer.Wpf.Rendering.Blocks;

public sealed class ThematicBreakRenderer : IBlockRenderer<ThematicBreakBlock>
{
    public UIElement Render(ThematicBreakBlock block, IRenderContext context)
    {
        ArgumentNullException.ThrowIfNull(block);
        ArgumentNullException.ThrowIfNull(context);

        Border border = new()
        {
            Height = 1,
            HorizontalAlignment = HorizontalAlignment.Stretch,
        };

        RenderHelpers.ApplyRole(border, ThemeKeys.ThematicBreakStyle);
        return border;
    }
}