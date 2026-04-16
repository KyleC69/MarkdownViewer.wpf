using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Documents;

using Markdig.Syntax.Inlines;

using MarkdownViewer.Wpf.Core;
using MarkdownViewer.Wpf.Theming;

namespace MarkdownViewer.Wpf.Rendering.Inlines;

public sealed class LinkInlineRenderer : IInlineRenderer<LinkInline>
{
    public System.Windows.Documents.Inline Render(LinkInline inline, IRenderContext context)
    {
        ArgumentNullException.ThrowIfNull(inline);
        ArgumentNullException.ThrowIfNull(context);

        string? url = inline.GetDynamicUrl?.Invoke() ?? inline.Url;
        if (inline.IsImage)
        {
            return RenderImage(url, context);
        }

        Hyperlink hyperlink = new();

        if (Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out Uri? uri))
        {
            MarkdownIntegrationServices.ConfigureHyperlink(hyperlink, uri, context);
        }

        if (!string.IsNullOrWhiteSpace(inline.Title))
        {
            hyperlink.ToolTip = inline.Title;
        }

        RenderHelpers.AppendInlines(hyperlink.Inlines, inline, context);
        return hyperlink;
    }

    internal static System.Windows.Documents.Inline RenderImage(string? url, IRenderContext context)
    {
        Image image = new();

        if (Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out Uri? uri))
        {
            image.Source = MarkdownIntegrationServices.ResolveImageSource(uri, context);
        }

        return new InlineUIContainer(image);
    }
}