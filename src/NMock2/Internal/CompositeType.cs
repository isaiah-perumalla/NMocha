//-----------------------------------------------------------------------
// <copyright file="CompositeType.cs" company="NMock2">
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
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Text;
    
    /// <summary>
    /// Represents one or more types that are to be mocked. Provides operations
    /// that work over top of all the contained types, as well as a means of
    /// grouping and identifying unique combinations of types.
    /// </summary>
    /// <remarks>Duplicate types are ignored when added. Only interface and class types are
    /// supported, and there may only be a maximum of one class type per CompositeType instance.</remarks>
    public class CompositeType : IEquatable<CompositeType>
    {
        private static readonly Type[] EmptyTypeArray = new Type[0];

        private Type[] containedTypes;
        private Type[] additionalTypes = EmptyTypeArray;

        /// <summary>
        /// Initializes a new instance of the CompositeType class from the supplied types.
        /// </summary>
        /// <param name="types">The types to include in the CompositeType.</param>
        public CompositeType(Type[] types)
        {
            if (types.Length == 0)
            {
                throw new ArgumentException("At least one type must be specified.", "types");
            }

            this.Initialize(types);
        }

        /// <summary>
        /// Initializes a new instance of the CompositeType class from the supplied types.
        /// </summary>
        /// <param name="type">The first type to include in the CompositeType. This cannot be null.</param>
        /// <param name="additionalTypes">Zero or more further types to include in the CompositeType.</param>
        /// <remarks>This constructor is mostly included for convenience.</remarks>
        public CompositeType(Type type, Type[] additionalTypes)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            Type[] combinedTypes = new Type[additionalTypes.Length + 1];

            combinedTypes[0] = type;
            Array.Copy(additionalTypes, 0, combinedTypes, 1, additionalTypes.Length);

            this.Initialize(combinedTypes);
        }

        /// <summary>
        /// Gets the 'primary' type we are mocking. This may be a class or an interface
        /// and will determine the proxy generation method that will be used.
        /// </summary>
        public Type PrimaryType
        {
            get { return this.containedTypes[0]; }
        }

        /// <summary>
        /// Gets any additional types to be mocked. These will always be interfaces.
        /// </summary>
        public Type[] AdditionalInterfaceTypes
        {
            get { return this.additionalTypes; }
        }

        /// <summary>
        /// Gets any methods of the contained type(s) that match the specified matcher.
        /// </summary>
        /// <param name="matcher">The matcher.</param>
        /// <param name="firstMatchOnly">if set to <c>true</c> then only the first match is returned.</param>
        /// <returns>The methods of the contained type(s) that match the specified matcher.</returns>
        /// <remarks>Only non-private methods can be matched.</remarks>
        public IList<MethodInfo> GetMatchingMethods(Matcher matcher, bool firstMatchOnly)
        {
            List<MethodInfo> matches = new List<MethodInfo>();

            foreach (Type type in this.containedTypes)
            {
                if (type.IsInterface)
                {
                    foreach (Type implementedInterface in this.GetInterfacesImplementedByType(type))
                    {
                        foreach (MethodInfo method in implementedInterface.GetMethods())
                        {
                            if (matcher.Matches(method))
                            {
                                matches.Add(method);

                                if (firstMatchOnly)
                                {
                                    return matches;
                                }
                            }
                        }
                    }
                }
                else
                {
                    foreach (MethodInfo method in type.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
                    {
                        if (this.IsMethodVisible(method) && matcher.Matches(method))
                        {
                            matches.Add(method);

                            if (firstMatchOnly)
                            {
                                return matches;
                            }
                        }
                    }
                }
            }

            return matches;
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>An Int32 containing the hash code for this instance.</returns>
        public override int GetHashCode()
        {
            int hashCode = 23;

            foreach (Type type in this.containedTypes)
            {
                hashCode = (hashCode * 37) + type.GetHashCode();
            }

            return hashCode;
        }

        /// <summary>
        /// Determines whether the specified Object is equal to the current CompositeType.
        /// </summary>
        /// <param name="obj">The Object to compare with the current CompositeType.</param>
        /// <returns>true if the specified Object is equal to the current CompositeType; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            CompositeType other = obj as CompositeType;

            if (other == null)
            {
                return false;
            }

            return this.Equals(other);
        }

        /// <summary>
        /// Returns a String that represents the current CompositeType.
        /// </summary>
        /// <returns>A String that represents the current CompositeType.</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("{");

            for (int i = 0; i < this.containedTypes.Length; i++)
            {
                if (i > 0)
                {
                    sb.Append(", ");
                }

                sb.Append(this.containedTypes[i].Name);
            }

            return sb.Append("}").ToString();
        }

        #region IEquatable<CompositeTypeKey> Members

        /// <summary>
        /// Determines whether the specified CompositeType is equal to the current CompositeType.
        /// </summary>
        /// <param name="other">The CompositeType to compare with the current CompositeType.</param>
        /// <returns>true if the specified CompositeType is equal to the current CompositeType; otherwise, false.</returns>
        public bool Equals(CompositeType other)
        {
            if (other == null)
            {
                return false;
            }

            if (this.containedTypes.Length != other.containedTypes.Length)
            {
                return false;
            }

            for (int i = 0; i < this.containedTypes.Length; i++)
            {
                if (!this.containedTypes[i].Equals(other.containedTypes[i]))
                {
                    return false;
                }
            }

            return true;
        }

        #endregion

        /// <summary>
        /// Initializes the specified types.
        /// </summary>
        /// <param name="types">The types.</param>
        private void Initialize(Type[] types)
        {
            // We want the types to be in a consistent order regardless
            // of the order they were orginally supplied in. We also want
            // to remove any duplicates, identify invalid types and decide
            // on which type will be considered the 'primary' type.
            this.containedTypes = this.RationalizeTypes(this.CreateOrderedCopyOfTypes(types));

            if (this.containedTypes.Length > 1)
            {
                this.additionalTypes = new Type[this.containedTypes.Length - 1];
                Array.Copy(this.containedTypes, 1, this.additionalTypes, 0, this.additionalTypes.Length);
            }
        }

        private Type[] CreateOrderedCopyOfTypes(Type[] types)
        {
            Type[] orderedTypes = types.Clone() as Type[];

            Array.Sort<Type>(orderedTypes, (t1, t2) => string.CompareOrdinal(t1.AssemblyQualifiedName, t2.AssemblyQualifiedName));

            return orderedTypes;
        }

        private Type[] RationalizeTypes(Type[] types)
        {
            Type baseClass = null;
            List<Type> uniqueTypes = new List<Type>();

            foreach (Type type in types)
            {
                if (!type.IsInterface)
                {
                    if (type.IsClass)
                    {
                        if (type.IsSubclassOf(typeof(Delegate)))
                        {
                            throw new ArgumentException("Cannot mock delegates.", "types");
                        }

                        if (baseClass != null)
                        {
                            throw new ArgumentException("Cannot mock more than one class type in a single mock instance.", "types");
                        }

                        baseClass = type;
                        uniqueTypes.Insert(0, type);
                    }
                    else
                    {
                        throw new ArgumentException("Can only mock class and interface types. Invalid type was: " + type.Name, "types");
                    }
                }
                else
                {
                    if (!uniqueTypes.Contains(type))
                    {
                        uniqueTypes.Add(type);
                    }
                }
            }

            return uniqueTypes.ToArray();
        }

        /// <summary>
        /// Gets the interfaces implemented by the specified type.
        /// </summary>
        /// <param name="type">The interface type to inspect.</param>
        /// <returns>The interfaces implemented by the specified type.</returns>
        private Type[] GetInterfacesImplementedByType(Type type)
        {
            List<Type> implementedTypes = new List<Type>();
            
            foreach (Type implementedInterface in type.GetInterfaces())
            {
                implementedTypes.AddRange(this.GetInterfacesImplementedByType(implementedInterface));
            }

            implementedTypes.Add(type);

            return implementedTypes.ToArray();
        }

        /// <summary>
        /// Filters out private methods.
        /// </summary>
        /// <param name="methodInfo">The method to test for visibility.</param>
        /// <returns>True if the method is not private, otherwise false.</returns>
        private bool IsMethodVisible(MethodInfo methodInfo)
        {
            return methodInfo.IsPublic
                    || methodInfo.IsFamily
                    || methodInfo.IsAssembly
                    || methodInfo.IsFamilyOrAssembly;
        }
    }
}
