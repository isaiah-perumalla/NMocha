//-----------------------------------------------------------------------
// <copyright file="ComparisonMatcher.cs" company="NMock2">
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

    /// <summary>
    /// Matcher that checks a value against upper and lower bounds.
    /// </summary>
    public class ComparisonMatcher : Matcher
    {
        /// <summary>
        /// Stores the value to be compared.
        /// </summary>
        private readonly IComparable value;
        
        /// <summary>
        /// Stores the minimum comparison result for a successful match.
        /// </summary>
        private readonly int minComparisonResult;

        /// <summary>
        /// Stores the maximum comparison result for a successful match.
        /// </summary>
        private readonly int maxComparisonResult;

        /// <summary>
        /// Initializes a new instance of the <see cref="ComparisonMatcher"/> class.
        /// </summary>
        /// <param name="value">The value to compare.</param>
        /// <param name="comparisonResult1">The first allowed comparison result (result of value.CompareTo(other)).</param>
        /// <param name="comparisonResult2">The second allowed comparison result (result of value.CompareTo(other)).</param>
        public ComparisonMatcher(IComparable value, int comparisonResult1, int comparisonResult2)
        {
            this.value = value;
            this.minComparisonResult = Math.Min(comparisonResult1, comparisonResult2);
            this.maxComparisonResult = Math.Max(comparisonResult1, comparisonResult2);
            
            if (this.minComparisonResult == -1 && this.maxComparisonResult == 1)
            {
                throw new ArgumentException("comparison result range too large", "comparisonResult1, comparisonResult2");
            }
        }

        /// <summary>
        /// Matches the specified object to this matcher and returns whether it matches.
        /// </summary>
        /// <param name="o">The object to match.</param>
        /// <returns>Whether the object compared to the value resulted in either of both specified comparison results.</returns>
        public override bool Matches(object o)
        {
            if (o.GetType() == this.value.GetType())
            {
                int comparisonResult = -this.value.CompareTo(o);
                return comparisonResult >= this.minComparisonResult
                    && comparisonResult <= this.maxComparisonResult;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Describes this object.
        /// </summary>
        /// <param name="writer">The text writer the description is added to.</param>
        public override void DescribeTo(TextWriter writer)
        {
            writer.Write("? ");
            if (this.minComparisonResult == -1)
            {
                writer.Write("<");
            }

            if (this.maxComparisonResult == 1)
            {
                writer.Write(">");
            }

            if (this.minComparisonResult == 0 || this.maxComparisonResult == 0)
            {
                writer.Write("=");
            }

            writer.Write(" ");
            writer.Write(this.value);
        }
    }
}
