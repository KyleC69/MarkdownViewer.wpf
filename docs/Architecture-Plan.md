# MarkdownViewer.Wpf — Architecture Specification **DO NOT ALTER THIS DOCUMENT WITHOUT APPROVAL**

## 1. Overview

**Purpose:**  
Define a modern, extensible, deterministic architecture for a WPF Markdown renderer built on **Markdig**, producing a **UIElement tree** instead of `FlowDocument`.

**Core pipeline:**

```text
Markdown text
    ↓
Markdig Pipeline (extensions, GFM, HTML)
    ↓
MarkdownDocument (AST)
    ↓
Renderer Pipeline (block/inline renderers)
    ↓
UIElement Tree (WPF visual tree)
    ↓
MarkdownView (WPF control)
```

---

## 2. Design goals

- **Deterministic:** No hidden mutation, no black‑box layout engines.
- **Extensible:** Pluggable renderers, themes, post‑processors, and integrations.
- **Themeable:** First‑class WPF styling and resource dictionaries.
- **Observable:** Diagnostics, tracing, and error surfaces are built‑in.
- **Composable:** Clear separation of parsing, rendering, theming, and post‑processing.
- **Future‑proof:** Supports custom blocks, HTML, math, diagrams, and interactive content.

---

## 3. Core concepts and types

### 3.1. Render context

```csharp
public interface IRenderContext
{
    ITheme Theme { get; }
    IServiceProvider Services { get; }

    UIElement RenderBlock(Block block);
    Inline RenderInline(Inline inline);

    void AddPostProcessor(IPostProcessor processor);
}
```

**Responsibilities:**

- Provide access to theme and services.
- Dispatch rendering of nested nodes.
- Collect post‑processors to run after tree construction.

### 3.2. Block and inline renderers

```csharp
public interface IBlockRenderer<TBlock> where TBlock : Block
{
    UIElement Render(TBlock block, IRenderContext context);
}

public interface IInlineRenderer<TInline> where TInline : Inline
{
    Inline Render(TInline inline, IRenderContext context);
}
```

**Characteristics:**

- One renderer per Markdig block/inline type.
- Pure functions: no global state, no side effects beyond UI creation.
- Throw explicit errors when required data is missing.

### 3.3. Post‑processors

```csharp
public interface IPostProcessor
{
    void Process(UIElement root, IRenderContext context);
}
```

**Use cases:**

- Syntax highlighting.
- Adding copy buttons to code blocks.
- Link rewriting (e.g., relative → absolute).
- Diagnostics overlays.
- Instrumentation (timing, logging).

---

## 4. Renderer dispatcher

### 4.1. Purpose

Central component that maps Markdig AST nodes to the appropriate renderer implementation.

### 4.2. API

```csharp
public sealed class RendererDispatcher
{
    private readonly Dictionary<Type, object> _blockRenderers;
    private readonly Dictionary<Type, object> _inlineRenderers;

    public RendererDispatcher(
        IEnumerable<object> blockRenderers,
        IEnumerable<object> inlineRenderers);

    public UIElement RenderBlock(Block block, IRenderContext context);
    public Inline RenderInline(Inline inline, IRenderContext context);
}
```

### 4.3. Behavior

- On construction, index renderers by their generic argument type.
- `RenderBlock`/`RenderInline`:
  - Look up renderer by `block.GetType()` / `inline.GetType()`.
  - If not found, throw a descriptive `RendererNotFoundException`.
  - Never silently skip or degrade behavior.

---

## 5. Rendering model

### 5.1. Block mapping

| Markdown block      | UIElement composition example                          |
|---------------------|--------------------------------------------------------|
| Paragraph           | `TextBlock` with `Inlines`                            |
| Heading             | `TextBlock` with style key `Heading{Level}Style`      |
| List (ordered/unordered) | `StackPanel` of list item elements              |
| ListItem            | `StackPanel` with bullet/number + content            |
| CodeBlock           | `Border` + `ScrollViewer` + `TextBlock`              |
| FencedCodeBlock     | Same as CodeBlock, with language metadata            |
| BlockQuote          | `Border` + nested rendered content                    |
| ThematicBreak       | `Separator` or styled `Border`                        |
| Table               | `Grid` with header row + body rows                    |
| HtmlBlock           | Delegated to HTML handler or placeholder element      |
| Custom blocks       | Handled by registered custom block renderers          |

### 5.2. Inline mapping

| Markdown inline | WPF inline example                     |
|-----------------|----------------------------------------|
| Literal         | `Run`                                  |
| Emphasis        | `Run` with italic style                |
| StrongEmphasis  | `Run` with bold style                  |
| Link            | `Hyperlink`                            |
| CodeInline      | `Run` with `CodeInlineStyle`           |
| LineBreak       | `LineBreak`                            |
| HtmlInline      | Delegated to HTML handler or ignored   |
| Custom inline   | Custom inline renderer                 |

---

## 6. Theming system

### 6.1. Theme abstraction

```csharp
public interface ITheme
{
    Style? GetStyle(string key);
    Brush? GetBrush(string key);
    Thickness? GetThickness(string key);
    double? GetFontSize(string key);
}
```

**Notes:**

- Theme is a read‑only facade over one or more `ResourceDictionary` instances.
- Missing keys are allowed but should be traceable (diagnostics).

### 6.2. Theme keys (examples)

- **Typography:**
  - `Heading1Style`, `Heading2Style`, `Heading3Style`
  - `ParagraphStyle`
  - `CodeInlineStyle`
- **Colors/brushes:**
  - `CodeBlockBackground`
  - `CodeBlockBorderBrush`
  - `LinkForeground`
  - `BlockQuoteBorderBrush`
- **Layout:**
  - `BlockQuoteMargin`
  - `ListItemIndent`
  - `CodeBlockPadding`

### 6.3. Theme implementations

- `DefaultTheme` (GitHub‑like).
- `DarkTheme`.
- `LightTheme`.
- `CompositeTheme` (wraps multiple dictionaries, e.g., base + app overrides).

### 6.4. Usage pattern

Renderers **never** hard‑code visual properties; they always query `ITheme`:

```csharp
var style = context.Theme.GetStyle("ParagraphStyle");
var textBlock = new TextBlock { TextWrapping = TextWrapping.Wrap };
if (style != null)
    textBlock.Style = style;
```

---

## 7. Extensibility model

### 7.1. Extension points

- **Block renderers:** `IBlockRenderer<TBlock>`
- **Inline renderers:** `IInlineRenderer<TInline>`
- **Post‑processors:** `IPostProcessor`
- **HTML handlers:** custom block/inline renderers for `HtmlBlock`/`HtmlInline`
- **Image loading:** service registered in `IServiceProvider`
- **Syntax highlighting:** post‑processor or specialized code block renderer
- **Math/diagram providers:** custom block types + renderers

### 7.2. Registration

Using DI or a simple builder:

```csharp
public sealed class MarkdownRendererBuilder
{
    public MarkdownRendererBuilder AddBlockRenderer<TBlock, TRenderer>()
        where TBlock : Block
        where TRenderer : IBlockRenderer<TBlock>;

    public MarkdownRendererBuilder AddInlineRenderer<TInline, TRenderer>()
        where TInline : Inline
        where TRenderer : IInlineRenderer<TInline>;

    public MarkdownRendererBuilder AddPostProcessor(IPostProcessor processor);

    public RendererDispatcher BuildDispatcher();
}
```

---

## 8. Diagnostics and observability

### 8.1. Goals

- Make failures visible and actionable.
- Provide introspection into AST and render tree.
- Enable performance analysis.

### 8.2. Features

- **AST viewer:** optional tool window or debug view that shows the Markdig AST.
- **Render tree viewer:** visual representation of the generated UIElement tree.
- **Missing renderer detection:** throws `RendererNotFoundException` with node type and context.
- **Theme resolution tracing:** logs missing theme keys and fallbacks.
- **Timing metrics:** per‑block render duration (e.g., via `Stopwatch` in debug builds).
- **Debug overlay:** optional post‑processor that draws borders/labels around rendered blocks.

---

## 9. MarkdownView control

### 9.1. Public API

```csharp
public class MarkdownView : Control
{
    public static readonly DependencyProperty MarkdownProperty;
    public static readonly DependencyProperty ThemeProperty;
    public static readonly DependencyProperty ServicesProperty;

    public string? Markdown
    {
        get => (string?)GetValue(MarkdownProperty);
        set => SetValue(MarkdownProperty, value);
    }

    public ITheme? Theme
    {
        get => (ITheme?)GetValue(ThemeProperty);
        set => SetValue(ThemeProperty, value);
    }

    public IServiceProvider? Services
    {
        get => (IServiceProvider?)GetValue(ServicesProperty);
        set => SetValue(ServicesProperty, value);
    }
}
```

### 9.2. Rendering flow

1. `Markdown` changes.
2. Control parses text using a configured **Markdig pipeline**.
3. Produces `MarkdownDocument` (AST).
4. Creates `IRenderContext` instance.
5. Uses `RendererDispatcher` to render top‑level blocks into a root `Panel` (e.g., `StackPanel`).
6. Runs registered `IPostProcessor` instances.
7. Sets the root panel as the visual content (e.g., via `ControlTemplate` with a `ContentPresenter`).

### 9.3. Async rendering (optional)

- For large documents, parsing and rendering can be done on a background thread.
- UIElement creation must occur on the UI thread; pipeline can precompute structures or chunk work.
- Support cancellation tokens to abort long renders when `Markdown` changes rapidly.

---

## 10. Markdig pipeline configuration

### 10.1. Default pipeline

- CommonMark
- GitHub‑flavored Markdown (tables, task lists)
- Emphasis extras
- Auto‑links
- Pipe tables
- Fenced code blocks
- Footnotes (optional)
- Custom extensions (admonitions, etc.)

### 10.2. Integration

```csharp
public sealed class MarkdownEngine
{
    public MarkdownEngine(MarkdownPipeline pipeline, RendererDispatcher dispatcher);

    public UIElement Render(string markdown, ITheme theme, IServiceProvider services);
}
```

---

## 11. Performance and scalability

### 11.1. Large documents

- Use a root `ItemsControl` with virtualization for top‑level blocks.
- Render each block into a container element on demand.
- Defer heavy operations (e.g., syntax highlighting) to post‑processors that can be throttled.

### 11.2. Caching

- Optional AST cache keyed by Markdown hash.
- Optional UI tree cache for static documents.

---

## 12. Project structure (suggested)

```text
MarkdownViewer.Wpf/
  Core/
    IRenderContext.cs
    IBlockRenderer.cs
    IInlineRenderer.cs
    IPostProcessor.cs
    RendererDispatcher.cs
    MarkdownEngine.cs

  Rendering/
    Blocks/
      ParagraphRenderer.cs
      HeadingRenderer.cs
      ListRenderer.cs
      CodeBlockRenderer.cs
      TableRenderer.cs
      BlockQuoteRenderer.cs
      ThematicBreakRenderer.cs
      HtmlBlockRenderer.cs
    Inlines/
      LiteralInlineRenderer.cs
      EmphasisInlineRenderer.cs
      LinkInlineRenderer.cs
      CodeInlineRenderer.cs
      HtmlInlineRenderer.cs

  Theming/
    ITheme.cs
    ResourceDictionaryTheme.cs
    DefaultTheme.xaml
    DarkTheme.xaml
    LightTheme.xaml

  Diagnostics/
    RendererNotFoundException.cs
    DiagnosticsOverlayPostProcessor.cs
    LoggingPostProcessor.cs

  Controls/
    MarkdownView.cs
    Themes/Generic.xaml

  Extensions/
    SyntaxHighlighting/
    Mermaid/
    Math/
```

---
