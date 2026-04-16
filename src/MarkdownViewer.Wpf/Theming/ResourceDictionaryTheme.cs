using System.Windows;
using System.Windows.Media;

using MarkdownViewer.Wpf.Diagnostics;

namespace MarkdownViewer.Wpf.Theming;

public class ResourceDictionaryTheme : ITheme
{
    private static readonly Lock DictionaryLoadLock = new();
    private readonly ResourceDictionary resourceDictionary;

    public ResourceDictionaryTheme(ResourceDictionary resourceDictionary)
    {
        this.resourceDictionary = resourceDictionary ?? throw new ArgumentNullException(nameof(resourceDictionary));
    }

    public Brush? GetBrush(string key)
    {
        return FindResource(key) as Brush;
    }

    public double? GetFontSize(string key)
    {
        return FindResource(key) switch
        {
            double value => value,
            _ => null,
        };
    }

    public Style? GetStyle(string key)
    {
        return FindResource(key) as Style;
    }

    public Thickness? GetThickness(string key)
    {
        return FindResource(key) switch
        {
            Thickness thickness => thickness,
            _ => null,
        };
    }

    protected internal object? FindResource(string key)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);

        object? resource = TryFindResource(resourceDictionary, key);
        if (resource is null)
        {
            MarkdownDiagnostics.ReportThemeKeyMissing(GetType(), key);
        }

        return resource;
    }

    internal static object? TryFindResource(ResourceDictionary dictionary, string key)
    {
        if (dictionary.Contains(key))
        {
            return dictionary[key];
        }

        foreach (ResourceDictionary mergedDictionary in dictionary.MergedDictionaries)
        {
            object? match = TryFindResource(mergedDictionary, key);
            if (match is not null)
            {
                return match;
            }
        }

        return null;
    }

    protected internal static ResourceDictionary LoadDictionary(string relativePackUri)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(relativePackUri);

        lock (DictionaryLoadLock)
        {
            return (ResourceDictionary)Application.LoadComponent(new Uri(relativePackUri, UriKind.Relative));
        }
    }
}