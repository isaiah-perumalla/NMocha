//-----------------------------------------------------------------------
// <copyright file="VisibilityAcceptanceTest.cs" company="NMock2">
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
// This is the easiest way to ignore StyleCop rules on this file, even if we shouldn't use this tag:
// <auto-generated />
//-----------------------------------------------------------------------

#region InternalsVisibleToAttribute declaration

// This will apply to the whole assembly, but as it is only really
// relevant for this fixture, it is being kept alongside it...

// For CastleMockObjectFactory:
using System.Runtime.CompilerServices;
using NUnit.Framework;

[assembly:
    InternalsVisibleTo(
        "DynamicProxyGenAssembly2, PublicKey=0024000004800000940000000602000000240000525341310004000001000100c547cac37abd99c8db225ef2f6c8a3602f3b3606cc9891605d02baa56104f4cfc0734aa39b93bf7852f7d9266654753cc297e7d2edfe0bac1cdcf9f717241550e0a7b191195b7667bb4f64bcb8e2121380fd1d9d46ad2d92d2d15605093924cceaf74c4861eff62abf69b9291ed0a340e113be11e6a7d3113e92484cf7045cc7"
        )]

#endregion

namespace NMock2.AcceptanceTests {
    [TestFixture, Explicit("require type to be strong named")]
    public class VisibilityAcceptanceTest : AcceptanceTestBase {
        // Note: For the internal visibility tests, the assembly containing the internal
        // types must be strong-named and decorated with the InternalsVisibleToAttribute,
        // identifying the DynamicProxy2 runtime-generated assembly (DynamicProxyGenAssembly2).

        // Ideally this fixture should also work with the legacy InterfaceOnlyMockObjectFactory
        // implementation, but unfortunately, one of the two generated assemblies is not
        // strong-named. This prevents us using the InternalsVisibleToAttribute as we have
        // for the Castle implementation. For now we just exclude the affected test cases
        // for this scenario by decorating them with the CastleOnlyAttribute category.

        internal interface ISomeInternalInterface {
            void DoWork();
        }

        internal class SomeInternalClass {
            public virtual void DoWork() {
            }
        }

        public class SomeClassWithInternalMembers {
            internal virtual void DoWork() {
            }
        }

        protected internal interface ISomeProtectedInternalInterface {
            void DoWork();
        }

        protected internal class SomeProtectedInternalClass {
            public virtual void DoWork() {
            }
        }

        public class SomeClassWithProtectedInternalMembers {
            protected internal virtual void DoWork() {
            }
        }

        [Test, Class]
        public void CanMockClassWithInternalMembers() {
            var mock = Mockery.NewInstanceOfRole<SomeClassWithInternalMembers>();
            Expect.Once.On(mock).Message("DoWork");
            mock.DoWork();
        }

        [Test, Class]
        public void CanMockClassWithProtectedInternalMembers() {
            var mock = Mockery.NewInstanceOfRole<SomeClassWithProtectedInternalMembers>();
            Expect.Once.On(mock).Message("DoWork");
            mock.DoWork();
        }

        [Test, Class]
        public void CanMockInternalClass() {
            var mock = Mockery.NewInstanceOfRole<SomeInternalClass>();
            Expect.Once.On(mock).Message("DoWork");
            mock.DoWork();
        }

        [Test, CastleOnly]
        public void CanMockInternalInterface() {
            var mock = Mockery.NewInstanceOfRole<ISomeInternalInterface>();
            Expect.Once.On(mock).Message("DoWork");
            mock.DoWork();
        }

        [Test, Class]
        public void CanMockProtectedInternalClass() {
            var mock = Mockery.NewInstanceOfRole<SomeProtectedInternalClass>();
            Expect.Once.On(mock).Message("DoWork");
            mock.DoWork();
        }

        [Test, CastleOnly]
        public void CanMockProtectedInternalInterface() {
            var mock = Mockery.NewInstanceOfRole<ISomeProtectedInternalInterface>();
            Expect.Once.On(mock).Message("DoWork");
            mock.DoWork();
        }
    }
}