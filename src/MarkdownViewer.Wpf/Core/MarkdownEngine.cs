using System.Windows;
using System.Windows.Controls;

using Markdig;
using Markdig.Extensions.AutoIdentifiers;
using Markdig.Syntax;

using MarkdownViewer.Wpf.Theming;

namespace MarkdownViewer.Wpf.Core;

public sealed class MarkdownEngine
{
    private readonly RendererDispatcher dispatcher;
    private readonly MarkdownPipeline pipeline;
    private readonly IReadOnlyList<IPostProcessor> postProcessors;

    public MarkdownEngine(MarkdownPipeline pipeline, RendererDispatcher dispatcher)
        : this(pipeline, dispatcher, Array.Empty<IPostProcessor>())
    {
    }

    internal MarkdownEngine(MarkdownPipeline pipeline, RendererDispatcher dispatcher, IReadOnlyList<IPostProcessor> postProcessors)
    {
        this.pipeline = pipeline ?? throw new ArgumentNullException(nameof(pipeline));
        this.dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
        this.postProcessors = postProcessors ?? throw new ArgumentNullException(nameof(postProcessors));
    }

    public UIElement Render(string markdown, IServiceProvider services, ResourceDictionary? themeResources = null)
    {
        ArgumentNullException.ThrowIfNull(markdown);
        ArgumentNullException.ThrowIfNull(services);

        MarkdownDocument document = Markdig.Markdown.Parse(markdown, pipeline);
        ResourceDictionary effectiveThemeResources = themeResources ?? new ResourceDictionary();
        ResourceDictionary scopedResources = new();
        if (effectiveThemeResources.Count > 0 || effectiveThemeResources.MergedDictionaries.Count > 0)
        {
            scopedResources.MergedDictionaries.Add(effectiveThemeResources);
        }

        StackPanel root = new()
        {
            Orientation = Orientation.Vertical,
            Resources = scopedResources,
        };
        RenderHelpers.ApplyRole(root, ThemeKeys.RootPanelStyle);

        RenderContext context = new(dispatcher, effectiveThemeResources, scopedResources, services, postProcessors);
        foreach (Block block in document)
        {
            root.Children.Add(context.RenderBlock(block));
        }

        foreach (IPostProcessor postProcessor in context.PostProcessors)
        {
            postProcessor.Process(root, context);
        }

        return root;
    }

    public static MarkdownPipeline CreateDefaultPipeline()
    {
        return new MarkdownPipelineBuilder()
            .UseEmphasisExtras()
            .UseAutoIdentifiers(AutoIdentifierOptions.GitHub)
            .UsePipeTables()
            .UseGridTables()
            .UseTaskLists()
            .UseAutoLinks()
            .Build();
    }

    public static MarkdownEngine CreateDefault()
    {
        MarkdownRendererBuilder builder = MarkdownRendererBuilder.CreateDefault();
        return new MarkdownEngine(CreateDefaultPipeline(), builder.BuildDispatcher(), builder.BuildPostProcessors());
    }
}