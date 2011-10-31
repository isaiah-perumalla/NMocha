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
    public class BuildableExpectation : IExpectation {
        private const string AddEventHandlerPrefix = "add_";
        private const string RemoveEventHandlerPrefix = "remove_";
        private readonly ArrayList actions = new ArrayList();

        private readonly string expectationDescription;
        private readonly ArrayList extraMatchers = new ArrayList();
        private readonly Matcher matchingCountMatcher;
        private readonly List<IOrderingConstraint> orderingConstraints = new List<IOrderingConstraint>();
        private readonly Matcher requiredCountMatcher;
        private readonly List<ISideEffect> sideEffects = new List<ISideEffect>();
        private Matcher argumentsMatcher = new AlwaysMatcher(true, "(any arguments)");
        private int callCount;
        private string expectationComment;
        private Matcher genericMethodTypeMatcher = new AlwaysMatcher(true, string.Empty);
        private Matcher methodMatcher = new AlwaysMatcher(true, "<any method>");
        private string methodSeparator = ".";
        private IMockObject receiver;

        /// <summary>
        /// Initializes a new instance of the <see cref="BuildableExpectation"/> class.
        /// </summary>
        /// <param name="expectationDescription">The expectation description.</param>
        /// <param name="requiredCountMatcher">The required count matcher.</param>
        /// <param name="matchingCountMatcher">The matching count matcher.</param>
        public BuildableExpectation(string expectationDescription, Matcher requiredCountMatcher,
                                    Matcher matchingCountMatcher) {
            this.expectationDescription = expectationDescription;
            this.requiredCountMatcher = requiredCountMatcher;
            this.matchingCountMatcher = matchingCountMatcher;
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
            get { return matchingCountMatcher.Matches(callCount + 1); }
        }

        public bool HasBeenMet {
            get { return requiredCountMatcher.Matches(callCount); }
        }

        /// <summary>
        /// Checks whether stored expectations matches the specified invocation.
        /// </summary>
        /// <param name="invocation">The invocation to check.</param>
        /// <returns>Returns whether one of the stored expectations has met the specified invocation.</returns>
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

        public void DescribeActiveExpectationsTo(TextWriter writer) {
            if (IsActive)
            {
                DescribeTo(writer);
            }
        }

        public void DescribeUnmetExpectationsTo(TextWriter writer) {
            if (!HasBeenMet)
            {
                DescribeTo(writer);
            }
        }

        /// <summary>
        /// Adds itself to the <paramref name="result"/> if the <see cref="Receiver"/> matches
        /// the specified <paramref name="mock"/>.
        /// </summary>
        /// <param name="mock">The mock for which expectations are queried.</param>
        /// <param name="result">The result to add matching expectations to.</param>
        public void QueryExpectationsBelongingTo(IMockObject mock, IList<IExpectation> result) {
            if (Receiver == mock)
            {
                result.Add(this);
            }
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

        private void DescribeTo(TextWriter writer) {
            writer.Write(expectationDescription);
            writer.Write(": ");
            writer.Write(receiver.MockName);
            writer.Write(methodSeparator);
            methodMatcher.DescribeTo(writer);
            genericMethodTypeMatcher.DescribeTo(writer);
            argumentsMatcher.DescribeTo(writer);
            foreach (Matcher extraMatcher in extraMatchers)
            {
                writer.Write(", ");
                extraMatcher.DescribeTo(writer);
            }

            if (actions.Count > 0)
            {
                writer.Write(", will ");
                ((IAction) actions[0]).DescribeTo(writer);
                for (int i = 1; i < actions.Count; i++)
                {
                    writer.Write(", ");
                    ((IAction) actions[i]).DescribeTo(writer);
                }
            }
            DescribeOrderingConstraintsOn(writer);
            sideEffects.ForEach(sideEffect => sideEffect.DescribeTo(writer));


            writer.Write(" [called ");
            writer.Write(callCount);
            writer.Write(" time");
            if (callCount != 1)
            {
                writer.Write("s");
            }

            writer.Write("]");

            if (!string.IsNullOrEmpty(expectationComment))
            {
                writer.Write(" Comment: ");
                writer.Write(expectationComment);
            }
        }

        private void DescribeOrderingConstraintsOn(TextWriter writer) {
            if (!orderingConstraints.Any()) return;
            writer.Write(" ");
            orderingConstraints.ForEach(constraint => constraint.DescribeTo(writer));
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