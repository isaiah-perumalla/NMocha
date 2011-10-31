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
        /// In the rare case where the default mock object factory is replaced, we hold on to the
        /// previous factory (or factories) in case we need to switch back to them.
        /// </summary>
        private static readonly Dictionary<Type, IMockObjectFactory> availableMockObjectFactories =
            new Dictionary<Type, IMockObjectFactory>();

        /// <summary>
        /// The mock object factory that will be used when a new Mockery instance is created
        /// </summary>
        private static IMockObjectFactory defaultMockObjectFactory;

        /// <summary>
        /// The mock object factory that is being used by this Mockery instance.
        /// </summary>
        private readonly IMockObjectFactory currentMockObjectFactory;

        private readonly List<IStates> stateMachines = new List<IStates>();

        /// <summary>
        /// Depth of cascaded ordered, unordered expectation blocks.
        /// </summary>
        private int depth;

        /// <summary>
        /// All expectations.
        /// </summary>
        private IExpectationOrdering expectations;

        /// <summary>
        /// The delegate used to resolve the default type returned as return value in calls to mocks with stub behavior.
        /// </summary>
        private ResolveTypeDelegate resolveTypeDelegate;

        /// <summary>
        /// If an unexpected invocation exception is thrown then it is stored here to re-throw it in the 
        /// <see cref="VerifyAllExpectationsHaveBeenMet"/> method - exception cannot be swallowed by tested code.
        /// </summary>
        private ExpectationException thrownUnexpectedInvocationException;

        /// <summary>
        /// Expectations at current cascade level.
        /// </summary>
        private IExpectationOrdering topOrdering;

        /// <summary>
        /// Initializes static members of the <see cref="NMock2.Mockery"/> class.
        /// </summary>
        static Mockery() {
            ChangeDefaultMockObjectFactory(typeof (CastleMockObjectFactory));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NMock2.Mockery"/> class.
        /// Clears all expectations.
        /// </summary>
        public Mockery() {
            currentMockObjectFactory = defaultMockObjectFactory;

            ClearExpectations();
        }

        /// <summary>
        /// Gets a disposable object and tells the mockery that the following expectations are ordered, i.e. they have to be met in the specified order.
        /// Dispose the returned value to return to previous mode.
        /// </summary>
        /// <value>Disposable object. When this object is disposed then the ordered expectation mode is set back to the mode it was previously
        /// to call to <see cref="Ordered"/>.</value>
        public IDisposable Ordered {
            get { return Push(new OrderedExpectations(depth)); }
        }

        /// <summary>
        /// Gets a disposable object and tells the mockery that the following expectations are unordered, i.e. they can be met in any order.
        /// Dispose the returned value to return to previous mode.
        /// </summary>
        /// <value>Disposable object. When this object is disposed then the unordered expectation mode is set back to the mode it was previously
        /// to the call to <see cref="Unordered"/>.</value>
        public IDisposable Unordered {
            get { return Push(new UnorderedExpectations(depth)); }
        }

        #region IDisposable Members

        /// <summary>
        /// Disposes the mockery be verifying that all expectations were met.
        /// </summary>
        public void Dispose() {
            VerifyAllExpectationsHaveBeenMet();
        }

        #endregion

        /// <summary>
        /// Allows the default <see cref="IMockObjectFactory"/> to be replaced with a different implementation.
        /// </summary>
        /// <param name="factoryType">The System.Type of the <see cref="IMockObjectFactory"/> implementation to use.
        /// This is expected to implement <see cref="IMockObjectFactory"/> and have a default constructor.</param>
        public static void ChangeDefaultMockObjectFactory(Type factoryType) {
            if (!typeof (IMockObjectFactory).IsAssignableFrom(factoryType))
            {
                throw new ArgumentException("Supplied factory type does not implement IMockObjectFactory", "factoryType");
            }

            lock (availableMockObjectFactories)
            {
                if (!availableMockObjectFactories.TryGetValue(factoryType, out defaultMockObjectFactory))
                {
                    try
                    {
                        defaultMockObjectFactory = (IMockObjectFactory) Activator.CreateInstance(factoryType);
                        availableMockObjectFactories[factoryType] = defaultMockObjectFactory;
                    }
                    catch (MissingMethodException)
                    {
                        throw new ArgumentException("Supplied factory type does not have a default constructor",
                                                    "factoryType");
                    }
                }
            }
        }

        /// <summary>
        /// Creates a new dynamic mock of the specified type using the supplied definition.
        /// </summary>
        /// <param name="mockedType">The type to mock.</param>
        /// <param name="definition">An <see cref="IMockDefinition"/> to create the mock from.</param>
        /// <returns>A dynamic mock for the specified type.</returns>
        public object NewInstanceOfRole(Type mockedType, IMockDefinition definition) {
            return definition.Create(mockedType, this, currentMockObjectFactory);
        }

        /// <summary>
        /// Creates a new dynamic mock of the specified type.
        /// </summary>
        /// <param name="mockedType">The type to mock.</param>
        /// <param name="constructorArgs">The arguments for the constructor of the class to be mocked.
        /// Only applicable when mocking classes with non-default constructors.</param>
        /// <returns>A dynamic mock for the specified type.</returns>
        public object NewInstanceOfRole(Type mockedType, params object[] constructorArgs) {
            return NewInstanceOfRole(mockedType, DefinedAs.WithArgs(constructorArgs));
        }

        /// <summary>
        /// Creates a new dynamic mock of the specified type using the supplied definition.
        /// </summary>
        /// <typeparam name="TMockedType">The type to mock.</typeparam>
        /// <param name="definition">An <see cref="IMockDefinition"/> to create the mock from.</param>
        /// <returns>A dynamic mock for the specified type.</returns>
        public TMockedType NewInstanceOfRole<TMockedType>(IMockDefinition definition) {
            return (TMockedType) definition.Create(typeof (TMockedType), this, currentMockObjectFactory);
        }

        /// <summary>
        /// Creates a new dynamic mock of the specified type.
        /// </summary>
        /// <typeparam name="TMockedType">The type to mock.</typeparam>
        /// <param name="constructorArgs">The arguments for the constructor of the class to be mocked.
        /// Only applicable when mocking classes with non-default constructors.</param>
        /// <returns>A dynamic mock for the specified type.</returns>
        public TMockedType NewInstanceOfRole<TMockedType>(params object[] constructorArgs) {
            return NewInstanceOfRole<TMockedType>(DefinedAs.WithArgs(constructorArgs));
        }

        /// <summary>
        /// Creates a new named dynamic mock of the specified type.
        /// </summary>
        /// <param name="mockedType">The type to mock.</param>
        /// <param name="name">A name for the mock that will be used in error messages.</param>
        /// <param name="constructorArgs">The arguments for the constructor of the class to be mocked.
        /// Only applicable when mocking classes with non-default constructors.</param>
        /// <returns>A named mock.</returns>
        public object NewNamedInstanceOfRole(Type mockedType, string name, params object[] constructorArgs) {
            return NewInstanceOfRole(mockedType, DefinedAs.Named(name).WithArgs(constructorArgs));
        }

        /// <summary>
        /// Creates a new named dynamic mock of the specified type.
        /// </summary>
        /// <typeparam name="TMockedType">The type to mock.</typeparam>
        /// <param name="name">A name for the mock that will be used in error messages.</param>
        /// <param name="constructorArgs">The arguments for the constructor of the class to be mocked.
        /// Only applicable when mocking classes with non-default constructors.</param>
        /// <returns>A named mock.</returns>
        public TMockedType NewNamedInstanceOfRole<TMockedType>(string name, params object[] constructorArgs) {
            return NewInstanceOfRole<TMockedType>(DefinedAs.Named(name).WithArgs(constructorArgs));
        }

        /// <summary>
        /// Verifies that all expectations have been met.
        /// Will be called in <see cref="Dispose"/>, too. 
        /// </summary>
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

        /// <summary>
        /// Sets the resolve type handler used to override default values returned by stubs.
        /// </summary>
        /// <param name="resolveTypeHandler">The resolve type handler.</param>
        public void SetResolveTypeHandler(ResolveTypeDelegate resolveTypeHandler) {
            resolveTypeDelegate = resolveTypeHandler;
        }

        /// <summary>
        /// Clears all expectation on the specified mock.
        /// </summary>
        /// <param name="mock">The mock for which all expectations are cleared.</param>
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

        /// <summary>
        /// Resolves the return value to be used in a call to a mock with stub behavior.
        /// </summary>
        /// <param name="mock">The mock on which the call is made.</param>
        /// <param name="requestedType">The type of the return value.</param>
        /// <returns>The object to be returned as return value; or <see cref="Missing.Value"/>
        /// if the default value should be used.</returns>
        internal object ResolveType(object mock, Type requestedType) {
            return resolveTypeDelegate != null ? resolveTypeDelegate(mock, requestedType) : Missing.Value;
        }

        /// <summary>
        /// Dispatches the specified invocation.
        /// </summary>
        /// <param name="invocation">The invocation.</param>
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

        /// <summary>
        /// Determines whether there exist expectations for the specified invocation.
        /// </summary>
        /// <param name="invocation">The invocation.</param>
        /// <returns><c>true</c> if there exist expectations for the specified invocation; otherwise, <c>false</c>.
        /// </returns>
        internal bool HasExpectationFor(Invocation invocation) {
            return expectations.Matches(invocation);
        }

        internal bool HasExpectationForIgnoringIsActive(Invocation invocation) {
            return expectations.MatchesIgnoringIsActive(invocation);
        }

        /// <summary>
        /// Casts the argument to <see cref="IMockObject"/>.
        /// </summary>
        /// <param name="mock">The object to cast.</param>
        /// <returns>The argument casted to <see cref="IMockObject"/></returns>
        /// <throws cref="ArgumentNullException">Thrown if <paramref name="mock"/> is null</throws>
        /// <throws cref="ArgumentException">Thrown if <paramref name="mock"/> is not a <see cref="IMockObject"/></throws>
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

        /// <summary>
        /// Clears the expectations.
        /// </summary>
        private void ClearExpectations() {
            depth = 1;
            expectations = new UnorderedExpectations();
            topOrdering = expectations;
        }

        /// <summary>
        /// Pushes the specified new ordering on the expectations stack.
        /// </summary>
        /// <param name="newOrdering">The new ordering.</param>
        /// <returns>Disposable popper.</returns>
        private Popper Push(IExpectationOrdering newOrdering) {
            topOrdering.AddExpectation(newOrdering);
            IExpectationOrdering oldOrdering = topOrdering;
            topOrdering = newOrdering;
            depth++;
            return new Popper(this, oldOrdering);
        }

        /// <summary>
        /// Pops the specified old ordering from the expectations stack.
        /// </summary>
        /// <param name="oldOrdering">The old ordering.</param>
        private void Pop(IExpectationOrdering oldOrdering) {
            topOrdering = oldOrdering;
            depth--;
        }

        /// <summary>
        /// Throws an exception listing all unmet expectations.
        /// </summary>
        private void FailUnmetExpectations() {
            var writer = new DescriptionWriter();
            writer.WriteLine("not all expected invocations were performed");
            expectations.DescribeUnmetExpectationsTo(writer);
            ClearExpectations();

            throw new ExpectationException(writer.ToString());
        }

        /// <summary>
        /// Throws an exception indicating that the specified invocation is not expected.
        /// </summary>
        /// <param name="invocation">The invocation.</param>
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

        /// <summary>
        /// A popper pops an expectation ordering from the expectations stack on disposal.
        /// </summary>
        private class Popper : IDisposable {
            /// <summary>
            /// The mockery.
            /// </summary>
            private readonly Mockery mockery;

            /// <summary>
            /// The previous expectation ordering.
            /// </summary>
            private readonly IExpectationOrdering previous;

            /// <summary>
            /// Initializes a new instance of the <see cref="Popper"/> class.
            /// </summary>
            /// <param name="mockery">The mockery.</param>
            /// <param name="previous">The previous.</param>
            public Popper(Mockery mockery, IExpectationOrdering previous) {
                this.previous = previous;
                this.mockery = mockery;
            }

            #region IDisposable Members

            /// <summary>
            /// Pops the expectation ordering from the stack.
            /// </summary>
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
    }
}