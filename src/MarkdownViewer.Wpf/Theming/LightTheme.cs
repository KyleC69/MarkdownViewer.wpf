namespace MarkdownViewer.Wpf.Theming;

public sealed class LightTheme : ResourceDictionaryTheme
{
    public LightTheme()
        : base(LoadDictionary("/MarkdownViewer.Wpf;component/Themes/LightTheme.xaml"))
    {
    }
}