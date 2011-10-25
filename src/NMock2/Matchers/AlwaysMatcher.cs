//-----------------------------------------------------------------------
// <copyright file="AlwaysMatcher.cs" company="NMock2">
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
    /// A matcher that will always or never match independent of the value matched but depending on how it is initialized.
    /// </summary>
    public class AlwaysMatcher : Matcher
    {
        /// <summary>
        /// Stores the matcher value which was given at initialization.
        /// </summary>
        private readonly bool matches;

        /// <summary>
        /// Stores the description which was given at initialization.
        /// </summary>
        private readonly string description;

        /// <summary>
        /// Initializes a new instance of the <see cref="AlwaysMatcher"/> class.
        /// </summary>
        /// <param name="matches">if set to <c>true</c> the matcher will always match, otherwise it will never match.</param>
        /// <param name="description">The description which will be printed out when calling <see cref="DescribeTo"/>.</param>
        public AlwaysMatcher(bool matches, string description)
        {
            this.matches = matches;
            this.description = description;
        }

        /// <summary>
        /// Matches the specified object to this matcher and returns whether it matches.
        /// </summary>
        /// <param name="o">The object to match.</param>
        /// <returns>Returns whether the object matches.</returns>
        public override bool Matches(object o)
        {
            return this.matches;
        }

        /// <summary>
        /// Describes this object.
        /// </summary>
        /// <param name="writer">The text writer the description is added to.</param>
        public override void DescribeTo(TextWriter writer)
        {
            writer.Write(this.description);
        }
    }
}
