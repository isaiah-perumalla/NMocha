using System;
using NMock2;

namespace NMocha.Internal {
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

        public void DescribeOn(IDescription description) {
            description.AppendText(name);
            description.AppendText(" is not ");
            description.AppendText(state);
        }

        public bool IsActive() {
            return !predicate(state);
        }

        #endregion
    }
}