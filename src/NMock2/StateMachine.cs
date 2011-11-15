using NMocha.Internal;
using NMock2;

namespace NMocha {
    public class StateMachine : IStates {
        private readonly string name;
        private string currentState;

        public StateMachine(string name) {
            this.name = name;
            currentState = null;
        }

        public string Name {
            get { return name; }
        }

        #region IStates Members

        public State Is(string state) {
            return State.IsIn(state, this);
        }

        public void StartAs(string s) {
            currentState = s;
        }

        public IStatePredicate IsNot(string state) {
            return new NotPredicate(name, IsCurrentStateEquals, state);
        }

        public void DescribeOn(IDescription description) {
            description.AppendText(name);
            if (string.IsNullOrEmpty(currentState))
            {
                description.AppendText(" has no current state");
            }
            else
            {
                description.AppendText(" is ")
                           .AppendText(currentState);
            }
        }

        #endregion

        public bool IsCurrentStateEquals(string s) {
            return s.Equals(currentState);
        }

        public void SetStateAs(string s) {
            currentState = s;
        }
    }
}