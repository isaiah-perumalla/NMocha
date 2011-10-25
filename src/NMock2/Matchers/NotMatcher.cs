//-----------------------------------------------------------------------
// <copyright file="NotMatcher.cs" company="NMock2">
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
    /// Matcher that negates another matcher.
    /// </summary>
    public class NotMatcher : Matcher
    {
        /// <summary>
        /// Holds the matcher to negate.
        /// </summary>
        private readonly Matcher negated;

        /// <summary>
        /// Initializes a new instance of the <see cref="NotMatcher"/> class.
        /// </summary>
        /// <param name="negated">The matcher to negate.</param>
        public NotMatcher(Matcher negated)
        {
            this.negated = negated;
        }

        /// <summary>
        /// Matches the specified object to this matcher and returns whether it matches.
        /// </summary>
        /// <param name="o">The object to match.</param>
        /// <returns>Whether the object does not matche the wrapped matcher.</returns>
        public override bool Matches(object o)
        {
            return !this.negated.Matches(o);
        }

        /// <summary>
        /// Describes this object.
        /// </summary>
        /// <param name="writer">The text writer the description is added to.</param>
        public override void DescribeTo(TextWriter writer)
        {
            writer.Write("not ");
            this.negated.DescribeTo(writer);
        }
    }
}
