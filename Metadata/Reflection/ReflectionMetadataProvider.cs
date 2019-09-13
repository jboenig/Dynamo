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
            DataType resDataType = null;

            if (!this.reflectionTypes.ContainsKey(fullName))
            {
                resDataType = ReflectionObjectType.Get(this, fullName);
                if (resDataType == null)
                {
                    throw new DataTypeNotFound(fullName);
                }
                this.reflectionTypes.Add(fullName, resDataType);
            }

            return resDataType as T;
        }
    }
}
