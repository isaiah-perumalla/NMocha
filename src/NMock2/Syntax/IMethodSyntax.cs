//-----------------------------------------------------------------------
// <copyright file="IMethodSyntax.cs" company="NMock2">
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
using System.Reflection;

namespace NMock2.Syntax {
    /// <summary>
    /// Syntax defining a method, property or event (de)registration.
    /// </summary>
    public interface IMethodSyntax {
        /// <summary>
        /// Gets an indexer (get operation).
        /// </summary>
        /// <value>Get indexer syntax defining the value returned by the indexer.</value>
        IGetIndexerSyntax Get { get; }

        /// <summary>
        /// Gets an indexer (set operation).
        /// </summary>
        /// <value>Set indexer syntax defining the value the indexer is set to.</value>
        ISetIndexerSyntax Set { get; }

        /// <summary>
        /// Defines a method.
        /// </summary>
        /// <param name="name">The name of the method.</param>
        /// <param name="typeParams">The generic type params to match.</param>
        /// <returns>
        /// Argument syntax defining the arguments of the method.
        /// </returns>
        IArgumentSyntax Message(string name, params Type[] typeParams);

        /// <summary>
        /// Defines a method.
        /// </summary>
        /// <param name="nameMatcher">Matcher defining the method.</param>
        /// <param name="typeParams">The generic type params to match.</param>
        /// <returns>Argument syntax defining the arguments of the method.</returns>
        IArgumentSyntax Message(Matcher nameMatcher, params Type[] typeParams);

        /// <summary>
        /// Defines a method.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <param name="typeParams">The generic type params to match.</param>
        /// <returns>Argument syntax defining the arguments of the method.</returns>
        IArgumentSyntax Message(MethodInfo method, params Type[] typeParams);


        /// <summary>
        /// Defines a method.
        /// </summary>
        /// <param name="methodMatcher">Matcher for matching the method on an invocation.</param>
        /// <param name="typeParamsMatcher">Matchers for matching type parameters.</param>
        /// <returns>Argument syntax defining the arguments of the method.</returns>
        IArgumentSyntax Message(Matcher methodMatcher, Matcher typeParamsMatcher);

        /// <summary>
        /// Defines a property setter.
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <returns>Value syntax defining the value of the property.</returns>
        IValueSyntax SetProperty(string name);

        /// <summary>
        /// Defines a property getter.
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <returns>Match Syntax defining the property behavior.</returns>
        IMatchSyntax GetProperty(string name);
    }
}