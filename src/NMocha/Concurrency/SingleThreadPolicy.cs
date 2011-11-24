using System;
using System.Threading;

namespace NMocha.Concurrency {
    public class SingleThreadPolicy : IThreadingPolicy {
        private readonly Thread testThread;

        public SingleThreadPolicy() {
            testThread = Thread.CurrentThread;
        }

        public void SynchronizeAction(Action action) {
            if(testThread != Thread.CurrentThread)
                throw new ConcurrentModificationException();
            action();
        }
    }
}