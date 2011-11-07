using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NMock2.Internal;

namespace NMock2 {
    public class NamedSequence : ISequence {
        private readonly string name;
        private readonly List<IExpectation> expectationSequence = new List<IExpectation>();

        public NamedSequence(string name) {
            this.name = name;
        }

        public void ConstrainAsNextInSeq(BuildableExpectation expectation) {
            var index = expectationSequence.Count;
            expectationSequence.Add(expectation);
            expectation.AddOrderingConstraint(new InSequenceConstraint(this, index));
        }

        public bool IsSatisfiedUptoIndex(int index) {
            return expectationSequence.Take(index).All(exp => exp.HasBeenMet);
        }

        public override string ToString() {
            return name;
        }

        class InSequenceConstraint : IOrderingConstraint
        {
            private readonly NamedSequence namedSequence;
            private readonly int index;

            public InSequenceConstraint(NamedSequence namedSequence, int index)
            {
                this.namedSequence = namedSequence;
                this.index = index;
            }

            public void DescribeOn(IDescription description)
            {
                description.AppendTextFormat("in sequence {0} ", namedSequence);
            }

            public bool AllowsInvocationNow()
            {
                return namedSequence.IsSatisfiedUptoIndex(index);
            }
        }


        
    }
}