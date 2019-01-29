namespace Twidlle.Infrastructure
{
    public static class ProductInfo
    {
        public const string Name      = "Twidlle Infrastructure";
        public const string Culture   = "";
        public const string Version   = "1.0.10.*";
        public const string Year      = "2018";
        public const string Copyright = "Copyright © " + Year + " " + CompanyInfo.Name;

    #if DEBUG
        public const string Configuration = "Debug";
    #else
        public const string Configuration = "Release";
    #endif
    }
}

