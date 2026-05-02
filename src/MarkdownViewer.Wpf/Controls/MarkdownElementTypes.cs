// Solution: MarkdownViewer.Wpf
// Project:   MarkdownViewer.Wpf
// File:         MarkdownElementTypes.cs
// Author: Kyle L. Crowder
// Build Date: 2026/05/02



using System.Windows.Controls;
using System.Windows.Documents;




namespace MarkdownViewer.Wpf.Controls;





/// <summary>
///     Implicit-style control types for Markdown elements.
///     Each type allows themes to define element-specific implicit styles.
/// </summary>
/// <summary>TextBlock for Markdown paragraphs.</summary>
public sealed class ParagraphTextBlock : TextBlock
{
}





/// <summary>TextBlock for Markdown headings (level 1).</summary>
public sealed class Heading1TextBlock : TextBlock
{
}





/// <summary>TextBlock for Markdown headings (level 2).</summary>
public sealed class Heading2TextBlock : TextBlock
{
}





/// <summary>TextBlock for Markdown headings (level 3).</summary>
public sealed class Heading3TextBlock : TextBlock
{
}





/// <summary>TextBlock for Markdown headings (level 4).</summary>
public sealed class Heading4TextBlock : TextBlock
{
}





/// <summary>TextBlock for Markdown headings (level 5).</summary>
public sealed class Heading5TextBlock : TextBlock
{
}





/// <summary>TextBlock for Markdown headings (level 6).</summary>
public sealed class Heading6TextBlock : TextBlock
{
}





/// <summary>TextBlock for code block content.</summary>
public sealed class CodeBlockTextBlock : TextBlock
{
}





/// <summary>Span for inline code.</summary>
public sealed class CodeInlineSpan : Span
{
}





/// <summary>Border for block quotes.</summary>
public sealed class BlockQuoteBorder : Border
{
}





/// <summary>Border for code blocks.</summary>
public sealed class CodeBlockBorder : Border
{
}





/// <summary>Border for code block header area.</summary>
public sealed class CodeBlockHeaderBorder : Border
{
}





/// <summary>DockPanel for code block header.</summary>
public sealed class CodeBlockHeaderPanel : DockPanel
{
}





/// <summary>TextBlock for code block language header.</summary>
public sealed class CodeBlockHeaderTextBlock : TextBlock
{
}





/// <summary>Button for code block copy button.</summary>
public sealed class CodeBlockCopyButton : Button
{
}





/// <summary>ScrollViewer for code block scrolling.</summary>
public sealed class CodeBlockScrollViewer : ScrollViewer
{
}





/// <summary>StackPanel for lists.</summary>
public sealed class ListPanel : StackPanel
{
}





/// <summary>Grid for list items.</summary>
public sealed class ListItemGrid : Grid
{
}





/// <summary>TextBlock for list item markers.</summary>
public sealed class ListItemMarkerTextBlock : TextBlock
{
}





/// <summary>StackPanel for list item content.</summary>
public sealed class ListItemContentPanel : StackPanel
{
}





/// <summary>Grid for tables.</summary>
public sealed class TableGrid : Grid
{
}





/// <summary>Border for table cells.</summary>
public sealed class TableCellBorder : Border
{
}





/// <summary>TextBlock for inline strikethrough.</summary>
public sealed class StrikeThroughSpan : Span
{
}





/// <summary>TextBlock for inline subscript.</summary>
public sealed class SubscriptSpan : Span
{
}





/// <summary>TextBlock for inline superscript.</summary>
public sealed class SuperscriptSpan : Span
{
}





/// <summary>TextBlock for inserted text.</summary>
public sealed class InsertedSpan : Span
{
}





/// <summary>TextBlock for marked/highlighted text.</summary>
public sealed class MarkedSpan : Span
{
}





/// <summary>Border for thematic breaks (horizontal rules).</summary>
public sealed class ThematicBreakBorder : Border
{
}





/// <summary>Root container for rendered markdown.</summary>
public sealed class MarkdownRootPanel : WrappingStackPanel
{
}