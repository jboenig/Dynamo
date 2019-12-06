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

namespace Headway.Dynamo.Metadata
{
	/// <summary>
	/// Cardinality for relationships.
	/// </summary>
	public enum RelationshipCardinality
	{
		/// <summary>
		/// Unknown or not set.
		/// </summary>
		None = 0,

		/// <summary>
		/// One to one relationship.
		/// </summary>
		OneToOne = 1,

		/// <summary>
		/// One to many relationship.
		/// </summary>
		OneToMany = 2,

		/// <summary>
		/// Many to one relationship.
		/// </summary>
		ManyToOne = 3,

		/// <summary>
		/// Many to many relationship.
		/// </summary>
		ManyToMany = 4
	}
}
