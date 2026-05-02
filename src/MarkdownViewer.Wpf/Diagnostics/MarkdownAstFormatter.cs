// Solution: MarkdownViewer.Wpf
// Project:   MarkdownViewer.Wpf
// File:         MarkdownAstFormatter.cs
// Author: Kyle L. Crowder
// Build Date: 2026/05/02



using System.Text;

using Markdig.Syntax;
using Markdig.Syntax.Inlines;




namespace MarkdownViewer.Wpf.Diagnostics;





public static class MarkdownAstFormatter
{

    private static void AppendBlock(StringBuilder builder, Block block, int depth)
    {
        builder.Append(' ', depth * 2);
        builder.AppendLine(block.GetType().Name);

        if (block is LeafBlock leaf && leaf.Inline is not null)
        {
            AppendInline(builder, leaf.Inline, depth + 1);
        }

        if (block is ContainerBlock container)
        {
            foreach (Block child in container)
            {
                AppendBlock(builder, child, depth + 1);
            }
        }
    }








    private static void AppendInline(StringBuilder builder, ContainerInline container, int depth)
    {
        for (Inline? current = container.FirstChild; current is not null; current = current.NextSibling)
        {
            builder.Append(' ', depth * 2);
            builder.AppendLine(current.GetType().Name);

            if (current is ContainerInline nested)
            {
                AppendInline(builder, nested, depth + 1);
            }
        }
    }








    public static string Format(MarkdownDocument document)
    {
        ArgumentNullException.ThrowIfNull(document);

        StringBuilder builder = new();
        foreach (Block block in document)
        {
            AppendBlock(builder, block, 0);
        }

        return builder.ToString().TrimEnd();
    }
}