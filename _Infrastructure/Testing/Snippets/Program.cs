// Файл не компилируется в составе инфраструктурных компонентов.
// Файл предназначен для включения как Link в состав автономных тестов (тестов-приложений). 

using NUnit.Framework;
using NUnitLite;
using Twidlle.Infrastructure.Testing;

namespace Twidlle.Infrastructure.Testing
{
    internal class Program
    {
        /// <summary> The main entry point for the application. </summary>
        /// <remarks> 
        /// For debugging single test with F5:
        ///  - mark test with attribute [Category("Debug")]
        ///  - use options --wait --where "cat=Debug" 
        /// </remarks>
        private static int Main(string[] args)
            => new AutoRun().Execute(args);
    }
}

namespace Twidlle
{
    [SetUpFixture]
    public class SetUpFixture : BaseSetUpFixture
    {
    }
}

