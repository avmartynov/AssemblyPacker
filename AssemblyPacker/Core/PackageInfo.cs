using System.Drawing;
using System.IO;

namespace Twidlle.AssemblyPacker.Core
{
    internal class PackageInfo
    {
        public string        PackageName        { get; set; }
        public DirectoryInfo OutputDir          { get; set; }
        public FileInfo[]    ResourceFiles      { get; set; }
        public Icon          AppIcon            { get; set; }
        public AppInfo       AppInfo            { get; set; }
        public string        PackageCsCode      { get; set; } 
        public string        ProgramCsCode      { get; set; }
        public string        InstallerCsCode    { get; set; }
        public string        AssemblyInfoCsCode { get; set; }
    }
}
