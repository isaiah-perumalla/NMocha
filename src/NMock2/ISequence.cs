using NMocha.Internal;

namespace NMock2 {
    public interface ISequence {
        void ConstrainAsNextInSeq(InvocationExpectation expectation);
    }
}