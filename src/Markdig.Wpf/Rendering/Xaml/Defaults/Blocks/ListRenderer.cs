// Build Date: 2026/04/15
// Solution: Markdig.Wpf
// Project:   MarkdownViewer.Wpf
// File:         ListRenderer.cs
// Author: Kyle L. Crowder
// Build Num: 184533



using Markdig.Syntax;




namespace MarkdownViewer.Wpf.Rendering.Xaml.Defaults.Blocks
{


    /// <summary>
    ///     A XAML renderer for a <see cref="ListBlock" />.
    /// </summary>
    /// <seealso cref="Xaml.XamlObjectRenderer{T}" />
    public class DefaultListRenderer : DefaultXamlBlockRenderer<ListBlock>
    {
        protected override void WriteObject(Abstractions.IXamlRenderContext renderer, ListBlock listBlock)
        {
            renderer.EnsureLine();

            renderer.Write("<List");
            if (listBlock.IsOrdered)
            {
                renderer.Write(" MarkerStyle=\"Decimal\"");

                if (listBlock.OrderedStart != null && listBlock.DefaultOrderedStart != listBlock.OrderedStart)
                {
                    renderer.Write(" StartIndex=\"").Write(listBlock.OrderedStart).Write("\"");
                }
            }
            else
            {
                renderer.Write(" MarkerStyle=\"Disc\"");
            }

            renderer.WriteLine(">");

            foreach (Block? item in listBlock)
            {
                ListItemBlock? listItem = (ListItemBlock)item;

                renderer.EnsureLine();
                renderer.WriteLine("<ListItem>");
                renderer.WriteChildren(listItem);
                renderer.WriteLine("</ListItem>");

            }

            renderer.WriteLine("</List>");
        }
    }


}