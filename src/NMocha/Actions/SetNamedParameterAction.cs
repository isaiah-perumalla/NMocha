//-----------------------------------------------------------------------
// <copyright file="SetNamedParameterAction.cs" company="NMock2">
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
using System.Reflection;
using NMocha;
using NMocha.Monitoring;
using NMock2.Monitoring;

namespace NMock2.Actions {
    /// <summary>
    /// Action that sets the parameter of the invocation with the specified name to the specified value.
    /// </summary>
    public class SetNamedParameterAction : IAction {
        /// <summary>
        /// Stores the name of the parameter when the class gets initialized.
        /// </summary>
        private readonly string name;

        /// <summary>
        /// Stores the value of the parameter when the class gets initialized.
        /// </summary>
        private readonly object value;

        /// <summary>
        /// Initializes a new instance of the <see cref="SetNamedParameterAction"/> class.
        /// </summary>
        /// <param name="name">The name of the parameter to set.</param>
        /// <param name="value">The value.</param>
        public SetNamedParameterAction(string name, object value) {
            this.name = name;
            this.value = value;
        }

        #region IAction Members

        /// <summary>
        /// Invokes this object. Sets the value of the parameter with the specified name of the invocation.
        /// </summary>
        /// <param name="invocation">The invocation.</param>
        public void Invoke(Invocation invocation) {
            ParameterInfo[] paramsInfo = invocation.Method.GetParameters();

            for (int i = 0; i < paramsInfo.Length; i++)
            {
                if (paramsInfo[i].Name == name)
                {
                    invocation.Parameters[i] = value;
                    return;
                }
            }

            throw new ArgumentException("no such parameter", name);
        }

        /// <summary>
        /// Describes this object.
        /// </summary>
        /// <param name="description"></param>
        public void DescribeOn(IDescription description) {
            description.AppendText("set ");
            description.AppendText(name);
            description.AppendText("=");
            description.AppendValue(value);
        }

        #endregion
    }
}