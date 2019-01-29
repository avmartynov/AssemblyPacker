using System;

namespace Twidlle.Infrastructure
{
    /// <inheritdoc />
    /// <summary> Натуральное (природное, естественное, настоящее) время. </summary>
    public class NaturalDateTimeProvider : IAutoDateTimeProvider
    {
        public DateTime Now => DateTime.Now;

        public double FlowTimeFactor => 1.0;
    }
}
