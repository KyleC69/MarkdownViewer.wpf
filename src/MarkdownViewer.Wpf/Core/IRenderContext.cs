using System.Windows;
using System.Windows.Documents;

namespace MarkdownViewer.Wpf.Core;

public interface IRenderContext
{
    ResourceDictionary Resources { get; }

    ResourceDictionary ThemeResources { get; }

    IServiceProvider Services { get; }

    UIElement RenderBlock(Markdig.Syntax.Block block);

    System.Windows.Documents.Inline RenderInline(Markdig.Syntax.Inlines.Inline inline);

    void AddPostProcessor(IPostProcessor processor);
}