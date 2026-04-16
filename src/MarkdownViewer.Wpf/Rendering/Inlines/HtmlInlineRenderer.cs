using System.Windows.Documents;

using Markdig.Syntax.Inlines;

using MarkdownViewer.Wpf.Core;
using MarkdownViewer.Wpf.Rendering.Html;

namespace MarkdownViewer.Wpf.Rendering.Inlines;

public sealed class HtmlInlineRenderer : IInlineRenderer<HtmlInline>
{
    public System.Windows.Documents.Inline Render(HtmlInline inline, IRenderContext context)
    {
        ArgumentNullException.ThrowIfNull(inline);
        ArgumentNullException.ThrowIfNull(context);
        return HtmlWpfRenderer.RenderInlineTag(inline.Tag ?? string.Empty, context);
    }
}