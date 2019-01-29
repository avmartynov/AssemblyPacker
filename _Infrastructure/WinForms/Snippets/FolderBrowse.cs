using System;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace Twidlle.Infrastructure.WinForms
{
    public static class Folder
    {
        public static void Browse(string dialogTitle,
                                  Action<string> actionIfDirectorySelected,
                                  string initialDirectory = null)
        {
            if (initialDirectory == null)
                initialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory);

            using (var folderDialog = new CommonOpenFileDialog(dialogTitle))
            {
                folderDialog.InitialDirectory = initialDirectory;
                folderDialog.IsFolderPicker = true;

                folderDialog.AllowNonFileSystemItems = true;
                folderDialog.Multiselect = true;

                if (folderDialog.ShowDialog() == CommonFileDialogResult.Ok)
                    actionIfDirectorySelected(folderDialog.FileName);
            }
        }
    }
}
