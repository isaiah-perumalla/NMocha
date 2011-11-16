using System;
using System.Threading;
using NMocha.Concurrency;
using NMock2.AcceptanceTests;
using NUnit.Framework;

namespace NMocha.AcceptanceTests {
    [TestFixture]
    public class ConcurrencyAcceptanceTests {
        [Test]
        public void ByDefaultShouldNotAllowInvocationsFromMultipleThreads() {
            var mockery = new Mockery();
            var blitzer = new Blitzer(16);
            var numberOfconcurrentExceptions = new AtomicInt(0);
            var mock = mockery.NewInstanceOfRole<ISpeaker>();
            Expect.Exactly(blitzer.TotalActionCount()).On(mock).Message("Hello");
            blitzer.Blitz(() => {
                              try
                              {
                                  mock.Hello();
                              }
                              catch (ConcurrentModificationException)
                              {
                                  numberOfconcurrentExceptions.Increment();
                              }
                          });

            Assert.AreEqual(numberOfconcurrentExceptions.Value, 16, "should intercept invocation from non test thread");
        }

        [Test]
        public void AllowsMultipleThreadToInvokeMock() {
            var mockery = new Mockery();
            IThreadingPolicy synchronizer = new Synchronizer();
            mockery.SetThreadingPolicy(synchronizer);

            var blitzer = new Blitzer(16);

            var mock = mockery.NewInstanceOfRole<ISpeaker>();
            Expect.Exactly(blitzer.TotalActionCount()).On(mock).Message("Hello");
            blitzer.Blitz(mock.Hello);

            mockery.VerifyAllExpectationsHaveBeenMet();
        }
    }

    public class AtomicInt {
        private int i;

        public AtomicInt(int initialValue) {
            i = initialValue;
        }

        public int Value {
            get { return i; }
        }

        public void Increment() {
            Interlocked.Increment(ref i);
        }
    }


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
            var runInNewThread = DecorateAction(action, countdownLatch);

            for (var i = 0; i < numberOfaction; i++)
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