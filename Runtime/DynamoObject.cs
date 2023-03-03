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

using System.Dynamic;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Headway.Dynamo.Metadata;
using Headway.Dynamo.Metadata.Dynamic;
using Headway.Dynamo.Exceptions;

namespace Headway.Dynamo.Runtime;

/// <summary>
/// This class derives from System.Dynamic.DynamicObject and extends
/// it with metadata provided via the <see cref="ObjectType"/>
/// interface and implements serialization.
/// </summary>
public class DynamoObject : IDynamicMetaObjectProvider,
                            IPropertyAccessor,
                            IDynamicPropertyAccessor
{
    #region Member Variables

    [JsonProperty(PropertyName = "DynamicPropertyValues")]
    private readonly PropertyValueDictionary values;

    /// <summary>
    /// Used to temporarily store name of object type
    /// during deserialization
    /// </summary>
    private string dataTypeName;

    #endregion

    #region Constructors

    /// <summary>
    /// Default constructor.
    /// </summary>
    public DynamoObject()
    {
        this.DataType = Metadata.Reflection.ReflectionObjectType.Get(this.GetType());
        this.values = new PropertyValueDictionary();
    }

    /// <summary>
    /// Constructs a <see cref="DynamoObject"/> given an
    /// <see cref="ObjectType"/> metadata object.
    /// </summary>
    /// <param name="objType">
    /// Object containing the metadata for this
    /// <see cref="DynamoObject"/> object.
    /// </param>
    public DynamoObject(ObjectType objType)
    {
        this.DataType = objType;
        this.values = new PropertyValueDictionary();
    }

    /// <summary>
    /// Constructs a copy of the specified <see cref="DynamoObject"/>
    /// object.
    /// </summary>
    /// <param name="source">Source object to copy</param>
    public DynamoObject(DynamoObject source)
    {
        this.DataType = source.DataType;
        this.values = new PropertyValueDictionary();
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
    [JsonIgnore]
    public ObjectType DataType
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the fully qualified name of the <see cref="ObjectType"/>
    /// metadata that defines the properties and metadata for this dynamic
    /// object.
    /// </summary>
    /// <seealso cref="DynamoObject.DataType"/>
    /// <remarks>
    /// Used 
    /// </remarks>
    [JsonProperty]
    internal string DataTypeName
    {
        get
        {
            if (this.DataType == null)
            {
                return this.dataTypeName;
            }
            return this.DataType.FullName;
        }
        set
        {
            if (value != this.dataTypeName)
            {
                this.dataTypeName = value;
                this.DataType = null;
            }
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

    [OnDeserialized]
    internal void OnDeserializedMethod(StreamingContext context)
    {
        ///////////////////////////////////////////////////////////////
        // Get IServiceProvider instance from streaming context
        var svcProvider = context.Context as IServiceProvider;
        if (svcProvider == null)
        {
            throw new InvalidOperationException("An instance of IServiceProvider must be attached to the StreamingContext in order to deserialize this object");
        }

        ///////////////////////////////////////////////////////////////
        // Load data type information
        var dataTypeName = this.DataTypeName;
        if (!string.IsNullOrEmpty(dataTypeName))
        {
            var metadataProvider = svcProvider.GetService(typeof(IMetadataProvider)) as IMetadataProvider;
            if (metadataProvider == null)
            {
                throw new ServiceNotFoundException(typeof(IMetadataProvider));
            }
            this.DataType = metadataProvider.GetDataType<ObjectType>(dataTypeName);
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
