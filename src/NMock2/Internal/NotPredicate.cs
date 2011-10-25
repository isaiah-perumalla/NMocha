using System;
using System.IO;

namespace NMock2.Internal {
    public class NotPredicate : IStatePredicate {
        private readonly string name;
        private readonly string state;
        private readonly Predicate<string> predicate;

        public NotPredicate(string name, Predicate<string> predicate, string state) {
            this.name = name;
            this.state = state;
            this.predicate = predicate;
            
        }

        public void DescribeTo(TextWriter writer) {
            writer.Write(name);
            writer.Write(" is not ");
            writer.Write(state);
        }

        public bool IsActive() {
            return !this.predicate(state);
        }
    }
}