//-----------------------------------------------------------------------
// <copyright file="ProxyInvokableAdapterTest.cs" company="NMock2">
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
    using System.Reflection;
    using NUnit.Framework;
    using NMock2.Monitoring;

    public interface ProxiedInterface
    {
        string Return();
        void Throw();
        void InOut( string inArg, ref string refArg, out string outArg );
    }
    
    [TestFixture]
    public class ProxyInvokableAdapterTest
    {
        static readonly MethodInfo RETURN_METHOD = typeof(ProxiedInterface).GetMethod("Return");
        static readonly MethodInfo INOUT_METHOD = typeof(ProxiedInterface).GetMethod("InOut");
        
        MockInvokable invokable;
        ProxyInvokableAdapter adapter;
        ProxiedInterface transparentProxy;

        [SetUp]
        public void SetUp()
        {
            invokable = new MockInvokable();
            adapter = new ProxyInvokableAdapter( typeof(ProxiedInterface), invokable );
            transparentProxy = (ProxiedInterface)adapter.GetTransparentProxy();
        }

        [Test]
        public void CapturesInvocationsOnTransparentProxyAndForwardsToInvokableObject()
        {
            invokable.Expected = new Invocation( transparentProxy, RETURN_METHOD, new object[0] );
            invokable.ResultSetOnInvocation = "result";
            
            transparentProxy.Return();

            Assert.IsNotNull(invokable.Actual, "received invocation");
        }
        
        [Test]
        public void ReturnsResultFromInvocationToCallerOfProxy()
        {
            invokable.ResultSetOnInvocation = "result";

            Assert.AreEqual( invokable.ResultSetOnInvocation, transparentProxy.Return() );
        }
        
        class TestException : Exception {}

        [Test, ExpectedException(typeof(TestException))]
        public void ThrowsExceptionFromInvocationToCallerOfProxy()
        {
            invokable.ExceptionSetOnInvocation = new TestException();

            transparentProxy.Throw();
        }
        
        [Test]
        public void ReturnsOutAndRefParametersFromInvocationToCallerOfProxy()
        {
            invokable.Expected = new Invocation(transparentProxy,INOUT_METHOD,new object[]{"inArg","refArg",null});
            invokable.Outputs = new object[]{null, "returnedRefArg", "returnedOutArg"};
            
            string refArg = "refArg";
            string outArg;
            
            transparentProxy.InOut("inArg", ref refArg, out outArg);

            Assert.AreEqual("returnedRefArg", refArg);
            Assert.AreEqual("returnedOutArg", outArg);
        }

        [Test, ExpectedException(typeof(TestException))]
        public void PassesExceptionThrownByInvokableToCallerOfProxy()
        {
            invokable.ThrownException = new TestException();

            transparentProxy.Throw();
        }
    }
}
