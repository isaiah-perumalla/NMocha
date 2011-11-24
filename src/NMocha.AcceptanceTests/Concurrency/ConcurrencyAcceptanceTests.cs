using System;
using System.Threading;
using NMocha.Concurrency;
using NMocha.Internal;
using NMocha.Utils;
using NMock2.AcceptanceTests;
using NUnit.Framework;
using Timeout = NMocha.Concurrency.Timeout;

namespace NMocha.AcceptanceTests.Concurrency {
    [TestFixture]

    
    public class ConcurrencyAcceptanceTests {
        private Mockery mockery;
        private Synchronizer synchronizer;

        [SetUp]
        public void BeforeTest() {
            mockery = new Mockery();
            synchronizer = new Synchronizer();
            mockery.SetThreadingPolicy(synchronizer);

        }

        [Test]
        public void ByDefaultShouldNotAllowInvocationsFromMultipleThreads() {
            mockery = new Mockery();
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
           
            var blitzer = new Blitzer(16);

            var mock = mockery.NewInstanceOfRole<ISpeaker>();
            Expect.Exactly(blitzer.TotalActionCount()).On(mock).Message("Hello");
            blitzer.Blitz(mock.Hello);

            mockery.VerifyAllExpectationsHaveBeenMet();
        }

        [Test]
        public void CanWaitUntilStateMachineIsInAGivenState()
        {
            var threads = mockery.States("threads");

            var blitzer = new Blitzer(5);
            var count = new AtomicInt(5);
            var mock = mockery.NewInstanceOfRole<ISpeaker>();
            Expect.Exactly(blitzer.TotalActionCount()).On(mock).Message("Hello");
            Expect.Once.On(mock).Message("Goodbye").Then(threads.Is("finished"));
            OnNewThread(() => blitzer.Blitz(() => {
                                                mock.Hello();
                                                if (count.Decrement() == 0) mock.Goodbye();
                                            }));

            synchronizer.WaitUntil(threads.Is("finished"));
            mockery.VerifyAllExpectationsHaveBeenMet();
        }


        [Test]
        public void ThrowsExpectationExceptionIfExpecatationViolationInBackgroundWhileWaitingForAGivenState() {
            var threads = mockery.States("threads");

            var blitzer = new Blitzer(5);
            var count = new AtomicInt(5);
            var mock = mockery.NewInstanceOfRole<ISpeaker>();
            
            Expect.Once.On(mock).Message("Goodbye").Then(threads.Is("finished"));
            OnNewThread(() => blitzer.Blitz(() =>
            {
                if (count.Decrement() == 0) mock.Goodbye();
                else
                mock.Hello();
               
            }));

            try
            {
                synchronizer.WaitUntil(threads.Is("finished"));
                Assert.Fail("shold have thrown expectation error when unexpected invocation in background");
            }
             catch(ExpectationException e)
             {
                Assert.That(e.Message, NUnit.Framework.Is.StringContaining("Hello()"));
             }
         }
       
        private static void OnNewThread(Action action) {
            ThreadPool.QueueUserWorkItem(x => action());
        }

        [Test, ExpectedException(typeof(TimeoutException))]
        public void TimeoutIfStateMachineDoesNotEnterSpecifiedStateWithinTimeout() {

            var threads = mockery.States("threads");
           
            var mock = mockery.NewInstanceOfRole<ISpeaker>();

            Expect.Once.On(mock).Message("Goodbye").Then(threads.Is("finished"));
            
            synchronizer.WaitUntil(threads.Is("finished"), Timeout.After(2.Seconds()));
            
        }
    }

    public struct AtomicInt {
        private int i;

        public AtomicInt(int initialValue) {
            i = initialValue;
        }

        public int Value {
            get { return i; }
        }

        public int Increment() {
            return Interlocked.Increment(ref i);
        }

        public int Decrement() {
            return Interlocked.Decrement(ref i);
        }
    }
}