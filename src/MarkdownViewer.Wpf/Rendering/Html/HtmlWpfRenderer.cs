using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

using HtmlAgilityPack;

using MarkdownViewer.Wpf.Controls;
using MarkdownViewer.Wpf.Core;
using MarkdownViewer.Wpf.Diagnostics;
using MarkdownViewer.Wpf.Rendering.Blocks;

namespace MarkdownViewer.Wpf.Rendering.Html;

internal static class HtmlWpfRenderer
{
    private static readonly HashSet<string> standaloneInlineElementNames =
    [
        "br", "hr", "img", "input", "wbr"
    ];

    private static readonly HashSet<string> blockElementNames =
    [
        "article", "aside", "blockquote", "details", "div", "figure", "figcaption", "footer", "h1", "h2", "h3", "h4", "h5", "h6", "header", "hr", "li", "main", "ol", "p", "pre", "section", "summary", "table", "tbody", "td", "tfoot", "th", "thead", "tr", "ul"
    ];

    public static UIElement RenderBlock(string html, IRenderContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        HtmlDocument document = new();
        document.LoadHtml($"<root>{html}</root>");

        List<UIElement> elements = [];
        foreach (HtmlNode child in document.DocumentNode.FirstChild.ChildNodes)
        {
            elements.AddRange(RenderBlockNode(child, context));
        }

        if (elements.Count == 0)
        {
            return new ParagraphTextBlock() { TextWrapping = TextWrapping.Wrap };
        }

        if (elements.Count == 1)
        {
            return elements[0];
        }

        StackPanel panel = new()
        {
            Orientation = Orientation.Vertical,
        };

        foreach (UIElement element in elements)
        {
            panel.Children.Add(element);
        }

        return panel;
    }

    public static System.Windows.Documents.Inline RenderInlineTag(string htmlTag, IRenderContext context)
    {
        return RenderInlineFragment(htmlTag, context);
    }

    public static System.Windows.Documents.Inline RenderInlineFragment(string htmlFragment, IRenderContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        if (string.IsNullOrWhiteSpace(htmlFragment))
        {
            return new Run(string.Empty);
        }

        HtmlDocument document = new();
        document.LoadHtml($"<root>{htmlFragment}</root>");

        List<System.Windows.Documents.Inline> inlines = [];
        foreach (HtmlNode child in document.DocumentNode.FirstChild.ChildNodes)
        {
            inlines.AddRange(RenderInlineNode(child, context));
        }

        if (inlines.Count == 0)
        {
            MarkdownDiagnostics.ReportHtmlInlineIgnored(htmlFragment);
            return new Run(string.Empty);
        }

        if (inlines.Count == 1)
        {
            return inlines[0];
        }

        Span span = new();
        foreach (System.Windows.Documents.Inline inline in inlines)
        {
            span.Inlines.Add(inline);
        }

        return span;
    }

    private static IEnumerable<UIElement> RenderBlockNode(HtmlNode node, IRenderContext context)
    {
        if (node.NodeType == HtmlNodeType.Comment)
        {
            return [];
        }

        if (node.NodeType == HtmlNodeType.Text)
        {
            string text = NormalizeText(node.InnerText);
            if (string.IsNullOrWhiteSpace(text))
            {
                return [];
            }

            ParagraphTextBlock textBlock = new() { TextWrapping = TextWrapping.Wrap };
            textBlock.Text = text;
            return [textBlock];
        }

        string tagName = node.Name.ToLowerInvariant();
        return tagName switch
        {
            "p" => [CreateTextBlock(node.ChildNodes, context)],
            "a" => [CreateTextBlock(new[] { node }, context)],
            "div" or "section" or "article" or "main" or "header" or "footer" or "aside" or "figure" or "details" => [CreateContainer(node, context)],
            "h1" => [CreateHeadingTextBlock(node.ChildNodes, context, 1)],
            "h2" => [CreateHeadingTextBlock(node.ChildNodes, context, 2)],
            "h3" => [CreateHeadingTextBlock(node.ChildNodes, context, 3)],
            "h4" => [CreateHeadingTextBlock(node.ChildNodes, context, 4)],
            "h5" => [CreateHeadingTextBlock(node.ChildNodes, context, 5)],
            "h6" => [CreateHeadingTextBlock(node.ChildNodes, context, 6)],
            "figcaption" or "summary" => [CreateTextBlock(node.ChildNodes, context)],
            "blockquote" => [CreateBlockQuote(node, context)],
            "pre" => [CreatePreformattedBlock(node, context)],
            "ul" => [CreateList(node, context, isOrdered: false)],
            "ol" => [CreateList(node, context, isOrdered: true)],
            "table" => [CreateTable(node, context)],
            "img" => [CreateImage(node, context)],
            "hr" => [CreateThematicBreak(context)],
            _ => node.HasChildNodes ? RenderDescendantBlocks(node.ChildNodes, context) : CreateFallbackText(node, context),
        };
    }

    private static UIElement CreateBlockQuote(HtmlNode node, IRenderContext context)
    {
        BlockQuoteBorder border = new();
        border.Child = CreateStackPanel(node.ChildNodes, context);
        return border;
    }

    private static UIElement CreateContainer(HtmlNode node, IRenderContext context)
    {
        if (HasBlockChildren(node))
        {
            return CreateStackPanel(node.ChildNodes, context);
        }

        ParagraphTextBlock textBlock = new() { TextWrapping = TextWrapping.Wrap };
        foreach (System.Windows.Documents.Inline inline in RenderInlineNodes(node.ChildNodes, context))
        {
            textBlock.Inlines.Add(inline);
        }

        return textBlock;
    }

    private static IReadOnlyList<UIElement> CreateFallbackText(HtmlNode node, IRenderContext context)
    {
        string text = NormalizeText(node.InnerText);
        if (string.IsNullOrWhiteSpace(text))
        {
            return [];
        }

        ParagraphTextBlock textBlock = new() { TextWrapping = TextWrapping.Wrap };
        textBlock.Text = text;
        return [textBlock];
    }

    private static UIElement CreateImage(HtmlNode node, IRenderContext context)
    {
        Image image = new();

        string source = node.GetAttributeValue("src", string.Empty);
        if (Uri.TryCreate(source, UriKind.RelativeOrAbsolute, out Uri? uri))
        {
            image.Source = MarkdownIntegrationServices.ResolveImageSource(uri, context);
        }

        string alternateText = node.GetAttributeValue("alt", string.Empty);
        if (!string.IsNullOrWhiteSpace(alternateText))
        {
            image.ToolTip = alternateText;
        }

        return image;
    }

    private static UIElement CreatePreformattedBlock(HtmlNode node, IRenderContext context)
    {
        HtmlNode? codeNode = node.ChildNodes.FirstOrDefault(static child => string.Equals(child.Name, "code", StringComparison.OrdinalIgnoreCase));
        string code = HtmlEntity.DeEntitize((codeNode ?? node).InnerText).TrimEnd('\r', '\n');
        string? language = codeNode is null ? null : GetCodeLanguage(codeNode);
        return CodeBlockRenderer.CreateCodeBlock(code, context, language);
    }

    private static UIElement CreateList(HtmlNode listNode, IRenderContext context, bool isOrdered)
    {
        ListPanel panel = new()
        {
            Orientation = Orientation.Vertical,
        };

        int orderedStart = Math.Max(1, listNode.GetAttributeValue("start", 1));
        int index = 0;
        foreach (HtmlNode listItem in listNode.ChildNodes.Where(static child => string.Equals(child.Name, "li", StringComparison.OrdinalIgnoreCase)))
        {
            string marker = isOrdered ? $"{orderedStart + index}." : "•";
            panel.Children.Add(ListItemRenderer.CreateListItem(marker, RenderDescendantBlocks(listItem.ChildNodes, context), context));
            index++;
        }

        return panel;
    }

    private static UIElement CreateStackPanel(HtmlNodeCollection nodes, IRenderContext context)
    {
        StackPanel panel = new()
        {
            Orientation = Orientation.Vertical,
        };

        foreach (UIElement element in RenderDescendantBlocks(nodes, context))
        {
            panel.Children.Add(element);
        }

        return panel;
    }

    private static Grid CreateTable(HtmlNode tableNode, IRenderContext context)
    {
        TableGrid grid = new();

        List<HtmlNode> rows = tableNode.Descendants().Where(static node => string.Equals(node.Name, "tr", StringComparison.OrdinalIgnoreCase)).ToList();
        int columnCount = rows.Count == 0
            ? 0
            : rows.Max(row => row.ChildNodes.Where(static cell => IsCell(cell.Name)).Sum(static cell => Math.Max(1, cell.GetAttributeValue("colspan", 1))));

        for (int columnIndex = 0; columnIndex < columnCount; columnIndex++)
        {
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        }

        for (int rowIndex = 0; rowIndex < rows.Count; rowIndex++)
        {
            HtmlNode row = rows[rowIndex];
            int columnIndex = 0;

            foreach (HtmlNode cell in row.ChildNodes.Where(static candidate => IsCell(candidate.Name)))
            {
                TableCellBorder border = new();
                bool isHeader = string.Equals(cell.Name, "th", StringComparison.OrdinalIgnoreCase);

                border.Child = HasBlockChildren(cell)
                    ? CreateStackPanel(cell.ChildNodes, context)
                    : CreateTextBlock(cell.ChildNodes, context);

                Grid.SetRow(border, rowIndex);
                Grid.SetColumn(border, columnIndex);

                int columnSpan = Math.Max(1, cell.GetAttributeValue("colspan", 1));
                int rowSpan = Math.Max(1, cell.GetAttributeValue("rowspan", 1));
                if (columnSpan > 1)
                {
                    Grid.SetColumnSpan(border, columnSpan);
                }

                if (rowSpan > 1)
                {
                    Grid.SetRowSpan(border, rowSpan);
                }

                grid.Children.Add(border);
                columnIndex += columnSpan;
            }
        }

        return grid;
    }

    private static Border CreateThematicBreak(IRenderContext context)
    {
        ThematicBreakBorder border = new()
        {
            Height = 1,
            HorizontalAlignment = HorizontalAlignment.Stretch,
        };
        return border;
    }

    private static TextBlock CreateTextBlock(IEnumerable<HtmlNode> nodes, IRenderContext context)
    {
        ParagraphTextBlock textBlock = new() { TextWrapping = TextWrapping.Wrap };
        foreach (System.Windows.Documents.Inline inline in RenderInlineNodes(nodes, context))
        {
            textBlock.Inlines.Add(inline);
        }

        return textBlock;
    }

    private static TextBlock CreateHeadingTextBlock(IEnumerable<HtmlNode> nodes, IRenderContext context, int level)
    {
        TextBlock textBlock = level switch
        {
            1 => new Heading1TextBlock { TextWrapping = TextWrapping.Wrap },
            2 => new Heading2TextBlock { TextWrapping = TextWrapping.Wrap },
            3 => new Heading3TextBlock { TextWrapping = TextWrapping.Wrap },
            4 => new Heading4TextBlock { TextWrapping = TextWrapping.Wrap },
            5 => new Heading5TextBlock { TextWrapping = TextWrapping.Wrap },
            _ => new Heading6TextBlock { TextWrapping = TextWrapping.Wrap },
        };
        foreach (System.Windows.Documents.Inline inline in RenderInlineNodes(nodes, context))
        {
            textBlock.Inlines.Add(inline);
        }

        return textBlock;
    }

    private static IReadOnlyList<UIElement> RenderDescendantBlocks(IEnumerable<HtmlNode> nodes, IRenderContext context)
    {
        List<UIElement> elements = [];
        foreach (HtmlNode child in nodes)
        {
            elements.AddRange(RenderBlockNode(child, context));
        }

        return elements;
    }

    private static IEnumerable<System.Windows.Documents.Inline> RenderInlineNodes(IEnumerable<HtmlNode> nodes, IRenderContext context)
    {
        foreach (HtmlNode node in nodes)
        {
            foreach (System.Windows.Documents.Inline inline in RenderInlineNode(node, context))
            {
                yield return inline;
            }
        }
    }

    private static IEnumerable<System.Windows.Documents.Inline> RenderInlineNode(HtmlNode node, IRenderContext context)
    {
        if (node.NodeType == HtmlNodeType.Comment)
        {
            yield break;
        }

        if (node.NodeType == HtmlNodeType.Text)
        {
            string text = HtmlEntity.DeEntitize(node.InnerText);
            if (!string.IsNullOrWhiteSpace(text))
            {
                yield return new Run(text);
            }

            yield break;
        }

        string tagName = node.Name.ToLowerInvariant();
        switch (tagName)
        {
            case "br":
                yield return new LineBreak();
                yield break;
            case "strong":
            case "b":
                yield return CreateSpan<Bold>(node, context);
                yield break;
            case "em":
            case "i":
                yield return CreateSpan<Italic>(node, context);
                yield break;
            case "u":
                yield return CreateSpan<Underline>(node, context);
                yield break;
            case "code":
                // Use custom Span type for implicit styling
                CodeInlineSpan codeSpan = new();
                codeSpan.Inlines.Add(new Run(HtmlEntity.DeEntitize(node.InnerText)));
                yield return codeSpan;
                yield break;
            case "mark":
                {
                    MarkedSpan span = new();
                    foreach (HtmlNode child in node.ChildNodes)
                    {
                        foreach (var inline in RenderInlineNode(child, context))
                        {
                            span.Inlines.Add(inline);
                        }
                    }
                    yield return span;
                }
                yield break;
            case "del":
            case "strike":
            case "s":
                {
                    StrikeThroughSpan span = new();
                    foreach (HtmlNode child in node.ChildNodes)
                    {
                        foreach (var inline in RenderInlineNode(child, context))
                        {
                            span.Inlines.Add(inline);
                        }
                    }
                    yield return span;
                }
                yield break;
            case "sup":
                {
                    SuperscriptSpan span = new();
                    foreach (HtmlNode child in node.ChildNodes)
                    {
                        foreach (var inline in RenderInlineNode(child, context))
                        {
                            span.Inlines.Add(inline);
                        }
                    }
                    yield return span;
                }
                yield break;
            case "sub":
                {
                    SubscriptSpan span = new();
                    foreach (HtmlNode child in node.ChildNodes)
                    {
                        foreach (var inline in RenderInlineNode(child, context))
                        {
                            span.Inlines.Add(inline);
                        }
                    }
                    yield return span;
                }
                yield break;
            case "a":
                yield return CreateHyperlink(node, context);
                yield break;
            case "img":
                yield return new InlineUIContainer(CreateImage(node, context));
                yield break;
            default:
                foreach (System.Windows.Documents.Inline inline in RenderInlineNodes(node.ChildNodes, context))
                {
                    yield return inline;
                }

                yield break;
        }
    }

    private static Hyperlink CreateHyperlink(HtmlNode node, IRenderContext context)
    {
        Hyperlink hyperlink = new();
        foreach (System.Windows.Documents.Inline inline in RenderInlineNodes(node.ChildNodes, context))
        {
            hyperlink.Inlines.Add(inline);
        }

        string href = node.GetAttributeValue("href", string.Empty);
        if (Uri.TryCreate(href, UriKind.RelativeOrAbsolute, out Uri? uri))
        {
            MarkdownIntegrationServices.ConfigureHyperlink(hyperlink, uri, context);
        }

        string title = node.GetAttributeValue("title", string.Empty);
        if (!string.IsNullOrWhiteSpace(title))
        {
            hyperlink.ToolTip = title;
        }

        return hyperlink;
    }

    private static TSpan CreateSpan<TSpan>(HtmlNode node, IRenderContext context)
        where TSpan : Span, new()
    {
        TSpan span = new();
        foreach (System.Windows.Documents.Inline inline in RenderInlineNodes(node.ChildNodes, context))
        {
            span.Inlines.Add(inline);
        }

        return span;
    }

    private static bool HasBlockChildren(HtmlNode node)
    {
        return node.ChildNodes.Any(static child => child.NodeType == HtmlNodeType.Element && blockElementNames.Contains(child.Name.ToLowerInvariant()));
    }

    private static bool IsCell(string tagName)
    {
        return string.Equals(tagName, "th", StringComparison.OrdinalIgnoreCase)
            || string.Equals(tagName, "td", StringComparison.OrdinalIgnoreCase);
    }

    private static string NormalizeText(string text)
    {
        return HtmlEntity.DeEntitize(text).Trim();
    }

    private static string? GetCodeLanguage(HtmlNode codeNode)
    {
        string classValue = codeNode.GetAttributeValue("class", string.Empty);
        foreach (string token in classValue.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            if (token.StartsWith("language-", StringComparison.OrdinalIgnoreCase))
            {
                return token["language-".Length..];
            }

            if (token.StartsWith("lang-", StringComparison.OrdinalIgnoreCase))
            {
                return token["lang-".Length..];
            }
        }

        return null;
    }

    public static bool TryDescribeTag(string htmlTag, out HtmlTagDescriptor descriptor)
    {
        descriptor = default;

        if (string.IsNullOrWhiteSpace(htmlTag))
        {
            return false;
        }

        string trimmed = htmlTag.Trim();
        if (!trimmed.StartsWith('<') || !trimmed.EndsWith('>'))
        {
            return false;
        }

        int index = 1;
        bool isClosing = false;
        if (index < trimmed.Length && trimmed[index] == '/')
        {
            isClosing = true;
            index++;
        }

        while (index < trimmed.Length && char.IsWhiteSpace(trimmed[index]))
        {
            index++;
        }

        int start = index;
        while (index < trimmed.Length && (char.IsLetterOrDigit(trimmed[index]) || trimmed[index] is '-' or ':'))
        {
            index++;
        }

        if (index <= start)
        {
            return false;
        }

        string name = trimmed[start..index].ToLowerInvariant();
        bool isStandalone = !isClosing && (trimmed.EndsWith("/>", StringComparison.Ordinal) || standaloneInlineElementNames.Contains(name));
        descriptor = new HtmlTagDescriptor(name, isClosing, isStandalone);
        return true;
    }
}

internal readonly record struct HtmlTagDescriptor(string Name, bool IsClosing, bool IsStandalone);