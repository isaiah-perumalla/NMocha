using System.IO;

namespace NMock2.Internal {
    public class ChangeStateEffect : ISideEffect {
        private readonly State state;

        public ChangeStateEffect(State state) {
            this.state = state;
        }

        public void DescribeTo(TextWriter writer) {
            writer.Write("\nthen ");
            state.DescribeTo(writer);
            writer.Write(";");
        }

        public void Apply() {
            state.Activate();
        }
    }
}