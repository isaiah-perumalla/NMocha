//-----------------------------------------------------------------------
// <copyright file="MockInvokable.cs" company="NMock2">
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
// This is the easiest way to ignore StyleCop rules on this file, even if we shouldn't use this tag:
// <auto-generated />
//-----------------------------------------------------------------------
namespace NMock2.Test.Monitoring
{
    using System;
    using NUnit.Framework;
    using NMock2.Monitoring;

    public class MockInvokable : IInvokable
    {
        public Invocation Expected;
        public Invocation Actual;
        public object[] Outputs;
        public object ResultSetOnInvocation = null;
        public Exception ExceptionSetOnInvocation = null;
        public Exception ThrownException = null;
        public bool expectNotCalled = false;

        public void ExpectNotCalled()
        {
            this.expectNotCalled = true;
        }

        public void Invoke(Invocation invocation)
        {
            Assert.IsFalse(expectNotCalled, "MockInvokable should not have been invoked");

            Actual = invocation;
            if (Expected != null) Assert.AreEqual( Expected.Method, Actual.Method, "method");
            if (Outputs != null)
            {
                for (int i = 0; i < Actual.Parameters.Count; i++)
                {
                    if (!Actual.Method.GetParameters()[i].IsIn)
                    {
                        Actual.Parameters[i] = Outputs[i];
                    }
                }
            }
            
            if (ThrownException != null) throw ThrownException;

            if (ExceptionSetOnInvocation != null)
            {
                invocation.Exception = ExceptionSetOnInvocation;
            }
            else if(invocation.Method.ReturnType != typeof(void))
            {
                invocation.Result = ResultSetOnInvocation;
            }
        }
    }
}