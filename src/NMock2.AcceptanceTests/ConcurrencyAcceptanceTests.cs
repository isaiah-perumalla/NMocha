using System;
using System.Threading;
using NMock2.AcceptanceTests;
using NUnit.Framework;

namespace NMocha.AcceptanceTests {
    
    [TestFixture]
    public class ConcurrencyAcceptanceTests {
    

        [Test]
        public void ByDefaultShouldNotAllowInvocationsFromMultipleThreads() {
            var mockery = new Mockery();
            var blitzer = new Blitzer(16);

            var mock = mockery.NewInstanceOfRole<ISpeaker>();
            Expect.Exactly(blitzer.TotalActionCount()).On(mock).Message("Hello");
            try
            {
                blitzer.Blitz(mock.Hello);
                Assert.Fail("should not allow invocation from multiple threads by default");
            }
            catch(ConcurrentModificationException e)
            {
                
            }

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

        private Action DecorateAction(Action action, CountdownEvent countdownLatch) {
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