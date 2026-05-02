using System.Windows;
using System.Windows.Controls;

namespace MarkdownViewer.Wpf.Controls;

public class WrappingStackPanel : Panel
{
    protected override Size MeasureOverride(Size availableSize)
    {
        double width = 0;
        double height = 0;

        foreach (UIElement child in InternalChildren)
        {
            child.Measure(new Size(availableSize.Width, double.PositiveInfinity));
            Size desired = child.DesiredSize;

            width = Math.Max(width, desired.Width);
            height += desired.Height;
        }

        return new Size(width, height);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        double y = 0;

        foreach (UIElement child in InternalChildren)
        {
            Size desired = child.DesiredSize;
            child.Arrange(new Rect(0, y, finalSize.Width, desired.Height));
            y += desired.Height;
        }

        return finalSize;
    }
}
