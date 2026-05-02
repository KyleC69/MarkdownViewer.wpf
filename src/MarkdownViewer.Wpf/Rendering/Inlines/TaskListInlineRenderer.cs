// Solution: MarkdownViewer.Wpf
// Project:   MarkdownViewer.Wpf
// File:         TaskListInlineRenderer.cs
// Author: Kyle L. Crowder
// Build Date: 2026/05/02



using System.Windows.Controls;
using System.Windows.Documents;

using Markdig.Extensions.TaskLists;

using MarkdownViewer.Wpf.Core;




namespace MarkdownViewer.Wpf.Rendering.Inlines;





public sealed class TaskListInlineRenderer : IInlineRenderer<TaskList>
{
    public Inline Render(TaskList inline, IRenderContext context)
    {
        ArgumentNullException.ThrowIfNull(inline);
        ArgumentNullException.ThrowIfNull(context);

        CheckBox checkBox = new() { IsChecked = inline.Checked, IsEnabled = false };
        return new InlineUIContainer(checkBox);
    }
}