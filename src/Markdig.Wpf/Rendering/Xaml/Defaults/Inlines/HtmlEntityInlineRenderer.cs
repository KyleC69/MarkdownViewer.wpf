// Build Date: 2026/04/15
// Solution: Markdig.Wpf
// Project:   MarkdownViewer.Wpf
// File:         HtmlEntityInlineRenderer.cs
// Author: Kyle L. Crowder
// Build Num: 184535



using Markdig.Helpers;
using Markdig.Syntax.Inlines;




namespace MarkdownViewer.Wpf.Rendering.Xaml.Defaults.Inlines
{


    /// <summary>
    ///     A XAML renderer for a <see cref="HtmlEntityInline" />.
    /// </summary>
    /// <seealso cref="Xaml.XamlObjectRenderer{T}" />
    public class DefaultHtmlEntityInlineRenderer : DefaultXamlInlineRenderer<HtmlEntityInline>
    {
        protected override void WriteObject(Abstractions.IXamlRenderContext renderer, HtmlEntityInline obj)
        {
            StringSlice transcoded = obj.Transcoded;
            WriteTextRun(renderer, ref transcoded);
        }
    }


}