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

namespace Headway.Dynamo.Metadata
{
    /// <summary>
    /// Interface used to retrieve metadata.
    /// </summary>
    public interface IMetadataProvider
    {
        /// <summary>
        /// Gets a <see cref="DataType"/> by fully
        /// qualified name.
        /// </summary>
        /// <param name="fullName">Full name of the data type to retrieve</param>
        /// <returns>
        /// The <see cref="DataType"/> matching the specified name or null
        /// if the data type does not exist.
        /// </returns>
        T GetDataType<T>(string fullName) where T : DataType;
    }

    /// <summary>
    /// Provides extension methods for the <see cref="IMetadataProvider"/>
    /// interface.
    /// </summary>
    public static class MetadataProviderExtensions
    {
        /// <summary>
        /// Gets the <see cref="DataType"/> for a given CLR type.
        /// </summary>
        /// <typeparam name="T">
        /// Type of <see cref="DataType"/> metadata object requested.
        /// </typeparam>
        /// <param name="metadataProvider">
        /// Reference to <see cref="IMetadataProvider"/> object.
        /// </param>
        /// <param name="clrType">
        /// CLR type to get <see cref="DataType"/> for
        /// </param>
        /// <returns>
        /// An instance of <see cref="DataType"/> or a subclass of
        /// <see cref="DataType"/>
        /// </returns>
        public static T GetDataType<T>(this IMetadataProvider metadataProvider, Type clrType)
            where T : DataType
        {
            return metadataProvider.GetDataType<T>(clrType.FullName);
        }

        /// <summary>
        /// Gets the <see cref="DataType"/> for a given CLR type.
        /// </summary>
        /// <param name="metadataProvider">
        /// Reference to <see cref="IMetadataProvider"/> object.
        /// </param>
        /// <param name="fullName">Full name of the data type to retrieve</param>
        /// <returns>
        /// An instance of <see cref="DataType"/>
        /// </returns>
        public static DataType GetDataType(this IMetadataProvider metadataProvider, string fullName)
        {
            return metadataProvider.GetDataType<DataType>(fullName);
        }

        /// <summary>
        /// Gets the <see cref="DataType"/> for a given CLR type.
        /// </summary>
        /// <param name="metadataProvider">
        /// Reference to <see cref="IMetadataProvider"/> object.
        /// </param>
        /// <param name="clrType">
        /// CLR type to get <see cref="DataType"/> for
        /// </param>
        /// <returns>
        /// An instance of <see cref="DataType"/>
        /// </returns>
        public static DataType GetDataType(this IMetadataProvider metadataProvider, Type clrType)
        {
            return metadataProvider.GetDataType<DataType>(clrType.FullName);
        }
    }
}
