using System.Windows;
using System.Windows.Controls;

using Markdig.Syntax;

using MarkdownViewer.Wpf.Controls;
using MarkdownViewer.Wpf.Core;

namespace MarkdownViewer.Wpf.Rendering.Blocks;

public sealed class ListItemRenderer : IBlockRenderer<ListItemBlock>
{
    public UIElement Render(ListItemBlock block, IRenderContext context)
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

        ListItemGrid grid = new()
        {
            Margin = new Thickness(0, 0, 0, 4),
        };
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

        ListItemMarkerTextBlock marker = new()
        {
            Text = markerText,
            VerticalAlignment = VerticalAlignment.Top,
            Margin = new Thickness(0, 0, 8, 0),
        };
        Grid.SetColumn(marker, 0);

        ListItemContentPanel content = new();
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