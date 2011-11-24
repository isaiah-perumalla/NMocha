//-----------------------------------------------------------------------
// <copyright file="InvocationSemanticsTest.cs" company="NMock2">
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
using System;
using NUnit.Framework;

namespace NMock2.Test.Monitoring {
    [TestFixture]
    public class InvocationSemanticsTest {
        private const int REF_PARAM_VALUE = 1;
        private const int OUT_PARAM_VALUE = 2;

        public void SetAndThrow(ref int refParam, out int outParam) {
            refParam = REF_PARAM_VALUE;
            outParam = OUT_PARAM_VALUE;
            throw new TestException();
        }

        [Test]
        public void OutParametersAreSetAfterExceptionThrown() {
            int refParam = 0;
            int outParam = 0;

            try
            {
                SetAndThrow(ref refParam, out outParam);
            }
            catch (TestException)
            {
            }

            Assert.AreEqual(REF_PARAM_VALUE, refParam);
            Assert.AreEqual(OUT_PARAM_VALUE, outParam);
        }
    }

    internal class TestException : ApplicationException {
    }
}