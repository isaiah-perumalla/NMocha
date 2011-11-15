//-----------------------------------------------------------------------
// <copyright file="ReturnAction.cs" company="NMock2">
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
using System.IO;
using NMocha;
using NMocha.Monitoring;
using NMock2.Monitoring;

namespace NMock2.Actions {
    /// <summary>
    /// Action that sets the result value on an invocation.
    /// </summary>
    public class ReturnAction : IAction {
        /// <summary>
        /// Stores the result to set on the invocation as the return value.
        /// </summary>
        private readonly object result;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReturnAction"/> class.
        /// </summary>
        /// <param name="result">The result to set on the invocation as the return value.</param>
        public ReturnAction(object result) {
            this.result = result;
        }

        #region IAction Members

        /// <summary>
        /// Invokes this object. Sets the result value of the invocation.
        /// </summary>
        /// <param name="invocation">The invocation.</param>
        public void Invoke(Invocation invocation) {
            invocation.Result = result;
        }

        /// <summary>
        /// Describes this object.
        /// </summary>
        /// <param name="description"></param>
        public void DescribeOn(IDescription description) {
            description.AppendText("return ");
            description.AppendValue(result);
        }

        #endregion
    }
}