// Solution: MarkdownViewer.Wpf
// Project:   MarkdownViewer.Wpf
// File:         ParagraphRenderer.cs
// Author: Kyle L. Crowder
// Build Date: 2026/05/02



using System.Windows;

using Markdig.Syntax;

using MarkdownViewer.Wpf.Controls;
using MarkdownViewer.Wpf.Core;




namespace MarkdownViewer.Wpf.Rendering.Blocks;





public sealed class ParagraphRenderer : IBlockRenderer<ParagraphBlock>
{
    public UIElement Render(ParagraphBlock block, IRenderContext context)
    {
        ArgumentNullException.ThrowIfNull(block);
        ArgumentNullException.ThrowIfNull(context);

        ParagraphTextBlock textBlock = new() { TextWrapping = context.WordWrap ? TextWrapping.Wrap : TextWrapping.NoWrap, Margin = new Thickness(0, 0, 0, 12) };
        RenderHelpers.AppendInlines(textBlock.Inlines, block.Inline, context);
        return textBlock;
    }
}
