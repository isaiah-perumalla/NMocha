//-----------------------------------------------------------------------
// <copyright file="GenericOutParamAcceptanceTest.cs" company="NMock2">
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
    using System.Collections.Generic;
    using NMock2.Actions;
    using NUnit.Framework;

    public interface IGenericOutParamInterface
    {
        bool SomeMethod(string arg_in, out List<string> vals_out);
    }

    public abstract class OutParamClass : IGenericOutParamInterface
    {
        public abstract bool SomeMethod(string arg_in, out List<string> vals_out);
    }


    [TestFixture]
    public class GenericOutParamAcceptanceTest : AcceptanceTestBase
    {
        [Test, CastleOnly]
        public void CanMockMethodWithOutParamOnInterface()
        {
            // InterfaceOnlyMockObjectFactory-generated mocks do not currently support this
            // - They expect out params to be explicitly set
            
            AssertCanMockMethodWithOutParam(Mocks.NewInstanceOfRole<IGenericOutParamInterface>());
        }

        [Test, Class]
        public void CanMockMethodWithOutParamOnClass()
        {
            AssertCanMockMethodWithOutParam(Mocks.NewInstanceOfRole<OutParamClass>());
        }

        private void AssertCanMockMethodWithOutParam(IGenericOutParamInterface someClass)
        {
            Expect.Once.On(someClass).Message("SomeMethod").Will(Return.Value(true));

            List<string> myList = new List<string>();
            someClass.SomeMethod("test", out myList);
        }

        [Test]
        public void CanMockMethodWithOutParamWithIsAnythingOnInterface()
        {
            AssertCanMockMethodWithOutParamWithIsAnything(Mocks.NewInstanceOfRole<IGenericOutParamInterface>());
        }

        [Test, Class]
        public void CanMockMethodWithOutParamWithIsAnythingOnClass()
        {
            AssertCanMockMethodWithOutParamWithIsAnything(Mocks.NewInstanceOfRole<OutParamClass>());
        }

        private void AssertCanMockMethodWithOutParamWithIsAnything(IGenericOutParamInterface someClass)
        {
            List<string> myList = new List<string>();

            Expect.Once.On(someClass).Message("SomeMethod").With(Is.Anything, Is.Out).Will(
                Return.Value(false),
                new SetNamedParameterAction("vals_out", myList)
                );

            bool ret = someClass.SomeMethod("test", out myList);
        }

        [Test]
        public void CanMockMethodWithOutParamWithDefinedValueOnInterface()
        {
            AssertCanMockMethodWithOutParamWithDefinedValue(Mocks.NewInstanceOfRole<IGenericOutParamInterface>());
        }

        [Test, Class]
        public void CanMockMethodWithOutParamWithDefinedValueOnClass()
        {
            AssertCanMockMethodWithOutParamWithDefinedValue(Mocks.NewInstanceOfRole<OutParamClass>());
        }

        private void AssertCanMockMethodWithOutParamWithDefinedValue(IGenericOutParamInterface someClass)
        {
            List<string> myList = new List<string>();

            Expect.Once.On(someClass).Message("SomeMethod").With("test", Is.Out).Will(
                Return.Value(false),
                new SetNamedParameterAction("vals_out", myList)
                );

            bool ret = someClass.SomeMethod("test", out myList);
        }
    }
}