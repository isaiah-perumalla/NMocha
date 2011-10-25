//-----------------------------------------------------------------------
// <copyright file="IMethodSyntax.cs" company="NMock2">
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
    using System;
    using System.Reflection;

    /// <summary>
    /// Syntax defining a method, property or event (de)registration.
    /// </summary>
    public interface IMethodSyntax
    {
        /// <summary>
        /// Gets an indexer (get operation).
        /// </summary>
        /// <value>Get indexer syntax defining the value returned by the indexer.</value>
        IGetIndexerSyntax Get { get; }

        /// <summary>
        /// Gets an indexer (set operation).
        /// </summary>
        /// <value>Set indexer syntax defining the value the indexer is set to.</value>
        ISetIndexerSyntax Set { get; }

        /// <summary>
        /// Defines a method.
        /// </summary>
        /// <param name="name">The name of the method.</param>
        /// <param name="typeParams">The generic type params to match.</param>
        /// <returns>
        /// Argument syntax defining the arguments of the method.
        /// </returns>
        IArgumentSyntax Method(string name, params Type[] typeParams);

        /// <summary>
        /// Defines a method.
        /// </summary>
        /// <param name="nameMatcher">Matcher defining the method.</param>
        /// <param name="typeParams">The generic type params to match.</param>
        /// <returns>Argument syntax defining the arguments of the method.</returns>
        IArgumentSyntax Method(Matcher nameMatcher, params Type[] typeParams);

        /// <summary>
        /// Defines a method.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <param name="typeParams">The generic type params to match.</param>
        /// <returns>Argument syntax defining the arguments of the method.</returns>
        IArgumentSyntax Method(MethodInfo method, params Type[] typeParams);

        /// <summary>
        /// Defines a method.
        /// </summary>
        /// <param name="methodMatcher">Matcher for matching the method on an invocation.</param>
        /// <param name="typeParamsMatcher">Matchers for matching type parameters.</param>
        /// <returns>Argument syntax defining the arguments of the method.</returns>
        IArgumentSyntax Method(Matcher methodMatcher, Matcher typeParamsMatcher);

        /// <summary>
        /// Defines a property setter.
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <returns>Value syntax defining the value of the property.</returns>
        IValueSyntax SetProperty(string name);

        /// <summary>
        /// Defines a property getter.
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <returns>Match Syntax defining the property behavior.</returns>
        IMatchSyntax GetProperty(string name);

        /// <summary>
        /// Defines an event registration.
        /// </summary>
        /// <param name="eventName">Name of the event.</param>
        /// <returns>Match syntax defining the behavior of the event adder.</returns>
        IMatchSyntax EventAdd(string eventName);

        /// <summary>
        /// Defines an event registration.
        /// </summary>
        /// <param name="eventName">Name of the event.</param>
        /// <param name="listenerMatcher">The listener matcher.</param>
        /// <returns>Match syntax defining the behavior of the event adder.</returns>
        IMatchSyntax EventAdd(string eventName, Matcher listenerMatcher);

        /// <summary>
        /// Defines an event registration.
        /// </summary>
        /// <param name="eventName">Name of the event.</param>
        /// <param name="equalListener">Delegate defining compatible listeners.</param>
        /// <returns>Match syntax defining the behavior of the event adder.</returns>
        IMatchSyntax EventAdd(string eventName, Delegate equalListener);

        /// <summary>
        /// Defines an event deregistration.
        /// </summary>
        /// <param name="eventName">Name of the event.</param>
        /// <returns>Match syntax defining the behavior of the event remover.</returns>
        IMatchSyntax EventRemove(string eventName);

        /// <summary>
        /// Defines an event deregistration.
        /// </summary>
        /// <param name="eventName">Name of the event.</param>
        /// <param name="listenerMatcher">The listener matcher.</param>
        /// <returns>Match syntax defining the behavior of the event remover.</returns>
        IMatchSyntax EventRemove(string eventName, Matcher listenerMatcher);

        /// <summary>
        /// Defines an event deregistration.
        /// </summary>
        /// <param name="eventName">Name of the event.</param>
        /// <param name="equalListener">Delegate defining compatible listeners.</param>
        /// <returns> Match syntax defining the behavior of the event remover.</returns>
        IMatchSyntax EventRemove(string eventName, Delegate equalListener);
    }
}
