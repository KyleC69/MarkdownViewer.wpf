// Solution: MarkdownViewer.Wpf
// Project:   MarkdownViewer.Wpf
// File:         MarkdownRenderTreeFormatter.cs
// Author: Kyle L. Crowder
// Build Date: 2026/05/02



using System.Text;
using System.Windows;
using System.Windows.Controls;




namespace MarkdownViewer.Wpf.Diagnostics;





public static class MarkdownRenderTreeFormatter
{

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








    public static string Format(UIElement root)
    {
        ArgumentNullException.ThrowIfNull(root);

        StringBuilder builder = new();
        AppendElement(builder, root, 0);
        return builder.ToString().TrimEnd();
    }
}