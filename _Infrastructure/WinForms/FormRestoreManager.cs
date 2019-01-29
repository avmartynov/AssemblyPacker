using System;
using System.Configuration;
using System.Drawing;
using System.Linq.Expressions;
using System.Windows.Forms;
using Twidlle.Infrastructure.CodeAnnotation;


namespace Twidlle.Infrastructure.WinForms
{
    /// <summary> Компонент сохраняет и восстанавливает положение формы на экране. </summary>
    /// <remarks> 
    /// Как использовать для формы AbcdForm:
    /// 1. В Settings надо добавить свойство с именем "AbcdForm".
    /// 
    /// 2. В конструкторе формы AbcdForm до Initialize надо вставить: 
    ///    FormRestoreManager.Initialize(this, Settings.Default, s => s.AbcdForm);
    /// 
    /// 3. Убедиться (обеспечить), что при завершении приложения
    ///     (например, в AbcdForm_FormClosed) вызывается Settings.Default.Save();
    /// </remarks>
    public static class FormRestoreManager 
    {
        public static void Initialize<TSettings>([NotNull] Form form,
                                                 [NotNull] TSettings settings,
                                                 [NotNull] Expression<Func<TSettings, string>> formStateProperty) 
            where TSettings : SettingsBase
        {
            settings = settings ?? throw new ArgumentNullException(nameof(settings));

            Initialize(form, () => settings.Get(formStateProperty), 
                              s => settings.Set(formStateProperty, s));
        }

        public static void Initialize([NotNull] Form form,
                                      [NotNull] Func<string>   loadFormState,
                                      [NotNull] Action<string> saveFormState)
        {
            form.Load        += (s, e) => OnLoad(form, loadFormState());
            form.FormClosing += (s, e) => saveFormState(GetFormState(form).ToJson());
        }


        private static void OnLoad(Form form, [NotNull] string formStateLoaded)
        {
            try
            {
                // При первом вызове формы используем состояние формы по умолчанию.
                if (! String.IsNullOrEmpty(formStateLoaded))
                    SetFormState(form, formStateLoaded.DeserializeJson<FormState>());
            }
            catch (Exception)
            {
                // Stay default form state.
            }
        }


        [NotNull]
        private static FormState GetFormState([NotNull] Form form)
            => new FormState
               { 
                    Size     = form.WindowState == FormWindowState.Normal ? form.Size     : form.RestoreBounds.Size,
                    Location = form.WindowState == FormWindowState.Normal ? form.Location : form.RestoreBounds.Location,
                    State    = form.WindowState
               };


        private static void SetFormState([NotNull] Form form, [NotNull] FormState formState)
        {
            form.Size        = formState.Size;
            form.Location    = formState.Location;
            form.WindowState = formState.State;
        }


        private class FormState
        {
            public Size Size { get; set; }
            public Point Location { get; set; }
            public FormWindowState State { get; set;}
        }
    }
}
