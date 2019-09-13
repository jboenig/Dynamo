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
	/// Encapsulates a <see cref="DataType"/> that a user can create
	/// dynamically.
	/// </summary>
	public sealed class UserDefinedDataType : DataType
	{
		/// <summary>
		/// Gets the full name.
		/// </summary>
		public override string FullName
		{
			get { throw new NotImplementedException(); }
		}

		/// <summary>
		/// Gets the assembly qualified name.
		/// </summary>
		public override string AssemblyQualifiedTypeName
		{
			get { throw new NotImplementedException(); }
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
			get
			{
				return null;
			}
		}

		/// <summary>
		/// Gets the CLR type.
		/// </summary>
		public override Type CLRType
		{
			get { throw new NotImplementedException(); }
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
		/// Determines if this class is assignable from the specified <see cref="DataType"/>.
		/// </summary>
		/// <param name="otherDataType"><see cref="DataType"/> to test.</param>
		/// <returns>
		/// Returns true if the specified <see cref="DataType"/> is compatible with this
		/// <see cref="ObjectType"/>, otherwise returns false.
		/// </returns>
		public override bool IsAssignableFrom(DataType otherDataType)
		{
			throw new NotImplementedException();
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

		#region Implementation

		/// <summary>
		/// Gets the <see cref="IMetadataProvider"/> that owns this data type.
		/// </summary>
		/// <returns></returns>
		//protected override IMetadataProvider GetMetadataProvider()
		//{
		//	return null;
		//}

		#endregion
	}
}
