using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Twidlle.Infrastructure.WinForms
{
    public class MessageBoxTextWriter : TextWriter
    {
        public override Encoding Encoding => Encoding.UTF8;

        public override void Write(string value)
        {
            MessageBox.Show(value, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
            base.Write(value);
        }

        public override void WriteLine(string value)
        {
            MessageBox.Show(value, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
            base.WriteLine(value);
        }
    }

}
