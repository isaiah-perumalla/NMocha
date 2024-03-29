//-----------------------------------------------------------------------
// <copyright file="MockMatcher.cs" company="NMock2">
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
using System.IO;
using NUnit.Framework;

namespace NMocha.Test.Matchers {
    internal class MockMatcher : Matcher {
        public int DescribeToCallCount;
        public string DescribeToOutput = "";
        public TextWriter ExpectedDescribeToWriter;
        public object ExpectedMatchesArg;
        public int MatchesCallCount;
        public bool MatchesResult;

        public override bool Matches(object o) {
            MatchesCallCount++;
            Assert.AreEqual(ExpectedMatchesArg, o, "Matches arg");
            return MatchesResult;
        }

        public void AssertMatchesCalled(string messageFormat, params object[] formatArgs) {
            AssertMatchesCalled(1, messageFormat, formatArgs);
        }

        public void AssertMatchesCalled(int times, string messageFormat, params object[] formatArgs) {
            Assert.AreEqual(times, MatchesCallCount, messageFormat, formatArgs);
        }

        public override void DescribeOn(IDescription description) {
            DescribeToCallCount++;
            if (ExpectedDescribeToWriter != null)
            {
                Assert.AreSame(ExpectedDescribeToWriter, description, "DescribeTo writer");
            }
            description.AppendText(DescribeToOutput);
        }
    }
}