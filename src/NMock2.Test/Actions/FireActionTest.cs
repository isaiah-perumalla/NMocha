//-----------------------------------------------------------------------
// <copyright file="FireActionTest.cs" company="NMock2">
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

namespace NMock2.Test.Actions
{
    using System.Reflection;
    using NMock2.Actions;
    using NMock2.Monitoring;
    using NUnit.Framework;
    
    [TestFixture]
    public class FireActionTest
    {
        public delegate void BellListener(string who);
        private void Salivate(string who) { dog = who; }
        private string dog;

        public interface IBell
        {
            void Ring();
            event BellListener Listeners;
        }

        [Test] 
        public void FiresEventOnInvocationReceiver()
        {
            Mockery mockery = new Mockery();
            IBell receiver = (IBell) mockery.NewInstanceOfRole(typeof(IBell));
            MethodInfo methodInfo = typeof(IBell).GetMethod("Ring");

            Expect.Once.On(receiver).EventAdd("Listeners", new BellListener(Salivate));

            IAction fireEvent = new FireAction("Listeners", "Rover");
            receiver.Listeners += new BellListener(Salivate);
            fireEvent.Invoke(new Invocation(receiver, methodInfo, new object[] { "unused" }));

            Assert.AreEqual("Rover", dog);
            mockery.VerifyAllExpectationsHaveBeenMet();
        }

        [Test]
        public void HasAReadableDescription()
        {
            IAction action = new FireAction("MyEvent", 123);

            AssertDescription.IsEqual(action, "fire MyEvent");
        }
    }
}