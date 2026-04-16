namespace MarkdownViewer.Wpf.Theming;

public sealed class DefaultTheme : ResourceDictionaryTheme
{
    public DefaultTheme()
        : base(LoadDictionary("/MarkdownViewer.Wpf;component/Themes/DefaultTheme.xaml"))
    {
    }
}