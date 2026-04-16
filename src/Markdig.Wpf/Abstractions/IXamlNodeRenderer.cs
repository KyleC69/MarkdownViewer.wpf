// Build Date: 2026/04/15
// Solution: Markdig.Wpf
// Project:   MarkdownViewer.Wpf
// File:         IXamlNodeRenderer.cs
// Author: Kyle L. Crowder
// Build Num: 184531



using Markdig.Syntax;




namespace MarkdownViewer.Wpf.Abstractions
{


    /// <summary>
    ///     Marker interface for XAML renderers that handle a specific markdown node type.
    /// </summary>
    /// <typeparam name="TNode">The markdown node type handled by the renderer.</typeparam>
    internal interface IXamlNodeRenderer<in TNode> where TNode : MarkdownObject
    {
    }


}