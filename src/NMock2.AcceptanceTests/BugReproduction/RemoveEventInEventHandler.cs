//-----------------------------------------------------------------------
// <copyright file="RemoveEventInEventHandler.cs" company="NMock2">
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

namespace NMock2.AcceptanceTests
{
    using System;
    using NUnit.Framework;

    /// <summary>
    /// Reproduces the defect that occurs when an event is deregistered in an event handler of this event.
    /// Exception: <c>System.InvalidOperationException: Collection was modified; enumeration operation may not execute.</c>
    /// Solution: copy event handler collection before invocation.
    /// </summary>
    [TestFixture]
    public class RemoveEventInEventHandler : AcceptanceTestBase
    {
        /// <summary>
        /// Dummy interface with an event.
        /// </summary>
        public interface IAnnouncer
        {
            /// <summary>
            /// Dummy event.
            /// </summary>
            event EventHandler Event;
        }

        /// <summary>
        /// An event can be deregistered in its event handler.
        /// </summary>
        [Test]
        public void DeregisterEventInEventHandler()
        {
            IAnnouncer announcer = Mocks.NewMock<IAnnouncer>();

            Expect.Once.On(announcer).EventAdd("Event");
            Expect.Once.On(announcer).EventRemove("Event");

            announcer.Event += this.HandleAnnouncerEvent;

            Fire.Event("Event").On(announcer).With(announcer, EventArgs.Empty);
        }

        /// <summary>
        /// Handles the announcer event and deregisters the event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void HandleAnnouncerEvent(object sender, EventArgs e)
        {
            ((IAnnouncer)sender).Event -= this.HandleAnnouncerEvent;
        }
    }
}