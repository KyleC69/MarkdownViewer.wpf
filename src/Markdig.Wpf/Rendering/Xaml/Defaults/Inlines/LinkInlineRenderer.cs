// Build Date: 2026/04/15
// Solution: Markdig.Wpf
// Project:   MarkdownViewer.Wpf
// File:         LinkInlineRenderer.cs
// Author: Kyle L. Crowder
// Build Num: 184535



using Markdig.Syntax.Inlines;




namespace MarkdownViewer.Wpf.Rendering.Xaml.Defaults.Inlines
{


    /// <summary>
    ///     A XAML renderer for a <see cref="LinkInline" />.
    /// </summary>
    /// <seealso cref="Xaml.XamlObjectRenderer{T}" />
    public class DefaultLinkInlineRenderer : DefaultXamlInlineRenderer<LinkInline>
    {
        protected override void WriteObject(Abstractions.IXamlRenderContext renderer, LinkInline obj)
        {
            var url = obj.GetDynamicUrl?.Invoke() ?? obj.Url;

            if (obj.IsImage)
            {
                renderer.Write("<Image");
                WriteStyleAttribute(renderer, nameof(Styles.ImageStyleKey));
                renderer.WriteLine(">");
                renderer.WriteLine("<Image.Source>");
                renderer.Write("<BitmapImage");
                renderer.Write(" UriSource=\"").WriteEscapeUrl(url).Write("\"");
                renderer.WriteLine(" />");
                renderer.WriteLine("</Image.Source>");
                renderer.WriteLine("</Image>");
            }
            else
            {
                renderer.Write("<Hyperlink");
                WriteStyleAttribute(renderer, nameof(Styles.HyperlinkStyleKey));
                WriteCommandAttribute(renderer, nameof(Commands.Hyperlink));
                renderer.Write(" CommandParameter=\"").WriteEscapeUrl(url).Write("\"");
                if (!string.IsNullOrEmpty(obj.Title))
                    renderer.Write(" ToolTip=\"").Write(obj.Title).Write("\"");
                renderer.WriteLine(">");
                renderer.WriteChildren(obj);
                renderer.EnsureLine();
                renderer.WriteLine("</Hyperlink>");
            }
        }
    }


}