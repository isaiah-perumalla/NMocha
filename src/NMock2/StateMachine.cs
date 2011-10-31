using System.IO;
using NMock2.Internal;

namespace NMock2 {
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

        public void DescribeTo(TextWriter writer) {
            writer.Write(name);
            if (string.IsNullOrEmpty(currentState))
            {
                writer.WriteLine(" has no current state");
            }
            else
            {
                writer.Write(" is ");
                writer.WriteLine(currentState);
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