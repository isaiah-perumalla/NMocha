//-----------------------------------------------------------------------
// <copyright file="IMockObjectFactory.cs" company="NMock2">
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
namespace NMock2.Monitoring
{
    using System;
    using NMock2.Internal;
    
    /// <summary>
    /// Implementations of this interface are responsible for generating runtime
    /// proxies of classes and interfaces for use as mock objects.
    /// </summary>
    /// <remarks>
    /// Returned instances are expected to implement IMockObject and take care of
    /// intercepting calls to their public members. Intercepted calls should be
    /// forwarded on to the supplied Mockery for processing against expectations.
    /// </remarks>
    public interface IMockObjectFactory
    {
        /// <summary>
        /// Creates a mock of the specified type(s).
        /// </summary>
        /// <param name="mockery">The mockery used to create this mock instance.</param>
        /// <param name="typesToMock">The type(s) to include in the mock.</param>
        /// <param name="name">The name to use for the mock instance.</param>
        /// <param name="mockStyle">The behaviour of the mock instance when first created.</param>
        /// <param name="constructorArgs">Constructor arguments for the class to be mocked. Only valid if mocking a class type.</param>
        /// <returns>A mock instance of the specified type(s).</returns>
        object CreateMock(Mockery mockery, CompositeType typesToMock, string name, MockStyle mockStyle, object[] constructorArgs);
    }
}
