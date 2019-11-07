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
using Headway.Dynamo.Metadata;
using Headway.Dynamo.Metadata.Dynamic;
using Headway.Dynamo.Exceptions;

namespace Headway.Dynamo.Runtime
{
    /// <summary>
    /// This class derives from System.Dynamic.DynamicObject and extends
    /// it with metadata provided via the <see cref="ObjectType"/>
    /// interface and implements serialization.
    /// </summary>
    [Serializable]
    public class Dynamo : IDynamicMetaObjectProvider,
                          IPropertyAccessor,
                          IDynamicPropertyAccessor,
                          ISerializable
    {
        #region Member Variables

        private readonly Dictionary<string, object> values;

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Dynamo()
        {
            this.DataType = Metadata.Reflection.ReflectionObjectType.Get(this.GetType());
            this.values = new Dictionary<string, object>();
        }

        /// <summary>
        /// Constructs a <see cref="Dynamo"/> given an
        /// <see cref="ObjectType"/> metadata object.
        /// </summary>
        /// <param name="objType">
        /// Object containing the metadata for this
        /// <see cref="Dynamo"/> object.
        /// </param>
        public Dynamo(ObjectType objType)
        {
            this.DataType = objType;
            this.values = new Dictionary<string, object>();
        }

        /// <summary>
        /// Constructs a copy of the specified <see cref="Dynamo"/>
        /// object.
        /// </summary>
        /// <param name="source">Source object to copy</param>
        public Dynamo(Dynamo source)
        {
            this.DataType = source.DataType;
            this.values = new Dictionary<string, object>();
            foreach (var curKey in source.values.Keys)
            {
                this.values.Add(curKey, source.values[curKey]);
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the <see cref="ObjectType"/> metadata that defines
        /// the properties and metadata for this dynamic object.
        /// </summary>
        public ObjectType DataType
        {
            get;
            set;
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
        public IPropertyAccessor SetPropertyValue<T>(string propertyName, T value)
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

            return this;
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

        #region IDynamicMetaObjectProvider

        DynamicMetaObject IDynamicMetaObjectProvider.GetMetaObject(
             System.Linq.Expressions.Expression parameter)
        {
            return new DynamoMetaObject(parameter, this);
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

        private const string PropNameDataTypeName = "DataTypeName";

        /// <summary>
        /// Serialization constructor.
        /// </summary>
        /// <param name="info">Serialiation info</param>
        /// <param name="context">Streaming context</param>
        /// <remarks>
        /// Deserializes the given SerializationInfo into a new
        /// instance of this class.
        /// </remarks>
        protected Dynamo(SerializationInfo info, StreamingContext context)
        {
            ///////////////////////////////////////////////////////////////
            // Create dictionary to store dynamic property values
            this.values = new Dictionary<string, object>();

            ///////////////////////////////////////////////////////////////
            // Get IServiceProvider instance from streaming context
            var svcProvider = context.Context as IServiceProvider;
            if (svcProvider == null)
            {
                throw new InvalidOperationException("An instance of IServiceProvider must be attached to the StreamingContext in order to deserialize this object");
            }

            ///////////////////////////////////////////////////////////////
            // Load data type information
            var dataTypeName = info.GetString(PropNameDataTypeName);

            if (!string.IsNullOrEmpty(dataTypeName))
            {
                var metadataProvider = svcProvider.GetService(typeof(IMetadataProvider)) as IMetadataProvider;
                if (metadataProvider == null)
                {
                    throw new ServiceNotFoundException(typeof(IMetadataProvider));
                }
                this.DataType = metadataProvider.GetDataType<ObjectType>(dataTypeName);
            }

            ///////////////////////////////////////////////////////////////
            // Iterate through all serialized properties and
            // use metadata to assign values to object

            var valEnumerator = info.GetEnumerator();
            while (valEnumerator.MoveNext())
            {
                var curPropName = valEnumerator.Current.Name;
                if (curPropName == PropNameDataTypeName)
                {
                    continue;
                }

                var curProp = this.DataType.GetPropertyByName(curPropName);
                if (curProp == null)
                {
                    // Property not found - create one
                    var propVal = info.GetValue(curPropName, typeof(object));
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
        /// Serializes this object to the given SerializationInfo object
        /// </summary>
        /// <param name="info">Serialiation info</param>
        /// <param name="context">Streaming context</param>
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(PropNameDataTypeName, this.DataType.FullName);

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
