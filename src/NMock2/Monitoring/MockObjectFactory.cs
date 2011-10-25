//-----------------------------------------------------------------------
// <copyright file="MockObjectFactory.cs" company="NMock2">
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
namespace NMock2.Monitoring
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;
    using Internal;

    internal class MockObjectFactory
    {
        private static readonly Hashtable createdTypes = new Hashtable();
        private readonly ModuleBuilder moduleBuilder;

        /// <summary>
        /// Initializes a new instance of the <see cref="MockObjectFactory"/> class.
        /// </summary>
        /// <param name="name">The name of the assembly to generate.</param>
        public MockObjectFactory(string name)
        {
            AssemblyName assemblyName = new AssemblyName();
            assemblyName.Name = name;
            assemblyName.KeyPair = new StrongNameKeyPair(Properties.Resources.NMock2);
            this.moduleBuilder =
                AppDomain.CurrentDomain.DefineDynamicAssembly(
                    assemblyName, AssemblyBuilderAccess.Run).DefineDynamicModule(name);
        }

        /// <summary>
        /// Returns an array of <see langword="string"/>s that represent
        /// the names of the generic type parameter.
        /// </summary>
        /// <param name="args">The parameter info array.</param>
        /// <returns>An array containing parameter names.</returns>
        public static string[] GetGenericParameterNames(Type[] args)
        {
            string[] names = new string[args.Length];
            for (int i = 0; i < args.Length; i++)
            {
                names[i] = args[i].Name;
            }

            return names;
        }

        /// <summary>
        /// Returns an array of parameter <see cref="System.Type"/>s for the
        /// specified parameter info array.
        /// </summary>
        /// <param name="args">The parameter info array.</param>
        /// <returns>
        /// An array containing parameter <see cref="System.Type"/>s.
        /// </returns>
        public static Type[] GetParameterTypes(ParameterInfo[] args)
        {
            Type[] types = new Type[args.Length];
            for (int i = 0; i < args.Length; i++)
            {
                types[i] = args[i].ParameterType;
            }

            return types;
        }

        public MockObject CreateMockObject(
            Mockery mockery, Type mockedType, string name)
        {
            return
                Activator.CreateInstance(
                     this.GetMockedType(
                         Id(new Type[] { mockedType, typeof(IMockObject) }), mockedType),
                     new object[] { mockery, mockedType, name })
                 as MockObject;
        }

        private Type GetMockedType(TypeId id1, Type mockedType)
        {
            Type type1;
            if (createdTypes.ContainsKey(id1))
            {
                type1 = (Type)createdTypes[id1];
            }
            else
            {
                createdTypes[id1] =
                    type1 = this.CreateType("MockObjectType" + (createdTypes.Count + 1), mockedType);
            }

            return type1;
        }

        private Type CreateType(string typeName, Type mockedType)
        {
            TypeBuilder builder1 =
                this.moduleBuilder.DefineType(
                    typeName,
                    TypeAttributes.Public,
                    typeof(MockObject),
                    new Type[] { mockedType });
            BuildConstructor(builder1);
            BuildAllInterfaceMethods(mockedType, builder1);
            return builder1.CreateType();
        }

        private static bool AllTypes(Type type, object criteria)
        {
            return true;
        }

        private static void BuildAllInterfaceMethods(
            Type mockedType, TypeBuilder typeBuilder)
        {
            Type[] typeArray1 = mockedType.FindInterfaces(AllTypes, null);
            foreach (Type type1 in typeArray1)
            {
                BuildInterfaceMethods(typeBuilder, type1);
            }

            BuildInterfaceMethods(typeBuilder, mockedType);
        }

        private static void BuildConstructor(TypeBuilder typeBuilder)
        {
            Type[] typeArray1 =
                new Type[] { typeof(Mockery), typeof(Type), typeof(string) };

            ILGenerator generator1 =
                typeBuilder.DefineConstructor(
                    MethodAttributes.Public, CallingConventions.HasThis, typeArray1).
                    GetILGenerator();

            ConstructorInfo info1 =
                typeof(MockObject).GetConstructor(
                    BindingFlags.NonPublic | BindingFlags.Instance, null, typeArray1, null);

            generator1.Emit(OpCodes.Ldarg_0);
            generator1.Emit(OpCodes.Ldarg_1);
            generator1.Emit(OpCodes.Ldarg_2);
            generator1.Emit(OpCodes.Ldarg_3);
            generator1.Emit(OpCodes.Call, info1);
            generator1.Emit(OpCodes.Ret);
        }

        private static void BuildInterfaceMethods(TypeBuilder typeBuilder, Type mockedType)
        {
            typeBuilder.AddInterfaceImplementation(mockedType);
            MethodInfo[] infoArray1 = mockedType.GetMethods();
            foreach (MethodInfo info1 in infoArray1)
            {
                GenerateMethodBody(typeBuilder, info1);
            }
        }

        private static void EmitReferenceMethodBody(ILGenerator gen)
        {
            gen.Emit(OpCodes.Ldnull);
            gen.Emit(OpCodes.Ret);
        }

        private static void EmitValueMethodBody(MethodInfo method, ILGenerator gen)
        {
            gen.DeclareLocal(method.ReturnType);
            gen.Emit(OpCodes.Ldloc_0);
            gen.Emit(OpCodes.Ret);
        }

        private static void GenerateMethodBody(TypeBuilder typeBuilder, MethodInfo method)
        {
            MethodBuilder methodBuilder = DefineMethod(typeBuilder, method, false);
            DefineParameters(methodBuilder, method);
            ILGenerator generator1 = methodBuilder.GetILGenerator();
            generator1.Emit(OpCodes.Ldarg_0);

            if (method.ReturnType == null)
            {
                generator1.Emit(OpCodes.Ret);
            }

            if (method.ReturnType.IsValueType)
            {
                EmitValueMethodBody(method, generator1);
            }
            else
            {
                EmitReferenceMethodBody(generator1);
            }
        }

        private static TypeId Id(params Type[] types)
        {
            return new TypeId(types);
        }

        /// <summary>
        /// Defines proxy method for the target object.
        /// </summary>
        /// <param name="typeBuilder">The type builder.</param>
        /// <param name="method">The method to proxy.</param>
        /// <param name="explicitImplementation"><see langword="true"/> if the supplied <paramref name="intfMethod"/> is to be
        /// implemented explicitly; otherwise <see langword="false"/>.</param>
        /// <returns>
        /// The <see cref="System.Reflection.Emit.MethodBuilder"/> for the proxy method.
        /// </returns>
        /// <remarks>
        /// Original code from Spring.Net http://springnet.cvs.sourceforge.net/springnet/Spring.Net/src/Spring/Spring.Core/Proxy/AbstractProxyMethodBuilder.cs
        /// </remarks>
        private static MethodBuilder DefineMethod(TypeBuilder typeBuilder, MethodInfo method, bool explicitImplementation)
        {
            string name = method.Name;
            MethodAttributes attributes = MethodAttributes.Public | MethodAttributes.ReuseSlot
                                          | MethodAttributes.HideBySig | MethodAttributes.Virtual;

            if (method.IsSpecialName)
            {
                attributes |= MethodAttributes.SpecialName;
            }

            MethodBuilder methodBuilder = typeBuilder.DefineMethod(
                name,
                attributes,
                method.CallingConvention,
                method.ReturnType,
                GetParameterTypes(method.GetParameters()));

            if (method.IsGenericMethodDefinition)
            {
                Type[] genericArguments = method.GetGenericArguments();

                // define generic parameters
                GenericTypeParameterBuilder[] gtpBuilders =
                    methodBuilder.DefineGenericParameters(GetGenericParameterNames(genericArguments));

                // define constraints for each generic parameter
                for (int i = 0; i < genericArguments.Length; i++)
                {
                    gtpBuilders[i].SetGenericParameterAttributes(genericArguments[i].GenericParameterAttributes);

                    Type[] constraints = genericArguments[i].GetGenericParameterConstraints();
                    List<Type> interfaces = new List<Type>(constraints.Length);
                    foreach (Type constraint in constraints)
                    {
                        if (constraint.IsClass)
                        {
                            gtpBuilders[i].SetBaseTypeConstraint(constraint);
                        }
                        else
                        {
                            interfaces.Add(constraint);
                        }
                    }

                    gtpBuilders[i].SetInterfaceConstraints(interfaces.ToArray());
                }
            }

            return methodBuilder;
        }

        /// <summary>
        /// Defines method parameters based on proxied method metadata.
        /// </summary>
        /// <param name="methodBuilder">The <see cref="System.Reflection.Emit.MethodBuilder"/> to use.</param>
        /// <param name="method">The method to proxy.</param>
        private static void DefineParameters(MethodBuilder methodBuilder, MethodInfo method)
        {
            int n = 1;
            foreach (ParameterInfo param in method.GetParameters())
            {
                ParameterBuilder pb = methodBuilder.DefineParameter(n, param.Attributes, param.Name);
                n++;
            }
        }

        #region Nested type: TypeId

        private class TypeId
        {
            private readonly Type[] types;

            /// <summary>
            /// Initializes a new instance of the <see cref="TypeId"/> class.
            /// </summary>
            /// <param name="types">The types.</param>
            public TypeId(params Type[] types)
            {
                this.types = types;
            }

            public override bool Equals(object obj)
            {
                return (obj is TypeId) && this.ContainsSameTypesAs((TypeId)obj);
            }

            public override int GetHashCode()
            {
                int num1 = 0;
                foreach (Type type1 in this.types)
                {
                    num1 ^= type1.GetHashCode();
                }

                return num1;
            }

            private bool ContainsSameTypesAs(TypeId other)
            {
                if (other.types.Length != this.types.Length)
                {
                    return false;
                }

                for (int num1 = 0; num1 < this.types.Length; num1++)
                {
                    if (Array.IndexOf(other.types, this.types[num1]) < 0)
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