//-----------------------------------------------------------------------
// <copyright file="BinaryOperator.cs" company="NMock2">
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
namespace NMock2.Matchers
{
    /// <summary>
    /// BinaryOperator is an abstract base class for matchers that combine two matchers into a single matcher. 
    /// </summary>
    public abstract class BinaryOperator : Matcher
    {
        /// <summary>
        /// The right hand side of the binary operator.
        /// </summary>
        protected readonly Matcher Right;

        /// <summary>
        /// The left hand side of the binary operator.
        /// </summary>
        protected readonly Matcher Left;

        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryOperator"/> class.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        protected BinaryOperator(Matcher left, Matcher right)
        {
            this.Left = left;
            this.Right = right;
        }
    }
}
