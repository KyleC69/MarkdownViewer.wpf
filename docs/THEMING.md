# WPF Markdown Viewer Theming

## Overview

MarkdownViewer.Wpf uses native WPF theming:

- Renderers emit markdown-specific control types such as `ParagraphTextBlock`, `CodeBlockBorder`, and `CodeBlockCopyButton`.
- The surrounding control tree and application resources participate in styling through normal WPF implicit-style lookup.
- The `MarkdownView.ThemeResources` property optionally scopes an additional `ResourceDictionary` to a single viewer instance.

This model follows normal WPF resource lookup behavior and avoids renderer-owned visual styling.

If `ThemeResources` is not set, the library does not inject a fallback theme. Rendered elements resolve styles from the host application's resources just like any other WPF element.

## Optional Built-in Theme Dictionaries

The library ships optional convenience dictionaries that you can merge explicitly when you want a ready-made visual treatment:

- `Themes/DefaultTheme.xaml`
- `Themes/LightTheme.xaml`
- `Themes/DarkTheme.xaml`


## Applying a Theme

### Host application styles only

If you want the markdown surface to inherit the parent application's look, define implicit styles for the rendered markdown control types in your window, user control, or application resources and leave `ThemeResources` unset.

```xml
<Application xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:controls="clr-namespace:MarkdownViewer.Wpf.Controls;assembly=MarkdownViewer.Wpf">
    <Application.Resources>
        <ResourceDictionary>
            <Style TargetType="controls:ParagraphTextBlock">
                <Setter Property="Foreground" Value="{DynamicResource PrimaryTextBrush}" />
            </Style>

            <Style TargetType="controls:CodeBlockBorder">
                <Setter Property="BorderBrush" Value="{DynamicResource AccentBrush}" />
                <Setter Property="BorderThickness" Value="1" />
            </Style>
        </ResourceDictionary>
    </Application.Resources>
</Application>
```

```xaml
<markdown:MarkdownView Markdown="{Binding MarkdownText}" />
```

### Per Control (recommended)

```xaml
<markdown:MarkdownView Markdown="{Binding MarkdownText}"
                       ThemeResources="{Binding CurrentThemeResources}"
                       Padding="20" />
```

```csharp
public ResourceDictionary CurrentThemeResources => new()
{
    Source = new Uri(
        "pack://application:,,,/MarkdownViewer.Wpf;component/Themes/DarkTheme.xaml",
        UriKind.Absolute)
};
```

### Application-wide optional theme

```xaml
<Application.Resources>
    <ResourceDictionary>
        <ResourceDictionary.MergedDictionaries>
            <ResourceDictionary Source="pack://application:,,,/MarkdownViewer.Wpf;component/Themes/DefaultTheme.xaml" />
        </ResourceDictionary.MergedDictionaries>
    </ResourceDictionary>
</Application.Resources>
```

## Overriding Specific Markdown Elements

Because the renderer creates specific markdown control types, overrides stay scoped and predictable. If you are extending one of the built-in theme dictionaries, base your override on the library's named style key instead of replacing the implicit style outright.

```xml
<ResourceDictionary xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                    xmlns:controls="clr-namespace:MarkdownViewer.Wpf.Controls;assembly=MarkdownViewer.Wpf">

    <Style TargetType="controls:CodeBlockBorder"
           BasedOn="{StaticResource CodeBlockBorderStyle}">
        <Setter Property="CornerRadius" Value="10" />
        <Setter Property="BorderThickness" Value="2" />
    </Style>

    <Style TargetType="controls:CodeBlockCopyButton"
           BasedOn="{StaticResource CodeBlockCopyButtonStyle}">
        <Setter Property="Padding" Value="12,5" />
        <Setter Property="FontWeight" Value="SemiBold" />
    </Style>

</ResourceDictionary>
```

Load that override dictionary after the base theme dictionary. If you are not extending a built-in theme, use ordinary implicit styles without `BasedOn` and let host-level resources fully define the look.

## Code Block Behavior

Current code block rendering behavior:

- Every block code element renders a header row.
- Header left: language label (`fence info string` for fenced blocks, otherwise `text`).
- Header right: copy button.
- Copy button copies the block contents to clipboard.
- Copy button shows a temporary visual confirmation (`Copied`) and then returns to `Copy`.


## Best Practices

1. Keep theme overrides in dedicated dictionaries and merge them after any base theme they extend.
2. Style markdown control types (`MarkdownViewer.Wpf.Controls.*`) rather than global `TextBlock` or `Border` unless you intentionally want broad application-wide effects.
3. Leave `ThemeResources` unset when you want markdown content to inherit ambient application resources.
4. Use `ThemeResources` only when you need per-view scoping or explicit theme composition.
5. When extending built-in theme dictionaries, base your override styles on the library's named keys such as `CodeBlockBorderStyle` and `CodeBlockCopyButtonStyle`.

## Conclusion

This project's theming model *MUST* follow standard WPF patterns to ensure compatibility and predictability when integrated into any WPF application. By leveraging implicit styles and control-specific types, it provides a flexible yet consistent way to customize markdown rendering without coupling visual design to the rendering logic.
