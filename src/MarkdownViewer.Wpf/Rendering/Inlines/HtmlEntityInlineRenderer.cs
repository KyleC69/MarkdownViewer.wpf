// Solution: MarkdownViewer.Wpf
// Project:   MarkdownViewer.Wpf
// File:         HtmlEntityInlineRenderer.cs
// Author: Kyle L. Crowder
// Build Date: 2026/05/02



using System.Windows.Documents;

using Markdig.Syntax.Inlines;

using MarkdownViewer.Wpf.Core;




namespace MarkdownViewer.Wpf.Rendering.Inlines;





public sealed class HtmlEntityInlineRenderer : IInlineRenderer<HtmlEntityInline>
{
    public System.Windows.Documents.Inline Render(HtmlEntityInline inline, IRenderContext context)
    {
        ArgumentNullException.ThrowIfNull(inline);
        ArgumentNullException.ThrowIfNull(context);
        return new Run(inline.Transcoded.ToString());
    }
}