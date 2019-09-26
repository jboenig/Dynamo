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

using System.Linq;
using System.Collections.Generic;

namespace Headway.Dynamo.Metadata
{
    /// <summary>
    /// Implements the <see cref="IMetadataProvider"/> service
    /// by aggregating one or more providers.
    /// </summary>
    public class AggregateMetadataProvider : IMetadataProvider
    {
        private readonly List<IMetadataProvider> providers;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public AggregateMetadataProvider()
        {
            this.providers = new List<IMetadataProvider>();
        }

        /// <summary>
        /// Constructs an <see cref="AggregateMetadataProvider"/>
        /// given one <see cref="IMetadataProvider"/>.
        /// </summary>
        /// <param name="provider">
        /// Metadata provider to add.
        /// </param>
        public AggregateMetadataProvider(IMetadataProvider provider)
        {
            this.providers = new List<IMetadataProvider>();
            this.AddProvider(provider);
        }

        /// <summary>
        /// Constructs an <see cref="AggregateMetadataProvider"/>
        /// given two <see cref="IMetadataProvider"/> services.
        /// </summary>
        /// <param name="provider1">
        /// First metadata provider to add.
        /// </param>
        /// <param name="provider2">
        /// Second metadata provider to add.
        /// </param>
        public AggregateMetadataProvider(IMetadataProvider provider1, IMetadataProvider provider2)
        {
            this.providers = new List<IMetadataProvider>();
            this.AddProvider(provider1);
            this.AddProvider(provider2);
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
        public T GetDataType<T>(string fullName) where T : DataType
        {
            foreach (var curProvider in this.providers)
            {
                var dt = curProvider.GetDataType<T>(fullName);
                if (dt != null)
                {
                    return dt;
                }
            }
            return default(T);
        }

        /// <summary>
        /// Add an <see cref="IMetadataProvider"/> to the service.
        /// </summary>
        /// <param name="provider">
        /// Reference to <see cref="IMetadataProvider"/> instance
        /// to add to the service.
        /// </param>
        public void AddProvider(IMetadataProvider provider)
        {
            this.providers.Add(provider);
        }
    }
}
