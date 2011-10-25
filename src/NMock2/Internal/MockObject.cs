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
namespace NMock2.Internal
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using Monitoring;
    using NMock2.Matchers;

    public class MockObject : IInvokable, IMockObject
    {
        /// <summary>
        /// Stores the backlink to the mockery which created this mock object.
        /// </summary>
        private readonly Mockery mockery;

        /// <summary>
        /// Stores the mocked type(s).
        /// </summary>
        private readonly CompositeType mockedTypes;

        /// <summary>
        /// Stores the name of the mock object.
        /// </summary>
        private readonly string name;

        /// <summary>
        /// Stores the event handlers that could be added to the mock object.
        /// </summary>
        private readonly IDictionary eventHandlers;

        /// <summary>
        /// Results that have been explicitly assigned via a call to a property setter.
        /// These will be returned for all subsequent calls to the matching property getter.
        /// </summary>
        private readonly Dictionary<string, object> assignedPropertyResults = new Dictionary<string, object>();

        /// <summary>
        /// Results that have been generated for methods or property getters.
        /// These will be returned for all subsequent calls to the same member.
        /// </summary>
        private readonly Dictionary<MethodInfo, object> rememberedMethodResults = new Dictionary<MethodInfo, object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="MockObject"/> class.
        /// This constructor is needed by the <see cref="InterfaceOnlyMockObjectFactory"/> (the IL generation has to be changed!)
        /// </summary>
        /// <param name="mockery">The mockery.</param>
        /// <param name="mockedType">Type of the mocked.</param>
        /// <param name="name">The name.</param>
        protected MockObject(Mockery mockery, Type mockedType, string name)
            : this(mockery, new CompositeType(new Type[] { mockedType }), name, MockStyle.Default)
        {    
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MockObject"/> class.
        /// </summary>
        /// <param name="mockery">The mockery.</param>
        /// <param name="mockedType">Type of the mocked.</param>
        /// <param name="name">The name.</param>
        /// <param name="mockStyle">The mock style.</param>
        protected MockObject(Mockery mockery, CompositeType mockedType, string name, MockStyle mockStyle)
        {
            this.mockery = mockery;
            this.MockStyle = mockStyle;
            this.name = name;
            this.eventHandlers = new Hashtable();
            this.mockedTypes = mockedType;
        }

        public string MockName
        {
            get { return this.name; }
        }

        protected CompositeType MockedTypes
        {
            get { return this.mockedTypes; }
        }

        protected Mockery Mockery
        {
            get { return this.mockery; }
        }

        /// <summary>
        /// Gets the mock style of this mock.
        /// </summary>
        protected MockStyle MockStyle { get; private set; }

        public override string ToString()
        {
            return this.name;
        }

        public void Invoke(Invocation invocation)
        {
            switch (this.MockStyle)
            {
                case MockStyle.Default:
                case MockStyle.Transparent:
                    this.mockery.Dispatch(invocation);
                    break;

                case MockStyle.Stub:
                    {
                        if (this.mockery.HasExpectationForIgnoringIsActive(invocation))
                        {
                            goto case MockStyle.Default;
                        }

                        // remember values set in a property setter
                        if (invocation.Method.Name.StartsWith("set_"))
                        {
                            string getter = "get_" + invocation.Method.Name.Substring(4);
                            if (this.assignedPropertyResults.ContainsKey(getter))
                            {
                                this.assignedPropertyResults[getter] = invocation.Parameters[0];
                            }
                            else
                            {
                                this.assignedPropertyResults.Add(getter, invocation.Parameters[0]);
                            }

                            return;
                        }
                        
                        if (invocation.Method.Name.StartsWith("add_"))
                        {
                            this.AddEventHandler(invocation.Method.Name.Substring(4), (Delegate)invocation.Parameters[0]);
                            return;
                        }
                        
                        if (invocation.Method.Name.StartsWith("remove_"))
                        {
                            this.RemoveEventHandler(invocation.Method.Name.Substring(7), (Delegate)invocation.Parameters[0]);
                            return;
                        }

                        // check whether we already have a value for this call
                        object result;

                        if (this.assignedPropertyResults.ContainsKey(invocation.Method.Name))
                        {
                            result = this.assignedPropertyResults[invocation.Method.Name];
                        }
                        else if (this.rememberedMethodResults.ContainsKey(invocation.Method))
                        {
                            result = this.rememberedMethodResults[invocation.Method];
                        }
                        else
                        {
                            result = this.GetStubResult(invocation);
                            this.rememberedMethodResults.Add(invocation.Method, result);
                        }

                        if (result != Missing.Value)
                        {
                            invocation.Result = result;
                        }

                        break;
                    }
            }
        }

        public bool HasMethodMatching(Matcher methodMatcher)
        {
            return this.mockedTypes.GetMatchingMethods(methodMatcher, true).Count > 0;
        }

        public IList<MethodInfo> GetMethodsMatching(Matcher methodMatcher)
        {
            return this.mockedTypes.GetMatchingMethods(methodMatcher, false);
        }

        public void AddExpectation(IExpectation expectation)
        {
            this.mockery.AddExpectation(expectation);
        }

        public void AddEventHandler(string eventName, Delegate handler)
        {
            ArrayList handlers = (ArrayList)this.eventHandlers[eventName];

            if (handlers == null)
            {
                handlers = new ArrayList();
                this.eventHandlers.Add(eventName, handlers);
            }

            if (! handlers.Contains(handler)) 
            { 
                handlers.Add(handler); 
            }
        }

        public void RaiseEvent(string eventName, params object[] args)
        {
            IEnumerable handlers = (IEnumerable)this.eventHandlers[eventName];
    
            if (handlers != null)
            {
                // copy handlers before invocation to prevent colection modified exception if event handler is removed within event handler itself.
                List<Delegate> delegates = new List<Delegate>();
                foreach (Delegate handler in handlers)
                {
                    delegates.Add(handler);    
                }

                foreach (Delegate d in delegates)
                {
                    try
                    {
                        d.DynamicInvoke(args);
                    }
                    catch (TargetInvocationException tie)
                    {
                        // replace stack trace with original stack trace
                        FieldInfo remoteStackTraceString = typeof(Exception).GetField("_remoteStackTraceString", BindingFlags.Instance | BindingFlags.NonPublic);
                        remoteStackTraceString.SetValue(tie.InnerException, tie.InnerException.StackTrace + Environment.NewLine);
                        throw tie.InnerException;
                    }
                }
            }
            else
            {
                // check whether the mocked type has this event
                if (!this.HasMethodMatching(new MethodNameMatcher("add_" + eventName)))
                {
                    throw new ArgumentException(
                           string.Format(
                               "Event with name {0} does not exist on type(s) {1}.",
                               eventName,
                               this.mockedTypes.ToString()),
                           "eventName");
                }
            }
        }

        public void RemoveEventHandler(string eventName, Delegate handler)
        {
            ArrayList handlers = (ArrayList)this.eventHandlers[eventName];

            if (handlers != null)
            {
                handlers.Remove(handler);
                if (handlers.Count == 0)
                {
                    this.eventHandlers.Remove(eventName);
                }
            }
        }

        /// <summary>
        /// Gets the default result for an invocation.
        /// </summary>
        /// <param name="invocation">The invocation.</param>
        /// <returns>The default value to return as result of the invocation. 
        /// <see cref="Missing.Value"/> if no default value was provided.</returns>
        private object GetStubResult(Invocation invocation)
        {
            Type returnType = invocation.Method.ReturnType;

            // void method
            if (returnType == typeof(void))
            {
                return Missing.Value;
            }

            // see if developer provides a return value
            object returnValue = this.mockery.ResolveType(invocation.Receiver, returnType);

            if (returnValue != Missing.Value)
            {
                return returnValue;
            }

            if (returnType.IsValueType)
            {
                // use default contructor for value types
                return Activator.CreateInstance(returnType);
            }

            if (returnType == typeof(string))
            {
                // string empty for strings
                return string.Empty;
            }
            
            if (returnType.IsClass && returnType.GetInterface("IEnumerable") != null)
            {
                // for enumerables (List, Dictionary) we create an empty object
                return Activator.CreateInstance(returnType);
            }
            
            if (returnType.IsSealed)
            {
                // null for sealed classes
                return null;
            }

            // a mock for interfaces and all cases no covered above
            return this.mockery.NewNamedMock(
                returnType, 
                this.GetMemberName(invocation),
                this.mockery.GetDependencyMockStyle(invocation.Receiver, returnType) ?? this.MockStyle);
        }

        /// <summary>
        /// Gets the name of the member to be used as the name for a mock returned an a call to a stub.
        /// </summary>
        /// <param name="invocation">The invocation.</param>
        /// <returns>Name of the mock created as a result value on a call to a stub.</returns>
        private string GetMemberName(Invocation invocation)
        {
            StringWriter writer = new StringWriter();
            invocation.DescribeTo(writer);
            return writer.ToString();
        }
    }
}
