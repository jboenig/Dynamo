////////////////////////////////////////////////////////////////////////////////
// Copyright 2019 Jeff Boenig
//
// This file is part of Headway.Dynamo.
//
// Headway.Dynamo is free software: you can redistribute it and/or modify it under
// the terms of the GNU General Public License as published by the Free Software
// Foundation, either version 3 of the License, or (at your option) any later
// version.
//
// Headway.Dynamo is distributed in the hope that it will be useful, but WITHOUT
// ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
// FOR PARTICULAR PURPOSE. See the GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License along with
// Headway.Dynamo. If not, see http://www.gnu.org/licenses/.
////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using Headway.Dynamo.Reflection;

namespace Headway.Dynamo.Metadata.Reflection
{
	/// <summary>
	/// Implements an <see cref="ObjectType"/> based on
    /// reflection.
	/// </summary>
    [Serializable]
	public sealed class ReflectionObjectType : ObjectTypeBaseImpl, ISerializable
	{
		#region Member Variables

		private readonly Type clrType;
        private ObjectType derivesFrom;

        #endregion

        #region Constructors

        private ReflectionObjectType(IMetadataProvider metadataProvider, Type clrType) :
            base(metadataProvider)
		{
			if (clrType == null)
			{
				throw new ArgumentNullException("clrType");
			}
			this.clrType = clrType;
		}

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the fully qualified name of the object type.
        /// </summary>
        public override string FullName
		{
			get { return this.clrType.FullName; }
		}

		/// <summary>
		/// Gets the fully qualified CLR type that this type maps to.
		/// </summary>
		public override string AssemblyQualifiedTypeName
		{
			get { return this.clrType.AssemblyQualifiedName; }
		}

		/// <summary>
		/// Gets a reference to the base class for this class.
		/// </summary>
		public override ObjectType DerivesFrom
		{
			get
			{
                if (this.derivesFrom == null)
                {
                    var baseType = this.clrType.BaseType;
                    if (baseType != null)
                    {
                        this.derivesFrom = this.MetadataProvider.GetDataType<ObjectType>(baseType.FullName);
                    }
                }
                return this.derivesFrom;
			}
		}

		/// <summary>
		/// Gets the default value for the data type.
		/// </summary>
		public override object DefaultValue
		{
			get { return null; }
		}

		/// <summary>
		/// Gets the version number of this <see cref="DataType"/>.
		/// </summary>
		public override long VersionNumber
		{
			get { return -1; }
		}

		/// <summary>
		/// Gets the collection of properties associated with this class.
		/// </summary>
		public override IEnumerable<Property> Properties
		{
			get
			{
				return this.GetReflectionProperties();
			}
		}

        /// <summary>
        /// Gets the CLR type that this type maps to.
        /// </summary>
        public override Type CLRType
        {
            get
            {
                return this.clrType;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="attributeName"></param>
        /// <returns></returns>
        public override IEnumerable<T> GetAttributes<T>(string attributeName)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Gets a list of the names of all available attributes.
		/// </summary>
		/// <returns>Enumerable collection of attribute names.</returns>
		public override IEnumerable<string> GetAttributeNames()
		{
			return new string[] { };
		}

		/// <summary>
		/// Gets the list of <see cref="DataType"/>s that this data type depends on.
		/// </summary>
		/// <param name="dependencies">
		/// Collection of <see cref="DataType"/>s that this data type depends on.
		/// </param>
		public override void GetDependencies(IList<DataType> dependencies)
		{
		}

        /// <summary>
        /// Adds a property to this type.
        /// </summary>
        /// <param name="name">Name of property to add</param>
        /// <param name="dataType">Data type of property to add</param>
        /// <returns>Returns the new property created</returns>
        public override Property AddProperty(string name, DataType dataType)
        {
            throw new InvalidOperationException("AddProperty is not supported for ReflectionObjectType");
        }

        #endregion

        #region Static Get Methods

        // Static cache of reflection object types
        private static readonly Dictionary<string, ObjectType> classCache = new Dictionary<string, ObjectType>();

        /// <summary>
        /// Gets an <see cref="ObjectType"/> matching the given fully
        /// qualified CLR class name.
        /// </summary>
        /// <param name="metadataProvider">
        /// Reference to <see cref="IMetadataProvider"/> service used
        /// to resolve data types.
        /// </param>
        /// <param name="fullName">
        /// Fully qualified name of the CLR class.
        /// </param>
        /// <returns>
        /// Returns an instance of <see cref="ObjectType"/> for
        /// the specified CLR class.
        /// </returns>
        internal static ObjectType Get(IMetadataProvider metadataProvider,
            string fullName)
		{
			return ReflectionObjectType.Get(metadataProvider, fullName,
                new DefaultAssemblyLoader());
		}

        /// <summary>
        /// Gets an <see cref="ObjectType"/> matching the given fully
        /// qualified CLR class name and assembly loader.
        /// </summary>
        /// <param name="metadataProvider">
        /// </param>
        /// <param name="fullName">
        /// Fully qualified name of the CLR class.
        /// </param>
        /// <param name="assemblyLoader">
        /// Assembly loader used to get available assemblies in
        /// which to search for the class.
        /// </param>
        /// <returns>
        /// Returns an instance of <see cref="ObjectType"/> for
        /// the specified CLR class.
        /// </returns>
        internal static ObjectType Get(IMetadataProvider metadataProvider,
            string fullName,
            IAssemblyLoader assemblyLoader)
		{
			if (string.IsNullOrEmpty(fullName))
			{
				throw new ArgumentNullException("fullName");
			}

			if (assemblyLoader == null)
			{
                //assemblyLoader = new DefaultAssemblyLoader();
                throw new ArgumentNullException("assemblyLoader");
			}

			if (classCache.ContainsKey(fullName))
			{
				return classCache[fullName];
			}

            // Performance optimization! Give preferential treatment to the Abp.Runtime
            // assembly since it is hit frequently.
            var objType = LoadObjectTypeFromAssembly(metadataProvider, typeof(ObjectType).Assembly, fullName);
			if (objType != null)
			{
				if (!classCache.ContainsKey(fullName))
				{
					classCache.Add(fullName, objType);
				}
				return objType;
			}

			// Now search through all available assemblies using the given assembly loader
			foreach (var curAssembly in assemblyLoader.GetAllAssemblies())
			{
				// Skip Abp.Runtime assembly since we already looked there
				if (curAssembly != typeof(ObjectType).Assembly)
				{
					objType = LoadObjectTypeFromAssembly(metadataProvider, curAssembly, fullName);
					if (objType != null)
					{
						if (!classCache.ContainsKey(fullName))
						{
							classCache.Add(fullName, objType);
						}
						return objType;
					}
				}
			}

			return null;
		}

        /// <summary>
        /// Gets an <see cref="ObjectType"/> given a CLR type.
        /// </summary>
        /// <param name="metadataProvider">
        /// </param>
        /// <param name="clrType">
        /// CLR type information
        /// </param>
        /// <returns>
        /// Returns an instance of <see cref="ObjectType"/> for
        /// the specified CLR class.
        /// </returns>
        internal static ObjectType Get(IMetadataProvider metadataProvider,
            Type clrType)
        {
            if (clrType == null)
            {
                throw new ArgumentNullException(nameof(clrType));
            }

            var fullName = clrType.FullName;

            if (classCache.ContainsKey(fullName))
            {
                return classCache[fullName];
            }

            return new ReflectionObjectType(metadataProvider, clrType);
        }

        #endregion

        #region Serialization

        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Implementation

        private static ObjectType LoadObjectTypeFromAssembly(IMetadataProvider metadataProvider, Assembly assembly, string objTypeName)
		{
			if (string.IsNullOrEmpty(objTypeName))
			{
				throw new ArgumentNullException("objTypeName");
			}

			if (assembly == null)
			{
				throw new ArgumentNullException("assembly");
			}

			var dataType = assembly.GetType(objTypeName);

			if (dataType != null && dataType.IsClass && !dataType.IsGenericType)
			{
				return new ReflectionObjectType(metadataProvider, dataType);
			}

			return null;
		}

        #endregion
	}
}
