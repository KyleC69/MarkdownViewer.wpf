using System.Windows;

using MarkdownViewer.Wpf.Core;

using Xunit;

namespace MarkdownViewer.Wpf.Tests;

public sealed class MergeThemeResourcesTests
{
    [Fact]
    public void MergeThemeResources_CopiesDirectEntries_FromSourceToTarget()
    {
        ResourceDictionary source = new();
        source["key1"] = "value1";
        source["key2"] = 42;
        ResourceDictionary target = new();

        MarkdownEngine.MergeThemeResources(target, source);

        Assert.Equal("value1", target["key1"]);
        Assert.Equal(42, target["key2"]);
    }

    [Fact]
    public void MergeThemeResources_AddsMergedDictionaries_FromSourceToTarget()
    {
        ResourceDictionary inner = new();
        inner["inner"] = "innerValue";
        ResourceDictionary source = new();
        source.MergedDictionaries.Add(inner);
        ResourceDictionary target = new();

        MarkdownEngine.MergeThemeResources(target, source);

        Assert.Contains(inner, target.MergedDictionaries);
    }

    [Fact]
    public void MergeThemeResources_CopiesBothDirectEntriesAndMergedDictionaries()
    {
        ResourceDictionary inner = new();
        ResourceDictionary source = new();
        source["direct"] = "directValue";
        source.MergedDictionaries.Add(inner);
        ResourceDictionary target = new();

        MarkdownEngine.MergeThemeResources(target, source);

        Assert.Equal("directValue", target["direct"]);
        Assert.Single(target.MergedDictionaries);
    }

    [Fact]
    public void MergeThemeResources_DoesNothing_WhenSourceIsEmpty()
    {
        ResourceDictionary source = new();
        ResourceDictionary target = new();

        MarkdownEngine.MergeThemeResources(target, source);

        Assert.Empty(target.Keys);
        Assert.Empty(target.MergedDictionaries);
    }

    [Fact]
    public void MergeThemeResources_OverwritesExistingKey_WhenTargetAlreadyHasSameKey()
    {
        ResourceDictionary source = new();
        source["key"] = "newValue";
        ResourceDictionary target = new();
        target["key"] = "oldValue";

        MarkdownEngine.MergeThemeResources(target, source);

        Assert.Equal("newValue", target["key"]);
    }

    [Fact]
    public void MergeThemeResources_PreservesMultipleMergedDictionaries_FromSource()
    {
        ResourceDictionary inner1 = new();
        ResourceDictionary inner2 = new();
        ResourceDictionary source = new();
        source.MergedDictionaries.Add(inner1);
        source.MergedDictionaries.Add(inner2);
        ResourceDictionary target = new();

        MarkdownEngine.MergeThemeResources(target, source);

        Assert.Equal(2, target.MergedDictionaries.Count);
        Assert.Contains(inner1, target.MergedDictionaries);
        Assert.Contains(inner2, target.MergedDictionaries);
    }
}
