//-----------------------------------------------------------------------
// <copyright file="MultiInterfaceFactory.cs" company="NMock2">
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
using System.Collections;
using System.Reflection;
using System.Reflection.Emit;

namespace NMock2.Monitoring {
    public class MultiInterfaceFactory {
        private static readonly Hashtable createdTypes = new Hashtable();
        private readonly ModuleBuilder moduleBuilder;

        /// <summary>
        /// Initializes a new instance of the <see cref="MultiInterfaceFactory"/> class.
        /// </summary>
        /// <param name="name">The name of the assembly to generate.</param>
        public MultiInterfaceFactory(string name) {
            var assemblyName = new AssemblyName();
            assemblyName.Name = name;

            AssemblyBuilder assemblyBuilder =
                AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
            moduleBuilder = assemblyBuilder.DefineDynamicModule(name);
        }

        public Type GetType(params Type[] baseInterfaces) {
            TypeId id = Id(baseInterfaces);
            if (createdTypes.ContainsKey(id))
            {
                return (Type) createdTypes[id];
            }
            else
            {
                string typeName = "MultiInterface" + (createdTypes.Count + 1);
                Type newType = CreateType(typeName, baseInterfaces);
                createdTypes[id] = newType;
                return newType;
            }
        }

        private Type CreateType(string typeName, Type[] baseInterfaces) {
            TypeBuilder typeBuilder = moduleBuilder.DefineType(
                typeName,
                TypeAttributes.Public | TypeAttributes.Abstract | TypeAttributes.Interface,
                null,
                baseInterfaces);

            return typeBuilder.CreateType();
        }

        private TypeId Id(params Type[] types) {
            return new TypeId(types);
        }

        #region Nested type: TypeId

        private class TypeId {
            private readonly Type[] types;

            /// <summary>
            /// Initializes a new instance of the <see cref="TypeId"/> class.
            /// </summary>
            /// <param name="types">The types.</param>
            public TypeId(params Type[] types) {
                this.types = types;
            }

            public override int GetHashCode() {
                int hashCode = 0;
                foreach (Type type in types)
                {
                    hashCode ^= type.GetHashCode();
                }

                return hashCode;
            }

            public override bool Equals(object obj) {
                return obj is TypeId
                       && ContainsSameTypesAs((TypeId) obj);
            }

            private bool ContainsSameTypesAs(TypeId other) {
                if (other.types.Length != types.Length)
                {
                    return false;
                }

                for (int i = 0; i < types.Length; i++)
                {
                    if (Array.IndexOf(other.types, types[i]) < 0)
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        #endregion
    }
}