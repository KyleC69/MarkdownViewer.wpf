// Build Date: 2026/04/15
// Solution: Markdig.Wpf
// Project:   MarkdownViewer.Wpf
// File:         ThematicBreakRenderer.cs
// Author: Kyle L. Crowder
// Build Num: 184534



using Markdig.Syntax;




namespace MarkdownViewer.Wpf.Rendering.Xaml.Defaults.Blocks
{


    /// <summary>
    ///     A XAML renderer for a <see cref="ThematicBreakBlock" />.
    /// </summary>
    /// <seealso cref="Xaml.XamlObjectRenderer{T}" />
    public class DefaultThematicBreakRenderer : DefaultXamlBlockRenderer<ThematicBreakBlock>
    {
        protected override void WriteObject(Abstractions.IXamlRenderContext renderer, ThematicBreakBlock obj)
        {
            renderer.EnsureLine();

            renderer.WriteLine("<Paragraph>");
            renderer.Write("<Line X2=\"1\"");
            WriteStyleAttribute(renderer, nameof(Styles.ThematicBreakStyleKey));
            renderer.WriteLine(" />");
            renderer.WriteLine("</Paragraph>");
        }
    }


}