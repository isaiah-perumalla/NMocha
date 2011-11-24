//-----------------------------------------------------------------------
// <copyright file="AcceptanceTestBase.cs" company="NMock2">
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
using NUnit.Framework;

namespace NMocha.AcceptanceTests {
    public abstract class AcceptanceTestBase {
        private bool doVerificationForCurrentTest = true;
        private bool doVerificationForEveryTestInFixture = true;
        private Mockery mockery;

        /// <summary>
        /// A default Mockery instance created for each test.
        /// </summary>
        protected Mockery Mockery {
            get { return mockery; }
        }

        [SetUp]
        public virtual void Setup() {
            mockery = new Mockery();
        }

        [TearDown]
        public virtual void Teardown() {
            if (doVerificationForCurrentTest && doVerificationForEveryTestInFixture)
            {
                mockery.VerifyAllExpectationsHaveBeenMet();
            }

            doVerificationForCurrentTest = true;
        }

        /// <summary>
        /// Prevents Mockery.VerifyAllExpectationsHaveBeenMet() being called after the current test.
        /// </summary>
        protected void SkipVerificationForThisTest() {
            doVerificationForCurrentTest = false;
        }

        /// <summary>
        /// Prevents Mockery.VerifyAllExpectationsHaveBeenMet() being called after every test in the current fixture.
        /// </summary>
        protected void SkipVerificationForThisFixture() {
            doVerificationForEveryTestInFixture = false;
        }
    }
}