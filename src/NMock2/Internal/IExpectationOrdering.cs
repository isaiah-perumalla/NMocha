//-----------------------------------------------------------------------
// <copyright file="IExpectationOrdering.cs" company="NMock2">
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
namespace NMock2.Internal {
    /// <summary>
    /// Represents expectations (order or unordered).
    /// </summary>
    public interface IExpectationOrdering : IExpectation {
        /// <summary>
        /// Adds an expectation.
        /// </summary>
        /// <param name="expectation">The expectation to add.</param>
        void AddExpectation(IExpectation expectation);

        /// <summary>
        /// Removes the specified expectation.
        /// </summary>
        /// <param name="expectation">The expectation to remove.</param>
        void RemoveExpectation(IExpectation expectation);
    }
}