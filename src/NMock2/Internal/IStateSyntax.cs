using NMock2.Syntax;

namespace NMock2.Internal {
    public interface IStateSyntax : ICommentSyntax {
        IStateSyntax When(IStatePredicate predicate);
        IStateSyntax Then(State state);
    }
}