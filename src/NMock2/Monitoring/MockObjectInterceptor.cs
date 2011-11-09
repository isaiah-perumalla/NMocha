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
using System;
using System.Collections.Generic;
using System.Reflection;
using Castle.Core.Interceptor;
using NMocha.Internal;
using NMock2.Monitoring;

namespace NMocha.Monitoring {
    internal class MockObjectInterceptor : MockObject, IInterceptor {
        private static readonly Dictionary<MethodInfo, object> mockObjectMethods = new Dictionary<MethodInfo, object>();

        /// <summary>
        /// Initializes static members of the <see cref="MockObjectInterceptor"/> class.
        /// </summary>
        static MockObjectInterceptor() {
            // We want to be able to quickly recognize any later invocations
            // on methods that belong to IMockObject or IInvokable, so we cache
            // their definitions here.
            foreach (MethodInfo methodInfo in typeof (IMockObject).GetMethods())
            {
                mockObjectMethods.Add(methodInfo, null);
            }

            foreach (MethodInfo methodInfo in typeof (IInvokable).GetMethods())
            {
                mockObjectMethods.Add(methodInfo, null);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MockObjectInterceptor"/> class.
        /// </summary>
        /// <param name="mockedType">Type of the mocked.</param>
        /// <param name="name">The name.</param>
        public MockObjectInterceptor(CompositeType mockedType,
            string name, IExpectationCollector expectationCollector, IInvocationListener invocationListener) : base(mockedType, name, expectationCollector, invocationListener) {
        }

        #region IInterceptor Members

        public void Intercept(IInvocation interceptedInvocation) {
            // Check for calls to basic NMock2 infrastructure
            if (mockObjectMethods.ContainsKey(interceptedInvocation.Method))
            {
                try
                {
                    interceptedInvocation.ReturnValue = interceptedInvocation.Method.Invoke(this,
                                                                                            interceptedInvocation.
                                                                                                Arguments);
                }
                catch (TargetInvocationException tie)
                {
                    // replace stack trace with original stack trace
                    FieldInfo remoteStackTraceString = typeof (Exception).GetField("_remoteStackTraceString",
                                                                                   BindingFlags.Instance |
                                                                                   BindingFlags.NonPublic);
                    remoteStackTraceString.SetValue(tie.InnerException,
                                                    tie.InnerException.StackTrace + Environment.NewLine);
                    throw tie.InnerException;
                }

                return;
            }

            // Ok, this call is targeting a member of the mocked class
            object invocationTarget = GetInvocationTarget(interceptedInvocation);
            var invocationForMock = new Invocation(
                invocationTarget,
                interceptedInvocation.Method,
                interceptedInvocation.Arguments);


            interceptedInvocation.ReturnValue = ProcessCallAgainstExpectations(invocationForMock);
        }

        private object GetInvocationTarget(IInvocation interceptedInvocation) {
            return MockedTypes.PrimaryType.IsInterface
                       ? interceptedInvocation.Proxy
                       : interceptedInvocation.InvocationTarget;
        }

        #endregion

        private object ProcessCallAgainstExpectations(Invocation invocationForMock) {
            Invoke(invocationForMock);

            if (invocationForMock.IsThrowing)
            {
                throw invocationForMock.Exception;
            }

            if (invocationForMock.Result == Missing.Value && invocationForMock.Method.ReturnType != typeof (void))
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