using System;

namespace Twidlle.Infrastructure.Testing
{
    /// <summary> Поставщик текущего времени с управлением временем в ручную. </summary>
    public class ManualDateTimeProvider: IDateTimeProvider
    {
        public DateTime Now
            => _virtualCurrentTime;

        public static void SetNaturalCurrentTime()
            => _virtualCurrentTime = DateTime.Now;

        public static void SetCurrentTime(DateTime dateTime)
            => _virtualCurrentTime = dateTime;

        public static void Add(TimeSpan span)
            => _virtualCurrentTime = _virtualCurrentTime + span;

        public static void AddDays(double value)
            => _virtualCurrentTime = _virtualCurrentTime + TimeSpan.FromDays(value);

        public static void AddHours(double value)
            => _virtualCurrentTime = _virtualCurrentTime + TimeSpan.FromHours(value);

        public static void AddMilliseconds(double value)
            => _virtualCurrentTime = _virtualCurrentTime + TimeSpan.FromMilliseconds(value);

        public static void AddMinutes(double value)
            => _virtualCurrentTime = _virtualCurrentTime + TimeSpan.FromMinutes(value);

        public static void AddSeconds(double value)
            => _virtualCurrentTime = _virtualCurrentTime + TimeSpan.FromSeconds(value);

        public static void AddTicks(long value)
            => _virtualCurrentTime = _virtualCurrentTime + TimeSpan.FromTicks(value);

        private static DateTime _virtualCurrentTime = DateTime.Now;
    }
}
