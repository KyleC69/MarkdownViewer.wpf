// Solution: MarkdownViewer.Wpf
// Project:   MarkdownViewer.Wpf
// File:         RendererNotFoundException.cs
// Author: Kyle L. Crowder
// Build Date: 2026/05/02



namespace MarkdownViewer.Wpf.Diagnostics;





public sealed class RendererNotFoundException : InvalidOperationException
{
    public RendererNotFoundException(Type markdownNodeType, string message) : base(message)
    {
        MarkdownNodeType = markdownNodeType ?? throw new ArgumentNullException(nameof(markdownNodeType));
    }








    public Type MarkdownNodeType { get; }
}