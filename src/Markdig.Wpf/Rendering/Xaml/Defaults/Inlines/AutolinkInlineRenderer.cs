// Build Date: 2026/04/15
// Solution: Markdig.Wpf
// Project:   MarkdownViewer.Wpf
// File:         AutolinkInlineRenderer.cs
// Author: Kyle L. Crowder
// Build Num: 184534



using Markdig.Syntax.Inlines;




namespace MarkdownViewer.Wpf.Rendering.Xaml.Defaults.Inlines
{


    /// <summary>
    ///     A XAML renderer for a <see cref="AutolinkInline" />.
    /// </summary>
    /// <seealso cref="Xaml.XamlObjectRenderer{T}" />
    public class DefaultAutolinkInlineRenderer : DefaultXamlInlineRenderer<AutolinkInline>
    {
        protected override void WriteObject(Abstractions.IXamlRenderContext renderer, AutolinkInline obj)
        {
            var url = obj.Url;
            if (obj.IsEmail)
            {
                url = "mailto:" + url;
            }

            renderer.Write("<Hyperlink");
            WriteStyleAttribute(renderer, nameof(Styles.HyperlinkStyleKey));
            WriteCommandAttribute(renderer, nameof(Commands.Hyperlink));
            renderer.Write(" CommandParameter=\"").WriteEscapeUrl(url).Write("\"");
            renderer.Write(">");
            WriteTextRun(renderer, obj.Url);
            renderer.WriteLine("</Hyperlink>");
        }
    }


}