// Build Date: 2026/04/15
// Solution: Markdig.Wpf
// Project:   MarkdownViewer.Wpf
// File:         Commands.cs
// Author: Kyle L. Crowder
// Build Num: 184538



using System.Windows.Input;




namespace MarkdownViewer.Wpf
{


    /// <summary>
    ///     List of supported commands.
    /// </summary>
    public static class Commands
    {
        /// <summary>
        ///     Routed command for copying a rendered code block.
        /// </summary>
        public static RoutedCommand CodeBlockCopy { get; } = new RoutedCommand(nameof(CodeBlockCopy), typeof(Commands));

        /// <summary>
        ///     Routed command for Hyperlink.
        /// </summary>
        public static RoutedCommand Hyperlink { get; } = new RoutedCommand(nameof(Hyperlink), typeof(Commands));

        /// <summary>
        ///     Routed command for Images.
        /// </summary>
        public static RoutedCommand Image { get; } = new RoutedCommand(nameof(Image), typeof(Commands));
    }


}