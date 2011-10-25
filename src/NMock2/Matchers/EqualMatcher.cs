//-----------------------------------------------------------------------
// <copyright file="EqualMatcher.cs" company="NMock2">
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
    using System.Collections;
    using System.IO;

    /// <summary>
    /// Matcher that checks whether the expected and actual value are equal.
    /// </summary>
    public class EqualMatcher : Matcher
    {
        private readonly object expected;

        /// <summary>
        /// Initializes a new instance of the <see cref="EqualMatcher"/> class.
        /// </summary>
        /// <param name="expected">The expected value.</param>
        public EqualMatcher(object expected)
        {
            this.expected = expected;
        }

        /// <summary>
        /// Matcheses the specified actual.
        /// </summary>
        /// <param name="actual">The actual value.</param>
        /// <returns>Whether the expected value is equal to the actual value.</returns>
        public override bool Matches(object actual)
        {
            return this.AreEqual(this.expected, actual);
        }

        /// <summary>
        /// Describes this object.
        /// </summary>
        /// <param name="writer">The text writer the description is added to.</param>
        public override void DescribeTo(TextWriter writer)
        {
            writer.Write("equal to ");
            writer.Write(this.expected);
        }

        private bool AreEqual(object o1, object o2)
        {
            if (o1 is Array)
            {
                return o2 is Array && this.ArraysEqual((Array)o1, (Array)o2);
            }

            if (o1 is IList)
            {
                return o2 is IList && this.ListsEqual((IList)o1, (IList)o2);
            }
            else
            {
                return Equals(o1, o2);
            }
        }

        private bool ArraysEqual(Array a1, Array a2)
        {
            return a1.Rank == a2.Rank
            && this.ArrayDimensionsEqual(a1, a2)
            && this.ArrayElementsEqual(a1, a2, new int[a1.Rank], 0);
        }

        private bool ArrayDimensionsEqual(Array a1, Array a2)
        {
            for (int dimension = 0; dimension < a1.Rank; dimension++)
            {
                if (a1.GetLength(dimension) != a2.GetLength(dimension))
                {
                    return false;
                }
            }

            return true;
        }

        private bool ArrayElementsEqual(Array a1, Array a2, int[] indices, int dimension)
        {
            if (dimension == a1.Rank)
            {
                return this.AreEqual(a1.GetValue(indices), a2.GetValue(indices));
            }
            else
            {
                for (indices[dimension] = 0;
                     indices[dimension] < a1.GetLength(dimension);
                     indices[dimension]++)
                {
                    if (!this.ArrayElementsEqual(a1, a2, indices, dimension + 1))
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        private bool ListsEqual(IList l1, IList l2)
        {
            return l1.Count == l2.Count && this.ListElementsEqual(l1, l2);
        }

        private bool ListElementsEqual(IList l1, IList l2)
        {
            for (int i = 0; i < l1.Count; i++)
            {
                if (!this.AreEqual(l1[i], l2[i]))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
