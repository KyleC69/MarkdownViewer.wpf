using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace MarkdownViewer.Wpf.Diagnostics;

public static class MarkdownRenderTreeFormatter
{
    public static string Format(UIElement root)
    {
        ArgumentNullException.ThrowIfNull(root);

        StringBuilder builder = new();
        AppendElement(builder, root, 0);
        return builder.ToString().TrimEnd();
    }

    private static void AppendElement(StringBuilder builder, UIElement element, int depth)
    {
        builder.Append(' ', depth * 2);
        builder.AppendLine(element.GetType().Name);

        switch (element)
        {
            case Panel panel:
                foreach (UIElement child in panel.Children)
                {
                    AppendElement(builder, child, depth + 1);
                }
                break;
            case Decorator decorator when decorator.Child is UIElement child:
                AppendElement(builder, child, depth + 1);
                break;
            case ContentControl contentControl when contentControl.Content is UIElement content:
                AppendElement(builder, content, depth + 1);
                break;
        }
    }
}