using System.Windows.Documents;

using Markdig.Syntax.Inlines;

using MarkdownViewer.Wpf.Core;

namespace MarkdownViewer.Wpf.Rendering.Inlines;

public sealed class DelimiterInlineRenderer : IInlineRenderer<DelimiterInline>
{
    public System.Windows.Documents.Inline Render(DelimiterInline inline, IRenderContext context)
    {
        ArgumentNullException.ThrowIfNull(inline);
        ArgumentNullException.ThrowIfNull(context);
        return new Run(inline.ToString());
    }
}