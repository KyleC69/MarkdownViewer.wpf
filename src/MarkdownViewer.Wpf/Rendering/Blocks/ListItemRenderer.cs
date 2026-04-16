using System.Windows;
using System.Windows.Controls;

using Markdig.Syntax;

using MarkdownViewer.Wpf.Core;
using MarkdownViewer.Wpf.Theming;

namespace MarkdownViewer.Wpf.Rendering.Blocks;

public sealed class ListItemRenderer : IBlockRenderer<ListItemBlock>
{
    public System.Windows.UIElement Render(ListItemBlock block, IRenderContext context)
    {
        ArgumentNullException.ThrowIfNull(block);
        ArgumentNullException.ThrowIfNull(context);

        return CreateListItem(GetMarker(block), block.Select(context.RenderBlock), context);
    }

    internal static Grid CreateListItem(string markerText, IEnumerable<UIElement> contentElements, IRenderContext context)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(markerText);
        ArgumentNullException.ThrowIfNull(contentElements);
        ArgumentNullException.ThrowIfNull(context);

        Grid grid = new();
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        RenderHelpers.TryApplyStyle(grid, context.Theme, ThemeKeys.ListItemContainerStyle);

        TextBlock marker = new()
        {
            Text = markerText,
        };
        RenderHelpers.TryApplyStyle(marker, context.Theme, ThemeKeys.ListItemMarkerStyle);
        Grid.SetColumn(marker, 0);

        StackPanel content = new()
        {
            Orientation = Orientation.Vertical,
        };
        RenderHelpers.TryApplyStyle(content, context.Theme, ThemeKeys.ListItemContentStyle);
        foreach (UIElement element in contentElements)
        {
            content.Children.Add(element);
        }

        Grid.SetColumn(content, 1);

        grid.Children.Add(marker);
        grid.Children.Add(content);
        return grid;
    }

    internal static string GetMarker(ListItemBlock block)
    {
        ListBlock? listBlock = block.Parent as ListBlock;
        if (listBlock is null)
        {
            return "•";
        }

        if (!listBlock.IsOrdered)
        {
            return "•";
        }

        int index = 0;
        foreach (Block sibling in listBlock)
        {
            if (ReferenceEquals(sibling, block))
            {
                break;
            }

            index++;
        }

        int orderedStart = int.TryParse(listBlock.OrderedStart, out int parsedOrderedStart)
            ? parsedOrderedStart
            : 1;

        return $"{orderedStart + index}.";
    }
}