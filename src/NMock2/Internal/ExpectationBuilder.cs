//-----------------------------------------------------------------------
// <copyright file="ExpectationBuilder.cs" company="NMock2">
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
using NMock2.Matchers;
using NMock2.Syntax;

namespace NMock2.Internal {
    public class ExpectationBuilder :
        IReceiverSyntax, IMethodSyntax, IArgumentSyntax {
        private readonly BuildableExpectation expectation;
        private IMockObject mockObject;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpectationBuilder"/> class.
        /// </summary>
        /// <param name="description">The description.</param>
        /// <param name="requiredCountMatcher">The required count matcher.</param>
        /// <param name="acceptedCountMatcher">The accepted count matcher.</param>
        public ExpectationBuilder(string description, Matcher requiredCountMatcher, Matcher acceptedCountMatcher) {
            expectation = new BuildableExpectation(description, requiredCountMatcher, acceptedCountMatcher);
        }

        #region IArgumentSyntax Members

        /// <summary>
        /// Defines the arguments that are expected on the method call.
        /// </summary>
        /// <param name="expectedArguments">The expected arguments.</param>
        /// <returns>Matcher syntax.</returns>
        public IMatchSyntax With(params object[] expectedArguments) {
            expectation.ArgumentsMatcher = new ArgumentsMatcher(ArgumentMatchers(expectedArguments));
            return this;
        }

        /// <summary>
        /// Defines that no arguments are expected on the method call.
        /// </summary>
        /// <returns>Matcher syntax.</returns>
        public IMatchSyntax WithNoArguments() {
            return With(new Matcher[0]);
        }

        /// <summary>
        /// Defines that all arguments are allowed on the method call.
        /// </summary>
        /// <returns>Matcher syntax.</returns>
        public IMatchSyntax WithAnyArguments() {
            expectation.ArgumentsMatcher = new AlwaysMatcher(true, "(any arguments)");
            return this;
        }


        public IStateSyntax When(IStatePredicate predicate) {
            expectation.AddOrderingConstraint(new InStateOrderingConstraint(predicate));
            return this;
        }

        public IStateSyntax Then(State state) {
            expectation.AddSideEffect(new ChangeStateEffect(state));
            return this;
        }

        /// <summary>
        /// Defines a matching criteria.
        /// </summary>
        /// <param name="matcher">The matcher.</param>
        /// <returns>
        /// Action syntax defining the action to take.
        /// </returns>
        public IActionSyntax Matching(Matcher matcher) {
            expectation.AddInvocationMatcher(matcher);
            return this;
        }

        /// <summary>
        /// Defines what will happen.
        /// </summary>
        /// <param name="actions">The actions to take.</param>
        /// <returns>
        /// Returns the comment syntax defined after will.
        /// </returns>
        public IStateSyntax Will(params IAction[] actions) {
            foreach (IAction action in actions)
            {
                expectation.AddAction(action);
            }

            return this;
        }

        /// <summary>
        /// Adds a comment for the expectation that is added to the error message if the expectation is not met.
        /// </summary>
        /// <param name="comment">The comment that is shown in the error message if this expectation is not met.
        /// You can describe here why this expectation has to be met.</param>
        public void Comment(string comment) {
            expectation.AddComment(comment);
        }

        #endregion

        #region IMethodSyntax Members

        /// <summary>
        /// Gets an indexer (get operation).
        /// </summary>
        /// <value>Get indexer syntax defining the value returned by the indexer.</value>
        public IGetIndexerSyntax Get {
            get {
                Matcher methodMatcher = NewMethodNameMatcher(string.Empty, "get_Item");
                EnsureMatchingMethodExistsOnMock(methodMatcher, "an indexed getter");

                expectation.DescribeAsIndexer();
                expectation.MethodMatcher = methodMatcher;
                return new IndexGetterBuilder(expectation, this);
            }
        }

        /// <summary>
        /// Gets an indexer (set operation).
        /// </summary>
        /// <value>Set indexer syntax defining the value the indexer is set to.</value>
        public ISetIndexerSyntax Set {
            get {
                Matcher methodMatcher = NewMethodNameMatcher(string.Empty, "set_Item");
                EnsureMatchingMethodExistsOnMock(methodMatcher, "an indexed getter");

                expectation.DescribeAsIndexer();
                expectation.MethodMatcher = methodMatcher;
                return new IndexSetterBuilder(expectation, this);
            }
        }

        /// <summary>
        /// Methods the specified method name.
        /// </summary>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="typeParams">The type params.</param>
        /// <returns></returns>
        public IArgumentSyntax Message(string methodName, params Type[] typeParams) {
            return Message(new MethodNameMatcher(methodName), typeParams);
        }

        /// <summary>
        /// Defines a method.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <param name="typeParams">The generic type params to match.</param>
        /// <returns>
        /// Argument syntax defining the arguments of the method.
        /// </returns>
        public IArgumentSyntax Message(MethodInfo method, params Type[] typeParams) {
            return Message(new DescriptionOverride(method.Name, Is.Same(method)), typeParams);
        }


        /// <summary>
        /// Methods the specified method matcher.
        /// </summary>
        /// <param name="methodMatcher">The method matcher.</param>
        /// <param name="typeParams">The type params.</param>
        /// <returns></returns>
        public IArgumentSyntax Message(Matcher methodMatcher, params Type[] typeParams) {
            if (typeParams != null && typeParams.Length > 0)
            {
                var typeMatchers = new List<Matcher>();
                foreach (Type type in typeParams)
                {
                    typeMatchers.Add(new DescriptionOverride(type.FullName, new SameMatcher(type)));
                }

                return Message(
                    methodMatcher, new GenericMethodTypeParametersMatcher(typeMatchers.ToArray()));
            }
            else
            {
                return Message(methodMatcher, new AlwaysMatcher(true, string.Empty));
            }
        }

        /// <summary>
        /// Defines a method.
        /// </summary>
        /// <param name="methodMatcher">Matcher for matching the method on an invocation.</param>
        /// <param name="typeParamsMatcher">Matchers for matching type parameters.</param>
        /// <returns>
        /// Argument syntax defining the arguments of the method.
        /// </returns>
        public IArgumentSyntax Message(Matcher methodMatcher, Matcher typeParamsMatcher) {
            EnsureMatchingMethodExistsOnMock(methodMatcher, "a method matching " + methodMatcher);

            expectation.MethodMatcher = methodMatcher;
            expectation.GenericMethodTypeMatcher = typeParamsMatcher;

            return this;
        }

        /// <summary>
        /// Gets the property.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        public IMatchSyntax GetProperty(string propertyName) {
            Matcher methodMatcher = NewMethodNameMatcher(propertyName, "get_" + propertyName);

            EnsureMatchingMethodExistsOnMock(methodMatcher, "a getter for property " + propertyName);

            expectation.MethodMatcher = methodMatcher;
            expectation.ArgumentsMatcher = new DescriptionOverride(string.Empty, new ArgumentsMatcher());

            return this;
        }

        /// <summary>
        /// Sets the property.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        public IValueSyntax SetProperty(string propertyName) {
            Matcher methodMatcher = NewMethodNameMatcher(propertyName + " = ", "set_" + propertyName);

            EnsureMatchingMethodExistsOnMock(methodMatcher, "a setter for property " + propertyName);

            expectation.MethodMatcher = methodMatcher;
            return new PropertyValueBuilder(this);
        }

        #endregion

        #region IReceiverSyntax Members

        /// <summary>
        /// Defines the receiver.
        /// </summary>
        /// <param name="receiver">The dynamic mock on which the expectation or stub is applied.</param>
        /// <returns>Method syntax defining the method, property or event.</returns>
        public IMethodSyntax On(object receiver) {
            if (receiver is IMockObject)
            {
                mockObject = (IMockObject) receiver;

                expectation.Receiver = mockObject;
                mockObject.AddExpectation(expectation);
            }
            else
            {
                throw new ArgumentException("not a mock object", "receiver");
            }

            return this;
        }

        #endregion

        /// <summary>
        /// Arguments the matchers.
        /// </summary>
        /// <param name="expectedArguments">The expected arguments.</param>
        /// <returns></returns>
        private static Matcher[] ArgumentMatchers(object[] expectedArguments) {
            var matchers = new Matcher[expectedArguments.Length];
            for (int i = 0; i < matchers.Length; i++)
            {
                object o = expectedArguments[i];
                matchers[i] = (o is Matcher) ? (Matcher) o : new EqualMatcher(o);
            }

            return matchers;
        }

        /// <summary>
        /// News the method name matcher.
        /// </summary>
        /// <param name="description">The description.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <returns></returns>
        private static Matcher NewMethodNameMatcher(string description, string methodName) {
            return new DescriptionOverride(description, new MethodNameMatcher(methodName));
        }

        /// <summary>
        /// Ensures the matching method exists on mock.
        /// </summary>
        /// <param name="methodMatcher">The method matcher.</param>
        /// <param name="methodDescription">The method description.</param>
        private void EnsureMatchingMethodExistsOnMock(Matcher methodMatcher, string methodDescription) {
            IList<MethodInfo> matches = mockObject.GetMethodsMatching(methodMatcher);

            if (matches.Count == 0)
            {
                throw new ArgumentException("mock object " + mockObject.MockName + " does not have " + methodDescription);
            }

            foreach (MethodInfo methodInfo in matches)
            {
                // Note that methods on classes that are implementations of an interface
                // method are considered virtual regardless of whether they are actually
                // marked as virtual or not. Hence the additional call to IsFinal.
                if ((methodInfo.IsVirtual || methodInfo.IsAbstract) && !methodInfo.IsFinal)
                {
                    return;
                }
            }

            throw new ArgumentException("mock object " + mockObject.MockName + " has " + methodDescription +
                                        ", but it is not virtual or abstract");
        }

        #region Nested type: IndexGetterBuilder

        private class IndexGetterBuilder : IGetIndexerSyntax {
            /// <summary>
            /// Holds the instance to the <see cref="ExpectationBuilder"/>.
            /// </summary>
            private readonly ExpectationBuilder builder;

            private readonly BuildableExpectation expectation;

            /// <summary>
            /// Initializes a new instance of the <see cref="IndexGetterBuilder"/> class.
            /// </summary>
            /// <param name="expectation">The expectation.</param>
            /// <param name="builder">The builder.</param>
            public IndexGetterBuilder(BuildableExpectation expectation, ExpectationBuilder builder) {
                this.expectation = expectation;
                this.builder = builder;
            }

            #region IGetIndexerSyntax Members

            public IMatchSyntax this[params object[] expectedArguments] {
                get {
                    expectation.ArgumentsMatcher = new IndexGetterArgumentsMatcher(ArgumentMatchers(expectedArguments));
                    return builder;
                }
            }

            #endregion
        }

        #endregion

        #region Nested type: IndexSetterBuilder

        private class IndexSetterBuilder : ISetIndexerSyntax, IValueSyntax {
            private readonly ExpectationBuilder builder;
            private readonly BuildableExpectation expectation;
            private Matcher[] matchers;

            /// <summary>
            /// Initializes a new instance of the <see cref="IndexSetterBuilder"/> class.
            /// </summary>
            /// <param name="expectation">The expectation.</param>
            /// <param name="builder">The builder.</param>
            public IndexSetterBuilder(BuildableExpectation expectation, ExpectationBuilder builder) {
                this.expectation = expectation;
                this.builder = builder;
            }

            #region ISetIndexerSyntax Members

            public IValueSyntax this[params object[] expectedArguments] {
                get {
                    Matcher[] indexMatchers = ArgumentMatchers(expectedArguments);
                    matchers = new Matcher[indexMatchers.Length + 1];
                    Array.Copy(indexMatchers, matchers, indexMatchers.Length);
                    SetValueMatcher(Is.Anything);
                    return this;
                }
            }

            #endregion

            #region IValueSyntax Members

            public IMatchSyntax To(Matcher matcher) {
                SetValueMatcher(matcher);
                return builder;
            }

            public IMatchSyntax To(object equalValue) {
                return To(Is.EqualTo(equalValue));
            }

            #endregion

            private void SetValueMatcher(Matcher matcher) {
                matchers[matchers.Length - 1] = matcher;
                expectation.ArgumentsMatcher = new IndexSetterArgumentsMatcher(matchers);
            }
        }

        #endregion

        #region Nested type: PropertyValueBuilder

        private class PropertyValueBuilder : IValueSyntax {
            private readonly ExpectationBuilder builder;

            /// <summary>
            /// Initializes a new instance of the <see cref="PropertyValueBuilder"/> class.
            /// </summary>
            /// <param name="builder">The builder.</param>
            public PropertyValueBuilder(ExpectationBuilder builder) {
                this.builder = builder;
            }

            #region IValueSyntax Members

            public IMatchSyntax To(Matcher valueMatcher) {
                return builder.With(valueMatcher);
            }

            public IMatchSyntax To(object equalValue) {
                return To(Is.EqualTo(equalValue));
            }

            #endregion
        }

        #endregion
        }
}