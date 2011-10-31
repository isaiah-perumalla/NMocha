using System.IO;

namespace NMock2.Internal {
    public class InStateOrderingConstraint : IOrderingConstraint {
        private readonly IStatePredicate predicate;

        public InStateOrderingConstraint(IStatePredicate predicate) {
            this.predicate = predicate;
        }

        #region IOrderingConstraint Members

        public bool AllowsInvocationNow() {
            return predicate.IsActive();
        }

        public void DescribeTo(TextWriter writer) {
            writer.Write("when ");
            predicate.DescribeTo(writer);
        }

        #endregion
    }
}