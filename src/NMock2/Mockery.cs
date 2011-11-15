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
using System.Reflection;
using System.Threading;
using NMocha.Internal;
using NMocha.Monitoring;
using NMock2;
using NMock2.Monitoring;

namespace NMocha {
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
    public class Mockery :  IDisposable, IInvocationListener {
        IMockObjectFactory mockObjectFactory;
        readonly InvocationDispatcher dispatcher;
        ResolveTypeDelegate resolveTypeDelegate;
        ExpectationException thrownUnexpectedInvocationException;
        private IThreadingPolicy threadingPolicy;

        public Mockery() {

            threadingPolicy = new SingleThreadPolicy();
            dispatcher = new InvocationDispatcher();
            mockObjectFactory = new CastleMockObjectFactory(dispatcher, this);
        }

        #region IDisposable Members

        
        public void Dispose() {
            VerifyAllExpectationsHaveBeenMet();
        }

        #endregion

 
        object NewInstanceOfRole(Type mockedType, IMockDefinition definition) {
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


        internal object ResolveType(object mock, Type requestedType) {
            return resolveTypeDelegate != null ? resolveTypeDelegate(mock, requestedType) : Missing.Value;
        }

      
        private void Dispatch(Invocation invocation) {
            try
            {
                dispatcher.Dispatch(invocation);
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


        private void FailUnmetExpectations() {
            var writer = new StringDescriptionWriter();
            writer.AppendLine("not all expected invocations were performed");
            dispatcher.DescribeUnmetExpectationsTo(writer);
            throw new ExpectationException(writer.ToString());
        }


       

       

        public IStates States(string name) {
            
            return dispatcher.NewStateMachine(name);
        }

       
        void IInvocationListener.NotifyInvocation(Invocation invocation) {
            threadingPolicy.SynchronizeAction(() => Dispatch(invocation));
           
        }

        public void SetMockFactoryAs(IMockObjectFactory factory) {
            this.mockObjectFactory = factory;
        }

        public ISequence Sequence(string name) {
            return new NamedSequence(name);
        }

        public void SetThreadingPolicy(IThreadingPolicy policy) {
            this.threadingPolicy = policy;

        }
    }

    public class SingleThreadPolicy : IThreadingPolicy {
        private Thread testThread;

        public SingleThreadPolicy() {
            testThread = Thread.CurrentThread;
        }

        public void SynchronizeAction(Action action) {
            if(testThread != Thread.CurrentThread)
                throw new ConcurrentModificationException();
            action();
        }
    }

    public interface IThreadingPolicy {
        void SynchronizeAction(Action action);
    }

    public class Synchronizer : IThreadingPolicy
    {
        public void SynchronizeAction(Action action) {
            
        }
    }

    public class ConcurrentModificationException : Exception
    {
    }

}