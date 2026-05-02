// Solution: MarkdownViewer.Wpf
// Project:   MarkdownViewer.Wpf
// File:         LiteralInlineRenderer.cs
// Author: Kyle L. Crowder
// Build Date: 2026/05/02



using System.Windows.Documents;

using Markdig.Syntax.Inlines;

using MarkdownViewer.Wpf.Core;




namespace MarkdownViewer.Wpf.Rendering.Inlines;





public sealed class LiteralInlineRenderer : IInlineRenderer<LiteralInline>
{
    public System.Windows.Documents.Inline Render(LiteralInline inline, IRenderContext context)
    {
        ArgumentNullException.ThrowIfNull(inline);
        ArgumentNullException.ThrowIfNull(context);

        return new Run(inline.Content.ToString());
    }
}