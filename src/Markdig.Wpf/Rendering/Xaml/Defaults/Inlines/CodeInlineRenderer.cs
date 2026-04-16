// Build Date: 2026/04/15
// Solution: Markdig.Wpf
// Project:   MarkdownViewer.Wpf
// File:         CodeInlineRenderer.cs
// Author: Kyle L. Crowder
// Build Num: 184534



using Markdig.Syntax.Inlines;




namespace MarkdownViewer.Wpf.Rendering.Xaml.Defaults.Inlines
{


    /// <summary>
    ///     A XAML renderer for a <see cref="CodeInline" />.
    /// </summary>
    /// <seealso cref="Xaml.XamlObjectRenderer{T}" />
    public class DefaultCodeInlineRenderer : DefaultXamlInlineRenderer<CodeInline>
    {
        protected override void WriteObject(Abstractions.IXamlRenderContext renderer, CodeInline obj)
        {
            renderer.Write("<Run");
            WriteStyleAttribute(renderer, nameof(Styles.CodeStyleKey));
            renderer.Write(" Text=\"").WriteEscape(obj.Content).Write("\"");
            renderer.Write(" />");
        }
    }


}