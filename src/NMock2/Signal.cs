//-----------------------------------------------------------------------
// <copyright file="Signal.cs" company="NMock2">
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
    using System.Threading;
    using NMock2.Actions;

    /// <summary>
    /// Defines that an <see cref="EventWaitHandle"/> should be signaled.
    /// </summary>
    public class Signal
    {
        /// <summary>
        /// Signals an <see cref="EventWaitHandle"/> to synchronizes threads.
        /// </summary>
        /// <param name="signal">The signal to set.</param>
        /// <returns>Action that signals an <see cref="EventWaitHandle"/>.</returns>
        public static IAction EventWaitHandle(EventWaitHandle signal)
        {
            return new SignalAction(signal);
        }
    }
}
