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

namespace Headway.Dynamo.Metadata
{
	/// <summary>
	/// Encapsulates a property that can be used to navigate
	/// a <see cref="Relationship"/>.
	/// </summary>
	public abstract class NavigationProperty : Property
	{
		/// <summary>
		/// Gets the <see cref="Relationship"/> bound to this property.
		/// </summary>
		public abstract Relationship Relationship
		{
			get;
		}

		/// <summary>
		/// Gets a flag indicating whether this <see cref="NavigationProperty"/> is
		/// the source or target of the <see cref="Relationship"/>.
		/// </summary>
		public abstract bool IsSource
		{
			get;
		}

		/// <summary>
		/// Gets the <see cref="DataType"/> of this property.
		/// </summary>
		public override DataType DataType
		{
			get
			{
				var rel = this.Relationship;
				if (rel == null)
				{
					var msg = string.Format("Navigation property {0} is not bound to a Relationship", this.Name);
					throw new InvalidOperationException(msg);
				}
				return (this.IsSource) ? (rel.Source) : (rel.Target);
			}
		}
	}
}
