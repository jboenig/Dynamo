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

namespace Headway.Dynamo.Metadata
{
	/// <summary>
	/// This class encapsulates integral data types.
	/// </summary>
    [Serializable]
	public sealed class IntegralType : DataType
	{
		#region Built-in Integral Types

		/// <summary>
		/// Integer.
		/// </summary>
		public static IntegralType Integer = new IntegralType(typeof(Int32));

		/// <summary>
		/// String.
		/// </summary>
		public static IntegralType String = new IntegralType(typeof(String));

		/// <summary>
		/// Float.
		/// </summary>
		public static IntegralType Float = new IntegralType(typeof(Single));

		/// <summary>
		/// Guid.
		/// </summary>
		public static IntegralType Guid = new IntegralType(typeof(Guid));

		/// <summary>
		/// Boolean.
		/// </summary>
		public static IntegralType Boolean = new IntegralType(typeof(Boolean));

		/// <summary>
		/// DateTime.
		/// </summary>
		public static IntegralType DateTime = new IntegralType(typeof(DateTime));

		/// <summary>
		/// Gets the <see cref="IntegralType"/> matching the specified CLR Type.
		/// </summary>
		/// <param name="clrType">
		/// CLR type for which to find the matching <see cref="IntegralType"/>.
		/// </param>
		/// <returns><see cref="IntegralType"/> object matching the specified Type</returns>
		public static IntegralType Get(Type clrType)
		{
			return Get(clrType.FullName);
		}

		/// <summary>
		/// Gets the <see cref="IntegralType"/> matching the specified fully-qualified name.
		/// </summary>
		/// <param name="fullTypeName">
		/// Name of type to get.
		/// </param>
		/// <returns><see cref="IntegralType"/> object matching the specified name</returns>
		public static IntegralType Get(string fullTypeName)
		{
			if (Integer.FullName.CompareTo(fullTypeName) == 0)
			{
				return Integer;
			}
			else if (String.FullName.CompareTo(fullTypeName) == 0)
			{
				return String;
			}
			else if (Float.FullName.CompareTo(fullTypeName) == 0)
			{
				return Float;
			}
			else if (String.FullName.CompareTo(fullTypeName) == 0)
			{
				return String;
			}
			else if (Guid.FullName.CompareTo(fullTypeName) == 0)
			{
				return Guid;
			}
			else if (Boolean.FullName.CompareTo(fullTypeName) == 0)
			{
				return Boolean;
			}
			else if (DateTime.FullName.CompareTo(fullTypeName) == 0)
			{
				return IntegralType.DateTime;
			}
			return null;
		}

		#endregion

		#region Member Variables

		private readonly Type dataType;
		private readonly object defaultValue;

		#endregion

		#region Constructors

		private IntegralType(Type dataType)
		{
			if (dataType == null)
			{
				throw new ArgumentNullException("dataType");
			}
			this.dataType = dataType;

			if (this.dataType.IsValueType && Nullable.GetUnderlyingType(this.dataType) == null)
			{
				this.defaultValue = Activator.CreateInstance(this.dataType);
			}
			else
			{
				this.defaultValue = null;
			}
		}

		private IntegralType(Type dataType, object defaultValue)
		{
			if (dataType == null)
			{
				throw new ArgumentNullException("dataType");
			}
			this.dataType = dataType;
			this.defaultValue = defaultValue;
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets the fully qualified name of the data type.
		/// </summary>
		public override string FullName
		{
			get { return this.dataType.FullName; }
		}

		/// <summary>
		/// Gets the fully qualified CLR type that this type maps to.
		/// </summary>
		public override string AssemblyQualifiedTypeName
		{
			get
			{
				return this.dataType.AssemblyQualifiedName;
			}
		}

		/// <summary>
		/// Gets a flag indicating whether or not this data type is enumerable.
		/// </summary>
		public override bool IsEnumerable
		{
			get { return false; }
		}

		/// <summary>
		/// Gets the <see cref="DataType"/> of items when the <see cref="IsEnumerable"/>
		/// flag is true.
		/// </summary>
		public override DataType ItemDataType
		{
			get { return null; }
		}

		/// <summary>
		/// Gets the CLR type that this type maps to.
		/// </summary>
		public override Type CLRType
		{
			get { return this.dataType; }
		}

		/// <summary>
		/// Gets the default value for the data type.
		/// </summary>
		public override object DefaultValue
		{
			get { return this.defaultValue; }
		}

		/// <summary>
		/// Gets the version number of this <see cref="DataType"/>.
		/// </summary>
		public override long VersionNumber
		{
			get { return -1; }
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
        public override bool IsAssignableFrom(DataType otherDataType)
		{
			if (otherDataType == null)
			{
				throw new ArgumentNullException("otherDataType");
			}
			var clrType = this.CLRType;
			return clrType.IsAssignableFrom(otherDataType.CLRType);
		}

		/// <summary>
		/// Gets the attribute associated with this data type matching the specified name.
		/// </summary>
		/// <typeparam name="T">Type of attribute.</typeparam>
		/// <param name="attributeName">Name of attribute.</param>
		/// <returns>Attribute matching the specified name.</returns>
		public override IEnumerable<T> GetAttributes<T>(string attributeName)
		{
			return new T[] { default(T) };
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

        #endregion

        #region Implementation

        /// <summary>
        /// Gets the <see cref="IMetadataService"/> that owns this data type.
        /// </summary>
        /// <returns></returns>
        //protected override IMetadataService GetMetadataService()
		//{
		//	return null;
		//}

		#endregion
	}
}