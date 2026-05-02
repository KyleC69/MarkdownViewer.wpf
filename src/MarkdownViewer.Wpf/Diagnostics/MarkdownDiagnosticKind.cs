// Solution: MarkdownViewer.Wpf
// Project:   MarkdownViewer.Wpf
// File:         MarkdownDiagnosticKind.cs
// Author: Kyle L. Crowder
// Build Date: 2026/05/02



namespace MarkdownViewer.Wpf.Diagnostics;





public enum MarkdownDiagnosticKind
{
    ThemeKeyMissing,
    BlockRendered,
    InlineRendered,
    ImageLoadFailed,
    LinkNavigationFailed,
    HtmlInlineIgnored
}