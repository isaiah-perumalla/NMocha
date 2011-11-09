//-----------------------------------------------------------------------
// <copyright file="InterfaceOnlyMockObjectFactory.cs" company="NMock2">
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
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using NMocha.Internal;
using NMocha.Monitoring;

namespace NMock2.Monitoring {
    /// <summary>
    /// Class that creates mocks for interfaces only. This was the original implementation
    /// of NMock2 mocks used before the Castle proxies were introduced.
    /// </summary>
    public class InterfaceOnlyMockObjectFactory : IMockObjectFactory {
        private static readonly Hashtable createdTypes = new Hashtable();
        private static readonly MultiInterfaceFactory facadeFactory = new MultiInterfaceFactory("Mocks");
        private readonly ModuleBuilder moduleBuilder;

        /// <summary>
        /// Initializes a new instance of the <see cref="InterfaceOnlyMockObjectFactory"/> class.
        /// </summary>
        public InterfaceOnlyMockObjectFactory() {
            string name = "MockObjects";
            var name1 = new AssemblyName();
            name1.Name = name;
            moduleBuilder =
                AppDomain.CurrentDomain.DefineDynamicAssembly(
                    name1, AssemblyBuilderAccess.Run).DefineDynamicModule(name);
        }

        #region IMockObjectFactory Members

        /// <summary>
        /// Creates a mock of the specified type(s).
        /// </summary>
        /// <param name="mockery">The mockery used to create this mock instance.</param>
        /// <param name="typesToMock">The type(s) to include in the mock.</param>
        /// <param name="name">The name to use for the mock instance.</param>
        /// <param name="constructorArgs">Constructor arguments for the class to be mocked. Only valid if mocking a class type.</param>
        /// <returns>
        /// A mock instance of the specified type(s).
        /// </returns>
        public object CreateMock(Mockery mockery, CompositeType typesToMock, string name, object[] constructorArgs) {
            Type mockedType = typesToMock.PrimaryType;

            if (mockedType.IsClass)
            {
                throw new NotSupportedException(GetType().Name + " does not support mocking of classes.");
            }

            if (typesToMock.AdditionalInterfaceTypes.Length > 0)
            {
                throw new NotSupportedException(GetType().Name + " does not support mocking of multiple interfaces.");
            }

            Type facadeType = facadeFactory.GetType(typeof (IMockObject), mockedType);

            var mockObject =
                Activator.CreateInstance(
                    GetMockedType(
                        Id(new[] {mockedType, typeof (IMockObject)}), mockedType),
                    new object[] {mockery, mockedType, name})
                as MockObject;

            var adapter =
                new ProxyInvokableAdapter(
                    facadeType,
                    new ProxiedObjectIdentity(mockObject, new Invoker(typeof (IMockObject), mockObject, mockObject)));

            return adapter.GetTransparentProxy();
        }

        #endregion

        /// <summary>
        /// Returns an array of <see langword="string"/>s that represent
        /// the names of the generic type parameter.
        /// </summary>
        /// <param name="args">The parameter info array.</param>
        /// <returns>An array containing parameter names.</returns>
        public static string[] GetGenericParameterNames(Type[] args) {
            var names = new string[args.Length];
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
        public static Type[] GetParameterTypes(ParameterInfo[] args) {
            var types = new Type[args.Length];
            for (int i = 0; i < args.Length; i++)
            {
                types[i] = args[i].ParameterType;
            }

            return types;
        }

        private static bool AllTypes(Type type, object criteria) {
            return true;
        }

        private static void BuildAllInterfaceMethods(
            Type mockedType, TypeBuilder typeBuilder) {
            Type[] typeArray1 = mockedType.FindInterfaces(AllTypes, null);
            foreach (Type type1 in typeArray1)
            {
                BuildInterfaceMethods(typeBuilder, type1);
            }

            BuildInterfaceMethods(typeBuilder, mockedType);
        }

        private static void BuildConstructor(TypeBuilder typeBuilder) {
            var typeArray1 =
                new[] {typeof (Mockery), typeof (Type), typeof (string)};

            ILGenerator generator1 =
                typeBuilder.DefineConstructor(
                    MethodAttributes.Public, CallingConventions.HasThis, typeArray1).
                    GetILGenerator();

            ConstructorInfo info1 =
                typeof (MockObject).GetConstructor(
                    BindingFlags.NonPublic | BindingFlags.Instance, null, typeArray1, null);

            generator1.Emit(OpCodes.Ldarg_0);
            generator1.Emit(OpCodes.Ldarg_1);
            generator1.Emit(OpCodes.Ldarg_2);
            generator1.Emit(OpCodes.Ldarg_3);
            generator1.Emit(OpCodes.Call, info1);
            generator1.Emit(OpCodes.Ret);
        }

        private static void BuildInterfaceMethods(TypeBuilder typeBuilder, Type mockedType) {
            typeBuilder.AddInterfaceImplementation(mockedType);
            MethodInfo[] infoArray1 = mockedType.GetMethods();
            foreach (MethodInfo info1 in infoArray1)
            {
                GenerateMethodBody(typeBuilder, info1);
            }
        }

        private static void EmitReferenceMethodBody(ILGenerator gen) {
            gen.Emit(OpCodes.Ldnull);
            gen.Emit(OpCodes.Ret);
        }

        private static void EmitValueMethodBody(MethodInfo method, ILGenerator gen) {
            gen.DeclareLocal(method.ReturnType);
            gen.Emit(OpCodes.Ldloc_0);
            gen.Emit(OpCodes.Ret);
        }

        private static void GenerateMethodBody(TypeBuilder typeBuilder, MethodInfo method) {
            MethodBuilder methodBuilder = DefineMethod(typeBuilder, method, false);
            DefineParameters(methodBuilder, method);
            ILGenerator generator1 = methodBuilder.GetILGenerator();

            ////ILGenerator generator1 = PrepareMethodGenerator(typeBuilder, method);
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

        private static TypeId Id(params Type[] types) {
            return new TypeId(types);
        }

        /// <summary>
        /// Defines proxy method for the target object.
        /// </summary>
        /// <param name="typeBuilder">The type builder.</param>
        /// <param name="method">The method to proxy.</param>
        /// <param name="explicitImplementation"><see langword="true"/> if the supplied <paramref name="method"/> is to be
        /// implemented explicitly; otherwise <see langword="false"/>.</param>
        /// <returns>
        /// The <see cref="System.Reflection.Emit.MethodBuilder"/> for the proxy method.
        /// </returns>
        /// <remarks>
        /// Original code from Spring.Net http://springnet.cvs.sourceforge.net/springnet/Spring.Net/src/Spring/Spring.Core/Proxy/AbstractProxyMethodBuilder.cs?revision=1.6&view=markup
        /// </remarks>
        private static MethodBuilder DefineMethod(
            TypeBuilder typeBuilder,
            MethodInfo method,
            bool explicitImplementation) {
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
                    var interfaces = new List<Type>(constraints.Length);
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
        private static void DefineParameters(MethodBuilder methodBuilder, MethodInfo method) {
            int n = 1;
            foreach (ParameterInfo param in method.GetParameters())
            {
                ParameterBuilder pb = methodBuilder.DefineParameter(n, param.Attributes, param.Name);
                n++;
            }
        }

        private Type CreateType(string typeName, Type mockedType) {
            TypeBuilder builder1 =
                moduleBuilder.DefineType(
                    typeName,
                    TypeAttributes.Public,
                    typeof (MockObject),
                    new[] {mockedType});
            BuildConstructor(builder1);
            BuildAllInterfaceMethods(mockedType, builder1);
            return builder1.CreateType();
        }

        private Type GetMockedType(TypeId id1, Type mockedType) {
            Type type1;
            if (createdTypes.ContainsKey(id1))
            {
                type1 = (Type) createdTypes[id1];
            }
            else
            {
                createdTypes[id1] =
                    type1 = CreateType("MockObjectType" + (createdTypes.Count + 1), mockedType);
            }

            return type1;
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

            public override bool Equals(object obj) {
                return (obj is TypeId) && ContainsSameTypesAs((TypeId) obj);
            }

            public override int GetHashCode() {
                int num1 = 0;
                foreach (Type type1 in types)
                {
                    num1 ^= type1.GetHashCode();
                }

                return num1;
            }

            private bool ContainsSameTypesAs(TypeId other) {
                if (other.types.Length != types.Length)
                {
                    return false;
                }

                for (int num1 = 0; num1 < types.Length; num1++)
                {
                    if (Array.IndexOf(other.types, types[num1]) < 0)
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