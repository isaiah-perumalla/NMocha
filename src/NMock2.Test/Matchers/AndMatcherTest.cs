//-----------------------------------------------------------------------
// <copyright file="AndMatcherTest.cs" company="NMock2">
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
using NMock2.Matchers;
using NUnit.Framework;

namespace NMock2.Test.Matchers {
    [TestFixture]
    public class AndMatcherTest {
        private static readonly object ignored = new object();
        private static readonly Matcher TRUE = new AlwaysMatcher(true, "TRUE");
        private static readonly Matcher FALSE = new AlwaysMatcher(false, "FALSE");

        private static readonly object[,] truthTable = {
                                                           {FALSE, FALSE, false},
                                                           {FALSE, TRUE, false},
                                                           {TRUE, FALSE, false},
                                                           {TRUE, TRUE, true}
                                                       };

        [Test]
        public void CalculatesLogicalConjunctionOfTwoMatchers() {
            for (int i = 0; i < truthTable.GetLength(0); i++)
            {
                Matcher matcher = new AndMatcher((Matcher) truthTable[i, 0], (Matcher) truthTable[i, 1]);

                Assert.AreEqual(truthTable[i, 2], matcher.Matches(ignored));
            }
        }

        [Test]
        public void CanUseOperatorOverloadingAsSyntacticSugar() {
            for (int i = 0; i < truthTable.GetLength(0); i++)
            {
                var arg1 = (Matcher) truthTable[i, 0];
                var arg2 = (Matcher) truthTable[i, 1];
                Matcher matcher = arg1 & arg2;

                Assert.AreEqual(truthTable[i, 2], matcher.Matches(ignored));
            }
        }

        [Test]
        public void HasAReadableDescription() {
            Matcher left = new MatcherWithDescription("<left>");
            Matcher right = new MatcherWithDescription("<right>");

            AssertDescription.IsComposed(new AndMatcher(left, right), "`{0}' and `{1}'", left, right);
        }
    }
}