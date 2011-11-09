//-----------------------------------------------------------------------
// <copyright file="BuildableExpectation.cs" company="NMock2">
//
//   http://www.sourceforge.net/projects/NMock2
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
// </copyright>
//-----------------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NMock2.Matchers;
using NMock2.Monitoring;

namespace NMock2.Internal {
    public class InvocationExpectation : IExpectation {
        private readonly Cardinality cardinality;
        private readonly ArrayList actions = new ArrayList();


        private readonly ArrayList extraMatchers = new ArrayList();
   
        private readonly List<IOrderingConstraint> orderingConstraints = new List<IOrderingConstraint>();
   
        private readonly List<ISideEffect> sideEffects = new List<ISideEffect>();
        private Matcher argumentsMatcher = new AlwaysMatcher(true, "(any arguments)");
        private int callCount;
        private string expectationComment;
        private Matcher genericMethodTypeMatcher = new AlwaysMatcher(true, string.Empty);
        private Matcher methodMatcher = new AlwaysMatcher(true, "<any method>");
        private string methodSeparator = ".";
        private IMockObject receiver;

        
        public InvocationExpectation(Cardinality cardinality) {
            this.cardinality = cardinality;
        }

        public IMockObject Receiver {
            get { return receiver; }
            set { receiver = value; }
        }

        public Matcher MethodMatcher {
            get { return methodMatcher; }
            set { methodMatcher = value; }
        }

        public Matcher GenericMethodTypeMatcher {
            get { return genericMethodTypeMatcher; }
            set { genericMethodTypeMatcher = value; }
        }

        public Matcher ArgumentsMatcher {
            get { return argumentsMatcher; }
            set { argumentsMatcher = value; }
        }

        #region IExpectation Members

        public bool IsActive {
            get { return cardinality.AllowsMoreInvocations(callCount); }
        }

        public bool HasBeenMet {
            get { return cardinality.IsSatisfied(callCount); }
        }

       
        public bool Matches(Invocation invocation) {
            return IsActive
                   && receiver == invocation.Receiver
                   && methodMatcher.Matches(invocation.Method)
                   && argumentsMatcher.Matches(invocation)
                   && ExtraMatchersMatch(invocation)
                   && GenericMethodTypeMatcher.Matches(invocation)
                   && IsInCorrectOrder();
        }

        public bool MatchesIgnoringIsActive(Invocation invocation) {
            return receiver == invocation.Receiver
                   && methodMatcher.Matches(invocation.Method)
                   && argumentsMatcher.Matches(invocation)
                   && ExtraMatchersMatch(invocation)
                   && GenericMethodTypeMatcher.Matches(invocation);
        }

        public void Perform(Invocation invocation) {
            callCount++;
            foreach (IAction action in actions)
            {
                action.Invoke(invocation);
            }
            sideEffects.ForEach(sideEffect => sideEffect.Apply());
        }

        public void DescribeActiveExpectationsTo(IDescription writer) {
            if (IsActive)
            {
                DescribeTo(writer);
            }
        }

        public void DescribeUnmetExpectationsTo(IDescription writer) {
             
            DescribeTo(writer);
        }

        #endregion

        public void AddInvocationMatcher(Matcher matcher) {
            extraMatchers.Add(matcher);
        }

        public void AddAction(IAction action) {
            actions.Add(action);
        }

        public void AddComment(string comment) {
            expectationComment = comment;
        }

        private bool IsInCorrectOrder() {
            return orderingConstraints.All(orderConstraint => orderConstraint.AllowsInvocationNow());
        }

        public void DescribeAsIndexer() {
            methodSeparator = string.Empty;
        }

        private bool ExtraMatchersMatch(Invocation invocation) {
            foreach (Matcher matcher in extraMatchers)
            {
                if (!matcher.Matches(invocation))
                {
                    return false;
                }
            }

            return true;
        }

        private void DescribeTo(IDescription writer) {
            DescribeMethod(writer);
            argumentsMatcher.DescribeOn(writer);
            foreach (Matcher extraMatcher in extraMatchers)
            {
                writer.AppendText(", ");
                extraMatcher.DescribeOn(writer);
            }

            if (actions.Count > 0)
            {
                writer.AppendText(", will ");
                ((IAction) actions[0]).DescribeOn(writer);
                for (int i = 1; i < actions.Count; i++)
                {
                    writer.AppendText(", ");
                    ((IAction) actions[i]).DescribeOn(writer);
                }
            }
            DescribeOrderingConstraintsOn(writer);
            sideEffects.ForEach(sideEffect => sideEffect.DescribeOn(writer));


            
            if (!string.IsNullOrEmpty(expectationComment))
            {
                writer.AppendText(" Comment: ")
                      .AppendText(expectationComment);
            }
        }

        private void DescribeMethod(IDescription description) {
            cardinality.DescribeOn(description);
            description.AppendText(", ");
            if (callCount == 0)
            {
                description.AppendText("never invoked");
            }
            else
            {
                description.AppendText("already invoked ");
                description.AppendText(callCount.ToString());
                description.AppendText(" time");
                if (callCount != 1)
                {
                    description.AppendText("s");
                }
            }

            description.AppendText(": ")
                  .AppendText(receiver.MockName)
                  .AppendText(methodSeparator);
            methodMatcher.DescribeOn(description);
            genericMethodTypeMatcher.DescribeOn(description);
        }

        private void DescribeOrderingConstraintsOn(IDescription writer) {
            if (!orderingConstraints.Any()) return;
            writer.AppendText(" ");
            orderingConstraints.ForEach(constraint => constraint.DescribeOn(writer));
        }

        public void AddOrderingConstraint(IOrderingConstraint orderingConstraint) {
            orderingConstraints.Add(orderingConstraint);
        }

        public void AddSideEffect(ISideEffect sideEffect) {
            sideEffects.Add(sideEffect);
        }
    }

    public interface ISideEffect : ISelfDescribing {
        void Apply();
    }
}