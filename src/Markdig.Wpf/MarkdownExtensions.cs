// Build Date: 2026/04/15
// Solution: Markdig.Wpf
// Project:   MarkdownViewer.Wpf
// File:         MarkdownExtensions.cs
// Author: Kyle L. Crowder
// Build Num: 184539



using System;

using Markdig;




// ReSharper disable once CheckNamespace
namespace MarkdownViewer.Wpf
{


    /// <summary>
    ///     Provides extension methods for <see cref="MarkdownPipeline" /> to enable several Markdown extensions.
    /// </summary>
    public static class MarkdownExtensions
    {
        /// <summary>
        ///     Uses all extensions supported by <c>MarkdownViewer.Wpf</c>.
        /// </summary>
        /// <param name="pipeline">The pipeline.</param>
        /// <returns>The modified pipeline</returns>
        public static MarkdownPipelineBuilder UseSupportedExtensions(this MarkdownPipelineBuilder pipeline)
        {
            if (pipeline == null) throw new ArgumentNullException(nameof(pipeline));
            return pipeline.UseEmphasisExtras().UseGridTables().UsePipeTables().UseTaskLists().UseAutoLinks();
        }
    }


}