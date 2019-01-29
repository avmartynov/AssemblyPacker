using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Configuration.Install;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using IWshRuntimeLibrary;
using NLog;
using NLog.Layouts;
using NLog.Targets;
using Twidlle.Infrastructure.CodeAnnotation;

namespace Twidlle.Infrastructure.WindowsService
{
    /// <summary>
    /// Различные варианты стартового кода приложения Windows-сервиса (реализиции метода Main).
    /// </summary>
    public static class WindowsServiceProcess
    {
        /// <summary> Точка входа Windows-сервиса.</summary>
        /// <remarks> Все параметры конфигурации сервиса могут задаваться явно, как параметры этого метода 
        /// (самостоятельно зачитываться вызывающим кодом из какого-то специфического места или каким-то ещё специфическим способом), 
        /// или могут автоматически зачитываться из конфигурационной секции с именем "windowsService". 
        /// Тип секции должен быть System.Configuration.NameValueFileSectionHandler
        /// </remarks>
        /// <example>
        /// 
        ///   <windowsService>
        ///       <add key="Name"               value="_Test Service" />
        ///       <add key="Description"        value="Test Service" />
        ///       <add key="ServicesDependedOn" value="RPCSS, MSMQ" />
        ///       <add key="StartUpComponent"   value="StartUp" />
        ///   </windowsService>
        /// 
        /// </example>
        /// 
        /// <param name="args"> Аргументы командной строки приложения </param>
        /// <param name="startUpObjectFactory"> Метод, создающий стартовый компонент сервиса по его имени. </param>
        /// <param name="serviceName"> Имя сервиса. Если задано, то перекрывает имя заданное в конфигурационном файле. </param>
        /// <param name="serviceDescription"> Описание сервиса. Если задано, то перекрывает имя заданное в конфигурационном файле. </param>
        /// <param name="servicesDependsOn"> Список сервисов-зависимостей данного сервиса. Если задано, то перекрывает имя заданное в конфигурационном файле. </param>
        /// <param name="startUpComponent"> Имя стартового комопонента сервиса. Если задано, то перекрывает имя заданное в конфигурационном файле. </param>
        /// <param name="serviceAccountName"> Специальная учётная запись для исполнения Windows-сервиса. Если задано, то перекрывает имя заданное в конфигурационном файле. </param>
        /// <param name="username"> Настраиваемая учётная запись для исполнения Windows-сервиса. Если задано, то перекрывает имя заданное в конфигурационном файле. </param>
        /// 
        public static void Run([NotNull] string[] args, [InstantHandle] Func<string, IDisposable> startUpObjectFactory, 
            [CanBeNull] string serviceName        = null, 
            [CanBeNull] string serviceDescription = null, 
            [CanBeNull] string servicesDependsOn  = null,
            [CanBeNull] string startUpComponent   = null, 
            [CanBeNull] string serviceAccountName = null, 
            [CanBeNull] string username           = null)
        {
            Run(args, () => startUpObjectFactory(ServiceConfig.StartUpComponent),
                serviceName, serviceDescription, servicesDependsOn, startUpComponent, serviceAccountName, username);
        }

        public static void Run([NotNull] string[] args, [InstantHandle] Func<IDisposable> startUpObjectFactory, 
            [CanBeNull] string serviceName        = null, 
            [CanBeNull] string serviceDescription = null, 
            [CanBeNull] string servicesDependsOn  = null,
            [CanBeNull] string startUpComponent   = null, 
            [CanBeNull] string serviceAccountName = null, 
            [CanBeNull] string username           = null)
        {
            var serviceAccount = ParseServiceAccount(serviceAccountName, ServiceConfig.ServiceAccount);

            ServiceConfig.Name              = serviceName       ?? ServiceConfig.Name; 
            ServiceConfig.Description       = serviceDescription      ?? ServiceConfig.Description; 
            ServiceConfig.ServicesDependsOn = servicesDependsOn ?? ServiceConfig.ServicesDependsOn; 
            ServiceConfig.StartUpComponent  = startUpComponent  ?? ServiceConfig.StartUpComponent; 
            ServiceConfig.ServiceAccount    = serviceAccount; 
            ServiceConfig.Username          = username          ?? ServiceConfig.Username;

            RunInternal(args, startUpObjectFactory);
        }

        internal static WindowsServiceConfig ServiceConfig { get; } = ReadServiceConfig();

        #region Private members

        private static void RunInternal([NotNull] IReadOnlyList<string> args, Func<IDisposable> startUpObjectFactory) 
        {
            if (Environment.UserInteractive || args[0] == "-run")
                RunAsConsoleApplication(args, startUpObjectFactory);
            else
                RunAsService(startUpObjectFactory);
        }


        private static void RunAsService(Func<IDisposable> serverFactory)
        {
            ServiceBase.Run(new WindowsServiceComponent(serverFactory));
        }


        private static void RunAsConsoleApplication([NotNull] IReadOnlyList<string> commandlineArguments, Func<IDisposable> serverFactory)
        {
            try
            {
                if (commandlineArguments.Count == 0 || commandlineArguments[0] == "-run")
                    RunAsConsoleServer(serverFactory);
                else
                    ExecuteCommand(commandlineArguments[0]);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
            }

            if (!commandlineArguments.Contains("-wait")
             && !commandlineArguments.Contains("-w"))
            {
                return;
            }

            Console.WriteLine("Hint <enter> key to continue...");
            Console.ReadLine();
        }


        private static void RunAsConsoleServer(Func<IDisposable> serverFactory)
        {
            var service = new WindowsServiceComponent(serverFactory);
            Console.Title = ServiceConfig.Name;

            service.Start();

            Console.WriteLine($"{ServiceConfig.Name} is running...");
            Console.WriteLine("Press <enter> key to stop.");
            Console.ReadLine();

            service.Stop();

            Console.WriteLine($"{ServiceConfig.Name} stopped.");
        }


        private static void ExecuteCommand([NotNull] string command)
        {
            switch (command.ToLower())
            {
                case "-i":
                case "-install":
                    InstallService();
                    break;

                case "-u":
                case "-uninstall":
                    UninstallService();
                    break;

                case "-start":
                    StartService();
                    break;

                case "-stop":
                    StopService();
                    break;

                case "-l":
                case "-log":
                    ShowLog();
                    break;

                case "-c":
                case "-config":
                    ShowConfig();
                    break;

                case "-r":
                case "-restart":
                    RestartService();
                    break;

                case "-g":
                case "-generateLinks":
                    GenerateLinkFiles();
                    break;

                case "-h":
                case "-help":
                case "/h":
                case "/help":
                case "?":
                    WriteHelp();
                    break;

                default:
                    WriteErrorMessage(command);
                    break;
            }
        }


        private static void InstallService()
        {
            using (var installer = CreateAssemblyInstaller())
            {
                try
                {
                    var state = new Hashtable();
                    installer.Install(state);
                    installer.Commit(state);

                    Console.WriteLine("Service {0} installed successfully.", ServiceConfig.Name);
                }
                catch
                {
                    installer.Rollback(new Hashtable());
                    throw;
                }
            }
        }


        private static void UninstallService()
        {
            using (var installer = CreateAssemblyInstaller())
            {
                installer.Uninstall(new Hashtable());

                Console.WriteLine("Service {0} uninstalled successfully.", ServiceConfig.Name);
            }
        }


        private static void StartService()
        {
            try
            {
                Console.WriteLine($"Service {ServiceConfig.Name} starting...");

                var serviceCtrl = new ServiceController(ServiceConfig.Name);
                serviceCtrl.Start();
                serviceCtrl.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(30));

                Console.WriteLine($"Service {ServiceConfig.Name} successfully started.");
            }
            catch (System.ServiceProcess.TimeoutException)
            {
                throw new InvalidOperationException("Time out has expired and operation has not been complited.");
            }
        }


        private static void StopService()
        {
            try
            {
                Console.WriteLine($"Service {ServiceConfig.Name} stopping...");

                var serviceCtrl = new ServiceController(ServiceConfig.Name);
                serviceCtrl.Stop();
                serviceCtrl.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(30));

                Console.WriteLine($"Service {ServiceConfig.Name} successfully stoped.");
            }
            catch (System.ServiceProcess.TimeoutException)
            {
                throw new InvalidOperationException("Time out has expired and operation has not been complited.");
            }
        }


        private static void RestartService()
        {
            try
            {
                StopService();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
            StartService();
        }


        private static void ShowLog()
        {
            var logFile = GetNLogFileTargetPath(targetName: "file");
            if (!System.IO.File.Exists(logFile))
                throw new InvalidOperationException($"File '{logFile}' does not exist.");

            ShowFile(logFile);
        }


        private static void ShowConfig()
        {
            var configFile = GetAppConfigFilePath();
            if (!System.IO.File.Exists(configFile))
                throw new InvalidOperationException($"File '{configFile}' does not exist.");

            ShowFile(configFile);
        }


        private static void GenerateLinkFiles()
        {
            CreateLinkFile("___Log",     "-log");
            CreateLinkFile("___Config",  "-config");
            CreateLinkFile("__Start",    "-start -w");
            CreateLinkFile("__Stop",     "-stop -w");
            CreateLinkFile("__Restart",  "-restart -w");
            CreateLinkFile("_Install",   "-install -w");
            CreateLinkFile("_Uninstall", "-uninstall -w");
        }


        [NotNull]
        private static AssemblyInstaller CreateAssemblyInstaller()
        {
            return new AssemblyInstaller
            {
                UseNewContext = true,
                Assembly = Assembly.GetEntryAssembly() 
            };
        }


        private static string GetNLogFileTargetPath(string targetName)
        {
            var target = LogManager.Configuration.AllTargets.FirstOrDefault(t => t.Name == targetName);
            if (target == null)
                throw new InvalidOperationException($"No nlog-target named '{targetName}'");

            if (!(target is FileTarget fileTarget))
                throw new InvalidOperationException($"Nlog-target '{targetName}' is not FileTarget,");

            if (!(fileTarget.FileName is SimpleLayout layout))
                throw new InvalidOperationException("Error"); // Формальность.

            return SimpleLayout.Evaluate(layout.Text);
        }


        [NotNull]
        private static string GetAppConfigFilePath()
            => ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None).FilePath;


        private static void ShowFile(string filePath)
        {
            Console.WriteLine("Opening file {0}...", filePath);

            Process.Start(new ProcessStartInfo(filePath) { UseShellExecute = true });
        }


        private static void CreateLinkFile(string linkFileName, string programArguments)
        {
            var shell = new WshShell();
            var link = (IWshShortcut)shell.CreateShortcut(linkFileName + ".lnk");
            link.TargetPath       = Assembly.GetEntryAssembly().CodeBase;
            link.WorkingDirectory = Path.GetDirectoryName(link.TargetPath);
            link.Arguments        = programArguments;
            link.Save();
        }


        private static void WriteErrorMessage(string wrongArgument)
        {
            Console.Error.WriteLine(@"
Invalid command line argument: {0}", wrongArgument);
            WriteHelp();
        }


        private static void WriteHelp()
        {
            Console.Out.WriteLine(@"
Usage: 
    Install service: 
        {0}.exe -install 

    Uninstall service: 
        {0}.exe -uninstall

    Start service: 
        {0}.exe -start

    Stop service: 
        {0}.exe -stop

    Restart service: 
        {0}.exe -restart

    Open log file: 
        {0}.exe -log

    Open configuration file: 
        {0}.exe -config

    Run as console application: 
        {0}.exe
",
                Assembly.GetEntryAssembly().GetName().Name);
        }

        /// <summary> Читает настройки Windows-сервиса из конфигурационной секции </summary>
        /// <remarks>
        /// Тип секции должен быть System.Configuration.NameValueFileSectionHandler.
        /// Если такой секции не настроено, используются аppSettings с ключами "Name" и "Description" 
        /// (это для совместимости со старыми сервисами).
        /// </remarks>
        [NotNull]
        private static WindowsServiceConfig ReadServiceConfig()
        {
            var section = (NameValueCollection)ConfigurationManager.GetSection("windowsService");
            if (section == null)
                throw new InvalidOperationException("There is no configuration section <windowsService>.");

            return new WindowsServiceConfig
                   {
                       Name              = section["Name"],
                       Description       = section["Description"],
                       ServicesDependsOn = section["ServicesDependsOn"],
                       StartUpComponent  = section["StartUpComponent"],
                       ServiceAccount    = ParseServiceAccount(section["ServiceAccount"], ServiceAccount.LocalSystem),
                       Username          = section["Username"]
                   };
        }

        private static ServiceAccount ParseServiceAccount([CanBeNull] string value, ServiceAccount defaultServiceAccount)
        {
            if (String.IsNullOrWhiteSpace(value))
                return defaultServiceAccount;

            if (Enum.TryParse(value, true, out ServiceAccount account)) 
                return account;

            _logger.Warn("Invalid format of ServiceAccount: {0}", value);
            return ServiceAccount.LocalSystem;
        }

        private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        
        #endregion Private members
    }
}
//
// http://geekswithblogs.net/BlackRabbitCoder/archive/2010/09/23/c-windows-services-1-of-2-creating-a-debuggable-windows.aspx