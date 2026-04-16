// Build Date: 2026/04/15
// Solution: Markdig.Wpf
// Project:   MarkdownViewer.Wpf
// File:         LineBreakInlineRenderer.cs
// Author: Kyle L. Crowder
// Build Num: 184535



using Markdig.Syntax.Inlines;




namespace MarkdownViewer.Wpf.Rendering.Xaml.Defaults.Inlines
{


    /// <summary>
    ///     A XAML renderer for a <see cref="LineBreakInline" />.
    /// </summary>
    /// <seealso cref="Xaml.XamlObjectRenderer{T}" />
    public class DefaultLineBreakInlineRenderer : DefaultXamlInlineRenderer<LineBreakInline>
    {
        protected override void WriteObject(Abstractions.IXamlRenderContext renderer, LineBreakInline obj)
        {
            if (obj.IsHard)
            {
                renderer.WriteLine("<LineBreak />");
            }
            else
            {
                renderer.WriteLine();
            }
        }
    }


}