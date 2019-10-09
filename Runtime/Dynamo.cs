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
using System.Linq;
using System.Dynamic;
using System.Collections.Generic;
using System.Runtime.Serialization;
//using Newtonsoft.Json;
//using Newtonsoft.Json.Linq;
using Headway.Dynamo.Metadata;
using Headway.Dynamo.Metadata.Dynamic;
using Headway.Dynamo.Exceptions;
using Headway.Dynamo.Runtime;
using System.Linq.Expressions;

namespace Headway.Dynamo.Runtime
{
    /// <summary>
    /// This class derives from System.Dynamic.DynamicObject and extends
    /// it with metadata provided via the <see cref="ObjectType"/>
    /// interface and implements serialization.
    /// </summary>
    [Serializable]
    public class Dynamo : DynamicObject, IPropertyAccessor, IDynamicPropertyAccessor, ISerializable
    {
        #region Member Variables

        private ObjectType dataType;
        private Dictionary<string, object> values;

        #endregion

        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objType"></param>
        public Dynamo(ObjectType objType)
        {
            this.dataType = objType;
            this.values = new Dictionary<string, object>();
        }

        /// <summary>
        /// Constructs a copy of the specified <see cref="Dynamo"/>
        /// object.
        /// </summary>
        /// <param name="source">Source object to copy</param>
        public Dynamo(Dynamo source)
        {
            this.dataType = source.DataType;
            this.values = new Dictionary<string, object>();
            foreach (var curKey in source.values.Keys)
            {
                this.values.Add(curKey, source.values[curKey]);
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the <see cref="ObjectType"/> metadata that defines the
        /// properties in this dynamic object.
        /// </summary>
        public ObjectType DataType
        {
            get
            {
                return this.dataType;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the value of the specified property
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public T GetPropertyValue<T>(string propertyName)
        {
            T actualValue = default(T);
            object intermediateValue = null;

            // Look for property name in metadata
            var prop = this.DataType.GetPropertyByName(propertyName);
            if (prop != null)
            {
                // Use property metadata to retrieve value
                intermediateValue = (T)prop.GetValue<object>(this);
            }
            else
            {
                // Property not found in metadata. Check the values dictionary.
                if (this.values.ContainsKey(propertyName))
                {
                    intermediateValue = this.values[propertyName];
                }
                else
                {
                    // Property not found in metadata or in values dictionary
                    throw new PropertyNotFoundException(this.DataType, propertyName);
                }
            }

            // Check to see if intermediate value is a variable name
            string variableName;
            if (TryGetVariableName(intermediateValue, out variableName))
            {
                // Intermediate value is a variable name - resolve it
                actualValue = this.ResolveVariable<T>(variableName);
            }
            else
            {
                // No variables found so intermediate value is the actual value
                actualValue = (T)intermediateValue;
            }

            return actualValue;
        }

        /// <summary>
        /// Sets the value of the specified property.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        public void SetPropertyValue<T>(string propertyName, T value)
        {
            var prop = this.DataType.GetPropertyByName(propertyName);
            if (prop == null)
            {
                // Property not found - create one
                DataType propType = IntegralType.Get(typeof(T));
                if (propType == null)
                {
                    var msg = string.Format("Error setting value on property {0}. Property not found and value is not an integral type. You must explicitly add the data type to the metadata provider.", propertyName);
                    throw new InvalidOperationException(msg);
                }
                prop = this.DataType.AddProperty(propertyName, propType);
            }

            prop.SetValue<T>(this, value);
        }

        /// <summary>
        /// Returns a collection of all available property names.
        /// </summary>
        /// <returns>
        /// An enumerable of property names.
        /// </returns>
        public IEnumerable<string> GetPropertyNames()
        {
            IEnumerable<string> propNames = null;

            if (this.DataType != null)
            {
                propNames = from p in this.DataType.FindAllProperties() select p.Name;
            }

            return propNames;
        }

        #endregion

        #region DynamicObject Implementation

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public override DynamicMetaObject GetMetaObject(Expression parameter)
        {
            return base.GetMetaObject(parameter);
        }

        /// <summary>
        /// Gets the number of dynamic property values.
        /// </summary>
        public int Count
        {
            get
            {
                return this.values.Count;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="binder"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            bool success = false;
            result = null;
            var prop = this.DataType.GetPropertyByName(binder.Name);
            if (prop != null)
            {
                result = prop.GetValue<object>(this);
                success = true;
            }
            return success;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="binder"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            bool success = false;
            var prop = this.DataType.GetPropertyByName(binder.Name);

            if (prop == null)
            {
                //DataType propType = IntegralType.Get(binder.ReturnType);
                DataType propType = IntegralType.Get(value.GetType());
                if (propType == null)
                {
                    var msg = string.Format("Error setting value on property {0}. Property not found and value is not an integral type. You must explicitly add the data type to the metadata provider.", binder.Name);
                    throw new InvalidOperationException(msg);
                }
                prop = this.DataType.AddProperty(binder.Name, propType);
            }

            if (prop != null)
            {
                prop.SetValue<object>(this, value);
                success = true;
            }

            return success;
        }

        #endregion

        #region IDynamicPropertyAccessor Implementation

        /// <summary>
        /// Gets the value of the property matching the specified name.
        /// </summary>
        /// <typeparam name="T">Data type of property</typeparam>
        /// <param name="propertyName">Name of property to retrieve</param>
        /// <returns>
        /// Value of the property or default(T) if there
        /// is no value stored for the specified property
        /// </returns>
        /// <remarks>
        /// This method only operates on dynamic properties with values stored
        /// in the values dictionary. This separate interface to access only
        /// the dynamic values is necessary in order to prevent a stack overflow,
        /// which was occurring when the implementation of
        /// <see cref="DynamicProperty"/> was calling back on the
        /// <see cref="IPropertyAccessor"/> interface to get its value.
        /// </remarks>
        T IDynamicPropertyAccessor.GetPropertyValue<T>(string propertyName)
        {
            if (this.values.ContainsKey(propertyName))
            {
                return (T)this.values[propertyName];
            }
            return default(T);
        }

        /// <summary>
        /// Sets the value of the specified property.
        /// </summary>
        /// <typeparam name="T">Data type of property</typeparam>
        /// <param name="propertyName">Name of property to set</param>
        /// <param name="value">Value to assign to the property</param>
        /// <remarks>
        /// This method only operates on dynamic properties with values stored
        /// in the values dictionary. This separate interface to access only
        /// the dynamic values is necessary in order to prevent a stack overflow,
        /// which was occurring when the implementation of
        /// <see cref="DynamicProperty"/> was calling back on the
        /// <see cref="IPropertyAccessor"/> interface to get its value.
        /// </remarks>
        void IDynamicPropertyAccessor.SetPropertyValue<T>(string propertyName, T value)
        {
            if (this.values.ContainsKey(propertyName))
            {
                this.values[propertyName] = value;
            }
            else
            {
                this.values.Add(propertyName, value);
            }
        }

        #endregion

        #region Serialization

#if false
        /// <summary>
        /// Converts this <see cref="DynamoObject"/> object to
        /// a Json string.
        /// </summary>
        /// <returns>
        /// Serialized Json string
        /// </returns>
        //public string ToJson()
        //{
        //    var serializerSettings = new JsonSerializerSettings()
        //    {
        //        TypeNameHandling = TypeNameHandling.Objects
        //    };

        //    return JsonConvert.SerializeObject(this, Formatting.None, serializerSettings);
        //}

        /// <summary>
        /// Determines if the given Json object contains all of the properties
        /// required for deserialization into a parameter model.
        /// </summary>
        /// <param name="jObj">
        /// Json object to test for completeness
        /// </param>
        /// <returns>
        /// Returns true if the specified Json object contains all required
        /// properties for deserialization. Returns false if it is incomplete.
        /// </returns>
        /// <remarks>
        /// This method is used to determine if the Json object should be
        /// directly used for deserialization or if it should be merged into
        /// a clean Json object that contains all properties with default
        /// values.
        /// </remarks>
        //public static bool IsJsonComplete(JObject jObj)
        //{
        //    bool isComplete = true;

        //    if (jObj["dataType"] == null)
        //    {
        //        isComplete = false;
        //    }

        //    return isComplete;
        //}
#endif

        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected Dynamo(SerializationInfo info, StreamingContext context)
        {
            try
            {
                this.dataType = info.GetValue("dataType", typeof(ObjectType)) as ObjectType;
            }
            catch
            {
//                this.dataType = DynamicObjectType.Create(this.MetadataProvider, typeof(DynamoObject).FullName, typeof(DynamoObject));
            }
            
            this.values = new Dictionary<string, object>();

            var valEnumerator = info.GetEnumerator();
            while (valEnumerator.MoveNext())
            {
                var curPropName = valEnumerator.Current.Name;

                var curProp = this.DataType.GetPropertyByName(curPropName);
                if (curProp == null)
                {
                    // Property not found - create one
                    var propVal = info.GetValue(curPropName, typeof(object));
                    //var jval = propVal as Newtonsoft.Json.Linq.JValue;
                    //if (jval != null)
                    //{
                    //    propVal = jval.Value;
                    //}
                    DataType propType = IntegralType.Get(propVal.GetType());
                    if (propType == null)
                    {
                        throw new NotSupportedException("DynamicExtObject only supports dynamic creation of integral types during deserialization");
                    }
                    curProp = this.DataType.AddProperty(curPropName, propType);
                    curProp.SetValue<object>(this, propVal);
                }
                else
                {
                    var propVal = info.GetValue(curPropName, curProp.DataType.CLRType);
                    curProp.SetValue<object>(this, propVal);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("dataType", this.DataType);

            foreach (var prop in this.DataType.FindAllProperties())
            {
                if (prop.Serialize)
                {
                    var propVal = prop.GetValue<object>(this);
                    if (propVal != null || !prop.DataType.CLRType.IsValueType)
                    {
                        info.AddValue(prop.Name, propVal);
                    }
                }
            }
        }

        #endregion

        #region Implementation

        private static bool TryGetVariableName(object intermediateValue, out string variableName)
        {
            variableName = null;

            string varName = intermediateValue as string;
            if (varName != null && !string.IsNullOrEmpty(varName))
            {
                if (varName.StartsWith("$(") && varName.EndsWith(")"))
                {
                    variableName = varName.Substring(2, varName.Length - 3);
                    return true;
                }
            }

            return false;
        }

        private T ResolveVariable<T>(string variableName)
        {
            if (variableName == "Now")
            {
                return (T)(object)DateTime.Now;
            }
            else if (variableName == "UtcNow")
            {
                return (T)(object)DateTime.UtcNow;
            }
            throw new ArgumentException(string.Format("Invalid variable name {0}", variableName), "variableName");
        }

        #endregion
    }
}
