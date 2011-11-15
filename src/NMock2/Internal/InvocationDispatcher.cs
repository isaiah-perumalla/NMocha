//-----------------------------------------------------------------------
// <copyright file="UnorderedExpectations.cs" company="NMock2">
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
using System.Collections.Generic;
using System.Linq;
using NMocha.Monitoring;

namespace NMocha.Internal {
    public class InvocationDispatcher : IExpectationCollector {
      
        private readonly int depth;
        private readonly List<IExpectation> expectations = new List<IExpectation>();
        private readonly List<IStates> stateMachines = new List<IStates>();

        public InvocationDispatcher() {
            depth = 0;
        }

        public InvocationDispatcher(int depth) {
            this.depth = depth;
        }

        #region IExpectationOrdering Members

        public bool IsActive {
            get { return expectations.Any(e => e.IsActive); }
        }

        public bool HasBeenMet {
            get { return expectations.All(e => e.HasBeenMet); }
        }

        
        bool Matches(Invocation invocation) {
            return expectations.Any(e => e.Matches(invocation));
        }


        void Perform(Invocation invocation) {
            foreach (IExpectation e in expectations.Where(e => e.Matches(invocation)))
            {
                e.Perform(invocation);
                return;
            }

            throw new InvalidOperationException("No matching expectation");
        }

        void DescribeActiveExpectationsTo(IDescription  writer) {
            DescribeUnmetExpectationsTo(writer);
        }

        public void DescribeUnmetExpectationsTo(IDescription writer) {

            writer.AppendLine("expectations:");
            foreach (IExpectation expectation in expectations)
            {
                   Indent(writer, depth + 1);
                    expectation.DescribeUnmetExpectationsTo(writer);
                    writer.AppendNewLine();
                
            }
        }

        public void Add(IExpectation expectation) {
            expectations.Add(expectation);
        }

        #endregion

        private static void Indent(IDescription writer, int n) {
            for (var i = 0; i < n; i++)
            {
                writer.AppendText("  ");
            }
        }

        public void Dispatch(Invocation invocation) {
            if (Matches(invocation))
            {
                Perform(invocation);
            }
            else
            {
                FailUnexpectedInvocation(invocation);
            }
        }

        private void FailUnexpectedInvocation(Invocation invocation)
        {
            var description = new StringDescriptionWriter();
            description.AppendText("unexpected invocation of ");
            invocation.DescribeOn(description);

            description.AppendNewLine();
            DescribeActiveExpectationsTo(description);
            
            description.AppendList("\nstates:\n" , Environment.NewLine, string.Empty, stateMachines);
                
            // try catch to get exception with stack trace.
            throw new ExpectationException(description.ToString());
            
            
        }

        public StateMachine NewStateMachine(string name) {
            var stateMachine = new StateMachine(name);
            stateMachines.Add(stateMachine);
            return stateMachine;
        }
    }
}