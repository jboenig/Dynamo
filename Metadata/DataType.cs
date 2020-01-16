////////////////////////////////////////////////////////////////////////////////
// The MIT License(MIT)
// Copyright(c) 2020 Jeff Boenig
// This file is part of Headway.Dynamo
//
// Permission is hereby granted, free of charge, to any person obtaining a
// copy of this software and associated documentation files (the "Software"),
// to deal in the Software without restriction, including without limitation
// the rights to use, copy, modify, merge, publish, distribute, sublicense,
// and/or sell copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included
// in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS
// OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY
// CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE
// SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
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

#if false
        /// <summary>
        /// Gets the <see cref="IMetadataService"/> that owns this data type.
        /// </summary>
        /// <returns></returns>
        //protected abstract IMetadataService GetMetadataService();
#endif

        #endregion
    }
}
