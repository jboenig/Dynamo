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

using System;
using Headway.Dynamo.Metadata.Reflection;
using Headway.Dynamo.Metadata.Dynamic;

namespace Headway.Dynamo.Metadata;

/// <summary>
/// Implements an <see cref="IMetadataProvider"/> that combines
/// a <see cref="DynamicMetadataProvider"/> and a
/// <see cref="ReflectionMetadataProvider"/>.
/// </summary>
public sealed class StandardMetadataProvider : IMetadataProvider
{
    private readonly ReflectionMetadataProvider reflectionProvider;
    private readonly DynamicMetadataProvider dynamicProvider;

    /// <summary>
    /// 
    /// </summary>
    public StandardMetadataProvider()
    {
        this.reflectionProvider = new ReflectionMetadataProvider();
        this.dynamicProvider = new DynamicMetadataProvider(this.reflectionProvider);
    }

    /// <summary>
    /// Gets a <see cref="DataType"/> by fully
    /// qualified name.
    /// </summary>
    /// <param name="fullName">Full name of the data type to retrieve</param>
    /// <returns>
    /// The <see cref="DataType"/> matching the specified name or null
    /// if the data type does not exist.
    /// </returns>
    public T GetDataType<T>(string fullName) where T : DataType
    {
        return this.dynamicProvider.GetDataType<T>(fullName);
    }

    /// <summary>
    /// Gets the <see cref="DynamicMetadataProvider"/>
    /// </summary>
    public DynamicMetadataProvider DynamicProvider
    {
        get { return this.dynamicProvider; }
    }

    /// <summary>
    /// Gets the <see cref="ReflectionMetadataProvider"/>
    /// </summary>
    public ReflectionMetadataProvider ReflectionProvider
    {
        get { return this.reflectionProvider; }
    }
}
