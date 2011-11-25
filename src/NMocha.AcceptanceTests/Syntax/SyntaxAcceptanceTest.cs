using NMock2.Syntax;
using NUnit.Framework;

namespace NMocha.AcceptanceTests.Syntax {
    [TestFixture]
    public class SyntaxAcceptanceTest {
        /*  
        [Test]
        public void SpecifyExpectationWithoutStringMethodName() {

            var mockery = new Mockery();
            var speaker = mockery.NewInstanceOfRole<ISpeaker>();
            speaker.WillReceiveMessage("Hello").With();

            Expect.On(speaker).
            Expect.Once.On(speaker).Message(x => x.Ask(With()));
        }*/
    }

    internal static class Extenstion {
        public static IArgumentSyntax ReceivesMessage<T>(this T role, string message) {
            return null;
        }
    }
}