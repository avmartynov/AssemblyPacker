using Twidlle.Infrastructure.CodeAnnotation;

namespace Twidlle.AssemblyPacker.Interface
{
    public interface IPacker
    {
        void Pack([NotNull] string inputDirPath, [NotNull] string outputDirPath, string packageName, bool scripting = false);

        void Unpack([NotNull] string packedAssemblyPath, [NotNull] string outputDirPath, bool createDirectoryForAssembly = false);

        string GetDefaultPackageName([NotNull] string inputDirPath);

        void ValidateAssemblyName([NotNull] string inputDirPath, [NotNull] string assemblyName);

        void ValidateInputDirectory([NotNull] string dirPath);

        void ValidateOutputDirectory([NotNull] string dirPath);
    }
}
