using System.IO;
using NMocha;
using NMocha.Internal;
using NUnit.Framework;

namespace NMock2.Internal {
    [TestFixture]
    public class InStateOrderingConstraintTest {
        [Test]
        public void AllowsInvocationWhenStateIsActive() {
            var state = new FakeStatePredicate();
            state.isActive = true;
            var inStateOrdering = new InStateOrderingConstraint(state);
            Assert.That(inStateOrdering.AllowsInvocationNow());

            state.isActive = false;
            Assert.IsFalse(inStateOrdering.AllowsInvocationNow());
        }

        [Test]
        public void DescribesItselfOnWriter() {
            var state = new FakeStatePredicate();
            state.description = "Fake Predicate";
            var inStateOrdering = new InStateOrderingConstraint(state);
            Assert.That(StringDescription.Describe(inStateOrdering),
                        NUnit.Framework.SyntaxHelpers.Is.EqualTo("when Fake Predicate"));
        }
    }

    public class StringDescription {
        public static string Describe(ISelfDescribing inStateOrdering) {
            var stringWriter = new StringDescriptionWriter();
            inStateOrdering.DescribeOn(stringWriter);
            return stringWriter.ToString();
        }
    }

    public class FakeStatePredicate : IStatePredicate {
        public string description;
        public bool isActive;

        #region IStatePredicate Members

        public bool IsActive() {
            return isActive;
        }

        public void DescribeOn(IDescription description1) {
            description1.AppendText(description);
        }

        #endregion
    }
}