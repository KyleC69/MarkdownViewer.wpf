using System.Windows;

using MarkdownViewer.Wpf.Core;

namespace MarkdownViewer.Wpf.Diagnostics;

public sealed class LoggingPostProcessor : IPostProcessor
{
    private readonly Action<string> log;

    public LoggingPostProcessor(Action<string> log)
    {
        this.log = log ?? throw new ArgumentNullException(nameof(log));
    }

    public void Process(UIElement root, IRenderContext context)
    {
        ArgumentNullException.ThrowIfNull(root);
        ArgumentNullException.ThrowIfNull(context);

        log($"Markdown render completed. Root element: {root.GetType().Name}, theme: {context.Theme.GetType().Name}.");
    }
}