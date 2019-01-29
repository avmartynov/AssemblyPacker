// Файл не компилируется в составе инфраструктурных компонентов.
// Файл предназначен для вклчения как Link в состав автономных тестов (тестов-приложений). 

using NUnit.Framework;
using Twidlle.Infrastructure.CodeAnnotation;
using Twidlle.Infrastructure.Diagnostics;

namespace Twidlle.Infrastructure.Testing
{
    [Explicit] // --wait --where "test=Twidlle.Infrastructure.Testing.TestSetFixture"
    public class TestSetFixture
    {
        [TestSet]
        public void Test([NotNull] string name, [NotNull] string cmdLine)
            => _facade.Execute(new {name, cmdLine }, () => TestSetUtility.RunTestItem(name, cmdLine));

        private static readonly TestFacade _facade = TestFacade.GetCurrentClassFacade();
    }
}
