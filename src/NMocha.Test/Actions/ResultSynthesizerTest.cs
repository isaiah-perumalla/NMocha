//-----------------------------------------------------------------------
// <copyright file="ResultSynthesizerTest.cs" company="NMock2">
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
using System.Collections;
using System.IO;
using System.Reflection;
using NMocha;
using NMocha.Monitoring;
using NMock2.Actions;
using NMock2.Monitoring;
using NMock2.Test.Monitoring;
using NUnit.Framework;
using Is = NMocha.Is;

namespace NMock2.Test.Actions {
    [TestFixture]
    public class ResultSynthesizerTest {
        public struct AValueType {
            public int value1, value2;
        }

        private Matcher IsAnEmpty(Type type) {
            return new IsEmptyCollectionMatcher(type);
        }

        private class IsEmptyCollectionMatcher : Matcher {
            private readonly Type collectionType;

            public IsEmptyCollectionMatcher(Type collectionType) {
                if (!typeof (ICollection).IsAssignableFrom(collectionType))
                {
                    throw new ArgumentException(collectionType.FullName + " is not derived from ICollection");
                }

                this.collectionType = collectionType;
            }

            public override bool Matches(object o) {
                return collectionType.IsInstanceOfType(o)
                       && ((ICollection) o).Count == 0;
            }

            public override void DescribeOn(IDescription description) {
                description.AppendText("an empty " + collectionType.Name);
            }
        }

        private static readonly object RECEIVER = new NamedObject("receiver");

        private void AssertReturnsValue(IAction action, Type returnType, object expectedResult) {
            AssertReturnsValue(action, returnType, Is.EqualTo(expectedResult));
        }

        private void AssertReturnsValue(IAction action, Type returnType, Matcher resultMatcher) {
            Verify.That(ValueReturnedForType(action, returnType), resultMatcher,
                        "result for type " + returnType);
        }

        private static object ValueReturnedForType(IAction action, Type returnType) {
            var method = new MethodInfoStub("method", new ParameterInfo[0]);
            method.StubReturnType = returnType;

            var invocation = new Invocation(RECEIVER, method, new object[0]);

            action.Invoke(invocation);

            return invocation.Result;
        }

        [Test]
        public void CanOverrideDefaultResultForType() {
            var synth = new ResultSynthesizer();
            string newResult = "new result";
            synth.SetResult(typeof (string), newResult);

            AssertReturnsValue(synth, typeof (string), newResult);
        }

        [Test]
        public void CanSpecifyResultForOtherType() {
            var synth = new ResultSynthesizer();
            var value = new NamedObject("value");
            synth.SetResult(typeof (NamedObject), value);

            AssertReturnsValue(synth, typeof (NamedObject), value);
        }

        [Test]
        public void CreatesDefaultValuesForBasicTypes() {
            var synth = new ResultSynthesizer();

            AssertReturnsValue(synth, typeof (bool), false);
            AssertReturnsValue(synth, typeof (byte), (byte) 0);
            AssertReturnsValue(synth, typeof (sbyte), (sbyte) 0);
            AssertReturnsValue(synth, typeof (short), (short) 0);
            AssertReturnsValue(synth, typeof (ushort), (ushort) 0U);
            AssertReturnsValue(synth, typeof (int), 0);
            AssertReturnsValue(synth, typeof (uint), 0U);
            AssertReturnsValue(synth, typeof (long), 0L);
            AssertReturnsValue(synth, typeof (ulong), 0UL);
            AssertReturnsValue(synth, typeof (char), '\0');
            AssertReturnsValue(synth, typeof (string), "");
        }

        [Test]
        public void DoesNotTryToSetResultForVoidReturnType() {
            var synth = new ResultSynthesizer();

            AssertReturnsValue(synth, typeof (void), Missing.Value);
        }

        [Test]
        public void HasAReadableDescription() {
            AssertDescription.IsEqual(new ResultSynthesizer(),
                                      "a synthesized result");
        }

        [Test]
        public void ReturnsADifferentCollectionOnEachInvocation() {
            var synth = new ResultSynthesizer();
            var list = (ArrayList) ValueReturnedForType(synth, typeof (ArrayList));
            list.Add("a new element");

            AssertReturnsValue(synth, typeof (ArrayList), IsAnEmpty(typeof (ArrayList)));
        }

        [Test]
        public void ReturnsDefaultValueOfValueTypes() {
            var synth = new ResultSynthesizer();

            AssertReturnsValue(synth, typeof (DateTime), new DateTime());
            AssertReturnsValue(synth, typeof (AValueType), new AValueType());
        }

        [Test]
        public void ReturnsEmptyCollections() {
            var synth = new ResultSynthesizer();

            AssertReturnsValue(synth, typeof (ArrayList), IsAnEmpty(typeof (ArrayList)));
            AssertReturnsValue(synth, typeof (SortedList), IsAnEmpty(typeof (SortedList)));
            AssertReturnsValue(synth, typeof (Hashtable), IsAnEmpty(typeof (Hashtable)));
            AssertReturnsValue(synth, typeof (Stack), IsAnEmpty(typeof (Stack)));
            AssertReturnsValue(synth, typeof (Queue), IsAnEmpty(typeof (Queue)));
        }

        [Test]
        public void ReturnsZeroLengthArrays() {
            var synth = new ResultSynthesizer();

            AssertReturnsValue(synth, typeof (int[]), new int[0]);
            AssertReturnsValue(synth, typeof (string[]), new string[0]);
            AssertReturnsValue(synth, typeof (object[]), new object[0]);
        }

        [Test, ExpectedException(typeof (InvalidOperationException))]
        public void ThrowsExceptionIfTriesToReturnValueForUnsupportedResultType() {
            var synth = new ResultSynthesizer();

            AssertReturnsValue(synth, typeof (NamedObject), Is.Nothing);
        }
    }
}