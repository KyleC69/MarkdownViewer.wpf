// Build Date: 2026/04/15
// Solution: Markdig.Wpf
// Project:   MarkdownViewer.Wpf
// File:         XamlRenderContext.cs
// Author: Kyle L. Crowder
// Build Num: 184537



using Markdig.Helpers;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

using MarkdownViewer.Wpf.Abstractions;




namespace MarkdownViewer.Wpf.Rendering.Xaml
{


    internal sealed class XamlRenderContext : IXamlRenderContext
    {
        private readonly XamlRenderer renderer;








        public XamlRenderContext(XamlRenderer renderer)
        {
            this.renderer = renderer;
        }








        public IXamlRenderContext EnsureLine()
        {
            renderer.EnsureLine();
            return this;
        }








        public bool IsFirstInContainer
        {
            get { return renderer.IsFirstInContainer; }
        }








        public IXamlRenderContext Write(string? content)
        {
            renderer.Write(content);
            return this;
        }








        public void WriteChildren(ContainerBlock markdownObject)
        {
            renderer.WriteChildren(markdownObject);
        }








        public void WriteChildren(ContainerInline markdownObject)
        {
            renderer.WriteChildren(markdownObject);
        }








        public IXamlRenderContext WriteEscape(string? content, bool softEscape = false)
        {
            renderer.WriteEscape(content, softEscape);
            return this;
        }








        public IXamlRenderContext WriteEscape(ref StringSlice slice, bool softEscape = false)
        {
            renderer.WriteEscape(ref slice, softEscape);
            return this;
        }








        public IXamlRenderContext WriteEscapeUrl(string? content)
        {
            renderer.WriteEscapeUrl(content);
            return this;
        }








        public void WriteLeafInline(LeafBlock leafBlock)
        {
            renderer.WriteLeafInline(leafBlock);
        }








        public IXamlRenderContext WriteLine()
        {
            renderer.WriteLine();
            return this;
        }








        public IXamlRenderContext WriteLine(string? content)
        {
            renderer.WriteLine(content);
            return this;
        }
    }


}