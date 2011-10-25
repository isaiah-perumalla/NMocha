//-----------------------------------------------------------------------
// <copyright file="FireAction.cs" company="NMock2">
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
    using NMock2.Internal;
    using NMock2.Monitoring;

    /// <summary>
    /// Action that fires an event.
    /// </summary>
    public class FireAction : IAction
    {
        /// <summary>
        /// Stores the name of the event to fire.
        /// </summary>
        private readonly string eventName;

        /// <summary>
        /// Stores the event arguments.
        /// </summary>
        private readonly object[] eventArgs;

        /// <summary>
        /// Initializes a new instance of the <see cref="FireAction"/> class.
        /// </summary>
        /// <param name="eventName">Name of the event.</param>
        /// <param name="eventArgs">The event args.</param>
        public FireAction(string eventName, params object[] eventArgs)
        {
            this.eventName = eventName;
            this.eventArgs = eventArgs;
        }

        /// <summary>
        /// Invokes this object. The event is fired on the receiver of the invocation.
        /// </summary>
        /// <param name="invocation">The invocation.</param>
        public void Invoke(Invocation invocation)
        {
            ((IMockObject)invocation.Receiver).RaiseEvent(this.eventName, this.eventArgs);
        }

        /// <summary>
        /// Describes this object.
        /// </summary>
        /// <param name="writer">The text writer the description is added to.</param>
        public void DescribeTo(TextWriter writer)
        {
            writer.Write("fire ");
            writer.Write(this.eventName);
        }
    }
}
