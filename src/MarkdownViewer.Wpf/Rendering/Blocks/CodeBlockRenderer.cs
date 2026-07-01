// Solution: MarkdownViewer.Wpf
// Project:   MarkdownViewer.Wpf
// File:         CodeBlockRenderer.cs
// Author: Kyle L. Crowder
// Build Date: 2026/05/02



using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using Markdig.Syntax;

using MarkdownViewer.Wpf.Controls;
using MarkdownViewer.Wpf.Core;




namespace MarkdownViewer.Wpf.Rendering.Blocks;





public class CodeBlockRenderer : IBlockRenderer<CodeBlock>
{
    private const string CopiedButtonLabel = "Copied";
    private const string CopyButtonLabel = "Copy";
    private const string DefaultLanguageLabel = "text";








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
        CodeBlockBorder border = new()
        {
            Margin = new Thickness(0, 0, 0, 12),
            BorderThickness = new Thickness(1),
            BorderBrush = SystemColors.ControlDarkBrush,
            Background = SystemColors.ControlLightLightBrush,
            CornerRadius = new CornerRadius(6),
            SnapsToDevicePixels = true
        };

        Grid grid = new();
        grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

        CodeBlockHeaderPanel header = new() { LastChildFill = false };

        CodeBlockHeaderTextBlock headerText = new()
        {
            Text = string.IsNullOrWhiteSpace(language) ? DefaultLanguageLabel : language,
            VerticalAlignment = VerticalAlignment.Center,
            FontFamily = new FontFamily("Consolas, Courier New"),
            FontSize = 12,
            FontWeight = FontWeights.SemiBold
        };
        DockPanel.SetDock(headerText, Dock.Left);

        CodeBlockCopyButton copyButton = new() { Content = CopyButtonLabel, Tag = code, Padding = new Thickness(8, 2, 8, 2), Margin = new Thickness(12, 0, 0, 0) };
        copyButton.Click += static (_, args) =>
        {
            if (args.Source is Button button && button.Tag is string text)
            {
                Clipboard.SetText(text);
                ShowCopyConfirmationAsync(button);
            }
        };
        DockPanel.SetDock(copyButton, Dock.Right);

        header.Children.Add(headerText);
        header.Children.Add(copyButton);

        CodeBlockHeaderBorder headerBorder = new()
        {
            Child = header,
            Background = SystemColors.ControlLightBrush,
            BorderBrush = SystemColors.ControlDarkBrush,
            BorderThickness = new Thickness(0, 0, 0, 1),
            Padding = new Thickness(10, 6, 10, 6)
        };
        Grid.SetRow(headerBorder, 0);
        grid.Children.Add(headerBorder);

        CodeBlockTextBlock textBlock = new() { Text = code, TextWrapping = TextWrapping.NoWrap, FontFamily = new FontFamily("Consolas, Courier New"), Margin = new Thickness(10, 8, 10, 10) };

        CodeBlockScrollViewer scrollViewer = new() { Content = textBlock, HorizontalScrollBarVisibility = ScrollBarVisibility.Auto, VerticalScrollBarVisibility = ScrollBarVisibility.Auto };
        Grid.SetRow(scrollViewer, 1);
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

        var info = property.GetValue(block)?.ToString()?.Trim() ?? string.Empty;
        return string.IsNullOrWhiteSpace(info) ? null : info;
    }








    private static async void ShowCopyConfirmationAsync(Button button)
    {
        ArgumentNullException.ThrowIfNull(button);

        var previousContent = button.Content;
        var previousEnabledState = button.IsEnabled;

        button.Content = CopiedButtonLabel;
        button.IsEnabled = false;

        await Task.Delay(1200).ConfigureAwait(false);

        await button.Dispatcher.InvokeAsync(() =>
        {
            button.Content = previousContent;
            button.IsEnabled = previousEnabledState;
        });
    }
}
