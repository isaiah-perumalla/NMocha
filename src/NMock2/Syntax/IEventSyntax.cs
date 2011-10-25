//-----------------------------------------------------------------------
// <copyright file="IEventSyntax.cs" company="NMock2">
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
    using NMock2.Syntax;

    /// <summary>
    /// Syntax for defining the object that fires an event. 
    /// </summary>
    public interface IEventSyntax
    {
        /// <summary>
        /// Defines the object that fires the event.
        /// </summary>
        /// <param name="o">The object firing the event.</param>
        /// <returns>Event argument syntax defining the arguments passed to the event.</returns>
        IEventArgumentSyntax On(object o);
    }

    /// <summary>
    /// Syntax for defining the event that is fired.
    /// </summary>
    public interface INewEventSyntax
    {
        /// <summary>
        /// Defines the event to fire.
        /// </summary>
        /// <param name="eventName">Name of the event.</param>
        /// <returns>Event argument syntax defining the arguments passed to the event.</returns>
        IEventArgumentSyntax Event(string eventName);
    }
}
