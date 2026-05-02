// Solution: MarkdownViewer.Wpf
// Project:   MarkdownViewer.Wpf
// File:         IInlineRenderer.cs
// Author: Kyle L. Crowder
// Build Date: 2026/05/02



namespace MarkdownViewer.Wpf.Core;





public interface IInlineRenderer<in TInline> where TInline : Markdig.Syntax.Inlines.Inline
{
    System.Windows.Documents.Inline Render(TInline inline, IRenderContext context);
}