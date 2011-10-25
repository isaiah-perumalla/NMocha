//-----------------------------------------------------------------------
// <copyright file="DefinedAs.cs" company="NMock2">
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

    using Internal;
    using Syntax;

    /// <summary>
    /// Defines the initial characteristics of a new mock object.
    /// This is normally used in conjunction with <see cref="Mockery.NewMock&lt;T&gt;(IMockDefinition)"/>
    /// </summary>
    public static class DefinedAs
    {
        /// <summary>
        /// Specifies a type that this mock should implement. This may be a class or interface,
        /// but there can only be a maximum of one class implemented by a mock.
        /// </summary>
        /// <typeparam name="T">The type to implement.</typeparam>
        /// <returns>The mock object definition.</returns>
        public static IMockDefinitionSyntax Implementing<T>()
        {
            return new MockBuilder().Implementing<T>();
        }

        /// <summary>
        /// Specifies the types that this mock should implement. These may be a class or interface,
        /// but there can only be a maximum of one class implemented by a mock.
        /// </summary>
        /// <param name="types">The types to implement.</param>
        /// <returns>The mock object definition.</returns>
        public static IMockDefinitionSyntax Implementing(params Type[] types)
        {
            return new MockBuilder().Implementing(types);
        }

        /// <summary>
        /// Specifies how the mock object should behave when first created.
        /// </summary>
        /// <param name="style">A MockStyle value.</param>
        /// <returns>The mock object definition.</returns>
        public static IMockDefinitionSyntax OfStyle(MockStyle style)
        {
            return new MockBuilder().OfStyle(style);
        }

        /// <summary>
        /// Specifies the arguments for the constructor of the class to be mocked.
        /// Only applicable when mocking a class with a non-default constructor.
        /// </summary>
        /// <param name="args">The arguments for the class constructor.</param>
        /// <returns>The mock object definition.</returns>
        public static IMockDefinitionSyntax WithArgs(params object[] args)
        {
            return new MockBuilder().WithArgs(args);
        }

        /// <summary>
        /// Specifies a name for the mock. This will be used in error messages,
        /// and as the return value of ToString() if not mocking a class.
        /// </summary>
        /// <param name="name">The name for the mock.</param>
        /// <returns>The mock object definition.</returns>
        public static IMockDefinitionSyntax Named(string name)
        {
            return new MockBuilder().Named(name);
        }
    }
}