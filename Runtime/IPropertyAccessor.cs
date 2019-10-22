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

namespace Headway.Dynamo.Runtime
{
    /// <summary>
    /// Interface to objects that have dynamic properties.
    /// </summary>
    public interface IPropertyAccessor
    {
        /// <summary>
        /// Gets the value of the specified property
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        T GetPropertyValue<T>(string propertyName);

        /// <summary>
        /// Sets the value of the specified property.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        /// <returns>Reference to this</returns>
        IPropertyAccessor SetPropertyValue<T>(string propertyName, T value);

        /// <summary>
        /// Returns a collection of all available property names.
        /// </summary>
        /// <returns>
        /// An enumerable of property names.
        /// </returns>
        IEnumerable<string> GetPropertyNames();
    }
}
