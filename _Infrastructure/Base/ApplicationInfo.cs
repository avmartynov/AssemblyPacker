using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using Twidlle.Infrastructure.CodeAnnotation;

namespace Twidlle.Infrastructure
{
    public static class ApplicationInfo
    {
        /// <summary> The fully qualified path that defines the location of the main module of current process. </summary>
        [NotNull]
        public static string FileName
            => Process.GetCurrentProcess().MainModule.FileName;

        [NotNull]
        public static string DirectoryName
            => Path.GetDirectoryName(FileName) ?? Environment.CurrentDirectory;

        /// <summary> Имя продукта </summary>
        [NotNull]
        public static string ProductName
            => Process.GetCurrentProcess().MainModule.FileVersionInfo.ProductName;

        /// <summary> Вeрсия продукта </summary>
        [NotNull]
        public static string Version
            => Process.GetCurrentProcess().MainModule.FileVersionInfo.FileVersion;

        /// <summary> Права на продукт </summary>
        [NotNull]
        public static string Copyright
            => Process.GetCurrentProcess().MainModule.FileVersionInfo.LegalCopyright;

        /// <summary> Имя компании-производителя продукта </summary>
        [NotNull]
        public static string CompanyName
            => Process.GetCurrentProcess().MainModule.FileVersionInfo.CompanyName;

        /// <summary> Путь к конфигурационному файлу приложения </summary>
        [NotNull]
        public static string ConfigFileName
            => ConfigurationManager.OpenExeConfiguration(FileName).FilePath;

        /// <summary> Путь к пользовательскому конфигурационному файлу приложения </summary>
        [NotNull]
        public static string UserConfigFileName
            => ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal).FilePath;
    }
}
