using System.IO;
using Castle.Windsor;
using Castle.Windsor.Installer;
using Twidlle.Infrastructure.CodeAnnotation;

namespace Twidlle.Infrastructure
{
    /// <summary> Обеспечивает глобальный доступ к единому IOC-контейнеру приложения. </summary>
    public static class ApplicationIoc
    {
        /// <summary> IOC-контейнер приложения </summary>
        [NotNull]
        public static IWindsorContainer Default => _instance ?? (_instance = new WindsorContainer());


        public static void InstallConfigFile(this IWindsorContainer container)
        {
            var castleConfigFile = Path.Combine(ApplicationInfo.Directory.FullName, "Castle.config");
            if (File.Exists(castleConfigFile))
            {
                container.Install(Configuration.FromXmlFile(castleConfigFile));
            }
            else if (File.Exists(ApplicationInfo.ConfigFilePath) 
                 && System.Configuration.ConfigurationManager.GetSection("castle") != null)
            {
                container.Install(Configuration.FromAppConfig());
            }
        }


        /// <summary> Удаляет IOC-контейнер приложения.
        /// IOC-контейнер будет заново создан при следующем обращении через ApplicationIoc.Default.</summary>
        public static void Reset()
        {
            _instance?.Dispose();
            _instance = null;
        }

        private static IWindsorContainer _instance;
    }
}