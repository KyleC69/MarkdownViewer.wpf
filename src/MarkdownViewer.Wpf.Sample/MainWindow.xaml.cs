// Build Date: 2026/05/01
// Solution: MarkdownViewer.Wpf
// Project:   MarkdownViewer.WPF.Sample
// File:         MainWindow.xaml.cs
// Author: Kyle L. Crowder
// Build Num: 212740



using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;




namespace MarkdownViewer.Wpf.Sample;





public partial class MainWindow : Window, INotifyPropertyChanged
{
    private const string SampleImageToken = "{{SampleImageUri}}";








    public MainWindow()
    {
        InitializeComponent();
        DataContext = this;
        LoadSampleMarkdown();
    }








    public string MarkdownText
    {
        get;
        set
        {
            if (field == value)
            {
                return;
            }

            field = value;
            OnPropertyChanged();
        }
    } = string.Empty;

    public event PropertyChangedEventHandler? PropertyChanged;








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