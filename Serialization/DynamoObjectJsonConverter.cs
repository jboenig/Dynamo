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
using Newtonsoft.Json.Linq;
using Headway.Dynamo.Metadata;
using Headway.Dynamo.Exceptions;

namespace Headway.Dynamo.Serialization
{
    /// <summary>
    /// Serializes and deserializes objects of type
    /// <see cref="DynamoObject"/>.
    /// </summary>
    public sealed class DynamoObjectJsonConverter : JsonConverter
    {
        /// <summary>
        /// Gets a flag indicating whether or not this
        /// converter can read.
        /// </summary>
        public override bool CanRead => true;

        /// <summary>
        /// Gets a flag indicating whether or not this
        /// converter can write.
        /// </summary>
        public override bool CanWrite => true;

        /// <summary>
        /// Determines whether or not the specified object
        /// type can be converted by this converter.
        /// </summary>
        /// <param name="objectType">
        /// Type of object to attempt to convert.
        /// </param>
        /// <returns>
        /// Returns true if the object can be converted and false
        /// if this converter does not support converting the
        /// specified type.
        /// </returns>
        public override bool CanConvert(Type objectType)
        {
            return (typeof(DynamoObject).IsAssignableFrom(objectType));
        }

        /// <summary>
        /// Reads a <see cref="DynamoObject"/> from the
        /// serialization stream.
        /// </summary>
        /// <param name="reader">
        /// Reader used to read data from the serialization stream.
        /// </param>
        /// <param name="objClrType">
        /// Type of object to deserialize.
        /// </param>
        /// <param name="existingValue">
        /// IGNORED
        /// </param>
        /// <param name="serializer">
        /// JSON serializer that encapsulates the serialization context
        /// </param>
        /// <returns>
        /// Returns a new <see cref="DynamoObject"/> containing the
        /// state information read from the serialization context.
        /// </returns>
        public override object ReadJson(JsonReader reader, Type objClrType, object existingValue, JsonSerializer serializer)
        {
            // Reference to service provider MUST be attached to the context
            var svcProvider = serializer.Context.Context as IServiceProvider;
            if (svcProvider == null)
            {
                throw new InvalidOperationException("Service provider not attached to serializer context");
            }

            // Get IMetadataProvider service
            var metadataProvider = svcProvider.GetService(typeof(IMetadataProvider)) as IMetadataProvider;
            if (metadataProvider == null)
            {
                throw new ServiceNotFoundException(typeof(IMetadataProvider));
            }

            // Parse JSON into JObject from stream
            JObject jObject = JObject.Load(reader);

            // Retrieve metadata for the object via
            // IMetadataProvider service
            ObjectType objType;
            var tokenDataTypeName = jObject.SelectToken(DataTypeNameToken);
            if (tokenDataTypeName != null)
            {
                // Retrieve metadata for object type
                var objTypeFullName = tokenDataTypeName.Value<string>();
                objType = metadataProvider.GetDataType<ObjectType>(objTypeFullName);
                if (objType == null)
                {
                    throw new DataTypeNotFound(objTypeFullName);
                }
            }
            else
            {
                objType = metadataProvider.GetDataType<ObjectType>(objClrType);
                if (objType == null)
                {
                    throw new DataTypeNotFound(objClrType.FullName);
                }
            }

            // Create object instance
            var resultObj = objType.CreateInstance<DynamoObject>(svcProvider, null);

            // Populate object by iterating through every
            // property in the parsedd JSON object
            foreach (var curJProp in jObject.Properties())
            {
                var curPropName = curJProp.Name;

                // Skip data type information
                if (curPropName.Equals(CLRTypeToken) ||
                    curPropName.Equals(DataTypeNameToken))
                {
                    continue;
                }

                // Get property metadata
                var curProp = objType.GetPropertyByName(curPropName);
                if (curProp != null)
                {
                    if (curProp.Serialize && curProp.CanWrite)
                    {
                        SetPropertyValue(resultObj, curProp, curJProp.Value);
                    }
                }
                else
                {
                    // Attempt to find property through reflection
                    var clrProp = objType.CLRType.GetProperty(curJProp.Name);
                    if (clrProp != null)
                    {
                        var obj = curJProp.ToObject(clrProp.PropertyType);
                        if (obj != null)
                        {
                            clrProp.SetValue(resultObj, obj);
                        }
                    }
                }
            }

            return resultObj;
        }

        /// <summary>
        /// Writes the given <see cref="DynamoObject"/> to the
        /// serialization stream.
        /// </summary>
        /// <param name="writer">
        /// Writer used to write data to the serialization stream.
        /// </param>
        /// <param name="value">
        /// Value to write - must be a <see cref="DynamoObject"/>
        /// </param>
        /// <param name="serializer">
        /// JSON serializer that encapsulates the serialization context
        /// </param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var dynamoObj = value as DynamoObject;
            if (dynamoObj == null)
            {
                var msg = string.Format("Unable to cast {0} to {1}",
                    value.GetType().FullName, nameof(DynamoObject));
                throw new InvalidCastException(msg);
            }

            // Write opening tag
            writer.WriteStartObject();

            // Write the $type property
            writer.WritePropertyName(CLRTypeToken);
            var clrType = value.GetType();
            var clrClassName = clrType.FullName;
            var clrAssemblyName = clrType.Assembly.GetName().Name;
            var clrTypeInfo = $"{clrClassName}, {clrAssemblyName}";
            writer.WriteValue(clrTypeInfo);

            // Write the DataTypeName property
            var dataTypeName = dynamoObj.DataTypeName;
            if (!string.IsNullOrEmpty(dataTypeName))
            {
                writer.WritePropertyName(DataTypeNameToken);
                writer.WriteValue(dataTypeName);
            }

            // Write each property of the object
            foreach (var curProp in dynamoObj.DataType.FindAllProperties())
            {
                // Skip property if it does not support serialize
                // or if it has no getter
                if (curProp.Serialize && curProp.CanRead)
                {
                    // Get the value of the current property
                    var curPropVal = curProp.GetValue<object>(dynamoObj);
                    if (curPropVal != null)
                    {
                        var curPropValType = curPropVal.GetType();
                        if (curPropValType == typeof(string) ||
                            curPropValType.IsValueType)
                        {
                            // Write value types and strings out as
                            // simple values
                            writer.WritePropertyName(curProp.Name);
                            writer.WriteValue(curPropVal);
                        }
                        else
                        {
                            // Serialize nested object
                            writer.WritePropertyName(curProp.Name);
                            serializer.Serialize(writer, curPropVal);
                        }
                    }
                }
            }

            // Write the end tag
            writer.WriteEndObject();
        }

        private const string CLRTypeToken = "$type";
        private const string DataTypeNameToken = "DataTypeName";

        private static void SetPropertyValue(DynamoObject target, Property prop, JToken token)
        {
            if (prop.DataType.CLRType == typeof(string))
            {
                prop.SetValue<string>(target, token.Value<string>());
            }
            else if (prop.DataType.CLRType == typeof(Int32))
            {
                prop.SetValue<Int32>(target, token.Value<Int32>());
            }
            else if (prop.DataType.CLRType == typeof(Int64))
            {
                prop.SetValue<Int64>(target, token.Value<Int64>());
            }
            else if (prop.DataType.CLRType == typeof(Int16))
            {
                prop.SetValue<Int16>(target, token.Value<Int16>());
            }
            else if (prop.DataType.CLRType == typeof(double))
            {
                prop.SetValue<double>(target, token.Value<double>());
            }
            else if (prop.DataType.CLRType == typeof(float))
            {
                prop.SetValue<float>(target, token.Value<float>());
            }
            else if (prop.DataType.CLRType == typeof(decimal))
            {
                prop.SetValue<decimal>(target, token.Value<decimal>());
            }
            else if (prop.DataType.CLRType == typeof(DateTime))
            {
                prop.SetValue<DateTime>(target, token.Value<DateTime>());
            }
            else
            {
                var objValue = token.ToObject(prop.DataType.CLRType);
                if (objValue != null)
                {
                    prop.SetValue<object>(target, objValue);
                }
            }
        }
    }
}
