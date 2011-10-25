//-----------------------------------------------------------------------
// <copyright file="Return.cs" company="NMock2">
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
    using NMock2.Actions;

    /// <summary>
    /// Defines the return value of a mocked method call.
    /// </summary>
    public class Return
    {
        /// <summary>
        /// Returns a value as method return value.
        /// </summary>
        /// <param name="result">The result value.</param>
        /// <returns>Action defining the return value of a method.</returns>
        public static IAction Value(object result)
        {
            return new ReturnAction(result);
        }

        /// <summary>
        /// Returns a clone as method return value.
        /// </summary>
        /// <param name="prototype">The prototype to clone.</param>
        /// <returns>Action defining the return value of a method.</returns>
        public static IAction CloneOf(ICloneable prototype)
        {
            return new ReturnCloneAction(prototype);
        }

        /// <summary>
        /// Defines the value returned by an out parameter.
        /// </summary>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <param name="value">The value to return.</param>
        /// <returns>Action defining the value of an out parameter.</returns>
        public static IAction OutValue(string parameterName, object value)
        {
            return new SetNamedParameterAction(parameterName, value);
        }

        /// <summary>
        /// Defines the value returned by an out parameter.
        /// </summary>
        /// <param name="parameterIndex">Index of the parameter.</param>
        /// <param name="value">The value to return.</param>
        /// <returns>Action defining the value of an out parameter.</returns>
        public static IAction OutValue(int parameterIndex, object value)
        {
            return new SetIndexedParameterAction(parameterIndex, value);
        }
    }
}
