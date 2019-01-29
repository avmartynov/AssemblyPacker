using System;
using System.Threading;

namespace Twidlle.Infrastructure.Testing
{
    /// <inheritdoc />
    /// <summary> Поставщик времени в системе с автоматическим
    /// ускоренным или замедленным движением времени, основанный
    /// на масштабировании естественного течения времени. </summary>
    public class AutoDateTimeProvider : IAutoDateTimeProvider
    {
        public AutoDateTimeProvider(double flowTimeFactor, DateTime startTime)
        {
            _startTime = startTime;
            _flowTimeFactor = flowTimeFactor;
        }


        public AutoDateTimeProvider(double flowTimeFactor) 
            : this (flowTimeFactor, DateTime.Now)
        {
        }


        public void SleepSeconds(double v)
            => Thread.Sleep(this.TimeSpanFromSeconds(v));


        public void SleepMinutes(double v)
            => Thread.Sleep(this.TimeSpanFromMinutes(v));


        public DateTime Now
            => _startTime + TimeSpan.FromMilliseconds((DateTime.Now - _startTime).TotalMilliseconds * FlowTimeFactor);

        public double FlowTimeFactor 
            => _flowTimeFactor;

        private static DateTime _startTime;
        private static double _flowTimeFactor;
    }
}
