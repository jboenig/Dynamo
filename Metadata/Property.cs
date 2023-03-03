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

namespace Headway.Dynamo.Metadata;

/// <summary>
/// Encapsulates a property of a <see cref="ComplexType"/>
/// </summary>
public abstract class Property : Member
{
    /// <summary>
    /// Gets the <see cref="DataType"/> of this property.
    /// </summary>
    public abstract DataType DataType
    {
        get;
    }

    /// <summary>
    /// Gets a flag indicating if the property is nullable.
    /// </summary>
    public abstract bool IsNullable
    {
        get;
    }

    /// <summary>
    /// Gets a flag indicating whether or not the property
    /// has a getter.
    /// </summary>
    public abstract bool CanRead
    {
        get;
    }

    /// <summary>
    /// Gets a flag indicating whether or not the property
    /// has a setter.
    /// </summary>
    public abstract bool CanWrite
    {
        get;
    }

    /// <summary>
    /// Gets or sets the default value of the property.
    /// </summary>
    public abstract object DefaultValue
    {
        get;
        set;
    }

    /// <summary>
    /// Gets a flag indicating whether or not this property should be serialized.
    /// </summary>
    public abstract bool Serialize
    {
        get;
    }

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
    public abstract T GetValue<T>(object source);

    /// <summary>
    /// Sets the value of this property on the specified target object.
    /// </summary>
    /// <typeparam name="T">Property type</typeparam>
    /// <param name="target">
    /// Target object to receive the property value.
    /// </param>
    /// <param name="value">Value to assign to the property</param>
    public abstract void SetValue<T>(object target, T value);
}
