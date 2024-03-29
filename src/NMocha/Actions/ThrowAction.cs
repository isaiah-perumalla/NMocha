//-----------------------------------------------------------------------
// <copyright file="ThrowAction.cs" company="NMock2">
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
using System;
using System.IO;
using NMocha;
using NMocha.Monitoring;
using NMock2.Monitoring;

namespace NMock2.Actions
{
    /// <summary>
    /// Action that sets the exception of an invocation.
    /// </summary>
    public class ThrowAction : IAction
    {
        /// <summary>
        /// Stores the exception to be thrown.
        /// </summary>
        private readonly Exception exception;

        /// <summary>
        /// Initializes a new instance of the <see cref="ThrowAction"/> class.
        /// </summary>
        /// <param name="exception">The exception.</param>
        public ThrowAction(Exception exception)
        {
            this.exception = exception;
        }

        #region IAction Members

        /// <summary>
        /// Invokes this object. Sets the exception the invocation will throw.
        /// </summary>
        /// <param name="invocation">The invocation.</param>
        public void Invoke(Invocation invocation)
        {
            invocation.Exception = exception;
        }

        /// <summary>
        /// Describes this object.
        /// </summary>
        /// <param name="description"></param>
        public void DescribeOn(IDescription description)
        {
            description.AppendText("throw ");
            description.AppendValue(exception);
        }

        #endregion
    }
}