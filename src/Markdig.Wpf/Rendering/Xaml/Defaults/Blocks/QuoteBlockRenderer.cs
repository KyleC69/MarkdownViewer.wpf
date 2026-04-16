// Build Date: 2026/04/15
// Solution: Markdig.Wpf
// Project:   MarkdownViewer.Wpf
// File:         QuoteBlockRenderer.cs
// Author: Kyle L. Crowder
// Build Num: 184534



using Markdig.Syntax;




namespace MarkdownViewer.Wpf.Rendering.Xaml.Defaults.Blocks
{


    /// <summary>
    ///     A XAML renderer for a <see cref="QuoteBlock" />.
    /// </summary>
    /// <seealso cref="Xaml.XamlObjectRenderer{T}" />
    public class DefaultQuoteBlockRenderer : DefaultXamlBlockRenderer<QuoteBlock>
    {
        protected override void WriteObject(Abstractions.IXamlRenderContext renderer, QuoteBlock obj)
        {
            renderer.EnsureLine();

            renderer.Write("<Section");
            WriteStyleAttribute(renderer, nameof(Styles.QuoteBlockStyleKey));
            renderer.WriteLine(">");
            renderer.WriteChildren(obj);
            renderer.WriteLine("</Section>");
        }
    }


}