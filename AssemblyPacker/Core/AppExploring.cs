using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using Twidlle.Infrastructure.CodeAnnotation;

namespace Twidlle.AssemblyPacker.Core
{
    internal static class AppExploring
    {
        [NotNull]
        public static AppInfo Explore([NotNull] string exeFilePath)
        {
            var file = new FileInfo(exeFilePath);
            if (! file .Exists)
                throw new InvalidOperationException("Startup application file does not exist.");

            Assembly appAsm;

            // AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve += (s, a) => Assembly.ReflectionOnlyLoad(a.Name);
            Assembly resolveEventHandler(object s, ResolveEventArgs a) => Assembly.Load(a.Name);
            try
            {
                AppDomain.CurrentDomain.AssemblyResolve += resolveEventHandler;
                try
                {
                    appAsm = Assembly.LoadFrom(exeFilePath);
                }
                catch (BadImageFormatException x)
                {
                    throw new InvalidOperationException(String.Format("Invalid executable file format: {0}", exeFilePath), x);
                }
            }
            finally
            {
                AppDomain.CurrentDomain.AssemblyResolve -= resolveEventHandler;
            }

            // PortableExecutableKinds pe;
            // ImageFileMachine imf;
            // try
            // {
            //     appAsm.ManifestModule.GetPEKind(out pe, out imf);
            // }
            // catch (Exception x)
            // {
            //     throw new InvalidOperationException(String.Format("Invalid excecutable file format: {0}", exeFilePath), x);
            // }

            if (appAsm.EntryPoint.DeclaringType == null)
                throw new InvalidOperationException(String.Format("Invalid executable file format: {0}", exeFilePath));

            var startupTypeName = appAsm.EntryPoint.DeclaringType.FullName;
            var console = ImageSubsystem(exeFilePath) == IMAGE_SUBSYSTEM_WINDOWS_CUI;

            return new AppInfo
                     {
                         File = file, 
                         AssemblyName       = appAsm.GetName(),
                         MainClassName    = startupTypeName,
                         MainHasParams      = appAsm.EntryPoint.GetParameters().Any(),
                         Console            = console,
                         InstallerTypeNames = FindInstallers(appAsm).ToArray(),
                     };
        }


        private static int ImageSubsystem([NotNull] string assemblyPath)
        {
            using (var s = new FileStream(assemblyPath, FileMode.Open, FileAccess.Read))
            {
                var rawPeSignatureOffset = new byte[4];
                s.Seek(0x3c, SeekOrigin.Begin);
                s.Read(rawPeSignatureOffset, 0, 4);
                int peSignatureOffset = rawPeSignatureOffset[0];
                peSignatureOffset |= rawPeSignatureOffset[1] << 8;
                peSignatureOffset |= rawPeSignatureOffset[2] << 16;
                peSignatureOffset |= rawPeSignatureOffset[3] << 24;
                var header = new byte[24];
                s.Seek(peSignatureOffset, SeekOrigin.Begin);
                s.Read(header, 0, 24);

                var signature = new[] { (byte)'P', (byte)'E', (byte)'\0', (byte)'\0' };

                for (var index = 0; index < 4; index++)
                {
                    if (header[index] != signature[index])
                        throw new InvalidOperationException("Attempted to check a non PE file for the console subsystem!");
                }
                var subsystemBytes = new byte[2];
                s.Seek(68, SeekOrigin.Current);
                s.Read(subsystemBytes, 0, 2);
                var subSystem = subsystemBytes[0] | subsystemBytes[1] << 8;
                return subSystem;
            }
        }


        [NotNull]
        private static IEnumerable<String> FindInstallers([NotNull] Assembly assembly)
        {
            return assembly.GetTypes()
                .Where(t => CustomAttributeData.GetCustomAttributes(t)
                                               .Any(a => a.AttributeType == typeof(RunInstallerAttribute)))
                .Select(t => t.AssemblyQualifiedName);
        }

        private const int IMAGE_SUBSYSTEM_WINDOWS_CUI = 3;
    }
}
