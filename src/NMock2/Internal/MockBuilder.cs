//-----------------------------------------------------------------------
// <copyright file="MockBuilder.cs" company="NMock2">
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
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    using Matchers;
    using Monitoring;
    using Syntax;

    /// <summary>
    /// Allows a mock object to be incrementally defined, and then finally created.
    /// </summary>
    public class MockBuilder : IMockDefinitionSyntax
    {
        /// <summary>
        /// A single empty array instance that is used as a default value
        /// for constructor arguments.
        /// </summary>
        private static readonly object[] EmptyArgsArray = new object[0];

        /// <summary>
        /// The name of the mock object. Null is a valid value.
        /// </summary>
        private string name = null;

        /// <summary>
        /// The types that the mock object needs to implement.
        /// </summary>
        private List<Type> types = new List<Type>();

        /// <summary>
        /// Constructor arguments for any class type that this mock might subclass.
        /// If not subclassing, or if using a default constructor, then this should
        /// be an empty array.
        /// </summary>
        private object[] constructorArgs = EmptyArgsArray;

        /// <summary>
        /// The MockStyle for the mock. If not specified, this will ultimately be
        /// assumed to be MockStyle.Default.
        /// </summary>
        private MockStyle? mockStyle = null;

        #region IMockDefinitionSyntax Members

        /// <summary>
        /// Specifies a type that this mock should implement. This may be a class or interface,
        /// but there can only be a maximum of one class implemented by a mock.
        /// </summary>
        /// <typeparam name="T">The type to implement.</typeparam>
        /// <returns>The mock object definition.</returns>
        public IMockDefinitionSyntax Implementing<T>()
        {
            this.types.Add(typeof(T));

            return this;
        }

        /// <summary>
        /// Specifies the types that this mock should implement. These may be a class or interface,
        /// but there can only be a maximum of one class implemented by a mock.
        /// </summary>
        /// <param name="types">The types to implement.</param>
        /// <returns>The mock object definition.</returns>
        public IMockDefinitionSyntax Implementing(params Type[] types)
        {
            this.types.AddRange(types);

            return this;
        }

        /// <summary>
        /// Specifies how the mock object should behave when first created.
        /// It is invalid to set the MockStyle of a mock more than once.
        /// </summary>
        /// <param name="style">A MockStyle value.</param>
        /// <returns>The mock object definition.</returns>
        public IMockDefinitionSyntax OfStyle(MockStyle style)
        {
            if (this.mockStyle.HasValue)
            {
                throw new InvalidOperationException("MockStyle has already been set for this mock definition.");
            }

            this.mockStyle = style;

            return this;
        }

        /// <summary>
        /// Specifies the arguments for the constructor of the class to be mocked.
        /// Only applicable when mocking a class with a non-default constructor.
        /// It is invalid to specify the constructor arguments of a mock more than once.
        /// </summary>
        /// <param name="args">The arguments for the class constructor.</param>
        /// <returns>The mock object definition.</returns>
        public IMockDefinitionSyntax WithArgs(params object[] args)
        {
            if (this.constructorArgs != EmptyArgsArray)
            {
                throw new InvalidOperationException("Constructor arguments have already been specified for this mock definition.");
            }

            this.constructorArgs = args;

            return this;
        }

        /// <summary>
        /// Specifies a name for the mock. This will be used in error messages,
        /// and as the return value of ToString() if not mocking a class.
        /// It is invalid to specify the name of a mock more than once.
        /// </summary>
        /// <param name="name">The name for the mock.</param>
        /// <returns>The mock object definition.</returns>
        public IMockDefinitionSyntax Named(string name)
        {
            if (this.name != null)
            {
                throw new InvalidOperationException("A name has already been specified for this mock definition.");
            }

            this.name = name;

            return this;
        }

        #endregion

        #region IMockDefinition Members

        /// <summary>
        /// This method supports NMock2 infrastructure and is not intended to be called directly from your code.
        /// </summary>
        /// <param name="primaryType">The primary type that is being mocked.</param>
        /// <param name="mockery">The current <see cref="Mockery"/> instance.</param>
        /// <param name="mockObjectFactory">An <see cref="IMockObjectFactory"/> to use when creating the mock.</param>
        /// <returns>A new mock instance.</returns>
        public object Create(Type primaryType, Mockery mockery, IMockObjectFactory mockObjectFactory)
        {
            if (this.name == null)
            {
                this.name = this.DefaultNameFor(primaryType);
            }

            CompositeType compositeType = new CompositeType(primaryType, this.types.ToArray());

            if (compositeType.PrimaryType.IsInterface)
            {
                if (this.constructorArgs.Length > 0)
                {
                    throw new InvalidOperationException("Cannot specify constructor arguments when mocking an interface");
                }

                CheckInterfacesDoNotContainToStringMethodDeclaration(compositeType);
            }

            return mockObjectFactory.CreateMock(
                mockery,
                compositeType,
                this.name,
                this.mockStyle ?? MockStyle.Default,
                this.constructorArgs);
        }

        #endregion

        /// <summary>
        /// Returns the default name for a type that is used to name mocks.
        /// </summary>
        /// <param name="type">The type to get the default name for.</param>
        /// <returns>Default name for the specified type.</returns>
        protected virtual string DefaultNameFor(Type type)
        {
            string name = type.Name;
            int firstLower = FirstLowerCaseChar(name);

            return firstLower == name.Length ?
                    name.ToLower() :
                    name.Substring(firstLower - 1, 1).ToLower() + name.Substring(firstLower);
        }

        /// <summary>
        /// Finds the first lower case char in the specified string.
        /// </summary>
        /// <param name="s">The string to inspect.</param>
        /// <returns>the first lower case char in the specified string.</returns>
        private static int FirstLowerCaseChar(string s)
        {
            int i = 0;
            while (i < s.Length && !Char.IsLower(s[i]))
            {
                i++;
            }

            return i;
        }

        /// <summary>
        /// Checks that interfaces do not contain ToString method declarations.
        /// </summary>
        /// <param name="mockedTypes">The types that are to be mocked.</param>
        private static void CheckInterfacesDoNotContainToStringMethodDeclaration(CompositeType mockedTypes)
        {
            foreach (MethodInfo method in mockedTypes.GetMatchingMethods(new MethodNameMatcher("ToString"), false))
            {
                if (method.ReflectedType.IsInterface && method.GetParameters().Length == 0)
                {
                    throw new ArgumentException("Interfaces must not contain a declaration for ToString().");
                }
            }
        }
    }
}