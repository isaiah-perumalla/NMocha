//-----------------------------------------------------------------------
// <copyright file="ExceptionsAreNotSwallowedTest.cs" company="NMock2">
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
    using Internal;
    using NUnit.Framework;

    /// <summary>
    /// Tests checking that tested code can not swallow NMock2 exceptions.
    /// </summary>
    [TestFixture]
    public class ExceptionsAreNotSwallowedTest : AcceptanceTestBase
    {
        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            SkipVerificationForThisFixture();
        }
        
        /// <summary>
        /// <see cref="ExpectationException"/>s are rethrown in <see cref="Mockery.VerifyAllExpectationsHaveBeenMet"/>.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ExpectationException))]
        public void UnexpectedInvocationExceptionsAreRethrownInVerify()
        {
            IHelloWorld mock = Mocks.NewInstanceOfRole<IHelloWorld>();

            try
            {
                mock.Ahh();
            }
            catch (ExpectationException)
            {
                // evil code >:-]
            }

            Mocks.VerifyAllExpectationsHaveBeenMet();
        }

        /// <summary>
        /// Exceptions are rethrown only once.
        /// </summary>
        [Test]
        public void UnexpectedInvocationExceptionIsClearedAfterBeingThrownInVerify()
        {
            IHelloWorld mock = Mocks.NewInstanceOfRole<IHelloWorld>();

            try
            {
                   mock.Ahh();
            }
            catch (ExpectationException)
            {
                   // evil code >:-]
            }

            try
            {
                // Exception should be initially rethrown here...
                Mocks.VerifyAllExpectationsHaveBeenMet();
            }
            catch (ExpectationException)
            {
            }

            // It should not be rethrown again...
            Mocks.VerifyAllExpectationsHaveBeenMet();
        }

        /// <summary>
        /// The first swallowed exception is thrown.
        /// </summary>
        [Test]
        public void FirstSwallowedUnexpectedInvocationExceptionIsRethrownInVerify()
        {
            IHelloWorld mock = Mocks.NewInstanceOfRole<IHelloWorld>();
            ExpectationException firstException = null;

            try
            {
                   mock.Ahh();
            }
            catch (ExpectationException ex)
            {
                   // evil code >:-]
                   firstException = ex;
            }

            try
            {
                   mock.Ahh();
            }
            catch (ExpectationException)
            {
                   // more evil code >:-]
            }

            try
            {
               Mocks.VerifyAllExpectationsHaveBeenMet();
            }
            catch (ExpectationException rethrownException)
            {
                   Assert.AreSame(
                       firstException, 
                       rethrownException, 
                       "Expected first unexpected invocation exception to be rethrown");

                   return;
            }

            Assert.Fail("Expected ExpectationException to be rethrown");
        }
    }
}