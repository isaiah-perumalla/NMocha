using System;
using System.Threading;
using NMocha.Internal;
using NMocha.Utils;

namespace NMocha.Concurrency {
    public class Synchronizer : IThreadingPolicy
    {
        private readonly object sync = new object();

        public void SynchronizeAction(Action action) {
            lock(sync)
            {
                try
                {
                    action();
                }
                finally
                {
                    Monitor.PulseAll(sync);
                }
            }
        }

        public void WaitUntil(IStatePredicate predicate) {
            WaitUntil(predicate, Timeout.Infinite);
        }

        private void WaitUntil(IStatePredicate predicate, Timeout timeout) {
            lock (sync)
            {
                while (!predicate.IsActive())
                {
                    if (timeout.HasTimedOut)
                    {
                        throw new TimeoutException(string.Format("timed out waiting for {0}", StringDescription.Describe(predicate)));
                    }
                    Monitor.Wait(sync, timeout.TimeRemaining);
                }
            }
        }
    }

    public struct Timeout {
        private readonly Func<TimeSpan> timeRemaining;
        public static Timeout Infinite = new Timeout(() => 1.Seconds());
        private static readonly TimeSpan zero = new TimeSpan(0,0,0,0);

        private Timeout(Func<TimeSpan> timeRemaining) {
            this.timeRemaining = timeRemaining;
        }


        public bool HasTimedOut
        {
            get { return timeRemaining() <= zero; }
        }

        public TimeSpan TimeRemaining
        {
            get { return timeRemaining(); }
        }
    }
}