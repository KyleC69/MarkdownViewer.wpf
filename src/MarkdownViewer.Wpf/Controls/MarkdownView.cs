using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

using MarkdownViewer.Wpf.Core;
using MarkdownViewer.Wpf.Theming;

namespace MarkdownViewer.Wpf;

public class MarkdownView : Control
{
    private static readonly DependencyPropertyKey RenderedContentPropertyKey = DependencyProperty.RegisterReadOnly(
        nameof(RenderedContent),
        typeof(UIElement),
        typeof(MarkdownView),
        new PropertyMetadata(null));

    private bool refreshPending;

    private static readonly MarkdownEngine DefaultEngine = MarkdownEngine.CreateDefault();

    public static readonly DependencyProperty MarkdownProperty = DependencyProperty.Register(
        nameof(Markdown),
        typeof(string),
        typeof(MarkdownView),
        new PropertyMetadata(null, OnRefreshPropertyChanged));

    public static readonly DependencyProperty ThemeProperty = DependencyProperty.Register(
        nameof(Theme),
        typeof(ITheme),
        typeof(MarkdownView),
        new PropertyMetadata(new DefaultTheme(), OnRefreshPropertyChanged));

    public static readonly DependencyProperty ServicesProperty = DependencyProperty.Register(
        nameof(Services),
        typeof(IServiceProvider),
        typeof(MarkdownView),
        new PropertyMetadata(EmptyServiceProvider.Instance, OnRefreshPropertyChanged));

    public static readonly DependencyProperty RenderedContentProperty = RenderedContentPropertyKey.DependencyProperty;

    static MarkdownView()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(MarkdownView), new FrameworkPropertyMetadata(typeof(MarkdownView)));
    }

    public string? Markdown
    {
        get => (string?)GetValue(MarkdownProperty);
        set => SetValue(MarkdownProperty, value);
    }

    public ITheme? Theme
    {
        get => (ITheme?)GetValue(ThemeProperty);
        set => SetValue(ThemeProperty, value);
    }

    public IServiceProvider? Services
    {
        get => (IServiceProvider?)GetValue(ServicesProperty);
        set => SetValue(ServicesProperty, value);
    }

    public UIElement? RenderedContent
    {
        get => (UIElement?)GetValue(RenderedContentProperty);
        protected set => SetValue(RenderedContentPropertyKey, value);
    }

    internal static void OnRefreshPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
    {
        ((MarkdownView)dependencyObject).RequestRefresh();
    }

    internal void RequestRefresh()
    {
        if (refreshPending)
        {
            return;
        }

        refreshPending = true;
        Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(RefreshContent));
    }

    internal void RefreshContent()
    {
        refreshPending = false;

        ApplyThemeSurface(Theme ?? new DefaultTheme());

        if (string.IsNullOrWhiteSpace(Markdown))
        {
            RenderedContent = null;
            return;
        }

        RenderedContent = DefaultEngine.Render(
            Markdown,
            Theme ?? new DefaultTheme(),
            Services ?? EmptyServiceProvider.Instance);
    }

    private void ApplyThemeSurface(ITheme theme)
    {
        ArgumentNullException.ThrowIfNull(theme);

        if (theme.GetBrush(ThemeKeys.ViewerBackgroundBrush) is Brush background)
        {
            SetCurrentValue(BackgroundProperty, background);
        }

        if (theme.GetBrush(ThemeKeys.ViewerBorderBrush) is Brush borderBrush)
        {
            SetCurrentValue(BorderBrushProperty, borderBrush);
        }

        if (theme.GetThickness(ThemeKeys.ViewerBorderThickness) is Thickness borderThickness)
        {
            SetCurrentValue(BorderThicknessProperty, borderThickness);
        }
    }
}