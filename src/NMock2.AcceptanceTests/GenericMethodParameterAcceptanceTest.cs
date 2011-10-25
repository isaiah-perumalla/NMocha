//-----------------------------------------------------------------------
// <copyright file="GenericMethodParameterAcceptanceTest.cs" company="NMock2">
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
// This is the easiest way to ignore StyleCop rules on this file, even if we shouldn't use this tag:
// <auto-generated />
//-----------------------------------------------------------------------
namespace NMock2.AcceptanceTests
{
    using System.Collections;
    using NUnit.Framework;

    /// <summary>
    /// Tests for generic method parameters and return values.
    /// <see cref="GenericMethodTypeParamAcceptanceTest"/> for acceptance tests about
    /// generic type parameters.
    /// </summary>
    /// <remarks>
    /// Created on user request for Generic return types.
    /// Request was fed by Adrian Krummenacher on 18-JAN-2008.
    /// </remarks>
    [TestFixture]
    public class GenericMethodParameterAcceptanceTest : AcceptanceTestBase
    {
        public interface IServiceOne
        {
            string ServiceOneGetsName();
        }

        public interface IServiceTwo
        {
            bool ServiceTwoSaves();
        }

        public interface ILocator
        {
            Hashtable Instances { get; }
            void Register<T>(T instance);
            T Get<T>();
        }

        public abstract class Locator : ILocator
        {
            public abstract Hashtable Instances { get; }
            public abstract void Register<T>(T instance);
            public abstract T Get<T>();
        }


        private class PersistentClass
        {
            private int integerValue;
            private string stringValue;

            public PersistentClass(int integerValue, string stringValue)
            {
                this.integerValue = integerValue;
                this.stringValue = stringValue;
            }
        }

        private PersistentClass CreatePersistentObject()
        {
            return new PersistentClass(23, "Persistent string");
        }

        [Test]
        public void CanMockGenericMethodOnInterface()
        {
            AssertCanMockGenericMethod(Mocks.NewMock<ILocator>());
        }

        [Test, Class]
        public void CanMockGenericMethodOnClass()
        {
            AssertCanMockGenericMethod(Mocks.NewMock<Locator>());
        }

        private void AssertCanMockGenericMethod(ILocator locatorMock)
        {
            IServiceOne serviceOneMock = Mocks.NewMock<IServiceOne>();
            IServiceTwo serviceTwoMock = Mocks.NewMock<IServiceTwo>();

            // That works only with Expect and if the order of calls to Get match the order of the expectations:
            Expect.Once.On(locatorMock).Method("Get").Will(Return.Value(serviceOneMock));
            Expect.Once.On(locatorMock).Method("Get").Will(Return.Value(serviceTwoMock));

            Expect.Once.On(serviceOneMock).Method("ServiceOneGetsName").Will(Return.Value("ServiceOne"));
            Expect.Once.On(serviceTwoMock).Method("ServiceTwoSaves").Will(Return.Value(true));

            // real call now; only works in same order as the expectations
            IServiceOne serviceOne = locatorMock.Get<IServiceOne>();
            string name = serviceOne.ServiceOneGetsName();
            Assert.AreEqual("ServiceOne", name, "Service one returned wrong name.");

            IServiceTwo serviceTwo = locatorMock.Get<IServiceTwo>();
            bool res = serviceTwo.ServiceTwoSaves();
            Assert.AreEqual(true, res, "Service two returned wrong boolean value.");
        }

        [Test]
        public void CanMockGenericMethodWithGenericParameterOnInterface()
        {
            AssertCanMockGenericMethodWithGenericParameter(Mocks.NewMock<IGenericHelloWorld>());
        }

        [Test, Class]
        public void CanMockGenericMethodWithGenericParameterOnClass()
        {
            AssertCanMockGenericMethodWithGenericParameter(Mocks.NewMock<GenericHelloWorld>());
        }

        public void AssertCanMockGenericMethodWithGenericParameter(IGenericHelloWorld genericMock)
        {
            const bool expectedSaveResult = true;
            PersistentClass persistentObject = CreatePersistentObject();

            Expect.Once.On(genericMock).Method("Save").With(persistentObject).Will(Return.Value(expectedSaveResult));

            bool saveResult = genericMock.Save<PersistentClass>(persistentObject);
            Assert.AreEqual(expectedSaveResult, saveResult,
                            "Generic method 'Save' with PersistentClass did not return correct value.");
        }

        [Test]
        public void CanMockGenericMethodWithGenericParameterUsingValueTypeOnInterface()
        {
            AssertCanMockGenericMethodWithGenericParameterUsingValueType(Mocks.NewMock<IGenericHelloWorld>());
        }

        [Test, Class]
        public void CanMockGenericMethodWithGenericParameterUsingValueTypeOnClass()
        {
            AssertCanMockGenericMethodWithGenericParameterUsingValueType(Mocks.NewMock<GenericHelloWorld>());
        }

        private void AssertCanMockGenericMethodWithGenericParameterUsingValueType(IGenericHelloWorld genericMock)
        {
            const bool expectedSaveResult = true;
            const decimal decimalValue = 13.5m;

            Expect.Once.On(genericMock).Method("Save").With(decimalValue).Will(Return.Value(expectedSaveResult));

            bool saveResult = genericMock.Save<decimal>(decimalValue);
            Assert.AreEqual(expectedSaveResult, saveResult,
                            "Generic method 'Save' with decimal value did not return correct value.");
        }

        [Test]
        public void CanMockGenericMethodWithGenericReturnValueUsingMixedTypesOnInterface()
        {
            AssertCanMockGenericMethodWithGenericReturnValueUsingMixedTypes(Mocks.NewMock<IGenericHelloWorld>());
        }

        [Test, Class]
        public void CanMockGenericMethodWithGenericReturnValueUsingMixedTypesOnClass()
        {
            AssertCanMockGenericMethodWithGenericReturnValueUsingMixedTypes(Mocks.NewMock<GenericHelloWorld>());
        }

        private void AssertCanMockGenericMethodWithGenericReturnValueUsingMixedTypes(IGenericHelloWorld genericMock)
        {
            const int integerValue = 12;
            const string stringValue = "Hello World";

            Expect.Once.On(genericMock).Method("Find").Will(Return.Value(integerValue));
            Expect.Once.On(genericMock).Method("Find").Will(Return.Value(stringValue));

            int integerFindResult = genericMock.Find<int>();
            Assert.AreEqual(integerValue, integerFindResult,
                            "Generic method did not return correct Value Type value.");

            string stringFindResult = genericMock.Find<string>();
            Assert.AreEqual(stringValue, stringFindResult,
                            "Generic method did not return correct Reference Type value.");
        }

        [Test]
        public void CanMockGenericMethodWithGenericReturnValueUsingReferenceTypeOnInterface()
        {
            AssertCanMockGenericMethodWithGenericReturnValueUsingReferenceType(Mocks.NewMock<IGenericHelloWorld>());
        }

        [Test, Class]
        public void CanMockGenericMethodWithGenericReturnValueUsingReferenceTypeOnClass()
        {
            AssertCanMockGenericMethodWithGenericReturnValueUsingReferenceType(Mocks.NewMock<GenericHelloWorld>());
        }

        public void AssertCanMockGenericMethodWithGenericReturnValueUsingReferenceType(IGenericHelloWorld genericMock)
        {
            const string stringValue = "Hello World";

            Expect.Once.On(genericMock).Method("Find").Will(Return.Value(stringValue));

            string findResult = genericMock.Find<string>();
            Assert.AreEqual(stringValue, findResult, "Generic method did not return correct Reference Type value.");
        }

        [Test]
        public void CanMockGenericMethodWithGenericReturnValueUsingValueTypeOnInterface()
        {
            AssertCanMockGenericMethodWithGenericReturnValueUsingValueType(Mocks.NewMock<IGenericHelloWorld>());
        }

        [Test, Class]
        public void CanMockGenericMethodWithGenericReturnValueUsingValueTypeOnClass()
        {
            AssertCanMockGenericMethodWithGenericReturnValueUsingValueType(Mocks.NewMock<GenericHelloWorld>());
        }

        private void AssertCanMockGenericMethodWithGenericReturnValueUsingValueType(IGenericHelloWorld genericMock)
        {
            const int integerValue = 12;

            Expect.Once.On(genericMock).Method("Find").Will(Return.Value(integerValue));

            int findResult = genericMock.Find<int>();
            Assert.AreEqual(integerValue, findResult, "Generic method did not return correct Value Type value.");
        }

        [Test]
        public void CanMockGenericMethodWithMixedParametersOnInterface()
        {
            AssertCanMockGenericMethodWithMixedParameters(Mocks.NewMock<IGenericHelloWorld>());
        }

        [Test, Class]
        public void CanMockGenericMethodWithMixedParametersOnClass()
        {
            AssertCanMockGenericMethodWithMixedParameters(Mocks.NewMock<GenericHelloWorld>());
        }

        private void AssertCanMockGenericMethodWithMixedParameters(IGenericHelloWorld genericMock)
        {
            string variableA = "Contents of variable a";
            string variableB = "Contents of variable b";
            Expect.Once.On(genericMock).Method("SetVariable").With("A", "Contents of variable a");
            Expect.Once.On(genericMock).Method("SetVariable").With("B", "Contents of variable b");
            Expect.Once.On(genericMock).Method("ReadVariable").With("A").Will(Return.Value(variableA));
            Expect.Once.On(genericMock).Method("ReadVariable").With("B").Will(Return.Value(variableB));

            genericMock.SetVariable<string>("A", "Contents of variable a");
            string resultA = genericMock.ReadVariable<string>("A");
            Assert.AreEqual("Contents of variable a", resultA, "Variable 'A' was not read correctly.");

            genericMock.SetVariable<string>("B", "Contents of variable b");
            string resultB = genericMock.ReadVariable<string>("B");
            Assert.AreEqual("Contents of variable b", resultB, "Variable 'B' was not read correctly.");
        }
    }
}
