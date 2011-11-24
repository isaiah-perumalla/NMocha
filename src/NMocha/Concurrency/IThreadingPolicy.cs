using System;

namespace NMocha.Concurrency {
    public interface IThreadingPolicy {
        void SynchronizeAction(Action action);
       
    }
}