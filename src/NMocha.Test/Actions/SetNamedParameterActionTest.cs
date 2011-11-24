//-----------------------------------------------------------------------
// <copyright file="SetNamedParameterActionTest.cs" company="NMock2">
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
using System;
using System.Reflection;
using NMocha.Monitoring;
using NMock2.Actions;
using NMock2.Monitoring;
using NMock2.Test.Monitoring;
using NUnit.Framework;

namespace NMock2.Test.Actions {
    [TestFixture]
    public class SetNamedParameterActionTest {
        [Test]
        public void HasReadableDescription() {
            string name = "param";
            object value = new NamedObject("value");

            var action = new SetNamedParameterAction(name, value);

            AssertDescription.IsEqual(action, "set param=<value>");
        }

        [Test]
        public void SetsNamedParameterOnInvocation() {
            var receiver = new object();
            var methodInfo = new MethodInfoStub("method",
                                                new ParameterInfoStub("p1", ParameterAttributes.In),
                                                new ParameterInfoStub("p2", ParameterAttributes.Out));
            string name = "p2";
            var value = new object();
            var invocation = new Invocation(receiver, methodInfo, new object[] {null, null});

            var action = new SetNamedParameterAction(name, value);

            action.Invoke(invocation);

            Assert.AreSame(value, invocation.Parameters[1], "set value");
        }

        [Test]
        [ExpectedException(typeof (ArgumentException))]
        public void SetsNamedParameterOnInvocationWrong() {
            var receiver = new object();
            var methodInfo = new MethodInfoStub("method",
                                                new ParameterInfoStub("p1", ParameterAttributes.In),
                                                new ParameterInfoStub("p2", ParameterAttributes.Out));
            string name = "p2_wrong";
            var value = new object();
            var invocation = new Invocation(receiver, methodInfo, new object[] {null, null});

            var action = new SetNamedParameterAction(name, value);

            action.Invoke(invocation);

            Assert.AreSame(value, invocation.Parameters[1], "set value");
        }
    }
}