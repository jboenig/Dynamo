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

using Headway.Dynamo.Metadata;

namespace Headway.Dynamo.Exceptions;

/// <summary>
/// This exception is thrown when a property is referenced that is not
/// declared on the specified <see cref="ComplexType"/>.
/// </summary>
public sealed class PropertyNotFoundException : Exception
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="dataType"></param>
    /// <param name="propertyName"></param>
    public PropertyNotFoundException(ComplexType dataType, string propertyName) :
        base(string.Format("Property name {0} does not exist on data type {1}", propertyName, dataType.Name))
    {
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="clrType"></param>
    /// <param name="propertyName"></param>
    public PropertyNotFoundException(Type clrType, string propertyName) :
        base(string.Format("Property name {0} does not exist on data type {1}", propertyName, clrType.Name))
    {
    }
}
