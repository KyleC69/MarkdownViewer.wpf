// Build Date: 2026/04/15
// Solution: Markdig.Wpf
// Project:   MarkdownViewer.Wpf
// File:         IXamlRenderContext.cs
// Author: Kyle L. Crowder
// Build Num: 184532



using Markdig.Helpers;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;




namespace MarkdownViewer.Wpf.Abstractions
{


    /// <summary>
    ///     Exposes the stable XAML emission operations available to node renderers.
    /// </summary>
    public interface IXamlRenderContext
    {
        /// <summary>
        ///     Gets a value indicating whether the current node is the first item in its container.
        /// </summary>
        bool IsFirstInContainer { get; }








        /// <summary>
        ///     Ensures subsequent content starts on a new line.
        /// </summary>
        IXamlRenderContext EnsureLine();








        /// <summary>
        ///     Writes raw XAML content.
        /// </summary>
        IXamlRenderContext Write(string? content);








        /// <summary>
        ///     Renders the children of the specified markdown object.
        /// </summary>
        void WriteChildren(ContainerBlock markdownObject);








        /// <summary>
        ///     Renders the children of the specified inline container.
        /// </summary>
        void WriteChildren(ContainerInline markdownObject);








        /// <summary>
        ///     Writes escaped XAML text.
        /// </summary>
        IXamlRenderContext WriteEscape(string? content, bool softEscape = false);








        /// <summary>
        ///     Writes escaped XAML text from a string slice.
        /// </summary>
        IXamlRenderContext WriteEscape(ref StringSlice slice, bool softEscape = false);








        /// <summary>
        ///     Writes an escaped URL value.
        /// </summary>
        IXamlRenderContext WriteEscapeUrl(string? content);








        /// <summary>
        ///     Renders the inline children of the specified leaf block.
        /// </summary>
        void WriteLeafInline(LeafBlock leafBlock);








        /// <summary>
        ///     Writes a line terminator.
        /// </summary>
        IXamlRenderContext WriteLine();








        /// <summary>
        ///     Writes content followed by a line terminator.
        /// </summary>
        IXamlRenderContext WriteLine(string? content);
    }


}