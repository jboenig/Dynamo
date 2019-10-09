using System;
using System.Collections.Generic;
using System.Text;

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
