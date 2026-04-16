using System.Windows;
using System.Windows.Media;

using MarkdownViewer.Wpf.Diagnostics;

namespace MarkdownViewer.Wpf.Theming;

public sealed class CompositeTheme : ITheme
{
    private readonly IReadOnlyList<ITheme> themes;

    public CompositeTheme(params ITheme[] themes)
    {
        ArgumentNullException.ThrowIfNull(themes);
        this.themes = themes;
    }

    public Brush? GetBrush(string key)
    {
        Brush? brush = themes.Select(theme => theme.GetBrush(key)).FirstOrDefault(candidate => candidate is not null);
        if (brush is null)
        {
            MarkdownDiagnostics.ReportThemeKeyMissing(GetType(), key);
        }

        return brush;
    }

    public double? GetFontSize(string key)
    {
        double? fontSize = themes.Select(theme => theme.GetFontSize(key)).FirstOrDefault(size => size is not null);
        if (fontSize is null)
        {
            MarkdownDiagnostics.ReportThemeKeyMissing(GetType(), key);
        }

        return fontSize;
    }

    public Style? GetStyle(string key)
    {
        Style? style = themes.Select(theme => theme.GetStyle(key)).FirstOrDefault(candidate => candidate is not null);
        if (style is null)
        {
            MarkdownDiagnostics.ReportThemeKeyMissing(GetType(), key);
        }

        return style;
    }

    public Thickness? GetThickness(string key)
    {
        Thickness? thickness = themes.Select(theme => theme.GetThickness(key)).FirstOrDefault(candidate => candidate is not null);
        if (thickness is null)
        {
            MarkdownDiagnostics.ReportThemeKeyMissing(GetType(), key);
        }

        return thickness;
    }
}