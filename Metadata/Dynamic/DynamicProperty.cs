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

using System.Runtime.Serialization;
using Headway.Dynamo.Runtime;

namespace Headway.Dynamo.Metadata.Dynamic;

/// <summary>
/// Implements <see cref="Property"/> for properties that can be dynamically
/// created at run-time and serialized.
/// </summary>
[Serializable]
public sealed class DynamicProperty : Property, ISerializable
{
    #region Member Variables

    private ComplexType owner;
    private string name;
    private DataType dataType;
    private object defaultValue;

    #endregion

    #region Constructors

    private DynamicProperty(ComplexType owner, string name, DataType dataType)
    {
        if (owner == null)
        {
            throw new ArgumentNullException("owner", "DynamicProperty objects must have an owner");
        }

        if (string.IsNullOrEmpty(name))
        {
            throw new ArgumentNullException("name", "DynamicProperty objects must have a name");
        }

        if (dataType == null)
        {
            throw new ArgumentNullException("dataType", "DataType must be specified for DynamicProperty objects");
        }

        this.owner = owner;
        this.name = name;
        this.dataType = dataType;
        this.defaultValue = this.dataType.DefaultValue;
    }

    private DynamicProperty(ComplexType owner, string name, DataType dataType, object defaultValue)
    {
        if (owner == null)
        {
            throw new ArgumentNullException("owner", "DynamicProperty objects must have an owner");
        }

        if (string.IsNullOrEmpty(name))
        {
            throw new ArgumentNullException("name", "DynamicProperty objects must have a name");
        }

        if (dataType == null)
        {
            throw new ArgumentNullException("dataType", "DataType must be specified for DynamicProperty objects");
        }

        this.owner = owner;
        this.name = name;
        this.dataType = dataType;
        this.defaultValue = defaultValue;
    }

    #endregion

    #region Create Methods

    /// <summary>
    /// Creates a new <see cref="DynamicProperty"/> given a name, data type, and serialization
    /// flag.
    /// </summary>
    /// <param name="owner"></param>
    /// <param name="name"></param>
    /// <param name="dataType"></param>
    /// <returns></returns>
    public static DynamicProperty Create(ComplexType owner, string name, DataType dataType)
    {
        return new DynamicProperty(owner, name, dataType, dataType.DefaultValue);
    }

    /// <summary>
    /// Creates a new <see cref="DynamicProperty"/> given a name, data type, and serialization
    /// flag.
    /// </summary>
    /// <param name="owner"></param>
    /// <param name="name"></param>
    /// <param name="dataType"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    public static DynamicProperty Create(ComplexType owner, string name, DataType dataType, object defaultValue = null)
    {
        return new DynamicProperty(owner, name, dataType, defaultValue);
    }

    #endregion

    #region Public Properties

    /// <summary>
    /// Gets the owner of this member.
    /// </summary>
    public override ComplexType Owner
    {
        get
        {
            return this.owner;
        }
    }

    /// <summary>
    /// Gets the name of the property.
    /// </summary>
    public override string Name
    {
        get { return this.name; }
    }

    /// <summary>
    /// Gets the <see cref="DataType"/> of the property.
    /// </summary>
    public override DataType DataType
    {
        get { return this.dataType; }
    }

    /// <summary>
    /// 
    /// </summary>
    public override bool IsNullable
    {
        get
        {
            if (this.DataType.CLRType.IsValueType)
            {
                return false;
            }
            return true;
        }
    }

    /// <summary>
    /// Gets or sets the default value for this property.
    /// </summary>
    public override object DefaultValue
    {
        get
        {
            return this.defaultValue;
        }
        set
        {
            this.defaultValue = value;
        }
    }

    /// <summary>
    /// Gets a flag indicating whether or not the property
    /// has a getter.
    /// </summary>
    public override bool CanRead => true;

    /// <summary>
    /// Gets a flag indicating whether or not the property
    /// has a setter.
    /// </summary>
    public override bool CanWrite => true;

    /// <summary>
    /// Gets a flag indicating whether or not this property should be serialized.
    /// </summary>
    public override bool Serialize
    {
        get
        {
            return true;
        }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <returns></returns>
    public override T GetValue<T>(object source)
    {
        var propAccessor = source as IDynamicPropertyAccessor;
        if (propAccessor == null)
        {
            throw new InvalidOperationException("Source object for GetValue must implement IDynamicPropertyAccessor");
        }
        return propAccessor.GetPropertyValue<T>(this.Name);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="target"></param>
    /// <param name="value"></param>
    public override void SetValue<T>(object target, T value)
    {
        var propAccessor = target as IDynamicPropertyAccessor;
        if (propAccessor == null)
        {
            throw new InvalidOperationException("Target object for SetValue must implement IDynamicPropertyAccessor");
        }
        propAccessor.SetPropertyValue<T>(this.Name, value);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="attributeName"></param>
    /// <returns></returns>
    public override IEnumerable<T> GetAttributes<T>(string attributeName)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Gets a list of the names of all available attributes.
    /// </summary>
    /// <returns>Enumerable collection of attribute names.</returns>
    public override IEnumerable<string> GetAttributeNames()
    {
        return new string[] { };
    }

    #endregion

    #region Serialization

    private DynamicProperty(SerializationInfo info, StreamingContext context)
    {
        this.name = info.GetString("name");

        var clrTypeName = info.GetString("clrType");
        if (!string.IsNullOrEmpty(clrTypeName))
        {
            this.dataType = IntegralType.Get(clrTypeName);
        }

        this.defaultValue = info.GetValue("defaultValue", this.DataType.CLRType);
    }

    void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
    {
        info.AddValue("name", this.name);

        var integralType = this.dataType as IntegralType;
        if (integralType == null)
        {
            throw new NotSupportedException("Serialization of complex types not implemented yet");
        }
        info.AddValue("clrType", this.dataType.CLRTypeName);
        info.AddValue("defaultValue", this.defaultValue);
    }

    /// <summary>
    /// This method must be called after deserializing
    /// a <see cref="DynamicProperty"/> so that the parent relationship
    /// can be re-established.
    /// </summary>
    /// <param name="owner">
    /// Reference to the <see cref="ComplexType"/> that owns this
    /// property
    /// </param>
    public void SetOwner(ComplexType owner)
    {
        this.owner = owner;
    }

    #endregion
}
