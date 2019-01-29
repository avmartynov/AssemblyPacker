using System.ServiceProcess;

namespace Twidlle.Infrastructure.WindowsService
{
    /// <summary> Данные конфигурации Windows-сервиса. </summary>
    internal class WindowsServiceConfig
    {
        /// <summary> Имя Windows-сервиса </summary>
        public string Name { get; set; }

        /// <summary> Описание Windows-сервиса </summary>
        public string Description { get; set; }

        /// <summary> Список Windows-сервисов, от которых зависит данный windows-сервис </summary>
        public string ServicesDependsOn { get; set; }

        /// <summary> Идентификатор cтартового Castle-компонента Windows-сервиса </summary>
        public string StartUpComponent { get; set; }

        /// <summary> Специальная учётная запись для исполнения Windows-сервиса </summary>
        public ServiceAccount ServiceAccount { get; set; }

        /// <summary> Настраиваемая учётная запись для исполнения Windows-сервиса </summary>
        public string Username { get; set; }
    }
}
