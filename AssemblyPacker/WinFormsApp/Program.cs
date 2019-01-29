using System;
using System.Threading;
using System.Windows.Forms;
using Twidlle.AssemblyPacker.WinFormsApp.Forms;
using Twidlle.AssemblyPacker.WinFormsApp.Properties;
using Twidlle.Infrastructure.WinForms;

namespace Twidlle.AssemblyPacker.WinFormsApp
{
    public sealed class Program : IDisposable
    {
        [STAThread]
        public static void Main(String[] args)
        {
            try
            {
                using (var program = new Program())
                    program.Run();
            }
            catch (Exception exception)
            {
                exception.ShowMessageBox();
            }
        }

        public Program()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.ThreadException += ThreadExceptionEventHandler;

            _mainForm = new MainForm();
        }


        private void Run()
        {
            Application.Run(_mainForm);
            Settings.Default.Save();
        }


        private void ThreadExceptionEventHandler(object sender, ThreadExceptionEventArgs args)
        {
            args.Exception.ShowMessageBox(_mainForm);
            Environment.Exit(1);
        }


        public void Dispose()
            => _mainForm.Dispose();


        private readonly Form _mainForm;
    }
}
