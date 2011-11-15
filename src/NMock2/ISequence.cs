using NMocha.Internal;

namespace NMocha {
    public interface ISequence {
        void ConstrainAsNextInSeq(InvocationExpectation expectation);
    }
}