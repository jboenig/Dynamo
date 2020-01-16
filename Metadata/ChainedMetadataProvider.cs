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

namespace Headway.Dynamo.Metadata
{
    /// <summary>
    /// Provides a base-class for <see cref="IMetadataProvider"/>
    /// implementations that are chained other providers.
    /// </summary>
    public abstract class ChainedMetadataProvider : IMetadataProvider
    {
        private readonly IMetadataProvider nextProvider;

        /// <summary>
        /// Constructs a <see cref="ChainedMetadataProvider"/> given a
        /// reference to the next <see cref="IMetadataProvider"/> in
        /// a chain.
        /// </summary>
        /// <param name="nextProvider">
        /// Next metadata provider in the chain
        /// </param>
        public ChainedMetadataProvider(IMetadataProvider nextProvider = null)
        {
            this.nextProvider = nextProvider;
        }

        /// <summary>
        /// Gets a <see cref="DataType"/> by fully
        /// qualified name.
        /// </summary>
        /// <typeparam name="T">Type of DataType to return</typeparam>
        /// <param name="fullName">Full name of the data type to retrieve</param>
        /// <returns>
        /// The <see cref="DataType"/> matching the specified name or null
        /// if the data type does not exist.
        /// </returns>
        public T GetDataType<T>(string fullName) where T : DataType
        {
            T result = this.GetDataTypeInternal<T>(fullName);

            if (result == default(T) && this.nextProvider != null)
            {
                result = this.nextProvider.GetDataType<T>(fullName);
            }

            return result;
        }

        /// <summary>
        /// Implements <see cref="IMetadataProvider.GetDataType{T}(string)"/>
        /// for this provider only - without chaining.
        /// </summary>
        /// <typeparam name="T">Type of DataType to return</typeparam>
        /// <param name="fullName">Full name of the data type to retrieve</param>
        /// <returns>
        /// The <see cref="DataType"/> matching the specified name or null
        /// if the data type does not exist.
        /// </returns>
        protected abstract T GetDataTypeInternal<T>(string fullName) where T : DataType;
    }
}
