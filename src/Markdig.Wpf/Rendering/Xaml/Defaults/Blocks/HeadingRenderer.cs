// Build Date: 2026/04/15
// Solution: Markdig.Wpf
// Project:   MarkdownViewer.Wpf
// File:         HeadingRenderer.cs
// Author: Kyle L. Crowder
// Build Num: 184533



using Markdig.Syntax;




namespace MarkdownViewer.Wpf.Rendering.Xaml.Defaults.Blocks
{


    /// <summary>
    ///     An XAML renderer for a <see cref="HeadingBlock" />.
    /// </summary>
    /// <seealso cref="Xaml.XamlObjectRenderer{T}" />
    public class DefaultHeadingRenderer : DefaultXamlBlockRenderer<HeadingBlock>
    {
        protected override void WriteObject(Abstractions.IXamlRenderContext renderer, HeadingBlock obj)
        {
            renderer.Write("<Paragraph");
            if (obj.Level > 0 && obj.Level <= 6)
            {
                WriteStyleAttribute(renderer, $"Heading{obj.Level}StyleKey");
            }

            renderer.WriteLine(">");
            renderer.WriteLeafInline(obj);
            renderer.EnsureLine();
            renderer.WriteLine("</Paragraph>");
        }
    }


}