// Solution: MarkdownViewer.Wpf
// Project:   MarkdownViewer.Wpf
// File:         BlockQuoteRenderer.cs
// Author: Kyle L. Crowder
// Build Date: 2026/05/02



using System.Windows;

using Markdig.Syntax;

using MarkdownViewer.Wpf.Controls;
using MarkdownViewer.Wpf.Core;




namespace MarkdownViewer.Wpf.Rendering.Blocks;





public sealed class BlockQuoteRenderer : IBlockRenderer<QuoteBlock>
{
    public UIElement Render(QuoteBlock block, IRenderContext context)
    {
        ArgumentNullException.ThrowIfNull(block);
        ArgumentNullException.ThrowIfNull(context);

        BlockQuoteBorder border = new() { BorderThickness = new Thickness(4, 0, 0, 0), BorderBrush = SystemColors.ControlDarkBrush, Padding = new Thickness(12, 6, 12, 6), Margin = new Thickness(0, 0, 0, 12) };
        border.Child = RenderHelpers.RenderChildBlocks(block, context);
        return border;
    }
}