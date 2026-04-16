using System.Windows;
using System.Windows.Documents;

using MarkdownViewer.Wpf.Theming;

namespace MarkdownViewer.Wpf.Core;

public interface IRenderContext
{
    ITheme Theme { get; }

    IServiceProvider Services { get; }

    UIElement RenderBlock(Markdig.Syntax.Block block);

    System.Windows.Documents.Inline RenderInline(Markdig.Syntax.Inlines.Inline inline);

    void AddPostProcessor(IPostProcessor processor);
}