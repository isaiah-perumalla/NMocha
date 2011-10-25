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
namespace NMock2.Matchers
{
    using System.IO;

    /// <summary>
    /// Matcher for indexer setters. Checks that the arguments passed to the indexer match.
    /// </summary>
    public class IndexSetterArgumentsMatcher : ArgumentsMatcher
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IndexSetterArgumentsMatcher"/> class.
        /// </summary>
        /// <param name="valueMatchers">The value matchers. This is an ordered list of matchers, each matching a single method argument.</param>
        public IndexSetterArgumentsMatcher(params Matcher[] valueMatchers) : base(valueMatchers)
        {
        }

        /// <summary>
        /// Describes this object.
        /// </summary>
        /// <param name="writer">The text writer the description is added to.</param>
        public override void DescribeTo(TextWriter writer)
        {
            writer.Write("[");
            WriteListOfMatchers(MatcherCount() - 1, writer);
            writer.Write("] = (");
            LastMatcher().DescribeTo(writer);
            writer.Write(")");
        }
    }
}
