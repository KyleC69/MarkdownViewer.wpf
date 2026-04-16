using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

using MarkdownViewer.Wpf.Core;

namespace MarkdownViewer.Wpf;

public class MarkdownView : Control
{
    private ResourceDictionary? appliedThemeResources;
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

    public static readonly DependencyProperty ThemeResourcesProperty = DependencyProperty.Register(
        nameof(ThemeResources),
        typeof(ResourceDictionary),
        typeof(MarkdownView),
        new PropertyMetadata(null, OnRefreshPropertyChanged));

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

    public MarkdownView()
    {
        EnsureThemeResources();
    }

    public string? Markdown
    {
        get => (string?)GetValue(MarkdownProperty);
        set => SetValue(MarkdownProperty, value);
    }

    public ResourceDictionary? ThemeResources
    {
        get => (ResourceDictionary?)GetValue(ThemeResourcesProperty);
        set => SetValue(ThemeResourcesProperty, value);
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
        EnsureThemeResources();

        if (string.IsNullOrWhiteSpace(Markdown))
        {
            RenderedContent = null;
            return;
        }

        RenderedContent = DefaultEngine.Render(
            Markdown,
            Services ?? EmptyServiceProvider.Instance,
            ResolveThemeResources());
    }

    private void EnsureThemeResources()
    {
        if (appliedThemeResources is not null)
        {
            Resources.MergedDictionaries.Remove(appliedThemeResources);
        }

        appliedThemeResources = ResolveThemeResources();
        if (appliedThemeResources is not null)
        {
            Resources.MergedDictionaries.Add(appliedThemeResources);
        }
    }

    private ResourceDictionary? ResolveThemeResources()
    {
        return ThemeResources;
    }
}