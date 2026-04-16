using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;

using MarkdownViewer.Wpf.Theming;

namespace MarkdownViewer.Wpf.Sample;

public partial class MainWindow : Window, INotifyPropertyChanged
{
    private const string SampleImageToken = "{{SampleImageUri}}";

    private string markdownText = string.Empty;
    private string selectedThemeKey = "Default";

    public MainWindow()
    {
        InitializeComponent();
        ThemeOptions = ["Default", "Light", "Dark"];
        DataContext = this;
        LoadSampleMarkdown();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public IReadOnlyList<string> ThemeOptions { get; }

    public string MarkdownText
    {
        get => markdownText;
        set
        {
            if (markdownText == value)
            {
                return;
            }

            markdownText = value;
            OnPropertyChanged();
        }
    }

    public string SelectedThemeKey
    {
        get => selectedThemeKey;
        set
        {
            if (selectedThemeKey == value)
            {
                return;
            }

            selectedThemeKey = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(CurrentTheme));
        }
    }

    public ITheme CurrentTheme => SelectedThemeKey switch
    {
        "Light" => new LightTheme(),
        "Dark" => new DarkTheme(),
        _ => new DefaultTheme(),
    };

    private void ReloadSampleMarkdown_Click(object sender, RoutedEventArgs e)
    {
        LoadSampleMarkdown();
    }

    private void LoadSampleMarkdown()
    {
        string markdownPath = Path.Combine(AppContext.BaseDirectory, "SampleMarkdown.md");
        string imagePath = Path.Combine(AppContext.BaseDirectory, "Assets", "sample-image.png");

        string content = File.ReadAllText(markdownPath);
        string imageUri = new Uri(imagePath, UriKind.Absolute).AbsoluteUri;
        MarkdownText = content.Replace(SampleImageToken, imageUri, StringComparison.Ordinal);
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}