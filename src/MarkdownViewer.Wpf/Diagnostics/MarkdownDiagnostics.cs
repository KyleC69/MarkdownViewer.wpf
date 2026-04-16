using System.Diagnostics;

namespace MarkdownViewer.Wpf.Diagnostics;

public static class MarkdownDiagnostics
{
    private static readonly HashSet<string> reportedThemeKeys = [];
    private static readonly object syncRoot = new();

    public static event EventHandler<MarkdownDiagnosticEventArgs>? Emitted;

    internal static void ReportHtmlInlineIgnored(string htmlTag)
    {
        Emit(new MarkdownDiagnosticEventArgs(
            MarkdownDiagnosticKind.HtmlInlineIgnored,
            $"Ignored unsupported inline HTML tag '{htmlTag}'."));
    }

    internal static void ReportImageLoadFailure(Uri uri, Exception exception)
    {
        Emit(new MarkdownDiagnosticEventArgs(
            MarkdownDiagnosticKind.ImageLoadFailed,
            $"Failed to load markdown image '{uri}'.",
            exception: exception));
    }

    internal static void ReportLinkNavigationFailure(Uri uri, Exception exception)
    {
        Emit(new MarkdownDiagnosticEventArgs(
            MarkdownDiagnosticKind.LinkNavigationFailed,
            $"Failed to navigate markdown link '{uri}'.",
            exception: exception));
    }

    internal static void ReportNodeRendered(Type nodeType, TimeSpan duration, bool isBlock)
    {
        Emit(new MarkdownDiagnosticEventArgs(
            isBlock ? MarkdownDiagnosticKind.BlockRendered : MarkdownDiagnosticKind.InlineRendered,
            $"Rendered markdown {(isBlock ? "block" : "inline")} node {nodeType.FullName} in {duration.TotalMilliseconds:F3}ms.",
            nodeType: nodeType,
            duration: duration));
    }

    internal static void ReportThemeKeyMissing(Type themeType, string key)
    {
        string identity = $"{themeType.FullName}:{key}";
        lock (syncRoot)
        {
            if (!reportedThemeKeys.Add(identity))
            {
                return;
            }
        }

        Emit(new MarkdownDiagnosticEventArgs(
            MarkdownDiagnosticKind.ThemeKeyMissing,
            $"Theme '{themeType.FullName}' did not contain a resource for key '{key}'.",
            key: key));
    }

    private static void Emit(MarkdownDiagnosticEventArgs args)
    {
        switch (args.Kind)
        {
            case MarkdownDiagnosticKind.ThemeKeyMissing:
            case MarkdownDiagnosticKind.HtmlInlineIgnored:
                Trace.TraceWarning(args.Message);
                break;
            case MarkdownDiagnosticKind.ImageLoadFailed:
            case MarkdownDiagnosticKind.LinkNavigationFailed:
                Trace.TraceError($"{args.Message} {args.Exception}");
                break;
            default:
                Trace.TraceInformation(args.Message);
                break;
        }

        Emitted?.Invoke(null, args);
    }
}