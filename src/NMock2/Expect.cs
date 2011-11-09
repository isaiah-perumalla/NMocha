//-----------------------------------------------------------------------
// <copyright file="Expect.cs" company="NMock2">
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
using System;
using NMock2.Internal;
using NMock2.Syntax;

namespace NMock2 {
    /// <summary>
    /// Defines expectations on dynamic mocks.
    /// Expectations that are not fulfilled result in exceptions in <see cref="Mockery.VerifyAllExpectationsHaveBeenMet"/>
    /// or when the <see cref="Mockery"/> is disposed.
    /// </summary>
    public class Expect {
        /// <summary>
        /// Gets a receiver of a method, property, etc. that must never be called.
        /// </summary>
        public static IReceiverSyntax Never {
            get { return new ExpectationBuilder(Cardinality.Exactly(0)); }
        }

        /// <summary>
        /// Gets a receiver of a method, property, etc. that has to be called exactly once.
        /// </summary>
        public static IReceiverSyntax Once {
            get { return Exactly(1); }
        }

        /// <summary>
        /// Gets a receiver of a method, property, etc. that has to be called at least once.
        /// </summary>
        public static IReceiverSyntax AtLeastOnce {
            get { return AtLeast(1); }
        }

        /// <summary>
        /// Gets a receiver of a method, property, etc. that has to be called exactly <paramref name="count"/> times.
        /// </summary>
        /// <param name="count">Expected number of invocations.</param>
        /// <returns>Returns a receiver of a method, property, etc. that has to be called exactly <paramref name="count"/> times.</returns>
        public static IReceiverSyntax Exactly(int count) {
            return new ExpectationBuilder(Cardinality.Exactly(count));
        }

        /// <summary>
        /// Gets a receiver of a method, property, etc. that has to be called at least <paramref name="count"/> times.
        /// </summary>
        /// <param name="count">Minimal allowed number of invocations.</param>
        /// <returns>Returns a receiver of a method, property, etc. that has to be called at least <paramref name="count"/> times.</returns>
        public static IReceiverSyntax AtLeast(int count) {
            return new ExpectationBuilder(Cardinality.AtLeast(count));
        }

        /// <summary>
        /// Gets a receiver of a method, property, etc. that has to be called at most <paramref name="count"/> times.
        /// </summary>
        /// <param name="count">Maximal allowed number of invocations.</param>
        /// <returns>Returns a receiver of a method, property, etc. that has to be called at most <paramref name="count"/> times.</returns>
        public static IReceiverSyntax AtMost(int count) {
            return new ExpectationBuilder(Cardinality.AtMost(count));
        }

        /// <summary>
        /// Gets a receiver of a method, property, etc. that has to be called between <paramref name="minCount"/>
        /// and <paramref name="maxCount"/> times.
        /// </summary>
        /// <param name="minCount">Minimal allowed number of invocations.</param>
        /// <param name="maxCount">Maximaal allowed number of invocations.</param>
        /// <returns>Returns a receiver of a method, property, etc. that has to be called between <paramref name="count"/> times.</returns>
        public static IReceiverSyntax Between(int minCount, int maxCount) {
            return new ExpectationBuilder(Cardinality.Between(minCount, maxCount));
        }

        /// <summary>
        /// Default expectation, specifies that a method, property, etc. that has to be called at least once.
        /// </summary>
        /// <param name="receiver">The receiver.</param>
        /// <returns>Returns a receiver of a method, property, etc. that has to be called at least once.</returns>
        public static IMethodSyntax On(object receiver) {
            return AtLeastOnce.On(receiver);
        }

        /// <summary>
        /// Returns a string representing grammatically correctness of n times depending on the value of <paramref name="n"/>.
        /// </summary>
        /// <param name="n">An integer value representing n times.</param>
        /// <returns>String representation of n times.</returns>
        private static string Times(int n) {
            return n + ((n == 1) ? " time" : " times");
        }
    }

    public  struct Cardinality : ISelfDescribing{
        private readonly int required;
        private readonly int maximum;
        public static readonly Cardinality AllowAny = AtLeast(0);
        private static readonly Cardinality NeverCardinality = new Cardinality(0,0);

        private Cardinality(int required, int maximum) {
            this.required = required;
            this.maximum = maximum;
        }

        public static Cardinality AtMost(int count) {
            return Between(0, count);
        }

        public static Cardinality Between(int required, int count) {
            return new Cardinality(required, count);
        }

        public static Cardinality AtLeast(int count) {
            return Between(count, int.MaxValue);
        }

        public static Cardinality Exactly(int count) {
            return Between(count, count);
        }

        public bool AllowsMoreInvocations(int callCount) {
            return callCount < maximum;
        }

        public bool IsSatisfied(int callCount) {
            return callCount >= required && callCount <= maximum;
        }

        public void DescribeOn(IDescription description) {
            if (Equals(AllowAny)) description.AppendText("allowed");
            else if (maximum == 1 && required == 1) DescribeExpected(description, "once");
            else if (maximum == int.MaxValue && required == 1) DescribeExpected(description, "atleast once", required);
            else if (maximum == int.MaxValue && required > 1) DescribeExpected(description, "atleast {0} times", required);
            else if (maximum == required && required > 1) DescribeExpected(description, "exactly {0} times", required);
            else if (0 == required && maximum > 0) DescribeExpected(description,"at most {0} times", maximum);
            else if (Equals( NeverCardinality)) DescribeExpected(description,"never");
           
            
        }

        private void DescribeExpected(IDescription description, string atleastTimes, params object[] args) {
            description.AppendText("expected ")
                .AppendTextFormat(atleastTimes, args);
        }

        public static Cardinality Never() {
            return NeverCardinality;
        }
    }
}