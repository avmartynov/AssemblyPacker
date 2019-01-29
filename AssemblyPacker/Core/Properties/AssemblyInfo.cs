using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Twidlle;
using Twidlle.AssemblyPacker;

[assembly: AssemblyTitle("Twidlle .Net Assembly Packer Core")]

[assembly: AssemblyProduct(ProductInfo.Name)]
[assembly: AssemblyCulture(ProductInfo.Culture)]
[assembly: AssemblyVersion(ProductInfo.Version)]
[assembly: AssemblyConfiguration(ProductInfo.Configuration)]
[assembly: AssemblyCopyright(ProductInfo.Copyright)]

[assembly: AssemblyCompany(CompanyInfo.Name)]
[assembly: AssemblyTrademark(CompanyInfo.Trademark)]

[assembly: ComVisible(false)]
[assembly: InternalsVisibleTo("Twidlle.AssemblyPacker.Core.Tests")]
