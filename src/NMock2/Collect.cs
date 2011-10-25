// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Collect.cs" company="NMock2">
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
// --------------------------------------------------------------------------------------------------------------------

namespace NMock2
{
    using Actions;

    /// <summary>
    /// Gather information about invocations.
    /// </summary>
    public class Collect
    {
        /// <summary>
        /// Calls the specified <paramref name="collectDelegate"/> with the method argument at index <paramref name="argumentIndex"/>.
        /// Can only be used as action of an expectation on a method call.
        /// </summary>
        /// <typeparam name="TArgument">The type of the argument.</typeparam>
        /// <param name="argumentIndex">Index of the argument.</param>
        /// <param name="collectDelegate">The collect delegate.</param>
        /// <returns>Action that collects a method argument.</returns>
        public static IAction MethodArgument<TArgument>(int argumentIndex, CollectAction<TArgument>.Collect collectDelegate)
        {
            return new CollectAction<TArgument>(argumentIndex, collectDelegate);
        }

        /// <summary>
        /// Calls the specified <paramref name="collectDelegate"/> with the value that is set to the property.
        /// Can only be used as action of an expectation on a property setter. 
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="collectDelegate">The collect delegate.</param>
        /// <returns>Action that collects a property value.</returns>
        public static IAction PropertyValue<TValue>(CollectAction<TValue>.Collect collectDelegate)
        {
            return new CollectAction<TValue>(0, collectDelegate);
        }
    }
}