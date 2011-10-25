//-----------------------------------------------------------------------
// <copyright file="SignalAction.cs" company="NMock2">
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
    using System.Threading;
    using NMock2.Monitoring;

    /// <summary>
    /// Action that signals an event.
    /// You can use this action to synchronize threads when an expectation is invoked.
    /// </summary>
    public class SignalAction : IAction
    {
        /// <summary>
        /// Stores the wait handle to be signalled.
        /// </summary>
        private readonly EventWaitHandle signal;

        /// <summary>
        /// Initializes a new instance of the <see cref="SignalAction"/> class.
        /// </summary>
        /// <param name="signal">The signal.</param>
        public SignalAction(EventWaitHandle signal)
        {
            this.signal = signal;
        }

        /// <summary>
        /// Gets the signal.
        /// You can use this signal to wait for this action beeing invoked.
        /// </summary>
        /// <value>The signal.</value>
        public EventWaitHandle Signal
        {
            get { return this.signal; }
        }

        /// <summary>
        /// Invokes this object by signaling the event.
        /// </summary>
        /// <param name="invocation">The invocation.</param>
        public void Invoke(Invocation invocation)
        {
            this.signal.Set();
        }

        /// <summary>
        /// Describes this object.
        /// </summary>
        /// <param name="writer">The text writer the description is added to.</param>
        public void DescribeTo(TextWriter writer)
        {
            writer.Write("signals");
        }
    }
}