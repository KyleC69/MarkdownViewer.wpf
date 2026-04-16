using System.Windows;

namespace MarkdownViewer.Wpf.Theming;

internal static class WpfResourceLookup
{
    public static object? TryFindResource(ResourceDictionary dictionary, object key)
    {
        ArgumentNullException.ThrowIfNull(dictionary);
        ArgumentNullException.ThrowIfNull(key);

        if (dictionary.Contains(key))
        {
            return dictionary[key];
        }

        for (int index = dictionary.MergedDictionaries.Count - 1; index >= 0; index--)
        {
            object? match = TryFindResource(dictionary.MergedDictionaries[index], key);
            if (match is not null)
            {
                return match;
            }
        }

        return null;
    }
}