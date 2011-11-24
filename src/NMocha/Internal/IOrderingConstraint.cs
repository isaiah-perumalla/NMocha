using NMock2;

namespace NMocha.Internal {

    public interface IOrderingConstraint : ISelfDescribing {
        bool AllowsInvocationNow();
    }
}