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


        private InvocationDispatcher dispatcher;

      
        private ResolveTypeDelegate resolveTypeDelegate;

  
        private ExpectationException thrownUnexpectedInvocationException;

        
      
     
        public Mockery() {
            mockObjectFactory = new CastleMockObjectFactory(this, this);

            ClearExpectations();
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

            if (!dispatcher.HasBeenMet)
            {
                FailUnmetExpectations();
            }
           
        }


        public void SetResolveTypeHandler(ResolveTypeDelegate resolveTypeHandler) {
            resolveTypeDelegate = resolveTypeHandler;
        }


        /// <summary>
        /// Adds the expectation.
        /// </summary>
        /// <param name="expectation">The expectation.</param>
        private void AddExpectation(IExpectation expectation) {
            dispatcher.Add(expectation);
        }

 
        internal object ResolveType(object mock, Type requestedType) {
            return resolveTypeDelegate != null ? resolveTypeDelegate(mock, requestedType) : Missing.Value;
        }

      
        private void Dispatch(Invocation invocation) {
            if (dispatcher.Matches(invocation))
            {
                dispatcher.Perform(invocation);
            }
            else
            {
                FailUnexpectedInvocation(invocation);
            }
        }


        private void ClearExpectations() {
            dispatcher = new InvocationDispatcher();
        }


        private void FailUnmetExpectations() {
            var writer = new StringDescriptionWriter();
            writer.AppendLine("not all expected invocations were performed");
            dispatcher.DescribeUnmetExpectationsTo(writer);
          

            throw new ExpectationException(writer.ToString());
        }


        private void FailUnexpectedInvocation(Invocation invocation) {
            var writer = new StringDescriptionWriter();
            writer.AppendText("unexpected invocation of ");
            invocation.DescribeOn(writer);

            writer.AppendNewLine();
            dispatcher.DescribeActiveExpectationsTo(writer);
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

        private void DescribeStatesOn(IDescription writer) {
            if (stateMachines.Any())
            {
                writer.AppendText("\nstates:\n");
                stateMachines.ForEach(stateMachine => stateMachine.DescribeOn(writer));
            }
        }

        public IStates States(string name) {
            var stateMachine = new StateMachine(name);
            stateMachines.Add(stateMachine);
            return stateMachine;
        }

        #region Nested type: Popper

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

        public ISequence Sequence(string name) {
            return new NamedSequence(name);
        }
    }
}