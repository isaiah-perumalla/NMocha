using NMocha.Internal;
using NUnit.Framework;

namespace NMocha.Test.Internal {
    
    [TestFixture]
    public class CardinalityTest {
        
        [Test]
        public void DescribesOnceCardinality() {
            Assert.AreEqual("expected once", StringDescription.Describe(Cardinality.Exactly(1)));
        }
        
        [Test]
        public void DescribesExactCardinality() {
            Assert.AreEqual("expected exactly 2 times", StringDescription.Describe(Cardinality.Exactly(2)));
        }
        
        [Test]
        public void DescribesAtMostCardinality() {
            Assert.AreEqual("expected at most 2 times", StringDescription.Describe(Cardinality.AtMost(2)));
        }

        [Test]
        public void DescribesAtleastOnceCardinality()
        {
            Assert.AreEqual("expected atleast once", StringDescription.Describe(Cardinality.AtLeast(1)));
        }
        
        [Test]
        public void DescribesAtleastCardinality()
        {
            Assert.AreEqual("expected atleast 2 times", StringDescription.Describe(Cardinality.AtLeast(2)));
        }
        
        
        [Test]
        public void DescribesNever()
        {
            Assert.AreEqual("expected never", StringDescription.Describe(Cardinality.Never()));
            Assert.AreEqual("expected never", StringDescription.Describe(Cardinality.Exactly(0)));
        }
        
        [Test]
        public void DescribesAtAllowingAnyCardinality()
        {
            Assert.AreEqual("allowed", StringDescription.Describe(Cardinality.AllowAny));
            Assert.AreEqual("allowed", StringDescription.Describe(Cardinality.AtLeast(0)));
        }
    }

}