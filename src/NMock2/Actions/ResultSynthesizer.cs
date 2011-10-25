//-----------------------------------------------------------------------
// <copyright file="ResultSynthesizer.cs" company="NMock2">
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
namespace NMock2.Actions
{
    using System;
    using System.Collections;
    using System.IO;
    using Monitoring;

    /// <summary>
    /// Responsible for handling the results of an invocation.
    /// </summary>
    public class ResultSynthesizer : IAction
    {
        /// <summary>
        /// Stores the default results.
        /// </summary>
        private static readonly Hashtable defaultResults = new Hashtable();

        /// <summary>
        /// Stores the results.
        /// </summary>
        private readonly Hashtable results = new Hashtable();

        /// <summary>
        /// Initializes static members of the <see cref="ResultSynthesizer"/> class.
        /// </summary>
        static ResultSynthesizer()
        {
            defaultResults[typeof(string)] = new ReturnAction(String.Empty);
            defaultResults[typeof(ArrayList)] = new ReturnCloneAction(new ArrayList());
            defaultResults[typeof(SortedList)] = new ReturnCloneAction(new SortedList());
            defaultResults[typeof(Hashtable)] = new ReturnCloneAction(new Hashtable());
            defaultResults[typeof(Queue)] = new ReturnCloneAction(new Queue());
            defaultResults[typeof(Stack)] = new ReturnCloneAction(new Stack());
        }

        /// <summary>
        /// Invokes this object.
        /// </summary>
        /// <param name="invocation">The invocation.</param>
        public void Invoke(Invocation invocation)
        {
            Type returnType = invocation.Method.ReturnType;

            if (returnType == typeof(void))
            {
                return; // sanity check
            }

            if (this.results.ContainsKey(returnType))
            {
                IAction action = this.GetAction(returnType, this.results);
                action.Invoke(invocation);
            }
            else if (returnType.IsArray)
            {
                invocation.Result = NewEmptyArray(returnType);
            }
            else if (returnType.IsValueType)
            {
                invocation.Result = Activator.CreateInstance(returnType);
            }
            else if (defaultResults.ContainsKey(returnType))
            {
                IAction action = this.GetAction(returnType, defaultResults);
                action.Invoke(invocation);
            }
            else
            {
                throw new InvalidOperationException("No action registered for return type " + returnType);
            }
        }

        /// <summary>
        /// Sets the result of the specified <paramref name="returnType"/>.
        /// </summary>
        /// <param name="returnType">The type to be returned as a result.</param>
        /// <param name="result">The result to be set.</param>
        public void SetResult(Type returnType, object result)
        {
            this.SetAction(returnType, Return.Value(result));
        }

        /// <summary>
        /// Describes this object to the specified <paramref name="writer"/>.
        /// </summary>
        /// <param name="writer">The text writer the description is added to.</param>
        public void DescribeTo(TextWriter writer)
        {
            writer.Write("a synthesized result");
        }

        /// <summary>
        /// Gets a new the empty array of the specified <paramref name="arrayType"/>.
        /// </summary>
        /// <param name="arrayType">Type of the array to be returned.</param>
        /// <returns>
        /// Returns a new empty array of the specified <paramref name="arrayType"/>.
        /// </returns>
        private static object NewEmptyArray(Type arrayType)
        {
            int rank = arrayType.GetArrayRank();
            int[] dimensions = new int[rank];

            return Array.CreateInstance(arrayType.GetElementType(), dimensions);
        }

        /// <summary>
        /// Gets the action of the specified <paramref name="returnType"/>.
        /// </summary>
        /// <param name="returnType">Type of the returned action.</param>
        /// <param name="results">The results to get the action from. This is used as a parameter for the <see cref="defaultResults"/>.</param>
        /// <returns>
        /// Returns the action of the specified <paramref name="returnType"/> out of the <paramref name="results"/>.
        /// </returns>
        private IAction GetAction(Type returnType, Hashtable results)
        {
            return (IAction)results[returnType];
        }

        /// <summary>
        /// Sets the action of the specified <paramref name="returnType"/>.
        /// </summary>
        /// <param name="returnType">Type of the action to be set.</param>
        /// <param name="action">The action to be set.</param>
        private void SetAction(Type returnType, IAction action)
        {
            this.results[returnType] = action;
        }
    }
}
