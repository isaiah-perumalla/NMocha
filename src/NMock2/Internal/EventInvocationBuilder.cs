//-----------------------------------------------------------------------
// <copyright file="EventInvocationBuilder.cs" company="NMock2">
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
namespace NMock2.Internal
{
    using System;
    using NMock2.Syntax;

    /// <summary>
    /// Builder for event invocations.
    /// </summary>
    public class EventInvocationBuilder : IEventSyntax, IEventArgumentSyntax
    {
        /// <summary>
        /// Stores the event name to be mocked.
        /// </summary>
        private readonly string eventName;

        /// <summary>
        /// Stores the mock when called in the On mehtod.
        /// </summary>
        private IMockObject mock;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventInvocationBuilder"/> class.
        /// </summary>
        /// <param name="eventName">Name of the event.</param>
        public EventInvocationBuilder(string eventName)
        {
            this.eventName = eventName;
        }

        /// <summary>
        /// Specifies the mock on which the event is fired.
        /// </summary>
        /// <param name="o">The mock on which the event is fired.</param>
        /// <returns>
        /// Event argument sytax defining the arguments passed to the event.
        /// </returns>
        public IEventArgumentSyntax On(object o)
        {
            if (!(o is IMockObject))
            {
                throw new ArgumentException("Must be a mock object.");
            }

            this.mock = o as IMockObject;
            return this;
        }

        /// <summary>
        /// Specifies the event arguments that are passed to the event and fires the event.
        /// </summary>
        /// <param name="args">The args to be passed to raise the event.</param>
        public void With(params object[] args)
        {
            if (this.mock == null)
            {
                throw new InvalidOperationException("Call 'On' method first to define the mock.");
            }

            this.mock.RaiseEvent(this.eventName, args);
        }
    }

    /// <summary>
    /// Builder for event invocations.
    /// </summary>
    public class NewEventInvocationBuilder : INewEventSyntax, IEventArgumentSyntax
    {
        /// <summary>
        /// Stores the event name to be mocked.
        /// </summary>
        private string eventName;

        /// <summary>
        /// Stores the mock when called in the On method.
        /// </summary>
        private IMockObject mock;

        /// <summary>
        /// Initializes a new instance of the <see cref="NewEventInvocationBuilder"/> class.
        /// </summary>
        /// <param name="mock">The mock.</param>
        public NewEventInvocationBuilder(object mock)
        {
            if (!(mock is IMockObject))
            {
                throw new ArgumentException("Must be a mock object.");
            }

            this.mock = mock as IMockObject;
        }

        /// <summary>
        /// Defines the event to fire.
        /// </summary>
        /// <param name="eventName">Name of the event.</param>
        /// <returns>
        /// Event argument syntax defining the arguments passed to the event.
        /// </returns>
        public IEventArgumentSyntax Event(string eventName)
        {
            this.eventName = eventName;
            return this;
        }

        /// <summary>
        /// Specifies the event arguments that are passed to the event and fires the event.
        /// </summary>
        /// <param name="args">The args to be passed to raise the event.</param>
        public void With(params object[] args)
        {
            this.mock.RaiseEvent(this.eventName, args);
        }
    }
}
