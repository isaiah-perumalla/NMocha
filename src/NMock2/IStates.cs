using NMock2;

namespace NMocha {
    public interface IStates : ISelfDescribing {
        State Is(string state);
        void StartAs(string s);
        IStatePredicate IsNot(string state);
    }
}