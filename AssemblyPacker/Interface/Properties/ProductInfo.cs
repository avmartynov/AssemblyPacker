namespace Twidlle.AssemblyPacker
{
    public static class ProductInfo
    {
        public const string Name = "Twidlle .Net Assembly Packer";
        public const string Version = "2.0.9.*";
        public const string Culture = "";
        public const string Year      = "2018";
        public const string Copyright = "Copyright © " + Year + " " + CompanyInfo.Name;

    #if DEBUG
        public const string Configuration = "Debug";
    #else
        public const string Configuration = "Release";
    #endif
    }
}
