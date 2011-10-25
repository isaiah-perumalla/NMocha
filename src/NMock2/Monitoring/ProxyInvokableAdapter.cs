//-----------------------------------------------------------------------
// <copyright file="ProxyInvokableAdapter.cs" company="NMock2">
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
    using System.Collections;
    using System.Reflection;
    using System.Runtime.Remoting.Messaging;
    using System.Runtime.Remoting.Proxies;

    public class ProxyInvokableAdapter : RealProxy
    {
        private readonly IInvokable invokable;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProxyInvokableAdapter"/> class.
        /// </summary>
        /// <param name="proxyType">Type of the proxy.</param>
        /// <param name="invokable">The invokable.</param>
        public ProxyInvokableAdapter(Type proxyType, IInvokable invokable) : base(proxyType)
        {
            this.invokable = invokable;
        }
        
        public override IMessage Invoke(IMessage msg)
        {
            MethodCall call = new MethodCall(msg);
            ParameterInfo[] parameters = call.MethodBase.GetParameters();
            Invocation invocation = new Invocation(
                GetTransparentProxy(),
                (MethodInfo)call.MethodBase,
                call.Args);
            
            this.invokable.Invoke(invocation);
            
            if (invocation.IsThrowing)
            {
                // TODO: it is impossible to set output parameters and throw an exception,
                //       even though this is allowed by .NET method call semantics.
                return new ReturnMessage(invocation.Exception, call);
            }
            else
            {
                object[] outArgs = CollectOutputArguments(invocation, call, parameters);

                MethodInfo methodInfo = (MethodInfo)call.MethodBase;
                if (invocation.Result == Missing.Value && methodInfo.ReturnType != typeof(void))
                {
                    throw new InvalidOperationException(
                        string.Format(
                            "You have to set the return value for method '{0}' on '{1}' mock.",
                            call.MethodName, 
                            call.MethodBase.DeclaringType.Name));
                }

                return new ReturnMessage(
                    invocation.Result, 
                    outArgs, 
                    outArgs.Length,
                    call.LogicalCallContext, 
                    call);
            }
        }
        
        private static object[] CollectOutputArguments(
            Invocation invocation, 
            MethodCall call, 
            ParameterInfo[] parameters)
        {
            ArrayList outArgs = new ArrayList(call.ArgCount);
            
            for (int i = 0; i < call.ArgCount; i++)
            {
                if (!parameters[i].IsIn)
                {
                    outArgs.Add(invocation.Parameters[i]);
                }
            }
            
            return outArgs.ToArray();
        }
    }
}
