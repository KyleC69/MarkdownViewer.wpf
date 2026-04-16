using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using MarkdownViewer.Wpf.Diagnostics;
using MarkdownViewer.Wpf.Theming;

using Xunit;

namespace MarkdownViewer.Wpf.Tests;

public sealed class ThemingTests
{
    [StaFact]
    public void CompositeTheme_GetStyle_ReturnsFirstNonNullStyle()
    {
        Style expected = new(typeof(TextBlock));
        CompositeTheme theme = new(
            new FakeTheme { StyleResolver = static _ => null },
            new FakeTheme { StyleResolver = _ => expected });

        Assert.Same(expected, theme.GetStyle("style"));
    }

    [StaFact]
    public void CompositeTheme_GetBrush_ReturnsFirstNonNullBrush()
    {
        SolidColorBrush expected = Brushes.CadetBlue;
        CompositeTheme theme = new(
            new FakeTheme { BrushResolver = static _ => null },
            new FakeTheme { BrushResolver = _ => expected });

        Assert.Same(expected, theme.GetBrush("brush"));
    }

    [Fact]
    public void CompositeTheme_GetFontSize_ReturnsFirstNonNullFontSize()
    {
        CompositeTheme theme = new(
            new FakeTheme { FontSizeResolver = static _ => null },
            new FakeTheme { FontSizeResolver = static _ => 18.5 });

        Assert.Equal(18.5, theme.GetFontSize("fontSize"));
    }

    [Fact]
    public void CompositeTheme_GetThickness_ReturnsFirstNonNullThickness()
    {
        CompositeTheme theme = new(
            new FakeTheme { ThicknessResolver = static _ => null },
            new FakeTheme { ThicknessResolver = static _ => new Thickness(4) });

        Assert.Equal(new Thickness(4), theme.GetThickness("padding"));
    }

    [StaFact]
    public void ResourceDictionaryTheme_Getters_ReturnTypedResources()
    {
        Style style = new(typeof(TextBlock));
        ResourceDictionaryTheme theme = MarkdownTestHelper.CreateTheme(
            ("style", style),
            ("brush", Brushes.Coral),
            ("font", 15.0),
            ("thickness", new Thickness(7)));

        Assert.Same(style, theme.GetStyle("style"));
        Assert.Same(Brushes.Coral, theme.GetBrush("brush"));
        Assert.Equal(15.0, theme.GetFontSize("font"));
        Assert.Equal(new Thickness(7), theme.GetThickness("thickness"));
        Assert.Null(theme.GetBrush("style"));
        Assert.Null(theme.GetStyle("missing"));
    }

    [StaFact]
    public void ResourceDictionaryTheme_FindResource_SearchesMergedDictionaries()
    {
        ResourceDictionary merged = new()
        {
            ["target"] = Brushes.Goldenrod,
        };
        ResourceDictionary root = new();
        root.MergedDictionaries.Add(merged);
        ResourceDictionaryTheme theme = new(root);

        Assert.Same(Brushes.Goldenrod, theme.FindResource("target"));
        Assert.Same(Brushes.Goldenrod, ResourceDictionaryTheme.TryFindResource(root, "target"));
    }

    [Fact]
    public void ResourceDictionaryTheme_FindResource_Throws_ForBlankKey()
    {
        ResourceDictionaryTheme theme = new(new ResourceDictionary());

        Assert.Throws<ArgumentException>(() => theme.FindResource(" "));
    }

    [Fact]
    public void ResourceDictionaryTheme_FindResource_EmitsMissingKeyDiagnostic()
    {
        List<MarkdownDiagnosticEventArgs> events = [];
        ResourceDictionaryTheme theme = new(new ResourceDictionary());

        MarkdownDiagnostics.Emitted += CaptureEvent;
        try
        {
            Assert.Null(theme.GetStyle("missing"));
        }
        finally
        {
            MarkdownDiagnostics.Emitted -= CaptureEvent;
        }

        MarkdownDiagnosticEventArgs diagnostic = Assert.Single(events);
        Assert.Equal(MarkdownDiagnosticKind.ThemeKeyMissing, diagnostic.Kind);
        Assert.Equal("missing", diagnostic.Key);

        void CaptureEvent(object? sender, MarkdownDiagnosticEventArgs args)
        {
            events.Add(args);
        }
    }

    [StaFact]
    public void ResourceDictionaryTheme_LoadDictionary_LoadsKnownTheme()
    {
        ResourceDictionary dictionary = ResourceDictionaryTheme.LoadDictionary("/MarkdownViewer.Wpf;component/Themes/DefaultTheme.xaml");

        Assert.NotNull(ResourceDictionaryTheme.TryFindResource(dictionary, ThemeKeys.ParagraphStyle));
        Assert.NotNull(ResourceDictionaryTheme.TryFindResource(dictionary, ThemeKeys.HyperlinkStyle));
    }

    [StaFact]
    public void DefaultTheme_ProvidesParagraphStyle()
    {
        DefaultTheme theme = new();

        Assert.NotNull(theme.GetStyle(ThemeKeys.ParagraphStyle));
        Assert.NotNull(theme.GetBrush(ThemeKeys.ViewerBackgroundBrush));
        Assert.NotNull(theme.GetBrush(ThemeKeys.ViewerBorderBrush));
        Assert.Equal(new Thickness(1), theme.GetThickness(ThemeKeys.ViewerBorderThickness));
    }

    [StaFact]
    public void LightTheme_ProvidesParagraphStyleFromThemeDictionary()
    {
        LightTheme theme = new();

        Assert.NotNull(theme.GetStyle(ThemeKeys.ParagraphStyle));
        Assert.NotNull(theme.GetBrush(ThemeKeys.ViewerBackgroundBrush));
    }

    [StaFact]
    public void DarkTheme_ProvidesDarkParagraphForeground()
    {
        DarkTheme theme = new();
        Style style = Assert.IsType<Style>(theme.GetStyle(ThemeKeys.ParagraphStyle));
        Setter foregroundSetter = Assert.IsType<Setter>(style.Setters.OfType<Setter>().Single(setter => setter.Property == TextBlock.ForegroundProperty));

        Assert.Equal("#FFE8EEF9", foregroundSetter.Value.ToString());
    }
}