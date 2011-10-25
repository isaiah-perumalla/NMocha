//-----------------------------------------------------------------------
// <copyright file="TypeMatcher.cs" company="NMock2">
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
    using System;
    using System.IO;

    /// <summary>
    /// Matcher that checks whether the actual object can be assigned to the expected type.
    /// </summary>
    public class TypeMatcher : Matcher
    {
        private readonly Type type;

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeMatcher"/> class.
        /// </summary>
        /// <param name="type">The expected type.</param>
        public TypeMatcher(Type type)
        {
            this.type = type;
        }

        /// <summary>
        /// Matches the specified object to this matcher and returns whether it matches.
        /// </summary>
        /// <param name="o">The object to match.</param>
        /// <returns>Whether the object castable to the expected type.</returns>
        public override bool Matches(object o)
        {
            return this.type.IsAssignableFrom(o.GetType()); 
        }

        /// <summary>
        /// Describes this object.
        /// </summary>
        /// <param name="writer">The text writer the description is added to.</param>
        public override void DescribeTo(TextWriter writer)
        {
            writer.Write("type assignable to ");
            writer.Write(this.type);
        }
    }
}
