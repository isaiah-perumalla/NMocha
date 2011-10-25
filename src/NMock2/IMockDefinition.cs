//-----------------------------------------------------------------------
// <copyright file="IMockDefinition.cs" company="NMock2">
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
    using System.ComponentModel;

    using Monitoring;

    /// <summary>
    /// The definition of a mock object.
    /// </summary>
    /// <remarks>
    /// Implementations of this interface encapsulate the details of
    /// how a mock object is defined, and provide the ability to be able to
    /// instantiate an instance of it.
    /// </remarks>
    public interface IMockDefinition
    {
        /// <summary>
        /// This method supports NMock2 infrastructure and is not intended to be called directly from your code.
        /// </summary>
        /// <param name="primaryType">The primary type that is being mocked.</param>
        /// <param name="mockery">The current <see cref="Mockery"/> instance.</param>
        /// <param name="mockFactory">An <see cref="IMockObjectFactory"/> to use when creating the mock.</param>
        /// <returns>A new mock instance.</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        object Create(Type primaryType, Mockery mockery, IMockObjectFactory mockFactory);
    }
}