using NMock2;

namespace NMocha {
    public class State : IStatePredicate {
        private readonly string state;
        private readonly StateMachine stateMachine;

        public State(StateMachine stateMachine, string state) {
            this.stateMachine = stateMachine;
            this.state = state;
        }

        #region IStatePredicate Members

        public bool IsActive() {
            return stateMachine.IsCurrentStateEquals(state);
        }

        public void DescribeOn(IDescription description) {
            description.AppendText(stateMachine.Name);
            description.AppendText(" is ");
            description.AppendText(state);
        }

        #endregion

        public void Activate() {
            stateMachine.SetStateAs(state);
        }

        public static State IsIn(string s, StateMachine stateMachine) {
            return new State(stateMachine, s);
        }
    }
}