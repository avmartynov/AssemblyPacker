using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Twidlle.Infrastructure.CodeAnnotation;

namespace Twidlle.Infrastructure.Testing
{
    /// <summary> Процедуры, помогающие проводить нагрузочные испытания (измерения). </summary>
    /// 
    public static class Loading
    {
        /// <summary> Измеряет производительсность выполнения действия. </summary>
        [NotNull]
        public static Task<double> StartMeasureTps(Action measure, CancellationToken ct)
        {
            return Task.Factory.StartNew(() => MeasureTps(measure, ct), ct, 
                                         TaskCreationOptions.LongRunning, 
                                         TaskScheduler.Default);
        }

        /// <summary> Подсчитывает, сколько раз выполняется действие, за время пока не сработал CancellationToken </summary>
        /// <remarks>Бывает нужно для проведения относительных нагрузочных испытаний (для сравнения производительности).</remarks>
        [NotNull]
        public static Task<int> StartMeasureCountOf(Action measure, CancellationToken ct)
        {
            return Task.Factory.StartNew(() => CountOf(measure, ct), ct, 
                                         TaskCreationOptions.LongRunning, 
                                         TaskScheduler.Default);
        }

        private static int CountOf(Action action, CancellationToken ct)
        {
            var count = 0;
            while (!ct.IsCancellationRequested)
            {
                action();
                count++;
            }
            return count;
        }

        private static double MeasureTps(Action action, CancellationToken ct)
        {
            var sw = Stopwatch.StartNew();
            var count = CountOf(action, ct);
            return count / sw.Elapsed.TotalSeconds;
        }
    }
}
