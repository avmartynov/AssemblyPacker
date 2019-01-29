using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Threading;
using Twidlle.Infrastructure.CodeAnnotation;

namespace Twidlle.Infrastructure
{
    public static class IOExtensions
    {
        public static void CopyTo([NotNull] this DirectoryInfo inputDir, [NotNull] DirectoryInfo outputDir, TimeSpan maxTimeSpan)
        {
            if (outputDir.Exists)
            {
                outputDir.ForceDelete(maxTimeSpan, recursive: true);
                outputDir.Refresh();
            }

            if (!outputDir.Exists)
            {
                outputDir.Create();
                outputDir.Refresh();
            }

            foreach (var dir in inputDir.GetDirectories("*.*", SearchOption.TopDirectoryOnly))
                dir.CopyTo(new DirectoryInfo(Path.Combine(outputDir.FullName, dir.Name)), maxTimeSpan);

            foreach (var sourceFile in inputDir.GetFiles("*.*", SearchOption.TopDirectoryOnly))
                sourceFile.CopyTo(Path.Combine(outputDir.FullName, sourceFile.Name), overwrite: true);
        }


        [NotNull] 
        public static FileInfo CopyTo([NotNull] this FileInfo sourceFile, [NotNull] string destFilePath, bool overwrite, bool compress)
        {
            if (!compress)
                return sourceFile.CopyTo(destFilePath, overwrite);

            if (!overwrite && File.Exists(destFilePath))
                throw new IOException("File " + destFilePath + " already exists.");

            using (var sourceStream = sourceFile.OpenRead())
            using (var gzipStream = new GZipStream(File.Create(destFilePath), CompressionMode.Compress))
                sourceStream.CopyTo(gzipStream);

            return new FileInfo(destFilePath);
        }


        public static void ForceDelete([NotNull] this DirectoryInfo dir, TimeSpan maxTimeSpan, bool recursive)
        {
            var sleepTime = TimeSpan.FromMilliseconds(maxTimeSpan.TotalMilliseconds / 10);
            for (var count = 0; dir.Exists; count++)
            {
                try
                {
                    dir.Delete(recursive);
                }
                catch (Exception)
                {
                    if (count > 10)
                        throw new IOException(string.Format("Can't delete directory {0}.", dir));

                    Thread.Sleep(sleepTime);
                }

                dir.Refresh();
            }
        }


        [NotNull] 
        public static DirectoryInfo ValidateAbsolute([NotNull] this DirectoryInfo dir)
        {
            dir = dir ?? throw new ArgumentNullException(nameof(dir));
            return ! String.IsNullOrEmpty(Path.GetPathRoot(dir.FullName)) ? dir 
                : throw new ArgumentException($"The directory path '{dir}' must be an absolute.", nameof(dir));
        }


        [NotNull] 
        public static DirectoryInfo ValidateExist([NotNull] this DirectoryInfo dir)
        {
            dir = dir ?? throw new ArgumentNullException(nameof(dir));
            return dir.Exists ? dir : throw new ArgumentException($"Directory '{dir}' must exist.", nameof(dir));
        }


        [NotNull] 
        public static DirectoryInfo CreateIfNotExist([NotNull] this DirectoryInfo dir)
        {
            dir = dir ?? throw new ArgumentNullException(nameof(dir));
            if (!dir.Exists)
            {
                dir.Create();
                dir.Refresh();
            }
            return dir;
        }


        public static void CopyToDir([NotNull] this FileInfo file, [NotNull] string dirPath)
            => file.CopyTo(Path.Combine(dirPath, file.Name));


        public static void CopyToDir([NotNull] this FileInfo file, [NotNull] string dirPath, bool overwrite)
            => file.CopyTo(Path.Combine(dirPath, file.Name), overwrite);


        [NotNull] 
        public static IEnumerable<AssemblyFileInfo> EnumerateAssemblies([NotNull] this DirectoryInfo dir)
        {
            return dir.EnumerateFiles("*.*", SearchOption.AllDirectories)
                .Where(f => f.Extension.ToUpper() == ".DLL" 
                         || f.Extension.ToUpper() == ".EXE")
                .Select(file => new AssemblyFileInfo { File = file, AssemblyName = GetAssemblyNameOrDefault(file.FullName)})
                .Where(a => a.AssemblyName != null); // Only successfully loaded
        }


        [NotNull] 
        public static IEnumerable<string> EnumerateAssemblyNames([NotNull] this DirectoryInfo dir)
            =>  EnumerateAssemblies(dir).Select(pair => pair.AssemblyName.Name);


        private static AssemblyName GetAssemblyNameOrDefault(String filePath)
        {
            try
            {
                return AssemblyName.GetAssemblyName(filePath);
            }
            catch (FileLoadException)
            {
                return null;
            }
            catch (BadImageFormatException)
            {
                return null;
            }
        }
    }

    public class AssemblyFileInfo
    {
        public FileInfo File { get; set; }
        public AssemblyName AssemblyName { get; set; }
    }
}
