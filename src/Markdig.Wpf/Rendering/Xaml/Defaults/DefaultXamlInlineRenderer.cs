// Build Date: 2026/04/15
// Solution: Markdig.Wpf
// Project:   MarkdownViewer.Wpf
// File:         DefaultXamlInlineRenderer.cs
// Author: Kyle L. Crowder
// Build Num: 184536



using Markdig.Syntax.Inlines;




namespace MarkdownViewer.Wpf.Rendering.Xaml.Defaults
{


    /// <summary>
    ///     Base class for default inline renderers.
    /// </summary>
    /// <typeparam name="TInline">The inline type rendered by the implementation.</typeparam>
    public abstract class DefaultXamlInlineRenderer<TInline> : DefaultXamlObjectRenderer<TInline> where TInline : Inline
    {
    }


}