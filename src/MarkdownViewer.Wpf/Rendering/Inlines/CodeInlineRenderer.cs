using System.Windows.Documents;

using Markdig.Syntax.Inlines;

using MarkdownViewer.Wpf.Core;
using MarkdownViewer.Wpf.Theming;

namespace MarkdownViewer.Wpf.Rendering.Inlines;

public sealed class CodeInlineRenderer : IInlineRenderer<CodeInline>
{
    public System.Windows.Documents.Inline Render(CodeInline inline, IRenderContext context)
    {
        ArgumentNullException.ThrowIfNull(inline);
        ArgumentNullException.ThrowIfNull(context);

        Run run = new(inline.Content);
        RenderHelpers.TryApplyStyle(run, context.Theme, ThemeKeys.CodeInlineStyle);
        return run;
    }
}