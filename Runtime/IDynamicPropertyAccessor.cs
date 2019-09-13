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
    /// Interface to objects that have dynamic properties. This interface
    /// is used exclusively to access dynamic properties only.
    /// </summary>
    /// <remarks>
    /// This interface was introduced in order to prevent a recursive
    /// stack overflow. The <see cref="DynamicProperty"/> cannot implement
    /// GetValue and SetValue by calling back on <see cref="IPropertyAccessor"/>,
    /// because that just calls back to the property and causes a recursive
    /// stack overflow. Objects that have dynamic properties need to implement
    /// this interface.
    /// </remarks>
    /// <see cref="DynamicProperty"/>
    public interface IDynamicPropertyAccessor
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
        void SetPropertyValue<T>(string propertyName, T value);
    }
}
