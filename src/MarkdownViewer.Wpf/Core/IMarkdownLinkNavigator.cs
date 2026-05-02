// Solution: MarkdownViewer.Wpf
// Project:   MarkdownViewer.Wpf
// File:         IMarkdownLinkNavigator.cs
// Author: Kyle L. Crowder
// Build Date: 2026/05/02



namespace MarkdownViewer.Wpf.Core;





public interface IMarkdownLinkNavigator
{
    bool TryNavigate(Uri uri, IRenderContext context);
}