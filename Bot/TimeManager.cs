using System;
using System.Threading;
using System.Threading.Tasks;

namespace PluginBot
{
    public static class TimeManager
    {
        internal static event EventHandler OnDayStart;
        internal static event EventHandler OnNightStart;
#if DEBUG
        public static TimeSpan DaySpan => TimeSpan.FromMinutes(2);
        public static TimeSpan NightSpan => TimeSpan.FromMinutes(2);
#else
        public static TimeSpan DaySpan { get; set; }
        public static TimeSpan NightSpan => TimeSpan.FromDays(1).Subtract(DaySpan);
#endif
        private static Task TimeWorker { get; set; }
        private static CancellationTokenSource cancellationToken;
        public static void Start()
        {
            cancellationToken = new CancellationTokenSource();
            TimeWorker = TimerTask();
        }

        public static void Stop()
        {
            cancellationToken.Cancel();
        }

        private static async Task TimerTask()
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(DaySpan, cancellationToken.Token);
                OnDayStart?.Invoke(null, new EventArgs());
                if (cancellationToken.IsCancellationRequested) return;
                await Task.Delay(NightSpan, cancellationToken.Token);
                OnNightStart?.Invoke(null, new EventArgs());
                if (cancellationToken.IsCancellationRequested) return;
            }
        }
    }
}