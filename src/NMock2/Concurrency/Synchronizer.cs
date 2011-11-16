using System;

namespace NMocha.Concurrency {
    public class Synchronizer : IThreadingPolicy
    {
        private readonly object sync = new object();

        public void SynchronizeAction(Action action) {
            lock(sync)
            {
                action();
            }
        }
    }
}