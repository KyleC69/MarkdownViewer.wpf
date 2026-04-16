using System.Windows.Controls;
using System.Windows.Documents;

using Markdig.Extensions.TaskLists;

using MarkdownViewer.Wpf.Core;
using MarkdownViewer.Wpf.Theming;

namespace MarkdownViewer.Wpf.Rendering.Inlines;

public sealed class TaskListInlineRenderer : IInlineRenderer<TaskList>
{
    public System.Windows.Documents.Inline Render(TaskList inline, IRenderContext context)
    {
        ArgumentNullException.ThrowIfNull(inline);
        ArgumentNullException.ThrowIfNull(context);

        CheckBox checkBox = new()
        {
            IsChecked = inline.Checked,
            IsEnabled = false,
        };
        return new InlineUIContainer(checkBox);
    }
}