// Build Date: 2026/04/15
// Solution: Markdig.Wpf
// Project:   MarkdownViewer.Wpf
// File:         HtmlInlineRenderer.cs
// Author: Kyle L. Crowder
// Build Num: 184535



using Markdig.Syntax.Inlines;




namespace MarkdownViewer.Wpf.Rendering.Xaml.Defaults.Inlines
{


    /// <summary>
    ///     A XAML renderer for a <see cref="HtmlInline" />.
    /// </summary>
    /// <seealso cref="Xaml.XamlObjectRenderer{T}" />
    public class DefaultHtmlInlineRenderer : DefaultXamlInlineRenderer<HtmlInline>
    {
        protected override void WriteObject(Abstractions.IXamlRenderContext renderer, HtmlInline obj)
        {
            if (!string.IsNullOrEmpty(obj.Tag))
            {
                WriteTextRun(renderer, obj.Tag, softEscape: true);
            }
        }
    }


}