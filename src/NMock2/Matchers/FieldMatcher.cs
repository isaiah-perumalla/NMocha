//-----------------------------------------------------------------------
// <copyright file="FieldMatcher.cs" company="NMock2">
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
    using System;
    using System.IO;
    using System.Reflection;

    /// <summary>
    /// Matcher that checks whether the specified field of the actual object matches with the specified matcher. 
    /// </summary>
    public class FieldMatcher : Matcher
    {
        /// <summary>
        /// Name of the field to match against the <seealso cref="valueMatcher"/>.
        /// </summary>
        private readonly string fieldName;

        /// <summary>
        /// The value <see cref="Matcher"/> used to match the field of the object under investigation.
        /// </summary>
        private readonly Matcher valueMatcher;

        /// <summary>
        /// Initializes a new instance of the <see cref="FieldMatcher"/> class.
        /// </summary>
        /// <param name="fieldName">Name of the field to match against the <paramref name="valueMatcher"/>.</param>
        /// <param name="valueMatcher">The value matcher.</param>
        public FieldMatcher(string fieldName, Matcher valueMatcher)
        {
            this.fieldName = fieldName;
            this.valueMatcher = valueMatcher;
        }

        /// <summary>
        /// Matches the specified object to this matcher and returns whether it matches.
        /// </summary>
        /// <param name="o">The object to match.</param>
        /// <returns>Whether the object matches.</returns>
        public override bool Matches(object o)
        {
            Type type = o.GetType();
            FieldInfo field = type.GetField(this.fieldName, BindingFlags.Public | BindingFlags.Instance);

            if (field == null)
            {
                return false;
            }
            
            object value = field.GetValue(o);
            return this.valueMatcher.Matches(value);
        }

        /// <summary>
        /// Describes this object.
        /// </summary>
        /// <param name="writer">The text writer the description is added to.</param>
        public override void DescribeTo(TextWriter writer)
        {
            writer.Write(string.Format("field '{0}' ", this.fieldName));
            this.valueMatcher.DescribeTo(writer);
        }
    }
}
