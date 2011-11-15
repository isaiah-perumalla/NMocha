//-----------------------------------------------------------------------
// <copyright file="SetIndexedParameterAction.cs" company="NMock2">
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
    /// Action that sets a parameter (method argument) of the invocation to the specified value.
    /// </summary>
    public class SetIndexedParameterAction : IAction {
        /// <summary>
        /// Stores the index of the paremter to set.
        /// </summary>
        private readonly int index;

        /// <summary>
        /// Stores the value of the parameter to set.
        /// </summary>
        private readonly object value;

        /// <summary>
        /// Initializes a new instance of the <see cref="SetIndexedParameterAction"/> class.
        /// </summary>
        /// <param name="index">The index of the parameter to set.</param>
        /// <param name="value">The value.</param>
        public SetIndexedParameterAction(int index, object value) {
            this.index = index;
            this.value = value;
        }

        #region IAction Members

        /// <summary>
        /// Invokes this object. Sets the parameter at the specified index of the invocation to the specified value.
        /// </summary>
        /// <param name="invocation">The invocation.</param>
        public void Invoke(Invocation invocation) {
            invocation.Parameters[index] = value;
        }

        /// <summary>
        /// Describes this object.
        /// </summary>
        /// <param name="description"></param>
        public void DescribeOn(IDescription description) {
            description.AppendText("set arg ")
                       .AppendText(index.ToString())
                       .AppendText("=")
                       .AppendValue(value);
        }

        #endregion
    }
}