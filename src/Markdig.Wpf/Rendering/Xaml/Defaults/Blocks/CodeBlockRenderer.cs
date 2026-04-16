// Build Date: 2026/04/15
// Solution: Markdig.Wpf
// Project:   MarkdownViewer.Wpf
// File:         CodeBlockRenderer.cs
// Author: Kyle L. Crowder
// Build Num: 184532



using System;
using System.Reflection;
using System.Text;

using Markdig.Helpers;
using Markdig.Syntax;




namespace MarkdownViewer.Wpf.Rendering.Xaml.Defaults.Blocks
{


    /// <summary>
    ///     A XAML renderer for a <see cref="CodeBlock" />.
    /// </summary>
    /// <seealso cref="Xaml.XamlObjectRenderer{T}" />
    public class DefaultCodeBlockRenderer : DefaultXamlBlockRenderer<CodeBlock>
    {
        protected override void WriteObject(Abstractions.IXamlRenderContext renderer, CodeBlock obj)
        {
            renderer.EnsureLine();

            var codeText = GetCodeText(obj);

            WriteContainerStart(renderer, obj, codeText);
            WriteHeader(renderer, obj, codeText);
            WriteBody(renderer, obj);
            WriteContainerEnd(renderer, obj, codeText);
        }








        protected virtual string GetCodeText(CodeBlock obj)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));

            StringBuilder builder = new();
            StringLineGroup lines = obj.Lines;
            var slices = lines.Lines;
            for (var index = 0; index < lines.Count; index++)
            {
                if (index > 0)
                {
                    builder.AppendLine();
                }

                StringSlice slice = slices[index].Slice;
                if (slice.Start <= slice.End)
                {
                    builder.Append(slice.Text, slice.Start, slice.Length);
                }
            }

            return builder.ToString();
        }








        protected virtual string GetCopyButtonText(CodeBlock obj)
        {
            return "Copy";
        }








        private static string? GetFencedCodeInfo(CodeBlock obj)
        {
            PropertyInfo? infoProperty = obj.GetType().GetProperty("Info", BindingFlags.Instance | BindingFlags.Public);
            if (infoProperty == null)
            {
                return null;
            }

            var infoValue = infoProperty.GetValue(obj);
            if (infoValue is string text)
            {
                return string.IsNullOrWhiteSpace(text) ? null : text.Trim();
            }

            if (infoValue is StringSlice slice)
            {
                if (slice.Start > slice.End)
                {
                    return null;
                }

                var value = slice.Text.Substring(slice.Start, slice.Length).Trim();
                return string.IsNullOrWhiteSpace(value) ? null : value;
            }

            var fallback = infoValue?.ToString()?.Trim();
            return string.IsNullOrWhiteSpace(fallback) ? null : fallback;
        }








        protected virtual string? GetHeaderText(CodeBlock obj)
        {
            return GetFencedCodeInfo(obj);
        }








        protected virtual bool ShowCopyButton(CodeBlock obj)
        {
            return true;
        }








        protected virtual void WriteBody(Abstractions.IXamlRenderContext renderer, CodeBlock obj)
        {
            renderer.Write("<Paragraph xml:space=\"preserve\"");
            WriteStyleAttribute(renderer, nameof(Styles.CodeBlockStyleKey));
            renderer.WriteLine(">");
            WriteLeafTextInlines(renderer, obj);
            renderer.WriteLine("</Paragraph>");
        }








        protected virtual void WriteContainerEnd(Abstractions.IXamlRenderContext renderer, CodeBlock obj, string codeText)
        {
            renderer.WriteLine("</Section>");
        }








        protected virtual void WriteContainerStart(Abstractions.IXamlRenderContext renderer, CodeBlock obj, string codeText)
        {
            renderer.Write("<Section");
            WriteStyleAttribute(renderer, nameof(Styles.CodeBlockContainerStyleKey));
            renderer.WriteLine(">");
        }








        protected virtual void WriteCopyButton(Abstractions.IXamlRenderContext renderer, CodeBlock obj, string codeText)
        {
            renderer.Write("<Button DockPanel.Dock=\"Right\"");
            WriteStyleAttribute(renderer, nameof(Styles.CodeBlockCopyButtonStyleKey));
            WriteCommandAttribute(renderer, nameof(Commands.CodeBlockCopy));
            renderer.Write(" Content=\"").WriteEscape(GetCopyButtonText(obj)).WriteLine("\">");
            renderer.WriteLine("<Button.CommandParameter>");
            renderer.Write("<x:String>").WriteEscape(codeText).WriteLine("</x:String>");
            renderer.WriteLine("</Button.CommandParameter>");
            renderer.WriteLine("</Button>");
        }








        protected virtual void WriteHeader(Abstractions.IXamlRenderContext renderer, CodeBlock obj, string codeText)
        {
            var headerText = GetHeaderText(obj);
            var showCopyButton = ShowCopyButton(obj);
            if (string.IsNullOrWhiteSpace(headerText) && !showCopyButton)
            {
                return;
            }

            renderer.WriteLine("<BlockUIContainer>");
            renderer.Write("<Border");
            WriteStyleAttribute(renderer, nameof(Styles.CodeBlockHeaderStyleKey));
            renderer.WriteLine(">");
            renderer.WriteLine("<DockPanel LastChildFill=\"False\">");

            if (!string.IsNullOrWhiteSpace(headerText))
            {
                renderer.Write("<TextBlock DockPanel.Dock=\"Left\"");
                WriteStyleAttribute(renderer, nameof(Styles.CodeBlockHeaderTextStyleKey));
                renderer.Write(" Text=\"").WriteEscape(headerText).WriteLine("\" />");
            }

            if (showCopyButton)
            {
                WriteCopyButton(renderer, obj, codeText);
            }

            renderer.WriteLine("</DockPanel>");
            renderer.WriteLine("</Border>");
            renderer.WriteLine("</BlockUIContainer>");
        }
    }


}