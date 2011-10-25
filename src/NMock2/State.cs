using System;
using System.IO;

namespace NMock2 {
    public class State : IStatePredicate
    {
        
        private readonly StateMachine stateMachine;
        private readonly string state;
        private readonly Predicate<string> statePredicate;

        public State(StateMachine stateMachine, string state, Predicate<string > statePredicate) {
            this.stateMachine = stateMachine;
            this.state = state;
            this.statePredicate = statePredicate;
        }

        public bool IsActive() {
            return this.statePredicate(state);
        }

        public void Activate() {
            stateMachine.SetStateAs(state);
        }

        public void DescribeTo(TextWriter writer) {
            writer.Write(stateMachine.Name);
            writer.Write(" is ");
            writer.Write(this.state);
        }
    }
}