using System.IO;

namespace NMock2 {
    public class State : IStatePredicate
    {
        
        private readonly StateMachine stateMachine;
        private readonly string state;

        public State(StateMachine stateMachine, string state) {
            this.stateMachine = stateMachine;
            this.state = state;
            
        }

        public bool IsActive() {
            return this.stateMachine.IsCurrentStateEquals(state);
        }

        public void Activate() {
            stateMachine.SetStateAs(state);
        }

        public void DescribeTo(TextWriter writer) {
            writer.Write(stateMachine.Name);
            writer.Write(" is ");
            writer.Write(this.state);
        }

        public static State IsIn(string s, StateMachine stateMachine) {
            return new State(stateMachine, s);
        }
    }
}