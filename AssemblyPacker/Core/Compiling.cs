using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.CSharp;
using Twidlle.Infrastructure;
using Twidlle.Infrastructure.CodeAnnotation;

namespace Twidlle.AssemblyPacker.Core
{
    internal static class Compiling
    {
        public static void Compile([NotNull] PackageInfo packageInfo)
        {
            var options = new Dictionary<string, string>{{"CompilerVersion", "v4.0"}};
            var cs = new CSharpCodeProvider(options);

            var mainClass = packageInfo.AppInfo == null ? null : packageInfo.PackageName + ".Program";
            var library = String.IsNullOrEmpty(mainClass);

            var packedAssemblyFileName = packageInfo.PackageName + (library ? ".dll" : ".exe");
            var outputAssembly = Path.Combine(packageInfo.OutputDir.FullName, packedAssemblyFileName);

            var compilerOptions = "/target:" + (library 
                                      ? "library" 
                                      : packageInfo.AppInfo != null && packageInfo.AppInfo.Console ? "exe" : "winexe");

            var iconFilePath = "";
            if (packageInfo.AppIcon != null)
            {
                iconFilePath = Path.ChangeExtension(Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString()), "ico");
                using (var s = new FileStream(iconFilePath, FileMode.CreateNew))
                    packageInfo.AppIcon.Save(s);

                compilerOptions += $" /win32icon:{iconFilePath}";
            }

            var compileParams = new CompilerParameters
                                    {
                                        CompilerOptions = compilerOptions,
                                        TempFiles = new TempFileCollection("."),
                                        GenerateExecutable = !library,
                                        OutputAssembly = outputAssembly,
                                        MainClass = library ? null : mainClass
                                    };

            compileParams.ReferencedAssemblies.AddRange(SYSTEM_ASSEMBLIES);
            compileParams.EmbeddedResources.AddRange(packageInfo.ResourceFiles.Select(r => r.FullName).ToArray());

            var compileResult = cs.CompileAssemblyFromSource(compileParams, 
                                                             packageInfo.PackageCsCode,
                                                             packageInfo.ProgramCsCode,
                                                             packageInfo.InstallerCsCode,
                                                             packageInfo.AssemblyInfoCsCode);
            var errors = compileResult.Errors
                                      .Cast<CompilerError>()
                                      .Select(i => i.ErrorText)
                                      .Join(Environment.NewLine);

            if (!String.IsNullOrEmpty(iconFilePath))
                File.Delete(iconFilePath);

            if (!String.IsNullOrEmpty(errors))
                throw new InvalidOperationException(errors);
        }


        public static bool ValidCustomAssemblyName(string assemblyName)
            => ! SYSTEM_ASSEMBLIES.Select(Path.GetFileNameWithoutExtension)
                                  .Any(n => n.Equals(assemblyName, StringComparison.OrdinalIgnoreCase));


        private static readonly string[] SYSTEM_ASSEMBLIES = 
            {
                "System.dll",
                "System.Core.dll",
                "System.Configuration.Install.dll"
            };
    }
}
