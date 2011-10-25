//-----------------------------------------------------------------------
// <copyright file="Invoker.cs" company="NMock2">
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
namespace NMock2.Monitoring
{
    using System;

    /// <summary>
    /// An invoker invokes an <see cref="Invocation"/> on a target if
    /// it is responsible for the target type, otherwise the invocation is passed
    /// to the next invoker in the 'next' chain.
    /// </summary>
    public class Invoker : IInvokable
    {
        /// <summary>
        /// Holds the type of the target. Can not be inferred from target because it could be a base type.
        /// </summary>
        private readonly Type targetType;

        /// <summary>
        /// Holds the target.
        /// </summary>
        private readonly object target;

        /// <summary>
        /// Holds the next <see cref="IInvokable"/> to pass the invocation to, 
        /// if this instance is not responsible for the target type on an invocation.
        /// </summary>
        private readonly IInvokable next;

        /// <summary>
        /// Initializes a new instance of the <see cref="Invoker"/> class.
        /// </summary>
        /// <param name="targetType">Type of the target. Can not be inferred from <paramref name="target"/> 
        /// because it could be a base type.</param>
        /// <param name="target">The target.</param>
        /// <param name="next">The next <see cref="IInvokable"/> to pass the invocation to, 
        /// if this instance is not responsible for the target type on an invocation.</param>
        public Invoker(Type targetType, object target, IInvokable next)
        {
            this.targetType = targetType;
            this.target = target;
            this.next = next;
        }

        /// <summary>
        /// Executes the <paramref name="invocation"/> on the target of this instance
        /// if the targetType of this instance matches the invocation, otherwise the invocation
        /// is passed to the next <see cref="IInvokable"/> specified in the constructor.
        /// </summary>
        /// <param name="invocation">The invocation.</param>
        public void Invoke(Invocation invocation)
        {
            if (this.targetType == invocation.Method.DeclaringType)
            {
                invocation.InvokeOn(this.target);
            }
            else
            {
                this.next.Invoke(invocation);
            }
        }
    }
}
