//-----------------------------------------------------------------------
// <copyright file="DescriptionWriter.cs" company="NMock2">
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
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NMock2.Internal {
    /// <summary>
    /// Used to describe Matchers and other classes for exception handling.
    /// </summary>
    public class StringDescriptionWriter :  IDescription {
        private readonly StringWriter stringWriter = new StringWriter();


        /// <summary>
        /// Formats the given <paramref name="value"/> depending on null and the type of the value.
        /// </summary>
        /// <param name="value">The value to format.</param>
        /// <returns>Returns the formatted string.</returns>
        private string FormatValue(object value) {
            if (value == null)
            {
                return "null";
            }
            else if (value is string)
            {
                return FormatString((string) value);
            }
            else
            {
                return "<" + value + ">";
            }
        }

        /// <summary>
        /// Replaces backslashes with three escaped backslashes.
        /// </summary>
        /// <param name="s">The string to replace backslashes.</param>
        /// <returns>Returns the escaped string.</returns>
        private string FormatString(string s) {
            const string Quote = "\"";
            const string EscapedQuote = "\\\"";

            return Quote + s.Replace(Quote, EscapedQuote) + Quote;
        }

        public IDescription AppendText(string s) {
            stringWriter.Write(s);
            return this;
        }

        public IDescription AppendLine(string s) {
            AppendText(s);
            AppendText(Environment.NewLine);
            return this;
        }

        public IDescription AppendTextFormat(string format, params object[] args) {
            stringWriter.Write(format, args);
            return this;
        }

        public IDescription AppendValue(object value) {
            AppendText(FormatValue(value));
            return this;
        }

        public IDescription AppendNewLine() {
            stringWriter.WriteLine();
            return this;
        }

        public void AppendList<T>(string start, string seperator, string end, IEnumerable<T> selfDescribing) where T : ISelfDescribing {
            if(!selfDescribing.Any()) return;
            stringWriter.Write(start);
            foreach (var item in selfDescribing)
            {
                item.DescribeOn(this);
                stringWriter.Write(seperator);

            }
            stringWriter.Write(end);
        }

        public override string ToString() {
          return stringWriter.ToString();
        }
    }
}