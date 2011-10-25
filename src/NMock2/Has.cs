//-----------------------------------------------------------------------
// <copyright file="Has.cs" company="NMock2">
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
namespace NMock2
{
    using NMock2.Matchers;

    /// <summary>
    /// Provides shortcuts to matchers.
    /// </summary>
    public class Has
    {
        /// <summary>
        /// Returns a matcher for testing string representation of objects.
        /// </summary>
        /// <param name="matcher">The wrapped matcher.</param>
        /// <returns>Returns a <see cref="ToStringMatcher"/> for testing string representation of objects.</returns>
        public static Matcher ToString(Matcher matcher)
        {
            return new ToStringMatcher(matcher);
        }

        /// <summary>
        /// Returns a matcher for checking property values.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="valueMatcher">The value matcher.</param>
        /// <returns>Returns a <see cref="PropertyMatcher"/> for checking property values.</returns>
        public static Matcher Property(string propertyName, Matcher valueMatcher)
        {
            return new PropertyMatcher(propertyName, valueMatcher);
        }

        /// <summary>
        /// Returns a matcher for checking field values.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="valueMatcher">The value matcher.</param>
        /// <returns>Returns a <see cref="FieldMatcher"/> for checking field values.</returns>
        public static Matcher Field(string fieldName, Matcher valueMatcher)
        {
            return new FieldMatcher(fieldName, valueMatcher);
        }
    }
}

