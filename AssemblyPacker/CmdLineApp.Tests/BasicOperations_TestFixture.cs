using System.IO;
using System.Linq;
using NLog;
using NUnit.Framework;
using Twidlle.Infrastructure.Diagnostics;
using Twidlle.Infrastructure.Testing;

using static Twidlle.AssemblyPacker.Core.Tests.TestStuff;

namespace Twidlle.AssemblyPacker.CmdLineApp.Tests
{
    public class BasicOperations_TestFixture
    {
        [Test]
        public void Pack_Dll_Test() => _facade.Execute(() =>
        {
            "Упаковываем dll".Trace();

            var args = $"pack --inputDirectory:{DLL_DIR_PATH} --outputDirectory:{OUTPUT_1_DIR_PATH} --assemblyName:Abcd";

            ConsoleAppControl.Execute(PACKER_CMD_EXE_PATH, args);

            var tgtFiles = Directory.EnumerateFiles(OUTPUT_1_DIR_PATH, "*.*", SearchOption.AllDirectories);
            Assert.AreEqual(1, tgtFiles.Count()); 
        });


        [Test]
        public void Pack_Dll_With_Scripting_Test() => _facade.Execute(() =>
        {
            "Упаковываем dll c сохранением сценария упаковки.".Trace();

            var args = $"pack --inputDirectory:{DLL_DIR_PATH} --outputDirectory:{OUTPUT_1_DIR_PATH} --assemblyName:Abcd --scripting";

            ConsoleAppControl.Execute(PACKER_CMD_EXE_PATH, args);

            var tgtFiles = Directory.EnumerateFiles(OUTPUT_1_DIR_PATH, "*.*", SearchOption.AllDirectories);
            Assert.AreEqual(6, tgtFiles.Count()); // dll и в папке PackScript 5 файлов
        });


        [Test]
        public void Pack_WinFormsApp_Test()  => _facade.Execute(() =>
        {
            "Упаковываем WinForms приложение.".Trace();

            var args = $"pack --inputDirectory:{WIN_FORMS_APP_DIR_PATH} --outputDirectory:{OUTPUT_1_DIR_PATH} --assemblyName:Abcd";

            ConsoleAppControl.Execute(PACKER_CMD_EXE_PATH, args);

            "Проверяем общее число файлов в выходном каталоге.".Trace();
            var tgtFiles = Directory.EnumerateFiles(OUTPUT_1_DIR_PATH, "*.*", SearchOption.AllDirectories);
            Assert.AreEqual(2, tgtFiles.Count()); // exe и config

            "Проверяем работоспособность сборки-пакета.".Trace();
            WinAppControl.StartWaitFinish(5, OUTPUT_1_DIR_PATH, "Abcd.exe");
        });


        [Test]
        public void Pack_WinFormsApp_With_Scripting_Test()  => _facade.Execute(() =>
        {
            "Упаковываем WinForms приложение c сохранением сценария упаковки.".Trace();

            var args = $"pack --inputDirectory:{WIN_FORMS_APP_DIR_PATH} --outputDirectory:{OUTPUT_1_DIR_PATH} --assemblyName:Abcd --scripting";

            ConsoleAppControl.Execute(PACKER_CMD_EXE_PATH, args);

            "Проверяем общее число файлов в выходном каталоге.".Trace();
            var tgtFiles = Directory.EnumerateFiles(OUTPUT_1_DIR_PATH, "*.*", SearchOption.AllDirectories);
            Assert.AreEqual(10, tgtFiles.Count()); // exe и config +  8 файлов скрипта

            "Проверяем работоспособность сборки-пакета.".Trace();
            WinAppControl.StartWaitFinish(5, OUTPUT_1_DIR_PATH, "Abcd.exe");
        });


        [SetUp]
        public void SetUp()
        {
            "Создать выходной каталог перед выполнением теста.".Trace();
            Directory.CreateDirectory(OUTPUT_1_DIR_PATH);
        }


        [TearDown]
        public void TearDown() 
        {
            "Удалить выходной каталог по окончании теста.".Trace();
            Directory.Delete(OUTPUT_1_DIR_PATH, recursive: true);
        }

        private static readonly TestFacade _facade = TestFacade.GetCurrentClassFacade();

        private const string PACKER_CMD_EXE_PATH = @"..\..\..\CmdLineApp\bin\DebugPacked\AssemblyPacker.Cmd.exe";
    }


    internal static class LocalExtensions
    {
        public static void Trace(this string message) 
            => _logger.Trace(message);

        private static readonly ILogger _logger = LogManager.GetLogger(typeof(BasicOperations_TestFixture).FullName);
    }
}
