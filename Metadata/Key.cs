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
	/// Encapsulates one or more properties that comprise a key.
	/// </summary>
	public abstract class Key
	{
		/// <summary>
		/// Name of the key.
		/// </summary>
		public abstract string Name
		{
			get;
		}

		/// <summary>
		/// Properties that make up the key.
		/// </summary>
		public abstract IEnumerable<Property> Properties
		{
			get;
		}

		/// <summary>
		/// Gets a flag that indicates whether or not the
		/// key is unique.
		/// </summary>
		public abstract bool IsUnique
		{
			get;
		}
	}
}
