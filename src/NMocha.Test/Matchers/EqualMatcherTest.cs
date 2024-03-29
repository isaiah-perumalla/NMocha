//-----------------------------------------------------------------------
// <copyright file="EqualMatcherTest.cs" company="NMock2">
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
using NMocha;
using NMock2.Matchers;
using NUnit.Framework;

namespace NMock2.Test.Matchers {
    [TestFixture]
    public class EqualMatcherTest {
        private const string EXPECTED = "expected";

        [Test]
        public void CanCompareAutoboxedValues() {
            Matcher matcher = new EqualMatcher(1);
            Assert.IsTrue(matcher.Matches(1), "equal value");
            Assert.IsFalse(matcher.Matches(2), "other value");
        }

        [Test]
        public void ComparesArgumentForEqualityToExpectedObject() {
            Matcher matcher = new EqualMatcher(EXPECTED);

            Assert.IsTrue(matcher.Matches(EXPECTED), "same object");
            Assert.IsTrue(matcher.Matches(EXPECTED.Clone()), "equal object");
            Assert.IsFalse(matcher.Matches("not expected"), "unequal object");
        }

        [Test]
        public void ComparesArraysForEqualityByContents() {
            int[] expected = {1, 2};
            int[] equal = {1, 2};
            int[] inequal = {2, 3};
            int[] longer = {1, 2, 3};
            int[] shorter = {1};
            int[] empty = {};
            int[,] otherRank = {{1, 2}, {3, 4}};
            Matcher matcher = new EqualMatcher(expected);

            Assert.IsTrue(matcher.Matches(expected), "same array");
            Assert.IsTrue(matcher.Matches(equal), "same contents");
            Assert.IsFalse(matcher.Matches(inequal), "different contents");
            Assert.IsFalse(matcher.Matches(longer), "longer");
            Assert.IsFalse(matcher.Matches(shorter), "shorter");
            Assert.IsFalse(matcher.Matches(empty), "empty");
            Assert.IsFalse(matcher.Matches(otherRank), "other rank");
        }

        [Test]
        public void ComparesMultidimensionalArraysForEquality() {
            int[,] expected = {{1, 2}, {3, 4}};
            int[,] equal = {{1, 2}, {3, 4}};
            int[,] inequal = {{3, 4}, {5, 6}};
            var empty = new int[0,0];
            int[] otherRank = {1, 2};
            Matcher matcher = new EqualMatcher(expected);

            Assert.IsTrue(matcher.Matches(expected), "same array");
            Assert.IsTrue(matcher.Matches(equal), "same contents");
            Assert.IsFalse(matcher.Matches(inequal), "different contents");
            Assert.IsFalse(matcher.Matches(empty), "empty");
            Assert.IsFalse(matcher.Matches(otherRank), "other rank");
        }

        [Test]
        public void HasAReadableDescription() {
            var value = new NamedObject("value");
            AssertDescription.IsEqual(new EqualMatcher(value), "equal to <" + value + ">");
        }

        [Test]
        public void IsNullSafe() {
            Assert.IsTrue(new EqualMatcher(null).Matches(null), "null matches null");
            Assert.IsFalse(new EqualMatcher("not null").Matches(null), "not null does not match null");
            Assert.IsFalse(new EqualMatcher(null).Matches("not null"), "null does not match not null");
        }

        [Test]
        public void RecursivelyComparesArrayContentsOfNestedArrays() {
            var expected = new[] {new[] {1, 2}, new[] {3, 4}};
            var equal = new[] {new[] {1, 2}, new[] {3, 4}};
            var inequal = new[] {new[] {2, 3}, new[] {4, 5}};
            Matcher matcher = new EqualMatcher(expected);

            Assert.IsTrue(matcher.Matches(expected), "same array");
            Assert.IsTrue(matcher.Matches(equal), "same contents");
            Assert.IsFalse(matcher.Matches(inequal), "different contents");
        }

        [Test]
        public void RecursivelyComparesContentsOfNestedLists() {
            var expected = new ArrayList(new[] {
                                                   new ArrayList(new[] {1, 2}),
                                                   new ArrayList(new[] {3, 4})
                                               });
            var equal = new ArrayList(new[] {
                                                new ArrayList(new[] {1, 2}),
                                                new ArrayList(new[] {3, 4})
                                            });
            var inequal = new ArrayList(new[] {
                                                  new ArrayList(new[] {2, 3}),
                                                  new ArrayList(new[] {4, 5})
                                              });
            Matcher matcher = new EqualMatcher(expected);

            Assert.IsTrue(matcher.Matches(expected), "same array");
            Assert.IsTrue(matcher.Matches(equal), "same contents");
            Assert.IsFalse(matcher.Matches(inequal), "different contents");
        }
    }
}