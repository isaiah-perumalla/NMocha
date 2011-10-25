using System;
using System.IO;
using NMock2.Internal;

namespace NMock2 {
    public class StateMachine : IStates {
        private readonly string name;
        private string currentState;

        public StateMachine(string name) {
            this.name = name;
            this.currentState = null;
        }

        public string Name {
            get {
                return name;
            }
            
        }

        public State Is(string state) {
            return State.IsIn(state, this);
        }

        public void StartAs(string s) {
            this.currentState = s;
        }

        public IStatePredicate IsNot(string state) {
            return new NotPredicate(name, IsCurrentStateEquals, state);
        }

        public bool IsCurrentStateEquals(string s) {
            return s.Equals(currentState);
        }

        public void SetStateAs(string s) {
            this.currentState = s;
        }

        public void DescribeTo(TextWriter writer) {
            writer.Write(this.name);
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
    }
}