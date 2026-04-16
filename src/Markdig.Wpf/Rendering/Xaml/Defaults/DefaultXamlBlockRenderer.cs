// Build Date: 2026/04/15
// Solution: Markdig.Wpf
// Project:   MarkdownViewer.Wpf
// File:         DefaultXamlBlockRenderer.cs
// Author: Kyle L. Crowder
// Build Num: 184535



using Markdig.Syntax;




namespace MarkdownViewer.Wpf.Rendering.Xaml.Defaults
{


    /// <summary>
    ///     Base class for default block renderers.
    /// </summary>
    /// <typeparam name="TBlock">The block type rendered by the implementation.</typeparam>
    public abstract class DefaultXamlBlockRenderer<TBlock> : DefaultXamlObjectRenderer<TBlock> where TBlock : Block
    {
    }


}