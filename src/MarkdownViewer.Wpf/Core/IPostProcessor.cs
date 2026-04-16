using System.Windows;

namespace MarkdownViewer.Wpf.Core;

public interface IPostProcessor
{
    void Process(UIElement root, IRenderContext context);
}