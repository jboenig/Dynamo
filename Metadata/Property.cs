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
	/// Encapsulates a property of a <see cref="ComplexType"/>
	/// </summary>
	public abstract class Property : Member
	{
		/// <summary>
		/// Gets the <see cref="DataType"/> of this property.
		/// </summary>
		public abstract DataType DataType
		{
			get;
		}

		/// <summary>
		/// Gets a flag indicating if the property is nullable.
		/// </summary>
		public abstract bool IsNullable
		{
			get;
		}

		/// <summary>
		/// Gets or sets the default value of the property.
		/// </summary>
		public abstract object DefaultValue
		{
			get;
            set;
		}

        /// <summary>
        /// Gets a flag indicating whether or not this property should be serialized.
        /// </summary>
        public abstract bool Serialize
		{
			get;
		}

		/// <summary>
		/// Gets the value of this property from the specified source object.
		/// </summary>
		/// <typeparam name="T">Property type</typeparam>
		/// <param name="source">
        /// Source object that contains the property.
        /// </param>
		/// <returns>
        /// Value of the property
        /// </returns>
		public abstract T GetValue<T>(object source);

        /// <summary>
        /// Sets the value of this property on the specified target object.
        /// </summary>
        /// <typeparam name="T">Property type</typeparam>
        /// <param name="target">
        /// Target object to receive the property value.
        /// </param>
        /// <param name="value">Value to assign to the property</param>
        public abstract void SetValue<T>(object target, T value);
	}
}
