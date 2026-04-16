// Build Date: 2026/04/15
// Solution: Markdig.Wpf
// Project:   MarkdownViewer.Wpf
// File:         ParagraphRenderer.cs
// Author: Kyle L. Crowder
// Build Num: 184533



using Markdig.Syntax;




namespace MarkdownViewer.Wpf.Rendering.Xaml.Defaults.Blocks
{


    /// <summary>
    ///     A XAML renderer for a <see cref="ParagraphBlock" />.
    /// </summary>
    /// <seealso cref="Xaml.XamlObjectRenderer{T}" />
    public class DefaultParagraphRenderer : DefaultXamlBlockRenderer<ParagraphBlock>
    {
        protected override void WriteObject(Abstractions.IXamlRenderContext renderer, ParagraphBlock obj)
        {
            if (!renderer.IsFirstInContainer)
            {
                renderer.EnsureLine();
            }

            renderer.Write("<Paragraph");
            WriteStyleAttribute(renderer, nameof(Styles.ParagraphStyleKey));
            renderer.WriteLine(">");
            renderer.WriteLeafInline(obj);
            renderer.EnsureLine();
            renderer.WriteLine("</Paragraph>");
        }
    }


}