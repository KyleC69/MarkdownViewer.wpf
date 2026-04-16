using System.Windows;

using Markdig.Syntax;

namespace MarkdownViewer.Wpf.Core;

public interface IBlockRenderer<in TBlock> where TBlock : Block
{
    UIElement Render(TBlock block, IRenderContext context);
}