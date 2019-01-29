using System;
using System.Configuration;
using System.Linq.Expressions;
using System.Windows;
using Twidlle.Infrastructure.CodeAnnotation;


namespace Twidlle.Infrastructure.Wpf
{
    /// <summary> Компонент сохраняет и восстанавливает положение окна на экране. </summary>
    /// <remarks> 
    /// Как использовать для формы AbcdWindow:
    /// 1. В Settings надо добавить свойство с именем "AbcdWindow".
    /// 
    /// 2. В конструкторе формы AbcdWindow до Initialize надо вставить: 
    ///    WindowRestoreManager.Initialize(this, Settings.Default, s => s.AbcdWindow);
    /// 
    /// 3. Убедиться (обеспечить), что при завершении приложения
    ///     (например, в App_OnExit) вызывается Settings.Default.Save();
    /// </remarks>
    public static class FormRestoreManager 
    {
        public static void Initialize<TSettings>([NotNull] Window window,
                                                 [NotNull] TSettings settings,
                                                 [NotNull] Expression<Func<TSettings, string>> formStateProperty) 
            where TSettings : SettingsBase
        {
            settings = settings ?? throw new ArgumentNullException(nameof(settings));

            Initialize(window, () => settings.Get(formStateProperty), 
                                s => settings.Set(formStateProperty, s));
        }

        public static void Initialize([NotNull] Window window,
                                      [NotNull] Func<string>   loadFormState,
                                      [NotNull] Action<string> saveFormState)
        {
            window.Initialized  += (s, e) => OnLoad(window, loadFormState());
            window.Closing += (s, e) => saveFormState(GetWindowState(window).ToJson());
        }


        private static void OnLoad(Window window, [NotNull] string formStateLoaded)
        {
            try
            {
                // При первом вызове формы используем состояние формы по умолчанию.
                if (! String.IsNullOrEmpty(formStateLoaded))
                    SetFormState(window, formStateLoaded.DeserializeJson<WindowStateSizeLocation>());
            }
            catch (Exception)
            {
                // Stay default form state.
            }
        }


        [NotNull]
        private static WindowStateSizeLocation GetWindowState([NotNull] Window window)
            => new WindowStateSizeLocation
               { 
                    Width  = window.WindowState == WindowState.Normal ? window.Width  : window.RestoreBounds.Size.Width,
                    Height = window.WindowState == WindowState.Normal ? window.Height : window.RestoreBounds.Size.Height,
                    Left   = window.WindowState == WindowState.Normal ? window.Left   : window.RestoreBounds.Location.X,
                    Top    = window.WindowState == WindowState.Normal ? window.Top    : window.RestoreBounds.Location.Y,
                    State  = window.WindowState
               };


        private static void SetFormState([NotNull] Window window, [NotNull] WindowStateSizeLocation stateSizeLocation)
        {
            window.Width  = stateSizeLocation.Width ;
            window.Height = stateSizeLocation.Height;
            window.Left   = stateSizeLocation.Left;
            window.Top    = stateSizeLocation.Top;
            window.WindowState = stateSizeLocation.State;
        }


        private class WindowStateSizeLocation
        {
            public double Width  { get; set; }
            public double Height { get; set; }

            public double Left   { get; set; }
            public double Top    { get; set; }

            public WindowState State { get; set;}
        }
    }
}
