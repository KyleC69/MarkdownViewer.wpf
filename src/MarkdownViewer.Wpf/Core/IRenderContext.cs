// Solution: MarkdownViewer.Wpf
// Project:   MarkdownViewer.Wpf
// File:         IRenderContext.cs
// Author: Kyle L. Crowder
// Build Date: 2026/05/02



using System.Windows;
using System.Windows.Documents;




namespace MarkdownViewer.Wpf.Core;





public interface IRenderContext
{
    ResourceDictionary Resources { get; }

    IServiceProvider Services { get; }

    ResourceDictionary ThemeResources { get; }


    void AddPostProcessor(IPostProcessor processor);


    UIElement RenderBlock(Markdig.Syntax.Block block);


    Inline RenderInline(Markdig.Syntax.Inlines.Inline inline);
}