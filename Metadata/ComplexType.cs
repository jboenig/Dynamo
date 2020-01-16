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
