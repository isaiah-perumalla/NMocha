//-----------------------------------------------------------------------
// <copyright file="GenericMethodTypeParamAcceptanceTest.cs" company="NMock2">
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
    /// <summary>
    /// Acceptance tests for generic method type parameters.
    /// <see cref="GenericMethodParameterAcceptanceTest"/> for acceptance tests
    /// about generic method parameters.
    /// </summary>
    [TestFixture]
    public class GenericMethodTypeParamAcceptanceTest : AcceptanceTestBase {
        private void AssertCanMockGenericMethodWithSpecifiedTypeParameter(IGenericSpeaker genericSpeaker) {
            const int iValue = 3;
            const string sValue = "test";

            Stub.On(genericSpeaker).Message("Find", typeof (int)).With().Will(Return.Value(iValue));
            Stub.On(genericSpeaker).Message("Find", typeof (string)).Will(Return.Value(sValue));
            Stub.On(genericSpeaker).Message("Find", typeof (ISpeaker), typeof (bool)).Will(
                Return.Value(Mockery.NewInstanceOfRole<ISpeaker>()));

            var s = genericSpeaker.Find<string>();
            var i = genericSpeaker.Find<int>();
            ISpeaker speaker = genericSpeaker.Find<ISpeaker, bool>();

            Assert.AreEqual(iValue, i);
            Assert.AreEqual(sValue, s);
            Assert.IsNotNull(speaker);
        }

        private void AssertCanMockGenericMethodWithUnspecifiedTypeParameter(IGenericSpeaker speaker) {
            const int iValue = 3;

            Stub.On(speaker).Message("Find").Will(Return.Value(iValue));

            var i = speaker.Find<int>();

            Assert.AreEqual(iValue, i);
        }

        private void AssertCanMockGenericMethodWithMultipleTypeParameters(IGenericSpeaker genericSpeaker) {
            Stub.On(genericSpeaker).Message("Cast", typeof (int), typeof (string)).With(3).Will(Return.Value("three"));

            string s = genericSpeaker.Cast<int, string>(3);

            Assert.AreEqual("three", s);
        }


        private void AssertHasCorrectErrorMessageOnUnexpectedInvocation(IGenericSpeaker speaker) {
            SkipVerificationForThisTest();

            try
            {
                speaker.Find<int, bool>();

                Assert.Fail("An ExpectationException should have been thrown");
            }
            catch (ExpectationException ex)
            {
                Assert.AreEqual(
                    "unexpected invocation of genericSpeaker.Find<System.Int32, System.Boolean>()\r\nexpectations:\r\n",
                    ex.Message);
            }
        }

        private void AssertHasCorrectErrorMessageOnNotMetExpectation(IGenericSpeaker speaker) {
            SkipVerificationForThisTest();

            Expect.Once.On(speaker).Message("Find", typeof (int)).Will(Return.Value(3));

            try
            {
                Mockery.VerifyAllExpectationsHaveBeenMet();

                Assert.Fail("An ExpectationException should have been thrown");
            }
            catch (ExpectationException ex)
            {
                Assert.AreEqual(
                    "not all expected invocations were performed\r\nexpectations:\r\n  expected once, never invoked: genericSpeaker.Find<System.Int32>(any arguments), will return <3>\r\n",
                    ex.Message);
            }
        }

        [Test, Class]
        public void CanMockGenericMethodWithMultipleTypeParametersOnClass() {
            AssertCanMockGenericMethodWithMultipleTypeParameters(Mockery.NewInstanceOfRole<GenericSpeaker>());
        }

        [Test]
        public void CanMockGenericMethodWithMultipleTypeParametersOnInterface() {
            AssertCanMockGenericMethodWithMultipleTypeParameters(Mockery.NewInstanceOfRole<IGenericSpeaker>());
        }

        [Test, Class]
        public void CanMockGenericMethodWithSpecifiedTypeParameterOnClass() {
            AssertCanMockGenericMethodWithSpecifiedTypeParameter(Mockery.NewInstanceOfRole<GenericSpeaker>());
        }

        [Test]
        public void CanMockGenericMethodWithSpecifiedTypeParameterOnInterface() {
            AssertCanMockGenericMethodWithSpecifiedTypeParameter(Mockery.NewInstanceOfRole<IGenericSpeaker>());
        }

        [Test, Class]
        public void CanMockGenericMethodWithUnspecifiedTypeParameterOnClass() {
            AssertCanMockGenericMethodWithUnspecifiedTypeParameter(Mockery.NewInstanceOfRole<GenericSpeaker>());
        }

        [Test]
        public void CanMockGenericMethodWithUnspecifiedTypeParameterOnInterface() {
            AssertCanMockGenericMethodWithUnspecifiedTypeParameter(Mockery.NewInstanceOfRole<IGenericSpeaker>());
        }

        [Test, Class]
        public void HasCorrectErrorMessageOnNotMetExpectationOnClass() {
            AssertHasCorrectErrorMessageOnNotMetExpectation(Mockery.NewInstanceOfRole<GenericSpeaker>());
        }

        [Test]
        public void HasCorrectErrorMessageOnNotMetExpectationOnInterface() {
            AssertHasCorrectErrorMessageOnNotMetExpectation(Mockery.NewInstanceOfRole<IGenericSpeaker>());
        }

        [Test, Class]
        public void HasCorrectErrorMessageOnUnexpectedInvocationOnClass() {
            AssertHasCorrectErrorMessageOnUnexpectedInvocation(Mockery.NewInstanceOfRole<GenericSpeaker>());
        }

        [Test]
        public void HasCorrectErrorMessageOnUnexpectedInvocationOnInterface() {
            AssertHasCorrectErrorMessageOnUnexpectedInvocation(Mockery.NewInstanceOfRole<IGenericSpeaker>());
        }
    }
}