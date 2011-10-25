//-----------------------------------------------------------------------
// <copyright file="ElementMatcherTest.cs" company="NMock2">
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
namespace NMock2.Test.Matchers
{
    using System.Collections;
    using NUnit.Framework;
    using NMock2.Matchers;

    [TestFixture]
    public class ElementMatcherTest
    {
    [Test]
    public void MatchesIfArgumentInCollection() {
        ICollection collection = CollectionOf(1,2,3,4);
        
        Matcher matcher = new ElementMatcher(collection);
        
        Assert.IsTrue(matcher.Matches(1), "should match 1");
        Assert.IsTrue(matcher.Matches(2), "should match 2");	    
        Assert.IsTrue(matcher.Matches(3), "should match 3");	    
        Assert.IsTrue(matcher.Matches(4), "should match 4");	    
        Assert.IsFalse(matcher.Matches(0), "should not match 0");
    }
    
    [Test]
    public void IsNullSafe() {
        ICollection collection = CollectionOf(1,2,null,4);
        
        Matcher matcher = new ElementMatcher(collection);
        
        Assert.IsTrue(matcher.Matches(1), "should match 1");
        Assert.IsTrue(matcher.Matches(2), "should match 2");	    
        Assert.IsTrue(matcher.Matches(null), "should match null");
        Assert.IsTrue(matcher.Matches(4), "should match 4");	    
        Assert.IsFalse(matcher.Matches(0), "should not match 0");
    }
    
    [Test]
    public void HasAReadableDescription()
    {
        AssertDescription.IsEqual(
            new ElementMatcher(CollectionOf("a","b", 1, 2)),
        "element of [\"a\", \"b\", <1>, <2>]");
        
        AssertDescription.IsEqual(
            new ElementMatcher(CollectionOf()),
        "element of []");
    }
    
    private ICollection CollectionOf(params object[] elements) {
        return elements;
    }
    }
}
