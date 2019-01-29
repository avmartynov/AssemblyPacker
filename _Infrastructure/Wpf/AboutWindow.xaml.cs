using System;
using System.Configuration;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Twidlle.Infrastructure.CodeAnnotation;

namespace Twidlle.Infrastructure.Wpf
{
    public partial class AboutWindow
    {
        public static void Show([NotNull] Window owner, 
                                [NotNull] string appResourceIconKey, 
                                [NotNull] string copyrightYears = ProductInfo.Year)
        {
            var icon = (BitmapImage) Application.Current.Resources[appResourceIconKey];
            var dlg = new AboutWindow(icon, copyrightYears) { Owner = owner };
            dlg.ShowDialog();
        }


        private AboutWindow([NotNull] ImageSource bitmapImage, 
                            [NotNull] string copyrightYears = ProductInfo.Year,
                            UriKind uriKind = UriKind.Relative)
        {
            InitializeComponent();

            Title                   += " " + ApplicationInfo.ProductName;
            _productLabel.Content   =        ApplicationInfo.ProductName;
            _versionLabel.Content   += " " + ApplicationInfo.Version;
            _copyrightLabel.Content += " " + copyrightYears + " " + ApplicationInfo.CompanyName;

            _image.Source = bitmapImage;
        }


        private void button_Click(object sender, RoutedEventArgs e)
            => Close();


        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if ((Keyboard.Modifiers & ModifierKeys.Control) != ModifierKeys.Control)
                return;

            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (e.Key)
            {
                case Key.D:
                    Clipboard.SetText(GetDiagnosticsInfo());
                    break;

                case Key.C:
                {
                    if (!TryOpenUserConfigFileFolder())
                        MessageBox.Show(this, "User config file does not exist.");
                    break;
                }
            }

        }

        private static bool TryOpenUserConfigFileFolder()
        {
            var fileName = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal).FilePath;
            var dirName = Path.GetDirectoryName(fileName);
            if (dirName == null)
                return false;

            var dir = new DirectoryInfo(dirName);
            if (!dir.Exists)
                return false;

            dir.Open();
            return true;
        }


        [NotNull]
        private static string GetDiagnosticsInfo()
        {
            return new
                   {
                       ClrVersion = Environment.Version,
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
