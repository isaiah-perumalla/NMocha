//-----------------------------------------------------------------------
// <copyright file="ReturnCloneAction.cs" company="NMock2">
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
    using System.IO;
    using NMock2.Monitoring;

    /// <summary>
    /// Action that set the result value of an invocation to a clone of the specified prototype.
    /// </summary>
    public class ReturnCloneAction : IAction
    {
        /// <summary>
        /// Stores the prototype that will be cloned.
        /// </summary>
        private readonly ICloneable prototype;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReturnCloneAction"/> class.
        /// </summary>
        /// <param name="prototype">The prototype.</param>
        public ReturnCloneAction(ICloneable prototype)
        {
            this.prototype = prototype;
        }

        /// <summary>
        /// Invokes this object. Sets the result value of the invocation to a clone of the prototype.
        /// </summary>
        /// <param name="invocation">The invocation.</param>
        public void Invoke(Invocation invocation)
        {
            invocation.Result = this.prototype.Clone();
        }

        /// <summary>
        /// Describes this object.
        /// </summary>
        /// <param name="writer">The text writer the description is added to.</param>
        public void DescribeTo(TextWriter writer)
        {
            writer.Write("a clone of ");
            writer.Write(this.prototype);
        }
    }
}
