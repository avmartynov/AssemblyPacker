using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Win32;

namespace Twidlle.Infrastructure.WinForms
{
    /// <summary>
    /// Полезняшки для приложений работающих в фоновом режиме (т.е. для приложений
    /// постоянно работающих, но редко взаимодействующих с пользователем).
    /// </summary>
    public static class BackgroundApplication
    {

        /// <summary> Уменьшает используемую память </summary>
        /// <remarks> Reducing WinForm Memory Footprint with SetWorkingSet
        /// http://www.west-wind.com/Weblog/posts/240.aspx
        /// </remarks>
        public static void CompactMemory()
        {
            var process = Process.GetCurrentProcess();
            process.MaxWorkingSet = (IntPtr)750000;
            process.MinWorkingSet = (IntPtr)100000;
        }

        /// <summary> Помечает приложение как требующее старта при каждом старте опереционной системы </summary>
        public static void RunAtSystemStartup(bool on)
        {
            const string key = @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Run";
            var ass     = Assembly.GetEntryAssembly();
            var value   = ass.GetName().Name;
            var exePath = Application.ExecutablePath;
            if (exePath == null)
                throw new InvalidOperationException();
            Registry.SetValue(key, value, on ? exePath : string.Empty);
        }

        /// <summary> Признак того, что ещё один экземпляр данного приложения уже стартован </summary>
        public static bool IsAlreadyRunning()
        {
            oneApplicationInstanceGuard = new Mutex(false, 
                Assembly.GetEntryAssembly().GetType().GUID.ToString(), 
                out var firstInstance);
            return firstInstance;
        }

        // ReSharper disable once NotAccessedField.Local
        private static Mutex oneApplicationInstanceGuard;
    }
}
