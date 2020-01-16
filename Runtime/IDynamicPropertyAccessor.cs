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

using Headway.Dynamo.Metadata.Dynamic;
using System;
using System.Collections.Generic;

namespace Headway.Dynamo.Runtime
{
    /// <summary>
    /// Interface to objects that have dynamic properties. This interface
    /// is used exclusively to access dynamic properties only.
    /// </summary>
    /// <remarks>
    /// This interface was introduced in order to prevent a recursive
    /// stack overflow. The <see cref="DynamicProperty"/> cannot implement
    /// GetValue and SetValue by calling back on <see cref="IPropertyAccessor"/>,
    /// because that just calls back to the property and causes a recursive
    /// stack overflow. Objects that have dynamic properties need to implement
    /// this interface.
    /// </remarks>
    /// <see cref="DynamicProperty"/>
    public interface IDynamicPropertyAccessor
    {
        /// <summary>
        /// Gets the value of the specified property
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        T GetPropertyValue<T>(string propertyName);

        /// <summary>
        /// Sets the value of the specified property.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        void SetPropertyValue<T>(string propertyName, T value);
    }
}
