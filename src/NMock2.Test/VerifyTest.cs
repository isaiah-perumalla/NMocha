//-----------------------------------------------------------------------
// <copyright file="VerifyTest.cs" company="NMock2">
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
namespace NMock2.Test
{
    using NMock2.Internal;
    using NUnit.Framework;

    [TestFixture]
    public class VerifyTest
    {
        [Test]
        public void VerifyThatPassesIfMatcherMatchesValue()
        {
            object value = new NamedObject("value");
            
            Verify.That(value, Is.Same(value));
        }

        [Test]
        public void VerifyThatFailsIfMatcherDoesNotMatchValue()
        {
            object expected = new NamedObject("expected");
            object actual = new NamedObject("actual");
            Matcher matcher = Is.Same(expected);
            
            try
            {
                Verify.That(actual, matcher);
            }
            catch (ExpectationException e)
            {
                Assert.AreEqual(
                    System.Environment.NewLine +
                    "Expected: "+matcher.ToString()+System.Environment.NewLine +
                    "Actual:   <"+actual.ToString()+">",
                    e.Message);
                return;
            }
            
            Assert.Fail("Verify.That should have failed");
        }

        [Test]
        public void CanPrependCustomMessageToDescriptionOfFailure()
        {
            object expected = new NamedObject("expected");
            object actual = new NamedObject("actual");
            Matcher matcher = Is.Same(expected);

            try
            {
                Verify.That(actual, matcher, "message {0} {1}", "0", 1);
            }
            catch (ExpectationException e)
            {
                Assert.AreEqual(
                    "message 0 1" + System.Environment.NewLine +
                    "Expected: "+matcher.ToString()+ System.Environment.NewLine +
                    "Actual:   <"+actual.ToString()+">",
                    e.Message);
                return;
            }

            Assert.Fail("Verify.That should have failed");
        }
    }
}
