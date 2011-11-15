//-----------------------------------------------------------------------
// <copyright file="MockObject.cs" company="NMock2">
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
using System.Collections.Generic;
using System.Reflection;
using NMocha.Monitoring;

namespace NMocha.Internal {
    public class MockObject : IInvokable, IMockObject {
      
        private readonly CompositeType mockedTypes;

        private readonly string name;

        private readonly IExpectationCollector expectationCollector;
        private readonly IInvocationListener invocationListener;

        protected MockObject(CompositeType mockedType, string name, IExpectationCollector expectationCollector, IInvocationListener invocationListener) {
            this.name = name;
            this.invocationListener = invocationListener;
            this.expectationCollector = expectationCollector;
            mockedTypes = mockedType;
        }

        protected CompositeType MockedTypes {
            get { return mockedTypes; }
        }

        #region IInvokable Members

        public void Invoke(Invocation invocation) {
            invocationListener.NotifyInvocation(invocation);
        }

        #endregion

        #region IMockObject Members

        public string MockName {
            get { return name; }
        }

        public bool HasMethodMatching(Matcher methodMatcher) {
            return mockedTypes.GetMatchingMethods(methodMatcher, true).Count > 0;
        }

        public IList<MethodInfo> GetMethodsMatching(Matcher methodMatcher) {
            return mockedTypes.GetMatchingMethods(methodMatcher, false);
        }

        public void AddExpectation(IExpectation expectation) {
            expectationCollector.Add(expectation);
        }

        #endregion

        public override string ToString() {
            return name;
        }
    }
}