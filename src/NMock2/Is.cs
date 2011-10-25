//-----------------------------------------------------------------------
// <copyright file="Is.cs" company="NMock2">
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
    using System;
    using System.Collections;
    using NMock2.Matchers;

    /// <summary>
    /// Provides shortcuts to <see cref="Matcher"/>s.
    /// </summary>
    public class Is
    {
        /// <summary>
        /// Matches anything.
        /// </summary>
        public static readonly Matcher Anything = new AlwaysMatcher(true, "anything");

        /// <summary>
        /// Matches nothing.
        /// </summary>
        public static readonly Matcher Nothing = new AlwaysMatcher(false, "nothing");
        
        /// <summary>
        /// Matches if the value is null.
        /// </summary>
        public static readonly Matcher Null = new NullMatcher();

        /// <summary>
        /// Matches if the value is not null.
        /// </summary>
        public static readonly Matcher NotNull = new NotMatcher(Null);

        /// <summary>
        /// Matches out parameters of methods.
        /// </summary>
        public static readonly Matcher Out = new ArgumentsMatcher.OutMatcher();

        /// <summary>
        /// Matches objects the are equal to the expected object.
        /// <seealso cref="Same"/>
        /// </summary>
        /// <param name="expected">The expected.</param>
        /// <returns>Returns a new instance of the <see cref="EqualMatcher"/> class.</returns>
        public static Matcher EqualTo(object expected)
        {
            return new EqualMatcher(expected);
        }

        /// <summary>
        /// Matches an expected object.
        /// <seealso cref="EqualTo"/>
        /// </summary>
        /// <param name="expected">The expected object.</param>
        /// <returns>Returns a new instance of the <see cref="SameMatcher"/> class.</returns>
        public static Matcher Same(object expected)
        {
            return new SameMatcher(expected);
        }

        /// <summary>
        /// Matches strings containing the specified <paramref name="substring"/>.
        /// </summary>
        /// <param name="substring">The substring.</param>
        /// <returns>Returns a new instance of the <see cref="StringContainsMatcher"/> class.</returns>
        public static Matcher StringContaining(string substring)
        {
            return new StringContainsMatcher(substring);
        }

        /// <summary>
        /// Matches objects that are greater than <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The value to compare.</param>
        /// <returns>Returns a new instance of the <see cref="ComparisonMatcher"/> class.</returns>
        public static Matcher GreaterThan(IComparable value)
        {
            return new ComparisonMatcher(value, 1, 1);
        }

        /// <summary>
        /// Matches objects that are at least equal to <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The value to compare.</param>
        /// <returns>Returns a new instance of the <see cref="ComparisonMatcher"/> class.</returns>
        public static Matcher AtLeast(IComparable value)
        {
            return new ComparisonMatcher(value, 0, 1);
        }

        /// <summary>
        /// Matches objects less than <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The value to compare.</param>
        /// <returns>Returns a new instance of the <see cref="ComparisonMatcher"/> class.</returns>
        public static Matcher LessThan(IComparable value)
        {
            return new ComparisonMatcher(value, -1, -1);
        }

        /// <summary>
        /// Matches objects that are less or equal to <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The value to compare.</param>
        /// <returns>Returns a new instance of the <see cref="ComparisonMatcher"/> class.</returns>
        public static Matcher AtMost(IComparable value)
        {
            return new ComparisonMatcher(value, -1, 0);
        }

        /// <summary>
        /// Matches objects in the specified collection.
        /// </summary>
        /// <param name="collection">The collection with objects to match.</param>
        /// <returns>Returns a new instance of the <see cref="ElementMatcher"/> class.</returns>
        public static Matcher In(ICollection collection)
        {
            return new ElementMatcher(collection);
        }

        /// <summary>
        /// Matches objects in the specified elements.
        /// </summary>
        /// <param name="elements">The elements to match.</param>
        /// <returns>Returns a new instance of the <see cref="ElementMatcher"/> class.</returns>
        public static Matcher OneOf(params object[] elements)
        {
            return new ElementMatcher(elements);
        }

        /// <summary>
        /// Matches objects of the specified type.
        /// </summary>
        /// <param name="type">The type to match.</param>
        /// <returns>Returns a new instance of the <see cref="TypeMatcher"/> class.</returns>
        public static Matcher TypeOf(Type type)
        {
            return new TypeMatcher(type);
        }

        /// <summary>
        /// Matches objects of the specified type.
        /// </summary>
        /// <typeparam name="T">The type to match.</typeparam>
        /// <returns>
        /// Returns a new instance of the <see cref="TypeMatcher"/> class.
        /// </returns>
        public static Matcher TypeOf<T>()
        {
            return new TypeMatcher(typeof(T));
        }

        /// <summary>
        /// Matches objects against the specified expression.
        /// </summary>
        /// <typeparam name="T">Type of the value to match.</typeparam>
        /// <param name="expression">The match expression.</param>
        /// <returns>returns a new instance of the <see cref="GenericMatcher{T}"/>.</returns>
        public static Matcher Match<T>(GenericMatcher<T>.MatchExpression expression)
        {
            return new GenericMatcher<T>(expression);
        }
    }
}

