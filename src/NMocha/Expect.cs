//-----------------------------------------------------------------------
// <copyright file="Expect.cs" company="NMock2">
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
using NMocha.Internal;
using NMock2.Syntax;

namespace NMocha {
    
    public class Expect {
        public static IReceiverSyntax Never {
            get { return new ExpectationBuilder(Cardinality.Exactly(0)); }
        }

        public static IReceiverSyntax Once {
            get { return Exactly(1); }
        }

        public static IReceiverSyntax AtLeastOnce {
            get { return AtLeast(1); }
        }

      
        public static IReceiverSyntax Exactly(int count) {
            return new ExpectationBuilder(Cardinality.Exactly(count));
        }

        
        public static IReceiverSyntax AtLeast(int count) {
            return new ExpectationBuilder(Cardinality.AtLeast(count));
        }

        
        public static IReceiverSyntax AtMost(int count) {
            return new ExpectationBuilder(Cardinality.AtMost(count));
        }

        public static IReceiverSyntax Between(int minCount, int maxCount) {
            return new ExpectationBuilder(Cardinality.Between(minCount, maxCount));
        }

        public static IMethodSyntax On(object receiver) {
            return AtLeastOnce.On(receiver);
        }
    }
}