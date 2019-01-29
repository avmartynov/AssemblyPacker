using System;
using System.Linq;
using System.IO;
using System.Windows.Forms;
using Twidlle.AssemblyPacker.Core;
using Twidlle.AssemblyPacker.Interface;
using Twidlle.AssemblyPacker.WinFormsApp.Properties;
using Twidlle.Infrastructure;
using Twidlle.Infrastructure.CodeAnnotation;
using Twidlle.Infrastructure.WinForms;

namespace Twidlle.AssemblyPacker.WinFormsApp.Forms
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            _packer = new Packer();

            FormRestoreManager.Initialize(this, Settings.Default, s => s.MainForm);

            InitializeComponent();

            _bindingSource.DataSource = new MainFormModel().CopyFrom(Settings.Default);
        }


        public MainFormModel Model 
            => (MainFormModel)_bindingSource.DataSource;


        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
            => Model.CopyTo(Settings.Default);


        private void MainForm_Load(object sender, EventArgs e)
        {
            Model.CopyFrom(Settings.Default);
            
            Text = ApplicationInfo.ProductName;
        }


        private void closeButton_Click(object sender, EventArgs e) 
            => Close();


        private void PackButton_Click(object sender, EventArgs e)
        {
            try
            {
                UseWaitCursor = true;
                SetDefaultPackageName();

                if (!TryValidateInputDirectory(Model.InputDirectory, out var errorMessage)
                 || !TryValidateOutputDirectory(Model.OutputDirectory, out errorMessage)
                 || !TryValidateAssemblyName(Model.InputDirectory, Model.PackedAssemblyName, out errorMessage))
                {
                    ShowErrorMessage(errorMessage);
                    return;
                }

                _packer.Pack(inputDirPath:  Model.InputDirectory,
                             outputDirPath: Model.OutputDirectory,
                             packageName:   Model.PackedAssemblyName,
                             scripting:     Model.Scripting);

                ShowMessage(Resources.PackagingSuccess);
            }
            catch (Exception x)
            {
                ShowExceptionMessage(x);
            }
            finally
            {
                UseWaitCursor = false;
            }
        }


        private void InputDirectoryBrowseButton_Click(object sender, EventArgs e)
        {
            Folder.Browse(Resources.BrowseForInputFolderTitle,
                          selectedDirectory =>
                          {
                              Model.InputDirectory = selectedDirectory;
                              _bindingSource.ResetCurrentItem();
                          }, 
                          initialDirectory: Model.InputDirectory);
        }


        private void OutputDirectoryBrowseButton_Click(object sender, EventArgs e)
        {
            Folder.Browse(Resources.BrowseForInputFolderTitle,
                          selectedDirectory =>
                          {
                              Model.OutputDirectory = selectedDirectory;
                              _bindingSource.ResetCurrentItem();
                          },
                          initialDirectory: Model.OutputDirectory);
        }


        private void InputDirectoryOpenButton_Click(object sender, EventArgs e)
        {
            if (Model.InputDirectory.Any(Path.GetInvalidPathChars().Contains))
            {
                MessageBox.Show(this, String.Format(Resources.InvalidInputDirectoryName, Model.InputDirectory), "Error");
                return;
            }

            OpenFolder(Model.InputDirectory);
        }


        private void OutputDirectoryOpenButton_Click(object sender, EventArgs e)
        {
            if (Model.OutputDirectory.Any(Path.GetInvalidPathChars().Contains))
            {
                MessageBox.Show(this, String.Format(Resources.InvalidOutputDirectoryName, Model.OutputDirectory), "Error");
                return;
            }

            if (!Path.IsPathRooted(Model.OutputDirectory))
                Model.OutputDirectory = Path.Combine(Environment.CurrentDirectory, Model.OutputDirectory);

            OpenFolder(Model.OutputDirectory);
        }

        // -------------------------------------------------

        private void OpenFolder([NotNull] string dirPath)
        {
            var dir = new DirectoryInfo(dirPath);
            if (!dir.Exists)
            {
                if (DialogResult.Yes != MessageBox.Show(
                          owner: this,
                           text: string.Format(Resources.DirectoryDoesNotExistsMessageFormat, dirPath),
                        caption: "",
                        buttons: MessageBoxButtons.YesNo,
                           icon: MessageBoxIcon.Question))
                {
                    return;
                }
                try
                {
                    dir.Create();
                    dir.Refresh();
                }
                catch (Exception x)
                {
                    MessageBox.Show(this, x.Message, "Error");
                    return;
                }
            }
            dir.Open();
        }


        private bool TryValidateInputDirectory(string inputDirPath, [CanBeNull] out string errorMessage)
        {
            errorMessage = null;
            try
            {
                _packer.ValidateInputDirectory(inputDirPath);
                return true;
            }
            catch (Exception e)
            {
                errorMessage = $"Incorrect input directory path: {e.Message}";
                return false;
            }
        }


        private bool TryValidateOutputDirectory(string outputDirPath, [CanBeNull] out string errorMessage)
        {
            errorMessage = null;

            try
            {
                _packer.ValidateOutputDirectory(outputDirPath);
                return true;
            }
            catch (Exception e)
            {
                errorMessage = $"Incorrect output directory path: {e.Message}";
                return false;
            }
        }


        private bool TryValidateAssemblyName(string inputDirPath, string assemblyName, [CanBeNull] out string errorMessage)
        {
            errorMessage = null;

            try
            {
                _packer.ValidateAssemblyName(inputDirPath, assemblyName);
                return true;
            }
            catch (Exception e)
            {
                errorMessage = $"Incorrect name for target assembly: {e.Message}";
                return false;
            }
        }


        private void SetDefaultPackageName()
        {
            Model.PackedAssemblyName = String.IsNullOrEmpty(Model.PackedAssemblyName) 
                ? _packer.GetDefaultPackageName(Model.InputDirectory) 
                : Model.PackedAssemblyName;
            _bindingSource.ResetCurrentItem();
        }


        private void ShowMessage(string message)
            => MessageBox.Show(this, message, ApplicationInfo.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);    


        private void ShowErrorMessage(string message)
            => MessageBox.Show(this, message, ApplicationInfo.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);    


        private void ShowExceptionMessage([NotNull] Exception x)
            => x.ShowMessageBox(this, message: null, caption: ApplicationInfo.ProductName);


        private readonly IPacker _packer;

        private void MainForm_KeyDown(object sender, [NotNull] KeyEventArgs e)
        {
            // Важно: this.KeyPreview = true 
            switch (e.KeyCode)
            {
                case Keys.F1:
                    new AboutForm(Resources.ApplicationIcon, ProductInfo.Year).ShowDialog();
                    break;
            }
        }
    }
}
