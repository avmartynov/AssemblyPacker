using System.IO;
using System.Text;
using System.Windows;

namespace Twidlle.Infrastructure.Wpf
{
    public class MessageBoxTextWriter : TextWriter
    {
        public override Encoding Encoding => Encoding.UTF8;

        public override void Write(string value)
        {
            MessageBox.Show(value, ApplicationInfo.ProductName, MessageBoxButton.OK, MessageBoxImage.Information);
            base.Write(value);
        }

        public override void WriteLine(string value)
        {
            MessageBox.Show(value, ApplicationInfo.ProductName, MessageBoxButton.OK, MessageBoxImage.Information);
            base.WriteLine(value);
        }
    }

}
