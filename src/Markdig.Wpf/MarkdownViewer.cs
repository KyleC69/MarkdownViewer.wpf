// Build Date: 2026/04/15
// Solution: Markdig.Wpf
// Project:   MarkdownViewer.Wpf
// File:         MarkdownViewer.cs
// Author: Kyle L. Crowder
// Build Num: 184539



using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Threading;

using Markdig;

using MarkdownViewer.Wpf.Abstractions;
using MarkdownViewer.Wpf.Hosting;




namespace MarkdownViewer.Wpf
{


    /// <summary>
    ///     A markdown viewer control.
    /// </summary>
    public class MarkdownViewer : Control
    {
        private string? lastRenderedMarkdown;
        private MarkdownPipeline? lastRenderedPipeline;
        private IXamlRendererRegistry? lastRenderedRendererRegistry;

        private bool refreshPending;
        protected static readonly MarkdownPipeline DefaultPipeline = new MarkdownPipelineBuilder().UseSupportedExtensions().Build();

        private static readonly DependencyPropertyKey DocumentPropertyKey = DependencyProperty.RegisterReadOnly(nameof(Document), typeof(FlowDocument), typeof(MarkdownViewer), new FrameworkPropertyMetadata());

        /// <summary>
        ///     Defines the <see cref="Document" /> property.
        /// </summary>
        public static readonly DependencyProperty DocumentProperty = DocumentPropertyKey.DependencyProperty;

        /// <summary>
        ///     Defines the <see cref="Markdown" /> property.
        /// </summary>
        public static readonly DependencyProperty MarkdownProperty = DependencyProperty.Register(nameof(Markdown), typeof(string), typeof(MarkdownViewer), new FrameworkPropertyMetadata(MarkdownChanged));

        /// <summary>
        ///     Defines the <see cref="Markdown" /> property.
        /// </summary>
        public static readonly DependencyProperty PipelineProperty = DependencyProperty.Register(nameof(Pipeline), typeof(MarkdownPipeline), typeof(MarkdownViewer), new FrameworkPropertyMetadata(PipelineChanged));

        /// <summary>
        ///     Defines the <see cref="RendererRegistry" /> property.
        /// </summary>
        public static readonly DependencyProperty RendererRegistryProperty = DependencyProperty.Register(nameof(RendererRegistry), typeof(IXamlRendererRegistry), typeof(MarkdownViewer), new FrameworkPropertyMetadata(RendererRegistryChanged));

        /// <summary>
        ///     Defines the <see cref="IsToolBarVisible" /> property.
        /// </summary>
        public static readonly DependencyProperty IsToolBarVisibleProperty = DependencyProperty.Register(nameof(IsToolBarVisible), typeof(bool), typeof(MarkdownViewer), new FrameworkPropertyMetadata(false));

        /// <summary>
        ///     Defines the <see cref="MaxZoom" /> property.
        /// </summary>
        public static readonly DependencyProperty MaxZoomProperty = DependencyProperty.Register(nameof(MaxZoom), typeof(double), typeof(MarkdownViewer), new FrameworkPropertyMetadata(400d));

        /// <summary>
        ///     Defines the <see cref="MinZoom" /> property.
        /// </summary>
        public static readonly DependencyProperty MinZoomProperty = DependencyProperty.Register(nameof(MinZoom), typeof(double), typeof(MarkdownViewer), new FrameworkPropertyMetadata(20d));

        /// <summary>
        ///     Defines the <see cref="Zoom" /> property.
        /// </summary>
        public static readonly DependencyProperty ZoomProperty = DependencyProperty.Register(nameof(Zoom), typeof(double), typeof(MarkdownViewer), new FrameworkPropertyMetadata(100d));








        static MarkdownViewer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MarkdownViewer), new FrameworkPropertyMetadata(typeof(MarkdownViewer)));
            CommandManager.RegisterClassCommandBinding(typeof(MarkdownViewer), new CommandBinding(Commands.CodeBlockCopy, ExecutedCodeBlockCopy, CanExecuteCodeBlockCopy));
        }








        /// <summary>
        ///     Gets the flow document to display.
        /// </summary>
        public FlowDocument? Document
        { get => (FlowDocument)GetValue(DocumentProperty); protected set => SetValue(DocumentPropertyKey, value);
        }

        /// <summary>
        ///     Gets or sets a value indicating whether the built-in document toolbar is visible.
        /// </summary>
        public bool IsToolBarVisible
        { get => (bool)GetValue(IsToolBarVisibleProperty); set => SetValue(IsToolBarVisibleProperty, value);
        }

        /// <summary>
        ///     Gets or sets the markdown to display.
        /// </summary>
        public string? Markdown
        { get => (string)GetValue(MarkdownProperty); set => SetValue(MarkdownProperty, value);
        }

        /// <summary>
        ///     Gets or sets the maximum zoom percentage applied by the underlying document viewer.
        /// </summary>
        public double MaxZoom
        { get => (double)GetValue(MaxZoomProperty); set => SetValue(MaxZoomProperty, value);
        }

        /// <summary>
        ///     Gets or sets the minimum zoom percentage applied by the underlying document viewer.
        /// </summary>
        public double MinZoom
        { get => (double)GetValue(MinZoomProperty); set => SetValue(MinZoomProperty, value);
        }

        /// <summary>
        ///     Gets or sets the markdown pipeline to use.
        /// </summary>
        public MarkdownPipeline Pipeline
        { get => (MarkdownPipeline)GetValue(PipelineProperty); set => SetValue(PipelineProperty, value);
        }

        /// <summary>
        ///     Gets or sets the XAML renderer registry used to configure the rendering pipeline.
        /// </summary>
        public IXamlRendererRegistry? RendererRegistry
        { get => (IXamlRendererRegistry?)GetValue(RendererRegistryProperty); set => SetValue(RendererRegistryProperty, value);
        }

        /// <summary>
        ///     Gets or sets the zoom percentage applied by the underlying document viewer.
        /// </summary>
        public double Zoom
        { get => (double)GetValue(ZoomProperty); set => SetValue(ZoomProperty, value);
        }








        private static void CanExecuteCodeBlockCopy(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = e.Parameter is string text && !string.IsNullOrEmpty(text);
            e.Handled = true;
        }








        private static void ExecutedCodeBlockCopy(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Parameter is string text && !string.IsNullOrEmpty(text))
            {
                Clipboard.SetText(text);
                e.Handled = true;
            }
        }








        private static void MarkdownChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            MarkdownViewer control = (MarkdownViewer)sender;
            control.RequestRefresh();
        }








        private static void PipelineChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            MarkdownViewer control = (MarkdownViewer)sender;
            control.RequestRefresh();
        }








        private void ProcessPendingRefresh()
        {
            refreshPending = false;
            RefreshDocument();
        }








        protected virtual void RefreshDocument()
        {
            string? markdown = Markdown;
            MarkdownPipeline pipeline = Pipeline ?? DefaultPipeline;
            IXamlRendererRegistry? rendererRegistry = RendererRegistry;

            if (string.Equals(markdown, lastRenderedMarkdown, System.StringComparison.Ordinal) && ReferenceEquals(pipeline, lastRenderedPipeline) && ReferenceEquals(rendererRegistry, lastRenderedRendererRegistry))
            {
                return;
            }

            FlowDocument? document = markdown != null ? FlowDocumentXamlLoader.LoadDocument(global::MarkdownViewer.Wpf.Markdown.ToXaml(markdown, pipeline, rendererRegistry)) : null;

            Document = document;
            lastRenderedMarkdown = markdown;
            lastRenderedPipeline = pipeline;
            lastRenderedRendererRegistry = rendererRegistry;
        }








        private static void RendererRegistryChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            MarkdownViewer control = (MarkdownViewer)sender;
            control.RequestRefresh();
        }








        private void RequestRefresh()
        {
            if (refreshPending)
            {
                return;
            }

            refreshPending = true;
            var unused = Dispatcher.BeginInvoke(DispatcherPriority.Background, new System.Action(ProcessPendingRefresh));
        }
    }


}