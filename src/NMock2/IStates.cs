using NMock2.Internal;

namespace NMock2 {
    public interface IStates : ISelfDescribing {
        State Is(string state);
        void StartAs(string s);
    }
}