// Build Date: 2026/04/15
// Solution: Markdig.Wpf
// Project:   MarkdownViewer.Wpf
// File:         XamlObjectRenderer.cs
// Author: Kyle L. Crowder
// Build Num: 184536



using System;

using Markdig.Renderers;
using Markdig.Syntax;

using MarkdownViewer.Wpf.Abstractions;




namespace MarkdownViewer.Wpf.Rendering.Xaml
{


    /// <summary>
    ///     A base class for XAML rendering <see cref="Block" /> and <see cref="Syntax.Inlines.Inline" /> Markdown objects.
    /// </summary>
    /// <typeparam name="TObject">The type of the object.</typeparam>
    /// <seealso cref="IMarkdownObjectRenderer" />
    public abstract class XamlObjectRenderer<TObject> : MarkdownObjectRenderer<XamlRenderer, TObject> where TObject : MarkdownObject
    {
        protected sealed override void Write(XamlRenderer renderer, TObject obj)
        {
            if (renderer == null) throw new ArgumentNullException(nameof(renderer));
            if (obj == null) throw new ArgumentNullException(nameof(obj));

            Write(new XamlRenderContext(renderer), obj);
        }








        protected abstract void Write(IXamlRenderContext renderer, TObject obj);
    }


}