//-----------------------------------------------------------------------
// <copyright file="ParameterListTest.cs" company="NMock2">
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
// This is the easiest way to ignore StyleCop rules on this file, even if we shouldn't use this tag:
// <auto-generated />
//-----------------------------------------------------------------------
namespace NMock2.Test.Monitoring
{
    using System;
    using System.Reflection;
    using NUnit.Framework;
    using NMock2.Monitoring;

    [TestFixture]
    public class ParameterListTest
    {
        static readonly object inValue = "inValue";
        static readonly object refValue = "refValue";
        
        const int IN_PARAMETER_INDEX = 0;
        const int REF_PARAMETER_INDEX = 1;
        const int OUT_PARAMETER_INDEX = 2;

        MethodInfo method;
        object[] parameterValues;
        ParameterList list;
        
        [SetUp]
        public void SetUp()
        {
            ParameterInfo inParam = new ParameterInfoStub("inParam", ParameterAttributes.In);
            ParameterInfo refParam = new ParameterInfoStub("refParam", ParameterAttributes.None);
            ParameterInfo outParam = new ParameterInfoStub("outParam", ParameterAttributes.Out);

            method = new MethodInfoStub("method", inParam, refParam, outParam);
            
            parameterValues = new object[]{inValue, refValue, null};

            list = new ParameterList(method, parameterValues);
        }
        
        [Test]
        public void ReturnsNumberOfParameters()
        {
            Assert.AreEqual( parameterValues.Length, list.Count, "size" );
        }
        
        [Test]
        public void ReturnsValuesOfInParameters()
        {
            Assert.IsTrue(list.IsValueSet(IN_PARAMETER_INDEX), "in parameter should be set");
            Assert.AreSame(inValue, list[IN_PARAMETER_INDEX], "in value");
            Assert.IsTrue(list.IsValueSet(REF_PARAMETER_INDEX), "ref parameter should be set");
            Assert.AreSame(refValue, list[REF_PARAMETER_INDEX], "ref value");
        }
        
        [Test, ExpectedException(typeof(InvalidOperationException))]
        public void DoesNotAllowAccessToValuesOfUnsetOutParameters()
        {
            Assert.IsFalse(list.IsValueSet(OUT_PARAMETER_INDEX), "out parameter is not set");
            Ignore( list[OUT_PARAMETER_INDEX] );
        }

        [Test]
        public void CanSetValuesOfOutAndRefParameters()
        {
            object newRefValue = "newRefValue";
            object outValue = "outValue";

            list[REF_PARAMETER_INDEX] = newRefValue;
            list[OUT_PARAMETER_INDEX] = outValue;
            
            Assert.AreSame(newRefValue, list[REF_PARAMETER_INDEX], "new ref value");
            Assert.IsTrue(list.IsValueSet(OUT_PARAMETER_INDEX), "out parameter is set");
            Assert.AreSame(outValue, list[OUT_PARAMETER_INDEX], "out value");
        }

        [Test, ExpectedException(typeof(InvalidOperationException))]
        public void DoesNotAllowValuesOfInputParametersToBeChanged()
        {
            object newValue = "newValue";

            list[IN_PARAMETER_INDEX] = newValue;
        }
        
        private void Ignore(object o) {
            // The things you have to do to ignore compiler warnings!
            object o2 = o;
            o = o2;
        }
    }
}
