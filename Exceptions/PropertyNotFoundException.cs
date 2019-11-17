﻿////////////////////////////////////////////////////////////////////////////////
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
using Headway.Dynamo.Metadata;

namespace Headway.Dynamo.Exceptions
{
    /// <summary>
    /// This exception is thrown when a property is referenced that is not
    /// declared on the specified <see cref="ComplexType"/>.
    /// </summary>
    public sealed class PropertyNotFoundException : Exception
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataType"></param>
        /// <param name="propertyName"></param>
        public PropertyNotFoundException(ComplexType dataType, string propertyName) :
            base(string.Format("Property name {0} does not exist on data type {1}", propertyName, dataType.Name))
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="clrType"></param>
        /// <param name="propertyName"></param>
        public PropertyNotFoundException(Type clrType, string propertyName) :
            base(string.Format("Property name {0} does not exist on data type {1}", propertyName, clrType.Name))
        {

        }
    }
}
