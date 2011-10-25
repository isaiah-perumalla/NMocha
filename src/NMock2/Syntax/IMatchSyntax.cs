//-----------------------------------------------------------------------
// <copyright file="IMatchSyntax.cs" company="NMock2">
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
    /// <summary>
    /// Syntax defining matching criterias.
    /// </summary>
    public interface IMatchSyntax : IActionSyntax
    {
        /// <summary>
        /// Defines a matching criteria.
        /// </summary>
        /// <param name="matcher">The matcher.</param>
        /// <returns>Action syntax defining the action to take.</returns>
        IActionSyntax Matching(Matcher matcher);
    }
}
