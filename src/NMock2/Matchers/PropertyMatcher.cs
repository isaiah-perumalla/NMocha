//-----------------------------------------------------------------------
// <copyright file="PropertyMatcher.cs" company="NMock2">
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
    /// Matcher that checks whether the actual object has a property with the specified name 
    /// and its value matches the specified matcher.
    /// </summary>
    public class PropertyMatcher : Matcher
    {
        private readonly string propertyName;
        private readonly Matcher valueMatcher;

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyMatcher"/> class.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="valueMatcher">The value matcher.</param>
        public PropertyMatcher(string propertyName, Matcher valueMatcher)
        {
            this.propertyName = propertyName;
            this.valueMatcher = valueMatcher;
        }

        /// <summary>
        /// Matches the specified object to this matcher and returns whether it matches.
        /// </summary>
        /// <param name="o">The object to match.</param>
        /// <returns>Whether the object has a property with the expected name and expected value.</returns>
        public override bool Matches(object o)
        {
            Type type = o.GetType();
            PropertyInfo property = type.GetProperty(this.propertyName, BindingFlags.Public | BindingFlags.Instance);

            if (property == null)
            {
                return false;
            }

            if (!property.CanRead)
            {
                return false;
            }
            
            object value = property.GetValue(o, null);
            return this.valueMatcher.Matches(value);
        }

        /// <summary>
        /// Describes this object.
        /// </summary>
        /// <param name="writer">The text writer the description is added to.</param>
        public override void DescribeTo(TextWriter writer)
        {
            writer.Write(string.Format("property '{0}' ", this.propertyName));
            this.valueMatcher.DescribeTo(writer);
        }
    }
}
