// Build Date: 2026/04/15
// Solution: Markdig.Wpf
// Project:   MarkdownViewer.Wpf
// File:         LiteralInlineRenderer.cs
// Author: Kyle L. Crowder
// Build Num: 184535



using Markdig.Syntax.Inlines;




namespace MarkdownViewer.Wpf.Rendering.Xaml.Defaults.Inlines
{


    /// <summary>
    ///     A XAML renderer for a <see cref="LiteralInline" />.
    /// </summary>
    /// <seealso cref="Xaml.XamlObjectRenderer{T}" />
    public class DefaultLiteralInlineRenderer : DefaultXamlInlineRenderer<LiteralInline>
    {
        protected override void WriteObject(Abstractions.IXamlRenderContext renderer, LiteralInline obj)
        {
            if (obj.Content.IsEmpty)
                return;

            renderer.Write("<Run");
            renderer.Write(" Text=\"").WriteEscape(ref obj.Content).Write("\"");
            renderer.Write(" />");
        }
    }


}