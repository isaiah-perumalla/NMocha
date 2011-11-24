//-----------------------------------------------------------------------
// <copyright file="Invocation.cs" company="NMock2">
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
using System.Text;
using NMocha.Internal;
using NMock2;
using NMock2.Monitoring;

namespace NMocha.Monitoring {
    /// <summary>
    /// Represents the invocation of a method on an object (receiver).
    /// </summary>
    public class Invocation : ISelfDescribing {
        /// <summary>
        /// Holds the method that is being invoked.
        /// </summary>
        public readonly MethodInfo Method;

        /// <summary>
        /// Holds the parameterlist of the invocation.
        /// </summary>
        public readonly ParameterList Parameters;

        /// <summary>
        /// Holds the receiver providing the method.
        /// </summary>
        public readonly object Receiver;

        /// <summary>
        /// Holds the exception to be thrown. When this field has been set, <see cref="isThrowing"/> will become true.
        /// </summary>
        private Exception exception;

        /// <summary>
        /// Holds a boolean value whether the method is throwing an exception or not.
        /// </summary>
        private bool isThrowing;

        /// <summary>
        /// Holds the result of the invocation.
        /// </summary>
        private object result = Missing.Value;

        /// <summary>
        /// Initializes a new instance of the <see cref="Invocation"/> class.
        /// </summary>
        /// <param name="receiver">The receiver providing the method.</param>
        /// <param name="method">The method.</param>
        /// <param name="parameters">The parameters passed to the method..</param>
        public Invocation(object receiver, MethodInfo method, object[] parameters) {
            Receiver = receiver;
            Method = method;
            Parameters = new ParameterList(method, parameters);
        }

        /// <summary>
        /// Gets or sets the result of the invocation.
        /// </summary>
        /// <value>The result.</value>
        public object Result {
            get { return result; }

            set {
                CheckReturnType(value);

                result = value;
                exception = null;
                isThrowing = false;
            }
        }

        /// <summary>
        /// Gets or sets the exception that is thrown on the invocation.
        /// </summary>
        /// <value>The exception.</value>
        public Exception Exception {
            get { return exception; }

            set {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                exception = value;
                result = null;
                isThrowing = true;
            }
        }

        /// <summary>
        /// Gets a value indicating whether an exception is thrown an this invocation.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this invocation is throwing an exception; otherwise, <c>false</c>.
        /// </value>
        public bool IsThrowing {
            get { return isThrowing; }
        }

        #region ISelfDescribing Members

        /// <summary>
        /// Describes this object to the specified <paramref name="writer"/>.
        /// </summary>
        /// <param name="description"></param>
        public void DescribeOn(IDescription description) {
            // This should really be a mock object in most cases, but a few testcases
            // seem to supply strings etc as a Receiver.
            var mock = Receiver as IMockObject;

            if (mock != null)
            {
                description.AppendText(mock.MockName);
            }
            else
            {
                description.AppendText(Receiver.ToString());
            }

            if (MethodIsIndexerGetter())
            {
                DescribeAsIndexerGetter(description);
            }
            else if (MethodIsIndexerSetter())
            {
                DescribeAsIndexerSetter(description);
            }
            
           
            else if (MethodIsProperty())
            {
                DescribeAsProperty(description);
            }
            else
            {
                DescribeNormalMethod(description);
            }
        }

        #endregion

        /// <summary>
        /// Invokes this invocation on the specified receiver and stores the result and exception
        /// returns/thrown by the invocation.
        /// </summary>
        /// <param name="otherReceiver">The other receiver.</param>
        public void InvokeOn(object otherReceiver) {
            try
            {
                Result = Method.Invoke(otherReceiver, Parameters.AsArray);
                Parameters.MarkAllValuesAsSet();
            }
            catch (TargetInvocationException e)
            {
                Exception = e.InnerException;
            }
        }

        /// <summary>
        /// Checks the returnType of the initialized method if it is valid to be mocked.
        /// </summary>
        /// <param name="value">The return value to be checked.</param>
        private void CheckReturnType(object value) {
            if (Method.ReturnType == typeof (void) && value != null)
            {
                throw new ArgumentException("cannot return a value from a void method", "value");
            }

            if (Method.ReturnType != typeof (void) && Method.ReturnType.IsValueType && value == null)
            {
                if (
                    !(Method.ReturnType.IsGenericType &&
                      Method.ReturnType.GetGenericTypeDefinition() == typeof (Nullable<>)))
                {
                    throw new ArgumentException("cannot return a null value type", "value");
                }
            }

            if (value != null && !Method.ReturnType.IsInstanceOfType(value))
            {
                throw new ArgumentException(
                    "cannot return a value of type " + DescribeType(value) + " from a method returning " +
                    Method.ReturnType,
                    "value");
            }
        }

        /// <summary>
        /// Determines whether the initialized method is a property.
        /// </summary>
        /// <returns>
        /// Returns true if initialized method is a property; false otherwise.
        /// </returns>
        private bool MethodIsProperty() {
            return Method.IsSpecialName &&
                   ((Method.Name.StartsWith("get_") && Parameters.Count == 0) ||
                    (Method.Name.StartsWith("set_") && Parameters.Count == 1));
        }

        /// <summary>
        /// Determines whether the initialized method is an index getter.
        /// </summary>
        /// <returns>
        /// Returns true if initialized method is an index getter; false otherwise.
        /// </returns>
        private bool MethodIsIndexerGetter() {
            return Method.IsSpecialName
                   && Method.Name == "get_Item"
                   && Parameters.Count >= 1;
        }

        /// <summary>
        /// Determines whether the initialized method is an index setter.
        /// </summary>
        /// <returns>
        /// Returns true if initialized method is an index setter; false otherwise.
        /// </returns>
        private bool MethodIsIndexerSetter() {
            return Method.IsSpecialName
                   && Method.Name == "set_Item"
                   && Parameters.Count >= 2;
        }

        /// <summary>
        /// Determines whether the initialized method is an event adder.
        /// </summary>
        /// <returns>
        /// Returns true if initialized method is an event adder; false otherwise.
        /// </returns>
        private bool MethodIsEventAdder() {
            return Method.IsSpecialName
                   && Method.Name.StartsWith("add_")
                   && Parameters.Count == 1
                   && typeof (Delegate).IsAssignableFrom(Method.GetParameters()[0].ParameterType);
        }

        /// <summary>
        /// Determines whether the initialized method is an event remover.
        /// </summary>
        /// <returns>
        /// Returns true if initialized method is an event remover; false otherwise.
        /// </returns>
        private bool MethodIsEventRemover() {
            return Method.IsSpecialName
                   && Method.Name.StartsWith("remove_")
                   && Parameters.Count == 1
                   && typeof (Delegate).IsAssignableFrom(Method.GetParameters()[0].ParameterType);
        }

        /// <summary>
        /// Describes the property with parameters to the specified <paramref name="writer"/>.
        /// </summary>
        /// <param name="writer">The writer where the description is written to.</param>
        private void DescribeAsProperty(IDescription writer) {
            writer.AppendText(".")
                  .AppendText(Method.Name.Substring(4));
            if (Parameters.Count > 0)
            {
                writer.AppendText(" = ")
                    .AppendValue(Parameters[0]);
            }
        }

        /// <summary>
        /// Describes the index setter with parameters to the specified <paramref name="writer"/>.
        /// </summary>
        /// <param name="writer">The writer where the description is written to.</param>
        private void DescribeAsIndexerGetter(IDescription writer) {
            writer.AppendText("[");
            WriteParameterList(writer, Parameters.Count);
            writer.AppendText("]");
        }

        /// <summary>
        /// Describes the index setter with parameters to the specified <paramref name="writer"/>.
        /// </summary>
        /// <param name="writer">The writer where the description is written to.</param>
        private void DescribeAsIndexerSetter(IDescription writer) {
            writer.AppendText("[");
            WriteParameterList(writer, Parameters.Count - 1);
            writer.AppendText("] = ")
                  .AppendValue(Parameters[Parameters.Count - 1]);
        }

        /// <summary>
        /// Describes the method with parameters to the specified <paramref name="writer"/>.
        /// </summary>
        /// <param name="writer">The writer where the description is written to.</param>
        private void DescribeNormalMethod(IDescription writer) {
            writer.AppendText(".")
            .AppendText(Method.Name);

            WriteTypeParams(writer);

            writer.AppendText("(");
            WriteParameterList(writer, Parameters.Count);
            writer.AppendText(")");
        }

        /// <summary>
        /// Writes the generic parameters of the method to the specified <paramref name="writer"/>.
        /// </summary>
        /// <param name="writer">The writer where the description is written to.</param>
        private void WriteTypeParams(IDescription writer) {
            Type[] types = Method.GetGenericArguments();
            if (types.Length > 0)
            {
                writer.AppendText("<");

                for (int i = 0; i < types.Length; i++)
                {
                    if (i > 0)
                    {
                        writer.AppendText(", ");
                    }

                    writer.AppendText(types[i].FullName);
                }

                writer.AppendText(">");
            }
        }

        /// <summary>
        /// Writes the parameter list to the specified <paramref name="writer"/>.
        /// </summary>
        /// <param name="writer">The writer where the description is written to.</param>
        /// <param name="count">The count of parameters to describe.</param>
        private void WriteParameterList(IDescription writer, int count) {
            for (int i = 0; i < count; i++)
            {
                if (i > 0)
                {
                    writer.AppendText(", ");
                }

                if (Method.GetParameters()[i].IsOut)
                {
                    writer.AppendText("out");
                }
                else
                {
                    writer.AppendValue(Parameters[i]);
                }
            }
        }

        /// <summary>
        /// Describes the interfaces used for <see cref="DescribeOn"/>.
        /// </summary>
        /// <param name="obj">The object which interfaces to describe.</param>
        /// <returns>
        /// Returns a string containing the description of the given object's interfaces.
        /// </returns>
        private string DescribeType(object obj) {
            Type type = obj.GetType();
            var sb = new StringBuilder();
            sb.Append(type);

            Type[] interfaceTypes = type.GetInterfaces();
            if (interfaceTypes.Length > 0)
            {
                sb.Append(": ");

                foreach (Type interfaceType in interfaceTypes)
                {
                    sb.Append(interfaceType);
                    sb.Append(", ");
                }

                sb.Length -= 2; // cut away last ", "
            }

            return sb.ToString();
        }
    }
}