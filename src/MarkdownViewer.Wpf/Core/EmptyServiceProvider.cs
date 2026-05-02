// Solution: MarkdownViewer.Wpf
// Project:   MarkdownViewer.Wpf
// File:         EmptyServiceProvider.cs
// Author: Kyle L. Crowder
// Build Date: 2026/05/02



namespace MarkdownViewer.Wpf.Core;





internal sealed class EmptyServiceProvider : IServiceProvider
{

    private EmptyServiceProvider()
    {
    }








    public static EmptyServiceProvider Instance { get; } = new();








    public object? GetService(Type serviceType)
    {
        return null;
    }
}