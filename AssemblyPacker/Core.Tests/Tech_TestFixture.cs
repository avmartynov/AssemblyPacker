using System;
using System.Drawing;
using System.IO;
using NLog;
using NUnit.Framework;
using Twidlle.Infrastructure.Diagnostics;
using Twidlle.Infrastructure.Testing;
using static Twidlle.AssemblyPacker.Core.Tests.TestStuff;

namespace Twidlle.AssemblyPacker.Core.Tests
{
    public class Tech_TestFixture
    {
        [Test]
        public void _05_AssemblyAttributes() => _facade.Execute(() =>
        {                      
            var dic = Packing.GetAssemblyAttributes(new FileInfo(CONSOLE_APP_FILE_PATH));

            Assert.That(dic["Title"],         Is.EqualTo("ConsoleApplication1"));
            Assert.That(dic["Configuration"], Is.EqualTo("config"));
            Assert.That(dic["Copyright"],     Is.EqualTo("Copyright ©  2015"));
            Assert.That(dic["Version"],       Is.EqualTo("1.0.15.0"));

            var icon = Icon.ExtractAssociatedIcon(new FileInfo(CONSOLE_APP_FILE_PATH).FullName);
            Assert.That(icon, Is.Not.Null);
        });


        [Test]
        public void _04_ValidAssemblyName_0_Test() => _facade.Execute(() =>
        {
            var dirPath = CONSOLE_APP_DIR_PATH;

            ExpectValid("Abcd");
            ExpectValid("A");
            ExpectNotValid("System");
            ExpectNotValid("System.Core");
            ExpectNotValid("System.Configuration.Install");
            ExpectValid("A.bc.d");
            ExpectNotValid("A bc.d");
            ExpectNotValid("A>bc.d");
            ExpectNotValid("Abcd&");
            ExpectNotValid("Abcd+");
            ExpectValid("Abcd_");
            ExpectNotValid("Abcd ");

            void ExpectValid(string testName) =>
                Assert.DoesNotThrow(() => Packing.ValidateAssemblyName(dirPath, testName));

            void ExpectNotValid(string testName) =>
                Assert.Throws<InvalidOperationException>(() => Packing.ValidateAssemblyName(dirPath, testName));
        });

        [Test]
        public void _01_Check_ConsoleApp_Test() => _facade.Execute(() =>
        {
            "Проверка работоспособности тестового приложения, т.е. приложения подлежащего упаковке.".Log();

            ConsoleAppControl.StartWaitFinish(CONSOLE_APP_FILE_PATH);
        });


        [Test]
        public void _02_Check_WinFormsApp_Test() => _facade.Execute(() =>
        {
            "Проверка работоспособности тестового приложения, т.е. приложения, подлежащего упаковке.".Log();

            WinAppControl.StartWaitFinish(waitSeconds: 5, paths: WIN_FORMS_APP_FILE_PATH);
        });


        [Test]
        public void _03_Check_WinSvr_Test() => _facade.Execute(() =>
        {
            "Проверка работоспособности тестового приложения, т.е. приложения, подлежащего упаковке.".Log();

            ConsoleAppControl.StartWaitFinish(new ConsoleAppStartInfo { ExePath = WIN_SVR_FILE_PATH, Arguments = "-run"});
        });


        [Test]
        public void _06_GetStartupInfo_ConsoleApp_Test() => _facade.Execute(() =>
        {
            var appInfo = AppExploring.Explore(CONSOLE_APP_FILE_PATH);

            Assert.IsTrue(appInfo.Console);
            Assert.IsTrue(appInfo.MainHasParams);
            Assert.AreEqual("ConsoleApplication1.Program", appInfo.MainClassName);
            Assert.AreEqual("ConsoleApplication1, Version=1.0.15.0, Culture=neutral, PublicKeyToken=null",
                appInfo.AssemblyName.FullName);
            CollectionAssert.IsEmpty(appInfo.InstallerTypeNames);
        });


        [Test]
        public void _07_GetStartupInfo_WinFormsApp_Test() => _facade.Execute(() =>
        {
            var appInfo = AppExploring.Explore(WIN_FORMS_APP_FILE_PATH);

            Assert.IsFalse(appInfo.Console);
            Assert.IsFalse(appInfo.MainHasParams);
            Assert.AreEqual("WindowsFormsApplication1.Program", appInfo.MainClassName);
            Assert.AreEqual("WindowsFormsApplication1, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null",
                appInfo.AssemblyName.FullName);
            CollectionAssert.IsEmpty(appInfo.InstallerTypeNames);
        });

        [Test]
        public void _08_GetStartupInfo_WinSvr_Test() => _facade.Execute(() =>
        {
            var appInfo = AppExploring.Explore(WIN_SVR_FILE_PATH);

            Assert.IsTrue(appInfo.Console);
            Assert.IsTrue(appInfo.MainHasParams);
            Assert.AreEqual("WindowsService0.Program", appInfo.MainClassName);
            Assert.AreEqual("WindowsService0, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null",
                appInfo.AssemblyName.FullName);
            CollectionAssert.IsNotEmpty(appInfo.InstallerTypeNames);
        });

        private static readonly TestFacade _facade = TestFacade.GetCurrentClassFacade();
    }

    internal static class Tech_TestFixtureExtensions
    {
        public static void Log(this string message) 
            => _logger.Trace(message);

        private static readonly ILogger _logger = LogManager.GetLogger(typeof(Tech_TestFixture).FullName);
    }
}
