﻿//-----------------------------------------------------------------------
// <copyright file="DescriptionOverride.cs" company="NMock2">
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
using System.IO;
using NMocha;

namespace NMock2.Matchers {
    /// <summary>
    /// Matcher that is used to change the description the wrapped matcher.
    /// </summary>
    public class DescriptionOverride : Matcher {
        /// <summary>
        /// Stores the new description for the wrapped matcher.
        /// </summary>
        private readonly string description;

        /// <summary>
        /// Stores the matcher to wrap.
        /// </summary>
        private readonly Matcher otherMatcher;

        /// <summary>
        /// Initializes a new instance of the <see cref="DescriptionOverride"/> class.
        /// </summary>
        /// <param name="description">The new description for the wrapped matcher.</param>
        /// <param name="otherMatcher">The matcher to wrap.</param>
        public DescriptionOverride(string description, Matcher otherMatcher) {
            this.description = description;
            this.otherMatcher = otherMatcher;
        }

        /// <summary>
        /// Matches the specified object to this matcher and returns whether it matches.
        /// </summary>
        /// <param name="o">The object to match.</param>
        /// <returns>Whether the wrapped matcher matches.</returns>
        public override bool Matches(object o) {
            return otherMatcher.Matches(o);
        }

        /// <summary>
        /// Describes this object.
        /// </summary>
        /// <param name="description1"></param>
        public override void DescribeOn(IDescription description1) {
            description1.AppendText(description);
        }
    }
}