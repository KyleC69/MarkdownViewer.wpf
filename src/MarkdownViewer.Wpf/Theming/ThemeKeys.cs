namespace MarkdownViewer.Wpf.Theming;

public static class ThemeKeys
{
    public const string RootPanelStyle = nameof(RootPanelStyle);
    public const string ParagraphStyle = nameof(ParagraphStyle);
    public const string Heading1Style = nameof(Heading1Style);
    public const string Heading2Style = nameof(Heading2Style);
    public const string Heading3Style = nameof(Heading3Style);
    public const string Heading4Style = nameof(Heading4Style);
    public const string Heading5Style = nameof(Heading5Style);
    public const string Heading6Style = nameof(Heading6Style);
    public const string ListStyle = nameof(ListStyle);
    public const string ListItemContainerStyle = nameof(ListItemContainerStyle);
    public const string ListItemMarkerStyle = nameof(ListItemMarkerStyle);
    public const string ListItemContentStyle = nameof(ListItemContentStyle);
    public const string BlockQuoteBorderStyle = nameof(BlockQuoteBorderStyle);
    public const string CodeBlockBorderStyle = nameof(CodeBlockBorderStyle);
    public const string CodeBlockHeaderPanelStyle = nameof(CodeBlockHeaderPanelStyle);
    public const string CodeBlockHeaderTextStyle = nameof(CodeBlockHeaderTextStyle);
    public const string CodeBlockTextStyle = nameof(CodeBlockTextStyle);
    public const string CodeBlockCopyButtonStyle = nameof(CodeBlockCopyButtonStyle);
    public const string CodeBlockScrollViewerStyle = nameof(CodeBlockScrollViewerStyle);
    public const string CodeInlineStyle = nameof(CodeInlineStyle);
    public const string HtmlBlockStyle = nameof(HtmlBlockStyle);
    public const string TableStyle = nameof(TableStyle);
    public const string TableCellBorderStyle = nameof(TableCellBorderStyle);
    public const string TableHeaderCellBorderStyle = nameof(TableHeaderCellBorderStyle);
    public const string ThematicBreakStyle = nameof(ThematicBreakStyle);
    public const string StrikeThroughStyle = nameof(StrikeThroughStyle);
    public const string SubscriptStyle = nameof(SubscriptStyle);
    public const string SuperscriptStyle = nameof(SuperscriptStyle);
    public const string InsertedStyle = nameof(InsertedStyle);
    public const string MarkedStyle = nameof(MarkedStyle);
    public const string DiagnosticsOverlayBorderBrush = nameof(DiagnosticsOverlayBorderBrush);
    public const string DiagnosticsOverlayBorderThickness = nameof(DiagnosticsOverlayBorderThickness);
    public const string DiagnosticsLabelBorderStyle = nameof(DiagnosticsLabelBorderStyle);
    public const string DiagnosticsLabelTextStyle = nameof(DiagnosticsLabelTextStyle);

    public static string GetHeadingStyle(int level)
    {
        return level switch
        {
            1 => Heading1Style,
            2 => Heading2Style,
            3 => Heading3Style,
            4 => Heading4Style,
            5 => Heading5Style,
            _ => Heading6Style,
        };
    }
}