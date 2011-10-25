//-----------------------------------------------------------------------
// <copyright file="GenericCollectAction.cs" company="NMock2">
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
    using System.IO;
    using Monitoring;

    /// <summary>
    /// Action that calls the collect delegate passed to constructor with the n-th element of the arguments to an invocation.
    /// </summary>
    /// <typeparam name="T">Type of the argument to collect.</typeparam>
    public class CollectAction<T> : IAction
    {
        /// <summary>
        /// Stores the index of the argument.
        /// </summary>
        private readonly int argumentIndex;

        /// <summary>
        /// Stores the collect delegate.
        /// </summary>
        private readonly Collect collectDelegate;

        /// <summary>
        /// Initializes a new instance of the <see cref="CollectAction&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="argumentIndex">Index of the argument.</param>
        /// <param name="collectDelegate">The collect delegate.</param>
        public CollectAction(int argumentIndex, Collect collectDelegate)
        {
            this.argumentIndex = argumentIndex;
            this.collectDelegate = collectDelegate;
        }

        /// <summary>
        /// Delegate that is called on collecting an argument.
        /// </summary>
        /// <param name="collectedParameter">The collected generic parameter.</param>
        public delegate void Collect(T collectedParameter);

        /// <summary>
        /// Invokes this object.
        /// </summary>
        /// <param name="invocation">The invocation.</param>
        public void Invoke(Invocation invocation)
        {
            this.collectDelegate((T)invocation.Parameters[this.argumentIndex]);
        }

        /// <summary>
        /// Describes this object.
        /// </summary>
        /// <param name="writer">The text writer the description is added to.</param>
        public void DescribeTo(TextWriter writer)
        {
            writer.Write("collect argument at index ");
            writer.Write(this.argumentIndex);
        }
    }
}