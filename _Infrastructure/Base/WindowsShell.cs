using System;
using System.Diagnostics;
using System.IO;
using Twidlle.Infrastructure.CodeAnnotation;

namespace Twidlle.Infrastructure
{
    /// <summary>
    /// Методы-расширения для работы с пользовательской оболочкой Windows.
    /// </summary>
    public static class WindowsShellExtensions
    { 
        /// <summary> Открывает папку в Windows Explorer. </summary>        
        public static void Open([NotNull] this DirectoryInfo directory)
        {
            if (directory == null)
                throw new ArgumentNullException(nameof(directory));

            if (!directory.Exists)
            {
                throw new InvalidOperationException($"Directory '{directory.FullName}' does not exist.");
            }

            ShellOpen(directory.FullName);
        }


        /// <summary> Открывает файл в программе, настроенной в Windows для просмотра файлов этого типа. </summary>        
        public static void Open([NotNull] this FileInfo file)
        {
            if (file == null)
                throw new ArgumentNullException(nameof(file));

            if (!file.Exists)
            {
                throw new InvalidOperationException($"File '{file.FullName}' does not exist.");
            }

            ShellOpen(file.FullName);
        }


        /// <summary> Открывает url-адрес в броузере по умолчанию. </summary>        
        public static void NavigateTo([NotNull] this Uri url)
        {
            if (url == null)
                throw new ArgumentNullException(nameof(url));

            ShellOpen(url.ToString());
        }

        #region Private members

        private static void ShellOpen([NotNull] string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentNullException(nameof(fileName));

            Process.Start(new ProcessStartInfo(fileName) { UseShellExecute = true });
        }

        #endregion Private members
    }
}
