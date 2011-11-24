//-----------------------------------------------------------------------
// <copyright file="MockeryAcceptanceTest.cs" company="NMock2">
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
using System.Collections;
using System.ComponentModel.Design;
using NMocha.Internal;
using NMock2;
using NMock2.Monitoring;
using NUnit.Framework;

namespace NMocha.AcceptanceTests {
    [TestFixture]
    public class MockeryAcceptanceTest : AcceptanceTestBase {
        #region Setup/Teardown

        [TearDown]
        public void TearDown() {
         
        }

        #endregion

        [TestFixtureSetUp]
        public void TestFixtureSetUp() {
            SkipVerificationForThisFixture();
        }

        [Test, Ignore("do we need this behaviour")]
        public void CallingVerifyOnMockeryShouldEnableMockeryToBeUsedSuccessfullyForOtherTests() {
            var mockWithUninvokedExpectations = (IMockedType) Mockery.NewInstanceOfRole(typeof (IMockedType));
            Expect.Once.On(mockWithUninvokedExpectations).Message("Method").WithNoArguments();
            try
            {
                Mockery.VerifyAllExpectationsHaveBeenMet();
                Assert.Fail("Expected ExpectationException to be thrown");
            }
            catch (ExpectationException expected)
            {
                Assert.IsTrue(expected.Message.IndexOf("not all expected invocations were performed") != -1);
            }

            var mockWithInvokedExpectations = (IMockedType) Mockery.NewInstanceOfRole(typeof (IMockedType));
            Expect.Once.On(mockWithInvokedExpectations).Message("Method").WithNoArguments();
            mockWithInvokedExpectations.Method();
            Mockery.VerifyAllExpectationsHaveBeenMet();
        }

        [Test, Class]
        public void CanMakeMultipleCallsToImplementingWhenCreatingMock() {
            var mock =
                Mockery.NewInstanceOfRole<IMockedType>(DefinedAs.Implementing<IEnumerable>().Implementing<IDisposable>());

            Assert.That(mock, NUnit.Framework.Is.InstanceOf(typeof (IMockedType)));
            Assert.That(mock, NUnit.Framework.Is.InstanceOf( typeof(IEnumerable)));
            Assert.That(mock, NUnit.Framework.Is.InstanceOf(typeof (IDisposable)));
        }

        [Test]
        public void ChangingDefaultMockObjectFactoryChangesBehaviourOfNewMockeryInstances() {
            var mocksA = new Mockery();
            mocksA.SetMockFactoryAs(new TestingMockObjectFactoryA());
            Assert.AreEqual("TestingMockObjectFactoryA", mocksA.NewInstanceOfRole<INamed>().GetName());

            
            mocksA.SetMockFactoryAs(new TestingMockObjectFactoryB());
            Assert.AreEqual("TestingMockObjectFactoryB", mocksA.NewInstanceOfRole<INamed>().GetName());
        }

       

        [Test]
        public void MockObjectsMayBePlacedIntoServiceContainers() {
            var container = new ServiceContainer();
            var mockedType = Mockery.NewInstanceOfRole(typeof (IMockedType)) as IMockedType;

            container.AddService(typeof (IMockedType), mockedType);

            Assert.AreSame(mockedType, container.GetService(typeof (IMockedType)));
        }

        [Test, ExpectedException(typeof (InvalidOperationException))]
        public void SpecifyingConstructorArgsTwiceWhenCreatingMockThrowsInvalidOperationException() {
            Mockery.NewInstanceOfRole<SomeBaseClass>(DefinedAs.WithArgs("ABC").WithArgs("DEF"));
        }

        [Test, ExpectedException(typeof (InvalidOperationException))]
        public void SpecifyingNameTwiceWhenCreatingMockThrowsInvalidOperationException() {
            Mockery.NewInstanceOfRole<IMockedType>(DefinedAs.Named("A").Named("B"));
        }
    }

    public interface IMockedType {
        void Method();
    }

    public interface INamed {
        string GetName();
    }

    public class Named : INamed {
        private readonly string name;

        public Named(string name) {
            this.name = name;
        }

        #region INamed Members

        public string GetName() {
            return name;
        }

        #endregion
    }

    public class SomeBaseClass {
        public SomeBaseClass(string input) {
        }
    }

    public class TestingMockObjectFactoryA : IMockObjectFactory {
        #region IMockObjectFactory Members

        public object CreateMock(Mockery mockery, CompositeType mockedTypes, string name, object[] constructorArgs) {
            return new Named(GetType().Name);
        }

        #endregion
    }

    public class TestingMockObjectFactoryB : TestingMockObjectFactoryA {
    }
}