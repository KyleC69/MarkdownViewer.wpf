// Solution: MarkdownViewer.Wpf
// Project:   MarkdownViewer.Wpf.Tests
// File:         MarkdownTestHelper.cs
// Author: Kyle L. Crowder
// Build Date: 2026/05/02



using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

using Markdig;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

using MarkdownViewer.Wpf.Controls;
using MarkdownViewer.Wpf.Core;

using Xunit;




namespace MarkdownViewer.Wpf.Tests;





internal static class MarkdownTestHelper
{
    public static readonly MarkdownPipeline Pipeline = MarkdownEngine.CreateDefaultPipeline();








    public static RenderContext CreateContext(RendererDispatcher? dispatcher = null, ResourceDictionary? themeResources = null, IServiceProvider? services = null)
    {
        ResourceDictionary effectiveThemeResources = themeResources ?? new ResourceDictionary();
        ResourceDictionary scopedResources = new();
        if (effectiveThemeResources.Count > 0 || effectiveThemeResources.MergedDictionaries.Count > 0)
        {
            scopedResources.MergedDictionaries.Add(effectiveThemeResources);
        }

        return new RenderContext(dispatcher ?? MarkdownRendererBuilder.CreateDefault().BuildDispatcher(), effectiveThemeResources, scopedResources, services ?? EmptyServiceProvider.Instance);
    }








    public static IServiceProvider CreateServiceProvider(params (Type serviceType, object implementation)[] services)
    {
        return new TestServiceProvider(services);
    }








    public static ResourceDictionary CreateThemeResources(params (object key, object value)[] entries)
    {
        ResourceDictionary dictionary = new();
        foreach (var (key, value) in entries) dictionary[key] = value;

        return dictionary;
    }








    private static T? FindBlock<T>(Markdig.Syntax.Block block) where T : Markdig.Syntax.Block
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








    public static T FindFirstBlock<T>(string markdown) where T : Markdig.Syntax.Block
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








    public static T FindFirstInline<T>(string markdown) where T : Markdig.Syntax.Inlines.Inline
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








    private static T? FindInline<T>(Markdig.Syntax.Block block) where T : Markdig.Syntax.Inlines.Inline
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








    private static T? FindInline<T>(ContainerInline container) where T : Markdig.Syntax.Inlines.Inline
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








    private static IEnumerable<System.Windows.Documents.Inline> FlattenInline(System.Windows.Documents.Inline inline)
    {
        yield return inline;

        if (inline is Span span)
        {
            foreach (System.Windows.Documents.Inline child in span.Inlines)
            {
                foreach (System.Windows.Documents.Inline descendant in FlattenInline(child)) yield return descendant;
            }
        }
    }








    public static IReadOnlyList<System.Windows.Documents.Inline> FlattenInlines(TextBlock textBlock)
    {
        return textBlock.Inlines.SelectMany(FlattenInline).ToArray();
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
                _ => string.Empty
        };
    }








    public static MarkdownDocument Parse(string markdown)
    {
        return Markdown.Parse(markdown, Pipeline);
    }








    public static T ParseFirstBlock<T>(string markdown) where T : Markdig.Syntax.Block
    {
        return Assert.IsType<T>(Parse(markdown)[0]);
    }








    public static MarkdownRootPanel RenderToPanel(string markdown, MarkdownEngine? engine = null, ResourceDictionary? themeResources = null, IServiceProvider? services = null)
    {
        engine ??= MarkdownEngine.CreateDefault();
        services ??= EmptyServiceProvider.Instance;

        return Assert.IsType<MarkdownRootPanel>(engine.Render(markdown, services, themeResources));
    }
}





internal sealed class TestServiceProvider(params (Type serviceType, object implementation)[] services) : IServiceProvider
{
    private readonly Dictionary<Type, object> services = services.ToDictionary(static item => item.serviceType, static item => item.implementation);








    public object? GetService(Type serviceType)
    {
        return services.TryGetValue(serviceType, out var implementation) ? implementation : null;
    }
}