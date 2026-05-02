// Solution: MarkdownViewer.Wpf
// Project:   MarkdownViewer.Wpf
// File:         ListRenderer.cs
// Author: Kyle L. Crowder
// Build Date: 2026/05/02



using System.Windows;

using Markdig.Syntax;

using MarkdownViewer.Wpf.Controls;
using MarkdownViewer.Wpf.Core;




namespace MarkdownViewer.Wpf.Rendering.Blocks;





public sealed class ListRenderer : IBlockRenderer<ListBlock>
{
    public UIElement Render(ListBlock block, IRenderContext context)
    {
        ArgumentNullException.ThrowIfNull(block);
        ArgumentNullException.ThrowIfNull(context);

        ListPanel panel = new() { Margin = new Thickness(0, 0, 0, 12) };

        foreach (Block child in block)
        {
            panel.Children.Add(context.RenderBlock(child));
        }

        return panel;
    }
}