namespace MarkdownViewer.Wpf.Core;

public interface IMarkdownLinkNavigator
{
    bool TryNavigate(Uri uri, IRenderContext context);
}