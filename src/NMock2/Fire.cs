//-----------------------------------------------------------------------
// <copyright file="Fire.cs" company="NMock2">
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
    using NMock2.Internal;
    using NMock2.Syntax;

    /// <summary>
    /// Fires a mocked event.
    /// </summary>
    public sealed class Fire
    {
        /// <summary>
        /// Fires the specified event.
        /// </summary>
        /// <param name="eventName">Name of the event.</param>
        /// <returns>Returns the event mock corresponding to the given <paramref name="eventName"/>.</returns>
        [Obsolete("Use new syntax Fire.On(mock).Event(EventName).With(arguments) instead.")]
        public static IEventSyntax Event(string eventName)
        {
            return new EventInvocationBuilder(eventName);
        }

        /// <summary>
        /// Defines the mock the event is fired on.
        /// </summary>
        /// <param name="mock">The mock the event is fired on.</param>
        /// <returns>Returns the event fire syntax.</returns>
        public static INewEventSyntax On(object mock)
        {
            return new NewEventInvocationBuilder(mock);
        }
    }
}
