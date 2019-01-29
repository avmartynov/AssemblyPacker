using System.Diagnostics;
using System.IO;
using System.Linq;
using NLog;
using NUnit.Framework;
using Twidlle.Infrastructure;
using Twidlle.Infrastructure.Diagnostics;
using Twidlle.Infrastructure.Testing;

using static Twidlle.AssemblyPacker.Core.Tests.TestStuff;

namespace Twidlle.AssemblyPacker.Core.Tests
{
    public class BasicOperations_TestFixture
    {
        [Test]
        public void _01_Pack_Unpack_Dll_Test() => _facade.Execute(() =>
        {
            "Упаковываем dll".Trace(_logger);

            const string ASM_PACKED_NAME = "Abcd";
            using (var domain = new AppDomainIsolated<Packer>())
            {
                domain.Proxy.Pack(inputDirPath:  DLL_DIR_PATH,
                                  outputDirPath: OUTPUT_1_DIR_PATH,
                                  packageName:   ASM_PACKED_NAME,
                                  scripting:     false);
            }

            var tgtFiles1 = Directory.EnumerateFiles(OUTPUT_1_DIR_PATH, "*.*", SearchOption.AllDirectories);
            Assert.AreEqual(1, tgtFiles1.Count());

            "Распаковываем упакованную на предыдущем шаге dll.".Trace(_logger);

            var asmPackedPath = Path.ChangeExtension(Path.Combine(OUTPUT_1_DIR_PATH, ASM_PACKED_NAME), "dll");
            using (var domain = new AppDomainIsolated<Packer>())
                domain.Proxy.Unpack(asmPackedPath, OUTPUT_2_DIR_PATH);
             
            var tgtFiles2 = Directory.EnumerateFiles(OUTPUT_2_DIR_PATH, "*.*", SearchOption.AllDirectories);
            Assert.AreEqual(3, tgtFiles2.Count());
        });


        [Test]
        public void _02_Pack_Dll_With_Scripting_Test() => _facade.Execute(() =>
        {
            "Упаковываем dll c сохранением сценария упаковки.".Trace(_logger);

            using (var domain = new AppDomainIsolated<Packer>())
            { 
                domain.Proxy.Pack(inputDirPath: DLL_DIR_PATH,
                                  outputDirPath: OUTPUT_1_DIR_PATH,
                                  packageName: "Abcd",
                                  scripting: true);
            }

            var tgtFiles = Directory.EnumerateFiles(OUTPUT_1_DIR_PATH, "*.*", SearchOption.AllDirectories);
            Assert.AreEqual(6, tgtFiles.Count()); // dll и в папке PackScript 5 файлов
        });


        [Test]
        public void _03_Pack_ConsoleApp_Test() => _facade.Execute(() =>
        {
            "Упаковываем консольное приложение.".Trace(_logger);

            using (var domain = new AppDomainIsolated<Packer>())
            {
                domain.Proxy.Pack(inputDirPath: CONSOLE_APP_DIR_PATH,
                    outputDirPath: OUTPUT_1_DIR_PATH,
                    packageName: "Abcd",
                    scripting: false);
            }

            var tgtFiles = Directory.EnumerateFiles(OUTPUT_1_DIR_PATH, "*.*", SearchOption.AllDirectories);
            Assert.AreEqual(2, tgtFiles.Count()); // exe и config

            ConsoleAppControl.StartWaitFinish(OUTPUT_1_DIR_PATH, "Abcd.exe");
        });


        [Test]
        public void _04_Pack_ConsoleApp_With_Scripting_Test() => _facade.Execute(() =>
        {
            "Упаковываем консольное приложение как dll.".Trace(_logger);

            using (var domain = new AppDomainIsolated<Packer>())
            {
                domain.Proxy.Pack(inputDirPath: CONSOLE_APP_DIR_PATH,
                    outputDirPath: OUTPUT_1_DIR_PATH,
                    packageName: "Abcd",
                    scripting: true);
            }

            var tgtFiles = Directory.EnumerateFiles(OUTPUT_1_DIR_PATH, "*.*", SearchOption.AllDirectories);
            Assert.AreEqual(10, tgtFiles.Count()); // dll и config, и в папке PackScript 7 файлов

            using (var p = Process.Start(Path.Combine(OUTPUT_1_DIR_PATH, @"PackScript\Build.bat")))
            {
                if (p == null)
                    Assert.Fail();

                p.WaitForExit();
            }

            ConsoleAppControl.StartWaitFinish(OUTPUT_1_DIR_PATH, "PackScript", "Abcd.exe");
        });


        [Test]
        public void _06_Pack_WinSvr_Test() => _facade.Execute(() =>
        {
            "Упаковываем windows-сервис.".Trace(_logger);

            using (var domain = new AppDomainIsolated<Packer>())
            {
                domain.Proxy.Pack(inputDirPath: WIN_SVR_DIR_PATH,
                    outputDirPath: OUTPUT_1_DIR_PATH,
                    packageName: "Abcd",
                    scripting: false);
            }

            "Проверяем общее число файлов в выходном каталоге.".Trace(_logger);
            var tgtFiles = Directory.EnumerateFiles(OUTPUT_1_DIR_PATH, "*.*", SearchOption.AllDirectories);
            Assert.AreEqual(2, tgtFiles.Count()); // exe и config

            "Проверяем работоспособность сборки-пакета.".Trace(_logger);
            var appInfo = new ConsoleAppStartInfo
                          {
                              ExePath = Path.Combine(OUTPUT_1_DIR_PATH, "Abcd.exe"),
                              Arguments = "-run"
                          };
            ConsoleAppControl.StartWaitFinish(appInfo);
        });


        [Test]
        public void _07_Pack_WinSvr_With_Scripting_Test() => _facade.Execute(() =>
        {
            "Упаковываем Windows-сервис с созданием упаковывающего скрипта.".Trace(_logger);

            using (var domain = new AppDomainIsolated<Packer>())
            {
                domain.Proxy.Pack(inputDirPath: WIN_SVR_DIR_PATH,
                    outputDirPath: OUTPUT_1_DIR_PATH,
                    packageName: "Abcd",
                    scripting: true);
            }

            "Проверяем общее число файлов в выходном каталоге.".Trace(_logger);
            var tgtFiles = Directory.EnumerateFiles(OUTPUT_1_DIR_PATH, "*.*", SearchOption.AllDirectories);
            Assert.AreEqual(16, tgtFiles.Count()); // exe и config

            "Проверяем работоспособность сборки-пакета.".Trace(_logger);
            var appInfo = new ConsoleAppStartInfo
                      {
                          ExePath = Path.Combine(OUTPUT_1_DIR_PATH, "Abcd.exe"),
                          Arguments = "-run"
                      };
            ConsoleAppControl.StartWaitFinish(appInfo);

            "Проверяем работоспособность упаковывающего скрипта.".Trace(_logger);
            appInfo = new ConsoleAppStartInfo
                          {
                              ExePath = Path.Combine(OUTPUT_1_DIR_PATH, "PackScript", "Build.bat"),
                              WaitLine = "Microsoft (R) Visual C#"
                          };

            ConsoleAppControl.StartWaitFinish(appInfo);

            "Проверяем работоспособность результата работы упаковывающего скрипта.".Trace(_logger);
            appInfo = new ConsoleAppStartInfo
                      {
                          ExePath = Path.Combine(OUTPUT_1_DIR_PATH, "PackScript", "Abcd.exe"),
                          Arguments = "-run"
                      };
            ConsoleAppControl.StartWaitFinish(appInfo);
        });


        [Test]
        public void _05_Pack_WinFormsApp_Test() => _facade.Execute(() =>
        {
            "Упаковываем WinForms приложение.".Trace(_logger);

            using (var domain = new AppDomainIsolated<Packer>())
            {
                domain.Proxy.Pack(inputDirPath: WIN_FORMS_APP_DIR_PATH,
                    outputDirPath: OUTPUT_1_DIR_PATH,
                    packageName: "Abcd",
                    scripting: false);
            }

            "Проверяем общее число файлов в выходном каталоге.".Trace(_logger);
            var tgtFiles = Directory.EnumerateFiles(OUTPUT_1_DIR_PATH, "*.*", SearchOption.AllDirectories);
            Assert.AreEqual(2, tgtFiles.Count()); // exe и config

            "Проверяем работоспособность сборки-пакета.".Trace(_logger);
            WinAppControl.StartWaitFinish(5, OUTPUT_1_DIR_PATH, "Abcd.exe");
        });

        [SetUp]
        public void SetUp()
        {
            "Создание выходного каталога перед выполнением каждого теста.".Trace(_logger);
            Directory.CreateDirectory(OUTPUT_1_DIR_PATH);
        }


        [TearDown]
        public void TearDown()
        {
            "Удаление выходного каталога по окончании каждого теста.".Trace(_logger);

            if (Directory.Exists(OUTPUT_1_DIR_PATH))
                Directory.Delete(OUTPUT_1_DIR_PATH, recursive: true);

            if (Directory.Exists(OUTPUT_2_DIR_PATH))
                Directory.Delete(OUTPUT_2_DIR_PATH, recursive: true);
        }

        private static readonly TestFacade _facade = TestFacade.GetCurrentClassFacade();
        private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();
    }
}
