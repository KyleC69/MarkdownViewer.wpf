using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using MarkdownViewer.Wpf.Core;
using MarkdownViewer.Wpf.Theming;

using Xunit;

namespace MarkdownViewer.Wpf.Tests;

public sealed class MarkdownViewTests
{
    [StaFact]
    public void SettingMarkdown_RefreshesRenderedContent()
    {
        MarkdownView view = new();

        view.Markdown = "# Title";
        DispatcherTestHelper.Drain();

        StackPanel panel = Assert.IsType<StackPanel>(view.RenderedContent);
        Assert.Single(panel.Children);
    }

    [StaFact]
    public void ClearingMarkdown_RemovesRenderedContent()
    {
        MarkdownView view = new();
        view.Markdown = "Paragraph";
        DispatcherTestHelper.Drain();

        view.Markdown = null;
        DispatcherTestHelper.Drain();

        Assert.Null(view.RenderedContent);
    }

    [StaFact]
    public void SettingMarkdown_AppliesThemeSurfaceResources()
    {
        SolidColorBrush background = new(Colors.MidnightBlue);
        SolidColorBrush borderBrush = new(Colors.CadetBlue);
        ResourceDictionaryTheme theme = MarkdownTestHelper.CreateTheme(
            (ThemeKeys.ViewerBackgroundBrush, background),
            (ThemeKeys.ViewerBorderBrush, borderBrush),
            (ThemeKeys.ViewerBorderThickness, new Thickness(3)),
            (ThemeKeys.ParagraphStyle, new Style(typeof(TextBlock))));

        MarkdownView view = new()
        {
            Theme = theme,
            Markdown = "Paragraph",
        };

        DispatcherTestHelper.Drain();

        Assert.Same(background, view.Background);
        Assert.Same(borderBrush, view.BorderBrush);
        Assert.Equal(new Thickness(3), view.BorderThickness);
    }
}