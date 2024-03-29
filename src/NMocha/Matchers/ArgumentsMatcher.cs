﻿//-----------------------------------------------------------------------
// <copyright file="ArgumentsMatcher.cs" company="NMock2">
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
using System.IO;
using System.Reflection;
using NMocha;
using NMocha.Monitoring;
using NMock2.Monitoring;

namespace NMock2.Matchers {
    /// <summary>
    /// Matcher that checks whether parameters of a method match with the specified list of matchers.
    /// </summary>
    public class ArgumentsMatcher : Matcher {
        /// <summary>
        /// Stores the out parameter.
        /// </summary>
        private static readonly object OutParameter = new object();

        /// <summary>
        /// Stores the valuematchers given at initialization.
        /// </summary>
        private readonly Matcher[] valueMatchers;

        /// <summary>
        /// Initializes a new instance of the <see cref="ArgumentsMatcher"/> class.
        /// </summary>
        /// <param name="valueMatchers">The value matchers. This is an ordered list of matchers, each matching a single method argument.</param>
        public ArgumentsMatcher(params Matcher[] valueMatchers) {
            this.valueMatchers = valueMatchers;
        }

        /// <summary>
        /// Matches the specified object to this matcher and returns whether it matches.
        /// </summary>
        /// <param name="o">The object to match.</param>
        /// <returns>Whether the object is an <see cref="Invocation"/> and all method arguments match their corresponding matcher.</returns>
        public override bool Matches(object o) {
            return o is Invocation && MatchesArguments((Invocation) o);
        }

        /// <summary>
        /// Describes this object.
        /// </summary>
        /// <param name="description"></param>
        public override void DescribeOn(IDescription description) {
            description.AppendText("(");
            WriteListOfMatchers(MatcherCount(), description);
            description.AppendText(")");
        }

        /// <summary>
        /// Number of argument matchers.
        /// </summary>
        /// <returns>Returns the number of argument matchers.</returns>
        protected int MatcherCount() {
            return valueMatchers.Length;
        }

        /// <summary>
        /// Returns the last argument matcher.
        /// </summary>
        /// <returns>Argument matcher</returns>
        protected Matcher LastMatcher() {
            return valueMatchers[valueMatchers.Length - 1];
        }

        /// <summary>
        /// Writes the list of matchers to a <see cref="TextWriter"/>.
        /// </summary>
        /// <param name="listLength">Length of the list.</param>
        /// <param name="writer">The writer.</param>
        protected void WriteListOfMatchers(int listLength, IDescription writer) {
            for (int i = 0; i < listLength; i++)
            {
                if (i > 0)
                {
                    writer.AppendText(", ");
                }

                valueMatchers[i].DescribeOn(writer);
            }
        }

        private bool MatchesArguments(Invocation invocation) {
            return invocation.Parameters.Count == valueMatchers.Length
                   && MatchesArgumentValues(invocation);
        }

        private bool MatchesArgumentValues(Invocation invocation) {
            ParameterInfo[] paramsInfo = invocation.Method.GetParameters();

            for (int i = 0; i < invocation.Parameters.Count; i++)
            {
                object value = paramsInfo[i].IsOut ? OutParameter : invocation.Parameters[i];

                if (!valueMatchers[i].Matches(value))
                {
                    return false;
                }
            }

            return true;
        }

        #region Nested type: OutMatcher

        /// <summary>
        /// Matcher that matches method out parameters. 
        /// </summary>
        public class OutMatcher : Matcher {
            /// <summary>
            /// Matches the specified object to this matcher and returns whether it matches.
            /// </summary>
            /// <param name="o">The object to match.</param>
            /// <returns>Whether the object mached is an out parameter.</returns>
            public override bool Matches(object o) {
                return o == OutParameter;
            }

            /// <summary>
            /// Describes this object.
            /// </summary>
            /// <param name="description"></param>
            public override void DescribeOn(IDescription description) {
                description.AppendText("out");
            }
        }

        #endregion
    }
}