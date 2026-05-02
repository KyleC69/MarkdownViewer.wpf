// Solution: MarkdownViewer.Wpf
// Project:   MarkdownViewer.Wpf
// File:         IPostProcessor.cs
// Author: Kyle L. Crowder
// Build Date: 2026/05/02



using System.Windows;




namespace MarkdownViewer.Wpf.Core;





public interface IPostProcessor
{
    void Process(UIElement root, IRenderContext context);
}