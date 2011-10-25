//-----------------------------------------------------------------------
// <copyright file="Throw.cs" company="NMock2">
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
    using NMock2.Actions;

    /// <summary>
    /// Defines action for throwing actions.
    /// </summary>
    public class Throw
    {
        /// <summary>
        /// Throws an exeception when the action is invoked.
        /// </summary>
        /// <param name="exception">The exception to throw when invoked.</param>
        /// <returns>Returns a new instance of the <see cref="ThrowAction"/> class.</returns>
        public static IAction Exception(Exception exception)
        {
            return new ThrowAction(exception);
        }
    }
}
