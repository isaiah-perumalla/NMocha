// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IsTest.cs" company="NMock2">
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
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace NMock2
{
    using Matchers;
    using NUnit.Framework;

    /// <summary>
    /// Tests the <see cref="Is"/> class.
    /// </summary>
    [TestFixture]
    public class IsTest
    {
        /// <summary>
        /// Is.TypeOf{T} returns a type matcher on the specified type."/>
        /// </summary>
        [Test]
        public void IsTypeOfGeneric()
        {
            Matcher matcher = Is.TypeOf<Matcher>();

            Assert.IsNotNull(matcher);
            Assert.IsInstanceOfType(typeof(TypeMatcher), matcher);
            Assert.IsTrue(matcher.Matches(matcher));
        }
    }
}