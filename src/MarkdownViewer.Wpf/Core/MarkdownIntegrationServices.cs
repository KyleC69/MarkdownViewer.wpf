using System.Diagnostics;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using MarkdownViewer.Wpf.Diagnostics;

namespace MarkdownViewer.Wpf.Core;

internal static class MarkdownIntegrationServices
{
    private static readonly IMarkdownImageSourceResolver defaultImageSourceResolver = new DefaultMarkdownImageSourceResolver();
    private static readonly IMarkdownLinkNavigator defaultLinkNavigator = new DefaultMarkdownLinkNavigator();

    public static void ConfigureHyperlink(Hyperlink hyperlink, Uri uri, IRenderContext context)
    {
        ArgumentNullException.ThrowIfNull(hyperlink);
        ArgumentNullException.ThrowIfNull(uri);
        ArgumentNullException.ThrowIfNull(context);

        hyperlink.NavigateUri = uri;
        hyperlink.Click += static (_, args) =>
        {
            if (args.Source is not Hyperlink source || source.NavigateUri is not Uri navigateUri)
            {
                return;
            }

            IRenderContext renderContext = (IRenderContext)source.GetValue(HyperlinkContextProperty);
            IMarkdownLinkNavigator navigator = renderContext.Services.GetService(typeof(IMarkdownLinkNavigator)) as IMarkdownLinkNavigator
                ?? defaultLinkNavigator;

            try
            {
                navigator.TryNavigate(navigateUri, renderContext);
            }
            catch (Exception exception)
            {
                MarkdownDiagnostics.ReportLinkNavigationFailure(navigateUri, exception);
            }
        };

        hyperlink.SetValue(HyperlinkContextProperty, context);
    }

    public static ImageSource? ResolveImageSource(Uri uri, IRenderContext context)
    {
        ArgumentNullException.ThrowIfNull(uri);
        ArgumentNullException.ThrowIfNull(context);

        IMarkdownImageSourceResolver resolver = context.Services.GetService(typeof(IMarkdownImageSourceResolver)) as IMarkdownImageSourceResolver
            ?? defaultImageSourceResolver;

        try
        {
            return resolver.ResolveImageSource(uri, context);
        }
        catch (Exception exception)
        {
            MarkdownDiagnostics.ReportImageLoadFailure(uri, exception);
            return null;
        }
    }

    private static readonly DependencyProperty HyperlinkContextProperty = DependencyProperty.RegisterAttached(
        "HyperlinkContext",
        typeof(IRenderContext),
        typeof(MarkdownIntegrationServices),
        new PropertyMetadata(null));

    private sealed class DefaultMarkdownImageSourceResolver : IMarkdownImageSourceResolver
    {
        public ImageSource? ResolveImageSource(Uri uri, IRenderContext context)
        {
            BitmapImage bitmap = new();
            bitmap.BeginInit();
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.UriSource = uri;
            bitmap.EndInit();

            if (bitmap.CanFreeze)
            {
                bitmap.Freeze();
            }

            return bitmap;
        }
    }

    private sealed class DefaultMarkdownLinkNavigator : IMarkdownLinkNavigator
    {
        public bool TryNavigate(Uri uri, IRenderContext context)
        {
            Process.Start(new ProcessStartInfo(uri.ToString()) { UseShellExecute = true });
            return true;
        }
    }
}