using System.Diagnostics;
using System.Windows;

using MarkdownViewer.Wpf.Diagnostics;

namespace MarkdownViewer.WPF.Sample;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        MarkdownDiagnostics.Emitted += OnMarkdownDiagnosticEmitted;
    }

    protected override void OnExit(ExitEventArgs e)
    {
        MarkdownDiagnostics.Emitted -= OnMarkdownDiagnosticEmitted;
        base.OnExit(e);
    }

    private static void OnMarkdownDiagnosticEmitted(object? sender, MarkdownDiagnosticEventArgs args)
    {
        string prefix = args.Kind switch
        {
            MarkdownDiagnosticKind.ThemeKeyMissing      => "[MarkdownDiag][THEME]",
            MarkdownDiagnosticKind.ImageLoadFailed      => "[MarkdownDiag][IMAGE]",
            MarkdownDiagnosticKind.LinkNavigationFailed => "[MarkdownDiag][LINK]",
            MarkdownDiagnosticKind.HtmlInlineIgnored    => "[MarkdownDiag][HTML]",
            MarkdownDiagnosticKind.BlockRendered        => "[MarkdownDiag][BLOCK]",
            MarkdownDiagnosticKind.InlineRendered       => "[MarkdownDiag][INLINE]",
            _                                           => "[MarkdownDiag]",
        };

        Debug.WriteLine($"{prefix} {args.Message}");

        if (args.Exception is not null)
        {
            Debug.WriteLine($"{prefix} Exception: {args.Exception}");
        }
    }
}

