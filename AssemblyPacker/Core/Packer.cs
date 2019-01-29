using System;
using System.IO;
using Twidlle.AssemblyPacker.Interface;
using Twidlle.Infrastructure;

namespace Twidlle.AssemblyPacker.Core
{
    /// <inheritdoc cref="IPacker" />
    public class Packer : MarshalByRefObject, IPacker 
    {
        public void Pack(string inputDirPath, string outputDirPath, string packageName, bool scripting = false)
            => Packing.Pack(inputDirPath, outputDirPath, packageName, scripting);


        public void Unpack(string packedAssemblyPath, string outputDirPath, bool createDirectoryForAssembly = false)
            => Packing.Unpack(packedAssemblyPath, new DirectoryInfo(outputDirPath).CreateIfNotExist(), createDirectoryForAssembly);


        public void ValidateInputDirectory(string dirPath)
            => Packing.ValidateInputDirectory(dirPath);


        public void ValidateOutputDirectory(string dirPath)
            => Packing.ValidateOutputDirectory(dirPath);


        public void ValidateAssemblyName(string inputDirPath, string assemblyName)
            => Packing.ValidateAssemblyName(inputDirPath, assemblyName);


        public string GetDefaultPackageName(string inputDirPath)
            => Packing.GetDefaultPackageName(inputDirPath);
   }
}