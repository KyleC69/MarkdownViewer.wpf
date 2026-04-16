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