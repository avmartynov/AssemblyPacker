using System.IO;
using System.Linq;
using System.Reflection;
using Twidlle.Infrastructure.CodeAnnotation;

namespace Twidlle.Infrastructure.Testing
{
    public static class TestEnvironment
    {
        /// <summary> Вычисляет абсолютный путь к каталогу теста (каталогу, где расположен exe-файл теста).</summary>
        [NotNull]
        public static string GetTestDirectory()
            => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? "";

        /// <summary> Вычисляет абсолютный путь к файлу по относительному </summary>
        /// <param name="path"> Относительный путь файла должен быть задан по отношению к каталогу, где расположен exe-файл теста. </param>
        /// <returns></returns>
        [NotNull]
        public static string GetFilePath([NotNull] string path)
            => Path.Combine(GetTestDirectory(), path).PathCanonicalize();

        [NotNull]
        public static string GetFilePath([NotNull] string path1, [NotNull] string path2)
            => Path.Combine(GetTestDirectory(), path1, path2).PathCanonicalize();

        [NotNull]
        public static string GetFilePath([NotNull] string path1, [NotNull] string path2, [NotNull] string path3)
            => Path.Combine(GetTestDirectory(), path1, path2, path3).PathCanonicalize();

        [NotNull]
        public static string GetFilePath([NotNull] string path1, [NotNull] string path2, [NotNull] string path3, [NotNull] string path4)
            => Path.Combine(GetTestDirectory(), path1, path2, path3, path4).PathCanonicalize();

        [NotNull]
        public static string GetFilePath([NotNull] params string[] paths)
            => Path.Combine(new [] {GetTestDirectory()}.Concat(paths).ToArray()).PathCanonicalize();
    }
}
