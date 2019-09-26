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

namespace Headway.Dynamo.Metadata.Dynamic
{
    /// <summary>
    /// Implementation of <see cref="IMetadataProvider"/> service
    /// that allows data types to be registered manually.
    /// </summary>
    public class DynamicMetadataProvider : IMetadataProvider
    {
        private readonly Dictionary<string, DynamicObjectType> objTypes;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public DynamicMetadataProvider()
        {
            this.objTypes = new Dictionary<string, DynamicObjectType>();
        }

        /// <summary>
        /// Gets a <see cref="DataType"/> by fully
        /// qualified name.
        /// </summary>
        /// <param name="fullName">Full name of the data type to retrieve</param>
        /// <returns>
        /// The <see cref="DataType"/> matching the specified name or null
        /// if the data type does not exist.
        /// </returns>
        public virtual T GetDataType<T>(string fullName) where T : DataType
        {
            T result = default(T);

            if (this.objTypes.ContainsKey(fullName))
            {
                result = this.objTypes[fullName] as T;
            }

            return result;
        }

        /// <summary>
        /// Registers a new <see cref="DynamicObjectType"/> with this
        /// <see cref="DynamicMetadataProvider"/>.
        /// </summary>
        /// <param name="metadataProvider"></param>
        /// <param name="fullName"></param>
        /// <param name="clrType"></param>
        /// <returns>
        /// Returns a new <see cref="ObjectType"/>
        /// </returns>
        public ObjectType RegisterObjectType(IMetadataProvider metadataProvider, string fullName,
            Type clrType)
        {
            if (this.objTypes.ContainsKey(fullName))
            {
                var msg = string.Format("Object type {0} already registered", fullName);
                throw new InvalidOperationException(msg);
            }

            if (metadataProvider == null)
            {
                metadataProvider = this;
            }

            var objType = DynamicObjectType.Create(metadataProvider, fullName, clrType);
            this.objTypes.Add(fullName, objType);
            return objType;
        }

        /// <summary>
        /// Registers the given <see cref="DynamicObjectType"/> with this
        /// <see cref="DynamicMetadataProvider"/>.
        /// </summary>
        /// <param name="metadataProvider"></param>
        /// <param name="objType"></param>
        public void RegisterObjectType(IMetadataProvider metadataProvider, DynamicObjectType objType)
        {
            if (objType == null)
            {
                throw new ArgumentNullException(nameof(objType));
            }

            if (this.objTypes.ContainsKey(objType.FullName))
            {
                var msg = string.Format("{0} is already registered with this dynamic metadata provider", objType.FullName);
                throw new InvalidOperationException(msg);
            }

            // Attach the metadata provider
            if (metadataProvider == null)
            {
                metadataProvider = this;
            }
            objType.AttachMetadataProvider(metadataProvider);

            this.objTypes.Add(objType.FullName, objType);
        }
    }
}
