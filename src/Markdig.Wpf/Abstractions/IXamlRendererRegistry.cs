// Build Date: 2026/04/15
// Solution: Markdig.Wpf
// Project:   MarkdownViewer.Wpf
// File:         IXamlRendererRegistry.cs
// Author: Kyle L. Crowder
// Build Num: 184532



using MarkdownViewer.Wpf.Rendering.Xaml;




namespace MarkdownViewer.Wpf.Abstractions
{


    /// <summary>
    ///     Configures a <see cref="XamlRendererRegistry" /> with the renderers required for a render session.
    /// </summary>
    public interface IXamlRendererRegistry
    {
        /// <summary>
        ///     Adds the configured registrations to the provided registry instance.
        /// </summary>
        /// <param name="registry">The registry to configure.</param>
        void Configure(XamlRendererRegistry registry);
    }


}