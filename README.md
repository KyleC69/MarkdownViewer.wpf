> [!IMPORTANT]  
> Because of a lack of time and interest, this repo is archived. If my situation changes, I might unarchive it.
> For the time being, consider forking it or use an alternative.
> Existing Nuget packages will still be available.

# MarkdownViewer.Wpf [![NuGet](https://img.shields.io/nuget/v/MarkdownViewer.Wpf.svg?logo=nuget)](https://www.nuget.org/packages/MarkdownViewer.Wpf/) [![NuGet](https://img.shields.io/nuget/dt/MarkdownViewer.Wpf.svg)](https://www.nuget.org/stats/packages/MarkdownViewer.Wpf?groupby=Version)
A WPF library for [lunet-io/markdig](https://github.com/lunet-io/markdig)

MarkdownViewer.Wpf is now a XAML-first package.

- Markdown is rendered to XAML through `Markdown.ToXaml(...)`
- `MarkdownViewer` is a host control that loads the generated XAML into a `FlowDocument`
- Renderer composition is registry-driven so individual markdown node renderers can be replaced without rebuilding the whole pipeline

## Usage

Render markdown directly to XAML:

```csharp
using MarkdownViewer.Wpf;

var pipeline = new MarkdownPipelineBuilder()
	.UseSupportedExtensions()
	.Build();

var xaml = Markdown.ToXaml("# Hello from MarkdownViewer.Wpf", pipeline);
```

Or bind markdown into the built-in WPF host control:

```xml
<Window
	xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
	xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
	xmlns:markdown="clr-namespace:MarkdownViewer.Wpf;assembly=MarkdownViewer.Wpf">
	<markdown:MarkdownViewer Markdown="## Viewer host" />
</Window>
```

The host control also supports basic viewer-level customization through normal control properties plus zoom settings:

```xml
<markdown:MarkdownViewer Markdown="## Styled viewer"
						 Background="White"
						 BorderBrush="#DDD"
						 BorderThickness="1"
						 Padding="16"
						 FontFamily="Segoe UI"
						 FontSize="14"
						 Zoom="125"
						 MinZoom="50"
						 MaxZoom="300"
						 IsToolBarVisible="False" />
```

Default document and host styles live in the theme dictionary and can be overridden with the same resource keys exposed by `MarkdownViewer.Wpf.Styles`, including `DocumentStyleKey`, `ParagraphStyleKey`, `CodeBlockStyleKey`, and `MarkdownViewerScrollViewerStyleKey`.

## Customization

Start from the package defaults and replace a single renderer by markdown node type:

```csharp
using Markdig.Syntax.Inlines;
using MarkdownViewer.Wpf;
using MarkdownViewer.Wpf.Abstractions;
using MarkdownViewer.Wpf.Rendering.Xaml;
using MarkdownViewer.Wpf.Rendering.Xaml.Defaults.Inlines;

var registry = XamlRendererRegistry.CreateDefault()
	.ReplaceFor<LinkInline, CustomLinkInlineRenderer>();

var xaml = Markdown.ToXaml("[docs](https://example.com)", rendererRegistry: registry);

public sealed class CustomLinkInlineRenderer : DefaultLinkInlineRenderer
{
	protected override void WriteObject(IXamlRenderContext renderer, LinkInline obj)
	{
		base.WriteObject(renderer, obj);
	}
}
```

The same registry can be assigned to `MarkdownViewer.RendererRegistry` when you want the control host to use the same custom rendering behavior.

Code blocks use the higher-level customization seam by default. The built-in renderer emits the styled wrapper and copy button automatically, and a derived renderer can change that behavior without rewriting the whole block renderer:

```csharp
using Markdig.Syntax;
using MarkdownViewer.Wpf.Rendering.Xaml.Defaults.Blocks;

public sealed class NoCopyCodeBlockRenderer : DefaultCodeBlockRenderer
{
	protected override bool ShowCopyButton(CodeBlock obj)
	{
		return false;
	}
}
```

When the generated XAML is hosted inside `MarkdownViewer`, the built-in `Commands.CodeBlockCopy` binding copies the block text to the clipboard automatically. If you load the XAML in a custom host, bind `Commands.CodeBlockCopy` yourself. The default code block container, header, header text, body, and copy button styles all live in the theme dictionary under `MarkdownViewer.Wpf.Styles`.


## Features

Supports all standard features from Markdig (i.e. fully CommonMark compliant).

Additionally, the following extensions are supported:
- **Auto-links**
- **Task lists**
- **Tables**
- **Extra emphasis**
