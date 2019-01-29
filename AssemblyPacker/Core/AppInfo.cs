using System.IO;
using System.Reflection;

namespace Twidlle.AssemblyPacker.Core
{
    internal class AppInfo
    {
        public FileInfo     File               { get; set; }
        public AssemblyName AssemblyName       { get; set; }
        public string       MainClassName      { get; set; }
        public bool         MainHasParams      { get; set; }
        public bool         Console            { get; set; }
        public string[]     InstallerTypeNames { get; set; }
    }
}
