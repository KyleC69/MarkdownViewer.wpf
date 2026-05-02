// Solution: MarkdownViewer.Wpf
// Project:   MarkdownViewer.Wpf
// File:         ThematicBreakRenderer.cs
// Author: Kyle L. Crowder
// Build Date: 2026/05/02



using System.Windows;

using Markdig.Syntax;

using MarkdownViewer.Wpf.Controls;
using MarkdownViewer.Wpf.Core;




namespace MarkdownViewer.Wpf.Rendering.Blocks;





public sealed class ThematicBreakRenderer : IBlockRenderer<ThematicBreakBlock>
{
    public UIElement Render(ThematicBreakBlock block, IRenderContext context)
    {
        ArgumentNullException.ThrowIfNull(block);
        ArgumentNullException.ThrowIfNull(context);

        ThematicBreakBorder border = new() { Height = 1, HorizontalAlignment = HorizontalAlignment.Stretch, Background = SystemColors.ControlDarkBrush, Margin = new Thickness(0, 4, 0, 16) };

        return border;
    }
}