//-----------------------------------------------------------------------
// <copyright file="SameMatcherTest.cs" company="NMock2">
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
using NMocha;
using NMock2.Matchers;
using NUnit.Framework;

namespace NMock2.Test.Matchers {
    [TestFixture]
    public class SameMatcherTest {
        [Test]
        public void HasAReadableDescription() {
            var same = new object();
            AssertDescription.IsEqual(new SameMatcher(same), "same as <" + same + ">");
        }

        [Test]
        public void IsNullSafe() {
            Assert.IsTrue(new SameMatcher(null).Matches(null), "null matches null");
            Assert.IsFalse(new SameMatcher("not null").Matches(null), "not null does not match null");
            Assert.IsFalse(new SameMatcher(null).Matches("not null"), "null does not match not null");
        }

        [Test]
        public void MatchesSameObject() {
            var same = new object();
            var other = new object();
            Matcher matcher = new SameMatcher(same);

            Assert.IsTrue(matcher.Matches(same), "same");
            Assert.IsFalse(matcher.Matches(other), "other");
        }
    }
}