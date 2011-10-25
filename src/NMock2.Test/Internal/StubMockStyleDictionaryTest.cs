//-----------------------------------------------------------------------
// <copyright file="StubMockStyleDictionaryTest.cs" company="NMock2">
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
    using NUnit.Framework;

    /// <summary>
    /// Tests the <see cref="StubMockStyleDictionary"/> class.
    /// </summary>
    [TestFixture]
    public class StubMockStyleDictionaryTest
    {
        /// <summary>
        /// The mock factory.
        /// </summary>
        private Mockery mockery;

        /// <summary>
        /// The object under test.
        /// </summary>
        private StubMockStyleDictionary testee;

        /// <summary>
        /// Creates mockery and <see cref="testee"/>.
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            this.mockery = new Mockery();
            this.testee = new StubMockStyleDictionary();    
        }

        /// <summary>
        /// If a mapping is defined for a mock then it can be requested either for the mock or for a type of that mock.
        /// </summary>
        [Test]
        public void MockStyleForStub()
        {
            IMockObject mock = this.mockery.NewMock<IMockObject>();

            this.testee[mock] = MockStyle.Stub;

            Assert.AreEqual(MockStyle.Stub, this.testee[mock]);
            Assert.AreEqual(MockStyle.Stub, this.testee[mock, typeof(IExpectException)]);
        }

        /// <summary>
        /// Mappings can be set and requested for a type on a mock.
        /// </summary>
        [Test]
        public void MockStyleForStubAndType()
        {
            IMockObject mock = this.mockery.NewMock<IMockObject>();

            this.testee[mock, typeof(IExpectException)] = MockStyle.Stub;

            Assert.AreEqual(MockStyle.Stub, this.testee[mock, typeof(IExpectException)]);
        }

        /// <summary>
        /// IF there is no entry for the requested mock and type then null is returned.
        /// </summary>
        [Test]
        public void RequestNonExistingItem()
        {
            IMockObject mock = this.mockery.NewMock<IMockObject>();

            Assert.IsNull(this.testee[mock, typeof(IExpectException)]);
        }

        /// <summary>
        /// A already defined mapping can be overridden.
        /// </summary>
        [Test]
        public void Override()
        {
            IMockObject mock = this.mockery.NewMock<IMockObject>();

            this.testee[mock] = MockStyle.Transparent;
            this.testee[mock, typeof(IExpectException)] = MockStyle.Stub;

            this.testee[mock, typeof(IExpectException)] = null;

            Assert.AreEqual(MockStyle.Transparent, this.testee[mock, typeof(IExpectException)]);

            this.testee[mock] = null;

            Assert.IsNull(this.testee[mock]);
            Assert.IsNull(this.testee[mock, typeof(IExpectException)]);
        }
    }
}