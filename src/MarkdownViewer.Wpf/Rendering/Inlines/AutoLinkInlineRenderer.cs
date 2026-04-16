using System.Windows.Documents;

using Markdig.Syntax.Inlines;

using MarkdownViewer.Wpf.Core;
using MarkdownViewer.Wpf.Theming;

namespace MarkdownViewer.Wpf.Rendering.Inlines;

public sealed class AutoLinkInlineRenderer : IInlineRenderer<AutolinkInline>
{
    public System.Windows.Documents.Inline Render(AutolinkInline inline, IRenderContext context)
    {
        ArgumentNullException.ThrowIfNull(inline);
        ArgumentNullException.ThrowIfNull(context);

        string url = inline.IsEmail ? $"mailto:{inline.Url}" : inline.Url;
        Hyperlink hyperlink = new(new Run(inline.Url));
        RenderHelpers.TryApplyStyle(hyperlink, context.Theme, ThemeKeys.HyperlinkStyle);

        if (Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out Uri? uri))
        {
            MarkdownIntegrationServices.ConfigureHyperlink(hyperlink, uri, context);
        }

        return hyperlink;
    }
}