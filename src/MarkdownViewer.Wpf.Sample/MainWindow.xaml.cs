using System.Windows;

namespace MarkdownViewerWpf.Sample;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private const string SampleMarkdown = """
        # MarkdownViewer.Wpf sample

        This sample shows a paragraph with *emphasis*, **strong text**, ~~strikethrough~~, `inline code`, [an inline link](https://github.com/KyleC69/markdig.wpf), an email autolink <sample@example.com>, and a URL autolink https://www.nuget.org/packages/MarkdownViewer.Wpf.

        ## Headings

        ### Level 3 heading

        #### Level 4 heading

        ## Quote block

        > Markdown quotes can contain **formatted text**
        > and continue across multiple lines.

        ## Lists

        - Bullet item
        - Nested content
          - Child bullet item

        1. Ordered item
        2. Ordered item

        - [x] Completed task
        - [ ] Incomplete task

        ## Line breaks and entities

        This line ends with a hard break\
        so the next sentence starts on a new line.

        Entity sample: &copy; MarkdownViewer.Wpf

        ---

        ## Table

        | Renderer | Example |
        | --- | --- |
        | Heading | `## Heading` |
        | Link | `[Markdig.Wpf](https://github.com/KyleC69/markdig.wpf)` |
        | Task list | `- [x] Done` |

        ## Code block

        ```csharp
        public static string Hello(string name)
        {
            return $"Hello, {name}!";
        }
        ```

        ## HTML inline

        Inline HTML like <kbd>Ctrl+C</kbd> is rendered as HTML content.

        ## HTML block

        <div>
            <strong>Block HTML</strong> content renders on its own block.
        </div>
        """;

    public MainWindow()
    {
        InitializeComponent();
        Viewer.Markdown = SampleMarkdown;
    }
}