using Markdig;
using Markdig.Extensions.AutoIdentifiers;
using Markdig.Extensions.AutoLinks;
using Markdig.Extensions.EmphasisExtras;
using Markdig.Extensions.Tables;
using Markdig.Extensions.TaskLists;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

using MarkdownViewer.Wpf.Rendering.Blocks;
using MarkdownViewer.Wpf.Rendering.Inlines;

namespace MarkdownViewer.Wpf.Core;

public sealed class MarkdownRendererBuilder
{
    private readonly List<object> blockRenderers = [];
    private readonly List<object> inlineRenderers = [];
    private readonly List<IPostProcessor> postProcessors = [];

    public MarkdownRendererBuilder AddBlockRenderer<TBlock, TRenderer>()
        where TBlock : Block
        where TRenderer : IBlockRenderer<TBlock>
    {
        object renderer = Activator.CreateInstance(typeof(TRenderer))
            ?? throw new InvalidOperationException($"Unable to create renderer instance for {typeof(TRenderer).FullName}.");

        blockRenderers.Add(renderer);
        return this;
    }

    public MarkdownRendererBuilder AddBlockRenderer<TBlock>(IBlockRenderer<TBlock> renderer)
        where TBlock : Block
    {
        ArgumentNullException.ThrowIfNull(renderer);
        blockRenderers.Add(renderer);
        return this;
    }

    public MarkdownRendererBuilder AddInlineRenderer<TInline, TRenderer>()
        where TInline : Markdig.Syntax.Inlines.Inline
        where TRenderer : IInlineRenderer<TInline>
    {
        object renderer = Activator.CreateInstance(typeof(TRenderer))
            ?? throw new InvalidOperationException($"Unable to create renderer instance for {typeof(TRenderer).FullName}.");

        inlineRenderers.Add(renderer);
        return this;
    }

    public MarkdownRendererBuilder AddInlineRenderer<TInline>(IInlineRenderer<TInline> renderer)
        where TInline : Markdig.Syntax.Inlines.Inline
    {
        ArgumentNullException.ThrowIfNull(renderer);
        inlineRenderers.Add(renderer);
        return this;
    }

    public MarkdownRendererBuilder AddPostProcessor(IPostProcessor processor)
    {
        ArgumentNullException.ThrowIfNull(processor);
        postProcessors.Add(processor);
        return this;
    }

    public RendererDispatcher BuildDispatcher()
    {
        return new RendererDispatcher(blockRenderers, inlineRenderers);
    }

    internal IReadOnlyList<IPostProcessor> BuildPostProcessors()
    {
        return postProcessors.ToArray();
    }

    public static MarkdownRendererBuilder CreateDefault()
    {
        return new MarkdownRendererBuilder()
            .AddBlockRenderer<ParagraphBlock, ParagraphRenderer>()
            .AddBlockRenderer<HeadingBlock, HeadingRenderer>()
            .AddBlockRenderer<ListBlock, ListRenderer>()
            .AddBlockRenderer<ListItemBlock, ListItemRenderer>()
            .AddBlockRenderer<CodeBlock, CodeBlockRenderer>()
            .AddBlockRenderer<FencedCodeBlock, FencedCodeBlockRenderer>()
            .AddBlockRenderer<QuoteBlock, BlockQuoteRenderer>()
            .AddBlockRenderer<ThematicBreakBlock, ThematicBreakRenderer>()
            .AddBlockRenderer<Table, TableRenderer>()
            .AddBlockRenderer<HtmlBlock, HtmlBlockRenderer>()
            .AddInlineRenderer<LiteralInline, LiteralInlineRenderer>()
            .AddInlineRenderer<EmphasisInline, EmphasisInlineRenderer>()
            .AddInlineRenderer<LinkInline, LinkInlineRenderer>()
            .AddInlineRenderer<AutolinkInline, AutoLinkInlineRenderer>()
            .AddInlineRenderer<CodeInline, CodeInlineRenderer>()
            .AddInlineRenderer<LineBreakInline, LineBreakInlineRenderer>()
            .AddInlineRenderer<HtmlInline, HtmlInlineRenderer>()
            .AddInlineRenderer<HtmlEntityInline, HtmlEntityInlineRenderer>()
            .AddInlineRenderer<DelimiterInline, DelimiterInlineRenderer>()
            .AddInlineRenderer<TaskList, TaskListInlineRenderer>();
    }
}