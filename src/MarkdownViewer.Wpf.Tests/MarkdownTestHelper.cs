using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

using Markdig;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

using MarkdownViewer.Wpf.Core;
using MarkdownViewer.Wpf.Theming;

using Xunit;

namespace MarkdownViewer.Wpf.Tests;

internal static class MarkdownTestHelper
{
    public static readonly MarkdownPipeline Pipeline = MarkdownEngine.CreateDefaultPipeline();

    public static ResourceDictionaryTheme CreateTheme(params (string key, object value)[] entries)
    {
        ResourceDictionary dictionary = new();
        foreach ((string key, object value) in entries)
        {
            dictionary[key] = value;
        }

        return new ResourceDictionaryTheme(dictionary);
    }

    public static RenderContext CreateContext(RendererDispatcher? dispatcher = null, ITheme? theme = null, IServiceProvider? services = null)
    {
        return new RenderContext(
            dispatcher ?? MarkdownRendererBuilder.CreateDefault().BuildDispatcher(),
            theme ?? new DefaultTheme(),
            services ?? EmptyServiceProvider.Instance);
    }

    public static IServiceProvider CreateServiceProvider(params (Type serviceType, object implementation)[] services)
    {
        return new TestServiceProvider(services);
    }

    public static StackPanel RenderToPanel(string markdown, MarkdownEngine? engine = null, ITheme? theme = null, IServiceProvider? services = null)
    {
        engine ??= MarkdownEngine.CreateDefault();
        theme ??= new DefaultTheme();
        services ??= EmptyServiceProvider.Instance;

        return Assert.IsType<StackPanel>(engine.Render(markdown, theme, services));
    }

    public static MarkdownDocument Parse(string markdown)
    {
        return Markdig.Markdown.Parse(markdown, Pipeline);
    }

    public static T ParseFirstBlock<T>(string markdown)
        where T : Markdig.Syntax.Block
    {
        return Assert.IsType<T>(Parse(markdown)[0]);
    }

    public static T FindFirstBlock<T>(string markdown)
        where T : Markdig.Syntax.Block
    {
        foreach (Markdig.Syntax.Block block in Parse(markdown))
        {
            T? match = FindBlock<T>(block);
            if (match is not null)
            {
                return match;
            }
        }

        throw new InvalidOperationException($"Unable to find markdown block of type {typeof(T).Name}.");
    }

    public static T FindFirstInline<T>(string markdown)
        where T : Markdig.Syntax.Inlines.Inline
    {
        foreach (Markdig.Syntax.Block block in Parse(markdown))
        {
            T? match = FindInline<T>(block);
            if (match is not null)
            {
                return match;
            }
        }

        throw new InvalidOperationException($"Unable to find markdown inline of type {typeof(T).Name}.");
    }

    public static string GetInlineText(TextBlock textBlock)
    {
        return string.Concat(textBlock.Inlines.Select(GetInlineText));
    }

    public static string GetInlineText(System.Windows.Documents.Inline inline)
    {
        return inline switch
        {
            Run run => run.Text,
            Span span => string.Concat(span.Inlines.Select(GetInlineText)),
            LineBreak => Environment.NewLine,
            _ => string.Empty,
        };
    }

    public static IReadOnlyList<System.Windows.Documents.Inline> FlattenInlines(TextBlock textBlock)
    {
        return textBlock.Inlines.SelectMany(FlattenInline).ToArray();
    }

    private static IEnumerable<System.Windows.Documents.Inline> FlattenInline(System.Windows.Documents.Inline inline)
    {
        yield return inline;

        if (inline is Span span)
        {
            foreach (System.Windows.Documents.Inline child in span.Inlines)
            {
                foreach (System.Windows.Documents.Inline descendant in FlattenInline(child))
                {
                    yield return descendant;
                }
            }
        }
    }

    private static T? FindBlock<T>(Markdig.Syntax.Block block)
        where T : Markdig.Syntax.Block
    {
        if (block is T match)
        {
            return match;
        }

        if (block is ContainerBlock containerBlock)
        {
            foreach (Markdig.Syntax.Block child in containerBlock)
            {
                T? nested = FindBlock<T>(child);
                if (nested is not null)
                {
                    return nested;
                }
            }
        }

        return null;
    }

    private static T? FindInline<T>(Markdig.Syntax.Block block)
        where T : Markdig.Syntax.Inlines.Inline
    {
        if (block is LeafBlock leafBlock && leafBlock.Inline is not null)
        {
            T? leafInline = FindInline<T>(leafBlock.Inline);
            if (leafInline is not null)
            {
                return leafInline;
            }
        }

        if (block is ContainerBlock containerBlock)
        {
            foreach (Markdig.Syntax.Block child in containerBlock)
            {
                T? nested = FindInline<T>(child);
                if (nested is not null)
                {
                    return nested;
                }
            }
        }

        return null;
    }

    private static T? FindInline<T>(ContainerInline container)
        where T : Markdig.Syntax.Inlines.Inline
    {
        for (Markdig.Syntax.Inlines.Inline? current = container.FirstChild; current is not null; current = current.NextSibling)
        {
            if (current is T match)
            {
                return match;
            }

            if (current is ContainerInline nestedContainer)
            {
                T? nested = FindInline<T>(nestedContainer);
                if (nested is not null)
                {
                    return nested;
                }
            }
        }

        return null;
    }
}

internal sealed class TestServiceProvider(params (Type serviceType, object implementation)[] services) : IServiceProvider
{
    private readonly Dictionary<Type, object> services = services.ToDictionary(static item => item.serviceType, static item => item.implementation);

    public object? GetService(Type serviceType)
    {
        return services.TryGetValue(serviceType, out object? implementation)
            ? implementation
            : null;
    }
}

internal sealed class FakeTheme : ITheme
{
    public Func<string, Brush?> BrushResolver { get; init; } = static _ => null;

    public Func<string, double?> FontSizeResolver { get; init; } = static _ => null;

    public Func<string, Style?> StyleResolver { get; init; } = static _ => null;

    public Func<string, Thickness?> ThicknessResolver { get; init; } = static _ => null;

    public Brush? GetBrush(string key) => BrushResolver(key);

    public double? GetFontSize(string key) => FontSizeResolver(key);

    public Style? GetStyle(string key) => StyleResolver(key);

    public Thickness? GetThickness(string key) => ThicknessResolver(key);
}