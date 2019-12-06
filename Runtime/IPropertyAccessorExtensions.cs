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

using Headway.Dynamo.Exceptions;

namespace Headway.Dynamo.Runtime
{
    /// <summary>
    /// Extension methods for the <see cref="IPropertyAccessor"/>
    /// interface.
    /// </summary>
    public static class IPropertyAccessorExtensions
    {
        /// <summary>
        /// Accesses properties more than one-level deep in an
        /// object graph by parsing property names that specify a
        /// path.  For example, a path might look like this -
        /// "Location.X" or "Customer.SSN".
        /// </summary>
        /// <typeparam name="T">
        /// Type of value to return.
        /// </typeparam>
        /// <param name="propAccessor">
        /// Reference to <see cref="IPropertyAccessor"/> object.
        /// </param>
        /// <param name="propertyName">
        /// Name of the property to return.  May optionally specify
        /// a path multiple levels deep. Each level in the path is
        /// separated using dot notation.
        /// </param>
        /// <returns>
        /// Returns the value of the property or null
        /// if not found.
        /// </returns>
        public static T GetPropertyValueRecursive<T>(this IPropertyAccessor propAccessor,
            string propertyName)
        {
            var sepIdx = propertyName.IndexOf('.');
            if (sepIdx >= 0)
            {
                var parentName = propertyName.Substring(0, sepIdx);
                var childName = propertyName.Substring(sepIdx + 1);
                var childPropAccessor = propAccessor.GetPropertyValue<IPropertyAccessor>(parentName);
                if (childPropAccessor == null)
                {
                    throw new PropertyNotFoundException(propAccessor.GetType(), parentName);
                }
                return childPropAccessor.GetPropertyValueRecursive<T>(childName);
            }
            return propAccessor.GetPropertyValue<T>(propertyName);
        }

        /// <summary>
        /// Gets a property value by name as an object.
        /// </summary>
        /// <param name="propertyAccessor">
        /// Reference to <see cref="IPropertyAccessor"/> object.
        /// </param>
        /// <param name="propertyName">
        /// Name of the property to retrieve.
        /// </param>
        /// <returns>
        /// Value of property as an object type.
        /// </returns>
        public static object GetPropertyValue(this IPropertyAccessor propertyAccessor,
            string propertyName)
        {
            return propertyAccessor.GetPropertyValue<object>(propertyName);
        }

        /// <summary>
        /// Sets a property by name given an object value
        /// </summary>
        /// <param name="propertyAccessor">
        /// Reference to <see cref="IPropertyAccessor"/> object.
        /// </param>
        /// <param name="propertyName">Name of property to set</param>
        /// <param name="value">
        /// Value to assign to the property.
        /// </param>
        public static void SetPropertyValue(this IPropertyAccessor propertyAccessor,
            string propertyName,
            object value)
        {
            propertyAccessor.SetPropertyValue<object>(propertyName, value);
        }
    }
}
