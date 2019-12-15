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
