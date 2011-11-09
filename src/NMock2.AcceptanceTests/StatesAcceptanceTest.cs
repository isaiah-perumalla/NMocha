using System;
using NMocha.Internal;
using NMock2.AcceptanceTests;
using NUnit.Framework;

namespace NMock2 {
    [TestFixture]
    public class StatesAcceptanceTest : AcceptanceTestBase {
        #region Setup/Teardown

        [SetUp]
        public void Before() {
            speaker = Mockery.NewInstanceOfRole<ISpeaker>();
        }

        #endregion

        private ISpeaker speaker;

        [Test]
        public void AllowsExpectationsToOccurInCorrectState() {
            IStates readiness = Mockery.States("readiness");
            Expect.Once.On(speaker).Message("Hello").When(readiness.Is("ready"));
            Expect.Once.On(speaker).Message("Umm").Then(readiness.Is("ready"));

            speaker.Umm();
            speaker.Hello();
        }

        [Test]
        [ExpectedException(typeof (ExpectationException))]
        public void CanConstrainExpectationToOccurOnlyInAGivenState() {
            SkipVerificationForThisTest();
            IStates readiness = Mockery.States("readiness");

            Expect.Once.On(speaker).Message("Hello").When(readiness.Is("ready"));
            Expect.Once.On(speaker).Message("Umm").Then(readiness.Is("ready"));

            speaker.Hello();
            speaker.Umm();
            Mockery.VerifyAllExpectationsHaveBeenMet();
        }

        [Test]
        [ExpectedException(typeof (ExpectationException))]
        public void CanConstrainExpectionToAllStateConstraints() {
            SkipVerificationForThisTest();
            IStates readiness = Mockery.States("readiness");
            readiness.StartAs("ready");
            IStates fruitiness = Mockery.States("fruitiness");

            Expect.Once.On(speaker).Message("Hello").When(readiness.Is("ready"))
                .When(fruitiness.Is("apple"));
            speaker.Hello();
        }

        [Test]
        public void CanStartInASpecificState() {
            IStates readiness = Mockery.States("readiness");
            readiness.StartAs("ready");
            Stub.On(speaker).Message("Hello").When(readiness.Is("ready"));
            speaker.Hello();
        }

        [Test]
        public void ErrorMessageShowsCurrentStates() {
            SkipVerificationForThisTest();
            IStates fruitness = Mockery.States("fruitness");
            fruitness.StartAs("apple");
            IStates vegginess = Mockery.States("veginess");
            vegginess.StartAs("Carrot");

            Stub.On(speaker).Message("Hello").When(fruitness.IsNot("apple"));
            try
            {
                speaker.Hello();
                Assert.Fail("should have failed with Expectation Exception");
            }
            catch (ExpectationException e)
            {
                Console.WriteLine(e.Message);
                Assert.That(e.Message.Contains("veginess is Carrot"),
                            "should contain veggieness is Carrot but  msg was '{0}'", e.Message);
                Assert.That(e.Message.Contains("fruitness is not apple"),
                            "should contain fruitness is not apple but  msg was '{0}'", e.Message);
            }
        }

        [Test]
        public void ErrorReportShowAllSideEffectsOfExpectedInvocation() {
            SkipVerificationForThisTest();
            IStates fruitness = Mockery.States("fruitness");
            fruitness.StartAs("apple");

            Expect.On(speaker).Message("Hello").When(fruitness.IsNot("apple"))
                .Then(fruitness.Is("orange"));
            try
            {
                speaker.Hello();
                Assert.Fail("should have failed with Expectation Exception");
            }
            catch (ExpectationException e)
            {
                Assert.That(e.Message.Contains("when fruitness is not apple"),
                            "should containwhen fruitness is not apple but  msg was '{0}'", e.Message);
                Assert.That(e.Message.Contains("then fruitness is orange"),
                            "should contain then fruitness is orange but  msg was '{0}'", e.Message);
            }
        }

        [Test]
        public void TransitionsStateAfterExceptionIsThrow() {
            IStates readiness = Mockery.States("readiness");

            Expect.Once.On(speaker).Message("Hello").Will(Throw.Exception(new TestException()))
                .Then(readiness.Is("ready"));
            Expect.Once.On(speaker).Message("Umm").When(readiness.Is("ready"));

            try
            {
                speaker.Hello();
            }
            catch (TestException)
            {
                speaker.Umm();
            }
        }
    }

    public class TestException : Exception {
    }
}