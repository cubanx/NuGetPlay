using System;
using System.Diagnostics;

namespace ClassLibrary1
{
    public static class Retry
    {
        public static void Until(Action stuffToRetry, Func<bool> stopWhenTrue, TimeSpan? timeToRetry = null)
        {
            var internalTimeToRetry = timeToRetry ?? 5.Seconds();

            var startTime = DateTime.Now;
            Func<bool> timeSinceStart = () => DateTime.Now - startTime > internalTimeToRetry;
            var timeIsUp = timeSinceStart();

            do
            {
                timeIsUp = timeSinceStart();
                stuffToRetry();
            } while (!timeIsUp && !stopWhenTrue());
        }
    }

    public class Tests
    {
        public void IfTrueRightAway()
        {
            var timesTried = 0;
            Retry.Until(() => timesTried++, () => true);
            Debug.Assert(timesTried == 1);
        }

        public void IfTrueAfterOneSecondAway()
        {
            Debug.Assert(TryFor(10.Milliseconds()) >= 1);
        }


        public void IfNeverTrue()
        {
            var startTime = DateTime.Now;
            var timesTried = TryFor(10.Seconds());
            var elapsedTime = DateTime.Now - startTime;
            Console.WriteLine(elapsedTime);
            Debug.Assert(elapsedTime < 5500.Milliseconds());
            Debug.Assert(timesTried > 1);
        }
        private static int TryFor(TimeSpan timeSpan)
        {
            var timesTried = 0;
            var startTime = DateTime.Now;
            Retry.Until(() => timesTried++, () => DateTime.Now - startTime > timeSpan);
            Console.WriteLine(timesTried);
            return timesTried;
        }

    }
}

namespace System
{
    public static class TimeSpanExtensions
    {
        public static TimeSpan Seconds(this int length)
        {
            return TimeSpan.FromSeconds(length);
        }

        public static TimeSpan Minutes(this int length)
        {
            return TimeSpan.FromMinutes(length);
        }

        public static TimeSpan Milliseconds(this int length)
        {
            return TimeSpan.FromMilliseconds(length);
        }

        public static TimeSpan Days(this int length)
        {
            return TimeSpan.FromDays(length);
        }

        public static TimeSpan Hours(this int length)
        {
            return TimeSpan.FromHours(length);
        }
    }
}
