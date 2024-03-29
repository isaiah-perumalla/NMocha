using System;
using System.Collections;
using NMock2;
using NUnit.Framework;

namespace NMocha.AcceptanceTests {
    [TestFixture]
    public class ClassMockAcceptanceTest : AcceptanceTestBase {
        public abstract class SampleClassWithVirtualAndNonVirtualMethods {
            public int Subtract(int x, int y) // non-virtual
            {
                return x - y;
            }

            public virtual int Add(int x, int y) // virtual, overloaded
            {
                return x + y;
            }

            public int Add(int x, int y, int z) // non-virtual, overloaded
            {
                return x + y + z;
            }

            public abstract int Add(decimal x, decimal y); // abstract, overloaded
        }

        public class SampleClassWithIMockObjectMembers {
            public string MockName {
                get { throw new Exception("The method or operation is not implemented."); }
            }

            public virtual int Multiply(int a, int b) {
                return a*b;
            }

            public bool HasMethodMatching(Matcher methodMatcher) {
                throw new Exception("The method or operation is not implemented.");
            }

            public void AddExpectation(IExpectation expectation) {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        public abstract class SampleAbstractClass {
            public abstract int Add(int a, int b);
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

        public class SampleSubClass : SampleClass {
            public int AddThenDouble(int a, int b) {
                return DoAdd(a, b)*2;
            }
        }

        public class SampleClassWithObjectOverrides {
            public override string ToString() {
                return base.ToString();
            }

            public override bool Equals(object obj) {
                return base.Equals(obj);
            }

            public override int GetHashCode() {
                return base.GetHashCode();
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

        public interface ISampleInterface {
            int SomeMethod();
            int SomeOtherMethod();
            int SomeGenericMethod<T>(T input);
        }

        public class SampleInterfaceImpl : SampleInterfaceImplSuperClass, ISampleInterface {
            // non-virtual impl of interface method

            #region ISampleInterface Members

            public int SomeMethod() {
                return 7;
            }

            // non-virtual impl of interface generic method
            public int SomeGenericMethod<T>(T input) {
                return 1;
            }

            #endregion
        }

        public class SampleInterfaceImplSuperClass {
            // non-virtual impl of interface method declared
            // at a differnt level of inheritance heirarchy
            public int SomeOtherMethod() {
                return 3;
            }
        }

        [Test, Class]
        public void CanHaveClassWithIMockObjectMembers() {
            var mock = Mockery.NewInstanceOfRole<SampleClassWithIMockObjectMembers>();
            Expect.Once.On(mock).Message("Multiply").Will(Return.Value(133));
            int result = mock.Multiply(12, 12);
            //// even if 12 by 12 is 144, we mocked the method with 133:
            Assert.AreEqual(133, result, "Mock wasn't created successful.");
        }

        [Test, Class]
        public void CanSetExpectationOnAbstractMethod() {
            var mock = Mockery.NewInstanceOfRole<SampleClassWithVirtualAndNonVirtualMethods>();

            // Abstract target method
            Expect.Once.On(mock).Message("Add").With(1.1m, 2.1m).Will(Return.Value(10));
            Assert.AreEqual(10, mock.Add(1.1m, 2.1m));
        }

        [Test, Class]
        public void CanSetExpectationOnAbstractMethodOfClassMock() {
            var mock = Mockery.NewInstanceOfRole<SampleAbstractClass>();

            Expect.Once.On(mock).Message("Add").Will(Return.Value(7));

            Assert.AreEqual(7, mock.Add(1, 2));
        }

        [Test, Class]
        public void CanSetExpectationOnGenericMethodOnMockedClass() {
            var mock = Mockery.NewInstanceOfRole<SampleClassWithGenericMethods>();

            Expect.Once.On(mock).Message("GetStringValue").Will(Return.Value("ABC"));

            Assert.AreEqual("ABC", mock.GetStringValue("XYZ"));
        }

        [Test, Class]
        public void CanSetExpectationOnGenericMethodWithConstraintOnMockedClass() {
            var mock = Mockery.NewInstanceOfRole<SampleClassWithGenericMethods>();

            Expect.Once.On(mock).Message("GetCount").Will(Return.Value(3));

            Assert.AreEqual(3, mock.GetCount(new int[5]));
        }

        [Test, Class]
        public void CanSetExpectationOnMethodOnMockedGenericClass() {
            var mock = Mockery.NewInstanceOfRole<SampleGenericClass<string>>();

            Expect.Once.On(mock).Message("GetDefault").Will(Return.Value("ABC"));

            Assert.AreEqual("ABC", mock.GetDefault());
        }

        [Test, Class]
        public void CanSetExpectationOnMethodOnSuperClassOfMockedClass() {
            var mock = Mockery.NewInstanceOfRole<SampleSubClass>();

            Expect.Once.On(mock).Message("DoAdd").Will(Return.Value(5));

            Assert.AreEqual(10, mock.AddThenDouble(1, 2));
        }

        [Test, Class]
        public void CanSetExpectationOnMethodWhenAtLeastOneMatchedOverloadIsVirtualOrAbstract() {
            var mock = Mockery.NewInstanceOfRole<SampleClassWithVirtualAndNonVirtualMethods>();

            // Three possible matches here...
            Expect.Exactly(2).On(mock).Message("Add").Will(Return.Value(10));
            Assert.AreEqual(10, mock.Add(1, 2), "Virtual method expectation failed");
            Assert.AreEqual(10, mock.Add(1.1m, 2.1m), "Abstract method expectation failed");
            Assert.AreEqual(6, mock.Add(1, 2, 3), "Expected call to non-virtual method to go to implementation");
        }

        [Test, Class]
        public void CanSetExpectationOnMethodWithOutParameterOnMockedClass() {
            var mock = Mockery.NewInstanceOfRole<SampleClass>();

            Expect.Once.On(mock).Message("Divide").Will(Return.Value(3), Return.OutValue("remainder", 15m));

            decimal remainder;
            mock.Divide(7, 2, out remainder);

            Assert.AreEqual(15m, remainder);
        }

        [Test, Class]
        public void CanSetExpectationOnOverriddenObjectMembersOnClassMock() {
            var mock = Mockery.NewInstanceOfRole<SampleClassWithObjectOverrides>();

            Expect.Once.On(mock).Message("ToString").Will(Return.Value("ABC"));
            Expect.Once.On(mock).Message("Equals").Will(Return.Value(false));
            Expect.Once.On(mock).Message("GetHashCode").Will(Return.Value(17));

            Assert.AreEqual("ABC", mock.ToString(), "unexpected ToString() value");
            Assert.IsFalse(mock.Equals(mock), "unexpected Equals() value");
            Assert.AreEqual(17, mock.GetHashCode(), "unexpected GetHashCode() value");
        }

        [Test, Class]
        public void CanSetExpectationOnVirtualMethod() {
            var mock = Mockery.NewInstanceOfRole<SampleClassWithVirtualAndNonVirtualMethods>();

            // Virtual target method
            Expect.Once.On(mock).Message("Add").With(1, 2).Will(Return.Value(10));
            Assert.AreEqual(10, mock.Add(1, 2));
        }

        [Test, Class]
        public void LocalCallsWithinMockedClassCanSatisfyExpectations() {
            var mock = Mockery.NewInstanceOfRole<SampleClass>();

            Expect.Once.On(mock).Message("DoAdd").Will(Return.Value(7)); // Should be called from Add()

            Assert.AreEqual(7, mock.Add(1, 2));
        }

        [Test, Class,
         ExpectedException(typeof (ArgumentException))]
        public void SettingExpectationOnInheritedNonVirtualImplOfInterfaceMethodThrowsArgumentException() {
            var mock = Mockery.NewNamedInstanceOfRole<SampleInterfaceImpl>("myMock");

            // The target method is non-virtual, so we expect an exception.
            Expect.Never.On(mock).Message("SomeOtherMethod");
        }

        [Test, Class,
         ExpectedException(typeof (ArgumentException))]
        public void SettingExpectationOnNonVirtualImplOfInterfaceGenericMethodThrowsArgumentException() {
            var mock = Mockery.NewNamedInstanceOfRole<SampleInterfaceImpl>("myMock");

            // The target method is non-virtual, so we expect an exception.
            Expect.Never.On(mock).Message("SomeGenericMethod", typeof (int));
        }

        [Test, Class,
         ExpectedException(typeof (ArgumentException))]
        public void SettingExpectationOnNonVirtualImplOfInterfaceMethodThrowsArgumentException() {
            var mock = Mockery.NewNamedInstanceOfRole<SampleInterfaceImpl>("myMock");

            // The target method is non-virtual, so we expect an exception.
            Expect.Never.On(mock).Message("SomeMethod");
        }

        [Test, Class,
         ExpectedException(typeof (ArgumentException))]
        public void SettingExpectationOnNonVirtualMethodThrowsArgumentException() {
            var mock = Mockery.NewNamedInstanceOfRole<SampleClassWithVirtualAndNonVirtualMethods>("myMock");

            // The target method is non-virtual, so we expect an exception.
            // (we use 'Never' here to avoid an undesired failure in teardown).
            Expect.Never.On(mock).Message("Subtract").With(2, 1).Will(Return.Value(10));
        }

        // This is a problem, as we can't easily use args to identify overload
        // as expectation is defined, and we can't do it at execution time, because
        // call will not be intercepted (proxy is only for virtual members).
        [Test, Class, ExpectedException(typeof (ArgumentException))]
        [Ignore("We can't identify overloaded non-virtual members by arguments.")]
        public void SettingExpectationOnNonVirtualOverloadOfVirtualMethodThrowsArgumentException() {
            var mock = Mockery.NewInstanceOfRole<SampleClassWithVirtualAndNonVirtualMethods>();

            // The target method is non-virtual, so we expect an exception.
            Expect.Once.On(mock).Message("Add").With(1, 2, 3).Will(Return.Value(10));
        }
    }
}