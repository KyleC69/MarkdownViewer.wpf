// Solution: MarkdownViewer.Wpf
// Project:   MarkdownViewer.Wpf
// File:         MarkdownEngine.cs
// Author: Kyle L. Crowder
// Build Date: 2026/05/02



using System.Collections;
using System.Windows;

using Markdig;
using Markdig.Extensions.AutoIdentifiers;
using Markdig.Syntax;

using MarkdownViewer.Wpf.Controls;




namespace MarkdownViewer.Wpf.Core;





public sealed class MarkdownEngine
{
    private readonly RendererDispatcher dispatcher;
    private readonly MarkdownPipeline pipeline;
    private readonly IReadOnlyList<IPostProcessor> postProcessors;








    public MarkdownEngine(MarkdownPipeline pipeline, RendererDispatcher dispatcher) : this(pipeline, dispatcher, Array.Empty<IPostProcessor>())
    {
    }








    internal MarkdownEngine(MarkdownPipeline pipeline, RendererDispatcher dispatcher, IReadOnlyList<IPostProcessor> postProcessors)
    {
        this.pipeline = pipeline ?? throw new ArgumentNullException(nameof(pipeline));
        this.dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
        this.postProcessors = postProcessors ?? throw new ArgumentNullException(nameof(postProcessors));
    }








    public static MarkdownEngine CreateDefault()
    {
        MarkdownRendererBuilder builder = MarkdownRendererBuilder.CreateDefault();
        return new MarkdownEngine(CreateDefaultPipeline(), builder.BuildDispatcher(), builder.BuildPostProcessors());
    }








    public static MarkdownPipeline CreateDefaultPipeline()
    {
        return new MarkdownPipelineBuilder().UseEmphasisExtras().UseAutoIdentifiers(AutoIdentifierOptions.GitHub).UsePipeTables().UseGridTables().UseTaskLists().UseAutoLinks().Build();
    }








    internal static void MergeThemeResources(ResourceDictionary target, ResourceDictionary source)
    {
        foreach (DictionaryEntry entry in source)
        {
            target[entry.Key] = entry.Value;
        }

        foreach (ResourceDictionary mergedDictionary in source.MergedDictionaries)
        {
            target.MergedDictionaries.Add(mergedDictionary);
        }
    }








    public UIElement Render(string markdown, IServiceProvider services, ResourceDictionary? themeResources = null)
    {
        ArgumentNullException.ThrowIfNull(markdown);
        ArgumentNullException.ThrowIfNull(services);

        MarkdownDocument document = Markdown.Parse(markdown, pipeline);
        ResourceDictionary effectiveThemeResources = themeResources ?? new ResourceDictionary();
        ResourceDictionary scopedResources = new();
        MergeThemeResources(scopedResources, effectiveThemeResources);

        MarkdownRootPanel root = new() { Resources = scopedResources };

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
}