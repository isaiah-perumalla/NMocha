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
namespace NMock2.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Matchers;
    using Syntax;

    public class ExpectationBuilder : 
        IReceiverSyntax, IMethodSyntax, IArgumentSyntax, IStateSyntax {
        private BuildableExpectation expectation;
        private IMockObject mockObject;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpectationBuilder"/> class.
        /// </summary>
        /// <param name="description">The description.</param>
        /// <param name="requiredCountMatcher">The required count matcher.</param>
        /// <param name="acceptedCountMatcher">The accepted count matcher.</param>
        public ExpectationBuilder(string description, Matcher requiredCountMatcher, Matcher acceptedCountMatcher)
        {
            this.expectation = new BuildableExpectation(description, requiredCountMatcher, acceptedCountMatcher);
        }

        /// <summary>
        /// Gets an indexer (get operation).
        /// </summary>
        /// <value>Get indexer syntax defining the value returned by the indexer.</value>
        public IGetIndexerSyntax Get
        {
            get
            {
                Matcher methodMatcher = NewMethodNameMatcher(string.Empty, "get_Item");
                this.EnsureMatchingMethodExistsOnMock(methodMatcher, "an indexed getter");

                this.expectation.DescribeAsIndexer();
                this.expectation.MethodMatcher = methodMatcher;
                return new IndexGetterBuilder(this.expectation, this);
            }
        }

        /// <summary>
        /// Gets an indexer (set operation).
        /// </summary>
        /// <value>Set indexer syntax defining the value the indexer is set to.</value>
        public ISetIndexerSyntax Set
        {
            get
            {
                Matcher methodMatcher = NewMethodNameMatcher(string.Empty, "set_Item");
                this.EnsureMatchingMethodExistsOnMock(methodMatcher, "an indexed getter");

                this.expectation.DescribeAsIndexer();
                this.expectation.MethodMatcher = methodMatcher;
                return new IndexSetterBuilder(this.expectation, this);
            }
        }

        /// <summary>
        /// Defines the receiver.
        /// </summary>
        /// <param name="receiver">The dynamic mock on which the expectation or stub is applied.</param>
        /// <returns>Method syntax defining the method, property or event.</returns>
        public IMethodSyntax On(object receiver)
        {
            if (receiver is IMockObject)
            {
                this.mockObject = (IMockObject)receiver;

                this.expectation.Receiver = this.mockObject;
                this.mockObject.AddExpectation(this.expectation);
            }
            else
            {
                throw new ArgumentException("not a mock object", "receiver");
            }
            
            return this;
        }

        /// <summary>
        /// Methods the specified method name.
        /// </summary>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="typeParams">The type params.</param>
        /// <returns></returns>
        public IArgumentSyntax Method(string methodName, params Type[] typeParams)
        {
            return this.Method(new MethodNameMatcher(methodName), typeParams);
        }

        /// <summary>
        /// Defines a method.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <param name="typeParams">The generic type params to match.</param>
        /// <returns>
        /// Argument syntax defining the arguments of the method.
        /// </returns>
        public IArgumentSyntax Method(MethodInfo method, params Type[] typeParams)
        {
            return this.Method(new DescriptionOverride(method.Name, Is.Same(method)), typeParams);
        }

        /// <summary>
        /// Methods the specified method matcher.
        /// </summary>
        /// <param name="methodMatcher">The method matcher.</param>
        /// <param name="typeParams">The type params.</param>
        /// <returns></returns>
        public IArgumentSyntax Method(Matcher methodMatcher, params Type[] typeParams)
        {
            if (typeParams != null && typeParams.Length > 0)
            {
                List<Matcher> typeMatchers = new List<Matcher>();
                foreach (Type type in typeParams)
                {
                    typeMatchers.Add(new DescriptionOverride(type.FullName, new SameMatcher(type)));
                }

                return this.Method(
                    methodMatcher, new GenericMethodTypeParametersMatcher(typeMatchers.ToArray()));
            }
            else
            {
                return this.Method(methodMatcher, new AlwaysMatcher(true, string.Empty)); 
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
        public IArgumentSyntax Method(Matcher methodMatcher, Matcher typeParamsMatcher)
        {
            this.EnsureMatchingMethodExistsOnMock(methodMatcher, "a method matching " + methodMatcher);

            this.expectation.MethodMatcher = methodMatcher;
            this.expectation.GenericMethodTypeMatcher = typeParamsMatcher;
            
            return this;
        }

        /// <summary>
        /// Gets the property.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        public IMatchSyntax GetProperty(string propertyName)
        {
            Matcher methodMatcher = NewMethodNameMatcher(propertyName, "get_" + propertyName);

            this.EnsureMatchingMethodExistsOnMock(methodMatcher, "a getter for property " + propertyName);

            this.expectation.MethodMatcher = methodMatcher;
            this.expectation.ArgumentsMatcher = new DescriptionOverride(string.Empty, new ArgumentsMatcher());
            
            return this;
        }

        /// <summary>
        /// Sets the property.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        public IValueSyntax SetProperty(string propertyName)
        {
            Matcher methodMatcher = NewMethodNameMatcher(propertyName + " = ", "set_" + propertyName);

            this.EnsureMatchingMethodExistsOnMock(methodMatcher, "a setter for property " + propertyName);

            this.expectation.MethodMatcher = methodMatcher;
            return new PropertyValueBuilder(this);
        }

        /// <summary>
        /// Defines an event registration.
        /// </summary>
        /// <param name="eventName">Name of the event.</param>
        /// <returns>
        /// Match syntax defining the behavior of the event adder.
        /// </returns>
        public IMatchSyntax EventAdd(string eventName)
        {
            return this.EventAdd(eventName, Is.Anything);
        }

        /// <summary>
        /// Defines an event registration.
        /// </summary>
        /// <param name="eventName">Name of the event.</param>
        /// <param name="listenerMatcher">The listener matcher.</param>
        /// <returns>
        /// Match syntax defining the behavior of the event adder.
        /// </returns>
        public IMatchSyntax EventAdd(string eventName, Matcher listenerMatcher)
        {
            Matcher methodMatcher = NewMethodNameMatcher(eventName + " += ", "add_" + eventName);
            
            this.EnsureMatchingMethodExistsOnMock(methodMatcher, "an event named " + eventName);

            this.expectation.MethodMatcher = methodMatcher;
            this.expectation.ArgumentsMatcher = new ArgumentsMatcher(listenerMatcher);
            return this;
        }

        /// <summary>
        /// Defines an event registration.
        /// </summary>
        /// <param name="eventName">Name of the event.</param>
        /// <param name="equalListener">Delegate defining compatible listeners.</param>
        /// <returns>
        /// Match syntax defining the behavior of the event adder.
        /// </returns>
        public IMatchSyntax EventAdd(string eventName, Delegate equalListener)
        {
            return this.EventAdd(eventName, Is.EqualTo(equalListener));
        }

        /// <summary>
        /// Defines an event deregistration.
        /// </summary>
        /// <param name="eventName">Name of the event.</param>
        /// <param name="listenerMatcher">The listener matcher.</param>
        /// <returns>
        /// Match syntax defining the behavior of the event remover.
        /// </returns>
        public IMatchSyntax EventRemove(string eventName, Matcher listenerMatcher)
        {
            Matcher methodMatcher = NewMethodNameMatcher(eventName + " -= ", "remove_" + eventName);
            
            this.EnsureMatchingMethodExistsOnMock(methodMatcher, "an event named " + eventName);

            this.expectation.MethodMatcher = methodMatcher;
            this.expectation.ArgumentsMatcher = new ArgumentsMatcher(listenerMatcher);
            return this;
        }

        /// <summary>
        /// Defines an event deregistration.
        /// </summary>
        /// <param name="eventName">Name of the event.</param>
        /// <returns>
        /// Match syntax defining the behavior of the event remover.
        /// </returns>
        public IMatchSyntax EventRemove(string eventName)
        {
            return this.EventRemove(eventName, Is.Anything);
        }

        /// <summary>
        /// Defines an event deregistration.
        /// </summary>
        /// <param name="eventName">Name of the event.</param>
        /// <param name="equalListener">Delegate defining compatible listeners.</param>
        /// <returns>
        /// Match syntax defining the behavior of the event remover.
        /// </returns>
        public IMatchSyntax EventRemove(string eventName, Delegate equalListener)
        {
            return this.EventRemove(eventName, Is.EqualTo(equalListener));
        }

        /// <summary>
        /// Defines the arguments that are expected on the method call.
        /// </summary>
        /// <param name="expectedArguments">The expected arguments.</param>
        /// <returns>Matcher syntax.</returns>
        public IMatchSyntax With(params object[] expectedArguments)
        {
            this.expectation.ArgumentsMatcher = new ArgumentsMatcher(ArgumentMatchers(expectedArguments));
            return this;
        }

        /// <summary>
        /// Defines that no arguments are expected on the method call.
        /// </summary>
        /// <returns>Matcher syntax.</returns>
        public IMatchSyntax WithNoArguments()
        {
            return this.With(new Matcher[0]);
        }

        /// <summary>
        /// Defines that all arguments are allowed on the method call.
        /// </summary>
        /// <returns>Matcher syntax.</returns>
        public IMatchSyntax WithAnyArguments()
        {
            this.expectation.ArgumentsMatcher = new AlwaysMatcher(true, "(any arguments)");
            return this;
        }

       
        public void When(IStatePredicate predicate) {
            this.expectation.AddOrderingConstraint(new InStateOrderingConstraint(predicate));
        }

        public void Then(State state) {
            this.expectation.AddSideEffect(state.Activate);

        }

        /// <summary>
        /// Defines a matching criteria.
        /// </summary>
        /// <param name="matcher">The matcher.</param>
        /// <returns>
        /// Action syntax defining the action to take.
        /// </returns>
        public IActionSyntax Matching(Matcher matcher)
        {
            this.expectation.AddInvocationMatcher(matcher);
            return this;
        }

        /// <summary>
        /// Defines what will happen.
        /// </summary>
        /// <param name="actions">The actions to take.</param>
        /// <returns>
        /// Returns the comment syntax defined after will.
        /// </returns>
        public ICommentSyntax Will(params IAction[] actions)
        {
            foreach (IAction action in actions)
            {
                this.expectation.AddAction(action);
            }

            return this;
        }

        /// <summary>
        /// Adds a comment for the expectation that is added to the error message if the expectation is not met.
        /// </summary>
        /// <param name="comment">The comment that is shown in the error message if this expectation is not met.
        /// You can describe here why this expectation has to be met.</param>
        public void Comment(string comment)
        {
            this.expectation.AddComment(comment);
        }

        /// <summary>
        /// Arguments the matchers.
        /// </summary>
        /// <param name="expectedArguments">The expected arguments.</param>
        /// <returns></returns>
        private static Matcher[] ArgumentMatchers(object[] expectedArguments)
        {
            Matcher[] matchers = new Matcher[expectedArguments.Length];
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
        private static Matcher NewMethodNameMatcher(string description, string methodName)
        {
            return new DescriptionOverride(description, new MethodNameMatcher(methodName));
        }

        /// <summary>
        /// Ensures the matching method exists on mock.
        /// </summary>
        /// <param name="methodMatcher">The method matcher.</param>
        /// <param name="methodDescription">The method description.</param>
        private void EnsureMatchingMethodExistsOnMock(Matcher methodMatcher, string methodDescription)
        {
            IList<MethodInfo> matches = this.mockObject.GetMethodsMatching(methodMatcher);

            if (matches.Count == 0)
            {
                throw new ArgumentException("mock object " + this.mockObject.MockName + " does not have " + methodDescription);
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

            throw new ArgumentException("mock object " + this.mockObject.MockName + " has " + methodDescription + ", but it is not virtual or abstract");
        }

        private class PropertyValueBuilder : IValueSyntax
        {
            private readonly ExpectationBuilder builder;

            /// <summary>
            /// Initializes a new instance of the <see cref="PropertyValueBuilder"/> class.
            /// </summary>
            /// <param name="builder">The builder.</param>
            public PropertyValueBuilder(ExpectationBuilder builder)
            {
                this.builder = builder;
            }

            public IMatchSyntax To(Matcher valueMatcher)
            {
                return this.builder.With(valueMatcher);
            }

            public IMatchSyntax To(object equalValue)
            {
                return this.To(Is.EqualTo(equalValue));
            }
        }

        private class IndexGetterBuilder : IGetIndexerSyntax
        {
            private readonly BuildableExpectation expectation;

            /// <summary>
            /// Holds the instance to the <see cref="ExpectationBuilder"/>.
            /// </summary>
            private readonly ExpectationBuilder builder;

            /// <summary>
            /// Initializes a new instance of the <see cref="IndexGetterBuilder"/> class.
            /// </summary>
            /// <param name="expectation">The expectation.</param>
            /// <param name="builder">The builder.</param>
            public IndexGetterBuilder(BuildableExpectation expectation, ExpectationBuilder builder)
            {
                this.expectation = expectation;
                this.builder = builder;
            }

            public IMatchSyntax this[params object[] expectedArguments]
            {
                get
                {
                    this.expectation.ArgumentsMatcher = new IndexGetterArgumentsMatcher(ArgumentMatchers(expectedArguments));
                    return this.builder;
                }
            }
        }

        private class IndexSetterBuilder : ISetIndexerSyntax, IValueSyntax
        {
            private readonly BuildableExpectation expectation;
            private readonly ExpectationBuilder builder;
            private Matcher[] matchers;

            /// <summary>
            /// Initializes a new instance of the <see cref="IndexSetterBuilder"/> class.
            /// </summary>
            /// <param name="expectation">The expectation.</param>
            /// <param name="builder">The builder.</param>
            public IndexSetterBuilder(BuildableExpectation expectation, ExpectationBuilder builder)
            {
                this.expectation = expectation;
                this.builder = builder;
            }

            public IValueSyntax this[params object[] expectedArguments]
            {
                get
                {
                    Matcher[] indexMatchers = ArgumentMatchers(expectedArguments);
                    this.matchers = new Matcher[indexMatchers.Length + 1];
                    Array.Copy(indexMatchers, this.matchers, indexMatchers.Length);
                    this.SetValueMatcher(Is.Anything);
                    return this;
                }
            }

            public IMatchSyntax To(Matcher matcher)
            {
                this.SetValueMatcher(matcher);
                return this.builder;
            }

            public IMatchSyntax To(object equalValue)
            {
                return this.To(Is.EqualTo(equalValue));
            }

            private void SetValueMatcher(Matcher matcher)
            {
                this.matchers[this.matchers.Length - 1] = matcher;
                this.expectation.ArgumentsMatcher = new IndexSetterArgumentsMatcher(this.matchers);
            }
        }
    }
}
