//-----------------------------------------------------------------------
// <copyright file="MockMultipleInterfacesAcceptanceTest.cs" company="NMock2">
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
    using System;
    using System.Collections;
    using System.IO;
    using NMock2.Monitoring;
    using NUnit.Framework;
    using NMock2.Internal;


    [TestFixture, CastleOnly]
    public class MockMultipleInterfacesAcceptanceTest : AcceptanceTestBase
    {
        public interface IHaveAllMemberTypes
        {
            int this[string key] { get; set; }
            int Value { get; set; }
            event EventHandler SomethingHappened;
            int DoSomething();
        }

        public interface IAmASubclass : IHaveAllMemberTypes
        {
            int DoSomethingElse();
        }

        public interface IHaveIdenticalMember
        {
            int DoSomething(); // Same member as on IHaveAllMemberTypes
        }

        public interface IAmGeneric<T>
        {
            T DoWork(T input);
        }

        public interface IAmAMarkerInterface
        {

        }

        public abstract class SomeBase
        {
            public abstract string GetName();
            public virtual int GetSerialNumber()
            {
                return 123;
            }
        }

        public abstract class SomeOtherClass
        {
            
        }



        [Test]
        public void CanMockMultipleInterfaces()
        {
            var mock = Mocks.NewMock<IEnumerable>(DefinedAs.Implementing<IHaveAllMemberTypes>());

            AssertExpectationsCanBeSet(mock as IEnumerable);
            AssertExpectationsCanBeSet(mock as IHaveAllMemberTypes);
        }

        [Test]
        public void CanMockClassAndInterfaces()
        {
            var mock = Mocks.NewMock<SomeBase>(DefinedAs.Implementing(typeof(IEnumerable), typeof(IHaveAllMemberTypes)));

            AssertExpectationsCanBeSet(mock as IEnumerable);
            AssertExpectationsCanBeSet(mock as IHaveAllMemberTypes);

            Expect.Once.On(mock).Method("GetName").Will(Return.Value("AAA"));
            Assert.AreSame("AAA", mock.GetName());
        }

        [Test]
        public void CanMockInheritedInterfaces()
        {
            var mock = Mocks.NewMock<IEnumerable>(DefinedAs.Implementing<IAmASubclass>());

            AssertExpectationsCanBeSet(mock as IEnumerable);
            AssertExpectationsCanBeSet(mock as IHaveAllMemberTypes);
            AssertExpectationsCanBeSet(mock as IAmASubclass);
        }

        [Test]
        public void DuplicateInterfacesAreIgnored()
        {
            var mock = Mocks.NewMock<IHaveAllMemberTypes>(DefinedAs.Implementing(typeof(IHaveAllMemberTypes), typeof(IHaveAllMemberTypes)));
            var otherMock = Mocks.NewMock<IHaveAllMemberTypes>(DefinedAs.Implementing<IHaveAllMemberTypes>());

            AssertExpectationsCanBeSet(mock);
            Assert.AreEqual(mock.GetType(), otherMock.GetType(), "Mocks were not of the same runtime type");
        }

        [Test, Ignore("This is an unlikely scenario. Is is mostly harmless and would have negative perf implications if fixed.")]
        public void DuplicateInheritedInterfacesAreIgnored()
        {
            var mock = Mocks.NewMock<IAmASubclass>(DefinedAs.Implementing<IHaveAllMemberTypes>());
            var otherMock = Mocks.NewMock<IAmASubclass>();

            Assert.AreEqual(mock.GetType(), otherMock.GetType());
        }

        [Test]
        public void OrderOfInterfacesIsIgnored()
        {
            var mock = Mocks.NewMock<IEnumerable>(DefinedAs.Implementing(typeof(IDisposable), typeof(IHaveAllMemberTypes)));
            var otherMock = Mocks.NewMock<IDisposable>(DefinedAs.Implementing(typeof(IHaveAllMemberTypes), typeof(IEnumerable)));

            Assert.AreEqual(mock.GetType(), otherMock.GetType());
        }

        [Test]
        public void CanMockEmptyMarkerInterfaces()
        {
            var mock = Mocks.NewMock<IEnumerable>(DefinedAs.Implementing<IAmAMarkerInterface>());

            Assert.IsInstanceOfType(typeof(IEnumerable), mock);
            Assert.IsInstanceOfType(typeof(IAmAMarkerInterface), mock);
        }

        [Test]
        public void CanMockASingleClassTypeAsAnAdditionalType()
        {
            var mock = Mocks.NewMock<IEnumerable>(DefinedAs.Implementing(typeof(IHaveAllMemberTypes), typeof(SomeBase)));

            AssertExpectationsCanBeSet(mock as IEnumerable);
            AssertExpectationsCanBeSet(mock as IHaveAllMemberTypes);

            Expect.Once.On(mock).Method("GetName").Will(Return.Value("AAA"));
            Assert.AreSame("AAA", (mock as SomeBase).GetName());
        }

        [Test, ExpectedException(typeof(ArgumentException))]
        public void MockingMultipleClassesAsAdditionalTypesThrowsArgumentException()
        {
            var mock = Mocks.NewMock<IEnumerable>(DefinedAs.Implementing(typeof(SomeBase), typeof(SomeOtherClass)));
        }

        [Test, ExpectedException(typeof(ArgumentException))]
        public void MockingAClassAsAnAdditionalTypeWhenAlreadyMockingAClassThrowsArgumentException()
        {
            var mock = Mocks.NewMock<SomeBase>(DefinedAs.Implementing(typeof(IEnumerable), typeof(SomeOtherClass)));
        }

        [Test]
        public void TransparentMocksAllowImplementationOfClassTypeToBeCalled()
        {
            var mock = Mocks.NewMock<IEnumerable>(DefinedAs.OfStyle(MockStyle.Transparent).Implementing<SomeBase>()) as SomeBase;

            Assert.AreEqual(123, mock.GetSerialNumber());
        }

        [Test]
        public void CallingAnInterfaceMethodOfATransparentMockWithAnExpectationSetWillSatisfyExpectation()
        {
            IEnumerator expected = new int[0].GetEnumerator();
            var mock = Mocks.NewMock<IEnumerable>(DefinedAs.OfStyle(MockStyle.Transparent).Implementing<SomeBase>());

            Expect.Once.On(mock).Method("GetEnumerator").Will(Return.Value(expected));

            mock.GetEnumerator();
        }

        [Test, ExpectedException(typeof(ExpectationException))]
        public void CallingAnInterfaceMethodOfATransparentMockWithoutAnExpectationThrowsAnExpectationException()
        {
            // If we don't do this, the expectation exception will get rethrown in Teardown()
            SkipVerificationForThisTest();

            var mock = Mocks.NewMock<IEnumerable>(DefinedAs.OfStyle(MockStyle.Transparent).Implementing<SomeBase>());

            mock.GetEnumerator();
        }

        private void AssertExpectationsCanBeSet(IHaveAllMemberTypes mock)
        {
            // Setup expectations
            Expect.Once.On(mock).SetProperty("Value");
            Expect.Once.On(mock).GetProperty("Value").Will(Return.Value(1));
            Expect.Once.On(mock).Set["A"].To(Is.Anything);
            Expect.Once.On(mock).Get["A"].Will(Return.Value(2));
            Expect.Once.On(mock).EventAdd("SomethingHappened");
            Expect.Once.On(mock).EventRemove("SomethingHappened");
            Expect.Once.On(mock).Method("DoSomething").Will(Return.Value(3));
            
            // Invoke members on mock and validate what we can
            mock.Value = 0;
            Assert.AreEqual(1, mock.Value);
            mock["A"] = 0;
            Assert.AreEqual(2, mock["A"]);
            Assert.AreEqual(3, mock.DoSomething());

            // Validate that events are working
            bool eventFired = false;
            EventHandler handler = (s, e) => eventFired = true;
            mock.SomethingHappened += handler;          
            Fire.On(mock).Event("SomethingHappened").With(mock, new EventArgs());
            mock.SomethingHappened -= handler;
            Assert.IsTrue(eventFired, "SomethingHappened event did not fire");
        }

        private void AssertExpectationsCanBeSet(IAmASubclass mock)
        {
            Expect.Once.On(mock).Method("DoSomethingElse").Will(Return.Value(4));
            
            Assert.AreEqual(4, mock.DoSomethingElse());
        }

        private void AssertExpectationsCanBeSet(IEnumerable mock)
        {
            IEnumerator enumerator = new ArrayList().GetEnumerator();
            Expect.Once.On(mock).Method("GetEnumerator").Will(Return.Value(enumerator));

            Assert.AreSame(enumerator, mock.GetEnumerator());
        }
    }
}