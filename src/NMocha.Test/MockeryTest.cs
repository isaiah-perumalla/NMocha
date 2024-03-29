//-----------------------------------------------------------------------
// <copyright file="MockeryTest.cs" company="NMock2">
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
using System.IO;
using System.Reflection;
using NMocha;
using NMocha.Internal;
using NMocha.Monitoring;
using NMock2.Internal;
using NMock2.Monitoring;
using NUnit.Framework;

namespace NMock2.Test {
    [TestFixture]
    public class MockeryTest {
        #region Setup/Teardown

        [SetUp]
        public void SetUp() {
            _mockery = new Mockery();
            _mock = (IMockedType) _mockery.NewNamedInstanceOfRole(typeof (IMockedType), "mock");

            _expectation1 = new MockExpectation();
            _expectation2 = new MockExpectation();
        }

        [TearDown]
        public void TearDown() {
           
        }

        #endregion

        public interface IMockedType {
            void DoStuff();
        }

        public interface InterfaceWithoutIPrefix {
        }

        public interface ARSEIInterfaceWithAdditionalPrefixBeforeI {
        }

        public interface INTERFACE_WITH_UPPER_CLASS_NAME {
        }

        private Mockery _mockery;
        private IMockedType _mock;
        private MockExpectation _expectation1;
        private MockExpectation _expectation2;

        private void AddExpectationsToMockery() {
            var mockObjectControl = (IMockObject) _mock;
            mockObjectControl.AddExpectation(_expectation1);
            mockObjectControl.AddExpectation(_expectation2);
        }

        // AKR Code provided by Adrian Krummenacher, Roche Rotkreuz
        public interface IMockObjectWithGenericMethod : IMockedType {
            T Find<T>();
            T Search<T>(T instance, string Name);
        }

        // AKR Code provided by Adrian Krummenacher, Roche Rotkreuz

        public interface IParentInterface {
            void DoSomething();
        }

        public interface IChildInterface : IParentInterface {
        }

        public class TestingMockObjectFactoryWithNoDefaultConstructor : IMockObjectFactory {
            public TestingMockObjectFactoryWithNoDefaultConstructor(string someArg) {
            }

            #region IMockObjectFactory Members

            public object CreateMock(Mockery mockery, CompositeType mockedTypes, string name, object[] constructorArgs) {
                return null;
            }

            #endregion
        }

        public sealed class SampleSealedClass {
            public int Add(int a, int b) {
                return a + b;
            }
        }

        public class SampleClass {
            public int Add(int a, int b) {
                return DoAdd(a, b);
            }

            protected virtual int DoAdd(int a, int b) {
                return a + b;
            }

            public virtual int Divide(int a, int b, out decimal remainder) {
                remainder = a%b;

                return a/b;
            }
        }

        public class SampleClassWithConstructorArgs {
            public SampleClassWithConstructorArgs(int i, string s) {
            }
        }

        public class SampleGenericClass<T> {
            public virtual T GetDefault() {
                return default(T);
            }
        }

        public class SampleClassWithGenericMethods {
            public virtual string GetStringValue<T>(T input) {
                return input.ToString();
            }

            public virtual int GetCount<T>(T list) where T : IList {
                return list.Count;
            }
        }

        [Test]
        public void CanCreateClassMockWithConstructor() {
            var mock = _mockery.NewInstanceOfRole<SampleClassWithConstructorArgs>(1, "A");
        }

        [Test]
        public void CanCreateMockWithoutCastingBySpecifingTypeAsGenericParameter() {
            var mock = _mockery.NewInstanceOfRole<IMockedType>();
            Console.WriteLine("Generic mock create test ran");
        }

        [Test]
        public void CanMockClassWithGenericMethods() {
            var mock = _mockery.NewInstanceOfRole<SampleClassWithGenericMethods>();
        }

        [Test]
        public void CanMockGenericClass() {
            var mock = _mockery.NewInstanceOfRole<SampleGenericClass<string>>();
        }

 
        [Test]
        public void ClassMockReturnsDefaultNameFromMockNameProperty() {
            var mock = (IMockObject) _mockery.NewInstanceOfRole<SampleClass>();

            Assert.AreEqual("sampleClass", mock.MockName);
        }

        [Test]
        public void CreateMocksWithGenericMethod() {
            object mock = _mockery.NewInstanceOfRole(typeof (IMockObjectWithGenericMethod));
        }

        [Test]
        public void CreatedClassMockComparesReferenceIdentityWithEqualsMethod() {
            object mock1 = _mockery.NewInstanceOfRole(typeof (SampleClass));
            object mock2 = _mockery.NewInstanceOfRole(typeof (SampleClass));

            Assert.IsTrue(mock1.Equals(mock1), "same object should be equal");
            Assert.IsFalse(mock1.Equals(mock2), "different objects should not be equal");
        }

        [Test]
        public void CreatedMockComparesReferenceIdentityWithEqualsMethod() {
            object mock1 = _mockery.NewNamedInstanceOfRole(typeof (IMockedType), "mock1");
            object mock2 = _mockery.NewNamedInstanceOfRole(typeof (IMockedType), "mock2");

            Assert.IsTrue(mock1.Equals(mock1), "same object should be equal");
            Assert.IsFalse(mock1.Equals(mock2), "different objects should not be equal");
        }

        [Test]
        public void CreatedMockReturnsNameFromToString() {
            object mock1 = _mockery.NewNamedInstanceOfRole(typeof (IMockedType), "mock1");
            object mock2 = _mockery.NewNamedInstanceOfRole(typeof (IMockedType), "mock2");

            Assert.AreEqual("mock1", mock1.ToString(), "mock1.ToString()");
            Assert.AreEqual("mock2", mock2.ToString(), "mock2.ToString()");
        }

        [Test]
        public void CreatesClassMocksThatCanBeCastToIMockObject() {
            object mock = _mockery.NewInstanceOfRole<SampleClass>();

            Assert.IsTrue(mock is IMockObject, "should be instance of IMock");
        }

        [Test]
        public void CreatesMocksThatCanBeCastToIMockObject() {
            object mock = _mockery.NewInstanceOfRole(typeof (IMockedType));

            Assert.IsTrue(mock is IMockObject, "should be instance of IMock");
        }

        [Test]
        public void CreatesMocksThatCanBeCastToMockedType() {
            object mock = _mockery.NewInstanceOfRole(typeof (IMockedType));

            Assert.IsTrue(mock is IMockedType, "should be instance of mocked type");
        }

        [Test, ExpectedException(typeof (ExpectationException))]
        public void DetectsWhenFirstExpectationHasNotBeenMet() {
            AddExpectationsToMockery();
            _expectation1.HasBeenMet = false;
            _expectation2.HasBeenMet = true;
            _mockery.VerifyAllExpectationsHaveBeenMet();
        }

        [Test, ExpectedException(typeof (ExpectationException))]
        public void DetectsWhenSecondExpectationHasNotBeenMet() {
            AddExpectationsToMockery();
            _expectation1.HasBeenMet = true;
            _expectation2.HasBeenMet = false;
            _mockery.VerifyAllExpectationsHaveBeenMet();
        }

        [Test]
        public void DispatchesInvocationBySearchingForMatchingExpectationInOrderOfAddition() {
            AddExpectationsToMockery();

            _expectation2.Previous = _expectation1;

            _expectation1.ExpectedInvokedObject = _expectation2.ExpectedInvokedObject = _mock;
            _expectation1.ExpectedInvokedMethod = _expectation2.ExpectedInvokedMethod =
                                                  typeof (IMockedType).GetMethod("DoStuff", new Type[0]);
            _expectation1.Matches_Result = false;
            _expectation2.Matches_Result = true;

            _mock.DoStuff();

            Assert.IsTrue(_expectation1.Matches_HasBeenCalled, "should have tried to match expectation1");
            Assert.IsFalse(_expectation1.Perform_HasBeenCalled, "should not have performed expectation1");

            Assert.IsTrue(_expectation2.Matches_HasBeenCalled, "should have tried to match expectation2");
            Assert.IsTrue(_expectation2.Perform_HasBeenCalled, "should have performed expectation2");
        }

        [Test, ExpectedException(typeof (ExpectationException))]
        public void FailsTestIfNoExpectationsMatch() {
            AddExpectationsToMockery();
            _expectation1.Matches_Result = false;
            _expectation2.Matches_Result = false;
            _mock.DoStuff();
        }

        [Test]
        public void GivesMocksDefaultNameIfNoNameSpecified() {
            Assert.AreEqual("mockedType", _mockery.NewInstanceOfRole(typeof (IMockedType)).ToString());
            Assert.AreEqual("interfaceWithoutIPrefix",
                            _mockery.NewInstanceOfRole(typeof (InterfaceWithoutIPrefix)).ToString());
            Assert.AreEqual("interfaceWithAdditionalPrefixBeforeI",
                            _mockery.NewInstanceOfRole(typeof (ARSEIInterfaceWithAdditionalPrefixBeforeI)).ToString());
            Assert.AreEqual("interface_with_upper_class_name",
                            _mockery.NewInstanceOfRole(typeof (INTERFACE_WITH_UPPER_CLASS_NAME)).ToString());
        }

        [Test]
        public void MockReturnsNameFromMockNameProperty() {
            var mock = (IMockObject) _mockery.NewNamedInstanceOfRole(typeof (IMockedType), "mock");

            Assert.AreEqual("mock", mock.MockName);
        }

        [Test]
        public void MockReturnsNameFromToString() {
            object mock = _mockery.NewNamedInstanceOfRole(typeof (IMockedType), "mock");

            Assert.AreEqual("mock", mock.ToString());
        }

        [Test]
        [ExpectedException(typeof (ArgumentException))]
        public void MockingSealedClassThrowsArgumentException() {
            var mock = _mockery.NewInstanceOfRole<SampleSealedClass>();
        }

        [Test]
        public void ShouldBeAbleToInvokeMethodOnInheritedInterface() {
            var mockery = new Mockery();
            var childMock = (IChildInterface) mockery.NewInstanceOfRole(typeof (IChildInterface));

            Expect.AtLeastOnce.On(childMock).Message("DoSomething");
            childMock.DoSomething();
            mockery.VerifyAllExpectationsHaveBeenMet();
        }

        [Test]
        public void StopsSearchingForMatchingExpectationAsSoonAsOneMatches() {
            AddExpectationsToMockery();
            _expectation2.Previous = _expectation1;

            _expectation1.ExpectedInvokedObject = _expectation2.ExpectedInvokedObject = _mock;
            _expectation1.ExpectedInvokedMethod = _expectation2.ExpectedInvokedMethod =
                                                  typeof (IMockedType).GetMethod("DoStuff", new Type[0]);
            _expectation1.Matches_Result = true;

            _mock.DoStuff();

            Assert.IsTrue(_expectation1.Matches_HasBeenCalled, "should have tried to match expectation1");
            Assert.IsTrue(_expectation1.Perform_HasBeenCalled, "should have performed expectation1");

            Assert.IsFalse(_expectation2.Matches_HasBeenCalled, "should not have tried to match expectation2");
            Assert.IsFalse(_expectation2.Perform_HasBeenCalled, "should not have performed expectation2");
        }

        [Test]
        public void VerifiesWhenAllExpectationsHaveBeenMet() {
            AddExpectationsToMockery();
            _expectation1.HasBeenMet = true;
            _expectation2.HasBeenMet = true;
            _mockery.VerifyAllExpectationsHaveBeenMet();
        }
    }

    internal class MockExpectation : IExpectation {
        public string Description = "";
        public MethodInfo ExpectedInvokedMethod;
        public object ExpectedInvokedObject;

        public bool Matches_HasBeenCalled;
        public bool Matches_Result;
        public bool Perform_HasBeenCalled;
        public MockExpectation Previous;

        #region IExpectation Members

        /// <summary>
        /// Checks whether stored expectations matches the specified invocation.
        /// </summary>
        /// <param name="invocation">The invocation to check.</param>
        /// <returns>Returns whether one of the stored expectations has met the specified invocation.</returns>
        public bool Matches(Invocation invocation) {
            CheckInvocation(invocation);
            Assert.IsTrue(Previous == null || Previous.Matches_HasBeenCalled,
                          "called out of order");
            Matches_HasBeenCalled = true;
            return Matches_Result;
        }

        public bool MatchesIgnoringIsActive(Invocation invocation) {
            throw new NotImplementedException();
        }

        public void Perform(Invocation invocation) {
            CheckInvocation(invocation);
            Assert.IsTrue(Matches_HasBeenCalled, "Matches should have been called");
            Perform_HasBeenCalled = true;
        }

        public void DescribeActiveExpectationsTo(IDescription writer) {
            writer.AppendText(Description);
        }

        public void DescribeUnmetExpectationsTo(IDescription writer) {
            writer.AppendText(Description);
        }

        public bool IsActive { get; set; }

        public bool HasBeenMet { get; set; }

        #endregion

        private void CheckInvocation(Invocation invocation) {
            Assert.IsTrue(ExpectedInvokedObject == null || ExpectedInvokedObject == invocation.Receiver,
                          "should have received invocation on expected object");
            Assert.IsTrue(ExpectedInvokedMethod == null || ExpectedInvokedMethod == invocation.Method,
                          "should have received invocation of expected method");
        }
    }
}