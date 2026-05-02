// Solution: MarkdownViewer.Wpf
// Project:   MarkdownViewer.Wpf
// File:         IMarkdownImageSourceResolver.cs
// Author: Kyle L. Crowder
// Build Date: 2026/05/02



using System.Windows.Media;




namespace MarkdownViewer.Wpf.Core;





public interface IMarkdownImageSourceResolver
{
    ImageSource? ResolveImageSource(Uri uri, IRenderContext context);
}