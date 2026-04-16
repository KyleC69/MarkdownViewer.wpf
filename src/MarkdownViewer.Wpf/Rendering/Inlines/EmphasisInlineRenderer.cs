using System.Windows.Documents;

using Markdig.Syntax.Inlines;

using MarkdownViewer.Wpf.Core;
using MarkdownViewer.Wpf.Theming;

namespace MarkdownViewer.Wpf.Rendering.Inlines;

public sealed class EmphasisInlineRenderer : IInlineRenderer<EmphasisInline>
{
    public System.Windows.Documents.Inline Render(EmphasisInline inline, IRenderContext context)
    {
        ArgumentNullException.ThrowIfNull(inline);
        ArgumentNullException.ThrowIfNull(context);

        Span span = CreateSpan(inline, context.Theme);
        RenderHelpers.AppendInlines(span.Inlines, inline, context);
        return span;
    }

    internal static Span CreateSpan(EmphasisInline inline, ITheme theme)
    {
        Span span = inline.DelimiterChar switch
        {
            '*' or '_' when inline.DelimiterCount >= 2 => new Bold(),
            '*' or '_' => new Italic(),
            _ => new Span(),
        };

        string? styleKey = inline.DelimiterChar switch
        {
            '~' when inline.DelimiterCount >= 2 => ThemeKeys.StrikeThroughStyle,
            '~' => ThemeKeys.SubscriptStyle,
            '^' => ThemeKeys.SuperscriptStyle,
            '+' => ThemeKeys.InsertedStyle,
            '=' => ThemeKeys.MarkedStyle,
            _ => null,
        };

        if (!string.IsNullOrWhiteSpace(styleKey))
        {
            RenderHelpers.TryApplyStyle(span, theme, styleKey!);
        }

        return span;
    }
}