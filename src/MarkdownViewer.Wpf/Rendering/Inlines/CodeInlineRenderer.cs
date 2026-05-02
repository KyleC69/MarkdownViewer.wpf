// Solution: MarkdownViewer.Wpf
// Project:   MarkdownViewer.Wpf
// File:         CodeInlineRenderer.cs
// Author: Kyle L. Crowder
// Build Date: 2026/05/02



using System.Windows.Documents;
using System.Windows.Media;

using Markdig.Syntax.Inlines;

using MarkdownViewer.Wpf.Controls;
using MarkdownViewer.Wpf.Core;




namespace MarkdownViewer.Wpf.Rendering.Inlines;





public sealed class CodeInlineRenderer : IInlineRenderer<CodeInline>
{
    private static readonly FontFamily MonospaceFont = new("Consolas, Courier New");








    public System.Windows.Documents.Inline Render(CodeInline inline, IRenderContext context)
    {
        ArgumentNullException.ThrowIfNull(inline);
        ArgumentNullException.ThrowIfNull(context);

        // Use custom Span type for implicit styling
        CodeInlineSpan span = new();
        span.Inlines.Add(new Run(inline.Content) { FontFamily = MonospaceFont });
        return span;
    }
}