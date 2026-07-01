// Solution: MarkdownViewer.Wpf
// Project:   MarkdownViewer.Wpf
// File:         RenderContext.cs
// Author: Kyle L. Crowder
// Build Date: 2026/05/02



using System.Windows;
using System.Windows.Documents;




namespace MarkdownViewer.Wpf.Core;





internal sealed class RenderContext : IRenderContext
{
    private readonly List<IPostProcessor> postProcessors;
    private readonly RendererDispatcher rendererDispatcher;








    public RenderContext(RendererDispatcher rendererDispatcher, ResourceDictionary themeResources, ResourceDictionary resources, IServiceProvider services, IEnumerable<IPostProcessor>? postProcessors = null)
        : this(rendererDispatcher, themeResources, resources, services, postProcessors, wordWrap: true)
    {
    }

    public RenderContext(RendererDispatcher rendererDispatcher, ResourceDictionary themeResources, ResourceDictionary resources, IServiceProvider services, IEnumerable<IPostProcessor>? postProcessors, bool wordWrap)
    {
        this.rendererDispatcher = rendererDispatcher ?? throw new ArgumentNullException(nameof(rendererDispatcher));
        ThemeResources = themeResources ?? throw new ArgumentNullException(nameof(themeResources));
        Resources = resources ?? throw new ArgumentNullException(nameof(resources));
        Services = services ?? throw new ArgumentNullException(nameof(services));
        this.postProcessors = postProcessors?.ToList() ?? [];
        WordWrap = wordWrap;
    }








    public IReadOnlyList<IPostProcessor> PostProcessors
    {
        get { return postProcessors; }
    }








    public void AddPostProcessor(IPostProcessor processor)
    {
        ArgumentNullException.ThrowIfNull(processor);
        postProcessors.Add(processor);
    }








    public UIElement RenderBlock(Markdig.Syntax.Block block)
    {
        return rendererDispatcher.RenderBlock(block, this);
    }








    public Inline RenderInline(Markdig.Syntax.Inlines.Inline inline)
    {
        return rendererDispatcher.RenderInline(inline, this);
    }








    public ResourceDictionary Resources { get; }

    public IServiceProvider Services { get; }

    public ResourceDictionary ThemeResources { get; }

    public bool WordWrap { get; }
}
