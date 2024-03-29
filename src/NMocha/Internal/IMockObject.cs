//-----------------------------------------------------------------------
// <copyright file="IMockObject.cs" company="NMock2">
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
using System.Collections.Generic;
using System.Reflection;
using NMock2;

namespace NMocha.Internal {
    /// <summary>
    /// Interface for mocks.
    /// </summary>
    public interface IMockObject {
        // Q: What happens if any of these members are also defined on class/interface to be mocked???
        // - Implement explicitly?

        /// <summary>
        /// Gets the name of the mock instance. This is often used in error messages
        /// to identify a specific mock instance.
        /// </summary>
        string MockName { get; }

        /// <summary>
        /// Determines if this mock has a particular method.
        /// </summary>
        /// <param name="methodMatcher">A Matcher to use in identifying the method.</param>
        /// <returns>True if a matching method exists, otherwise false.</returns>
        bool HasMethodMatching(Matcher methodMatcher);

        /// <summary>
        /// Retrieves all matching methods on this mock. 
        /// </summary>
        /// <param name="methodMatcher">A Matcher to use in identifying the methods.</param>
        /// <returns>A list of zero or more matching MethodInfo instances.</returns>
        IList<MethodInfo> GetMethodsMatching(Matcher methodMatcher);

        /// <summary>
        /// Adds an expectation to this mock.
        /// </summary>
        /// <param name="expectation">The expectation to add.</param>
        void  AddExpectation(IExpectation expectation);
    }
}