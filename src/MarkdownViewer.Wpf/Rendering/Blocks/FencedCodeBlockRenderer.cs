using System.Windows;

using Markdig.Syntax;

using MarkdownViewer.Wpf.Core;

namespace MarkdownViewer.Wpf.Rendering.Blocks;

public sealed class FencedCodeBlockRenderer : IBlockRenderer<FencedCodeBlock>
{
    public UIElement Render(FencedCodeBlock block, IRenderContext context)
    {
        ArgumentNullException.ThrowIfNull(block);
        ArgumentNullException.ThrowIfNull(context);

        return CodeBlockRenderer.CreateCodeBlock(block, context, CodeBlockRenderer.GetLanguage(block));
    }
}