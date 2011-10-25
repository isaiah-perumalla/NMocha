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
namespace NMock2.Test
{
    using System;
using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using NUnit.Framework;
    using NMock2.Internal;
    using NMock2.Monitoring;

    [TestFixture]
    public class MockeryTest
    {
        public interface IMockedType
        {
            void DoStuff();
        }

        public interface InterfaceWithoutIPrefix
        {
        }

        public interface ARSEIInterfaceWithAdditionalPrefixBeforeI
        {
        }

        public interface INTERFACE_WITH_UPPER_CLASS_NAME
        {
        }

        private Mockery _mockery;
        IMockedType _mock;
        MockExpectation _expectation1;
        MockExpectation _expectation2;

        [SetUp]
        public void SetUp()
        {
            _mockery = new Mockery();
			_mock = (IMockedType)_mockery.NewNamedMock(typeof(IMockedType), "mock");
            
            _expectation1 = new MockExpectation();
            _expectation2 = new MockExpectation();
        }

		[TearDown]
		public void TearDown()
		{
			// We're mucking around with changing the default IMockObjectFactory in some tests
			// in this fixture. Here we restore things back to normal after each test.
			Mockery.ChangeDefaultMockObjectFactory(typeof(CastleMockObjectFactory));
		}

        private void AddExpectationsToMockery()
        {
            IMockObject mockObjectControl = (IMockObject)_mock;
            mockObjectControl.AddExpectation(_expectation1);
            mockObjectControl.AddExpectation(_expectation2);
        }

        [Test]
        public void CreatesMocksThatCanBeCastToMockedType()
        {
            object mock = _mockery.NewMock(typeof(IMockedType));

            Assert.IsTrue(mock is IMockedType, "should be instance of mocked type");
        }

        [Test]
        public void CanCreateMockWithoutCastingBySpecifingTypeAsGenericParameter()
        {
            IMockedType mock = _mockery.NewMock<IMockedType>();
            Console.WriteLine("Generic mock create test ran");
        }

        [Test]
        public void CreatesMocksThatCanBeCastToIMockObject()
        {
            object mock = _mockery.NewMock(typeof(IMockedType));

            Assert.IsTrue(mock is IMockObject, "should be instance of IMock");
        }

        // AKR Code provided by Adrian Krummenacher, Roche Rotkreuz
        public interface IMockObjectWithGenericMethod : IMockedType
        {
            T Find<T>();
            T Search<T>(T instance, string Name);
        }

        // AKR Code provided by Adrian Krummenacher, Roche Rotkreuz
        [Test]
        public void CreateMocksWithGenericMethod()
        {
            object mock = _mockery.NewMock(typeof(IMockObjectWithGenericMethod));
        }

        [Test]
        public void MockReturnsNameFromToString()
        {
			object mock = _mockery.NewNamedMock(typeof(IMockedType), "mock");

            Assert.AreEqual("mock", mock.ToString());
        }

		[Test]
		public void MockReturnsNameFromMockNameProperty()
		{
			IMockObject mock = (IMockObject)_mockery.NewNamedMock(typeof(IMockedType), "mock");

			Assert.AreEqual("mock", mock.MockName);
		}

        [Test]
        public void GivesMocksDefaultNameIfNoNameSpecified()
        {
            Assert.AreEqual("mockedType", _mockery.NewMock(typeof(IMockedType)).ToString());
            Assert.AreEqual("interfaceWithoutIPrefix", _mockery.NewMock(typeof(InterfaceWithoutIPrefix)).ToString());
            Assert.AreEqual("interfaceWithAdditionalPrefixBeforeI", _mockery.NewMock(typeof(ARSEIInterfaceWithAdditionalPrefixBeforeI)).ToString());
            Assert.AreEqual("interface_with_upper_class_name", _mockery.NewMock(typeof(INTERFACE_WITH_UPPER_CLASS_NAME)).ToString());
        }

        [Test]
        public void CreatedMockComparesReferenceIdentityWithEqualsMethod()
        {
			object mock1 = _mockery.NewNamedMock(typeof(IMockedType), "mock1");
			object mock2 = _mockery.NewNamedMock(typeof(IMockedType), "mock2");

            Assert.IsTrue(mock1.Equals(mock1), "same object should be equal");
            Assert.IsFalse(mock1.Equals(mock2), "different objects should not be equal");
        }

        [Test]
        public void CreatedMockReturnsNameFromToString()
        {
			object mock1 = _mockery.NewNamedMock(typeof(IMockedType), "mock1");
			object mock2 = _mockery.NewNamedMock(typeof(IMockedType), "mock2");

            Assert.AreEqual("mock1", mock1.ToString(), "mock1.ToString()");
            Assert.AreEqual("mock2", mock2.ToString(), "mock2.ToString()");
        }

        [Test]
        public void DispatchesInvocationBySearchingForMatchingExpectationInOrderOfAddition()
        {
            AddExpectationsToMockery();

            _expectation2.Previous = _expectation1;

            _expectation1.ExpectedInvokedObject = _expectation2.ExpectedInvokedObject = _mock;
            _expectation1.ExpectedInvokedMethod = _expectation2.ExpectedInvokedMethod =
                typeof(IMockedType).GetMethod("DoStuff", new Type[0]);
            _expectation1.Matches_Result = false;
            _expectation2.Matches_Result = true;

            _mock.DoStuff();

            Assert.IsTrue(_expectation1.Matches_HasBeenCalled, "should have tried to match expectation1");
            Assert.IsFalse(_expectation1.Perform_HasBeenCalled, "should not have performed expectation1");

            Assert.IsTrue(_expectation2.Matches_HasBeenCalled, "should have tried to match expectation2");
            Assert.IsTrue(_expectation2.Perform_HasBeenCalled, "should have performed expectation2");
        }

        [Test]
        public void StopsSearchingForMatchingExpectationAsSoonAsOneMatches()
        {
            AddExpectationsToMockery();
            _expectation2.Previous = _expectation1;

            _expectation1.ExpectedInvokedObject = _expectation2.ExpectedInvokedObject = _mock;
            _expectation1.ExpectedInvokedMethod = _expectation2.ExpectedInvokedMethod =
                typeof(IMockedType).GetMethod("DoStuff", new Type[0]);
            _expectation1.Matches_Result = true;

            _mock.DoStuff();

            Assert.IsTrue(_expectation1.Matches_HasBeenCalled, "should have tried to match expectation1");
            Assert.IsTrue(_expectation1.Perform_HasBeenCalled, "should have performed expectation1");

            Assert.IsFalse(_expectation2.Matches_HasBeenCalled, "should not have tried to match expectation2");
            Assert.IsFalse(_expectation2.Perform_HasBeenCalled, "should not have performed expectation2");
        }

        [Test, ExpectedException(typeof(NMock2.Internal.ExpectationException))]
        public void FailsTestIfNoExpectationsMatch()
        {
            AddExpectationsToMockery();
            _expectation1.Matches_Result = false;
            _expectation2.Matches_Result = false;
            _mock.DoStuff();
        }

        [Test]
        public void VerifiesWhenAllExpectationsHaveBeenMet()
        {
            AddExpectationsToMockery();
            _expectation1.HasBeenMet = true;
            _expectation2.HasBeenMet = true;
            _mockery.VerifyAllExpectationsHaveBeenMet();
        }
        
        [Test, ExpectedException(typeof(NMock2.Internal.ExpectationException))]
        public void DetectsWhenSecondExpectationHasNotBeenMet()
        {
            AddExpectationsToMockery();
            _expectation1.HasBeenMet = true;
            _expectation2.HasBeenMet = false;
            _mockery.VerifyAllExpectationsHaveBeenMet();
        }
        
        [Test, ExpectedException(typeof(NMock2.Internal.ExpectationException))]
        public void DetectsWhenFirstExpectationHasNotBeenMet()
        {
            AddExpectationsToMockery();
            _expectation1.HasBeenMet = false;
            _expectation2.HasBeenMet = true;
            _mockery.VerifyAllExpectationsHaveBeenMet();
        }

        [Test]
        public void AssertionExceptionThrownWhenSomeExpectationsHaveNotBeenMetContainsDescriptionOfUnMetExpectations()
        {
            IMockObject mockObjectControl = (IMockObject)_mock;

            MockExpectation expectation3 = new MockExpectation();

            _expectation1.Description = "expectation1";
            _expectation1.HasBeenMet = false;
            _expectation1.IsActive = true;
            _expectation2.Description = "expectation2";
            _expectation2.HasBeenMet = true;
            _expectation2.IsActive = true;
            expectation3.Description = "expectation3";
            expectation3.HasBeenMet = false;
            expectation3.IsActive = true;

            mockObjectControl.AddExpectation(_expectation1);
            mockObjectControl.AddExpectation(_expectation2);
            mockObjectControl.AddExpectation(expectation3);

            try
            {
                _mockery.VerifyAllExpectationsHaveBeenMet();
            }
            catch (ExpectationException e)
            {
                string newLine = Environment.NewLine;

                Assert.AreEqual(
                    "not all expected invocations were performed" + newLine +
                    "Expected:" + newLine +
                    "  expectation1" + newLine +
                    "  expectation3" + newLine,

                    e.Message);
            }
        }

        [Test]
        public void AssertionExceptionThrownWhenNoExpectationsMatchContainsDescriptionOfActiveExpectations()
        {
            IMockObject mockObjectControl = (IMockObject)_mock;

            MockExpectation expectation3 = new MockExpectation();

            _expectation1.Description = "expectation1";
            _expectation1.IsActive = true;
            _expectation1.Matches_Result = false;
            _expectation2.IsActive = false;
            _expectation2.Matches_Result = false;
            expectation3.Description = "expectation3";
            expectation3.IsActive = true;
            expectation3.Matches_Result = false;

            mockObjectControl.AddExpectation(_expectation1);
            mockObjectControl.AddExpectation(_expectation2);
            mockObjectControl.AddExpectation(expectation3);

            try
            {
                _mock.DoStuff();
            }
            catch (ExpectationException e)
            {
                string newLine = System.Environment.NewLine;

                Assert.AreEqual(
                    "unexpected invocation of mock.DoStuff()" + newLine +
                    "Expected:" + newLine +
                    "  expectation1" + newLine +
                    "  expectation3" + newLine,
                    e.Message);
            }
        }

        public interface IParentInterface
        {
            void DoSomething();
        }

        public interface IChildInterface : IParentInterface
        {
        }

        [Test]
        public void ShouldBeAbleToInvokeMethodOnInheritedInterface()
        {
            Mockery mockery = new Mockery();
            IChildInterface childMock = (IChildInterface)mockery.NewMock(typeof(IChildInterface));

            Expect.AtLeastOnce.On(childMock).Method("DoSomething");
            childMock.DoSomething();
            mockery.VerifyAllExpectationsHaveBeenMet();
		}

		public class TestingMockObjectFactoryWithNoDefaultConstructor : IMockObjectFactory
		{
			public TestingMockObjectFactoryWithNoDefaultConstructor(string someArg) { }

            public object CreateMock(Mockery mockery, CompositeType mockedTypes, string name, MockStyle mockStyle, object[] constructorArgs)
			{
				return null;
			}
		}

		[Test, ExpectedException(typeof(ArgumentException))]
		public void ChangingDefaultMockObjectFactoryToClassWithNoDefaultConstructorThrowsArgumentException()
		{
			Mockery.ChangeDefaultMockObjectFactory(typeof(TestingMockObjectFactoryWithNoDefaultConstructor));
		}

		[Test, ExpectedException(typeof(ArgumentException))]
		public void ChangingDefaultMockObjectFactoryToClassThatDoesnotImplementIMockObjectFactoryThrowsArgumentException()
		{
			Mockery.ChangeDefaultMockObjectFactory(this.GetType());
		}

        [Test]
        public void ClearExpectation()
        {
            Mockery testee = new Mockery();
            ISelfDescribing mock = testee.NewMock<ISelfDescribing>();

            Expect.Once.On(mock).Method("DescribeTo");

            testee.ClearExpectation(mock);

            testee.VerifyAllExpectationsHaveBeenMet();
        }

		#region Class mocking tests

		// TODO: Tests/impl for these cases:
		// - Not setting an expectation on abstract method of transparent proxy and calling it
		// - Setting an expectation on a sealed method
		// - Setting an expectation on a non-virtual method
		// - Bad constructor arguments (Better error for missing constructor signature)

		[Test]
		public void CreatesClassMocksThatCanBeCastToIMockObject()
		{
			object mock = _mockery.NewMock<SampleClass>();

			Assert.IsTrue(mock is IMockObject, "should be instance of IMock");
		}

		[Test]
		public void ClassMockReturnsDefaultNameFromMockNameProperty()
		{
			IMockObject mock = (IMockObject)_mockery.NewMock<SampleClass>();

			Assert.AreEqual("sampleClass", mock.MockName);
		}

		[Test]
		public void CreatedClassMockComparesReferenceIdentityWithEqualsMethod()
		{
			object mock1 = _mockery.NewMock(typeof(SampleClass));
			object mock2 = _mockery.NewMock(typeof(SampleClass));

			Assert.IsTrue(mock1.Equals(mock1), "same object should be equal");
			Assert.IsFalse(mock1.Equals(mock2), "different objects should not be equal");
		}

		[Test]
		public void CanCreateClassMockWithConstructor()
		{
			SampleClassWithConstructorArgs mock = _mockery.NewMock<SampleClassWithConstructorArgs>(1, "A");
		}

		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void MockingSealedClassThrowsArgumentException()
		{
			SampleSealedClass mock = _mockery.NewMock<SampleSealedClass>();
		}

		[Test]
		public void TransparentClassMockWithoutExpectationsExposesRealMethodImplementations()
		{
			SampleClass mock = _mockery.NewMock<SampleClass>(MockStyle.Transparent);

			Assert.AreEqual(3, mock.Add(1, 2));
		}

		[Test]
		public void CanMockGenericClass()
		{
			SampleGenericClass<string> mock = _mockery.NewMock<SampleGenericClass<string>>();
		}

		[Test]
		public void CanMockClassWithGenericMethods()
		{
			SampleClassWithGenericMethods mock = _mockery.NewMock<SampleClassWithGenericMethods>();
		}

		#endregion

		#region Testing classes for class mock tests

		public sealed class SampleSealedClass
		{
			public int Add(int a, int b)
			{
				return a + b;
			}
		}

		public class SampleClass
		{
			public int Add(int a, int b)
			{
				return DoAdd(a, b);
			}

			protected virtual int DoAdd(int a, int b)
			{
				return a + b;
			}

			public virtual int Divide(int a, int b, out decimal remainder)
			{
				remainder = a % b;

				return a / b;
			}
		}

		public class SampleClassWithConstructorArgs
		{
			public SampleClassWithConstructorArgs(int i, string s) { }
		}

		public class SampleGenericClass<T>
		{
			public virtual T GetDefault()
			{
				return default(T);
			}
		}

		public class SampleClassWithGenericMethods
		{
			public virtual string GetStringValue<T>(T input)
			{
				return input.ToString();
			}

			public virtual int GetCount<T>(T list) where T : IList
			{
				return list.Count;
			}
		}

		#endregion
	}

    class MockExpectation : IExpectation
    {
        public object ExpectedInvokedObject = null;
        public MethodInfo ExpectedInvokedMethod = null;

        public MockExpectation Previous;

        public bool Matches_Result = false;
        public bool Matches_HasBeenCalled = false;

        /// <summary>
        /// Checks whether stored expectations matches the specified invocation.
        /// </summary>
        /// <param name="invocation">The invocation to check.</param>
        /// <returns>Returns whether one of the stored expectations has met the specified invocation.</returns>
        public bool Matches(Invocation invocation)
        {
            CheckInvocation(invocation);
            Assert.IsTrue(Previous == null || Previous.Matches_HasBeenCalled,
                          "called out of order");
            Matches_HasBeenCalled = true;
            return Matches_Result;
        }

        public bool MatchesIgnoringIsActive(Invocation invocation)
        {
            throw new NotImplementedException();
        }

        public bool Perform_HasBeenCalled = false;

        public void Perform(Invocation invocation)
        {
            CheckInvocation(invocation);
            Assert.IsTrue(Matches_HasBeenCalled, "Matches should have been called");
            Perform_HasBeenCalled = true;
        }

        public string Description = "";

        public void DescribeActiveExpectationsTo(TextWriter writer)
        {
            writer.Write(Description);
        }

        public void DescribeUnmetExpectationsTo(TextWriter writer)
        {
            writer.Write(Description);
        }

        public void QueryExpectationsBelongingTo(IMockObject mock, IList<IExpectation> result)
        {
        }

        private bool isActive_Value = false;

        public bool IsActive
        {
            get { return isActive_Value; }
            set { isActive_Value = value; }
        }

        private bool hasBeenMet_Value = false;

        public bool HasBeenMet
        {
            get { return hasBeenMet_Value; }
            set { hasBeenMet_Value = value; }
        }

        private void CheckInvocation(Invocation invocation)
        {
            Assert.IsTrue(ExpectedInvokedObject == null || ExpectedInvokedObject == invocation.Receiver,
                          "should have received invocation on expected object");
            Assert.IsTrue(ExpectedInvokedMethod == null || ExpectedInvokedMethod == invocation.Method,
                "should have received invocation of expected method");
        }
    }
}
