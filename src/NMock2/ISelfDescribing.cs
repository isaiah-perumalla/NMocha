//-----------------------------------------------------------------------
// <copyright file="ISelfDescribing.cs" company="NMock2">
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
using System.Collections.Generic;
using System.IO;

namespace NMock2 {
    /// <summary>
    /// This interface is used to get a description of the implementator.
    /// </summary>
    public interface ISelfDescribing {
        /// <summary>
        /// Describes this object.
        /// </summary>
        /// <param name="description"></param>
        void DescribeOn(IDescription description);
    }

    public interface IDescription {
        IDescription AppendText(string s);
        IDescription AppendLine(string s);
        IDescription AppendTextFormat(string format, params object[] args);
        IDescription AppendValue(object value);
        IDescription AppendNewLine();
        void AppendList<T>(string start, string seperator, string end, IEnumerable<T> selfDescribing) where  T: ISelfDescribing;
    }
}