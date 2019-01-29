using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using NLog;
using NUnit.Framework;
using Twidlle.Infrastructure.CodeAnnotation;

namespace Twidlle.Infrastructure.Testing
{
    /// <summary> Класс-источник множества тестов. </summary>
    /// <remarks> Позволяет создавать тест, который представляет собой выполнение 
    /// и анализ совокупного результата выполнения набора других тестов.
    /// </remarks>
    /// <example> Пример использования. 
    /// 
    ///    using NUnit.Framework;
    ///    using Twidlle.Infrastructure.Testing;
    ///    
    ///    namespace Twidlle.Sandglass.AllTests
    ///    {
    ///        [Explicit]
    ///        public class AllTests_TestFixture
    ///        {
    ///            [TestSet(sectionName: "testSet")]
    ///            public void Test(string name, string cmdLine) 
    ///                => TestSet.RunTestItem(name, cmdLine);
    ///        }
    ///    }
    ///
    /// <configuration>
    ///   <configSections>
    ///     <section name="testSet" type="Twidlle.Infrastructure.CustomConfigSection, Twidlle.Infrastructure" />
    ///   </configSections>
    ///   <testSet BaseDir="..\..\..\"  Config="Debug">
    ///       <Item ProjectDir ="Core.Tests"        FileName="Twidlle.Sandglass.Core.Tests.exe" Arguments="--where &quot;cat!=Long&quot;" />
    ///       <Item ProjectDir ="WinFormsApp.Tests" FileName="Twidlle.Sandglass.WinFormsApp.Tests.exe"  Arguments="--where &quot;cat!=Long&quot;" />
    ///   </testSet>
    /// </configuration>
    /// 
    /// </example>
    public static class TestSetUtility
    {
        /// <summary> Выполняет один тест - элемент множества тестов </summary>
        /// <param name="testItemName"> Имя теста - элемента множества тестов </param>
        /// <param name="commandLine"> Командная строка для выполнения теста </param>
        public static void RunTestItem([NotNull] string testItemName, [NotNull] string commandLine)
            => Assert.IsTrue(Run(testItemName, commandLine));


        /// <summary> Поставщик данных для запуска тестов-элементов набота тестов. </summary>
        /// <param name="sectionName"> Имя секции конфигурации.
        /// Тип секции должен быть 'Twidlle.Infrastructure.CustomConfigSection, Twidlle.Infrastructure'
        /// </param>
        /// <returns></returns>
        [NotNull]
        internal static object[] GetItems(string sectionName)
        {
            var testSet = CustomConfigSection<TestSetSection>.Read(sectionName);
            return testSet.Items.Select(i => (object)new object[]
                {
                    i.ProjectDir,
                    Path.Combine(TestEnvironment.GetTestDirectory(), testSet.BaseDir, i.ProjectDir, 
                        $"bin\\{testSet.Config}", i.FileName) + " " + i.Arguments
                }).ToArray();
        }


        private static bool Run([NotNull] string testName, [NotNull] string commandLine)
        {
            var testDirectory = Path.GetDirectoryName(commandLine) ?? "";
            var argumentsIndex = commandLine.IndexOf(" ", StringComparison.InvariantCulture);
            argumentsIndex = argumentsIndex == -1 ? commandLine.Length : argumentsIndex;


            var resultFile = Path.Combine(testDirectory, "TestResult.xml").PathCanonicalize();
            if (File.Exists(resultFile))
                File.Delete(resultFile);

            _logger.Info($"Running test '{testName}', '{commandLine}' ...");

            var process = Process.Start(
                new ProcessStartInfo
                {
                    WorkingDirectory = testDirectory,
                    FileName  = Path.GetFileName(commandLine),
                    Arguments = commandLine.Substring(argumentsIndex)
                });
            if (process == null)
            {
                _logger.Error($"Can't start test '{testName}', '{commandLine}'.");
                return false;
            }
            process.WaitForExit();
            return AnalyzeResultFile(resultFile);
        }


        private static bool AnalyzeResultFile(string resultFilePath)
        {
            if (!File.Exists(resultFilePath))
            {
                _logger.Error($"Can't find result file '{resultFilePath}'.");
                return false;
            }
            var xmlDoc = XDocument.Load(resultFilePath);
            var result = (string)xmlDoc.XPathEvaluate("string(/test-run/@result)");
            return result == "Passed";
        }

        private static readonly ILogger _logger   = LogManager.GetCurrentClassLogger();
    }
}
