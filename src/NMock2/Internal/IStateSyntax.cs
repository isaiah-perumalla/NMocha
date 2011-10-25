namespace NMock2.Internal {
    public interface IStateSyntax {
        void When(IStatePredicate predicate);
        void Then(State state);
    }
}