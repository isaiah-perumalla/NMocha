using System;
using System.Threading;

namespace NMocha.AcceptanceTests.Concurrency {
    public class Blitzer {
        private readonly int numberOfaction;

        public Blitzer(int numberOfaction) {
            this.numberOfaction = numberOfaction;
        }

        public int TotalActionCount() {
            return numberOfaction;
        }

        public void Blitz(Action action) {
            var countdownLatch = new CountdownEvent(numberOfaction);
            Action runInNewThread = DecorateAction(action, countdownLatch);

            for (int i = 0; i < numberOfaction; i++)
            {
                var thread = new Thread(new ThreadStart(runInNewThread));
                thread.Start();
            }
            countdownLatch.Wait();
        }

        private static Action DecorateAction(Action action, CountdownEvent countdownLatch) {
            return () => {
                       try
                       {
                           action();
                       }
                       finally
                       {
                           countdownLatch.Signal();
                       }
                   };
        }
    }
}