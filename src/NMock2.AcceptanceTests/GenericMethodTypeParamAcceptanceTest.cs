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
namespace NMock2.AcceptanceTests
{
    using NMock2.Internal;
    using NUnit.Framework;

    /// <summary>
    /// Acceptance tests for generic method type parameters.
    /// <see cref="GenericMethodParameterAcceptanceTest"/> for acceptance tests
    /// about generic method parameters.
    /// </summary>
    [TestFixture]
    public class GenericMethodTypeParamAcceptanceTest : AcceptanceTestBase
    {
        [Test]
        public void CanMockGenericMethodWithSpecifiedTypeParameterOnInterface()
        {
            AssertCanMockGenericMethodWithSpecifiedTypeParameter(Mocks.NewInstanceOfRole<IGenericHelloWorld>());
        }

        [Test, Class]
        public void CanMockGenericMethodWithSpecifiedTypeParameterOnClass()
        {
            AssertCanMockGenericMethodWithSpecifiedTypeParameter(Mocks.NewInstanceOfRole<GenericHelloWorld>());
        }

        private void AssertCanMockGenericMethodWithSpecifiedTypeParameter(IGenericHelloWorld genericHelloWorld)
        {
            const int iValue = 3;
            const string sValue = "test";

            Stub.On(genericHelloWorld).Message("Find", typeof(int)).With().Will(Return.Value(iValue));
            Stub.On(genericHelloWorld).Message("Find", typeof(string)).Will(Return.Value(sValue));
            Stub.On(genericHelloWorld).Message("Find", typeof(IHelloWorld), typeof(bool)).Will(Return.Value(Mocks.NewInstanceOfRole<IHelloWorld>()));

            string s = genericHelloWorld.Find<string>();
            int i = genericHelloWorld.Find<int>();
            IHelloWorld helloWorld = genericHelloWorld.Find<IHelloWorld, bool>();

            Assert.AreEqual(iValue, i);
            Assert.AreEqual(sValue, s);
            Assert.IsNotNull(helloWorld);
        }

        [Test]
        public void CanMockGenericMethodWithUnspecifiedTypeParameterOnInterface()
        {
            AssertCanMockGenericMethodWithUnspecifiedTypeParameter(Mocks.NewInstanceOfRole<IGenericHelloWorld>());
        }

        [Test, Class]
        public void CanMockGenericMethodWithUnspecifiedTypeParameterOnClass()
        {
            AssertCanMockGenericMethodWithUnspecifiedTypeParameter(Mocks.NewInstanceOfRole<GenericHelloWorld>());
        }

        private void AssertCanMockGenericMethodWithUnspecifiedTypeParameter(IGenericHelloWorld helloWorld)
        {
            const int iValue = 3;

            Stub.On(helloWorld).Message("Find").Will(Return.Value(iValue));

            int i = helloWorld.Find<int>();

            Assert.AreEqual(iValue, i);
        }

        [Test]
        public void CanMockGenericMethodWithMultipleTypeParametersOnInterface()
        {
            AssertCanMockGenericMethodWithMultipleTypeParameters(Mocks.NewInstanceOfRole<IGenericHelloWorld>());
        }

        [Test, Class]
        public void CanMockGenericMethodWithMultipleTypeParametersOnClass()
        {
            AssertCanMockGenericMethodWithMultipleTypeParameters(Mocks.NewInstanceOfRole<GenericHelloWorld>());
        }

        private void AssertCanMockGenericMethodWithMultipleTypeParameters(IGenericHelloWorld genericHelloWorld)
        {
            Stub.On(genericHelloWorld).Message("Cast", typeof(int), typeof(string)).With(3).Will(Return.Value("three"));

            string s = genericHelloWorld.Cast<int, string>(3);

            Assert.AreEqual("three", s);
        }


        [Test]
        public void HasCorrectErrorMessageOnUnexpectedInvocationOnInterface()
        {
            AssertHasCorrectErrorMessageOnUnexpectedInvocation(Mocks.NewInstanceOfRole<IGenericHelloWorld>());
        }

        [Test, Class]
        public void HasCorrectErrorMessageOnUnexpectedInvocationOnClass()
        {
            AssertHasCorrectErrorMessageOnUnexpectedInvocation(Mocks.NewInstanceOfRole<GenericHelloWorld>());
        }

        private void AssertHasCorrectErrorMessageOnUnexpectedInvocation(IGenericHelloWorld helloWorld)
        {
            SkipVerificationForThisTest();
            
            try
            {
                helloWorld.Find<int, bool>();

                Assert.Fail("An ExpectationException should have been thrown");
            }
            catch (ExpectationException ex)
            {
                Assert.AreEqual("unexpected invocation of genericHelloWorld.Find<System.Int32, System.Boolean>()\r\nExpected:\r\n", ex.Message);
            }
        }

        [Test]
        public void HasCorrectErrorMessageOnNotMetExpectationOnInterface()
        {
            AssertHasCorrectErrorMessageOnNotMetExpectation(Mocks.NewInstanceOfRole<IGenericHelloWorld>());
        }

        [Test, Class]
        public void HasCorrectErrorMessageOnNotMetExpectationOnClass()
        {
            AssertHasCorrectErrorMessageOnNotMetExpectation(Mocks.NewInstanceOfRole<GenericHelloWorld>());
        }

        private void AssertHasCorrectErrorMessageOnNotMetExpectation(IGenericHelloWorld helloWorld)
        {
            SkipVerificationForThisTest();

            Expect.Once.On(helloWorld).Message("Find", typeof(int)).Will(Return.Value(3));

            try
            {
                Mocks.VerifyAllExpectationsHaveBeenMet();

                Assert.Fail("An ExpectationException should have been thrown");
            }
            catch (ExpectationException ex)
            {
                Assert.AreEqual("not all expected invocations were performed\r\nExpected:\r\n  1 time: genericHelloWorld.Find<System.Int32>(any arguments), will return <3> [called 0 times]\r\n", ex.Message);
            }
        }
    }
}