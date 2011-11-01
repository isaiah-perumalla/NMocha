//-----------------------------------------------------------------------
// <copyright file="ErrorMessageDemo.cs" company="NMock2">
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
using NMock2.Internal;
using NUnit.Framework;

namespace NMock2.AcceptanceTests {
    [TestFixture]
    public class ErrorMessageDemo : AcceptanceTestBase {
        public delegate void Action();

        public interface ISyntacticSugar {
            string Property { get; set; }
            string this[string s] { get; set; }
            int this[int i, string s] { get; set; }

            event Action Actions;
        }

        [TestFixtureSetUp]
        public void TestFixtureSetUp() {
            SkipVerificationForThisFixture();
        }

        private void DoAction() {
            throw new NotSupportedException();
        }

        [Test, ExpectedException(typeof (ExpectationException))]
        public void EventAdd() {
            var sugar = (ISyntacticSugar) Mockery.NewNamedInstanceOfRole(typeof (ISyntacticSugar), "sugar");

            sugar.Actions += DoAction;
        }

        [Test, ExpectedException(typeof (ExpectationException))]
        public void IndexerSet() {
            var sugar = (ISyntacticSugar) Mockery.NewNamedInstanceOfRole(typeof (ISyntacticSugar), "sugar");

            Expect.Once.On(sugar).Set[10, "goodbye"].To(12);

            sugar[10, "hello"] = 11;
        }

        [Test, ExpectedException(typeof (ExpectationException))]
        public void UnexpectedInvocation() {
            var speaker = (ISpeaker) Mockery.NewInstanceOfRole(typeof (ISpeaker));

            Expect.Once.On(speaker).Message("Hello").WithNoArguments();
            Expect.Between(2, 4).On(speaker).Message("Ask").With("What color is the fish?")
                .Will(Return.Value("purple"));
            Expect.AtLeast(1).On(speaker).Message("Ask").With("How big is the fish?")
                .Will(Throw.Exception(new InvalidOperationException("stop asking about the fish!")));

            speaker.Hello();
            speaker.Ask("What color is the fish?");
            speaker.Ask("What color is the hippo?");
        }

        [Test, ExpectedException(typeof (ExpectationException))]
        public void VerifyFailure() {
            var speaker = (ISpeaker) Mockery.NewInstanceOfRole(typeof (ISpeaker));

            Expect.Once.On(speaker).Message("Hello").WithNoArguments();
            Expect.Between(2, 4).On(speaker).Message("Ask").With("What color is the fish?")
                .Will(Return.Value("purple"));
            Expect.AtLeast(1).On(speaker).Message("Ask").With("How big is the fish?")
                .Will(Throw.Exception(new InvalidOperationException("stop asking about the fish!")));

            speaker.Hello();
            speaker.Ask("What color is the fish?");

            Mockery.VerifyAllExpectationsHaveBeenMet();
        }
    }
}