//-----------------------------------------------------------------------
// <copyright file="MockIEnumerableAcceptanceTest.cs" company="NMock2">
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
using System.Collections;
using System.IO;
using NMocha;
using NMocha.AcceptanceTests;
using NMocha.Monitoring;
using NMock2.Monitoring;
using NUnit.Framework;

namespace NMock2.AcceptanceTests {
    /// <summary>
    /// Fixture that provides tests for a stub that enumerates a few data items.
    /// </summary>
    [TestFixture]
    public class MockIEnumerableAcceptanceTest : AcceptanceTestBase {
        #region Setup/Teardown

        /// <summary>
        /// Initializes the fixture before each testwill run.
        /// </summary>
        /// <remarks>
        /// This method creates a stub thatimplements the <see cref="IEnumerable.GetEnumerator()"/> method.
        /// The <see cref="IEnumerator"/> the stubreturns should really enumerate a few strings in an array.
        /// </remarks>
        [SetUp]
        public override void Setup() {
            base.Setup();

            myEnumerable = Mockery.NewInstanceOfRole<IMyEnumerable>();

            data = new[] {"a", "b", "c", "d", "e"};

            Stub.On(myEnumerable).Message("GetEnumerator").WithNoArguments().Will(new CallGetEnumeratorAction(data));
        }

        #endregion

        /// <summary>
        /// A simple interface that stub should provide.
        /// Really we are only interested in the <seecref="IEnumerable.GetEnumerator()"/>
        /// method, that is provided by the <seecref="IEnumerable"/> interface.
        /// </summary>
        public interface IMyEnumerable : IEnumerable {
            string Name { get; }
        }

        private string[] data;

        private IMyEnumerable myEnumerable;

        public class CallGetEnumeratorAction : IAction {
            private readonly string[] data;

            public CallGetEnumeratorAction(string[] data) {
                this.data = data;
            }

            #region IAction Members

            public void Invoke(Invocation invocation) {
                invocation.Result = data.GetEnumerator();
            }

            public void DescribeOn(IDescription description) {
                description.AppendText("Test");
            }

            #endregion
        }

        /// <summary>
        /// Verifies that the each of the enumerated strings match the corresponding original string
        /// and number of the enumerated strings equal to length of the string array.
        /// </summary>
        private void ShouldEnumerateData() {
            int dataIdx = 0;

            foreach (string s in myEnumerable)
            {
                Assert.AreEqual(data[dataIdx++], s);
            }
            Assert.AreEqual(data.Length, dataIdx);
        }

        /// <summary>
        /// Verifies that the string successfully enumerated once.
        /// </summary>
        [Test]
        public void ShouldEnumerateDataOnce() {
            ShouldEnumerateData();
        }

        /// <summary>
        /// Verifies that the string successfully enumerated twice.
        /// </summary>
        [Test]
        public void ShouldEnumerateDataTwice() {
            ShouldEnumerateData();

            ShouldEnumerateData();
        }
    }
}