//-----------------------------------------------------------------------
// <copyright file="ParameterList.cs" company="NMock2">
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
using System.Collections;
using System.Reflection;

namespace NMock2.Monitoring {
    /// <summary>
    /// Manages a list of parameters for a mocked method together with the parameter's values.
    /// </summary>
    public class ParameterList {
        /// <summary>
        /// Holds a boolean for each value if it was set or not.
        /// </summary>
        private readonly BitArray isValueSet;

        /// <summary>
        /// Holds the method to be mocked.
        /// </summary>
        private readonly MethodInfo method;

        /// <summary>
        /// An array holding the values of the parameters.
        /// </summary>
        private readonly object[] values;

        /// <summary>
        /// Initializes a new instance of the <see cref="ParameterList"/> class.
        /// </summary>
        /// <param name="method">The method to be mocked.</param>
        /// <param name="values">The values of the parameters.</param>
        public ParameterList(MethodInfo method, object[] values) {
            this.method = method;
            this.values = values;
            isValueSet = new BitArray(values.Length);

            ParameterInfo[] parameters = method.GetParameters();
            for (int i = 0; i < parameters.Length; i++)
            {
                isValueSet[i] = !parameters[i].IsOut;
            }
        }

        /// <summary>
        /// Gets the number of values.
        /// </summary>
        /// <value>The number of values.</value>
        public int Count {
            get { return values.Length; }
        }

        /// <summary>
        /// Gets the values as array.
        /// </summary>
        /// <value>Values as array.</value>
        internal object[] AsArray {
            get { return values; }
        }

        /// <summary>
        /// Gets or sets the <see cref="System.Object"/> with the specified index.
        /// </summary>
        /// <param name="index">The index of the value to be get or set.</param>
        /// <value>
        /// The value of a parameter specified by its <paramref name="index"/>.
        /// </value>
        public object this[int index] {
            get {
                if (IsValueSet(index))
                {
                    return values[index];
                }

                throw new InvalidOperationException(string.Format("Parameter '{0}' has not been set.",
                                                                  GetParameterName(index)));
            }

            set {
                if (CanValueBeSet(index))
                {
                    values[index] = value;
                    isValueSet[index] = true;
                }
                else
                {
                    throw new InvalidOperationException(string.Format("Cannot set the value of in parameter '{0}'",
                                                                      GetParameterName(index)));
                }
            }
        }

        /// <summary>
        /// Determines whether the value specified by index was set.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>
        /// Returns <c>true</c> if value specified by index was set; otherwise, <c>false</c>.
        /// </returns>
        public bool IsValueSet(int index) {
            return isValueSet[index];
        }

        /// <summary>
        /// Marks all values as set.
        /// </summary>
        internal void MarkAllValuesAsSet() {
            isValueSet.SetAll(true);
        }

        /// <summary>
        /// Determines whether the parameter specified by index can be set.
        /// </summary>
        /// <param name="index">The index of the parameter.</param>
        /// <returns>
        /// Returns <c>true</c> if the parameter specified by index can be set; otherwise, <c>false</c>.
        /// </returns>
        private bool CanValueBeSet(int index) {
            return !method.GetParameters()[index].IsIn;
        }

        /// <summary>
        /// Gets the parameter name by index.
        /// </summary>
        /// <param name="index">The index of the parameter name to get.</param>
        /// <returns>
        /// Returns the parameter name with the given index.
        /// </returns>
        private string GetParameterName(int index) {
            return method.GetParameters()[index].Name;
        }
    }
}