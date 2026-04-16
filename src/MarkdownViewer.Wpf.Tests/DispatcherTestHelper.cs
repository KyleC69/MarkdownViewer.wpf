using System.Windows.Threading;

namespace MarkdownViewer.Wpf.Tests;

internal static class DispatcherTestHelper
{
    public static void Drain()
    {
        DispatcherFrame frame = new();

        Dispatcher.CurrentDispatcher.BeginInvoke(
            DispatcherPriority.Background,
            new DispatcherOperationCallback(static state =>
            {
                ((DispatcherFrame)state!).Continue = false;
                return null;
            }),
            frame);

        Dispatcher.PushFrame(frame);
    }
}