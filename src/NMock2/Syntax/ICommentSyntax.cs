//-----------------------------------------------------------------------
// <copyright file="ICommentSyntax.cs" company="NMock2">
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
    /// Syntax for adding an explanation for the expectation.
    /// </summary>
    public interface ICommentSyntax
    {
        /// <summary>
        /// Adds a comment for the expectation that is added to the error message if the expectation is not met.
        /// </summary>
        /// <param name="comment">The comment that is shown in the error message if this expectation is not met.
        /// You can describe here why this expectation has to be met.</param>
        void Comment(string comment);
    }
}
