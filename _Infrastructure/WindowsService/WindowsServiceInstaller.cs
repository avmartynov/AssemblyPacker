using System;
using System.Configuration.Install;
using System.ServiceProcess;

namespace Twidlle.Infrastructure.WindowsService
{
    public class WindowsServiceInstaller : Installer
    {
        public WindowsServiceInstaller()
        {
            var config = WindowsServiceProcess.ServiceConfig;

            var dependServices = (config.ServicesDependsOn ?? "").Split(new[] { ',' },
                    StringSplitOptions.RemoveEmptyEntries);

            var process = new ServiceProcessInstaller
            {
                Account  = config.ServiceAccount,
                Username = config.Username
            };
            var service = new ServiceInstaller
            {
                ServiceName        = config.Name,
                DisplayName        = config.Name,
                Description        = config.Description,
                StartType          = ServiceStartMode.Automatic,
                ServicesDependedOn = dependServices
            };
            Installers.Add(process);
            Installers.Add(service);
        }
    }
}
