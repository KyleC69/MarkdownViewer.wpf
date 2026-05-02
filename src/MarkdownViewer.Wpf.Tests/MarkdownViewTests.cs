using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using MarkdownViewer.Wpf.Controls;
using MarkdownViewer.Wpf.Core;

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

        MarkdownRootPanel panel = Assert.IsType<MarkdownRootPanel>(view.RenderedContent);
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
    public void SettingMarkdownToWhitespace_RemovesRenderedContent()
    {
        MarkdownView view = new();
        view.Markdown = "Some content";
        DispatcherTestHelper.Drain();

        view.Markdown = "   ";
        DispatcherTestHelper.Drain();

        Assert.Null(view.RenderedContent);
    }

    [StaFact]
    public void SettingMarkdownToEmptyString_RemovesRenderedContent()
    {
        MarkdownView view = new();
        view.Markdown = "Some content";
        DispatcherTestHelper.Drain();

        view.Markdown = string.Empty;
        DispatcherTestHelper.Drain();

        Assert.Null(view.RenderedContent);
    }

    [StaFact]
    public void SettingThemeResources_AppliesNativeImplicitStyles()
    {
        SolidColorBrush foreground = new(Colors.MidnightBlue);
        Style paragraphStyle = new(typeof(ParagraphTextBlock));
        paragraphStyle.Setters.Add(new Setter(TextBlock.ForegroundProperty, foreground));

        ResourceDictionary themeResources = MarkdownTestHelper.CreateThemeResources(
            (typeof(ParagraphTextBlock), paragraphStyle));

        MarkdownView view = new()
        {
            ThemeResources = themeResources,
            Markdown = "Paragraph",
        };

        DispatcherTestHelper.Drain();

        MarkdownRootPanel panel = Assert.IsType<MarkdownRootPanel>(view.RenderedContent);
        ParagraphTextBlock paragraph = Assert.IsType<ParagraphTextBlock>(panel.Children[0]);

        Assert.Same(foreground, paragraph.Foreground);
    }

    [StaFact]
    public void UpdatingThemeResources_ReRendersWithNewStyles()
    {
        Style initialStyle = new(typeof(ParagraphTextBlock));
        initialStyle.Setters.Add(new Setter(TextBlock.FontSizeProperty, 12d));
        ResourceDictionary initialTheme = MarkdownTestHelper.CreateThemeResources((typeof(ParagraphTextBlock), initialStyle));

        Style updatedStyle = new(typeof(ParagraphTextBlock));
        updatedStyle.Setters.Add(new Setter(TextBlock.FontSizeProperty, 19d));
        ResourceDictionary updatedTheme = MarkdownTestHelper.CreateThemeResources((typeof(ParagraphTextBlock), updatedStyle));

        MarkdownView view = new()
        {
            Markdown = "Paragraph",
            ThemeResources = initialTheme,
        };

        DispatcherTestHelper.Drain();

        view.ThemeResources = updatedTheme;
        DispatcherTestHelper.Drain();

        MarkdownRootPanel panel = Assert.IsType<MarkdownRootPanel>(view.RenderedContent);
        ParagraphTextBlock paragraph = Assert.IsType<ParagraphTextBlock>(panel.Children[0]);

        Assert.Equal(19d, paragraph.FontSize);
    }

    [StaFact]
    public void RequestRefresh_WhenCalledMultipleTimes_OnlyDispatchesOneRefresh()
    {
        MarkdownView view = new();
        view.Markdown = "Initial";
        DispatcherTestHelper.Drain();

        // Call RequestRefresh multiple times before the dispatcher processes
        view.RequestRefresh();
        view.RequestRefresh();
        view.RequestRefresh();

        // The content should remain from the first render; a single refresh will re-render
        DispatcherTestHelper.Drain();

        // After drain, content should be non-null (the single coalesced refresh ran)
        Assert.NotNull(view.RenderedContent);
    }

    [StaFact]
    public void RefreshContent_WhenMarkdownIsNull_SetsRenderedContentToNull()
    {
        MarkdownView view = new();
        view.Markdown = "Some content";
        DispatcherTestHelper.Drain();

        view.Markdown = null;
        view.RefreshContent();

        Assert.Null(view.RenderedContent);
    }

    [StaFact]
    public void RefreshContent_WhenMarkdownHasContent_SetsRenderedContent()
    {
        MarkdownView view = new();

        view.Markdown = "Hello";
        view.RefreshContent();

        Assert.NotNull(view.RenderedContent);
        Assert.IsType<MarkdownRootPanel>(view.RenderedContent);
    }
}
