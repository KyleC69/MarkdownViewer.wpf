// Build Date: 2026/04/15
// Solution: Markdig.Wpf
// Project:   MarkdownViewer.Wpf
// File:         Markdown.cs
// Author: Kyle L. Crowder
// Build Num: 184539



using System;
using System.IO;

using Markdig;
using Markdig.Syntax;

using MarkdownViewer.Wpf.Abstractions;
using MarkdownViewer.Wpf.Rendering.Xaml;




namespace MarkdownViewer.Wpf
{


    /// <summary>
    ///     Provides methods for parsing a Markdown string to a syntax tree and converting it to other formats.
    /// </summary>
    public static partial class Markdown
    {
        /// <summary>
        ///     Converts a Markdown string to XAML.
        /// </summary>
        /// <param name="markdown">A Markdown text.</param>
        /// <param name="pipeline">The pipeline used for the conversion.</param>
        /// <param name="rendererRegistry">The renderer registry used to configure the XAML renderer.</param>
        /// <returns>The result of the conversion</returns>
        /// <exception cref="ArgumentNullException">if markdown variable is null</exception>
        public static string ToXaml(string markdown, MarkdownPipeline? pipeline = null, IXamlRendererRegistry? rendererRegistry = null)
        {
            if (markdown == null) throw new ArgumentNullException(nameof(markdown));
            using (StringWriter writer = new())
            {
                ToXaml(markdown, writer, pipeline, rendererRegistry);
                return writer.ToString();
            }
        }








        /// <summary>
        ///     Converts a Markdown string to XAML and output to the specified writer.
        /// </summary>
        /// <param name="markdown">A Markdown text.</param>
        /// <param name="writer">The destination <see cref="TextWriter" /> that will receive the result of the conversion.</param>
        /// <param name="pipeline">The pipeline used for the conversion.</param>
        /// <param name="rendererRegistry">The renderer registry used to configure the XAML renderer.</param>
        /// <returns>The Markdown document that has been parsed</returns>
        /// <exception cref="ArgumentNullException">if reader or writer variable are null</exception>
        public static MarkdownDocument ToXaml(string markdown, TextWriter writer, MarkdownPipeline? pipeline = null, IXamlRendererRegistry? rendererRegistry = null)
        {
            if (markdown == null)
            {
                throw new ArgumentNullException(nameof(markdown));
            }

            if (writer == null)
            {
                throw new ArgumentNullException(nameof(writer));
            }

            pipeline = pipeline ?? new MarkdownPipelineBuilder().Build();

            XamlRenderer renderer = new(writer, rendererRegistry);
            pipeline.Setup(renderer);

            MarkdownDocument? document = Markdig.Markdown.Parse(markdown, pipeline);
            renderer.Render(document);
            writer.Flush();

            return document;
        }
    }


}