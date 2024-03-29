//-----------------------------------------------------------------------
// <copyright file="IExpectation.cs" company="NMock2">
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
using NMocha.Monitoring;

namespace NMocha {
    /// <summary>
    /// Represents an expectation.
    /// </summary>
    public interface IExpectation {
        /// <summary>
        /// Gets a value indicating whether this instance is active.
        /// </summary>
        /// <value><c>true</c> if this instance is active; otherwise, <c>false</c>.</value>
        bool IsActive { get; }

        /// <summary>
        /// Gets a value indicating whether this instance has been met.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance has been met; otherwise, <c>false</c>.
        /// </value>
        bool HasBeenMet { get; }

        /// <summary>
        /// Checks whether stored expectations matches the specified invocation.
        /// </summary>
        /// <param name="invocation">The invocation to check.</param>
        /// <returns>Returns whether one of the stored expectations has met the specified invocation.</returns>
        bool Matches(Invocation invocation);

        /// <summary>
        /// Matcheses the ignoring is active.
        /// </summary>
        /// <param name="invocation">The invocation.</param>
        /// <returns></returns>
        bool MatchesIgnoringIsActive(Invocation invocation);

        /// <summary>
        /// Performs the specified invocation.
        /// </summary>
        /// <param name="invocation">The invocation.</param>
        void Perform(Invocation invocation);

        /// <summary>
        /// Describes the active expectations to.
        /// </summary>
        /// <param name="writer">The writer.</param>
        void DescribeActiveExpectationsTo(IDescription writer);

        /// <summary>
        /// Describes the unmet expectations to.
        /// </summary>
        /// <param name="writer">The writer.</param>
        void DescribeUnmetExpectationsTo(IDescription writer);
    }
}