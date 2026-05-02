// Solution: MarkdownViewer.Wpf
// Project:   MarkdownViewer.Wpf
// File:         LineBreakInlineRenderer.cs
// Author: Kyle L. Crowder
// Build Date: 2026/05/02



using System.Windows.Documents;

using Markdig.Syntax.Inlines;

using MarkdownViewer.Wpf.Core;




namespace MarkdownViewer.Wpf.Rendering.Inlines;





public sealed class LineBreakInlineRenderer : IInlineRenderer<LineBreakInline>
{
    public System.Windows.Documents.Inline Render(LineBreakInline inline, IRenderContext context)
    {
        ArgumentNullException.ThrowIfNull(inline);
        ArgumentNullException.ThrowIfNull(context);
        return new LineBreak();
    }
}