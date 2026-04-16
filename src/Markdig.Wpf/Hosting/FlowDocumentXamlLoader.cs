// Build Date: 2026/04/15
// Solution: Markdig.Wpf
// Project:   MarkdownViewer.Wpf
// File:         FlowDocumentXamlLoader.cs
// Author: Kyle L. Crowder
// Build Num: 184532



using System;
using System.Windows.Documents;
using System.Windows.Markup;




namespace MarkdownViewer.Wpf.Hosting
{


    internal static class FlowDocumentXamlLoader
    {
        public static FlowDocument? LoadDocument(string? xaml)
        {
            if (string.IsNullOrWhiteSpace(xaml))
            {
                return null;
            }

            return XamlReader.Parse(xaml) as FlowDocument ?? throw new InvalidOperationException("The generated XAML did not produce a FlowDocument.");
        }
    }


}