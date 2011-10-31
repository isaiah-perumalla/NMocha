//-----------------------------------------------------------------------
// <copyright file="CastleMockObjectFactory.cs" company="NMock2">
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
using Castle.Core.Interceptor;
using Castle.DynamicProxy;
using NMock2.Internal;

namespace NMock2.Monitoring {
    /// <summary>
    /// Class that creates mocks for interfaces and classes (virtual members only) using the
    /// Castle proxy generator.
    /// </summary>
    public class CastleMockObjectFactory : IMockObjectFactory {
        private readonly Dictionary<CompositeType, Type> cachedProxyTypes = new Dictionary<CompositeType, Type>();

        #region IMockObjectFactory Members

        /// <summary>
        /// Creates a mock of the specified type(s).
        /// </summary>
        /// <param name="mockery">The mockery used to create this mock instance.</param>
        /// <param name="typesToMock">The type(s) to include in the mock.</param>
        /// <param name="name">The name to use for the mock instance.</param>
        /// <param name="constructorArgs">Constructor arguments for the class to be mocked. Only valid if mocking a class type.</param>
        /// <returns>A mock instance of the specified type(s).</returns>
        public object CreateMock(Mockery mockery, CompositeType typesToMock, string name, object[] constructorArgs) {
            Type proxyType = GetProxyType(typesToMock);

            return InstantiateProxy(typesToMock, proxyType, mockery, name, constructorArgs);
        }

        #endregion

        private Type GetProxyType(CompositeType compositeType) {
            if (!cachedProxyTypes.ContainsKey(compositeType))
            {
                var proxyBuilder = new DefaultProxyBuilder();
                Type[] additionalInterfaceTypes =
                    BuildAdditionalTypeArrayForProxyType(compositeType.AdditionalInterfaceTypes);
                Type proxyType;

                if (compositeType.PrimaryType.IsClass)
                {
                    if (compositeType.PrimaryType.IsSealed)
                    {
                        throw new ArgumentException("Cannot mock sealed classes.");
                    }

                    proxyType = proxyBuilder.CreateClassProxy(
                        compositeType.PrimaryType,
                        additionalInterfaceTypes,
                        ProxyGenerationOptions.Default);
                }
                else
                {
                    proxyType = proxyBuilder.CreateInterfaceProxyTypeWithoutTarget(
                        compositeType.PrimaryType,
                        additionalInterfaceTypes,
                        new ProxyGenerationOptions {BaseTypeForInterfaceProxy = typeof (InterfaceMockBase)});
                }

                cachedProxyTypes[compositeType] = proxyType;
            }

            return cachedProxyTypes[compositeType];
        }

        private object InstantiateProxy(
            CompositeType compositeType,
            Type proxyType,
            Mockery mockery,
            string name,
            object[] constructorArgs) {
            IInterceptor interceptor = new MockObjectInterceptor(compositeType, name, mockery, mockery);
            object[] activationArgs;

            if (compositeType.PrimaryType.IsClass)
            {
                activationArgs = new object[constructorArgs.Length + 1];
                constructorArgs.CopyTo(activationArgs, 1);
                activationArgs[0] = new[] {interceptor};
            }
            else
            {
                activationArgs = new[] {new[] {interceptor}, new object(), name};
            }

            return Activator.CreateInstance(proxyType, activationArgs);
        }

        private Type[] BuildAdditionalTypeArrayForProxyType(Type[] additionalTypes) {
            var allAdditionalTypes = new Type[additionalTypes.Length + 1];

            allAdditionalTypes[0] = typeof (IMockObject);

            if (additionalTypes.Length > 0)
            {
                additionalTypes.CopyTo(allAdditionalTypes, 1);
            }

            return allAdditionalTypes;
        }

        #region Nested type: InterfaceMockBase

        /// <summary>
        /// Used as a base for interface mocks in order to provide a holder
        /// for a meaningful ToString() value.
        /// </summary>
        public class InterfaceMockBase {
            private readonly string stringValue;

            /// <summary>
            /// Initializes a new instance of the <see cref="InterfaceMockBase"/> class.
            /// </summary>
            /// <param name="stringValue">The string value.</param>
            public InterfaceMockBase(string stringValue) {
                this.stringValue = stringValue;
            }

            public override string ToString() {
                return stringValue;
            }
        }

        #endregion
    }
}