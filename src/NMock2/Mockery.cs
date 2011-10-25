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

using System.Linq;
using NMock2.Syntax;

namespace NMock2
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Internal;
    using Monitoring;

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
    public class Mockery : IDisposable
    {
        /// <summary>
        /// In the rare case where the default mock object factory is replaced, we hold on to the
        /// previous factory (or factories) in case we need to switch back to them.
        /// </summary>
        private static readonly Dictionary<Type, IMockObjectFactory> availableMockObjectFactories = new Dictionary<Type, IMockObjectFactory>();

        /// <summary>
        /// The mock object factory that is being used by this Mockery instance.
        /// </summary>
        private readonly IMockObjectFactory currentMockObjectFactory;

        /// <summary>
        /// Holds all mapping from mocks/types to mock styles.
        /// </summary>
        private readonly StubMockStyleDictionary stubMockStyleDictionary = new StubMockStyleDictionary();

        /// <summary>
        /// The mock object factory that will be used when a new Mockery instance is created
        /// </summary>
        private static IMockObjectFactory defaultMockObjectFactory;

        /// <summary>
        /// Depth of cascaded ordered, unordered expectation blocks.
        /// </summary>
        private int depth;

        /// <summary>
        /// All expectations.
        /// </summary>
        private IExpectationOrdering expectations;

        /// <summary>
        /// Expectations at current cascade level.
        /// </summary>
        private IExpectationOrdering topOrdering;

        /// <summary>
        /// If an unexpected invocation exception is thrown then it is stored here to re-throw it in the 
        /// <see cref="VerifyAllExpectationsHaveBeenMet"/> method - exception cannot be swallowed by tested code.
        /// </summary>
        private ExpectationException thrownUnexpectedInvocationException;

        /// <summary>
        /// The delegate used to resolve the default type returned as return value in calls to mocks with stub behavior.
        /// </summary>
        private ResolveTypeDelegate resolveTypeDelegate;

        readonly List<IStates> stateMachines = new List<IStates>();

        /// <summary>
        /// Initializes static members of the <see cref="NMock2.Mockery"/> class.
        /// </summary>
        static Mockery()
        {
            ChangeDefaultMockObjectFactory(typeof(CastleMockObjectFactory));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NMock2.Mockery"/> class.
        /// Clears all expectations.
        /// </summary>
        public Mockery()
        {
            this.currentMockObjectFactory = defaultMockObjectFactory;
            
            this.ClearExpectations();
        }

        /// <summary>
        /// Gets a disposable object and tells the mockery that the following expectations are ordered, i.e. they have to be met in the specified order.
        /// Dispose the returned value to return to previous mode.
        /// </summary>
        /// <value>Disposable object. When this object is disposed then the ordered expectation mode is set back to the mode it was previously
        /// to call to <see cref="Ordered"/>.</value>
        public IDisposable Ordered
        {
            get { return this.Push(new OrderedExpectations(this.depth)); }
        }

        /// <summary>
        /// Gets a disposable object and tells the mockery that the following expectations are unordered, i.e. they can be met in any order.
        /// Dispose the returned value to return to previous mode.
        /// </summary>
        /// <value>Disposable object. When this object is disposed then the unordered expectation mode is set back to the mode it was previously
        /// to the call to <see cref="Unordered"/>.</value>
        public IDisposable Unordered
        {
            get { return this.Push(new UnorderedExpectations(this.depth)); }
        }

        /// <summary>
        /// Allows the default <see cref="IMockObjectFactory"/> to be replaced with a different implementation.
        /// </summary>
        /// <param name="factoryType">The System.Type of the <see cref="IMockObjectFactory"/> implementation to use.
        /// This is expected to implement <see cref="IMockObjectFactory"/> and have a default constructor.</param>
        public static void ChangeDefaultMockObjectFactory(Type factoryType)
        {
            if (!typeof(IMockObjectFactory).IsAssignableFrom(factoryType))
            {
                throw new ArgumentException("Supplied factory type does not implement IMockObjectFactory", "factoryType");
            }

            lock (availableMockObjectFactories)
            {
                if (!availableMockObjectFactories.TryGetValue(factoryType, out defaultMockObjectFactory))
                {
                    try
                    {
                        defaultMockObjectFactory = (IMockObjectFactory)Activator.CreateInstance(factoryType);
                        availableMockObjectFactories[factoryType] = defaultMockObjectFactory;
                    }
                    catch (MissingMethodException)
                    {
                        throw new ArgumentException("Supplied factory type does not have a default constructor", "factoryType");
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
        public object NewMock(Type mockedType, IMockDefinition definition)
        {
            return definition.Create(mockedType, this, this.currentMockObjectFactory);
        }

        /// <summary>
        /// Creates a new dynamic mock of the specified type.
        /// </summary>
        /// <param name="mockedType">The type to mock.</param>
        /// <param name="constructorArgs">The arguments for the constructor of the class to be mocked.
        /// Only applicable when mocking classes with non-default constructors.</param>
        /// <returns>A dynamic mock for the specified type.</returns>
        public object NewMock(Type mockedType, params object[] constructorArgs)
        {
            return this.NewMock(mockedType, DefinedAs.WithArgs(constructorArgs));
        }

        /// <summary>
        /// Creates a new dynamic mock of the specified type.
        /// </summary>
        /// <param name="mockedType">The type to mock.</param>
        /// <param name="mockStyle">Specifies how the mock object should behave when first created.</param>
        /// <param name="constructorArgs">The arguments for the constructor of the class to be mocked.
        /// Only applicable when mocking classes with non-default constructors.</param>
        /// <returns>A named dynamic mock for the specified type.</returns>
        public object NewMock(Type mockedType, MockStyle mockStyle, params object[] constructorArgs)
        {
            return this.NewMock(mockedType, DefinedAs.OfStyle(mockStyle).WithArgs(constructorArgs));
        }

        /// <summary>
        /// Creates a new dynamic mock of the specified type using the supplied definition.
        /// </summary>
        /// <typeparam name="TMockedType">The type to mock.</typeparam>
        /// <param name="definition">An <see cref="IMockDefinition"/> to create the mock from.</param>
        /// <returns>A dynamic mock for the specified type.</returns>
        public TMockedType NewMock<TMockedType>(IMockDefinition definition)
        {
            return (TMockedType)definition.Create(typeof(TMockedType), this, this.currentMockObjectFactory);
        }

        /// <summary>
        /// Creates a new dynamic mock of the specified type.
        /// </summary>
        /// <typeparam name="TMockedType">The type to mock.</typeparam>
        /// <param name="constructorArgs">The arguments for the constructor of the class to be mocked.
        /// Only applicable when mocking classes with non-default constructors.</param>
        /// <returns>A dynamic mock for the specified type.</returns>
        public TMockedType NewMock<TMockedType>(params object[] constructorArgs)
        {
            return this.NewMock<TMockedType>(DefinedAs.WithArgs(constructorArgs));
        }

        /// <summary>
        /// Creates a new dynamic mock of the specified type.
        /// </summary>
        /// <typeparam name="TMockedType">The type to mock.</typeparam>
        /// <param name="mockStyle">Specifies how the mock object should behave when first created.</param>
        /// <param name="constructorArgs">The arguments for the constructor of the class to be mocked.
        /// Only applicable when mocking classes with non-default constructors.</param>
        /// <returns>A dynamic mock for the specified type.</returns>
        public TMockedType NewMock<TMockedType>(MockStyle mockStyle, params object[] constructorArgs)
        {
            return this.NewMock<TMockedType>(DefinedAs.OfStyle(mockStyle).WithArgs(constructorArgs));
        }

        /// <summary>
        /// Creates a new named dynamic mock of the specified type.
        /// </summary>
        /// <param name="mockedType">The type to mock.</param>
        /// <param name="name">A name for the mock that will be used in error messages.</param>
        /// <param name="constructorArgs">The arguments for the constructor of the class to be mocked.
        /// Only applicable when mocking classes with non-default constructors.</param>
        /// <returns>A named mock.</returns>
        public object NewNamedMock(Type mockedType, string name, params object[] constructorArgs)
        {
            return this.NewMock(mockedType, DefinedAs.Named(name).WithArgs(constructorArgs));
        }

        /// <summary>
        /// Creates a new named dynamic mock of the specified type and allows the style
        /// of the mock to be specified.
        /// </summary>
        /// <param name="mockedType">The type to mock.</param>
        /// <param name="name">A name for the mock that will be used in error messages.</param>
        /// <param name="mockStyle">Specifies how the mock object should behave when first created.</param>
        /// <param name="constructorArgs">The arguments for the constructor of the class to be mocked.
        /// Only applicable when mocking classes with non-default constructors.</param>
        /// <returns>A named mock.</returns>
        public object NewNamedMock(Type mockedType, string name, MockStyle mockStyle, params object[] constructorArgs)
        {
            return this.NewMock(mockedType, DefinedAs.Named(name).OfStyle(mockStyle).WithArgs(constructorArgs));
        }

        /// <summary>
        /// Creates a new named dynamic mock of the specified type.
        /// </summary>
        /// <typeparam name="TMockedType">The type to mock.</typeparam>
        /// <param name="name">A name for the mock that will be used in error messages.</param>
        /// <param name="constructorArgs">The arguments for the constructor of the class to be mocked.
        /// Only applicable when mocking classes with non-default constructors.</param>
        /// <returns>A named mock.</returns>
        public TMockedType NewNamedMock<TMockedType>(string name, params object[] constructorArgs)
        {
            return this.NewMock<TMockedType>(DefinedAs.Named(name).WithArgs(constructorArgs));
        }

        /// <summary>
        /// Creates a new named dynamic mock of the specified type and allows the style
        /// of the mock to be specified.
        /// </summary>
        /// <typeparam name="TMockedType">The type to mock.</typeparam>
        /// <param name="name">A name for the mock that will be used in error messages.</param>
        /// <param name="mockStyle">Specifies how the mock object should behave when first created.</param>
        /// <param name="constructorArgs">The arguments for the constructor of the class to be mocked.
        /// Only applicable when mocking classes with non-default constructors.</param>
        /// <returns>A named mock.</returns>
        public TMockedType NewNamedMock<TMockedType>(string name, MockStyle mockStyle, params object[] constructorArgs)
        {
            return this.NewMock<TMockedType>(DefinedAs.Named(name).OfStyle(mockStyle).WithArgs(constructorArgs));
        }

        /// <summary>
        /// Verifies that all expectations have been met.
        /// Will be called in <see cref="Dispose"/>, too. 
        /// </summary>
        public void VerifyAllExpectationsHaveBeenMet()
        {
            // check for swallowed exception
            if (this.thrownUnexpectedInvocationException != null)
            {
                Exception exceptionToBeRethrown = this.thrownUnexpectedInvocationException;
                this.thrownUnexpectedInvocationException = null; // only rethrow once
                throw exceptionToBeRethrown;
            }

            if (!this.expectations.HasBeenMet)
            {
                this.FailUnmetExpectations();
            }
        }

        /// <summary>
        /// Disposes the mockery be verifying that all expectations were met.
        /// </summary>
        public void Dispose()
        {
            this.VerifyAllExpectationsHaveBeenMet();
        }

        /// <summary>
        /// Sets the resolve type handler used to override default values returned by stubs.
        /// </summary>
        /// <param name="resolveTypeHandler">The resolve type handler.</param>
        public void SetResolveTypeHandler(ResolveTypeDelegate resolveTypeHandler)
        {
            this.resolveTypeDelegate = resolveTypeHandler;
        }

        /// <summary>
        /// Sets the mock style used for all properties and methods returning a value of any type of the <paramref name="mock"/>.
        /// Can be overridden with a type specific mock style with <see cref="SetStubMockStyle{TStub}"/>.
        /// </summary>
        /// <param name="mock">The mock (with mock style Stub).</param>
        /// <param name="nestedMockStyle">The nested mock style.</param>
        public void SetStubMockStyle(object mock, MockStyle nestedMockStyle)
        {
            IMockObject mockObject = CastToMockObject(mock);
            this.stubMockStyleDictionary[mockObject] = nestedMockStyle;
        }

        /// <summary>
        /// Sets the mock style used for all properties and methods returning a value of type <typeparamref name="TStub"/>
        /// of the <paramref name="mock"/>.
        /// </summary>
        /// <typeparam name="TStub">The type of the stub.</typeparam>
        /// <param name="mock">The mock (with mock style Stub).</param>
        /// <param name="nestedMockStyle">The nested mock style.</param>
        public void SetStubMockStyle<TStub>(object mock, MockStyle nestedMockStyle)
        {
            this.SetStubMockStyle(mock, typeof(TStub), nestedMockStyle);
        }

        /// <summary>
        /// Sets the mock style used for all properties and methods returning a value of type <paramref name="nestedMockType"/>
        /// of the <paramref name="mock"/>.
        /// </summary>
        /// <param name="mock">The mock (with mock style Stub).</param>
        /// <param name="nestedMockType">Type of the nested mock.</param>
        /// <param name="nestedMockStyle">The nested mock style.</param>
        public void SetStubMockStyle(object mock, Type nestedMockType, MockStyle nestedMockStyle)
        {
            IMockObject mockObject = CastToMockObject(mock);
            this.stubMockStyleDictionary[mockObject, nestedMockType] = nestedMockStyle;
        }

        /// <summary>
        /// Clears all expectation on the specified mock.
        /// </summary>
        /// <param name="mock">The mock for which all expectations are cleared.</param>
        public void ClearExpectation(object mock)
        {
            IMockObject mockObject = CastToMockObject(mock);

            List<IExpectation> result = new List<IExpectation>();
            this.expectations.QueryExpectationsBelongingTo(mockObject, result);

            result.ForEach(expectation => this.expectations.RemoveExpectation(expectation));
        }

        /// <summary>
        /// Adds the expectation.
        /// </summary>
        /// <param name="expectation">The expectation.</param>
        internal void AddExpectation(IExpectation expectation)
        {
            this.topOrdering.AddExpectation(expectation);
        }

        /// <summary>
        /// Resolves the return value to be used in a call to a mock with stub behavior.
        /// </summary>
        /// <param name="mock">The mock on which the call is made.</param>
        /// <param name="requestedType">The type of the return value.</param>
        /// <returns>The object to be returned as return value; or <see cref="Missing.Value"/>
        /// if the default value should be used.</returns>
        internal object ResolveType(object mock, Type requestedType)
        {
            return this.resolveTypeDelegate != null ? this.resolveTypeDelegate(mock, requestedType) : Missing.Value;
        }

        /// <summary>
        /// Gets the mock style to be used for a mock created for a return value of a call to mock with stub behavior.
        /// </summary>
        /// <param name="mock">The mock that wants to create a mock.</param>
        /// <param name="requestedType">The type of the requested mock.</param>
        /// <returns>The mock style to use on the created mock. Null if <see cref="MockStyle.Default"/> has to be used.</returns>
        internal MockStyle? GetDependencyMockStyle(object mock, Type requestedType)
        {
            IMockObject mockObject = CastToMockObject(mock);
            return this.stubMockStyleDictionary[mockObject, requestedType];
        }
        
        /// <summary>
        /// Dispatches the specified invocation.
        /// </summary>
        /// <param name="invocation">The invocation.</param>
        internal void Dispatch(Invocation invocation)
        {
            if (this.expectations.Matches(invocation))
            {
                this.expectations.Perform(invocation);
            }
            else
            {
                this.FailUnexpectedInvocation(invocation);
            }
        }

        /// <summary>
        /// Determines whether there exist expectations for the specified invocation.
        /// </summary>
        /// <param name="invocation">The invocation.</param>
        /// <returns><c>true</c> if there exist expectations for the specified invocation; otherwise, <c>false</c>.
        /// </returns>
        internal bool HasExpectationFor(Invocation invocation)
        {
            return this.expectations.Matches(invocation);
        }

        internal bool HasExpectationForIgnoringIsActive(Invocation invocation)
        {
            return this.expectations.MatchesIgnoringIsActive(invocation);
        }

        /// <summary>
        /// Casts the argument to <see cref="IMockObject"/>.
        /// </summary>
        /// <param name="mock">The object to cast.</param>
        /// <returns>The argument casted to <see cref="IMockObject"/></returns>
        /// <throws cref="ArgumentNullException">Thrown if <paramref name="mock"/> is null</throws>
        /// <throws cref="ArgumentException">Thrown if <paramref name="mock"/> is not a <see cref="IMockObject"/></throws>
        private static IMockObject CastToMockObject(object mock)
        {
            if (mock == null)
            {
                throw new ArgumentNullException("mock", "mock must not be null");
            }

            IMockObject mockObject = mock as IMockObject;

            if (mockObject != null)
            {
                return mockObject;
            }

            throw new ArgumentException("argument must be a mock", "mock");
        }

        /// <summary>
        /// Clears the expectations.
        /// </summary>
        private void ClearExpectations()
        {
            this.depth = 1;
            this.expectations = new UnorderedExpectations();
            this.topOrdering = this.expectations;
        }

        /// <summary>
        /// Pushes the specified new ordering on the expectations stack.
        /// </summary>
        /// <param name="newOrdering">The new ordering.</param>
        /// <returns>Disposable popper.</returns>
        private Popper Push(IExpectationOrdering newOrdering)
        {
            this.topOrdering.AddExpectation(newOrdering);
            IExpectationOrdering oldOrdering = this.topOrdering;
            this.topOrdering = newOrdering;
            this.depth++;
            return new Popper(this, oldOrdering);
        }

        /// <summary>
        /// Pops the specified old ordering from the expectations stack.
        /// </summary>
        /// <param name="oldOrdering">The old ordering.</param>
        private void Pop(IExpectationOrdering oldOrdering)
        {
            this.topOrdering = oldOrdering;
            this.depth--;
        }

        /// <summary>
        /// Throws an exception listing all unmet expectations.
        /// </summary>
        private void FailUnmetExpectations()
        {
            DescriptionWriter writer = new DescriptionWriter();
            writer.WriteLine("not all expected invocations were performed");
            this.expectations.DescribeUnmetExpectationsTo(writer);
            this.ClearExpectations();

            throw new ExpectationException(writer.ToString());
        }

        /// <summary>
        /// Throws an exception indicating that the specified invocation is not expected.
        /// </summary>
        /// <param name="invocation">The invocation.</param>
        private void FailUnexpectedInvocation(Invocation invocation)
        {
            DescriptionWriter writer = new DescriptionWriter();
            writer.Write("unexpected invocation of ");
            invocation.DescribeTo(writer);
            if(stateMachines.Any())
            {
                writer.Write("\nstates:\n");
                stateMachines.ForEach(stateMachine => stateMachine.DescribeTo(writer));
            }
            writer.WriteLine();
            this.expectations.DescribeActiveExpectationsTo(writer);

            // try catch to get exception with stack trace.
            try
            {
                throw new ExpectationException(writer.ToString());
            }
            catch (ExpectationException e)
            {
                // remember only first exception
                if (this.thrownUnexpectedInvocationException == null)
                {
                    this.thrownUnexpectedInvocationException = e;
                }

                throw;
            }
        }

        /// <summary>
        /// A popper pops an expectation ordering from the expectations stack on disposal.
        /// </summary>
        private class Popper : IDisposable
        {
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
            public Popper(Mockery mockery, IExpectationOrdering previous)
            {
                this.previous = previous;
                this.mockery = mockery;
            }

            /// <summary>
            /// Pops the expectation ordering from the stack.
            /// </summary>
            public void Dispose()
            {
                this.mockery.Pop(this.previous);
            }
        }

        public IStates States(string name) {
            var stateMachine = new StateMachine(name);
            stateMachines.Add(stateMachine);
            return stateMachine;
        }
    }
}
