//-----------------------------------------------------------------------
// <copyright file="DelegateAction.cs" company="NMock2">
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
    /// Action that executes the delegate passed to the constructor.
    /// </summary>
    public class DelegateAction : IAction
    {
        /// <summary>
        /// Stores the handler of the delegate action.
        /// </summary>
        private readonly Handler handler;

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateAction"/> class.
        /// </summary>
        /// <param name="actionHandler">The action handler.</param>
        public DelegateAction(Handler actionHandler)
        {
            this.handler = actionHandler;
        }

        /// <summary>
        /// Delegate that is executed on invocation of the action.
        /// </summary>
        public delegate void Handler();

        /// <summary>
        /// Invokes this object.
        /// </summary>
        /// <param name="invocation">The invocation.</param>
        public void Invoke(Invocation invocation)
        {
            this.handler();
        }

        /// <summary>
        /// Describes this object.
        /// </summary>
        /// <param name="writer">The text writer the description is added to.</param>
        public void DescribeTo(TextWriter writer)
        {
            writer.Write("execute delegate");
        }
    }
}