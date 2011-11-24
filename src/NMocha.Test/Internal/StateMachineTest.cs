using System;
using System.Collections.Generic;
using NMocha;
using NMocha.Internal;
using NUnit.Framework;

namespace NMock2.Internal {
    [TestFixture]
    public class StateMachineTest {
        private readonly HashSet<string> states = new HashSet<string> {"state-1", "state-2", "state-3"};

        private static HashSet<string> Except(string s, HashSet<string> states) {
            var hashSet = new HashSet<string>(states);
            hashSet.Remove(s);
            return hashSet;
        }

        [Test]
        public void CanEnterAState() {
            HashSet<string> otherStates = Except("state1", states);
            var stateMachine = new StateMachine("test-state");
            stateMachine.Is("state1").Activate();
            Assert.That(stateMachine.Is("state1").IsActive(), "should be active");
            foreach (string otherState in otherStates)
            {
                Assert.IsFalse(stateMachine.Is(otherState).IsActive(), "should not be in other state");
                Assert.That(stateMachine.IsNot(otherState).IsActive(), "should not be in other state");
            }
        }

        [Test]
        public void DescribesNameAndItsCurrentState() {
            var stateMachine = new StateMachine("fruitness");

            Assert.That(StringDescription.Describe(stateMachine),
                        NUnit.Framework.Is.EqualTo(string.Format("fruitness has no current state")));
            stateMachine.Is("apple").Activate();

            Assert.That(StringDescription.Describe(stateMachine),
                        NUnit.Framework.Is.EqualTo(string.Format("fruitness is apple")));
        }

        [Test]
        public void ShouldNotBeInAnyStateInitially() {
            var stateMachine = new StateMachine("test-state");
            foreach (string state in states)
            {
                Assert.IsFalse(stateMachine.Is(state).IsActive(), "state machine should not be in any state");
                Assert.IsTrue(stateMachine.IsNot(state).IsActive(), "state machine should not be in any state");
            }
        }
    }
}