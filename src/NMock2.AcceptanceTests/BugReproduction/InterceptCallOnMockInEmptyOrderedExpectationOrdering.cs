//-----------------------------------------------------------------------
// <copyright file="MockServiceModelIClientChannel.cs" company="NMock2">
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
namespace NMock2.AcceptanceTests
{
    using NUnit.Framework;

    /// <summary>
    /// Tests the defect that a call on a stub cannot be performed in an empty ordered expectation block.
    /// </summary>
    [TestFixture]
    public class InterceptCallOnMockInEmptyOrderedExpectationOrdering
    {
        private Mockery mockery;

        /// <summary>
        /// Set up tests
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            this.mockery = new Mockery();
        }

        [TearDown]
        public void TearDown()
        {
            this.mockery.VerifyAllExpectationsHaveBeenMet();
        }

        /// <summary>
        /// This test is to check behavior with default mocks.
        /// </summary>
        [Test]
        public void StubDeclaredExplicitly()
        {
            IDependency dependency = this.mockery.NewInstanceOfRole<IDependency>();
            IAnotherDependency anotherDependency = this.mockery.NewInstanceOfRole<IAnotherDependency>();

            Stub.On(dependency).GetProperty("AnotherDependency").Will(Return.Value(anotherDependency));

            using (this.mockery.Ordered)
            {
                Expect.Once.On(dependency.AnotherDependency).Message("DoSomething");
            }

            dependency.AnotherDependency.DoSomething();
        }

        /// <summary>
        /// This test reproduces the defect.
        /// </summary>
        [Test]
        [Class]
        public void StubMockStyle()
        {
            IDependency dependency = this.mockery.NewInstanceOfRole<IDependency>(MockStyle.Stub);

            using (this.mockery.Ordered)
            {
                Expect.Once.On(dependency.AnotherDependency).Message("DoSomething");
            }

            dependency.AnotherDependency.DoSomething();
        }

        /// <summary>
        /// An interface for testing.
        /// </summary>
        public interface IDependency
        {
            /// <summary>
            /// Gets another dependency.
            /// </summary>
            /// <value>Another dependency.</value>
            IAnotherDependency AnotherDependency { get; }
        }

        /// <summary>
        /// Another interface for testing.
        /// </summary>
        public interface IAnotherDependency
        {
            /// <summary>
            /// Does something.
            /// </summary>
            void DoSomething();
        }
    }
}