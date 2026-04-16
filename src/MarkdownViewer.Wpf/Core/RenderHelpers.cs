using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

using HtmlAgilityPack;

using Markdig.Syntax;
using Markdig.Syntax.Inlines;

using MarkdownViewer.Wpf.Rendering.Html;
using MarkdownViewer.Wpf.Theming;

namespace MarkdownViewer.Wpf.Core;

internal static class RenderHelpers
{
    public static void AppendInlines(InlineCollection collection, ContainerInline? container, IRenderContext context)
    {
        if (container is null)
        {
            return;
        }

        for (Markdig.Syntax.Inlines.Inline? current = container.FirstChild; current is not null;)
        {
            if (current is HtmlInline htmlInline
                && TryRenderHtmlFragment(htmlInline, context, out System.Windows.Documents.Inline? renderedHtml, out Markdig.Syntax.Inlines.Inline? lastConsumed))
            {
                collection.Add(renderedHtml);
                current = lastConsumed.NextSibling;
                continue;
            }

            collection.Add(context.RenderInline(current));
            current = current.NextSibling;
        }
    }

    public static StackPanel RenderChildBlocks(ContainerBlock container, IRenderContext context, string? styleKey = null)
    {
        StackPanel panel = new()
        {
            Orientation = Orientation.Vertical,
        };

        if (!string.IsNullOrWhiteSpace(styleKey))
        {
            TryApplyStyle(panel, context.Theme, styleKey);
        }

        foreach (Markdig.Syntax.Block child in container)
        {
            panel.Children.Add(context.RenderBlock(child));
        }

        return panel;
    }

    public static TextBlock CreateTextBlock(IRenderContext context, string styleKey)
    {
        TextBlock textBlock = new()
        {
            TextWrapping = TextWrapping.Wrap,
        };

        TryApplyStyle(textBlock, context.Theme, styleKey);
        return textBlock;
    }

    public static void TryApplyStyle(FrameworkElement element, ITheme theme, string key)
    {
        ArgumentNullException.ThrowIfNull(element);
        ArgumentNullException.ThrowIfNull(theme);

        if (theme.GetStyle(key) is Style style)
        {
            element.Style = style;
        }
    }

    public static void TryApplyStyle(TextElement element, ITheme theme, string key)
    {
        ArgumentNullException.ThrowIfNull(element);
        ArgumentNullException.ThrowIfNull(theme);

        if (theme.GetStyle(key) is Style style)
        {
            element.Style = style;
        }
    }

    public static string GetLiteral(LeafBlock block)
    {
        return string.Join(Environment.NewLine, block.Lines.Lines.Take(block.Lines.Count).Select(line => line.Slice.ToString()));
    }

    public static Border CreateDebugLabel(string text, ITheme theme)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(text);
        ArgumentNullException.ThrowIfNull(theme);

        Border border = new();
        TryApplyStyle(border, theme, ThemeKeys.DiagnosticsLabelBorderStyle);

        TextBlock textBlock = new()
        {
            Text = text,
        };
        TryApplyStyle(textBlock, theme, ThemeKeys.DiagnosticsLabelTextStyle);
        border.Child = textBlock;
        return border;
    }

    private static bool TryRenderHtmlFragment(HtmlInline startInline, IRenderContext context, out System.Windows.Documents.Inline renderedInline, out Markdig.Syntax.Inlines.Inline lastConsumed)
    {
        ArgumentNullException.ThrowIfNull(startInline);
        ArgumentNullException.ThrowIfNull(context);

        renderedInline = null!;
        lastConsumed = startInline;

        string tag = startInline.Tag ?? string.Empty;
        if (!HtmlWpfRenderer.TryDescribeTag(tag, out HtmlTagDescriptor descriptor))
        {
            return false;
        }

        if (descriptor.IsStandalone)
        {
            renderedInline = HtmlWpfRenderer.RenderInlineTag(tag, context);
            return true;
        }

        if (descriptor.IsClosing)
        {
            return false;
        }

        StringBuilder fragmentBuilder = new();
        fragmentBuilder.Append(tag);

        int depth = 1;
        for (Markdig.Syntax.Inlines.Inline? current = startInline.NextSibling; current is not null; current = current.NextSibling)
        {
            if (!TryConvertInlineToHtml(current, out string html))
            {
                return false;
            }

            fragmentBuilder.Append(html);
            lastConsumed = current;

            if (current is HtmlInline htmlInline
                && HtmlWpfRenderer.TryDescribeTag(htmlInline.Tag ?? string.Empty, out HtmlTagDescriptor currentDescriptor)
                && string.Equals(currentDescriptor.Name, descriptor.Name, StringComparison.OrdinalIgnoreCase))
            {
                if (currentDescriptor.IsClosing)
                {
                    depth--;
                }
                else if (!currentDescriptor.IsStandalone)
                {
                    depth++;
                }

                if (depth == 0)
                {
                    renderedInline = HtmlWpfRenderer.RenderInlineFragment(fragmentBuilder.ToString(), context);
                    return true;
                }
            }
        }

        return false;
    }

    private static bool TryConvertInlineToHtml(Markdig.Syntax.Inlines.Inline inline, out string html)
    {
        ArgumentNullException.ThrowIfNull(inline);

        switch (inline)
        {
            case HtmlInline htmlInline:
                html = htmlInline.Tag ?? string.Empty;
                return true;
            case LiteralInline literalInline:
                html = HtmlEntity.Entitize(literalInline.Content.ToString());
                return true;
            case HtmlEntityInline htmlEntityInline:
                html = htmlEntityInline.ToString() ?? string.Empty;
                return true;
            case LineBreakInline:
                html = "<br />";
                return true;
            default:
                html = string.Empty;
                return false;
        }
    }
}