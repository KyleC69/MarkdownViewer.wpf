using System.Reflection;
using System.Windows;
using System.Windows.Controls;

using Markdig.Syntax;

using MarkdownViewer.Wpf.Core;
using MarkdownViewer.Wpf.Theming;

namespace MarkdownViewer.Wpf.Rendering.Blocks;

public class CodeBlockRenderer : IBlockRenderer<CodeBlock>
{
    public virtual UIElement Render(CodeBlock block, IRenderContext context)
    {
        ArgumentNullException.ThrowIfNull(block);
        ArgumentNullException.ThrowIfNull(context);

        return CreateCodeBlock(RenderHelpers.GetLiteral(block), context, GetLanguage(block));
    }

    internal static Border CreateCodeBlock(CodeBlock block, IRenderContext context, string? language)
    {
        return CreateCodeBlock(RenderHelpers.GetLiteral(block), context, language);
    }

    internal static Border CreateCodeBlock(string code, IRenderContext context, string? language)
    {
        Border border = new();
        RenderHelpers.ApplyRole(border, ThemeKeys.CodeBlockBorderStyle);

        Grid grid = new();
        if (!string.IsNullOrWhiteSpace(language))
        {
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        }

        grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

        if (!string.IsNullOrWhiteSpace(language))
        {
            DockPanel header = new()
            {
                LastChildFill = false,
            };
            RenderHelpers.ApplyRole(header, ThemeKeys.CodeBlockHeaderPanelStyle);

            TextBlock headerText = new()
            {
                Text = language,
                VerticalAlignment = VerticalAlignment.Center,
            };
            RenderHelpers.ApplyRole(headerText, ThemeKeys.CodeBlockHeaderTextStyle);
            DockPanel.SetDock(headerText, Dock.Left);

            Button copyButton = new()
            {
                Content = "Copy",
                HorizontalAlignment = HorizontalAlignment.Right,
                Tag = code,
            };
            RenderHelpers.ApplyRole(copyButton, ThemeKeys.CodeBlockCopyButtonStyle);
            copyButton.Click += static (_, args) =>
            {
                if (args.Source is Button button && button.Tag is string text)
                {
                    Clipboard.SetText(text);
                }
            };
            DockPanel.SetDock(copyButton, Dock.Right);

            header.Children.Add(headerText);
            header.Children.Add(copyButton);
            Grid.SetRow(header, 0);
            grid.Children.Add(header);
        }

        TextBlock textBlock = new()
        {
            Text = code,
            TextWrapping = TextWrapping.NoWrap,
        };
        RenderHelpers.ApplyRole(textBlock, ThemeKeys.CodeBlockTextStyle);

        ScrollViewer scrollViewer = new()
        {
            Content = textBlock,
            HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
            VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
        };
        RenderHelpers.ApplyRole(scrollViewer, ThemeKeys.CodeBlockScrollViewerStyle);
        Grid.SetRow(scrollViewer, string.IsNullOrWhiteSpace(language) ? 0 : 1);
        grid.Children.Add(scrollViewer);

        border.Child = grid;
        return border;
    }

    internal static string? GetLanguage(CodeBlock block)
    {
        PropertyInfo? property = block.GetType().GetProperty("Info");
        if (property?.GetValue(block) is null)
        {
            return null;
        }

        string info = property.GetValue(block)?.ToString()?.Trim() ?? string.Empty;
        return string.IsNullOrWhiteSpace(info) ? null : info;
    }
}