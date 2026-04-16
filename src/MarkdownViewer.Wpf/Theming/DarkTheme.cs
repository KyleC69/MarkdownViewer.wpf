namespace MarkdownViewer.Wpf.Theming;

public sealed class DarkTheme : ResourceDictionaryTheme
{
    public DarkTheme()
        : base(LoadDictionary("/MarkdownViewer.Wpf;component/Themes/DarkTheme.xaml"))
    {
    }
}