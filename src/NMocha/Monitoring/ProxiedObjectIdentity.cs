//-----------------------------------------------------------------------
// <copyright file="ProxiedObjectIdentity.cs" company="NMock2">
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
using System.Reflection;
using NMock2.Monitoring;

namespace NMocha.Monitoring {
    public class ProxiedObjectIdentity : IInvokable {
        private static readonly MethodInfo EqualsMethod =
            typeof (object).GetMethod("Equals", new[] {typeof (object)});

        private readonly object identityProvider;
        private readonly IInvokable next;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProxiedObjectIdentity"/> class.
        /// </summary>
        /// <param name="identityProvider">The identity provider.</param>
        /// <param name="next">The next object to be invoked.</param>
        public ProxiedObjectIdentity(object identityProvider, IInvokable next) {
            this.identityProvider = identityProvider;
            this.next = next;
        }

        #region IInvokable Members

        public void Invoke(Invocation invocation) {
            if (invocation.Method.DeclaringType == typeof (object))
            {
                if (invocation.Method.Equals(EqualsMethod))
                {
                    invocation.Result = ReferenceEquals(invocation.Receiver, invocation.Parameters[0]);
                }
                else
                {
                    invocation.InvokeOn(identityProvider);
                }
            }
            else
            {
                next.Invoke(invocation);
            }
        }

        #endregion
    }
}