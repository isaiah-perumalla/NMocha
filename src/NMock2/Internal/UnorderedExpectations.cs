//-----------------------------------------------------------------------
// <copyright file="UnorderedExpectations.cs" company="NMock2">
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
using System;
using System.Collections.Generic;
using System.IO;
using NMock2.Monitoring;

namespace NMock2.Internal {
    public class UnorderedExpectations : IExpectationOrdering {
        /// <summary>
        /// Stores the calling depth for the document writer output.
        /// </summary>
        private readonly int depth;

        /// <summary>
        /// Stores the expectations that could be added.
        /// </summary>
        private readonly List<IExpectation> expectations = new List<IExpectation>();

        /// <summary>
        /// Stores the string to be presented whe describing the expectation.
        /// </summary>
        private readonly string prompt;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnorderedExpectations"/> class.
        /// </summary>
        public UnorderedExpectations() {
            depth = 0;
            prompt = "Expected:";
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnorderedExpectations"/> class.
        /// </summary>
        /// <param name="depth">The calling depth.</param>
        public UnorderedExpectations(int depth) {
            this.depth = depth;
            prompt = "Unordered:";
        }

        #region IExpectationOrdering Members

        /// <summary>
        /// Gets a value indicating whether this instance is active.
        /// </summary>
        /// <value><c>true</c> if this instance is active; otherwise, <c>false</c>.</value>
        public bool IsActive {
            get {
                foreach (IExpectation e in expectations)
                {
                    if (e.IsActive)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance has been met.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance has been met; otherwise, <c>false</c>.
        /// </value>
        public bool HasBeenMet {
            get {
                foreach (IExpectation e in expectations)
                {
                    if (!e.HasBeenMet)
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        /// <summary>
        /// Checks whether stored expectations matches the specified invocation.
        /// </summary>
        /// <param name="invocation">The invocation to check.</param>
        /// <returns>Returns whether one of the stored expectations has met the specified invocation.</returns>
        public bool Matches(Invocation invocation) {
            foreach (IExpectation e in expectations)
            {
                if (e.Matches(invocation))
                {
                    return true;
                }
            }

            return false;
        }

        public bool MatchesIgnoringIsActive(Invocation invocation) {
            foreach (IExpectation e in expectations)
            {
                if (e.MatchesIgnoringIsActive(invocation))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Performs the specified invocation on the corresponding expectation if a match was found.
        /// </summary>
        /// <param name="invocation">The invocation to match.</param>
        public void Perform(Invocation invocation) {
            foreach (IExpectation e in expectations)
            {
                if (e.Matches(invocation))
                {
                    e.Perform(invocation);
                    return;
                }
            }

            throw new InvalidOperationException("No matching expectation");
        }

        public void DescribeActiveExpectationsTo(IDescription  writer) {
            writer.AppendLine(prompt);
            foreach (IExpectation expectation in expectations)
            {
                if (expectation.IsActive)
                {
                    Indent(writer, depth + 1);
                    expectation.DescribeActiveExpectationsTo(writer);
                    writer.AppendNewLine();
                }
            }
        }

        public void DescribeUnmetExpectationsTo(IDescription writer) {
            writer.AppendLine(prompt);
            foreach (IExpectation expectation in expectations)
            {
                   Indent(writer, depth + 1);
                    expectation.DescribeUnmetExpectationsTo(writer);
                    writer.AppendNewLine();
                
            }
        }

        /// <summary>
        /// Adds all expectations to <paramref name="result"/> that are associated to <paramref name="mock"/>.
        /// </summary>
        /// <param name="mock">The mock for which expectations are queried.</param>
        /// <param name="result">The result to add matching expectations to.</param>
        public void QueryExpectationsBelongingTo(IMockObject mock, IList<IExpectation> result) {
            expectations.ForEach(expectation => expectation.QueryExpectationsBelongingTo(mock, result));
        }

        public void AddExpectation(IExpectation expectation) {
            expectations.Add(expectation);
        }

        public void RemoveExpectation(IExpectation expectation) {
            expectations.Remove(expectation);
        }

        #endregion

        private void Indent(IDescription writer, int n) {
            for (int i = 0; i < n; i++)
            {
                writer.AppendText("  ");
            }
        }
    }
}