namespace Twidlle.Infrastructure.Testing
{
    /// <summary> Данные для старта консольного приложения. Работает вместе с классом ConsoleApp. </summary>
    /// 
    public class ConsoleAppStartInfo
    {
        /// <summary> Путь к исполняемому файлу приложения. </summary>
        public string ExePath { get; set; }

        /// <summary> Путь к текущему каталогу для исполнения приложения. </summary>
        public string WorkingDirectory { get; set; }

        /// <summary> Аргументы командной строки для запуска приложения. </summary>
        public string Arguments { get; set; }

        /// <summary> Выводимая приложением на консоль строка при переходе приложения в режим ожидания нажатия [Enter] или ввода QuitLine. </summary>
        public string WaitLine { get; set; }

        /// <summary> Ввод этой строки останавливает приложение. Если эти строка не задана или пуста, 
        /// то приложение должно закрываться при нажании [Enter]. </summary>
        public string QuitLine { get; set; }

        /// <summary> Признак отмены ожидания перехода приложения в режим ожидания нажатия [Enter]. </summary>
        public bool NoWait { get; set; }
    }
}
