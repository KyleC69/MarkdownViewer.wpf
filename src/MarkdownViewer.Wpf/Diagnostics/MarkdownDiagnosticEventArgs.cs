namespace MarkdownViewer.Wpf.Diagnostics;

public sealed class MarkdownDiagnosticEventArgs : EventArgs
{
    public MarkdownDiagnosticEventArgs(
        MarkdownDiagnosticKind kind,
        string message,
        Type? nodeType = null,
        string? key = null,
        TimeSpan? duration = null,
        Exception? exception = null)
    {
        Kind = kind;
        Message = message ?? throw new ArgumentNullException(nameof(message));
        NodeType = nodeType;
        Key = key;
        Duration = duration;
        Exception = exception;
    }

    public TimeSpan? Duration { get; }

    public Exception? Exception { get; }

    public string? Key { get; }

    public MarkdownDiagnosticKind Kind { get; }

    public string Message { get; }

    public Type? NodeType { get; }
}