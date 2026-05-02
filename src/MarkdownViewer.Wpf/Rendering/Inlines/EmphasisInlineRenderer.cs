using System.Windows;
using System.Windows.Documents;

using Markdig.Syntax.Inlines;

using MarkdownViewer.Wpf.Controls;
using MarkdownViewer.Wpf.Core;

namespace MarkdownViewer.Wpf.Rendering.Inlines;

public sealed class EmphasisInlineRenderer : IInlineRenderer<EmphasisInline>
{
    public System.Windows.Documents.Inline Render(EmphasisInline inline, IRenderContext context)
    {
        ArgumentNullException.ThrowIfNull(inline);
        ArgumentNullException.ThrowIfNull(context);

        Span span = inline.DelimiterChar switch
        {
            '*' or '_' when inline.DelimiterCount >= 2 => new Bold(),
            '*' or '_' => new Italic(),
            '~' when inline.DelimiterCount >= 2 => new StrikeThroughSpan { TextDecorations = TextDecorations.Strikethrough },
            '~' => new SubscriptSpan { Typography = { Variants = FontVariants.Subscript } },
            '^' => new SuperscriptSpan { Typography = { Variants = FontVariants.Superscript } },
            '+' => new InsertedSpan { TextDecorations = TextDecorations.Underline },
            '=' => new MarkedSpan(),
            _ => new Span(),
        };

        RenderHelpers.AppendInlines(span.Inlines, inline, context);
        return span;
    }
}