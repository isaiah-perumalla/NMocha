//-----------------------------------------------------------------------
// <copyright file="LazyReturnAction.cs" company="NMock2">
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
    /// Action that sets the result value on an invocation. The value is aquired by calling the delegate specified in the constructor.
    /// </summary>
    public class LazyReturnAction : IAction {
        #region Delegates

        /// <summary>
        /// Delegate that is used to get the return value.
        /// </summary>
        /// <returns>
        /// Returns an object...
        /// </returns>
        public delegate object Evaluate();

        #endregion

        /// <summary>
        /// Stores the evaluate delegate for this action.
        /// </summary>
        private readonly Evaluate evaluate;

        /// <summary>
        /// Initializes a new instance of the <see cref="LazyReturnAction"/> class.
        /// </summary>
        /// <param name="evaluate">The delegate used to aquire the return value.</param>
        public LazyReturnAction(Evaluate evaluate) {
            this.evaluate = evaluate;
        }

        #region IAction Members

        /// <summary>
        /// Invokes this object.
        /// </summary>
        /// <param name="invocation">The invocation.</param>
        public void Invoke(Invocation invocation) {
            invocation.Result = evaluate();
        }

        /// <summary>
        /// Describes this object.
        /// </summary>
        /// <param name="description"></param>
        public void DescribeOn(IDescription description) {
            description.AppendText("lazy return value");
        }

        #endregion
    }
}