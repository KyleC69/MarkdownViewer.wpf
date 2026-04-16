// Build Date: 2026/04/15
// Solution: Markdig.Wpf
// Project:   MarkdownViewer.Wpf
// File:         DefaultTaskListRenderer.cs
// Author: Kyle L. Crowder
// Build Num: 184534



using Markdig.Extensions.TaskLists;




namespace MarkdownViewer.Wpf.Rendering.Xaml.Defaults.Inlines
{


    /// <summary>
    ///     Default XAML renderer for markdown task list items.
    /// </summary>
    public sealed class DefaultTaskListRenderer : DefaultXamlInlineRenderer<TaskList>
    {
        protected override void WriteObject(Abstractions.IXamlRenderContext renderer, TaskList taskList)
        {
            renderer.WriteLine("<InlineUIContainer>");
            renderer.Write("<CheckBox IsEnabled=\"False\"");
            WriteStyleAttribute(renderer, nameof(Styles.TaskListStyleKey));
            renderer.Write(" IsChecked=\"").Write(taskList.Checked ? "True" : "False").Write("\"");

            renderer.WriteLine(" />");
            renderer.WriteLine("</InlineUIContainer>");
        }
    }


}