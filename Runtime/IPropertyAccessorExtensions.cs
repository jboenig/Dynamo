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
