using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

using MarkdownViewer.Wpf.Core;
using MarkdownViewer.Wpf.Diagnostics;
using MarkdownViewer.Wpf.Theming;

using Xunit;

namespace MarkdownViewer.Wpf.Tests;

public sealed class DiagnosticsTests
{
    [Fact]
    public void LoggingPostProcessor_Constructor_ThrowsOnNullLogDelegate()
    {
        Assert.Throws<ArgumentNullException>(() => new LoggingPostProcessor(null!));
    }

    [StaFact]
    public void LoggingPostProcessor_Process_LogsRootAndTheme()
    {
        List<string> messages = [];
        LoggingPostProcessor processor = new(messages.Add);

        processor.Process(new Border(), MarkdownTestHelper.CreateContext(theme: new DarkTheme()));

        string message = Assert.Single(messages);
        Assert.Contains("Border", message);
        Assert.Contains(nameof(DarkTheme), message);
    }

    [StaFact]
    public void DiagnosticsOverlayPostProcessor_Annotate_AddsDefaultsToBareBorder()
    {
        Border border = new();
        IRenderContext context = MarkdownTestHelper.CreateContext(theme: new DefaultTheme());

        DiagnosticsOverlayPostProcessor.Annotate(border, context);

        Assert.Equal(context.Theme.GetBrush(ThemeKeys.DiagnosticsOverlayBorderBrush), border.BorderBrush);
        Assert.Equal(new Thickness(1), border.BorderThickness);
    }

    [StaFact]
    public void DiagnosticsOverlayPostProcessor_Annotate_PreservesExistingBrushAndThickness()
    {
        Border border = new()
        {
            BorderBrush = Brushes.Blue,
            BorderThickness = new Thickness(3),
        };
        IRenderContext context = MarkdownTestHelper.CreateContext(theme: new DefaultTheme());

        DiagnosticsOverlayPostProcessor.Annotate(border, context);

        Assert.Equal(Brushes.Blue, border.BorderBrush);
        Assert.Equal(new Thickness(3), border.BorderThickness);
    }

    [StaFact]
    public void DiagnosticsOverlayPostProcessor_Process_TraversesNestedElements()
    {
        Border nested = new();
        Border inner = new() { Child = nested };
        ContentControl contentControl = new() { Content = inner };
        StackPanel root = new();
        root.Children.Add(contentControl);

        new DiagnosticsOverlayPostProcessor().Process(root, MarkdownTestHelper.CreateContext(theme: new DefaultTheme()));

        Assert.Equal(new Thickness(1), inner.BorderThickness);
        Assert.Equal(new Thickness(1), nested.BorderThickness);
        Assert.IsType<Grid>(inner.Child);
    }

    [Fact]
    public void MarkdownAstFormatter_FormatsDocumentTree()
    {
        string formatted = MarkdownAstFormatter.Format(MarkdownTestHelper.Parse("# Heading\n\nParagraph"));

        Assert.Contains("HeadingBlock", formatted);
        Assert.Contains("ParagraphBlock", formatted);
    }

    [StaFact]
    public void MarkdownRenderTreeFormatter_FormatsVisualTree()
    {
        StackPanel panel = new();
        panel.Children.Add(new Border { Child = new TextBlock { Text = "content" } });

        string formatted = MarkdownRenderTreeFormatter.Format(panel);

        Assert.Contains("StackPanel", formatted);
        Assert.Contains("Border", formatted);
        Assert.Contains("TextBlock", formatted);
    }
}