using System;
using System.IO;
using System.Linq;
using Twidlle.Infrastructure;
using Twidlle.Infrastructure.CodeAnnotation;

namespace Twidlle.AssemblyPacker.Core
{
    internal static class Scripting
    {
        public static void GenerateScript([NotNull] PackageInfo packageInfo)
        {
            var scriptDirPath = Path.Combine(packageInfo.OutputDir.FullName, PACK_SCRIPT_DIRECTORY);
            Directory.CreateDirectory(scriptDirPath);

            foreach (var resourceFile in packageInfo.ResourceFiles)
                resourceFile.CopyToDir(scriptDirPath);

            var iconFilePath = "";
            if (packageInfo.AppIcon != null)
            {
                iconFilePath = Path.ChangeExtension(Path.Combine(scriptDirPath, packageInfo.PackageName), "ico");
                using (var s = new FileStream(iconFilePath, FileMode.CreateNew))
                    packageInfo.AppIcon.Save(s);
            }

            var scriptParam = "";
            if (packageInfo.AppInfo == null)
            {
                scriptParam += $"{Environment.NewLine}\t/out:{packageInfo.PackageName}.dll ^";
                scriptParam += $"{Environment.NewLine}\t/target:library ^";
            }
            else
            {
                var packedAppConfig = Path.Combine(packageInfo.OutputDir.FullName, packageInfo.PackageName + ".exe") + ".config";
                if (File.Exists(packedAppConfig))
                {
                    var appConfigFileName = Path.GetFileName(packedAppConfig);
                    File.Copy(packedAppConfig, Path.Combine(scriptDirPath, appConfigFileName), overwrite: true);
                }
                var target = packageInfo.AppInfo.Console ? "exe" : "winexe";

                scriptParam += $"{Environment.NewLine}\t/out:{packageInfo.PackageName}.exe ^";
                scriptParam += $"{Environment.NewLine}\t/target:{target} ^";
            }

            if (packageInfo.AppIcon != null)
                scriptParam += $"{Environment.NewLine}\t/win32icon:{Path.GetFileName(iconFilePath)} ^";

            scriptParam += packageInfo.ResourceFiles.Aggregate("", (s, file) => s + FormatResourceOption(file));

            WriteSourceCodeFile("Package.cs", packageInfo.PackageCsCode);
            WriteSourceCodeFile("Program.cs", packageInfo.ProgramCsCode);
            WriteSourceCodeFile("Installer.cs", packageInfo.InstallerCsCode);
            WriteSourceCodeFile("AssemblyInfo.cs", packageInfo.AssemblyInfoCsCode);

            WriteScriptFile("Build.bat", BUILD_SCRIPT_HEADER + scriptParam);

            void WriteScriptFile(string fileName, string content)
                => File.WriteAllText(Path.Combine(scriptDirPath, fileName), content);

            void WriteSourceCodeFile(string fileName, string content)
            {
                if (String.IsNullOrEmpty(content))
                    return;

                WriteScriptFile(fileName, content);

                scriptParam += $"{Environment.NewLine}\t{fileName} ^";
            }
        }

        [NotNull] 
        private static string FormatResourceOption([NotNull] FileSystemInfo resourceFile)
            => $"{Environment.NewLine}\t/resource:\"{resourceFile.Name}\" ^";

        private const string PACK_SCRIPT_DIRECTORY = "PackScript";

        private const string BUILD_SCRIPT_HEADER = @"
call ""%VS140COMNTOOLS%..\..\VC\vcvarsall.bat"" x86
pushd ""%~dp0""

csc.exe ^";
    }
}
