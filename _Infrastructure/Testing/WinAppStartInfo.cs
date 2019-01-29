namespace Twidlle.Infrastructure.Testing
{
    /// <summary> Данные для старта оконого приложения. Работает вместе с классом WinAppController. </summary>
    public class WinAppStartInfo
    {
        /// <summary> Путь к исполняемому файлу приложения. </summary>
        public string ExePath { get; set; }

        /// <summary> Путь к текущему каталогу для исполнения приложения. </summary>
        public string WorkingDirectory { get; set; }

        /// <summary> Аргументы командной строки для запуска приложения. </summary>
        public string Arguments { get; set; }
    }
}
