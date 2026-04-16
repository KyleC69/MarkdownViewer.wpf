// Build Date: 2026/04/15
// Solution: Markdig.Wpf
// Project:   MarkdownViewer.Wpf
// File:         DefaultXamlRendererRegistry.cs
// Author: Kyle L. Crowder
// Build Num: 184536



using MarkdownViewer.Wpf.Rendering.Xaml.Defaults.Blocks;
using MarkdownViewer.Wpf.Rendering.Xaml.Defaults.Inlines;




namespace MarkdownViewer.Wpf.Rendering.Xaml
{


    /// <summary>
    ///     Registers the package default XAML renderer implementations.
    /// </summary>
    internal static class DefaultXamlRendererRegistry
    {
        private static readonly XamlRendererRegistry registry = CreateRegistry();








        internal static void ApplyTo(XamlRenderer renderer)
        {
            registry.ApplyTo(renderer);
        }








        /// <summary>
        ///     Creates a mutable registry preloaded with the package default renderers.
        /// </summary>
        internal static XamlRendererRegistry CreateMutable()
        {
            return CreateRegistry();
        }








        private static XamlRendererRegistry CreateRegistry()
        {
            return new XamlRendererRegistry().Add<DefaultCodeBlockRenderer>().Add<DefaultListRenderer>().Add<DefaultHeadingRenderer>().Add<DefaultHtmlBlockRenderer>().Add<DefaultParagraphRenderer>().Add<DefaultQuoteBlockRenderer>().Add<DefaultThematicBreakRenderer>().Add<DefaultTableRenderer>().Add<DefaultAutolinkInlineRenderer>().Add<DefaultCodeInlineRenderer>().Add<DefaultDelimiterInlineRenderer>().Add<DefaultEmphasisInlineRenderer>().Add<DefaultLineBreakInlineRenderer>().Add<DefaultHtmlInlineRenderer>().Add<DefaultHtmlEntityInlineRenderer>().Add<DefaultLinkInlineRenderer>().Add<DefaultLiteralInlineRenderer>().Add<DefaultTaskListRenderer>();
        }
    }


}