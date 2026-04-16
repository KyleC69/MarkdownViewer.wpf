// Build Date: 2026/04/15
// Solution: Markdig.Wpf
// Project:   MarkdownViewer.Wpf
// File:         XamlRenderer.cs
// Author: Kyle L. Crowder
// Build Num: 184537

// Some parts taken from https://github.com/lunet-io/markdig
// Copyright (c) Alexandre Mutel. All rights reserved.



using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

using Markdig.Helpers;
using Markdig.Renderers;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

using MarkdownViewer.Wpf.Abstractions;




namespace MarkdownViewer.Wpf.Rendering.Xaml
{


    /// <summary>
    ///     XAML renderer for a Markdown <see cref="MarkdownDocument" /> object.
    /// </summary>
    /// <seealso cref="Renderers.TextRendererBase{T}" />
    public class XamlRenderer : TextRendererBase<XamlRenderer>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="XamlRenderer" /> class.
        /// </summary>
        /// <param name="writer">The writer.</param>
        internal XamlRenderer(TextWriter writer, IXamlRendererRegistry? rendererRegistry = null) : base(writer)
        {
            if (rendererRegistry == null)
            {
                DefaultXamlRendererRegistry.ApplyTo(this);
            }
            else
            {
                XamlRendererRegistry registrations = new();
                rendererRegistry.Configure(registrations);
                registrations.ApplyTo(this);
            }

            EnableHtmlEscape = true;
        }








        public override object Render(MarkdownObject markdownObject)
        {
            object result;
            if (markdownObject is MarkdownDocument)
            {
                this.Write("<FlowDocument");
                this.Write(" xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"");
                this.Write(" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\"");
                this.Write(" xmlns:markdig=\"clr-namespace:MarkdownViewer.Wpf;assembly=MarkdownViewer.Wpf\"");
                this.Write(" Style=\"{DynamicResource {x:Static markdig:Styles.DocumentStyleKey}}\"");
                this.WriteLine(">");
                result = base.Render(markdownObject);
                this.Write("</FlowDocument>");
            }
            else
            {
                result = base.Render(markdownObject);
            }

            return result;
        }








        internal bool EnableHtmlEscape { get; set; }








        /// <summary>
        ///     Writes the content escaped for XAML.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <returns>This instance</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal XamlRenderer WriteEscape(string? content, bool softEscape = false)
        {
            if (string.IsNullOrEmpty(content))
            {
                return this;
            }

            WriteEscape(content, 0, content.Length, softEscape);
            return this;
        }








        /// <summary>
        ///     Writes the content escaped for XAML.
        /// </summary>
        /// <param name="slice">The slice.</param>
        /// <param name="softEscape">Only escape &lt; and &amp;</param>
        /// <returns>This instance</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal XamlRenderer WriteEscape(ref StringSlice slice, bool softEscape = false)
        {
            if (slice.Start > slice.End)
            {
                return this;
            }

            return WriteEscape(slice.Text, slice.Start, slice.Length, softEscape);
        }








        /// <summary>
        ///     Writes the content escaped for XAML.
        /// </summary>
        /// <param name="slice">The slice.</param>
        /// <param name="softEscape">Only escape &lt; and &amp;</param>
        /// <returns>This instance</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal XamlRenderer WriteEscape(StringSlice slice, bool softEscape = false)
        {
            return WriteEscape(ref slice, softEscape);
        }








        /// <summary>
        ///     Writes the content escaped for XAML.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="length">The length.</param>
        /// <param name="softEscape">Only escape &lt; and &amp;</param>
        /// <returns>This instance</returns>
        internal XamlRenderer WriteEscape(string? content, int offset, int length, bool softEscape = false)
        {
            if (string.IsNullOrEmpty(content) || length == 0)
            {
                return this;
            }

            var end = offset + length;
            var previousOffset = offset;
            for (; offset < end; offset++)
                switch (content[offset])
                {
                    case '<':
                        this.Write(content, previousOffset, offset - previousOffset);
                        if (EnableHtmlEscape)
                        {
                            this.Write("&lt;");
                        }

                        previousOffset = offset + 1;
                        break;

                    case '>':
                        if (!softEscape)
                        {
                            this.Write(content, previousOffset, offset - previousOffset);
                            if (EnableHtmlEscape)
                            {
                                this.Write("&gt;");
                            }

                            previousOffset = offset + 1;
                        }

                        break;

                    case '&':
                        this.Write(content, previousOffset, offset - previousOffset);
                        if (EnableHtmlEscape)
                        {
                            this.Write("&amp;");
                        }

                        previousOffset = offset + 1;
                        break;

                    case '"':
                        if (!softEscape)
                        {
                            this.Write(content, previousOffset, offset - previousOffset);
                            if (EnableHtmlEscape)
                            {
                                this.Write("&quot;");
                            }

                            previousOffset = offset + 1;
                        }

                        break;
                }

            this.Write(content, previousOffset, end - previousOffset);
            return this;
        }








        /// <summary>
        ///     Writes the URL escaped for XAML.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <returns>This instance</returns>
        internal XamlRenderer WriteEscapeUrl(string? content)
        {
            if (content == null)
            {
                return this;
            }

            var previousPosition = 0;
            var length = content.Length;

            for (var i = 0; i < length; i++)
            {
                var c = content[i];

                if (c < 128)
                {
                    var escape = HtmlHelper.EscapeUrlCharacter(c);
                    if (escape != null)
                    {
                        this.Write(content, previousPosition, i - previousPosition);
                        previousPosition = i + 1;
                        this.Write(escape);
                    }
                }
                else
                {
                    this.Write(content, previousPosition, i - previousPosition);
                    previousPosition = i + 1;

                    byte[] bytes;
                    if (c >= '\ud800' && c <= '\udfff' && previousPosition < length)
                    {
                        bytes = Encoding.UTF8.GetBytes(new[] { c, content[previousPosition] });
                        // Skip next char as it is decoded above
                        i++;
                        previousPosition = i + 1;
                    }
                    else
                    {
                        bytes = Encoding.UTF8.GetBytes(new[] { c });
                    }

                    foreach (var t in bytes) this.Write($"%{t:X2}");
                }
            }

            this.Write(content, previousPosition, length - previousPosition);
            return this;
        }








        internal new void WriteLeafInline(LeafBlock leafBlock)
        {
            if (leafBlock == null)
            {
                throw new ArgumentNullException(nameof(leafBlock));
            }

            Inline? inline = leafBlock.Inline?.FirstChild;
            while (inline != null)
            {
                this.Write(inline);
                inline = inline.NextSibling;
            }
        }








        /// <summary>
        ///     Writes the lines of a <see cref="LeafBlock" />
        /// </summary>
        /// <param name="leafBlock">The leaf block.</param>
        /// <param name="writeEndOfLines">if set to <c>true</c> write end of lines.</param>
        /// <param name="escape">if set to <c>true</c> escape the content for XAML</param>
        /// <param name="softEscape">Only escape &lt; and &amp;</param>
        /// <returns>This instance</returns>
        internal XamlRenderer WriteLeafRawLines(LeafBlock leafBlock, bool writeEndOfLines, bool escape, bool softEscape = false)
        {
            if (leafBlock == null)
            {
                throw new ArgumentNullException(nameof(leafBlock));
            }

            if (leafBlock.Lines.Lines != null)
            {
                StringLineGroup lines = leafBlock.Lines;
                var slices = lines.Lines;
                for (var i = 0; i < lines.Count; i++)
                {
                    if (!writeEndOfLines && i > 0)
                    {
                        this.WriteLine();
                    }

                    if (escape)
                    {
                        WriteEscape(ref slices[i].Slice, softEscape);
                    }
                    else
                    {
                        this.Write(ref slices[i].Slice);
                    }

                    if (writeEndOfLines)
                    {
                        this.WriteLine();
                    }
                }
            }

            return this;
        }
    }


}