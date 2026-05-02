// Solution: MarkdownViewer.Wpf
// Project:   MarkdownViewer.Wpf
// File:         RendererDispatcher.cs
// Author: Kyle L. Crowder
// Build Date: 2026/05/02



using System.Diagnostics;
using System.Windows;
using System.Windows.Documents;

using MarkdownViewer.Wpf.Diagnostics;




namespace MarkdownViewer.Wpf.Core;





public sealed class RendererDispatcher
{
    private readonly Dictionary<Type, object> blockRenderers;
    private readonly Dictionary<Type, object> inlineRenderers;








    public RendererDispatcher(IEnumerable<object> blockRenderers, IEnumerable<object> inlineRenderers)
    {
        this.blockRenderers = IndexRenderers(blockRenderers, typeof(IBlockRenderer<>));
        this.inlineRenderers = IndexRenderers(inlineRenderers, typeof(IInlineRenderer<>));
    }








    internal static Dictionary<Type, object> IndexRenderers(IEnumerable<object> renderers, Type genericInterface)
    {
        ArgumentNullException.ThrowIfNull(renderers);

        Dictionary<Type, object> index = [];
        foreach (var renderer in renderers)
        {
            ArgumentNullException.ThrowIfNull(renderer);

            Type rendererType = renderer.GetType();
            Type? handledType = rendererType.GetInterfaces().FirstOrDefault(candidate => candidate.IsGenericType && candidate.GetGenericTypeDefinition() == genericInterface)?.GetGenericArguments()[0];

            if (handledType is null)
            {
                throw new InvalidOperationException($"Renderer type {rendererType.FullName} does not implement {genericInterface.Name}.");
            }

            index[handledType] = renderer;
        }

        return index;
    }








    public UIElement RenderBlock(Markdig.Syntax.Block block, IRenderContext context)
    {
        ArgumentNullException.ThrowIfNull(block);
        ArgumentNullException.ThrowIfNull(context);

        if (!blockRenderers.TryGetValue(block.GetType(), out var renderer))
        {
            throw new RendererNotFoundException(block.GetType(), $"No block renderer is registered for markdown block type {block.GetType().FullName}.");
        }

        Stopwatch stopwatch = Stopwatch.StartNew();
        UIElement result = ((dynamic)renderer).Render((dynamic)block, context);
        stopwatch.Stop();
        MarkdownDiagnostics.ReportNodeRendered(block.GetType(), stopwatch.Elapsed, isBlock: true);
        return result;
    }








    public Inline RenderInline(Markdig.Syntax.Inlines.Inline inline, IRenderContext context)
    {
        ArgumentNullException.ThrowIfNull(inline);
        ArgumentNullException.ThrowIfNull(context);

        if (!inlineRenderers.TryGetValue(inline.GetType(), out var renderer))
        {
            throw new RendererNotFoundException(inline.GetType(), $"No inline renderer is registered for markdown inline type {inline.GetType().FullName}.");
        }

        Stopwatch stopwatch = Stopwatch.StartNew();
        Inline result = ((dynamic)renderer).Render((dynamic)inline, context);
        stopwatch.Stop();
        MarkdownDiagnostics.ReportNodeRendered(inline.GetType(), stopwatch.Elapsed, isBlock: false);
        return result;
    }
}