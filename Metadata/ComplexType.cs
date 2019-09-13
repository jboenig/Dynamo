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
	/// Encapsulates data types that contain properties.
	/// </summary>
	public abstract class ComplexType : DataType
	{
		#region Public Properties

		/// <summary>
		/// Gets the collection of properties associated with this class.
		/// </summary>
		public abstract IEnumerable<Property> Properties
		{
			get;
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Gets the property matching the specified name.
		/// </summary>
		/// <param name="propertyName">Name of property to get.</param>
		/// <returns><see cref="Property"/> matching the specified name.</returns>
		public abstract Property GetPropertyByName(string propertyName);

		/// <summary>
		/// Gets all properties of this class including inherited properties.
		/// </summary>
		/// <returns>List of <see cref="Property"/> objects.</returns>
		public abstract IEnumerable<Property> FindAllProperties();

		/// <summary>
		/// Gets all properties matching the specified filter - includes inherited properties.
		/// </summary>
		/// <param name="filter">Filter criteria.</param>
		/// <returns>List of <see cref="Property"/> objects.</returns>
		public abstract IEnumerable<Property> FindProperties(Func<Property, bool> filter);

        /// <summary>
        /// Adds a property to this type.
        /// </summary>
        /// <param name="propertyName">Name of property to add</param>
        /// <param name="dataType">Data type of property to add</param>
        /// <returns>Returns the new property created</returns>
        public abstract Property AddProperty(string propertyName, DataType dataType);

		#endregion
	}
}
