# MarkdownViewer.Wpf Architecture Redesign Plan

## Purpose

This document defines the implementation plan to refocus MarkdownViewer.Wpf around a single XAML-based rendering architecture, improve extensibility, and create cleaner seams between responsibilities while staying in a single package.

This plan explicitly excludes:

- Testing work
- Sample applications
- Sample-driven validation work

## Target Direction

MarkdownViewer.Wpf will become a XAML-first rendering library with optional hosting support.

The redesign will adopt these principles:

- Render markdown to XAML only
- Remove FlowDocument rendering and its supporting renderer stack
- Rename the project and all owned namespaces to `MarkdownViewer.Wpf`
- Keep the package as a single distributable assembly
- Introduce explicit contracts and abstract bases for extensibility
- Separate parsing, rendering orchestration, XAML emission, styling policy, and hosting concerns
- Complete all currently incomplete block rendering
- Align supported markdown extensions with actual XAML rendering support

## Architectural Goals

### 1. XAML-Only Rendering

The package should expose one rendering path and one rendering model.

- `Markdown.ToXaml(...)` becomes the primary rendering API
- FlowDocument rendering APIs and renderer implementations are removed
- Any retained viewer control becomes a XAML consumer rather than a FlowDocument producer

### 2. Explicit Extensibility Contracts

Each rendered markdown type should have a clear extension seam.

- Introduce contracts for render context, writer services, renderer registry, and node renderers
- Provide reusable abstract base classes that contain the common logic for each renderer category
- Provide a default implementation for each supported block and inline type that derives from those base classes
- Replace ad hoc renderer wiring with explicit registration and composition
- Ensure custom block and inline rendering can be added by deriving from defaults or base classes with minimal code

### 3. Cleaner Internal Boundaries

The package remains a single package, but internal boundaries should become explicit.

- Public API stays small and stable
- Concrete renderer implementations become internal wherever possible
- Rendering orchestration, output emission, styling, and hosting are separated into dedicated layers

### 4. Complete Rendering Support

The package should not advertise support for constructs that are partially or not implemented.

- Complete HTML block rendering behavior
- Add XAML renderers for markdown extensions that are currently enabled but not rendered in XAML
- Define deterministic behavior for unsupported content

## Proposed Package Structure

The physical package remains one assembly, but code should be reorganized into the following logical areas:

### `MarkdownViewer.Wpf`

Public entry points and package-facing API.

- `Markdown`
- `MarkdownExtensions`
- retained public options/configuration types

### `MarkdownViewer.Wpf.Abstractions`

Contracts for extensibility and internal seams.

- `IXamlRenderContext`
- `IXamlNodeWriter`
- `IXamlRendererRegistry`
- `IXamlNodeRenderer<TNode>`
- abstract base renderer types for blocks and inlines

### `MarkdownViewer.Wpf.Rendering.Xaml.Defaults`

Default built-in renderer implementations that derive from shared base classes.

- default block renderer implementations
- default inline renderer implementations
- package-owned default behavior for each supported markdown element type

### `MarkdownViewer.Wpf.Rendering.Xaml`

Rendering orchestration and session-level services.

- XAML render session
- renderer composition root
- writer and formatting services
- shared emission helpers

### `MarkdownViewer.Wpf.Rendering.Xaml.Blocks`

Block renderer implementations.

- heading
- paragraph
- list
- quote
- thematic break
- code block
- html block
- extension blocks such as tables and task lists

### `MarkdownViewer.Wpf.Rendering.Xaml.Inlines`

Inline renderer implementations.

- hyperlink
- image
- code inline
- emphasis
- text
- line break
- entity and HTML inline behavior

### `MarkdownViewer.Wpf.Hosting`

Optional control and XAML-loading concerns.

- retained viewer control if kept
- XAML loading abstraction
- UI update/coalescing behavior

### `MarkdownViewer.Wpf.Styling`

Style keys, command surfaces, and styling policy.

- style resource keys
- command definitions if still needed
- style resolution/policy abstractions

## Public Surface Direction

The target public surface should stay intentionally small.

### Public

- `Markdown`
- `MarkdownExtensions`
- style and configuration types that consumers are expected to use
- a viewer host only if hosting is still part of the product direction

### Internal by Default

- concrete renderer implementations
- renderer registry internals
- emission helpers
- markdown node renderer registration details
- low-level writer mechanics

Extensibility should happen through contracts and configuration hooks rather than through direct subclassing of many concrete renderer types.

The preferred customization model is:

- derive from a reusable abstract base class when creating a new renderer family
- derive from the package default implementation for a specific element type when only small behavior changes are needed
- override only the node-specific customization points rather than rewriting full rendering logic

## Contracts to Introduce

The redesign should introduce a small contract set for renderer composition.

### Render Context

The context should provide renderer services and rendering state.

Responsibilities:

- current writer
- current markdown pipeline or render options
- styling policy access
- node dispatch access
- container and indentation state
- helper methods for child rendering

### XAML Writer Abstraction

The writer should centralize low-level emission behavior.

Responsibilities:

- writing elements and attributes
- escaping values and text
- URI encoding
- indentation and line management
- container-awareness
- common patterns such as style and command attribute emission

### Renderer Registry

The registry should own all block and inline renderer registrations.

Responsibilities:

- register renderer by markdown node type
- resolve renderer by node type
- support package defaults plus consumer extensions
- build immutable or stable render configurations

### Node Renderer Contracts

Each markdown node renderer should be aligned to a consistent contract.

Examples:

- `IXamlNodeRenderer<TNode>`
- `XamlBlockRenderer<TBlock>`
- `XamlInlineRenderer<TInline>`

The abstract base classes should hold reusable logic such as:

- common validation and null checking
- writer access patterns
- style application helpers
- common element open/close patterns
- child rendering helpers
- fallback handling hooks

Each supported markdown element type should also have a default implementation, for example:

- `DefaultParagraphRenderer`
- `DefaultHeadingRenderer`
- `DefaultListRenderer`
- `DefaultLinkInlineRenderer`

This allows implementors to customize behavior by deriving from the default implementation and overriding only the relevant extension points.

These abstractions should allow each renderer to focus only on node-specific behavior.

## Identity Alignment Plan

The redesign must explicitly include identity alignment work for the package and source tree.

### Required Rename Work

- rename the project identity to `MarkdownViewer.Wpf`
- align owned namespaces to `MarkdownViewer.Wpf` and its sub-namespaces
- align documentation and package-facing naming to `MarkdownViewer.Wpf`
- remove any remaining legacy project naming from code paths retained after the redesign

This rename work is part of the redesign implementation rather than a separate cleanup effort.

## Rendering Composition Model

The current constructor-driven registration model should be replaced.

### Current Problem

The XAML renderer currently registers its renderers directly in its constructor, which hard-codes composition and makes extensibility indirect and brittle.

### Target Model

Use a dedicated composition root that:

- builds the default registry once
- registers all built-in block and inline renderers
- supports optional extension hooks
- creates render sessions that use the registry without re-registering renderers per call

This composition root becomes the only place that defines default rendering behavior.

## FlowDocument Removal Plan

FlowDocument support is removed as part of the redesign.

### Remove

- `Markdown.ToFlowDocument(...)`
- FlowDocument-oriented rendering infrastructure
- WPF renderer classes and WPF object renderers
- direct dependency on FlowDocument rendering as a primary architectural path

### Retain Only if Needed for Hosting

- WPF-specific hosting types that load generated XAML for display
- style resources needed by the XAML output contract

## Hosting Strategy

If `MarkdownViewer` is retained, it should be repositioned as a XAML-consuming host.

### Responsibilities of Hosting Layer

- accept markdown input
- request XAML from the rendering layer
- load generated XAML through a dedicated loader seam
- manage UI-thread update timing and coalescing

### Responsibilities That Move Out of Hosting

- markdown parsing decisions
- markdown node rendering logic
- direct XAML string composition
- styling behavior beyond selecting or exposing theme resources

This keeps rendering pure and hosting optional.

## Block Rendering Completion Plan

All block-level rendering must become explicit and complete.

### Immediate Completion Item

- Implement `HtmlBlockRenderer`

### Required Policy Decision

For markdown content that cannot be faithfully mapped to XAML, define one documented behavior:

- escape and preserve as text
- omit intentionally
- transform into a fallback representation

Every block renderer must follow a deterministic policy.

## Extension Parity Plan

The extension list enabled by `UseSupportedExtensions()` must match actual XAML support.

### Currently Required Additions

- XAML table renderer support
- XAML task list renderer support

### Required Outcome

If an extension is enabled by default, the XAML rendering pipeline must include a corresponding renderer and a styling path for its output.

If an extension is not supported, it should not be enabled by the default helper.

## Performance Plan

Performance work is part of the redesign and not a follow-up phase.

### Rendering Performance Goals

- keep streaming output to a `TextWriter`
- avoid repeated renderer registration per render
- reduce repeated ad hoc string composition inside individual node renderers
- centralize style and attribute writing in the writer abstraction
- reduce duplicate allocation patterns for repeated markup fragments

### Hosting Performance Goals

If a viewer host remains:

- coalesce repeated markdown updates
- avoid redundant renders for identical content and options
- keep expensive rendering work off the immediate UI update path where possible

## Implementation Phases

### Phase 1. Freeze Direction and Public Surface

- Confirm XAML-only rendering direction in code and documentation
- confirm the target project and namespace identity as `MarkdownViewer.Wpf`
- Mark FlowDocument rendering for removal
- Define the target public API set
- Define the supported extension set for the redesign

### Phase 2. Introduce Contracts and Composition Root

- Add rendering abstractions under `Abstractions`
- Add the XAML writer abstraction
- Add registry/composition types
- Add reusable abstract base classes for block and inline renderer categories
- Define the default renderer class model for each supported markdown element type
- Implement render session orchestration around the new contracts

### Phase 3. Reorganize Internal Structure

- Rename project and owned namespaces to `MarkdownViewer.Wpf`
- Move code into the new logical namespaces and folders
- Add a defaults area for package-provided renderer implementations
- Separate blocks, inlines, styling, hosting, and orchestration
- Convert current XAML renderer support classes to the new architecture

### Phase 4. Migrate Existing XAML Renderers

- Rebuild paragraph, list, heading, quote, code block, and thematic break renderers as default implementations on the new base contracts
- Rebuild inline renderers as default implementations on the new inline contract model
- Move shared formatting logic out of concrete renderers and into the writer/context seam

### Phase 5. Complete Missing Rendering

- Implement HTML block rendering behavior
- Add XAML table rendering
- Add XAML task list rendering
- Confirm all enabled extensions have corresponding XAML output behavior

### Phase 6. Refactor Hosting

- Rework `MarkdownViewer` as a XAML-consuming host if it remains
- Introduce a XAML loader abstraction if needed
- Add update coalescing and cleaner scheduling boundaries

### Phase 7. Remove Old FlowDocument Stack

- Delete FlowDocument APIs
- Remove WPF renderer infrastructure
- remove outdated namespaces and code paths tied to direct FlowDocument object construction

### Phase 8. Tighten Visibility and Finalize Seams

- make concrete renderer internals non-public where appropriate
- preserve only stable customization contracts publicly
- finalize style and command seams
- remove transitional compatibility code

## Definition of Done

The redesign is complete when all of the following are true:

- The package renders markdown to XAML only
- FlowDocument rendering infrastructure has been removed
- The project and owned namespaces are aligned to `MarkdownViewer.Wpf`
- Extensibility is provided through explicit contracts, reusable abstract bases, and default implementations for each supported element type
- Rendering composition is registry-driven rather than constructor-driven
- Parsing, orchestration, emission, styling, and hosting have clean seams
- HTML block rendering is complete and deterministic
- Table and task-list XAML rendering are implemented or default extension behavior is adjusted to match reality
- The public API surface is intentionally small and stable
- The package remains a single distributable package

## Non-Goals

This redesign plan does not include:

- Unit test implementation planning
- Integration test planning
- Benchmark harness work
- Sample application creation or migration
- Documentation beyond the architectural redesign and implementation direction
