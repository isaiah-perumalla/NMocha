//-----------------------------------------------------------------------
// <copyright file="MockStyle.cs" company="NMock2">
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
    /// <summary>
    /// Specifies how a mock object should behave when it is first created.
    /// </summary>
    public enum MockStyle
    {
        /// <summary>
        /// Calls to members that do not have expectations set will
        /// result in ExpectationExceptions.
        /// </summary>
        Default = 0,

        /// <summary>
        /// Calls to members that do not have expectations set will
        /// pass through to the underlying implementation on the class
        /// being mocked.
        /// </summary>
        Transparent = 1,

        /// <summary>
        /// Calls to members that do not have expectations set will
        /// be ignored. Default values are used for return values 
        /// (default value of the return type, stub or empty enumerable)
        /// and the same value is returned on every call to the same member.
        /// </summary>
        Stub = 2,
    }
}
