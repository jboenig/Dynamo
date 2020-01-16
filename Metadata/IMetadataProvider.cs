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

using System;
using Headway.Dynamo.Exceptions;

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

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="metadataProvider"></param>
        /// <param name="svcProvider"></param>
        /// <param name="objTypeFullName"></param>
        /// <param name="paramList"></param>
        /// <returns></returns>
        public static T CreateInstance<T>(this IMetadataProvider metadataProvider, IServiceProvider svcProvider, string objTypeFullName, params object[] paramList) where T : class
        {
            if (string.IsNullOrEmpty(objTypeFullName))
            {
                throw new ArgumentNullException(nameof(objTypeFullName));
            }

            var objType = metadataProvider.GetDataType<ObjectType>(objTypeFullName);
            if (objType == null)
            {
                throw new DataTypeNotFound(objTypeFullName);
            }

            return objType.CreateInstance<T>(svcProvider, paramList);
        }
    }
}
