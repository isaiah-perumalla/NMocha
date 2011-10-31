using NMock2.Monitoring;

namespace NMock2.Internal {
    public interface IInvocationListener {
        void NotifyInvocation(Invocation invocation);
    }
}