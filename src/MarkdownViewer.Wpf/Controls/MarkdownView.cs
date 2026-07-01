// Solution: MarkdownViewer.Wpf
// Project:   MarkdownViewer.Wpf
// File:         MarkdownView.cs
// Author: Kyle L. Crowder
// Build Date: 2026/05/02



using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Threading;

using MarkdownViewer.Wpf.Core;




namespace MarkdownViewer.Wpf;





public class MarkdownView : Control
{

    private bool refreshPending;

    private static readonly MarkdownEngine DefaultEngine = MarkdownEngine.CreateDefault();

    private static readonly DependencyPropertyKey RenderedContentPropertyKey = DependencyProperty.RegisterReadOnly(nameof(RenderedContent), typeof(UIElement), typeof(MarkdownView), new PropertyMetadata(null));

    public static readonly DependencyProperty MarkdownProperty = DependencyProperty.Register(nameof(Markdown), typeof(string), typeof(MarkdownView), new PropertyMetadata(null, OnRefreshPropertyChanged));

    public static readonly DependencyProperty ThemeResourcesProperty = DependencyProperty.Register(nameof(ThemeResources), typeof(ResourceDictionary), typeof(MarkdownView), new PropertyMetadata(null, OnRefreshPropertyChanged));

    public static readonly DependencyProperty ServicesProperty = DependencyProperty.Register(nameof(Services), typeof(IServiceProvider), typeof(MarkdownView), new PropertyMetadata(EmptyServiceProvider.Instance, OnRefreshPropertyChanged));

    public static readonly DependencyProperty WordWrapProperty = DependencyProperty.Register(nameof(WordWrap), typeof(bool), typeof(MarkdownView), new PropertyMetadata(true, OnRefreshPropertyChanged));

    public static readonly DependencyProperty RenderedContentProperty = RenderedContentPropertyKey.DependencyProperty;








    static MarkdownView()
    {
        // Register the default style for the control once. Duplicate registration causes a PropertyMetadata already registered exception.
        DefaultStyleKeyProperty.OverrideMetadata(typeof(MarkdownView), new FrameworkPropertyMetadata(typeof(MarkdownView)));
    }








    public string? Markdown
    {
        get { return (string?)this.GetValue(MarkdownProperty); }
        set { this.SetValue(MarkdownProperty, value); }
    }

    public UIElement? RenderedContent
    {
        get { return (UIElement?)this.GetValue(RenderedContentProperty); }
        protected set { this.SetValue(RenderedContentPropertyKey, value); }
    }

    public IServiceProvider? Services
    {
        get { return (IServiceProvider?)this.GetValue(ServicesProperty); }
        set { this.SetValue(ServicesProperty, value); }
    }

    public ResourceDictionary? ThemeResources
    {
        get { return (ResourceDictionary?)this.GetValue(ThemeResourcesProperty); }
        set { this.SetValue(ThemeResourcesProperty, value); }
    }

    public bool WordWrap
    {
        get { return (bool)this.GetValue(WordWrapProperty); }
        set { this.SetValue(WordWrapProperty, value); }
    }

    // New properties to control scrollbars visibility
    public static readonly DependencyProperty HorizontalScrollBarVisibilityProperty = DependencyProperty.Register(
        nameof(HorizontalScrollBarVisibility), typeof(ScrollBarVisibility), typeof(MarkdownView), new PropertyMetadata(ScrollBarVisibility.Auto));

    public static readonly DependencyProperty VerticalScrollBarVisibilityProperty = DependencyProperty.Register(
        nameof(VerticalScrollBarVisibility), typeof(ScrollBarVisibility), typeof(MarkdownView), new PropertyMetadata(ScrollBarVisibility.Auto));

    public ScrollBarVisibility HorizontalScrollBarVisibility
    {
        get { return (ScrollBarVisibility)this.GetValue(HorizontalScrollBarVisibilityProperty); }
        set { this.SetValue(HorizontalScrollBarVisibilityProperty, value); }
    }

    public ScrollBarVisibility VerticalScrollBarVisibility
    {
        get { return (ScrollBarVisibility)this.GetValue(VerticalScrollBarVisibilityProperty); }
        set { this.SetValue(VerticalScrollBarVisibilityProperty, value); }
    }








    internal static void OnRefreshPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
    {
        ((MarkdownView)dependencyObject).RequestRefresh();
    }








    internal void RefreshContent()
    {
        refreshPending = false;

        if (string.IsNullOrWhiteSpace(Markdown))
        {
            RenderedContent = null;
            return;
        }

        RenderedContent = DefaultEngine.Render(Markdown, Services ?? EmptyServiceProvider.Instance, ThemeResources, WordWrap);
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
}
