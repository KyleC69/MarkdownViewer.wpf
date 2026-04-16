using System.Windows.Media;

namespace MarkdownViewer.Wpf.Core;

public interface IMarkdownImageSourceResolver
{
    ImageSource? ResolveImageSource(Uri uri, IRenderContext context);
}