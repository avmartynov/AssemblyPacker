using System;
using System.Runtime.InteropServices;

namespace Twidlle.Infrastructure.WinForms
{
    public static class NativeMethods
    {
        /// <summary> Проигрывает звуковой файл ассинхронно </summary>
        /// <param name="wavFile"></param>
        /// <returns> false - в случае проблем</returns>
        public static bool PlaySoundAsync(string wavFile)
            => PlaySound(wavFile, IntPtr.Zero, 1 /* SND_ASYNC*/);

        /// <summary> Предотвращает включение скринсэйвера и отключение работы компьютера во время работы программы. </summary>
        public static void StartKeepingApplicationAlive()
            => SetThreadExecutionState(EXECUTION_STATE.ES_DISPLAY_REQUIRED
                                     | EXECUTION_STATE.ES_SYSTEM_REQUIRED
                                     | EXECUTION_STATE.ES_CONTINUOUS);


        /// <summary> Отменяет предотвращение включения скринсэйвера и отключения работы компьютера во время работы программы. </summary>
        public static void StopKeepingApplicationAlive()
            => SetThreadExecutionState(EXECUTION_STATE.ES_CONTINUOUS);


        [DllImport("winmm.DLL", EntryPoint = "PlaySound", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern bool PlaySound(string szSound, IntPtr hMod, int flags);


        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern uint SetThreadExecutionState(EXECUTION_STATE esFlags);


        [Flags]
        public enum EXECUTION_STATE : uint
        {
            ES_AWAY_MODE_REQUIRED = 0x00000040,
            ES_CONTINUOUS = 0x80000000,
            ES_DISPLAY_REQUIRED = 0x00000002,
            ES_SYSTEM_REQUIRED = 0x00000001
            // Legacy flag, should not be used.
            // ES_USER_PRESENT = 0x00000004
        }
    }
}
