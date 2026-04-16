// Build Date: 2026/04/15
// Solution: Markdig.Wpf
// Project:   MarkdownViewer.Wpf
// File:         HtmlBlockRenderer.cs
// Author: Kyle L. Crowder
// Build Num: 184533



using Markdig.Syntax;




namespace MarkdownViewer.Wpf.Rendering.Xaml.Defaults.Blocks
{


    /// <summary>
    ///     A XAML renderer for a <see cref="HtmlBlock" />.
    /// </summary>
    /// <seealso cref="Xaml.XamlObjectRenderer{T}" />
    public class DefaultHtmlBlockRenderer : DefaultXamlBlockRenderer<HtmlBlock>
    {
        protected override void WriteObject(Abstractions.IXamlRenderContext renderer, HtmlBlock obj)
        {
            renderer.EnsureLine();
            renderer.Write("<Paragraph xml:space=\"preserve\"");
            WriteStyleAttribute(renderer, nameof(Styles.CodeBlockStyleKey));
            renderer.WriteLine(">");
            WriteLeafTextInlines(renderer, obj, softEscape: true);
            renderer.WriteLine("</Paragraph>");
        }
    }


}