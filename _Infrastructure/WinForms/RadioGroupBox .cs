using System;
using System.Linq;
using System.Windows.Forms;
using Twidlle.Infrastructure.CodeAnnotation;

namespace Twidlle.Infrastructure.WinForms
{
    public class RadioGroupBox : GroupBox
    {
        public int Selected
        {
            get => _selected;
            set
            {
                var radioButton = this.Controls.OfType<RadioButton>()
                    .FirstOrDefault(radio => radio.TaggedBy(value));

                if (radioButton == null)
                    return;

                radioButton.Checked = true;
                _selected = value;
            }
        }

        public event EventHandler SelectedChanged = delegate { };


        protected override void OnControlAdded(ControlEventArgs e)
        {
            base.OnControlAdded(e);

            if (e.Control is RadioButton radioButton)
                radioButton.CheckedChanged += radioButton_CheckedChanged;
        }

        private void radioButton_CheckedChanged(object sender, EventArgs e)
        {
            var radio = (RadioButton)sender;
            if (radio.Checked && int.TryParse(radio.Tag?.ToString(), out var val))
            {
                _selected = val;
                SelectedChanged(this, new EventArgs());
            }
        }

        private int _selected;
    }

    public static class LocalExtensions
    {
        public static bool TaggedBy([NotNull] this Control radio, int checkedValue)
            => Int32.TryParse(radio.Tag?.ToString(), out var val) && val == checkedValue;
    }
}
