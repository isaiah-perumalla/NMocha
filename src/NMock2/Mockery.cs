//-----------------------------------------------------------------------
// <copyright file="Mockery.cs" company="NMock2">
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
using System.Reflection;
using NMock2.Internal;
using NMock2.Monitoring;

namespace NMock2 {
    /// <summary>
    /// Delegate used to override default type returned in stub behavior.
    /// </summary>
    /// <param name="mock">The mock that has to return a value.</param>
    /// <param name="requestedType">Type of the return value.</param>
    /// <returns>The object to return as return value for the requested type.</returns>
    public delegate object ResolveTypeDelegate(object mock, Type requestedType);

    /// <summary>
    /// The mockery is used to create dynamic mocks and check that all expectations were met during a unit test.
    /// </summary>
    /// <remarks>Name inspired by Ivan Moore.</remarks>
    public class Mockery :  IDisposable, IExpectationCollector, IInvocationListener {
        /// <summary>
        /// The mock object factory that is being used by this Mockery instance.
        /// </summary>
        private IMockObjectFactory mockObjectFactory;

        private readonly List<IStates> stateMachines = new List<IStates>();

 
        private int depth;

       
        private IExpectationOrdering expectations;

      
        private ResolveTypeDelegate resolveTypeDelegate;

  
        private ExpectationException thrownUnexpectedInvocationException;

        
        private IExpectationOrdering topOrdering;

     
        public Mockery() {
            mockObjectFactory = new CastleMockObjectFactory(this, this);

            ClearExpectations();
        }

     
        public IDisposable Ordered {
            get { return Push(new OrderedExpectations(depth)); }
        }

        
        public IDisposable Unordered {
            get { return Push(new UnorderedExpectations(depth)); }
        }

        #region IDisposable Members

        
        public void Dispose() {
            VerifyAllExpectationsHaveBeenMet();
        }

        #endregion

 
        public object NewInstanceOfRole(Type mockedType, IMockDefinition definition) {
            return definition.Create(mockedType, this, mockObjectFactory);
        }

       
        public object NewInstanceOfRole(Type mockedType, params object[] constructorArgs) {
            return NewInstanceOfRole(mockedType, DefinedAs.WithArgs(constructorArgs));
        }

  
        public TMockedType NewInstanceOfRole<TMockedType>(IMockDefinition definition) {
            return (TMockedType) definition.Create(typeof (TMockedType), this, mockObjectFactory);
        }

        
        public TMockedType NewInstanceOfRole<TMockedType>(params object[] constructorArgs) {
            return NewInstanceOfRole<TMockedType>(DefinedAs.WithArgs(constructorArgs));
        }

       
        public object NewNamedInstanceOfRole(Type mockedType, string name, params object[] constructorArgs) {
            return NewInstanceOfRole(mockedType, DefinedAs.Named(name).WithArgs(constructorArgs));
        }

     
        public TMockedType NewNamedInstanceOfRole<TMockedType>(string name, params object[] constructorArgs) {
            return NewInstanceOfRole<TMockedType>(DefinedAs.Named(name).WithArgs(constructorArgs));
        }

    
        public void VerifyAllExpectationsHaveBeenMet() {
            // check for swallowed exception
            if (thrownUnexpectedInvocationException != null)
            {
                Exception exceptionToBeRethrown = thrownUnexpectedInvocationException;
                thrownUnexpectedInvocationException = null; // only rethrow once
                throw exceptionToBeRethrown;
            }

            if (!expectations.HasBeenMet)
            {
                FailUnmetExpectations();
            }
        }


        public void SetResolveTypeHandler(ResolveTypeDelegate resolveTypeHandler) {
            resolveTypeDelegate = resolveTypeHandler;
        }

        
        public void ClearExpectation(object mock) {
            IMockObject mockObject = CastToMockObject(mock);

            var result = new List<IExpectation>();
            expectations.QueryExpectationsBelongingTo(mockObject, result);

            result.ForEach(expectation => expectations.RemoveExpectation(expectation));
        }

        /// <summary>
        /// Adds the expectation.
        /// </summary>
        /// <param name="expectation">The expectation.</param>
        private void AddExpectation(IExpectation expectation) {
            topOrdering.AddExpectation(expectation);
        }

 
        internal object ResolveType(object mock, Type requestedType) {
            return resolveTypeDelegate != null ? resolveTypeDelegate(mock, requestedType) : Missing.Value;
        }

      
        private void Dispatch(Invocation invocation) {
            if (expectations.Matches(invocation))
            {
                expectations.Perform(invocation);
            }
            else
            {
                FailUnexpectedInvocation(invocation);
            }
        }

       
        private static IMockObject CastToMockObject(object mock) {
            if (mock == null)
            {
                throw new ArgumentNullException("mock", "mock must not be null");
            }

            var mockObject = mock as IMockObject;

            if (mockObject != null)
            {
                return mockObject;
            }

            throw new ArgumentException("argument must be a mock", "mock");
        }

  
        private void ClearExpectations() {
            depth = 1;
            expectations = new UnorderedExpectations();
            topOrdering = expectations;
        }

     
        private Popper Push(IExpectationOrdering newOrdering) {
            topOrdering.AddExpectation(newOrdering);
            IExpectationOrdering oldOrdering = topOrdering;
            topOrdering = newOrdering;
            depth++;
            return new Popper(this, oldOrdering);
        }


        private void Pop(IExpectationOrdering oldOrdering) {
            topOrdering = oldOrdering;
            depth--;
        }

     
        private void FailUnmetExpectations() {
            var writer = new DescriptionWriter();
            writer.WriteLine("not all expected invocations were performed");
            expectations.DescribeUnmetExpectationsTo(writer);
            ClearExpectations();

            throw new ExpectationException(writer.ToString());
        }


        private void FailUnexpectedInvocation(Invocation invocation) {
            var writer = new DescriptionWriter();
            writer.Write("unexpected invocation of ");
            invocation.DescribeTo(writer);

            writer.WriteLine();
            expectations.DescribeActiveExpectationsTo(writer);
            DescribeStatesOn(writer);
            // try catch to get exception with stack trace.
            try
            {
                throw new ExpectationException(writer.ToString());
            }
            catch (ExpectationException e)
            {
                // remember only first exception
                if (thrownUnexpectedInvocationException == null)
                {
                    thrownUnexpectedInvocationException = e;
                }

                throw;
            }
        }

        private void DescribeStatesOn(DescriptionWriter writer) {
            if (stateMachines.Any())
            {
                writer.Write("\nstates:\n");
                stateMachines.ForEach(stateMachine => stateMachine.DescribeTo(writer));
            }
        }

        public IStates States(string name) {
            var stateMachine = new StateMachine(name);
            stateMachines.Add(stateMachine);
            return stateMachine;
        }

        #region Nested type: Popper

        private class Popper : IDisposable {
           
            private readonly Mockery mockery;

     
            private readonly IExpectationOrdering previous;

           
            public Popper(Mockery mockery, IExpectationOrdering previous) {
                this.previous = previous;
                this.mockery = mockery;
            }

            #region IDisposable Members

        
            public void Dispose() {
                mockery.Pop(previous);
            }

            #endregion
        }

        #endregion

        void IExpectationCollector.Add(IExpectation expectation) {
            this.AddExpectation(expectation);
        }

        void IInvocationListener.NotifyInvocation(Invocation invocation) {
            Dispatch(invocation);
        }

        public void SetMockFactoryAs(IMockObjectFactory factory) {
            this.mockObjectFactory = factory;
        }
    }
}