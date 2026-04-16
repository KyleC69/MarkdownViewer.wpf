// Build Date: 2026/04/15
// Solution: Markdig.Wpf
// Project:   MarkdownViewer.Wpf
// File:         DefaultXamlObjectRenderer.cs
// Author: Kyle L. Crowder
// Build Num: 184536



using System;

using Markdig.Helpers;
using Markdig.Syntax;

using MarkdownViewer.Wpf.Abstractions;




namespace MarkdownViewer.Wpf.Rendering.Xaml.Defaults
{


    /// <summary>
    ///     Common validation and helper functionality for default XAML renderer implementations.
    /// </summary>
    /// <typeparam name="TObject">The markdown object type rendered by the implementation.</typeparam>
    public abstract class DefaultXamlObjectRenderer<TObject> : XamlObjectRenderer<TObject>, IXamlNodeRenderer<TObject> where TObject : MarkdownObject
    {
        protected sealed override void Write(IXamlRenderContext renderer, TObject obj)
        {
            if (renderer == null) throw new ArgumentNullException(nameof(renderer));
            if (obj == null) throw new ArgumentNullException(nameof(obj));

            WriteObject(renderer, obj);
        }








        protected static void WriteCommandAttribute(IXamlRenderContext renderer, string commandName)
        {
            renderer.Write(" Command=\"{x:Static markdig:Commands.").Write(commandName).Write("}\"");
        }








        protected static void WriteLeafTextInlines(IXamlRenderContext renderer, LeafBlock leafBlock, bool softEscape = false)
        {
            if (leafBlock == null) throw new ArgumentNullException(nameof(leafBlock));

            if (leafBlock.Lines.Lines == null)
            {
                return;
            }

            StringLineGroup lines = leafBlock.Lines;
            var slices = lines.Lines;
            for (var index = 0; index < lines.Count; index++)
            {
                StringSlice slice = slices[index].Slice;
                WriteTextRun(renderer, ref slice, softEscape);
                if (index < lines.Count - 1)
                {
                    renderer.WriteLine("<LineBreak />");
                }
            }
        }








        protected abstract void WriteObject(IXamlRenderContext renderer, TObject obj);








        protected static void WriteStyleAttribute(IXamlRenderContext renderer, string styleKeyName)
        {
            renderer.Write(" Style=\"{DynamicResource {x:Static markdig:Styles.").Write(styleKeyName).Write("}}\"");
        }








        protected static void WriteTextRun(IXamlRenderContext renderer, string? text, bool softEscape = false)
        {
            if (string.IsNullOrEmpty(text))
            {
                return;
            }

            renderer.Write("<Run Text=\"");
            renderer.WriteEscape(text, softEscape);
            renderer.Write("\" />");
        }








        protected static void WriteTextRun(IXamlRenderContext renderer, ref StringSlice slice, bool softEscape = false)
        {
            if (slice.Start > slice.End)
            {
                return;
            }

            renderer.Write("<Run Text=\"");
            renderer.WriteEscape(ref slice, softEscape);
            renderer.Write("\" />");
        }
    }


}