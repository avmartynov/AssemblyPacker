using System;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Twidlle.Infrastructure.CodeAnnotation;

namespace Twidlle.Infrastructure.WinForms
{
    public partial class AboutForm : Form
    {
        public AboutForm([NotNull] Icon icon, [NotNull] string copyrightYears = ProductInfo.Year)
        {
            InitializeComponent();
            _pictureBox.Image = icon.ToBitmap();
            _copyrightLabel.Text += " " + copyrightYears;
        }


        private void AboutForm_Load(object sender, EventArgs e)
        {
            Text                 += " " + Application.ProductName;
            _productLabel.Text   =        Application.ProductName;
            _versionLabel.Text   += " " + Application.ProductVersion;
            _copyrightLabel.Text += " " + Application.CompanyName;
        }


        private void AboutForm_KeyDown(object sender, [NotNull] KeyEventArgs e)
        {
            if (!e.Control)
                return;

            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (e.KeyCode)
            {
                case Keys.D:
                    Clipboard.SetText(GetDiagnosticsInfo());
                    break;
                case Keys.C:
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
