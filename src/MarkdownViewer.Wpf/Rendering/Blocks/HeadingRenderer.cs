// Solution: MarkdownViewer.Wpf
// Project:   MarkdownViewer.Wpf
// File:         HeadingRenderer.cs
// Author: Kyle L. Crowder
// Build Date: 2026/05/02



using System.Windows;
using System.Windows.Controls;

using Markdig.Syntax;

using MarkdownViewer.Wpf.Controls;
using MarkdownViewer.Wpf.Core;




namespace MarkdownViewer.Wpf.Rendering.Blocks;





public sealed class HeadingRenderer : IBlockRenderer<HeadingBlock>
{
    public UIElement Render(HeadingBlock block, IRenderContext context)
    {
        ArgumentNullException.ThrowIfNull(block);
        ArgumentNullException.ThrowIfNull(context);

        TextBlock textBlock = block.Level switch
        {
                1 => new Heading1TextBlock { FontSize = 30, FontWeight = FontWeights.Bold, Margin = new Thickness(0, 8, 0, 16) },
                2 => new Heading2TextBlock { FontSize = 24, FontWeight = FontWeights.Bold, Margin = new Thickness(0, 8, 0, 14) },
                3 => new Heading3TextBlock { FontSize = 20, FontWeight = FontWeights.SemiBold, Margin = new Thickness(0, 6, 0, 12) },
                4 => new Heading4TextBlock { FontSize = 18, FontWeight = FontWeights.SemiBold, Margin = new Thickness(0, 6, 0, 10) },
                5 => new Heading5TextBlock { FontSize = 16, FontWeight = FontWeights.SemiBold, Margin = new Thickness(0, 4, 0, 8) },
                _ => new Heading6TextBlock { FontSize = 14, FontWeight = FontWeights.SemiBold, Margin = new Thickness(0, 4, 0, 8) }
        };
        textBlock.TextWrapping = TextWrapping.Wrap;
        RenderHelpers.AppendInlines(textBlock.Inlines, block.Inline, context);
        return textBlock;
    }
}