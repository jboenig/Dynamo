using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Headway.Dynamo.Runtime;
using System.Diagnostics.CodeAnalysis;

namespace Headway.Dynamo.Serialization
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class PropertyValueDictionaryJsonConverter : JsonConverter<PropertyValueDictionary>
    {
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
        public override PropertyValueDictionary ReadJson(JsonReader reader, Type objectType, PropertyValueDictionary existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        /// <param name="serializer"></param>
        public override void WriteJson(JsonWriter writer, PropertyValueDictionary value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
