//-----------------------------------------------------------------------
// <copyright file="OrderedExpectations.cs" company="NMock2">
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
namespace NMock2.Internal
{
    using System.Collections.Generic;
    using System.IO;
    using NMock2.Monitoring;

    public class OrderedExpectations : IExpectationOrdering
    {
        private readonly List<IExpectation> expectations = new List<IExpectation>();
        private int current = 0;
        private int depth;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderedExpectations"/> class.
        /// </summary>
        /// <param name="depth">The depth.</param>
        public OrderedExpectations(int depth)
        {
            this.depth = depth;
        }

        /// <summary>
        /// Gets a value indicating whether this instance is active.
        /// </summary>
        /// <value><c>true</c> if this instance is active; otherwise, <c>false</c>.</value>
        public bool IsActive
        {
            get { return this.expectations.Count > 0 && this.CurrentExpectation.IsActive; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance has been met.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has been met; otherwise, <c>false</c>.
        /// </value>
        public bool HasBeenMet
        {
            get
            {
                // Count == 0 fixes issue 1912662 of NMock
                // (http://sourceforge.net/tracker/index.php?func=detail&aid=1912662&group_id=66591&atid=515017)
                return this.expectations.Count == 0
                    || (this.CurrentExpectation.HasBeenMet && this.NextExpectationHasBeenMet());
            }
        }

        /// <summary>
        /// Gets the current expectation.
        /// </summary>
        /// <value>The current expectation.</value>
        private IExpectation CurrentExpectation
        {
            get { return this.expectations[this.current]; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance has next expectation.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance has next expectation; otherwise, <c>false</c>.
        /// </value>
        private bool HasNextExpectation
        {
            get { return this.current < this.expectations.Count - 1; }
        }

        /// <summary>
        /// Gets the next expectation.
        /// </summary>
        /// <value>The next expectation.</value>
        private IExpectation NextExpectation
        {
            get { return this.expectations[this.current + 1]; }
        }

        /// <summary>
        /// Checks whether stored expectations matches the specified invocation.
        /// </summary>
        /// <param name="invocation">The invocation to check.</param>
        /// <returns>Returns whether one of the stored expectations has met the specified invocation.</returns>
        public bool Matches(Invocation invocation)
        {
            return this.expectations.Count != 0 &&
                   (this.CurrentExpectation.Matches(invocation) ||
                       (this.CurrentExpectation.HasBeenMet && this.NextExpectationMatches(invocation)));
        }

        public bool MatchesIgnoringIsActive(Invocation invocation)
        {
            return this.expectations.Count != 0 &&
                   (this.CurrentExpectation.MatchesIgnoringIsActive(invocation) ||
                       (this.CurrentExpectation.HasBeenMet && this.NextExpectationMatchesIgnoringIsActive(invocation)));
        }

        public void AddExpectation(IExpectation expectation)
        {
            this.expectations.Add(expectation);
        }

        public void RemoveExpectation(IExpectation expectation)
        {
            this.expectations.Remove(expectation);
        }

        public void Perform(Invocation invocation)
        {
            // If the current expectation doesn't match, it must have been met, by the contract
            // for the IExpectation interface and due to the implementation of this.Matches
            if (!this.CurrentExpectation.Matches(invocation))
            {
                this.current++;
            }

            this.CurrentExpectation.Perform(invocation);
        }

        public void DescribeActiveExpectationsTo(TextWriter writer)
        {
            writer.WriteLine("Ordered:");
            for (int i = 0; i < this.expectations.Count; i++)
            {
                IExpectation expectation = (IExpectation)this.expectations[i];

                if (expectation.IsActive)
                {
                    this.Indent(writer, this.depth + 1);
                    expectation.DescribeActiveExpectationsTo(writer);
                    writer.WriteLine();
                }
            }
        }

        public void DescribeUnmetExpectationsTo(TextWriter writer)
        {
            writer.WriteLine("Ordered:");
            for (int i = 0; i < this.expectations.Count; i++)
            {
                IExpectation expectation = (IExpectation)this.expectations[i];

                if (!expectation.HasBeenMet)
                {
                    this.Indent(writer, this.depth + 1);
                    expectation.DescribeUnmetExpectationsTo(writer);
                    writer.WriteLine();
                }
            }
        }

        /// <summary>
        /// Adds all expectations to <paramref name="result"/> that are associated to <paramref name="mock"/>.
        /// </summary>
        /// <param name="mock">The mock for which expectations are queried.</param>
        /// <param name="result">The result to add matching expectations to.</param>
        public void QueryExpectationsBelongingTo(IMockObject mock, IList<IExpectation> result)
        {
            this.expectations.ForEach(expectation => expectation.QueryExpectationsBelongingTo(mock, result));
        }

        private bool NextExpectationHasBeenMet()
        {
            return (!this.HasNextExpectation) || this.NextExpectation.HasBeenMet;
        }

        private bool NextExpectationMatches(Invocation invocation)
        {
            return this.HasNextExpectation && this.NextExpectation.Matches(invocation);
        }

        private bool NextExpectationMatchesIgnoringIsActive(Invocation invocation)
        {
            return this.HasNextExpectation && this.NextExpectation.MatchesIgnoringIsActive(invocation);
        }
        
        private void Indent(TextWriter writer, int n)
        {
            for (int i = 0; i < n; i++)
            {
                writer.Write("  ");
            }
        }
    }
}
