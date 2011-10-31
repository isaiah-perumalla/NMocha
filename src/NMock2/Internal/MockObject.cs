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
using System;
using System.Collections.Generic;
using System.Reflection;
using NMock2.Monitoring;

namespace NMock2.Internal {
    public class MockObject : IInvokable, IMockObject {
        /// <summary>
        /// Stores the mocked type(s).
        /// </summary>
        private readonly CompositeType mockedTypes;

        /// <summary>
        /// Stores the name of the mock object.
        /// </summary>
        private readonly string name;

        /// <summary>
        /// Results that have been generated for methods or property getters.
        /// These will be returned for all subsequent calls to the same member.
        /// </summary>
        private readonly Dictionary<MethodInfo, object> rememberedMethodResults = new Dictionary<MethodInfo, object>();

        private IExpectationCollector expectationCollector;
        private IInvocationListener invocationListener;

        /// <summary>
        /// Initializes a new instance of the <see cref="MockObject"/> class.
        /// This constructor is needed by the <see cref="InterfaceOnlyMockObjectFactory"/> (the IL generation has to be changed!)
        /// </summary>
        /// <param name="mockedType">Type of the mocked.</param>
        /// <param name="name">The name.</param>
        protected MockObject(Type mockedType, string name, IExpectationCollector expectationCollector, IInvocationListener invocationListener)
            : this(new CompositeType(new[] {mockedType}), name, expectationCollector, invocationListener) {
            this.expectationCollector = expectationCollector;
            this.invocationListener = invocationListener;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MockObject"/> class.
        /// </summary>
        /// <param name="mockedType">Type of the mocked.</param>
        /// <param name="name">The name.</param>
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

    public interface IInvocationListener {
        void NotifyInvocation(Invocation invocation);
    }

    public interface IExpectationCollector {
        void Add(IExpectation expectation);
    }
}