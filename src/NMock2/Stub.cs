//-----------------------------------------------------------------------
// <copyright file="Stub.cs" company="NMock2">
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
    using NMock2.Internal;
    using NMock2.Syntax;

    /// <summary>
    /// Defines stubs for interfaces. Stubs are used when it does not matter how many times (even 0) something is called.
    /// A stub is the same as an "at least once" expecation.
    /// Normally you use stubs on mocks that just provide information to your object under test.
    /// </summary>
    public class Stub
    {
        /// <summary>
        /// Defines the mock that is stubbed.
        /// </summary>
        /// <param name="mock">The mock to stub.</param>
        /// <returns>Method syntax defining the method, property or event to stub.</returns>
        public static IMethodSyntax On(object mock)
        {
            ExpectationBuilder builder = new ExpectationBuilder("Stub", Is.Anything, Is.Anything);
            return builder.On(mock);
        }
    }
}
