// Build Date: 2026/04/15
// Solution: Markdig.Wpf
// Project:   MarkdownViewer.Wpf
// File:         DelimiterInlineRenderer.cs
// Author: Kyle L. Crowder
// Build Num: 184534



using Markdig.Syntax.Inlines;




namespace MarkdownViewer.Wpf.Rendering.Xaml.Defaults.Inlines
{


    /// <summary>
    ///     A XAML renderer for a <see cref="DelimiterInline" />.
    /// </summary>
    /// <seealso cref="Xaml.XamlObjectRenderer{T}" />
    public class DefaultDelimiterInlineRenderer : DefaultXamlInlineRenderer<DelimiterInline>
    {
        protected override void WriteObject(Abstractions.IXamlRenderContext renderer, DelimiterInline obj)
        {
            WriteTextRun(renderer, obj.ToLiteral());
            renderer.WriteChildren(obj);
        }
    }


}