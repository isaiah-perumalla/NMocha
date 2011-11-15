//-----------------------------------------------------------------------
// <copyright file="IndexSetterArgumentsMatcher.cs" company="NMock2">
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
using System.IO;
using NMocha;

namespace NMock2.Matchers {
    /// <summary>
    /// Matcher for indexer setters. Checks that the arguments passed to the indexer match.
    /// </summary>
    public class IndexSetterArgumentsMatcher : ArgumentsMatcher {
        /// <summary>
        /// Initializes a new instance of the <see cref="IndexSetterArgumentsMatcher"/> class.
        /// </summary>
        /// <param name="valueMatchers">The value matchers. This is an ordered list of matchers, each matching a single method argument.</param>
        public IndexSetterArgumentsMatcher(params Matcher[] valueMatchers) : base(valueMatchers) {
        }

        /// <summary>
        /// Describes this object.
        /// </summary>
        /// <param name="description"></param>
        public override void DescribeOn(IDescription description) {
            description.AppendText("[");
            WriteListOfMatchers(MatcherCount() - 1, description);
            description.AppendText("] = (");
            LastMatcher().DescribeOn(description);
            description.AppendText(")");
        }
    }
}