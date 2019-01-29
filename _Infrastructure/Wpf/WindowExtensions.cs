using System;
using System.Windows;
using Twidlle.Infrastructure.CodeAnnotation;

namespace Twidlle.Infrastructure.Wpf
{
    public static class WindowExtensions
    {
        public static void InvokeUI([NotNull] this Window owner, [NotNull] Action action)
        {
            owner  = owner  ?? throw new ArgumentNullException(nameof(owner));
            action = action ?? throw new ArgumentNullException(nameof(action));
            try
            {
                owner.Dispatcher.Invoke(action);
            }
            catch (ObjectDisposedException)
            {
                // В очереди сообщений могут оставаться сообщения для форм, которые уже освобождены. 
            }
            catch (Exception x)
            {
                owner.ShowMessageBox(x);
            }
        }


        /// <summary> Шаблонный метод-обработчик события в форме. </summary>
        public static void InvokeChecked([NotNull] this Window owner, [NotNull] Action action)
        {
            owner  = owner  ?? throw new ArgumentNullException(nameof(owner));
            action = action ?? throw new ArgumentNullException(nameof(action));
            try
            {
                action.Invoke();
            }
            catch (Exception x)
            {
                owner.ShowMessageBox(x);
            }
        }


        public static void ShowMessageBox([NotNull] this Window owner, [NotNull] Exception exception, 
            [CanBeNull] string message = null,
            [CanBeNull] string caption = null)
        {
            var diagnosticsInfo = $"ClrVersion = {Environment.Version}, " +
                                  $"OSVersion = {Environment.OSVersion}, " +
                                  $"Is64BitOS = {Environment.Is64BitOperatingSystem}, " +
                                  $"Is64BitProcess = {Environment.Is64BitProcess}, " +
                                  $"ProcessorCount = {Environment.ProcessorCount}, " +
                                  $"CurrentDirectory = {Environment.CurrentDirectory}, " +
                                  $"CommandLine = {Environment.CommandLine}, " +
                                  $"MachineName = {Environment.MachineName}, " +
                                  $"DomainName = {Environment.UserDomainName}, " +
                                  $"UserName = {Environment.UserName}, " + Environment.NewLine +
                                  exception;
            Clipboard.SetText(diagnosticsInfo);

            caption = caption ?? Application.Current.MainWindow?.Title ?? "";
            message = $"{exception.Message}" + Environment.NewLine + Environment.NewLine
                      + (message ?? "Clipboard contains details of error.");

            MessageBox.Show(owner, message, caption, MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
