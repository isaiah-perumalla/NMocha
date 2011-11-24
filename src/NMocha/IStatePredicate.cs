using NMock2;

namespace NMocha {
    public interface IStatePredicate : ISelfDescribing {
        bool IsActive();
    }
}