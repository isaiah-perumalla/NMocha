//-----------------------------------------------------------------------
// <copyright file="MethodNameMatcher.cs" company="NMock2">
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
namespace NMock2.Matchers
{
    using System.IO;
    using System.Reflection;

    /// <summary>
    /// Matcher that checks whether the actual object is a <see cref="MethodInfo"/> and its name is equal to the expected name.
    /// </summary>
    public class MethodNameMatcher : Matcher
    {
        /// <summary>
        /// Holds the expected name of the mocked method.
        /// </summary>
        private readonly string methodName;

        /// <summary>
        /// Initializes a new instance of the <see cref="MethodNameMatcher"/> class.
        /// </summary>
        /// <param name="methodName">The expected name of the method.</param>
        public MethodNameMatcher(string methodName)
        {
            this.methodName = methodName;
        }

        /// <summary>
        /// Matches the specified object to this matcher and returns whether it matches.
        /// </summary>
        /// <param name="o">The MethodInfo to match.</param>
        /// <returns>Whether the object is a MethodInfo and its name matches the expected one.</returns>
        public override bool Matches(object o)
        {
            return o is MethodInfo && ((MethodInfo)o).Name == this.methodName;
        }

        /// <summary>
        /// Describes this object.
        /// </summary>
        /// <param name="writer">The text writer the description is added to.</param>
        public override void DescribeTo(TextWriter writer)
        {
            writer.Write(this.methodName);
        }
    }
}
