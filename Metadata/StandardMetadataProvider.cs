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
    /// <summary>
    /// Implements an <see cref="IMetadataProvider"/> that combines
    /// a <see cref="DynamicMetadataProvider"/> and a
    /// <see cref="ReflectionMetadataProvider"/>.
    /// </summary>
    public sealed class StandardMetadataProvider : IMetadataProvider
    {
        private readonly ReflectionMetadataProvider reflectionProvider;
        private readonly DynamicMetadataProvider dynamicProvider;

        /// <summary>
        /// 
        /// </summary>
        public StandardMetadataProvider()
        {
            this.dynamicProvider = new DynamicMetadataProvider();
            this.reflectionProvider = new ReflectionMetadataProvider();
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
            T dt = this.dynamicProvider.GetDataType<T>(fullName);
            if (dt == null)
            {
                dt = this.reflectionProvider.GetDataType<T>(fullName);
            }
            return dt;
        }

        /// <summary>
        /// Gets the <see cref="DynamicMetadataProvider"/>
        /// </summary>
        public DynamicMetadataProvider DynamicProvider
        {
            get { return this.dynamicProvider; }
        }

        /// <summary>
        /// Gets the <see cref="ReflectionMetadataProvider"/>
        /// </summary>
        public ReflectionMetadataProvider ReflectionProvider
        {
            get { return this.reflectionProvider; }
        }
    }
}
