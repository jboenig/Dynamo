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
using System.Linq;
using System.Collections.Generic;

namespace Headway.Dynamo.Metadata
{
	/// <summary>
	/// Interface to object that contains named
    /// Attributes.
	/// </summary>
	public interface IAttributeContainer
	{
		/// <summary>
		/// Gets all attributes matching the specified name.
		/// </summary>
		/// <typeparam name="T">
        /// Type of attributes to return.
        /// </typeparam>
		/// <param name="attributeName">
        /// Name of attributes to get
        /// </param>
		/// <returns>
        /// Collection of attributes matching the specified name.
        /// </returns>
		IEnumerable<T> GetAttributes<T>(string attributeName) where T : Attribute;

		/// <summary>
		/// Gets a list of the names of all available attributes.
		/// </summary>
		/// <returns>Enumerable collection of attribute names.</returns>
		IEnumerable<string> GetAttributeNames();
	}

	/// <summary>
	/// 
	/// </summary>
	public static class AttributeContainerHelper
	{
		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="attributeContainer"></param>
		/// <param name="attributeName"></param>
		/// <returns></returns>
		public static T GetAttribute<T>(this IAttributeContainer attributeContainer, string attributeName) where T : Attribute
		{
			return attributeContainer.GetAttributes<T>(attributeName).FirstOrDefault();
		}
	}
}
