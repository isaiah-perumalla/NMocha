//-----------------------------------------------------------------------
// <copyright file="IActionSyntax.cs" company="NMock2">
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
using NMocha;
using NMocha.Internal;

namespace NMock2.Syntax {
    /// <summary>
    /// Syntax for defining actions.
    /// </summary>
    public interface IActionSyntax : IStateSyntax, ISequenceSyntax {
        /// <summary>
        /// Defines what will happen.
        /// </summary>
        /// <param name="actions">The actions to take.</param>
        /// <returns>Returns the comment syntax defined after will.</returns>
        IStateSyntax Will(params IAction[] actions);
    }
}