using System.Reflection;
using System.Runtime.InteropServices;
using Twidlle;
using Twidlle.AssemblyPacker;

[assembly: AssemblyTitle(ProductInfo.Name + " Command Line Utility")]
[assembly: AssemblyDescription("")]

[assembly: AssemblyProduct(ProductInfo.Name)]
[assembly: AssemblyCulture(ProductInfo.Culture)]
[assembly: AssemblyVersion(ProductInfo.Version)]
[assembly: AssemblyConfiguration(ProductInfo.Configuration)]
[assembly: AssemblyCopyright(ProductInfo.Copyright)]

[assembly: AssemblyCompany(CompanyInfo.Name)]
[assembly: AssemblyTrademark(CompanyInfo.Trademark)]

[assembly: ComVisible(false)]