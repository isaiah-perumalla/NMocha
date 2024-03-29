//-----------------------------------------------------------------------
// <copyright file="Matcher.cs" company="NMock2">
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
using NMocha.Internal;
using NMock2;
using NMock2.Matchers;

namespace NMocha {
    /// <summary>
    /// A matcher is used to match objects against it.
    /// </summary>
    public abstract class Matcher : ISelfDescribing {
        #region ISelfDescribing Members

        /// <summary>
        /// Describes this object.
        /// </summary>
        /// <param name="description"></param>
        public abstract void DescribeOn(IDescription description);

        #endregion

        /// <summary>
        /// Logical and of to matchers.
        /// </summary>
        /// <param name="m1">First matcher.</param>
        /// <param name="m2">Second matcher.</param>
        /// <returns>Matcher combining the two operands.</returns>
        public static Matcher operator &(Matcher m1, Matcher m2) {
            return new AndMatcher(m1, m2);
        }

        /// <summary>
        /// Logical or of to matchers.
        /// </summary>
        /// <param name="m1">First matcher.</param>
        /// <param name="m2">Second matcher.</param>
        /// <returns>Matcher combining the two operands.</returns>
        public static Matcher operator |(Matcher m1, Matcher m2) {
            return new OrMatcher(m1, m2);
        }

        /// <summary>
        /// Negation of a matcher.
        /// </summary>
        /// <param name="m">Matcher to negate.</param>
        /// <returns>Negation of the specified matcher.</returns>
        public static Matcher operator !(Matcher m) {
            return new NotMatcher(m);
        }

        /// <summary>
        /// Matches the specified object to this matcher and returns whether it matches.
        /// </summary>
        /// <param name="o">The object to match.</param>
        /// <returns>Whether the object matches.</returns>
        public abstract bool Matches(object o);

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString() {
            var writer = new StringDescriptionWriter();
            DescribeOn(writer);
            return writer.ToString();
        }
    }
}