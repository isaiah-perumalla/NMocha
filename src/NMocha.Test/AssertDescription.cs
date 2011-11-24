//-----------------------------------------------------------------------
// <copyright file="AssertDescription.cs" company="NMock2">
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
using System.IO;
using NMocha;
using NMocha.Internal;
using NMock2.Internal;
using NUnit.Framework;

namespace NMock2.Test {
    public abstract class AssertDescription {
        public static void IsEqual(ISelfDescribing selfDescribing, string expectedDescription) {
            Assert.AreEqual(expectedDescription, DescriptionOf(selfDescribing), "description");
        }

        private static string DescriptionOf(ISelfDescribing selfDescribing) {
            var writer = new StringDescriptionWriter();

            selfDescribing.DescribeOn(writer);
            return writer.ToString();
        }

        public static void IsComposed(ISelfDescribing selfDescribing, string format, params ISelfDescribing[] components) {
            var componentDescriptions = new string[components.Length];
            for (int i = 0; i < components.Length; i++)
            {
                componentDescriptions[i] = DescriptionOf(components[i]);
            }

            IsEqual(selfDescribing, String.Format(format, componentDescriptions));
        }
    }
}