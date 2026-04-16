using System.Windows;
using System.Windows.Media;

namespace MarkdownViewer.Wpf.Theming;

public interface ITheme
{
    Style? GetStyle(string key);

    Brush? GetBrush(string key);

    Thickness? GetThickness(string key);

    double? GetFontSize(string key);
}