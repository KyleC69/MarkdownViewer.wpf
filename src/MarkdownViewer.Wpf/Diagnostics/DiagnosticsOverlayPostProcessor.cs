using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using MarkdownViewer.Wpf.Core;
using MarkdownViewer.Wpf.Theming;

namespace MarkdownViewer.Wpf.Diagnostics;

public sealed class DiagnosticsOverlayPostProcessor : IPostProcessor
{
    private const string OverlayTag = nameof(DiagnosticsOverlayPostProcessor);
    private const string OverlayLabelTag = nameof(DiagnosticsOverlayPostProcessor) + ".Label";

    public void Process(UIElement root, IRenderContext context)
    {
        ArgumentNullException.ThrowIfNull(root);
        ArgumentNullException.ThrowIfNull(context);

        Annotate(root, context);
    }

    internal static void Annotate(UIElement element, IRenderContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        if (element is FrameworkElement ownedElement && string.Equals(ownedElement.Tag as string, OverlayLabelTag, StringComparison.Ordinal))
        {
            return;
        }

        if (element is Border border)
        {
            border.BorderBrush ??= context.Theme.GetBrush(ThemeKeys.DiagnosticsOverlayBorderBrush) ?? Brushes.OrangeRed;
            border.BorderThickness = border.BorderThickness == default
                ? context.Theme.GetThickness(ThemeKeys.DiagnosticsOverlayBorderThickness) ?? new Thickness(1)
                : border.BorderThickness;

            ApplyLabel(border, context);
        }

        if (element is Panel panel)
        {
            foreach (UIElement child in panel.Children)
            {
                Annotate(child, context);
            }
        }
        else if (element is Decorator decorator && decorator.Child is UIElement child)
        {
            Annotate(child, context);
        }
        else if (element is ContentControl contentControl && contentControl.Content is UIElement content)
        {
            Annotate(content, context);
        }
    }

    private static void ApplyLabel(Border border, IRenderContext context)
    {
        if (border.Child is not UIElement child)
        {
            return;
        }

        if (border.Child is Grid existing && string.Equals(existing.Tag as string, OverlayTag, StringComparison.Ordinal))
        {
            return;
        }

        Grid overlay = new()
        {
            Tag = OverlayTag,
        };
        border.Child = null;
        overlay.Children.Add(child);

        Border label = RenderHelpers.CreateDebugLabel(child.GetType().Name, context.Theme);
        label.Tag = OverlayLabelTag;
        label.HorizontalAlignment = HorizontalAlignment.Left;
        label.VerticalAlignment = VerticalAlignment.Top;
        overlay.Children.Add(label);

        border.Child = overlay;
    }
}