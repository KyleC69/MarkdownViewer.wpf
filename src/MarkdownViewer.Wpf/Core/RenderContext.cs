using System.Windows;
using System.Windows.Documents;

using MarkdownViewer.Wpf.Theming;

namespace MarkdownViewer.Wpf.Core;

internal sealed class RenderContext : IRenderContext
{
    private readonly List<IPostProcessor> postProcessors;
    private readonly RendererDispatcher rendererDispatcher;

    public RenderContext(RendererDispatcher rendererDispatcher, ITheme theme, IServiceProvider services, IEnumerable<IPostProcessor>? postProcessors = null)
    {
        this.rendererDispatcher = rendererDispatcher ?? throw new ArgumentNullException(nameof(rendererDispatcher));
        Theme = theme ?? throw new ArgumentNullException(nameof(theme));
        Services = services ?? throw new ArgumentNullException(nameof(services));
        this.postProcessors = postProcessors?.ToList() ?? [];
    }

    public ITheme Theme { get; }

    public IServiceProvider Services { get; }

    public IReadOnlyList<IPostProcessor> PostProcessors => postProcessors;

    public void AddPostProcessor(IPostProcessor processor)
    {
        ArgumentNullException.ThrowIfNull(processor);
        postProcessors.Add(processor);
    }

    public UIElement RenderBlock(Markdig.Syntax.Block block)
    {
        return rendererDispatcher.RenderBlock(block, this);
    }

    public System.Windows.Documents.Inline RenderInline(Markdig.Syntax.Inlines.Inline inline)
    {
        return rendererDispatcher.RenderInline(inline, this);
    }
}