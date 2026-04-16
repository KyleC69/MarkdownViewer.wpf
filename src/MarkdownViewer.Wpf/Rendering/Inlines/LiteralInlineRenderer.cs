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