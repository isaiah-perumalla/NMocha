namespace NMock2.Internal {

    public interface IOrderingConstraint : ISelfDescribing {
        bool AllowsInvocationNow();
    }
}