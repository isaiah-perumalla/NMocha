//-----------------------------------------------------------------------
// <copyright file="AlwaysMatcherTest.cs" company="NMock2">
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
namespace NMock2.Test.Matchers
{
    using NUnit.Framework;
    using NMock2.Matchers;

    [TestFixture]
    public class AlwaysMatcherTest
    {
        [Test]
        public void AlwaysReturnsFixedBooleanValueFromMatchesMethod()
        {
            Matcher matcher = new AlwaysMatcher(true, "");
            Assert.IsTrue(matcher.Matches("something"));
            Assert.IsTrue(matcher.Matches("something else"));
            Assert.IsTrue(matcher.Matches(null));
            Assert.IsTrue(matcher.Matches(1));
            Assert.IsTrue(matcher.Matches(1.0));
            Assert.IsTrue(matcher.Matches(new object()));

            matcher = new AlwaysMatcher(false, "");
            Assert.IsFalse(matcher.Matches("something"));
            Assert.IsFalse(matcher.Matches("something else"));
            Assert.IsFalse(matcher.Matches(null));
            Assert.IsFalse(matcher.Matches(1));
            Assert.IsFalse(matcher.Matches(1.0));
            Assert.IsFalse(matcher.Matches(new object()));
        }
        
        [Test]
        public void IsGivenADescription()
        {
            string description = "DESCRIPTION";
            bool irrelevantFlag = false;

            AssertDescription.IsEqual(new AlwaysMatcher(irrelevantFlag, description), description);
        }
    }
}
