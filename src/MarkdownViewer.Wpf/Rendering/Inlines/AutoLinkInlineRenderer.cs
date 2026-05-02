// Solution: MarkdownViewer.Wpf
// Project:   MarkdownViewer.Wpf
// File:         AutoLinkInlineRenderer.cs
// Author: Kyle L. Crowder
// Build Date: 2026/05/02



using System.Windows.Documents;

using Markdig.Syntax.Inlines;

using MarkdownViewer.Wpf.Core;




namespace MarkdownViewer.Wpf.Rendering.Inlines;





public sealed class AutoLinkInlineRenderer : IInlineRenderer<AutolinkInline>
{
    public System.Windows.Documents.Inline Render(AutolinkInline inline, IRenderContext context)
    {
        ArgumentNullException.ThrowIfNull(inline);
        ArgumentNullException.ThrowIfNull(context);

        var url = inline.IsEmail ? $"mailto:{inline.Url}" : inline.Url;
        Hyperlink hyperlink = new(new Run(inline.Url));

        if (Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out Uri? uri))
        {
            MarkdownIntegrationServices.ConfigureHyperlink(hyperlink, uri, context);
        }

        return hyperlink;
    }
}