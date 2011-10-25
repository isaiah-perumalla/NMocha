//-----------------------------------------------------------------------
// <copyright file="IMockDefinitionSyntax.cs" company="NMock2">
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
namespace NMock2.Syntax
{
    using System;

    /// <summary>
    /// Syntax describing the initial characteristics of a new mock object.
    /// </summary>
    public interface IMockDefinitionSyntax : IMockDefinition
    {
        /// <summary>
        /// Specifies a type that this mock should implement. This may be a class or interface,
        /// but there can only be a maximum of one class implemented by a mock.
        /// </summary>
        /// <typeparam name="T">The type to implement.</typeparam>
        /// <returns>The mock object definition.</returns>
        IMockDefinitionSyntax Implementing<T>();

        /// <summary>
        /// Specifies the types that this mock should implement. These may be a class or interface,
        /// but there can only be a maximum of one class implemented by a mock.
        /// </summary>
        /// <param name="types">The types to implement.</param>
        /// <returns>The mock object definition.</returns>
        IMockDefinitionSyntax Implementing(params Type[] types);

        /// <summary>
        /// Specifies how the mock object should behave when first created.
        /// It is invalid to set the MockStyle of a mock more than once.
        /// </summary>
        /// <param name="style">A MockStyle value.</param>
        /// <returns>The mock object definition.</returns>
        IMockDefinitionSyntax OfStyle(MockStyle style);

        /// <summary>
        /// Specifies the arguments for the constructor of the class to be mocked.
        /// Only applicable when mocking a class with a non-default constructor.
        /// It is invalid to specify the constructor arguments of a mock more than once.
        /// </summary>
        /// <param name="args">The arguments for the class constructor.</param>
        /// <returns>The mock object definition.</returns>
        IMockDefinitionSyntax WithArgs(params object[] args);

        /// <summary>
        /// Specifies a name for the mock. This will be used in error messages,
        /// and as the return value of ToString() if not mocking a class.
        /// It is invalid to specify the name of a mock more than once.
        /// </summary>
        /// <param name="name">The name for the mock.</param>
        /// <returns>The mock object definition.</returns>
        IMockDefinitionSyntax Named(string name);
    }
}