//-----------------------------------------------------------------------
// <copyright file="OutParamAcceptanceTest.cs" company="NMock2">
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
using NMock2;
using NMock2.Actions;
using NUnit.Framework;

namespace NMocha.AcceptanceTests {
    public interface IAdder {
        void Add(int a, int b, out int c);
        int Add(int a, int b);
    }

    public abstract class Adder : IAdder {
        #region IAdder Members

        public abstract void Add(int a, int b, out int c);
        public abstract int Add(int a, int b);

        #endregion
    }

    [TestFixture]
    public class OutParamAcceptanceTest : AcceptanceTestBase {
        private void AssertCanMockInAndOutParamOnMethod(IAdder adder) {
            Expect.Once.On(adder).Message("Add").With(3, 5, Is.Out).Will(
                new SetNamedParameterAction("c", 8)
                );

            int outValue;
            adder.Add(3, 5, out outValue);
            Assert.AreEqual(8, outValue, "Outvalue was not set correctly.");
        }

        private void AssertCanMockInAndOutParamWithReturnValueOnMethod(IAdder adder) {
            Expect.Once.On(adder).Message("Add").With(4, 7).Will(Return.Value(11));

            int c = adder.Add(4, 7);
            Assert.AreEqual(11, c, "Result was not set correctly.");
        }

        [Test, Class]
        public void CanMockInAndOutParamOnClassMethod() {
            AssertCanMockInAndOutParamOnMethod(Mockery.NewInstanceOfRole<Adder>());
        }

        [Test]
        public void CanMockInAndOutParamOnInterfaceMethod() {
            AssertCanMockInAndOutParamOnMethod(Mockery.NewInstanceOfRole<IAdder>());
        }

        [Test, Class]
        public void CanMockInAndOutParamWithReturnValueOnClassMethod() {
            AssertCanMockInAndOutParamWithReturnValueOnMethod(Mockery.NewInstanceOfRole<Adder>());
        }

        [Test]
        public void CanMockInAndOutParamWithReturnValueOnInterfaceMethod() {
            AssertCanMockInAndOutParamWithReturnValueOnMethod(Mockery.NewInstanceOfRole<IAdder>());
        }

        [Test]
        public void CanMockOutParameterUsingShortcutOnReturnClass() {
            var adder = Mockery.NewInstanceOfRole<IAdder>();

            Expect.Once.On(adder).Message("Add").With(3, 5, Is.Out).Will(Return.OutValue("c", 8));

            int outValue;
            adder.Add(3, 5, out outValue);
            Assert.AreEqual(8, outValue, "Outvalue was not set correctly.");
        }
    }
}