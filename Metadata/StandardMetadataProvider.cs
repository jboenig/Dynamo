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
using Headway.Dynamo.Metadata.Reflection;
using Headway.Dynamo.Metadata.Dynamic;

namespace Headway.Dynamo.Metadata
{
    public sealed class StandardMetadataProvider : AggregateMetadataProvider
    {
        private readonly ReflectionMetadataProvider reflectionProvider;
        private readonly DynamicMetadataProvider dynamicProvider;

        public StandardMetadataProvider()
        {
            this.reflectionProvider = new ReflectionMetadataProvider();
            this.dynamicProvider = new DynamicMetadataProvider();
            this.AddProvider(this.reflectionProvider);
            this.AddProvider(this.dynamicProvider);
        }

        /// <summary>
        /// Registers a new <see cref="DynamicObjectType"/> with this
        /// <see cref="DynamicMetadataProvider"/>.
        /// </summary>
        /// <param name="clrType"></param>
        /// <returns>
        /// Returns a new <see cref="ObjectType"/>
        /// </returns>
        public ObjectType RegisterObjectType(string fullName,
            Type clrType)
        {
            return this.dynamicProvider.RegisterObjectType(fullName, clrType);
        }

        /// <summary>
        /// Registers the given <see cref="DynamicObjectType"/> with this
        /// <see cref="DynamicMetadataProvider"/>.
        /// </summary>
        /// <param name="objType"></param>
        public void RegisterObjectType(DynamicObjectType objType)
        {
            this.dynamicProvider.RegisterObjectType(objType);
        }
    }
}
