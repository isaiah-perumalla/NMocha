//-----------------------------------------------------------------------
// <copyright file="CollectAction.cs" company="NMock2">
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
namespace NMock2.Actions
{
    using System.IO;
    using NMock2.Monitoring;

    /// <summary>
    /// Action that returns the n-th element of the arguments to an invocation.
    /// </summary>
    public class CollectAction : IAction
    {
        /// <summary>
        /// Stores the index of the argument.
        /// </summary>
        private readonly int argumentIndex;

        /// <summary>
        /// Stores the parameter when this action gets invoked.
        /// </summary>
        private object collectedArgumentValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="CollectAction"/> class.
        /// </summary>
        /// <param name="argumentIndex">Index of the argument to collect.</param>
        public CollectAction(int argumentIndex)
        {
            this.argumentIndex = argumentIndex;
        }

        /// <summary>
        /// Gets the collected parameter.
        /// </summary>
        /// <value>The collected parameter (n-th parameter of parameter list of the method's call.</value>
        public object Parameter
        {
            get { return this.collectedArgumentValue; }
        }

        /// <summary>
        /// Invokes this object.
        /// </summary>
        /// <param name="invocation">The invocation.</param>
        public void Invoke(Invocation invocation)
        {
            this.collectedArgumentValue = invocation.Parameters[this.argumentIndex];
        }

        /// <summary>
        /// Describes this object.
        /// </summary>
        /// <param name="writer">The text writer the description is added to.</param>
        public void DescribeTo(TextWriter writer)
        {
            writer.Write("collect argument at index ");
            writer.Write(this.argumentIndex);
        }
    }
}
