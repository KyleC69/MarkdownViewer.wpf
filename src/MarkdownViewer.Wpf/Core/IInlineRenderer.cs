namespace MarkdownViewer.Wpf.Core;

public interface IInlineRenderer<in TInline> where TInline : Markdig.Syntax.Inlines.Inline
{
    System.Windows.Documents.Inline Render(TInline inline, IRenderContext context);
}