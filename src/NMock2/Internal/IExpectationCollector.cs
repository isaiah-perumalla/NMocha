using NMock2;

namespace NMocha.Internal {
    public interface IExpectationCollector {
        void Add(IExpectation expectation);
    }
}