//-----------------------------------------------------------------------
// <copyright file="GenericMatcher.cs" company="NMock2">
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
    /// Matcher that checks whether a value matches the check provided as a delegate.
    /// the expectation.
    /// </summary>
    /// <typeparam name="T">The type of the expected value.</typeparam>
    public class GenericMatcher<T> : Matcher
    {
        /// <summary>
        /// The test that is performed to see if the value matches the expectation.
        /// </summary>
        private readonly MatchExpression matchExpression;

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericMatcher{T}"/> class.
        /// </summary>
        /// <param name="matchExpression">The test that is performed to check if the value matches expectation.</param>
        /// <exception cref="ArgumentNullException"><c>matchExpression</c> is null.</exception>
        public GenericMatcher(MatchExpression matchExpression)
        {
            if (matchExpression == null)
            {
                throw new ArgumentNullException("matchExpression", "matchExpression must not be null.");
            }

            this.matchExpression = matchExpression;
        }

        /// <summary>
        /// The test that is performed to check if the <paramref name="value"/> matches the expectation.
        /// </summary>
        /// <param name="value">The actually received value.</param>
        /// <returns>True then value matches the expectation.</returns>
        public delegate bool MatchExpression(T value);

        /// <summary>
        /// Matches the specified object to this matcher and returns whether it matches.
        /// </summary>
        /// <param name="o">The object to match.</param>
        /// <returns>Whether the object matches.</returns>
        public override bool Matches(object o)
        {
            return o is T && this.matchExpression((T)o);
        }

        /// <summary>
        /// Describes this object.
        /// </summary>
        /// <param name="writer">The text writer the description is added to.</param>
        public override void DescribeTo(TextWriter writer)
        {
            writer.Write("generic match");
        }
    }
}