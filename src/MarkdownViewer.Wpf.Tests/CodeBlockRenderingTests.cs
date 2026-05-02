using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

using Markdig.Syntax;

using MarkdownViewer.Wpf.Controls;
using MarkdownViewer.Wpf.Core;
using MarkdownViewer.Wpf.Rendering.Blocks;

using Xunit;

namespace MarkdownViewer.Wpf.Tests;

public sealed class CodeBlockRenderingTests
{
    [StaFact]
    public void FencedCodeBlock_RendersLanguageHeaderAndCopyButton()
    {
        MarkdownEngine engine = MarkdownEngine.CreateDefault();

        MarkdownRootPanel root = Assert.IsType<MarkdownRootPanel>(engine.Render("```csharp\nConsole.WriteLine(1);\n```", EmptyServiceProvider.Instance));
        CodeBlockBorder border = Assert.IsType<CodeBlockBorder>(root.Children[0]);

        Grid grid = Assert.IsType<Grid>(border.Child);
        Assert.Equal(2, grid.RowDefinitions.Count);

        CodeBlockHeaderBorder headerBorder = Assert.IsType<CodeBlockHeaderBorder>(grid.Children[0]);
        CodeBlockHeaderPanel header = Assert.IsType<CodeBlockHeaderPanel>(headerBorder.Child);
        CodeBlockHeaderTextBlock language = Assert.IsType<CodeBlockHeaderTextBlock>(header.Children[0]);
        CodeBlockCopyButton copy = Assert.IsType<CodeBlockCopyButton>(header.Children[1]);

        Assert.Equal("csharp", language.Text);
        Assert.Equal("Copy", copy.Content);

        CodeBlockScrollViewer viewer = Assert.IsType<CodeBlockScrollViewer>(grid.Children[1]);
        CodeBlockTextBlock codeText = Assert.IsType<CodeBlockTextBlock>(viewer.Content);

        Assert.Equal(TextWrapping.NoWrap, codeText.TextWrapping);
    }

    [StaFact]
    public void IndentedCodeBlock_RendersDefaultLanguageLabel()
    {
        MarkdownEngine engine = MarkdownEngine.CreateDefault();

        MarkdownRootPanel root = Assert.IsType<MarkdownRootPanel>(engine.Render("    Console.WriteLine(42);", EmptyServiceProvider.Instance));
        CodeBlockBorder border = Assert.IsType<CodeBlockBorder>(root.Children[0]);
        Grid grid = Assert.IsType<Grid>(border.Child);
        CodeBlockHeaderBorder headerBorder = Assert.IsType<CodeBlockHeaderBorder>(grid.Children[0]);
        CodeBlockHeaderPanel header = Assert.IsType<CodeBlockHeaderPanel>(headerBorder.Child);
        CodeBlockHeaderTextBlock language = Assert.IsType<CodeBlockHeaderTextBlock>(header.Children[0]);

        Assert.Equal("text", language.Text);
    }

    [StaFact]
    public void FencedCodeBlock_WithNoLanguage_RendersDefaultLanguageLabel()
    {
        MarkdownEngine engine = MarkdownEngine.CreateDefault();

        MarkdownRootPanel root = Assert.IsType<MarkdownRootPanel>(engine.Render("```\nsome code\n```", EmptyServiceProvider.Instance));
        CodeBlockBorder border = Assert.IsType<CodeBlockBorder>(root.Children[0]);
        Grid grid = Assert.IsType<Grid>(border.Child);
        CodeBlockHeaderBorder headerBorder = Assert.IsType<CodeBlockHeaderBorder>(grid.Children[0]);
        CodeBlockHeaderPanel header = Assert.IsType<CodeBlockHeaderPanel>(headerBorder.Child);
        CodeBlockHeaderTextBlock language = Assert.IsType<CodeBlockHeaderTextBlock>(header.Children[0]);

        Assert.Equal("text", language.Text);
    }

    [StaFact]
    public void FencedCodeBlock_CodeTextContent_MatchesSourceCode()
    {
        MarkdownEngine engine = MarkdownEngine.CreateDefault();
        const string sourceCode = "var x = 1;\nvar y = 2;";

        MarkdownRootPanel root = Assert.IsType<MarkdownRootPanel>(engine.Render($"```\n{sourceCode}\n```", EmptyServiceProvider.Instance));
        CodeBlockBorder border = Assert.IsType<CodeBlockBorder>(root.Children[0]);
        Grid grid = Assert.IsType<Grid>(border.Child);
        CodeBlockScrollViewer viewer = Assert.IsType<CodeBlockScrollViewer>(grid.Children[1]);
        CodeBlockTextBlock codeText = Assert.IsType<CodeBlockTextBlock>(viewer.Content);

        string normalizedText = codeText.Text.Replace("\r\n", "\n", StringComparison.Ordinal);
        Assert.Equal(sourceCode, normalizedText);
    }

    [StaFact]
    public void CopyButton_HasCorrectInitialStateAndCodeTag()
    {
        MarkdownEngine engine = MarkdownEngine.CreateDefault();

        MarkdownRootPanel root = Assert.IsType<MarkdownRootPanel>(engine.Render("```txt\nalpha\nbeta\n```", EmptyServiceProvider.Instance));
        CodeBlockBorder border = Assert.IsType<CodeBlockBorder>(root.Children[0]);
        Grid grid = Assert.IsType<Grid>(border.Child);
        CodeBlockHeaderBorder headerBorder = Assert.IsType<CodeBlockHeaderBorder>(grid.Children[0]);
        CodeBlockHeaderPanel header = Assert.IsType<CodeBlockHeaderPanel>(headerBorder.Child);
        CodeBlockCopyButton copy = Assert.IsType<CodeBlockCopyButton>(header.Children[1]);

        Assert.Equal("Copy", copy.Content);
        Assert.True(copy.IsEnabled);
        string? tag = copy.Tag as string;
        Assert.NotNull(tag);
        Assert.Contains("alpha", tag, StringComparison.Ordinal);
        Assert.Contains("beta", tag, StringComparison.Ordinal);
    }

    [Fact]
    public void GetLanguage_ReturnsLanguageString_ForFencedCodeBlockWithLanguage()
    {
        FencedCodeBlock block = MarkdownTestHelper.ParseFirstBlock<FencedCodeBlock>("```csharp\ncode\n```");

        string? language = CodeBlockRenderer.GetLanguage(block);

        Assert.Equal("csharp", language);
    }

    [Fact]
    public void GetLanguage_ReturnsNull_ForFencedCodeBlockWithNoLanguage()
    {
        FencedCodeBlock block = MarkdownTestHelper.ParseFirstBlock<FencedCodeBlock>("```\ncode\n```");

        string? language = CodeBlockRenderer.GetLanguage(block);

        Assert.Null(language);
    }

    [Fact]
    public void GetLanguage_ReturnsNull_ForIndentedCodeBlock()
    {
        CodeBlock block = MarkdownTestHelper.ParseFirstBlock<CodeBlock>("    some code");

        string? language = CodeBlockRenderer.GetLanguage(block);

        Assert.Null(language);
    }

    [Fact]
    public void GetLanguage_ReturnsNull_ForFencedCodeBlockWithWhitespaceLanguage()
    {
        FencedCodeBlock block = MarkdownTestHelper.ParseFirstBlock<FencedCodeBlock>("```   \ncode\n```");

        string? language = CodeBlockRenderer.GetLanguage(block);

        Assert.Null(language);
    }

    [Theory]
    [InlineData("```csharp\ncode\n```", "csharp")]
    [InlineData("```javascript\ncode\n```", "javascript")]
    [InlineData("```python\ncode\n```", "python")]
    [InlineData("```xml\ncode\n```", "xml")]
    public void GetLanguage_ReturnsCorrectLanguage_ForVariousLanguageIdentifiers(string markdown, string expectedLanguage)
    {
        FencedCodeBlock block = MarkdownTestHelper.ParseFirstBlock<FencedCodeBlock>(markdown);

        string? language = CodeBlockRenderer.GetLanguage(block);

        Assert.Equal(expectedLanguage, language);
    }
}

