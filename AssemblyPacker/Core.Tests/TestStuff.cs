using System.IO;

using Twidlle.Infrastructure.Testing;

namespace Twidlle.AssemblyPacker.Core.Tests
{
    internal static class TestStuff
    {
        public const string OUTPUT_1_DIR_NAME    = "Output1";
        public const string OUTPUT_2_DIR_NAME    = "Output2";
        public const string TEST_STUFF_DIR_NAME  = @"..\..\..\TestStuff";
        public const string APP_BIN_DIR          = @"bin\Debug";

        public static readonly string OUTPUT_1_DIR_PATH   = Path.Combine(TestEnvironment.GetTestDirectory(), OUTPUT_1_DIR_NAME);
        public static readonly string OUTPUT_2_DIR_PATH   = Path.Combine(TestEnvironment.GetTestDirectory(), OUTPUT_2_DIR_NAME);
        public static readonly string TEST_STUFF_DIR_PATH = Path.Combine(TestEnvironment.GetTestDirectory(), TEST_STUFF_DIR_NAME);

        // Dll
        public const           string DLL_NAME      = "ClassLibrary3";
        public static readonly string DLL_FILE_NAME = Path.ChangeExtension(DLL_NAME, "dll");
        public static readonly string DLL_DIR_PATH  = Path.Combine(TEST_STUFF_DIR_PATH,  DLL_NAME, APP_BIN_DIR);
        public static readonly string DLL_FILE_PATH = Path.Combine(DLL_DIR_PATH, DLL_FILE_NAME);

        // Конcольное приложение
        public const           string CONSOLE_APP_NAME      = "ConsoleApplication1";
        public static readonly string CONSOLE_APP_FILE_NAME = Path.ChangeExtension(CONSOLE_APP_NAME, "exe");
        public static readonly string CONSOLE_APP_DIR_PATH  = Path.Combine(TEST_STUFF_DIR_PATH,  CONSOLE_APP_NAME, APP_BIN_DIR);
        public static readonly string CONSOLE_APP_FILE_PATH = Path.Combine(CONSOLE_APP_DIR_PATH, CONSOLE_APP_FILE_NAME);

        // WinForms приложение
        public const           string WIN_FORMS_APP_NAME      = "WindowsFormsApplication1";
        public static readonly string WIN_FORMS_APP_FILE_NAME = Path.ChangeExtension(WIN_FORMS_APP_NAME, "exe");
        public static readonly string WIN_FORMS_APP_DIR_PATH  = Path.Combine(TEST_STUFF_DIR_PATH, WIN_FORMS_APP_NAME, APP_BIN_DIR);
        public static readonly string WIN_FORMS_APP_FILE_PATH = Path.Combine(WIN_FORMS_APP_DIR_PATH, WIN_FORMS_APP_FILE_NAME);

        // Windows Service
        public const           string WIN_SVR_NAME      = "WindowsService0";
        public static readonly string WIN_SVR_FILE_NAME = Path.ChangeExtension(WIN_SVR_NAME, "exe");
        public static readonly string WIN_SVR_DIR_PATH  = Path.Combine(TEST_STUFF_DIR_PATH, WIN_SVR_NAME, APP_BIN_DIR);
        public static readonly string WIN_SVR_FILE_PATH = Path.Combine(WIN_SVR_DIR_PATH, WIN_SVR_FILE_NAME);
    }
}
