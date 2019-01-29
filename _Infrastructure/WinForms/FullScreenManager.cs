using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Twidlle.Infrastructure.CodeAnnotation;

namespace Twidlle.Infrastructure.WinForms
{
    /// <summary> Компонент обеспечивает выполнение команды на переход режим FullScreen. </summary>
    public class FullScreenManager
    {
        public FullScreenManager([NotNull] Form form, 
                                 [NotNull] Control centralControl)
        {
            _form = form ?? throw new ArgumentNullException(nameof(form));
            _centralControl = centralControl ?? throw new ArgumentNullException(nameof(centralControl));

            form.FormClosing += (s, e) => OnFormClosing();
        }


        public void ToggleFullScreen()
        {
            if (_fullScreen)
                ResumeFullScreen();
            else
                SetFullScreen();                
        }

        public bool FullScreen => _fullScreen;

        #region Private members

        private void OnFormClosing()
        {
            if (_fullScreen)
                ResumeFullScreen();
        }


        private void SetFullScreen()
        {
            _hiddenControls.Clear();

            foreach (Control c in _form.Controls)
            {
                if (!c.Visible || c == _centralControl)
                    continue;

                c.Visible = false;
                c.Enabled = false;
                _hiddenControls.Add(c);
            }
            _formBorderStyle = _form.FormBorderStyle;
            _formWindowState = _form.WindowState;
            _formMaximumSize = _form.MaximumSize;

            _form.MaximumSize     = new Size(Int16.MaxValue, Int16.MaxValue);
            _form.FormBorderStyle = FormBorderStyle.None;
            _form.WindowState     = FormWindowState.Maximized;

            _fullScreen = true;
            _centralControl.Visible = true;
        }


        private void ResumeFullScreen()
        {
            foreach (var c in _hiddenControls)
            {
                c.Visible = true;
                c.Enabled = true;
            }

            _form.FormBorderStyle = _formBorderStyle;
            _form.WindowState     = _formWindowState;
            _form.MaximumSize     = _formMaximumSize;

            _fullScreen = false;
            _centralControl.Visible = false;
        }

        private bool _fullScreen;
        private FormBorderStyle _formBorderStyle;
        private FormWindowState _formWindowState;
        private Size            _formMaximumSize;

        private readonly List<Control> _hiddenControls = new List<Control>();
        private readonly Form _form;
        private readonly Control _centralControl;

        #endregion
    }
}
