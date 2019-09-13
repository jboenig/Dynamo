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
    public static class IPropertyAccessorExtensions
    {
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
                    throw new PropertyNotFoundException(null, parentName);
                }
                return childPropAccessor.GetPropertyValueRecursive<T>(childName);
            }
            return propAccessor.GetPropertyValue<T>(propertyName);
        }
    }
}
