using System.Windows;

namespace MarkdownViewer.Wpf.Theming;

public static class MarkdownTheming
{
    public static readonly DependencyProperty RoleProperty = DependencyProperty.RegisterAttached(
        "Role",
        typeof(string),
        typeof(MarkdownTheming),
        new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits));

    public static string? GetRole(DependencyObject dependencyObject)
    {
        ArgumentNullException.ThrowIfNull(dependencyObject);
        return (string?)dependencyObject.GetValue(RoleProperty);
    }

    public static void SetRole(DependencyObject dependencyObject, string? value)
    {
        ArgumentNullException.ThrowIfNull(dependencyObject);
        dependencyObject.SetValue(RoleProperty, value);
    }
}