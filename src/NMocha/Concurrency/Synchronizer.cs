using System;
using System.Threading;
using NMocha.Internal;
using NMocha.Utils;

namespace NMocha.Concurrency {
    public class Synchronizer : IThreadingPolicy
    {
        private readonly object sync = new object();
        private Exception firstException;

        public void SynchronizeAction(Action action) {
            lock(sync)
            {
                try
                {
                    action();
                }
                catch(Exception e)
                {
                    if (firstException == null) firstException = e;
                    throw;

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

        public void WaitUntil(IStatePredicate predicate, Timeout timeout) {
           
            lock (sync)
            {
                for (; ; )
                {
                    if (firstException != null) throw firstException;
                    if (predicate.IsActive()) break;
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

        public static Timeout After(TimeSpan timeout) {
            var start = DateTime.Now;
            return new Timeout(() => timeout - (DateTime.Now - start));
        }
    }
}