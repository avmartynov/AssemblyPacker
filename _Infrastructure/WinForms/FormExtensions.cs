using System;
using System.Windows.Forms;
using Twidlle.Infrastructure.CodeAnnotation;

namespace Twidlle.Infrastructure.WinForms
{
    public static class FormExtensions
    {
        /// <summary> Шаблонный метод-уведомление формы из NotUI-потока. </summary>
        public static void InvokeUI([NotNull] this Form owner, [NotNull] Action action)
        {
            action = action ?? throw new ArgumentNullException(nameof(action));
            try
            {
                if (owner.InvokeRequired)
                    owner.Invoke(action);
                else
                    action();
            }
            catch (ObjectDisposedException)
            {
                // В очереди сообщений могут оставаться сообщения для форм, которые уже освобождены. 
            }
            catch (Exception x)
            {
                x.ShowMessageBox(owner ?? throw new ArgumentNullException(nameof(owner)));
            }
        }


        /// <summary> Шаблонный метод-обработчик события в форме. </summary>
        public static void InvokeChecked([NotNull] this Form owner, [NotNull] Action action)
        {
            try
            {
                (action ?? throw new ArgumentNullException(nameof(action))).Invoke();
            }
            catch (Exception x)
            {
                x.ShowMessageBox(owner ?? throw new ArgumentNullException(nameof(owner)));
            }
        }


        public static void ShowMessageBox([NotNull] this Exception exception,
                                          [CanBeNull] IWin32Window owner = null,
                                          [CanBeNull] string message = null,
                                          [CanBeNull] string caption = null)
        {
            Clipboard.SetText(GetDiagnosticsInfo()+ ", " + Environment.NewLine + exception);

            caption = caption ?? Application.ProductName;
            message = $"{exception.Message}" + Environment.NewLine + Environment.NewLine
                      + (message ?? "Clipboard contains details of error.");

            MessageBox.Show(owner, message, caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }


        [NotNull]
        private static string GetDiagnosticsInfo()
        {
            return new { ClrVersion = Environment.Version,
                           OSVersion  = Environment.OSVersion.VersionString,
                           Is64BitOS  = Environment.Is64BitOperatingSystem,
                           Environment.Is64BitProcess,
                           Environment.ProcessorCount,
                           Environment.CurrentDirectory,
                           Environment.CommandLine,
                           Environment.MachineName,
                           DomainName = Environment.UserDomainName,
                           Environment.UserName
                       }.ToJson();
        }
    }
}
