using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using TsudaKageyu;
using Twidlle.Infrastructure;
using Twidlle.Infrastructure.CodeAnnotation;

namespace Twidlle.AssemblyPacker.Core
{
    internal static class Packing
    { 
        /// <param name="inputDirPath"> Input directory path. Path must be absolute (rooted) path. Directory must exists. Directory must contain single dotNet application </param>
        /// <param name="outputDirPath"> Output directory path. </param>
        /// <param name="packageName"> Name of packed assembly must be valid for application default namespace. Must be checked by ValidAssemblyName method. </param>
        /// <param name="scripting"> Flag indicated that packing script will be generated. </param>
        /// 
        public static void Pack([NotNull]   string inputDirPath,
                                [NotNull]   string outputDirPath,
                                [CanBeNull] string packageName = null,
                                bool        scripting = false)
        {
            $"Call with parameters: {new { inputDirPath, outputDirPath, assemblyPackageName = packageName, scripting }.ToJson()}.".Trace(_source);

            var pi = new PackageInfo();

            "Validate input parameters".Trace(_source);
            var inputDir = new DirectoryInfo(inputDirPath).ValidateExist();
            pi.OutputDir = new DirectoryInfo(outputDirPath).CreateIfNotExist();
            ValidateInputDirectory(inputDirPath);

            "Copy all files from input directory to output directory.".Trace(_source);
            inputDir.CopyTo(pi.OutputDir, maxTimeSpan: TimeSpan.FromSeconds(1));

            "Copy compressed and renamed assemblies.".Trace(_source);
            var listOfAssemblyNames = inputDir.EnumerateAssemblies().ToList();
            pi.ResourceFiles = CopyAsResourcesTo(pi.OutputDir, listOfAssemblyNames).ToArray();

            var appAssembly = listOfAssemblyNames.SingleOrDefault(i => Path.GetExtension(i.File.FullName).ToUpper() == ".EXE");
            pi.PackageName = packageName ?? Path.GetFileNameWithoutExtension(appAssembly?.File.Name) ?? "AssemblyPackage";
            ValidateAssemblyName(inputDirPath, pi.PackageName);

            "Delete packed assemblies fom output directory.".Trace(_source);
            DeleteAssemblies(listOfAssemblyNames.Select(i => i.File), inputDir.FullName, pi.OutputDir.FullName);

            if (appAssembly != null)
            {
                $"Explore application '{appAssembly.File.FullName}'.".Trace(_source);
                pi.AppInfo = AppExploring.Explore(appAssembly.File.FullName);
                var appAttributes = GetAssemblyAttributes(appAssembly.File);

                $"Extract application icon from '{appAssembly.File.FullName}'.".Trace(_source);
                var iconExtractor = new IconExtractor(appAssembly.File.FullName);
                pi.AppIcon = iconExtractor.GetAllIcons().FirstOrDefault();

                "Copy config file.".Trace(_source);
                CopyAppConfig(pi.AppInfo.File, Path.Combine(outputDirPath, pi.PackageName+ ".exe"));

                "Format main class cs-code.".Trace(_source);
                pi.ProgramCsCode = FormatProgramCSharpCode(pi.PackageName, pi.AppInfo);

                "Format installer cs-code.".Trace(_source);
                pi.InstallerCsCode = FormatInstallerCSharpCode(pi.PackageName, pi.AppInfo.InstallerTypeNames);

                "Format AssemblyInfo cs-code.".Trace(_source);
                pi.AssemblyInfoCsCode = FormatAssemblyInfoCSharpCode(appAttributes);
            }

            "Format package cs-code.".Trace(_source);
            pi.PackageCsCode = FormatPackageCSharpCode(pi.PackageName);

            if (scripting)
            { 
                "Generate packaging script.".Trace(_source);
                Scripting.GenerateScript(pi);
            }

            "Compile package assembly.".Trace(_source);
            Compiling.Compile(pi);

            "Delete compressed and renamed assemblies.".Trace(_source);
            foreach (var resourceFile in pi.ResourceFiles)
                resourceFile.Delete();
        }


        public static void Unpack([NotNull] string packedAssemblyPath, 
                                  [NotNull] DirectoryInfo outputDir,
                                  bool createDirectoryForAssembly = false)
        {
            packedAssemblyPath = packedAssemblyPath ?? throw new ArgumentNullException(nameof(packedAssemblyPath));
            outputDir = outputDir ?? throw new ArgumentNullException(nameof(outputDir));

            var packedAssembly = Assembly.ReflectionOnlyLoadFrom(packedAssemblyPath);
            var resourceNames = packedAssembly.GetManifestResourceNames()
                                              .Where(n => n.StartsWith(RESOURCE_PREFIX));
            foreach (var resourceName in resourceNames)
            {
                var fullAssemblyName = resourceName.Substring(RESOURCE_PREFIX.Length + 5);
                var extension = resourceName.Substring(RESOURCE_PREFIX.Length, 4);
                var assemblyDirName = createDirectoryForAssembly ? Path.Combine(outputDir.FullName, fullAssemblyName) : outputDir.FullName;
                var assemblyDir = new DirectoryInfo(assemblyDirName).CreateIfNotExist();
            
                var assemblyName = new AssemblyName(fullAssemblyName).Name + extension;
                var assemblyFileName = Path.Combine(assemblyDir.FullName, assemblyName);
                var assemblyFile = new FileInfo(assemblyFileName);
                if (assemblyFile.Exists)
                    assemblyFile.Delete();
            
                var resourceStream = packedAssembly.GetManifestResourceStream(resourceName);
                if (resourceStream == null)
                    throw new InvalidOperationException(string.Format("Can't read resource {0}", resourceName));
            
                var data = new BinaryReader(resourceStream).ReadBytes((int) resourceStream.Length);
                using (var f = new FileStream(assemblyFile.FullName, FileMode.CreateNew))
                using (var g = new GZipStream(new MemoryStream(data), CompressionMode.Decompress))
                {
                    g.CopyTo(f);
                }
            }
        }


        public static void ValidateInputDirectory([NotNull] string dirPath)
        {
            var dir = new DirectoryInfo(dirPath);
            dir.ValidateAbsolute().ValidateExist();

            if (dir.EnumerateAssemblies().Count(a => Path.GetExtension(a.File.FullName).ToUpper() == ".EXE") > 1)
                throw new InvalidOperationException("The directory must contain one application.");
        }


        public static void ValidateOutputDirectory([NotNull] string dirPath)
            => new DirectoryInfo(dirPath).ValidateAbsolute().CreateIfNotExist();


        public static void ValidateAssemblyName([NotNull] string inputDirPath, [NotNull] string assemblyName)
        {
            var inputDir = new DirectoryInfo(inputDirPath);

            if (assemblyName.Any(Path.GetInvalidFileNameChars().Contains))
                throw new InvalidOperationException("Assembly name contains invalid file name character.");

            // http://stackoverflow.com/questions/15264018/how-to-validate-namespace-name-in-codedom            
            if (!assemblyName.Split('.').All(CodeGenerator.IsValidLanguageIndependentIdentifier))
                throw new InvalidOperationException("Invalid assembly name.");

            var namesOfExistingAssemblies = inputDir.EnumerateAssemblyNames();
            if (namesOfExistingAssemblies.Any(n => n.Equals(assemblyName, StringComparison.OrdinalIgnoreCase)))
                throw new InvalidOperationException("Invalid assembly name.");

            if (! Compiling.ValidCustomAssemblyName(assemblyName))
                throw new InvalidOperationException("Invalid assembly name.");
        }


        public static string GetDefaultPackageName([NotNull] string inputDirPath)
            => Path.ChangeExtension(Path.GetFileNameWithoutExtension(
                new DirectoryInfo(inputDirPath)
                    .EnumerateAssemblies()
                    .SingleOrDefault(i => Path.GetExtension(i.File.FullName).ToUpper() == ".EXE")
                    ?.File.Name), "Packed");


        [NotNull] 
        public static Dictionary<string, string> GetAssemblyAttributes([NotNull] FileInfo asmFile)
        {
            Assembly resolveEventHandler(object s, ResolveEventArgs a) => Assembly.ReflectionOnlyLoad(a.Name);
            try
            {
                AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve += resolveEventHandler;
                var asm = Assembly.ReflectionOnlyLoadFrom(asmFile.FullName);
                return new Dictionary<string, string>
                       {
                           ["Title"] = ValueOf<AssemblyTitleAttribute>(),
                           ["Product"] = ValueOf<AssemblyProductAttribute>(),
                           ["Version"] = AssemblyName.GetAssemblyName(asmFile.FullName).Version.ToString(4),
                           ["Configuration"] = ValueOf<AssemblyConfigurationAttribute>(),
                           ["Company"] = ValueOf<AssemblyCompanyAttribute>(),
                           ["Copyright"] = ValueOf<AssemblyCopyrightAttribute>(),
                           ["Trademark"] = ValueOf<AssemblyTrademarkAttribute>(),
                       };

                string ValueOf<T>() 
                    => (string)asm.CustomAttributes.FirstOrDefault(a => a.AttributeType == typeof(T))
                        ?.ConstructorArguments[0].Value;
            }
            finally
            {
                AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve -= resolveEventHandler;
            }
        }
        // -----------------------------------
        
        /// <summary> Адаптирует шаблон класса Package к заданному имени целевой сборки. </summary>
        [NotNull] 
        internal static string FormatPackageCSharpCode(string packedAssemblyName)
        {
            packedAssemblyName = (CSHARP_KEYWORDS.Contains(packedAssemblyName) ? "@" : "") + packedAssemblyName;

            return ReadEmbeddedResourceText(typeof (Packer).Namespace, "EmbeddedResources.Package.cs")
                   .Replace(PACKED_ASSEMBLY_NAME_PLACEHOLDER, packedAssemblyName);
        }


        /// <summary> Адаптирует шаблон класса Program к заданному имени целевой сборки. </summary>
        [NotNull] 
        internal static string FormatProgramCSharpCode(string packedAssemblyName, [NotNull] AppInfo appInfo)
        {
            packedAssemblyName = (CSHARP_KEYWORDS.Contains(packedAssemblyName) ? "@" : "") + packedAssemblyName;

            return ReadEmbeddedResourceText(typeof (Packer).Namespace, "EmbeddedResources.Program.cs")
                .Replace(PACKED_ASSEMBLY_NAME_PLACEHOLDER,  packedAssemblyName)
                .Replace("AppAssemblyFullName", appInfo.AssemblyName.FullName)
                .Replace("StartupTypeName",     appInfo.MainClassName)
                .CallIf(!appInfo.MainHasParams, i => i.Replace("new object[]{args}", "null"))
                .CallIf(appInfo.Console,        i => i.Replace("[STAThread]", ""));
        }


        /// <summary> Адаптирует шаблон класса Installer к заданному имени целевой сборки. </summary>
        [NotNull] 
        internal static string FormatInstallerCSharpCode(string packedAssemblyName,
            [NotNull] string[] installerTypeNames)
        {
            if (!installerTypeNames.Any())
                return "";

            packedAssemblyName = (CSHARP_KEYWORDS.Contains(packedAssemblyName) ? "@" : "") + packedAssemblyName;

            return ReadEmbeddedResourceText(typeof (Packer).Namespace, "EmbeddedResources.Installer.cs")
                .Replace(PACKED_ASSEMBLY_NAME_PLACEHOLDER, packedAssemblyName)
                .Replace(INSTALLER_TYPE_NAMES_PLACEHOLDER, string.Join(";", installerTypeNames));
        }

        /// <summary>  </summary>
        internal static string FormatAssemblyInfoCSharpCode([NotNull] Dictionary<string, string> attributeValues)
        {
            var code = ReadEmbeddedResourceText(typeof(Packer).Namespace, "EmbeddedResources.AssemblyInfo.cs");
            attributeValues.Keys.Where(k => !String.IsNullOrEmpty(attributeValues[k])).ToList()
                                .ForEach(i => code = code.Replace("\"" + i + "\"", "\"" + attributeValues[i] + "\""));
            return code;
        }

        /// <summary> Копирует все сборки в выходной каталог в сжатом виде под специальными именами.  </summary>
        [NotNull] 
        internal static IEnumerable<FileInfo> CopyAsResourcesTo(DirectoryInfo outputDir, [NotNull] IEnumerable<AssemblyFileInfo> asmFiles)
            => asmFiles.Select(i => CopyAsResource(outputDir, i))
                       .GroupBy(i => i.FullName)
                       .Select(i => i.First());


        /// <summary> Копирует сборку в выходной каталог со сжатием, под специальным именем.  </summary>
        [NotNull]
        internal static FileInfo CopyAsResource([NotNull] DirectoryInfo outputDir, [NotNull] AssemblyFileInfo assembly)
        {
            var resourceName = RESOURCE_PREFIX + Path.GetExtension(assembly.File.Name) + "." +  assembly.AssemblyName;
            return assembly.File.CopyTo(Path.Combine(outputDir.FullName, resourceName), overwrite: true, compress: true);
        }


        internal static void CopyAppConfig([NotNull] FileInfo inputAppFile, string packedAppFilePath)
        {
            var inputConfig = RelocatedPath(inputAppFile.FullName + ".config",
                                            Path.GetDirectoryName(inputAppFile.FullName) ?? "",
                                            Path.GetDirectoryName(packedAppFilePath) ?? "");
            var packedAppConfig = packedAppFilePath + ".config";
            if (File.Exists(inputConfig))
                File.Move(inputConfig, packedAppConfig);
        }


        /// <summary> Удаляет оригиналы упакованных сборок и сопутствующие им файлы </summary>
        internal static void DeleteAssemblies([NotNull] IEnumerable<FileInfo> assembliesFiles, string srcDirPath, string tgtDirPath)
            => assembliesFiles.Select(asm => RelocatedPath(asm.FullName, srcDirPath, tgtDirPath))                   
                              .ToList().ForEach(DeleteAssemblyFile);

        // ------------------------------

        /// <summary> Удаляет сборку и сопутствующие ей файлы (pdb, xml) </summary>
        private static void DeleteAssemblyFile([NotNull] string assemblyFilePath)
        {
            File.Delete(assemblyFilePath);
            DeleteFile(assemblyFilePath, ".pdb");
            DeleteFile(assemblyFilePath, ".xml");
        }


        private static void DeleteFile([NotNull] string fileMaster, string extension)
        {
            var slaveFilePath = Path.ChangeExtension(fileMaster, extension);

            if (File.Exists(slaveFilePath))
                File.Delete(slaveFilePath);
        }


        /// <summary> Находит положение файла после перемещения одного из его родительских каталогов в другой каталог. </summary>
        [NotNull] 
        private static string RelocatedPath([NotNull] string filePath, [NotNull] string parentDirPath, [NotNull] string targetRootDirPath)
            => Path.Combine(targetRootDirPath, filePath.Substring(parentDirPath.Length + 1));

 
        [NotNull] 
        private static string ReadEmbeddedResourceText(string defaultNamespace, string resourceName)
        {
            resourceName = defaultNamespace + "." + resourceName;
            var asm = Assembly.GetCallingAssembly();
            var stream = asm.GetManifestResourceStream(resourceName);
            if (stream == null)
            {
                throw new InvalidOperationException(String.Format(
                    "Can't find '{0}' embedded resource.", resourceName));
            }

            using (var s = new StreamReader(stream))
                return s.ReadToEnd();
        }

        // -----------------------------------------------------

        public const  string RESOURCE_PREFIX                  = "PackedAssembly";
        private const string PACKED_ASSEMBLY_NAME_PLACEHOLDER = "PackedAssemblyName";
        private const string INSTALLER_TYPE_NAMES_PLACEHOLDER = "InstallerTypeNames";

        private static readonly string[] CSHARP_KEYWORDS = new[] 
        {
            "abstract", "as", "base", "bool", "break", "byte", "case", "catch", "char", "checked",
            "class", "const", "continue", "decimal", "default", "delegate", "do", "double", "else",
            "enum", "event", "explicit", "extern", "false", "finally", "fixed", "float", "for", "foreach "
            , "goto", "if", "implicit", "in", "int", "interface", "internal", "is", "lock", "long",
            "namespace", "new", "null", "object", "operator", "out", "override", "params", "private",
            "protected", "public", "readonly", "ref", "return", "sbyte", "sealed", "short", "sizeof",
            "stackalloc", "static", "string", "struct", "switch", "this", "throw", "true", "try",
            "typeof", "uint", "ulong", "unchecked", "unsafe", "ushort", "using", "virtual", "void",
            "volatile", "while", "add", "alias", "ascending", "async", "await", "descending", "dynamic",
            "from", "get", "global", "group", "into", "join", "let", "orderby", "partial", "remove",
            "select", "set", "value", "var", "where", "yield"
        };

        private static readonly TraceSource _source = MethodBase.GetCurrentMethod().GetTraceSource();
    }
}
