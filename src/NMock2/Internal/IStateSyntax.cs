using NMock2;
using NMock2.Syntax;

namespace NMocha.Internal {
    public interface IStateSyntax : ICommentSyntax {
        IStateSyntax When(IStatePredicate predicate);
        IStateSyntax Then(State state);
    }
}