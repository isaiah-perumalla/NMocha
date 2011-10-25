using NMock2.AcceptanceTests;
using NMock2.Internal;
using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace NMock2 {

    [TestFixture]
    public class StatesAcceptanceTest : AcceptanceTestBase {
        
        [Test]
        [ExpectedException(typeof(ExpectationException))]
        public void CanConstrainExpectationToOccurOnlyInAGivenState() {
            this.SkipVerificationForThisTest();
            var readiness = Mocks.States("readiness");
            var helloWorld = Mocks.NewInstanceOfRole<IHelloWorld>();
            Expect.Once.On(helloWorld).Message("Hello").When(readiness.Is("ready"));
            Expect.Once.On(helloWorld).Message("Umm").Then(readiness.Is("ready"));

            helloWorld.Hello();
            helloWorld.Umm();
            Mocks.VerifyAllExpectationsHaveBeenMet();
            
        }

        [Test]
        public void AllowsExpectationsToOccurInCorrectState() {
            var readiness = Mocks.States("readiness");
            var helloWorld = Mocks.NewInstanceOfRole<IHelloWorld>();
            Expect.Once.On(helloWorld).Message("Hello").When(readiness.Is("ready"));
            Expect.Once.On(helloWorld).Message("Umm").Then(readiness.Is("ready"));
          
            helloWorld.Umm();
            helloWorld.Hello();
        }

        [Test]
        public void CanStartInASpecificState() {
            var readiness = Mocks.States("readiness");
            readiness.StartAs("ready");
            var helloWorld = Mocks.NewInstanceOfRole<IHelloWorld>();
            Stub.On(helloWorld).Message("Hello").When(readiness.Is("ready"));
            helloWorld.Hello();
           
        }

        [Test]
        public void EnsureErrorMessageIncludesCurrentStates() {
            this.SkipVerificationForThisTest();
            var fruitness = Mocks.States("fruitness");
            var vegginess = Mocks.States("veginess");
            vegginess.StartAs("Carrot");
            var helloWorld = Mocks.NewInstanceOfRole<IHelloWorld>();
            Stub.On(helloWorld).Message("Hello").When(fruitness.Is("apple"));
            try
            {
                helloWorld.Hello();
                Assert.Fail("should have failed with Expectation Exception");
            }
            catch(ExpectationException e)
            {
                Assert.That(e.Message.Contains("veginess is Carrot"), "should contain veggieness is Carrot but  msg was '{0}'", e.Message);
                Assert.That(e.Message.Contains("fruitness is apple"), "should contain fruitness is apple but  msg was '{0}'", e.Message);
               
            }


        }
    }
}