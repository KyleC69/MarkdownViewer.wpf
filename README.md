# MarkdownViewer.Wpf

`MarkdownViewer.Wpf` is a WPF control for rendering Markdown into a native `UIElement` tree. It uses `Markdig` for parsing and applies WPF styles and brushes through a theme abstraction, making it suitable for live previews, embedded documentation panes, and markdown-driven desktop UI.

## Features

- Native WPF `MarkdownView` control
- Deterministic rendering to WPF visual elements instead of a browser surface
- Built-in themes: `DefaultTheme`, `LightTheme`, and `DarkTheme`
- Support for common markdown elements, including:
  - headings and paragraphs
  - bold, italic, strikethrough, subscript, superscript, inserted, marked, and inline code
  - ordered, unordered, and task lists
  - block quotes and thematic breaks
  - indented and fenced code blocks
  - tables
  - links, auto-links, and images
  - HTML blocks and supported inline HTML
- Extensibility through injectable services for:
  - link navigation via `IMarkdownLinkNavigator`
  - image loading via `IMarkdownImageSourceResolver`
- Diagnostics surface through `MarkdownDiagnostics.Emitted`
- Sample app included for live editing and theme switching

## Requirements

- .NET 9
- WPF on Windows

## Installation

Add the NuGet package reference:

```xml
<ItemGroup>
  <PackageReference Include="MarkdownViewer.Wpf" Version="6.0.0" />
</ItemGroup>
```

## Basic usage

Add the XML namespace in XAML:

```xml
xmlns:markdown="clr-namespace:MarkdownViewer.Wpf;assembly=MarkdownViewer.Wpf"
```

Then place the control in your view:

```xml
<markdown:MarkdownView Markdown="{Binding MarkdownText}" Padding="16" />
```

You can also set the markdown directly:

```xml
<markdown:MarkdownView
    Markdown="# Hello\n\nThis is **MarkdownViewer.Wpf**."
    Padding="16" />
```

## Theming

The control exposes a `Theme` property. Built-in options are available from `MarkdownViewer.Wpf.Theming`.

```xml
<markdown:MarkdownView
    Markdown="{Binding MarkdownText}"
    Theme="{Binding CurrentTheme}"
    Padding="20" />
```

Example view-model property:

```csharp
using MarkdownViewer.Wpf.Theming;

public ITheme CurrentTheme => IsDarkModeEnabled
    ? new DarkTheme()
    : new DefaultTheme();
```

If needed, custom themes can be created by implementing `ITheme`, by deriving from `ResourceDictionaryTheme`, or by composing multiple themes with `CompositeTheme`.

## Services and integration points

`MarkdownView` accepts an `IServiceProvider` through its `Services` property. The renderer uses that provider to resolve integration services.

### Custom link navigation

Implement `IMarkdownLinkNavigator` to intercept hyperlink clicks.

```csharp
using MarkdownViewer.Wpf.Core;

public sealed class AppLinkNavigator : IMarkdownLinkNavigator
{
    public bool TryNavigate(Uri uri, IRenderContext context)
    {
        // Handle in-app routing or custom navigation here.
        return true;
    }
}
```

### Custom image loading

Implement `IMarkdownImageSourceResolver` to control how markdown images are resolved.

```csharp
using System.Windows.Media;
using MarkdownViewer.Wpf.Core;

public sealed class AppImageResolver : IMarkdownImageSourceResolver
{
    public ImageSource? ResolveImageSource(Uri uri, IRenderContext context)
    {
        // Resolve local files, cached images, or protected resources here.
        return null;
    }
}
```

Provide those services from your application service provider and assign it to `MarkdownView.Services`.

## Diagnostics

The library emits diagnostics through `MarkdownDiagnostics` for events such as:

- rendered block and inline nodes
- missing theme keys
- ignored unsupported inline HTML
- image load failures
- link navigation failures

Example subscription:

```csharp
using System.Diagnostics;
using MarkdownViewer.Wpf.Diagnostics;

MarkdownDiagnostics.Emitted += (_, args) =>
{
    Debug.WriteLine($"[{args.Kind}] {args.Message}");
};
```

## Running the sample app

The repository contains a sample WPF application in `src/MarkdownViewer.Wpf.Sample` that demonstrates:

- two-pane editing and preview
- theme switching
- tables, task lists, code blocks, links, images, and HTML rendering

Run it from the repository root:

```powershell
dotnet run --project .\\src\\MarkdownViewer.Wpf.Sample\\MarkdownViewer.Wpf.Sample.csproj
```

## Repository layout

- `src/MarkdownViewer.Wpf` - reusable control library
- `src/MarkdownViewer.Wpf.Sample` - sample WPF application
- `src/MarkdownViewer.Wpf.Tests` - automated tests
