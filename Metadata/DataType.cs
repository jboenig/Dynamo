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
using Headway.Dynamo.Runtime;

namespace Headway.Dynamo.Metadata
{
    /// <summary>
    /// Base class for data types.
    /// </summary>
	public abstract class DataType : INamedObject, IAttributeContainer
	{
		#region Public Properties

		/// <summary>
		/// Gets the name of the data type.
		/// </summary>
		public string Name
		{
            get { return NameHelpers.GetName(this.FullName); }
        }

        /// <summary>
        /// Gets the namespace of the data type.
        /// </summary>
        public string Namespace
		{
            get { return NameHelpers.GetNamespace(this.FullName); }
        }

        /// <summary>
        /// Gets the fully qualified name of the data type.
        /// </summary>
        public abstract string FullName
		{
			get;
		}

		/// <summary>
		/// Gets the fully qualified CLR type name.
		/// </summary>
		public abstract string AssemblyQualifiedTypeName
		{
			get;
		}

		/// <summary>
		/// Gets a flag indicating whether or not this data type is enumerable.
		/// </summary>
		public abstract bool IsEnumerable
		{
			get;
		}

		/// <summary>
		/// Gets the <see cref="DataType"/> of items when the <see cref="IsEnumerable"/>
		/// flag is true.
		/// </summary>
		public abstract DataType ItemDataType
		{
			get;
		}

		/// <summary>
		/// Gets the name of the assembly in which the <see cref="CLRType"/> is compiled.
		/// </summary>
		public string AssemblyName
		{
			get
			{
				var typeAssemblyQualifiedName = this.AssemblyQualifiedTypeName;
				if (!string.IsNullOrEmpty(typeAssemblyQualifiedName))
				{
					var indexOfFirstComma = typeAssemblyQualifiedName.IndexOf(',', 0);
					if (indexOfFirstComma >= 0 && indexOfFirstComma + 1 < typeAssemblyQualifiedName.Length)
					{
						return typeAssemblyQualifiedName.Substring(indexOfFirstComma + 1).Trim();
					}
				}
				return null;
			}
		}

		/// <summary>
		/// Gets the name of the CLR type sans the assembly information.
		/// </summary>
		public string CLRTypeName
		{
			get
			{
				var typeAssemblyQualifiedName = this.AssemblyQualifiedTypeName;
				if (!string.IsNullOrEmpty(typeAssemblyQualifiedName))
				{
					var indexOfFirstComma = typeAssemblyQualifiedName.IndexOf(',', 0);
					if (indexOfFirstComma > 0)
					{
						return typeAssemblyQualifiedName.Substring(0, indexOfFirstComma).Trim();
					}
				}
				return null;
			}
		}

		/// <summary>
		/// Gets the CLR type that this type maps to.
		/// </summary>
		public abstract Type CLRType
		{
			get;
		}

		/// <summary>
		/// Gets the default value for the data type.
		/// </summary>
		public abstract object DefaultValue
		{
			get;
		}

		/// <summary>
		/// Gets the version number of this <see cref="DataType"/>.
		/// </summary>
		public abstract long VersionNumber
		{
			get;
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Determines if this class is assignable from the specified <see cref="DataType"/>.
		/// </summary>
		/// <param name="otherDataType"><see cref="DataType"/> to test.</param>
		/// <returns>
		/// Returns true if the specified <see cref="DataType"/> is compatible with this
		/// <see cref="ObjectType"/>, otherwise returns false.
		/// </returns>
		public abstract bool IsAssignableFrom(DataType otherDataType);

		/// <summary>
		/// Gets the attributes associated with this data type matching the specified name.
		/// </summary>
		/// <typeparam name="T">Type of attribute.</typeparam>
		/// <param name="attributeName">Name of attribute.</param>
		/// <returns>Attribute matching the specified name.</returns>
		public abstract IEnumerable<T> GetAttributes<T>(string attributeName) where T : Attribute;

		/// <summary>
		/// Gets a list of the names of all available attributes.
		/// </summary>
		/// <returns>Enumerable collection of attribute names.</returns>
		public abstract IEnumerable<string> GetAttributeNames();

		/// <summary>
		/// Gets the list of <see cref="DataType"/>s that this data type depends on.
		/// </summary>
		/// <param name="dependencies">
		/// Collection of <see cref="DataType"/>s that this data type depends on.
		/// </param>
		public abstract void GetDependencies(IList<DataType> dependencies);

        #endregion

        #region Protected Methods

        /// <summary>
        /// Gets the <see cref="IMetadataService"/> that owns this data type.
        /// </summary>
        /// <returns></returns>
        //protected abstract IMetadataService GetMetadataService();

		#endregion
	}
}
