//-----------------------------------------------------------------------
// <copyright file="ErrorCheckingAcceptanceTest.cs" company="NMock2">
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
using System;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace NMock2.AcceptanceTests {
    [TestFixture]
    public class ErrorCheckingAcceptanceTest : AcceptanceTestBase {
        [TestFixtureSetUp]
        public void TestFixtureSetUp() {
            SkipVerificationForThisFixture();
        }

        public interface IMyHelloWorld {
            bool IsPrime(int number);
        }

        public interface IOperator {
            IChild Get();
        }

        public interface IBase {
        }

        public interface IChild : IBase {
        }

        public interface InterfaceWithToStringMethod {
            string ToString();
        }

        [Test, ExpectedException(typeof (ArgumentException))]
        public void CannotExpectAMethodOnAnRealObject() {
            var realObject = new object();
            Expect.Once.On(realObject).Message(Is.Anything);
        }

        [Test, ExpectedException(typeof (ArgumentException))]
        public void CannotExpectAMethodThatDoesNotExistInTheMockedType() {
            var speaker = (ISpeaker) Mockery.NewInstanceOfRole(typeof (ISpeaker));

            Expect.Once.On(speaker).Message("NonexistentMethod");
        }

        [Test, ExpectedException(typeof (ArgumentException))]
        public void CannotExpectAMethodWithAnInvalidName() {
            var speaker = (ISpeaker) Mockery.NewInstanceOfRole(typeof (ISpeaker));

            Expect.Once.On(speaker).Message("Invalid Name!");
        }

        [Test, ExpectedException(typeof (ArgumentException))]
        public void CannotExpectGetOfAnInvalidProperty() {
            var speaker = (ISpeaker) Mockery.NewInstanceOfRole(typeof (ISpeaker));

            Expect.Once.On(speaker).GetProperty("NonexistentProperty");
        }

        [Test, ExpectedException(typeof (ArgumentException))]
        public void CannotExpectGetOfIndexerIfNoIndexerInMockedType() {
            var speaker = (ISpeaker) Mockery.NewInstanceOfRole(typeof (ISpeaker));

            Expect.Once.On(speaker).Get["arg"].Will(Return.Value("something"));
        }

        [Test, ExpectedException(typeof (ArgumentException))]
        public void CannotExpectSetOfAnInvalidProperty() {
            var speaker = (ISpeaker) Mockery.NewInstanceOfRole(typeof (ISpeaker));

            Expect.Once.On(speaker).SetProperty("NonexistentProperty").To("something");
        }

        [Test, ExpectedException(typeof (ArgumentException))]
        public void CannotExpectSetOfIndexerIfNoIndexerInMockedType() {
            var speaker = (ISpeaker) Mockery.NewInstanceOfRole(typeof (ISpeaker));

            Expect.Once.On(speaker).Set["arg"].To("something");
        }


        [Test, ExpectedException(typeof (InvalidOperationException),
            "You have to set the return value for method 'IsPrime' on 'IMyHelloWorld' mock.")]
        public void CorrectMessageWhenReturnValueNotSet() {
            var myHelloWorld = (IMyHelloWorld) Mockery.NewInstanceOfRole(typeof (IMyHelloWorld));

            Expect.Once.On(myHelloWorld).Message("IsPrime");
                //.Will(Return.Value(true));//, new SetNamedParameterAction("number", 3)); //.With();

            int i = 3;
            bool result = myHelloWorld.IsPrime(i);
        }

        [Test]
        [ExpectedException(typeof (ArgumentException),
            "Interfaces must not contain a declaration for ToString().")]
        public void MockingInterfaceWithToStringMethodThrowsException() {
            Mockery.NewInstanceOfRole<InterfaceWithToStringMethod>();
        }

        [Test]
        public void NullReturnValue() {
            var speaker = Mockery.NewInstanceOfRole<ISpeaker>();

            Expect.Once.On(speaker).Message("Ask").Will(Return.Value(null));

            string s = speaker.Ask("What?");

            Assert.IsNull(s);
        }

        [Test]
        [ExpectedException(typeof (ArgumentException))]
        public void UnnecessaryReturnValue() {
            var speaker = Mockery.NewInstanceOfRole<ISpeaker>();

            Expect.Once.On(speaker).Message("Hello").Will(Return.Value("What?"));

            speaker.Hello();
        }

        [Test]
        public void WrongReturnType() {
            var o = Mockery.NewInstanceOfRole<IOperator>();
            var b = Mockery.NewInstanceOfRole<IBase>();

            Expect.Once.On(o).Message("Get").Will(Return.Value(b));

            try
            {
                o.Get();
            }
            catch (ArgumentException e)
            {
                // Make sure that exception message identifies type that was returned, and what was expected
                // (other interfaces will be present in type description, but we don't care too much about them here).
                Assert.IsTrue(Regex.IsMatch(e.Message,
                                            @".*NMock2\.AcceptanceTests\.ErrorCheckingAcceptanceTest\+IBase.*from a method returning NMock2\.AcceptanceTests\.ErrorCheckingAcceptanceTest\+IChild",
                                            RegexOptions.IgnoreCase),
                              "Exception message wrong: should contain text 'NMock2.AcceptanceTests.ErrorCheckingAcceptanceTest+IBase' as well as text 'from a method returning NMock2.AcceptanceTests.ErrorCheckingAcceptanceTest+IChild' but is: " +
                              e.Message);
            }
        }
    }
}