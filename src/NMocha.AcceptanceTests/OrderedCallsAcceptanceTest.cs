//-----------------------------------------------------------------------
// <copyright file="OrderedCallsAcceptanceTest.cs" company="NMock2">
//
//   http://www.sourceforge.net/projects/NMock2
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
// </copyright>
// This is the easiest way to ignore StyleCop rules on this file, even if we shouldn't use this tag:
// <auto-generated />
//-----------------------------------------------------------------------
using NMocha.Internal;
using NMock2;
using NUnit.Framework;

namespace NMocha.AcceptanceTests {
    [TestFixture]
    public class OrderedCallsAcceptanceTest : AcceptanceTestBase {
        #region Setup/Teardown

        [SetUp]
        public void SetUp() {
            base.Setup();

            speaker = (ISpeaker) Mockery.NewInstanceOfRole(typeof (ISpeaker));
        }

        #endregion

        private ISpeaker speaker;

        [Test]
        public void AllowsExpectationsInSpecifiedSeq() {
            ISequence seq = Mockery.Sequence("s");
            Expect.Once.On(speaker).Message("Hello").InSequence(seq);
            Expect.Once.On(speaker).Message("Goodbye").InSequence(seq);


            speaker.Hello();
            speaker.Goodbye();
        }


        [Test]
        public void DoesNotEnforceTheOrderOfCallsByDefault() {
            Expect.Once.On(speaker).Message("Hello");
            Expect.Once.On(speaker).Message("Goodbye");

            speaker.Goodbye();
            speaker.Hello();
        }

        [Test, ExpectedException(typeof (ExpectationException))]
        public void EnforcesTheOrderOfCallsWithinASequence() {
            SkipVerificationForThisTest();
            ISequence seq = Mockery.Sequence("s");
            Expect.Once.On(speaker).Message("Hello").InSequence(seq);
            Expect.Once.On(speaker).Message("Goodbye").InSequence(seq);


            speaker.Goodbye();
            speaker.Hello();
        }

        [Test]
        public void ExpectationCanBelongToMoreThanOneSequence() {
            SkipVerificationForThisTest();
            ISequence seqA = Mockery.Sequence("seqA");
            ISequence seqB = Mockery.Sequence("seqB");

            Expect.Once.On(speaker).Message("Hello").InSequence(seqA);
            Expect.Once.On(speaker).Message("Umm").InSequence(seqB);
            Expect.Once.On(speaker).Message("Err");

            Expect.Once.On(speaker).Message("Ask").With("name please ?")
                .InSequence(seqA)
                .InSequence(seqB);

            speaker.Err();
            speaker.Hello();
            try
            {
                speaker.Ask("name please ?");
                Assert.Fail("Expectation Error should have been thrown");
            }

            catch (ExpectationException e)
            {
                Assert.That(e.Message.Contains("in sequence seqA "), string.Format("error message was {0}", e.Message));
                Assert.That(e.Message.Contains("in sequence seqB "), string.Format("error message was {0}", e.Message));
            }
        }

        [Test]
        public void ExpectationIncludesSequenceInTheDescription() {
            SkipVerificationForThisTest();
            ISequence seq = Mockery.Sequence("s");
            Expect.Once.On(speaker).Message("Hello").InSequence(seq);
            Expect.Once.On(speaker).Message("Goodbye").InSequence(seq);

            try
            {
                speaker.Goodbye();
                Assert.Fail("Should have thrown expectation exception");
            }
            catch (ExpectationException e)
            {
                Assert.That(e.Message.Contains("in sequence s "), string.Format("error message was {0}", e.Message));
            }
        }


        [Test]
        public void UnorderedExpectationsMatchInOrderOfSpecification() {
            Expect.Once.On(speaker).Message("Ask").With(Is.Anything).Will(Return.Value("1"));
            Expect.Once.On(speaker).Message("Ask").With(Is.Anything).Will(Return.Value("2"));

            Assert.AreEqual("1", speaker.Ask("ignored"), "first call");
            Assert.AreEqual("2", speaker.Ask("ignored"), "second call");
        }
    }
}