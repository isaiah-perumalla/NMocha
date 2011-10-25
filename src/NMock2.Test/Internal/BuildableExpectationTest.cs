//-----------------------------------------------------------------------
// <copyright file="BuildableExpectationTest.cs" company="NMock2">
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
namespace NMock2.Internal.Test
{
    using System.IO;
    using NMock2.Matchers;
    using NMock2.Monitoring;
    using NMock2.Test.Monitoring;
    using NUnit.Framework;

    [TestFixture]
    public class BuildableExpectationTest
    {
        private Invocation invocation;
        private IMockObject receiver;
        
        [SetUp]
        public void SetUp()
        {
            Mockery mockery = new Mockery();
            this.receiver = mockery.NewInstanceOfRole<IMockObject>(DefinedAs.Named("receiver"));
            invocation = new Invocation(receiver, new MethodInfoStub("method"), new object[] { "arg" });
        }
        
        [Test]
        public void MatchesIfAllMatchersMatch()
        {
            BuildableExpectation e = BuildExpectation(true, true, this.receiver, true, true, true, true);
            Assert.IsTrue( e.Matches(this.invocation), "should match");
        }

        [Test]
        public void DoesNotMatchIfAnyMatcherDoesNotMatch()
        {
            const bool ignoreRequiredCallCount = true;
            
            for (int i = 1; i < 64; i++)
            {
                BuildableExpectation e = BuildExpectation(
                    ignoreRequiredCallCount,
                    (i & 1) == 0,
                    (i & 2) == 0 ? this.receiver : null,
                    (i & 4) == 0,
                    (i & 8) == 0,
                    (i & 16) == 0,
                    (i & 32) == 0);
                
                Assert.IsFalse(e.Matches(this.invocation), "should not match (iteration "+i+")");
            }
        }
    
        [Test]
        public void InvokesAListOfActionsToPerformAnInvocation()
        {
            BuildableExpectation e = BuildExpectation(true,true,this.receiver,true,true,true,true);
            MockAction action1 = new MockAction();
            MockAction action2 = new MockAction();
            
            e.AddAction(action1);
            e.AddAction(action2);
            
            e.Perform(invocation);
            
            Assert.AreSame( invocation, action1.Received, "action1 received");
            Assert.AreSame( invocation, action2.Received, "action1 received");
        }

        [Test]
        public void MatchesCallCountWhenMatchingInvocation()
        {
            Matcher irrelevant = Is.Anything;
            
            BuildableExpectation expectation = BuildExpectation(
                "description",
                irrelevant,
                Is.AtMost(4),
                this.receiver,
                irrelevant,
                irrelevant,
                irrelevant,
                irrelevant);
            
            AssertIsActive(expectation, "should be active before any invocation");
            Assert.IsTrue(expectation.Matches(invocation), "should match 1st invocation");
            expectation.Perform(invocation);

            AssertIsActive(expectation, "should be active before 2nd invocation");
            Assert.IsTrue(expectation.Matches(invocation), "should match 2nd invocation");
            expectation.Perform(invocation);

            AssertIsActive(expectation, "should be active before 3rd invocation");
            Assert.IsTrue(expectation.Matches(invocation), "should match 3rd invocation");
            expectation.Perform(invocation);

            AssertIsActive(expectation, "should be active before 4th invocation");
            Assert.IsTrue(expectation.Matches(invocation), "should match 4th invocation");
            expectation.Perform(invocation);
            
            AssertIsNotActive(expectation, "should not be active after 4th invocation");
            Assert.IsFalse(expectation.Matches(invocation), "should not match 5th invocation");
        }

        [Test]
        public void ChecksCallCountToAssertThatItHasBeenMet()
        {
            Matcher irrelevant = Is.Anything;
            IMockObject mock = null;
            
            BuildableExpectation expectation = BuildExpectation(
                "description",
                Is.AtLeast(2),
                Is.AtMost(4),
                mock,
                irrelevant,
                irrelevant,
                irrelevant,
                irrelevant);

            AssertHasNotBeenMet(expectation, "should not have been met after no invocations");
            
            expectation.Perform(invocation);
            AssertHasNotBeenMet(expectation, "should not have been met after 1 invocation");
            
            expectation.Perform(invocation);
            AssertHasBeenMet(expectation, "should have been met after 2 invocations");

            expectation.Perform(invocation);
            AssertHasBeenMet(expectation, "should have been met after 3 invocations");

            expectation.Perform(invocation);
            AssertHasBeenMet(expectation, "should have been met after 4 invocations");
        }
        
        [Test]
        public void HasReadableDescription()
        {
            BuildableExpectation expectation = BuildExpectation(
                "expectation",
                "required call count description is ignored",
                "matching call count description is ignored",
                this.receiver,
                "method",
                "(arguments)",
                "extra matcher 1", "extra matcher 2"
            );
            
            expectation.AddAction(new MockAction("action 1"));
            expectation.AddAction(new MockAction("action 2"));
            
            AssertDescriptionIsEqual(expectation,
                "expectation: receiver.method(arguments), extra matcher 1, extra matcher 2, will action 1, action 2 [called 0 times]");
        }
        
        [Test]
        public void WillNotPrintAPeriodBetweenReceiverAndMethodIfToldToDescribeItselfAsAnIndexer()
        {
            BuildableExpectation expectation = BuildExpectation(
                "expectation",
                "required call count description is ignored",
                "matching call count description is ignored",
                this.receiver,
                "",
                "[arguments]",
                "extra matcher 1", "extra matcher 2"
                );
            
            expectation.AddAction(new MockAction("action 1"));
            expectation.AddAction(new MockAction("action 2"));
            expectation.DescribeAsIndexer();
            
            AssertDescriptionIsEqual(expectation,
                "expectation: receiver[arguments], extra matcher 1, extra matcher 2, will action 1, action 2 [called 0 times]");
        }

        private static BuildableExpectation BuildExpectation(
            bool matchRequiredCallCount,
            bool matchMatchingCallCount,
            IMockObject receiver,
            bool matchMethod,
            bool matchArguments,
            params bool[] matchExtraMatchers)
        {
            Matcher[] extraMatchers = new Matcher[matchExtraMatchers.Length];
            for (int i = 0; i < extraMatchers.Length; i++)
            {
                extraMatchers[i] = new AlwaysMatcher(matchExtraMatchers[i], "extra matcher "+(i+1));
            }
            
            return BuildExpectation(
                "description",
                new AlwaysMatcher(matchRequiredCallCount, "required call count"),
                new AlwaysMatcher(matchMatchingCallCount, "matching call count"),
                receiver,
                new AlwaysMatcher(matchMethod, "method"),
                new AlwaysMatcher(matchArguments, "argument"),
                extraMatchers );
        }

        private static BuildableExpectation BuildExpectation(
            string expectationDescription,
            string matchRequiredCallCountDescription,
            string matchMatchingCallCountDescription,
            IMockObject receiver,
            string matchMethodDescription,
            string matchArgumentsDescription,
            params string[] extraMatcherDescriptions)
        {
            bool irrelevant = true;

            Matcher[] extraMatchers = new Matcher[extraMatcherDescriptions.Length];
            for (int i = 0; i < extraMatchers.Length; i++)
            {
                extraMatchers[i] = new AlwaysMatcher(irrelevant, extraMatcherDescriptions[i]);
            }
            
            return BuildExpectation(
                expectationDescription,
                new AlwaysMatcher(irrelevant, matchRequiredCallCountDescription),
                new AlwaysMatcher(irrelevant, matchMatchingCallCountDescription),
                receiver,
                new AlwaysMatcher(irrelevant, matchMethodDescription),
                new AlwaysMatcher(irrelevant, matchArgumentsDescription),
                extraMatchers);
        }

        private static BuildableExpectation BuildExpectation(
            string description,
            Matcher requiredCallCountMatcher,
            Matcher matchingCallCountMatcher,
            IMockObject receiver,
            Matcher methodMatcher,
            Matcher argumentsMatcher,
            params Matcher[] extraMatchers)
        {
            BuildableExpectation e =
                new BuildableExpectation(description, requiredCallCountMatcher, matchingCallCountMatcher);
            e.ArgumentsMatcher = argumentsMatcher;
            e.MethodMatcher = methodMatcher;
            e.Receiver = receiver;
            foreach (Matcher extraMatcher in extraMatchers) e.AddInvocationMatcher(extraMatcher);
            return e;
        }
        
        private void AssertIsActive(IExpectation expectation, string message)
        {
            Assert.IsTrue(expectation.IsActive, message);
        }
        
        private void AssertHasBeenMet(IExpectation expectation, string message)
        {
            Assert.IsTrue(expectation.HasBeenMet, message);
        }
        
        private void AssertHasNotBeenMet(IExpectation expectation, string message)
        {
            Assert.IsFalse(expectation.HasBeenMet, message);
        }

        private void AssertIsNotActive(IExpectation expectation, string message)
        {
            Assert.IsFalse(expectation.IsActive, message);
        }

        private void AssertDescriptionIsEqual(BuildableExpectation expectation, string expected)
        {
            DescriptionWriter writer = new DescriptionWriter();
            expectation.DescribeActiveExpectationsTo(writer);
            
            Assert.AreEqual(expected, writer.ToString());
        }
    }
    
    class MockAction : IAction
    {
        public Invocation Received = null;
        public MockAction Previous = null;
        public string Description;

        public MockAction() : this("MockAction")
        {
        }

        public MockAction(string description)
        {
            this.Description = description;
        }
        
        public void Invoke(Invocation invocation)
        {
            if (Previous != null) Assert.IsNotNull(Previous.Received, "called out of order");
            Received = invocation;
        }

        public void DescribeTo(TextWriter writer)
        {
            writer.Write(Description);
        }
    }
}
