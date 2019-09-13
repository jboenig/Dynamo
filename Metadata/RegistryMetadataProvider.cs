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

namespace Headway.Dynamo.Metadata
{
    /// <summary>
    /// Implementation of <see cref="IMetadataProvider"/> service
    /// that allows data types to be registered manually.
    /// </summary>
    public class RegistryMetadataProvider : IMetadataProvider
    {
        private readonly Dictionary<string, DataType> dataTypes;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public RegistryMetadataProvider()
        {
            this.dataTypes = new Dictionary<string, DataType>();
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

            if (this.dataTypes.ContainsKey(fullName))
            {
                result = (T)this.dataTypes[fullName];
            }

            return result;
        }

        /// <summary>
        /// Registers the given <see cref="DataType"/> with this
        /// <see cref="MetadataProvider"/>.
        /// </summary>
        /// <param name="dataType"></param>
        public void RegisterDataType(DataType dataType)
        {
            if (dataType == null)
            {
                throw new ArgumentNullException("dataType");
            }

            if (this.dataTypes.ContainsKey(dataType.FullName))
            {
                var msg = string.Format("{0} is already registered with this metadata provider", dataType.FullName);
                throw new InvalidOperationException(msg);
            }

            this.dataTypes.Add(dataType.FullName, dataType);
        }
    }
}
