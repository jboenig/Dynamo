﻿////////////////////////////////////////////////////////////////////////////////
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

using System.Reflection;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace Headway.Dynamo.Metadata.Reflection;

/// <summary>
/// Implements <see cref="Property"/> using reflection.
/// </summary>
internal sealed class ReflectionProperty : Property
{
    #region Member Variables

    private readonly IMetadataProvider metadataProvider;
    private readonly ComplexType owner;
    private PropertyInfo propInfo;
    private DataType dataType;
    private object defaultValue;
    private bool? isSerializable;

    #endregion

    #region Constructors

    private ReflectionProperty(IMetadataProvider metadataProvider, ComplexType owner, PropertyInfo propInfo)
    {
        if (propInfo == null)
        {
            throw new ArgumentNullException(nameof(propInfo));
        }
        if (owner == null)
        {
            throw new ArgumentNullException(nameof(owner));

        }

        this.metadataProvider = metadataProvider;
        this.owner = owner;
        this.propInfo = propInfo;
        this.defaultValue = null;
    }

    #endregion

    #region Create Methods

    /// <summary>
    /// Creates a new <see cref="ReflectionProperty"/> given a PropertyInfo
    /// object retrieved through .NET reflection.
    /// </summary>
    /// <param name="metadataProvider">
    /// Reference to <see cref="IMetadataProvider"/> service used to
    /// resolve type information.
    /// </param>
    /// <param name="owner">Owner of the property</param>
    /// <param name="propInfo">Property info object to associate with this property.</param>
    /// <returns>A new <see cref="ReflectionProperty"/> object</returns>
    public static ReflectionProperty Create(IMetadataProvider metadataProvider, ComplexType owner, PropertyInfo propInfo)
    {
        return new ReflectionProperty(metadataProvider, owner, propInfo);
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
        get { return this.propInfo.Name; }
    }

    /// <summary>
    /// Gets the <see cref="DataType"/> of the property.
    /// </summary>
    public override DataType DataType
    {
        get
        {
            if (this.dataType == null)
            {
                this.dataType = IntegralType.Get(this.propInfo.PropertyType);
                if (this.dataType == null)
                {
                    this.dataType = this.metadataProvider.GetDataType(this.propInfo.PropertyType.FullName);
                }
            }

            return this.dataType;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public override bool IsNullable
    {
        get
        {
            if (this.propInfo.PropertyType.IsValueType)
            {
                return false;
            }
            return true;
        }
    }

    /// <summary>
    /// Gets a flag indicating whether or not the property
    /// has a getter.
    /// </summary>
    public override bool CanRead
    {
        get { return this.propInfo.CanRead; }
    }

    /// <summary>
    /// Gets a flag indicating whether or not the property
    /// has a setter.
    /// </summary>
    public override bool CanWrite
    {
        get { return this.propInfo.CanWrite; }
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
    /// Gets a flag indicating whether or not this property should be serialized.
    /// </summary>
    public override bool Serialize
    {
        get
        {
            if (this.isSerializable.HasValue)
            {
                return this.isSerializable.Value;
            }

            this.isSerializable = true;

            // Property must have read and write access
            if (!this.propInfo.CanWrite || !this.propInfo.CanRead)
            {
                this.isSerializable = false;
            }

            var jsonIgnoreAttrs = this.propInfo.GetCustomAttributes(typeof(JsonIgnoreAttribute), true);
            if (jsonIgnoreAttrs != null && jsonIgnoreAttrs.Length > 0)
            {
                this.isSerializable = false;
            }

            var xmlIgnoreAttrs = this.propInfo.GetCustomAttributes(typeof(XmlIgnoreAttribute), true);
            if (xmlIgnoreAttrs != null && xmlIgnoreAttrs.Length > 0)
            {
                this.isSerializable = false;
            }

            return this.isSerializable.Value;
        }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Gets the value of this property from the specified source object.
    /// </summary>
    /// <typeparam name="T">Property type</typeparam>
    /// <param name="source">
    /// Source object that contains the property.
    /// </param>
    /// <returns>
    /// Value of the property
    /// </returns>
    public override T GetValue<T>(object source)
    {
        return (T)this.propInfo.GetValue(source, null);
    }

    /// <summary>
    /// Sets the value of this property on the specified target object.
    /// </summary>
    /// <typeparam name="T">Property type</typeparam>
    /// <param name="target">
    /// Target object to receive the property value.
    /// </param>
    /// <param name="value">Value to assign to the property</param>
    public override void SetValue<T>(object target, T value)
    {
        this.propInfo.SetValue(target, value, null);
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
}
