using NMock2;

namespace NMocha.Internal {
    public class ChangeStateEffect : ISideEffect {
        private readonly State state;

        public ChangeStateEffect(State state) {
            this.state = state;
        }

        #region ISideEffect Members

        public void DescribeOn(IDescription description) {
            description.AppendText("\nthen ");
            state.DescribeOn(description);
            description.AppendText(";");
        }

        public void Apply() {
            state.Activate();
        }

        #endregion
    }
}