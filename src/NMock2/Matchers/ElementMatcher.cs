//-----------------------------------------------------------------------
// <copyright file="ElementMatcher.cs" company="NMock2">
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
    using System.Collections;
    using System.IO;

    /// <summary>
    /// Matcher that checks whether a single object is in a collection of elements.
    /// </summary>
    public class ElementMatcher : Matcher
    {
        private readonly ICollection collection;

        /// <summary>
        /// Initializes a new instance of the <see cref="ElementMatcher"/> class.
        /// </summary>
        /// <param name="collection">The collection to match against.</param>
        public ElementMatcher(ICollection collection)
        {
            this.collection = collection;
        }

        /// <summary>
        /// Matches the specified object to this matcher and returns whether it matches.
        /// </summary>
        /// <param name="actual">The object to match.</param>
        /// <returns>Whether to object matches.</returns>
        public override bool Matches(object actual)
        {
            foreach (object element in this.collection)
            {
                if (Equals(element, actual))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Describes this object.
        /// </summary>
        /// <param name="writer">The text writer the description is added to.</param>
        public override void DescribeTo(TextWriter writer)
        {
            writer.Write("element of [");

            bool separate = false;
            foreach (object element in this.collection)
            {
                if (separate)
                {
                    writer.Write(", ");
                }

                writer.Write(element);
                separate = true;
            }

            writer.Write("]");
        }
    }
}
