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
using System.Collections.Generic;
using Headway.Dynamo.Exceptions;

namespace Headway.Dynamo.Metadata.Reflection
{
    /// <summary>
    /// Implements the <see cref="IMetadataProvider"/> service
    /// using reflection.
    /// </summary>
    public sealed class ReflectionMetadataProvider : IMetadataProvider
    {
        private readonly Dictionary<string, DataType> reflectionTypes;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ReflectionMetadataProvider()
        {
            this.reflectionTypes = new Dictionary<string, DataType>();
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
            T resDataType = null;

            // Check reflection object type cache
            if (!this.reflectionTypes.ContainsKey(fullName))
            {
                // Type not found in cache
                resDataType = ReflectionObjectType.Get(this, fullName) as T;
                if (resDataType != null)
                {
                    this.reflectionTypes.Add(fullName, resDataType);
                }
            }
            else
            {
                // Type found in cache
                resDataType = (T)this.reflectionTypes[fullName];
            }

            if (resDataType == default(T) && this.NextProvider != null)
            {
                // Type not found. Type the next metadata provider.
                resDataType = this.NextProvider.GetDataType<T>(fullName);
            }

            return resDataType;
        }

        /// <summary>
        /// Gets or sets the next <see cref="IMetadataProvider"/> in the chain.
        /// </summary>
        public IMetadataProvider NextProvider
        {
            get;
            set;
        }
    }
}
