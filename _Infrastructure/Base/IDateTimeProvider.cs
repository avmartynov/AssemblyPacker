using System;
using Twidlle.Infrastructure.CodeAnnotation;

namespace Twidlle.Infrastructure
{
    /// <summary> Поставщик времени в системе. </summary>
    public interface IDateTimeProvider
    {
        DateTime Now { get; }
    }


    /// <inheritdoc />
    /// <summary> Поставщик времени в системе с автоматическим, равномерным движением времени. </summary>
    public interface IAutoDateTimeProvider : IDateTimeProvider 
    {
        /// <summary> Скорость движения времени относительно естественного времени. </summary>
        double FlowTimeFactor { get; }
    }


    public static class AutoDateTimeProviderExtensions
    {
        public static TimeSpan TimeSpanFromDays([NotNull] this IAutoDateTimeProvider timeProvider, double value)
            => TimeSpan.FromDays(value / timeProvider.FlowTimeFactor);

        public static TimeSpan TimeSpanFromHours([NotNull] this IAutoDateTimeProvider timeProvider, double value)
            => TimeSpan.FromHours(value / timeProvider.FlowTimeFactor);

        public static TimeSpan TimeSpanFromSeconds([NotNull] this IAutoDateTimeProvider timeProvider, double value)
            => TimeSpan.FromSeconds(value / timeProvider.FlowTimeFactor);

        public static TimeSpan TimeSpanFromMinutes([NotNull] this IAutoDateTimeProvider timeProvider, double value)
            => TimeSpan.FromMinutes(value / timeProvider.FlowTimeFactor);

        public static TimeSpan TimeSpanFromMilliseconds([NotNull] this IAutoDateTimeProvider timeProvider, double value)
            => TimeSpan.FromMilliseconds(value / timeProvider.FlowTimeFactor);

        public static TimeSpan TimeSpanFromTicks([NotNull] this IAutoDateTimeProvider timeProvider, long value)
            => TimeSpan.FromTicks((long)(value / timeProvider.FlowTimeFactor));

        public static TimeSpan FromNaturalTimeSpan([NotNull] this IAutoDateTimeProvider timeProvider, TimeSpan value)
            => TimeSpan.FromMilliseconds(value.TotalMilliseconds / timeProvider.FlowTimeFactor);
    }
}
