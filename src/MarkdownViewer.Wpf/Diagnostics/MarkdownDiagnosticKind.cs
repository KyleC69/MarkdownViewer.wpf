namespace MarkdownViewer.Wpf.Diagnostics;

public enum MarkdownDiagnosticKind
{
    ThemeKeyMissing,
    BlockRendered,
    InlineRendered,
    ImageLoadFailed,
    LinkNavigationFailed,
    HtmlInlineIgnored,
}