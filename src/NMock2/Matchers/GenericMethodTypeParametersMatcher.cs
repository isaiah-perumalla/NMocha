//-----------------------------------------------------------------------
// <copyright file="GenericMethodTypeParametersMatcher.cs" company="NMock2">
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
    using System.Reflection;
    using NMock2.Monitoring;

    /// <summary>
    /// Matcher that checks whether parameters of a method match with the specified list of matchers.
    /// </summary>
    public class GenericMethodTypeParametersMatcher : Matcher
    {
        /// <summary>
        /// An ordered list of type <see cref="Matcher"/>'s each matching a single method argument.
        /// </summary>
        private readonly Matcher[] typeMatchers;

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericMethodTypeParametersMatcher"/> class.
        /// </summary>
        /// <param name="typeMatchers">The value matchers. This is an ordered list of matchers, each matching a single method argument.</param>
        public GenericMethodTypeParametersMatcher(params Matcher[] typeMatchers)
        {
            this.typeMatchers = typeMatchers;
        }

        /// <summary>
        /// Matches the specified object to this matcher and returns whether it matches.
        /// </summary>
        /// <param name="o">The object to match.</param>
        /// <returns>Whether the object is an <see cref="Invocation"/> and all method arguments match their corresponding matcher.</returns>
        public override bool Matches(object o)
        {
            return o is Invocation && this.MatchesTypes((Invocation)o);
        }
        
        /// <summary>
        /// Describes this object.
        /// </summary>
        /// <param name="writer">The text writer the description is added to.</param>
        public override void DescribeTo(TextWriter writer)
        {
            writer.Write("<");
            this.WriteListOfMatchers(this.MatcherCount(), writer);
            writer.Write(">");
        }

        /// <summary>
        /// Number of argument matchers.
        /// </summary>
        /// <returns>Returns the number of argument matchers.</returns>
        protected int MatcherCount()
        {
            return this.typeMatchers.Length;
        }

        /// <summary>
        /// Returns the last argument matcher.
        /// </summary>
        /// <returns>Argument matcher</returns>
        protected Matcher LastMatcher()
        {
            return this.typeMatchers[this.typeMatchers.Length - 1];
        }

        /// <summary>
        /// Writes the list of matchers to a <see cref="TextWriter"/>.
        /// </summary>
        /// <param name="listLength">Length of the list.</param>
        /// <param name="writer">The writer.</param>
        protected void WriteListOfMatchers(int listLength, TextWriter writer)
        {
            for (int i = 0; i < listLength; i++)
            {
                if (i > 0)
                {
                    writer.Write(", ");
                }

                this.typeMatchers[i].DescribeTo(writer);
            }
        }

        /// <summary>
        /// Determines whether the arguments of the invocation matches the initial arguments.
        /// </summary>
        /// <param name="invocation">The invocation to match against the initial arguments.</param>
        /// <returns>
        /// Returns true if invocation matches the initial arguments; false otherwise.
        /// </returns>
        private bool MatchesTypes(Invocation invocation)
        {
            return invocation.Method.GetGenericArguments().Length == this.typeMatchers.Length
                   && this.MatchesTypeValues(invocation);
        }

        /// <summary>
        /// Determines whether all argument types of the generic method matches the invocation.
        /// </summary>
        /// <param name="invocation">The invocation to match against the initial argument types.</param>
        /// <returns>
        /// Returns true if invocation types matches the inital argument types; false otherwise.
        /// </returns>
        private bool MatchesTypeValues(Invocation invocation)
        {
            Type[] types = invocation.Method.GetGenericArguments();

            for (int i = 0; i < types.Length; i++)
            {
                if (!this.typeMatchers[i].Matches(types[i]))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
