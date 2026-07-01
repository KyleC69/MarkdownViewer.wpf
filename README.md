# MarkdownViewer.Wpf

`MarkdownViewer.Wpf` is a WPF control for rendering Markdown into a native `UIElement` tree. It uses `Markdig` for parsing and applies WPF styles, brushes, and merged resource dictionaries through the normal WPF theming model, making it suitable for live previews, embedded documentation panes, and markdown-driven desktop UI.

## Features

- Native WPF `MarkdownView` control
- Deterministic rendering to WPF visual elements instead of a browser surface
- Markdown-specific WPF control subtypes for predictable, element-scoped implicit styling
- Optional built-in theme resource dictionaries for default, light, and dark presentations
- Support for common markdown elements, including:
  - headings (H1–H6) and paragraphs
  - bold, italic, strikethrough, subscript, superscript, inserted, marked, and inline code
  - ordered, unordered, and task lists
  - block quotes and thematic breaks
  - indented and fenced code blocks with a header row, language label, and copy button
  - tables
  - links, auto-links, and images
  - HTML blocks and supported inline HTML
- Extensibility through injectable services for:
  - link navigation via `IMarkdownLinkNavigator`
  - image loading via `IMarkdownImageSourceResolver`
- Custom renderer registration via `MarkdownRendererBuilder`
- Post-processing pipeline via `IPostProcessor`
- Diagnostics surface through `MarkdownDiagnostics.Emitted`
- Sample app included for live editing and theme switching
- Runtime-adjustable preview/editor split in the sample app

## Requirements

- .NET 10
- WPF on Windows

## Installation

Add the NuGet package reference:

```xml
<ItemGroup>
  <PackageReference Include="MarkdownViewer.Wpf" Version="2.0.*" />
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

The renderer emits markdown-specific WPF control subtypes defined in `MarkdownViewer.Wpf.Controls`. Each subtype maps to a distinct markdown concept, allowing themes to apply fully independent implicit styles without relying on attached properties, triggers, or shared target types.

Available rendered control types include:

| Type | Represents |
|---|---|
| `ParagraphTextBlock` | Paragraph text |
| `Heading1TextBlock` – `Heading6TextBlock` | Headings H1–H6 |
| `CodeBlockTextBlock` | Code block content |
| `CodeInlineSpan` | Inline code |
| `BlockQuoteBorder` | Block quote container |
| `CodeBlockBorder` | Code block outer border |
| `CodeBlockHeaderBorder` | Code block header area |
| `CodeBlockHeaderTextBlock` | Code block language label |
| `CodeBlockCopyButton` | Code block copy button |
| `CodeBlockScrollViewer` | Code block scroll container |
| `ListPanel` | List container |
| `ListItemGrid` | List item layout grid |
| `ListItemMarkerTextBlock` | List item bullet or number |
| `ListItemContentPanel` | List item content area |

The control exposes a `ThemeResources` property that accepts a WPF `ResourceDictionary`. If `ThemeResources` is not set, the rendered tree inherits styles from the surrounding control tree and application resources through normal WPF resource lookup. The library does not inject a fallback theme automatically.

### Optional built-in themes

The library ships three convenience resource dictionaries:

- `Themes/DefaultTheme.xaml`
- `Themes/LightTheme.xaml`
- `Themes/DarkTheme.xaml`

Example — switching themes from a view model:

```csharp
using System.Windows;

public ResourceDictionary CurrentThemeResources => new()
{
    Source = new Uri(
        IsDarkModeEnabled
            ? "pack://application:,,,/MarkdownViewer.Wpf;component/Themes/DarkTheme.xaml"
            : "pack://application:,,,/MarkdownViewer.Wpf;component/Themes/DefaultTheme.xaml",
        UriKind.Absolute)
};
```

```xml
<markdown:MarkdownView
    Markdown="{Binding MarkdownText}"
    ThemeResources="{Binding CurrentThemeResources}"
    Padding="20" />
```

### Applying a theme at the application level

```xml
<Application.Resources>
    <ResourceDictionary>
        <ResourceDictionary.MergedDictionaries>
            <ResourceDictionary Source="pack://application:,,,/MarkdownViewer.Wpf;component/Themes/DefaultTheme.xaml" />
        </ResourceDictionary.MergedDictionaries>
    </ResourceDictionary>
</Application.Resources>
```

### Merging theme overrides per control

```xml
<markdown:MarkdownView Markdown="{Binding MarkdownText}" Padding="20">
    <markdown:MarkdownView.ThemeResources>
        <ResourceDictionary>
            <ResourceDictionary.MergedDictionaries>
                <ResourceDictionary Source="pack://application:,,,/MarkdownViewer.Wpf;component/Themes/DefaultTheme.xaml" />
                <ResourceDictionary Source="Themes/MarkdownOverrides.xaml" />
            </ResourceDictionary.MergedDictionaries>
        </ResourceDictionary>
    </markdown:MarkdownView.ThemeResources>
</markdown:MarkdownView>
```

For a full theming reference see [docs/THEMING.md](docs/THEMING.md) and [docs/RENDERED-TYPE-CUSTOMIZATION-GUIDE.md](docs/RENDERED-TYPE-CUSTOMIZATION-GUIDE.md).

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

## Custom renderers and post-processors

`MarkdownRendererBuilder` lets you compose a fully custom rendering pipeline. Call `MarkdownRendererBuilder.CreateDefault()` to start from the built-in renderer set and add or replace renderers as needed.

```csharp
using MarkdownViewer.Wpf.Core;

MarkdownEngine engine = new MarkdownRendererBuilder()
    .AddBlockRenderer<MyCustomBlock, MyCustomBlockRenderer>()
    .AddInlineRenderer<MyCustomInline, MyCustomInlineRenderer>()
    .AddPostProcessor(new MyPostProcessor())
    .BuildDispatcher()
    // pass to MarkdownEngine constructor
```

Implement `IPostProcessor` to inspect or modify the rendered `UIElement` tree after each render pass:

```csharp
using System.Windows;
using MarkdownViewer.Wpf.Core;

public sealed class MyPostProcessor : IPostProcessor
{
    public void Process(UIElement root, IRenderContext context)
    {
        // Walk or mutate the rendered visual tree here.
    }
}
```

Use `MarkdownEngine.CreateDefault()` to get a ready-made engine using all built-in renderers:

```csharp
MarkdownEngine engine = MarkdownEngine.CreateDefault();
```

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

The repository contains a sample WPF application in `src/MarkdownViewer.WPF.Sample` that demonstrates:

- two-pane editing and preview
- runtime-adjustable pane split via `GridSplitter`
- theme switching
- tables, task lists, code blocks, links, images, and HTML rendering

Run it from the repository root:

```powershell
dotnet run --project .\src\MarkdownViewer.WPF.Sample\MarkdownViewer.Wpf.Sample.csproj
```

## Repository layout

```
src/
  MarkdownViewer.Wpf/
    Controls/       — MarkdownView control and markdown-specific WPF element types
    Core/           — Rendering engine, builder, dispatcher, and service interfaces
    Diagnostics/    — Diagnostics events and formatters
    Rendering/
      Blocks/       — Block renderers (paragraph, heading, list, code, table, etc.)
      Inlines/      — Inline renderers (emphasis, link, code, task list, etc.)
      Html/         — HTML fragment renderer
  MarkdownViewer.WPF.Sample/  — Sample WPF application
  MarkdownViewer.Wpf.Tests/   — Automated tests
docs/
  THEMING.md                          — Full theming reference
  RENDERED-TYPE-CUSTOMIZATION-GUIDE.md — End-to-end style override example
```
