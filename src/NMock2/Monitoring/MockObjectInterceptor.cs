//-----------------------------------------------------------------------
// <copyright file="MockObjectInterceptor.cs" company="NMock2">
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
    using System.Collections.Generic;
    using System.Reflection;
    using System.Text;
    using Castle.Core.Interceptor;
    using NMock2.Internal;

    internal class MockObjectInterceptor : MockObject, IInterceptor
    {
        private static readonly Dictionary<MethodInfo, object> mockObjectMethods = new Dictionary<MethodInfo, object>();

        /// <summary>
        /// Initializes static members of the <see cref="MockObjectInterceptor"/> class.
        /// </summary>
        static MockObjectInterceptor()
        {
            // We want to be able to quickly recognize any later invocations
            // on methods that belong to IMockObject or IInvokable, so we cache
            // their definitions here.
            foreach (MethodInfo methodInfo in typeof(IMockObject).GetMethods())
            {
                mockObjectMethods.Add(methodInfo, null);
            }

            foreach (MethodInfo methodInfo in typeof(IInvokable).GetMethods())
            {
                mockObjectMethods.Add(methodInfo, null);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MockObjectInterceptor"/> class.
        /// </summary>
        /// <param name="mockery">The mockery.</param>
        /// <param name="mockedType">Type of the mocked.</param>
        /// <param name="name">The name.</param>
        /// <param name="mockStyle">The mock style.</param>
        public MockObjectInterceptor(
            Mockery mockery,
            CompositeType mockedType,
            string name,
            MockStyle mockStyle) : base(mockery, mockedType, name, mockStyle)
        {
        }

        #region IInterceptor Members

        public void Intercept(IInvocation interceptedInvocation)
        {
            // Check for calls to basic NMock2 infrastructure
            if (mockObjectMethods.ContainsKey(interceptedInvocation.Method))
            {
                try
                {
                    interceptedInvocation.ReturnValue = interceptedInvocation.Method.Invoke(this, interceptedInvocation.Arguments);
                }
                catch (TargetInvocationException tie)
                {
                    // replace stack trace with original stack trace
                    FieldInfo remoteStackTraceString = typeof(Exception).GetField("_remoteStackTraceString", BindingFlags.Instance | BindingFlags.NonPublic);
                    remoteStackTraceString.SetValue(tie.InnerException, tie.InnerException.StackTrace + Environment.NewLine);
                    throw tie.InnerException;
                }

                return;
            }

            // Ok, this call is targeting a member of the mocked class
            object invocationTarget = MockedTypes.PrimaryType.IsInterface ? interceptedInvocation.Proxy : interceptedInvocation.InvocationTarget;
            Invocation invocationForMock = new Invocation(
                invocationTarget,
                interceptedInvocation.Method,
                interceptedInvocation.Arguments);

            if (this.ShouldCallInvokeImplementation(invocationForMock))
            {
                interceptedInvocation.Proceed();
                return;
            }

            interceptedInvocation.ReturnValue = this.ProcessCallAgainstExpectations(invocationForMock);
        }

        #endregion

        private bool ShouldCallInvokeImplementation(Invocation invocationForMock)
        {
            // Only transparent mocks of classes can have their implemenation invoked
            if (this.MockStyle == MockStyle.Transparent
                && !MockedTypes.PrimaryType.IsInterface)
            {
                // The implementation should only be invoked if no expectations
                // have been set for this method
                if (!Mockery.HasExpectationFor(invocationForMock))
                {
                    // As classes and interfaces can be combined into a single mock, we have
                    // to be sure that the target method actually belongs to a class
                    if (!invocationForMock.Method.DeclaringType.IsInterface)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private object ProcessCallAgainstExpectations(Invocation invocationForMock)
        {
            this.Invoke(invocationForMock);

            if (invocationForMock.IsThrowing)
            {
                throw invocationForMock.Exception;
            }

            if (invocationForMock.Result == Missing.Value && invocationForMock.Method.ReturnType != typeof(void))
            {
                throw new InvalidOperationException(
                    string.Format(
                        "You have to set the return value for method '{0}' on '{1}' mock.",
                        invocationForMock.Method.Name,
                        invocationForMock.Method.DeclaringType.Name));
            }

            return invocationForMock.Result;
        }
    }
}
