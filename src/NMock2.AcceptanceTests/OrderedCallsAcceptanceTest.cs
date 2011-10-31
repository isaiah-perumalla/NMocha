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
using NMock2.Internal;
using NUnit.Framework;

namespace NMock2.AcceptanceTests {
    [TestFixture]
    public class OrderedCallsAcceptanceTest : AcceptanceTestBase {
        #region Setup/Teardown

        [SetUp]
        public void SetUp() {
            base.Setup();

            speaker = (ISpeaker) Mocks.NewInstanceOfRole(typeof (ISpeaker));
        }

        #endregion

        private ISpeaker speaker;

        [Test]
        public void AllowsCallsIfCalledInSameOrderAsExpectedWithinAnInOrderBlock() {
            using (Mocks.Ordered)
            {
                Expect.Once.On(speaker).Message("Hello");
                Expect.Once.On(speaker).Message("Goodbye");
            }

            speaker.Hello();
            speaker.Goodbye();
        }

        [Test]
        [ExpectedException(typeof (ExpectationException))]
        public void CallsInOrderedBlocksThatAreNotMatchedFailVerification() {
            SkipVerificationForThisTest();

            using (Mocks.Ordered)
            {
                Expect.Once.On(speaker).Message("Hello");
                Expect.Once.On(speaker).Message("Goodbye");
            }

            speaker.Hello();

            Mocks.VerifyAllExpectationsHaveBeenMet();
        }

        [Test]
        public void CallsWithinAnInOrderedBlockCanBeExpectedMoreThanOnce() {
            using (Mocks.Ordered)
            {
                Expect.Once.On(speaker).Message("Hello");
                Expect.AtLeastOnce.On(speaker).Message("Err");
                Expect.Once.On(speaker).Message("Goodbye");
            }

            speaker.Hello();
            speaker.Err();
            speaker.Err();
            speaker.Goodbye();
        }

        [Test]
        public void CanExpectUnorderedCallsWithinAnOrderedSequence() {
            using (Mocks.Ordered)
            {
                Expect.Once.On(speaker).Message("Hello");
                using (Mocks.Unordered)
                {
                    Expect.Once.On(speaker).Message("Umm");
                    Expect.Once.On(speaker).Message("Err");
                }
                Expect.Once.On(speaker).Message("Goodbye");
            }

            speaker.Hello();
            speaker.Err();
            speaker.Umm();
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
        public void EnforcesTheOrderOfCallsWithinAnInOrderBlock() {
            SkipVerificationForThisTest();

            using (Mocks.Ordered)
            {
                Expect.Once.On(speaker).Message("Hello");
                Expect.Once.On(speaker).Message("Goodbye");
            }

            speaker.Goodbye();
            speaker.Hello();
        }

        [Test, ExpectedException(typeof (ExpectationException))]
        public void UnorderedCallsWithinAnInOrderedBlockCannotBeCalledAfterTheEndOfTheUnorderedExpectations() {
            SkipVerificationForThisTest();

            using (Mocks.Ordered)
            {
                Expect.Once.On(speaker).Message("Hello");
                using (Mocks.Unordered)
                {
                    Expect.Once.On(speaker).Message("Umm");
                    Expect.Once.On(speaker).Message("Err");
                }
                Expect.Once.On(speaker).Message("Goodbye");
            }

            speaker.Hello();
            speaker.Err();
            speaker.Goodbye();
            speaker.Umm();
        }

        [Test, ExpectedException(typeof (ExpectationException))]
        public void UnorderedCallsWithinAnInOrderedBlockCannotBeCalledBeforeTheStartOfTheUnorderedExpectations() {
            SkipVerificationForThisTest();

            using (Mocks.Ordered)
            {
                Expect.Once.On(speaker).Message("Hello");
                using (Mocks.Unordered)
                {
                    Expect.Once.On(speaker).Message("Umm");
                    Expect.Once.On(speaker).Message("Err");
                }
                Expect.Once.On(speaker).Message("Goodbye");
            }

            speaker.Err();
            speaker.Hello();
            speaker.Umm();
            speaker.Goodbye();
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