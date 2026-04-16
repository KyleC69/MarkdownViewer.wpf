namespace MarkdownViewer.Wpf.Diagnostics;

public sealed class RendererNotFoundException : InvalidOperationException
{
    public RendererNotFoundException(Type markdownNodeType, string message)
        : base(message)
    {
        MarkdownNodeType = markdownNodeType ?? throw new ArgumentNullException(nameof(markdownNodeType));
    }

    public Type MarkdownNodeType { get; }
}