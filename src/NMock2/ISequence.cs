using NMock2.Internal;

namespace NMock2 {
    public interface ISequence {
        void ConstrainAsNextInSeq(BuildableExpectation expectation);
    }
}