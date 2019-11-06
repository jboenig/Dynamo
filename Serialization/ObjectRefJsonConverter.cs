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
using Newtonsoft.Json;
using Headway.Dynamo.Runtime;
using Headway.Dynamo.Exceptions;

namespace Headway.Dynamo.Serialization
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TObject"></typeparam>
    public class ObjectRefJsonConverter<TKey, TObject> : JsonConverter<TObject> where TObject : class
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public ObjectRefJsonConverter()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public override bool CanRead => true;

        /// <summary>
        /// 
        /// </summary>
        public override bool CanWrite => true;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="objectType"></param>
        /// <param name="existingValue"></param>
        /// <param name="hasExistingValue"></param>
        /// <param name="serializer"></param>
        /// <returns></returns>
        public override TObject ReadJson(JsonReader reader,
            Type objectType,
            TObject existingValue,
            bool hasExistingValue,
            JsonSerializer serializer)
        {
            var serviceProvider = serializer.Context.Context as IServiceProvider;
            if (serviceProvider == null)
            {
                throw new ServiceNotFoundException(typeof(IServiceProvider));
            }

            var objectEntityRef = serializer.Deserialize<EntityRef>(reader);
            if (objectEntityRef.PrimaryKey != null)
            {
                var keyType = Type.GetType(objectEntityRef.KeyType, false, false);
                if (keyType == null)
                {
                    throw new InvalidOperationException();
                }

                var objResolverType = typeof(IObjectResolver<,>).MakeGenericType(new Type[]
                {
                    keyType,
                    objectType
                });

                var objResolver = serviceProvider.GetService(objResolverType) as IObjectResolver<string, TObject>;
                if (objResolver == null)
                {
                    throw new ServiceNotFoundException(objResolverType);
                }

                return objResolver.Resolve((string)objectEntityRef.PrimaryKey);
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        /// <param name="serializer"></param>
        public override void WriteJson(JsonWriter writer,
            TObject value,
            JsonSerializer serializer)
        {
            var pkAccessor = value as IPrimaryKeyAccessor;
            if (pkAccessor == null)
            {
                var msg = string.Format("Attempt to serialize object reference for type {0} failed because it does not implement {1}",
                    nameof(TObject),
                    nameof(IPrimaryKeyAccessor));
                throw new InvalidOperationException(msg);
            }

            var objectEntityRef = new EntityRef()
            {
                ObjectType = typeof(TObject).Name,
                PrimaryKey = pkAccessor.PrimaryKey,
                KeyType = typeof(TKey).FullName
            };
            serializer.Serialize(writer, objectEntityRef);
        }
    }
}
