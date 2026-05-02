// Solution: MarkdownViewer.Wpf
// Project:   MarkdownViewer.Wpf
// File:         IBlockRenderer.cs
// Author: Kyle L. Crowder
// Build Date: 2026/05/02



using System.Windows;

using Markdig.Syntax;




namespace MarkdownViewer.Wpf.Core;





public interface IBlockRenderer<in TBlock> where TBlock : Block
{
    UIElement Render(TBlock block, IRenderContext context);
}