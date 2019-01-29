using Microsoft.WindowsAPICodePack.Taskbar;

namespace Twidlle.Infrastructure.WinForms
{
    /// <summary>
    /// В момент вызова этих методов надо быть уверенным, что главное окно приложение уже активировано
    /// (i.e. что событие Activated уже произошло, а оно происходит позже Load).
    /// </summary>
    public static class TaskBar
    {
        public static void SetProgress(int progressPercentage)
        {
            if (!TaskbarManager.IsPlatformSupported)
                return;

            TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Normal);
            TaskbarManager.Instance.SetProgressValue(progressPercentage, 100);
        }


        public static void ResetProgress()
        {
            if (!TaskbarManager.IsPlatformSupported)
                return;

            TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Normal);
            TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.NoProgress);
        }


        public static void SetPaused()
        {
            if (!TaskbarManager.IsPlatformSupported)
                return;

            TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Paused);
        }


        public static void SetError()
        {
            if (!TaskbarManager.IsPlatformSupported)
                return;

            TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Error);
        }
    }
}
