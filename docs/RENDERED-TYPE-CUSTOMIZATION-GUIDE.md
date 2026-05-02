# Rendered Type Customization Guide

## Purpose

This guide explains how to customize styling for a specific rendered markdown type using native WPF target-type styles.

The end-to-end example customizes code block rendering only, while leaving the rest of the theme intact.

## How Customization Works

1. Start with one of the built-in theme dictionaries.
2. Add a second dictionary that contains target-type overrides.
3. Merge the base theme first and overrides second.
4. Pass the merged dictionary through `MarkdownView.ThemeResources`.

## End-to-End Example: Code Blocks

This is the exact example used by the sample app.

### 1. Create an override dictionary

File: `src/MarkdownViewer.Wpf.Sample/Themes/CodeBlockOverrides.xaml`

```xml
<ResourceDictionary xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                    xmlns:controls="clr-namespace:MarkdownViewer.Wpf.Controls;assembly=MarkdownViewer.Wpf">

    <!--
      Each implicit style below references the explicitly keyed base-theme style via BasedOn.
      Because the named keys (e.g. CodeBlockBorderStyle) are defined only in the base theme
      dictionary and never duplicated here, there is no possibility of circular style
      resolution across merged-dictionary boundaries.
    -->

    <Style TargetType="controls:CodeBlockBorder"
           BasedOn="{StaticResource CodeBlockBorderStyle}">
        <Setter Property="CornerRadius" Value="14" />
        <Setter Property="BorderThickness" Value="2" />
    </Style>

    <Style TargetType="controls:CodeBlockHeaderBorder"
           BasedOn="{StaticResource CodeBlockHeaderBorderStyle}">
        <Setter Property="CornerRadius" Value="13,13,0,0" />
    </Style>

    <Style TargetType="controls:CodeBlockHeaderTextBlock"
           BasedOn="{StaticResource CodeBlockHeaderTextBlockStyle}">
        <Setter Property="FontWeight" Value="Bold" />
        <Setter Property="TextOptions.TextFormattingMode" Value="Display" />
    </Style>

    <Style TargetType="controls:CodeBlockCopyButton"
           BasedOn="{StaticResource CodeBlockCopyButtonStyle}">
        <Setter Property="MinWidth" Value="72" />
        <Setter Property="Padding" Value="12,5" />
    </Style>

    <Style TargetType="controls:CodeBlockTextBlock"
           BasedOn="{StaticResource CodeBlockTextBlockStyle}">
        <Setter Property="Padding" Value="14" />
    </Style>

</ResourceDictionary>
```

### 2. Merge base theme plus overrides

File: `src/MarkdownViewer.Wpf.Sample/MainWindow.xaml.cs`

```csharp
public ResourceDictionary CurrentThemeResources => SelectedThemeKey switch
{
    "Light" => CreateMergedThemeResources("LightTheme.xaml"),
    "Dark" => CreateMergedThemeResources("DarkTheme.xaml"),
    _ => CreateMergedThemeResources("DefaultTheme.xaml"),
};

private static ResourceDictionary CreateThemeDictionary(string themeFile)
{
    return new ResourceDictionary
    {
        Source = new Uri($"pack://application:,,,/MarkdownViewer.Wpf;component/Themes/{themeFile}", UriKind.Absolute),
    };
}

private static ResourceDictionary CreateMergedThemeResources(string themeFile)
{
    ResourceDictionary merged = new();
    merged.MergedDictionaries.Add(CreateThemeDictionary(themeFile));
    merged.MergedDictionaries.Add(new ResourceDictionary
    {
        Source = new Uri("pack://application:,,,/MarkdownViewer.Wpf.Sample;component/Themes/CodeBlockOverrides.xaml", UriKind.Absolute),
    });

    return merged;
}
```

### 3. Bind the merged theme resources to MarkdownView

File: `src/MarkdownViewer.Wpf.Sample/MainWindow.xaml`

```xaml
<markdown:MarkdownView Markdown="{Binding MarkdownText}"
                       ThemeResources="{Binding CurrentThemeResources}"
                       Padding="20" />
```

## Verification Checklist

1. Run the sample app.
2. Switch between Default, Light, and Dark.
3. Confirm code blocks keep the override shape and spacing.
4. Confirm non-code markdown elements continue to follow the selected base theme.

## Appendix: All Targetable Rendered Control Types

The renderer emits the following control types in the `MarkdownViewer.Wpf.Controls` namespace.

### Text blocks

- `ParagraphTextBlock : TextBlock`
- `Heading1TextBlock : TextBlock`
- `Heading2TextBlock : TextBlock`
- `Heading3TextBlock : TextBlock`
- `Heading4TextBlock : TextBlock`
- `Heading5TextBlock : TextBlock`
- `Heading6TextBlock : TextBlock`
- `CodeBlockTextBlock : TextBlock`
- `CodeBlockHeaderTextBlock : TextBlock`
- `ListItemMarkerTextBlock : TextBlock`

### Panels and containers

- `MarkdownRootPanel : WrappingStackPanel`
- `ListPanel : StackPanel`
- `ListItemGrid : Grid`
- `ListItemContentPanel : StackPanel`
- `TableGrid : Grid`
- `CodeBlockHeaderPanel : DockPanel`
- `CodeBlockScrollViewer : ScrollViewer`

### Borders

- `BlockQuoteBorder : Border`
- `CodeBlockBorder : Border`
- `CodeBlockHeaderBorder : Border`
- `TableCellBorder : Border`
- `ThematicBreakBorder : Border`

### Buttons

- `CodeBlockCopyButton : Button`

### Inline spans

- `CodeInlineSpan : Span`
- `StrikeThroughSpan : Span`
- `SubscriptSpan : Span`
- `SuperscriptSpan : Span`
- `InsertedSpan : Span`
- `MarkedSpan : Span`

If you target these types with implicit styles in a merged dictionary, your styles will apply only to markdown-rendered elements of that type.
