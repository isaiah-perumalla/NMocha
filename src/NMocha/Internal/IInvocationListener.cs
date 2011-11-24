using NMocha.Monitoring;
using NMock2.Monitoring;

namespace NMocha.Internal {
    public interface IInvocationListener {
        void NotifyInvocation(Invocation invocation);
    }
}