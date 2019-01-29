using System;
using NUnit.Framework;
using Twidlle.Infrastructure.CodeAnnotation;

namespace Twidlle.Infrastructure.Testing
{
    /// <inheritdoc />
    /// <summary> Связывает набор тестов с конфигурационной секцией. </summary>
    /// <remarks>Конфигурационная секция должна иметь тип "Twidlle.Infrastructure.CustomConfigSection, Twidlle.Infrastructure".
    /// </remarks>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class TestSetAttribute : TestCaseSourceAttribute
    {
        public TestSetAttribute([NotNull]string sectionName = "testSet")
            : base (typeof(TestSetUtility), nameof(TestSetUtility.GetItems), new object [] {sectionName})
        {
            if (string.IsNullOrEmpty(sectionName))
                throw new ArgumentNullException(nameof(sectionName));
        }
    }
}
