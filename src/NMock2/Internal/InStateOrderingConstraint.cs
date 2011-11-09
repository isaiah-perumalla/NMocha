using NMock2;

namespace NMocha.Internal {
    public class InStateOrderingConstraint : IOrderingConstraint {
        private readonly IStatePredicate predicate;

        public InStateOrderingConstraint(IStatePredicate predicate) {
            this.predicate = predicate;
        }

        #region IOrderingConstraint Members

        public bool AllowsInvocationNow() {
            return predicate.IsActive();
        }

        public void DescribeOn(IDescription description) {
            description.AppendText("when ");
            predicate.DescribeOn(description);
        }

        #endregion
    }
}