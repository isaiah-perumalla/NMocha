//-----------------------------------------------------------------------
// <copyright file="GenericMatcherTest.cs" company="NMock2">
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
//-----------------------------------------------------------------------

using NMock2.Matchers;
using NUnit.Framework;

namespace NMock2.Test.Matchers {
    /// <summary>
    /// Tests the implementation of <see cref="GenericMatcher{T}"/>
    /// </summary>
    [TestFixture]
    public class GenericMatcherTest {
        #region Setup/Teardown

        /// <summary>
        /// Sets up the tests.
        /// Creates the testee.
        /// </summary>
        [SetUp]
        public void SetUp() {
            testee = new GenericMatcher<string>(value => value.Contains("abc"));
        }

        #endregion

        /// <summary>
        /// The object under test
        /// </summary>
        private GenericMatcher<string> testee;

        /// <summary>
        /// A correct value matches the expectation.
        /// </summary>
        [Test]
        public void CorrectValueMatch() {
            Assert.IsTrue(testee.Matches("wwwabcxxx"));
        }

        /// <summary>
        /// A wrong type does not match the expectation.
        /// </summary>
        [Test]
        public void WrongTypeDoesNotMatch() {
            Assert.IsFalse(testee.Matches(4));
        }

        /// <summary>
        /// A wrong value does not match the expectation.
        /// </summary>
        [Test]
        public void WrongValueDoesNotMatch() {
            Assert.IsFalse(testee.Matches("www"));
        }
    }
}