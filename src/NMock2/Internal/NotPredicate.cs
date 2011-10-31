using System;
using System.IO;

namespace NMock2.Internal {
    public class NotPredicate : IStatePredicate {
        private readonly string name;
        private readonly Predicate<string> predicate;
        private readonly string state;

        public NotPredicate(string name, Predicate<string> predicate, string state) {
            this.name = name;
            this.state = state;
            this.predicate = predicate;
        }

        #region IStatePredicate Members

        public void DescribeTo(TextWriter writer) {
            writer.Write(name);
            writer.Write(" is not ");
            writer.Write(state);
        }

        public bool IsActive() {
            return !predicate(state);
        }

        #endregion
    }
}