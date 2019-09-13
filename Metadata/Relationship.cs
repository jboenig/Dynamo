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

using Headway.Dynamo.Runtime;

namespace Headway.Dynamo.Metadata
{
	/// <summary>
	/// Encapsulates an relationship between two <see cref="ObjectType"/> objects.
	/// </summary>
	public abstract class Relationship : INamedObject
	{
		/// <summary>
		/// Gets the name of the relationship.
		/// </summary>
		public abstract string Name
		{
			get;
		}

		/// <summary>
		/// Gets the namespace of the relationship.
		/// </summary>
		public abstract string Namespace
		{
			get;
		}

		/// <summary>
		/// Gets the fully qualified name of the relationship.
		/// </summary>
		public abstract string FullName
		{
			get;
		}

		/// <summary>
		/// Gets the <see cref="ObjectType"/> that is the source of the relationship.
		/// </summary>
		public abstract ObjectType Source
		{
			get;
		}

		/// <summary>
		/// Gets the <see cref="ObjectType"/> that is the target of the relationship.
		/// </summary>
		public abstract ObjectType Target
		{
			get;
		}

		/// <summary>
		/// Gets the cardinality of the relationship.
		/// </summary>
		public abstract RelationshipCardinality Cardinality
		{
			get;
		}

		/// <summary>
		/// Gets an attribute of the relationship by name.
		/// </summary>
		/// <typeparam name="T">Type of attribute to return.</typeparam>
		/// <param name="attributeName">Name of attribute to return.</param>
		/// <returns>Attribute matching the specified name or null if it is not found.</returns>
		public abstract T GetAttribute<T>(string attributeName);
	}
}
