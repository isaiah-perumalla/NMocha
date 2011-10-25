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
namespace NMock2.Internal
{
    using System.IO;

    /// <summary>
    /// Used to describe Matchers and other classes for exception handling.
    /// </summary>
    public class DescriptionWriter : StringWriter
    {
        /// <summary>
        /// Writes the text representation of an object to the text stream by calling ToString on that object.
        /// </summary>
        /// <param name="value">The object to write.</param>
        /// <exception cref="T:System.ObjectDisposedException">
        /// The <see cref="T:System.IO.TextWriter"/> is closed.
        /// </exception>
        /// <exception cref="T:System.IO.IOException">
        /// An I/O error occurs.
        /// </exception>
        public override void Write(object value)
        {
            this.Write(this.FormatValue(value));
        }

        /// <summary>
        /// Formats the given <paramref name="value"/> depending on null and the type of the value.
        /// </summary>
        /// <param name="value">The value to format.</param>
        /// <returns>Returns the formatted string.</returns>
        private string FormatValue(object value)
        {
            if (value == null)
            {
                return "null";
            }
            else if (value is string)
            {
                return this.FormatString((string)value);
            }
            else
            {
                return "<" + value.ToString() + ">";
            }
        }

        /// <summary>
        /// Replaces backslashes with three escaped backslashes.
        /// </summary>
        /// <param name="s">The string to replace backslashes.</param>
        /// <returns>Returns the escaped string.</returns>
        private string FormatString(string s)
        {
            const string Quote = "\"";
            const string EscapedQuote = "\\\"";

            return Quote + s.Replace(Quote, EscapedQuote) + Quote;
        }
    }
}
