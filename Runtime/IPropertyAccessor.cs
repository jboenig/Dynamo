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

namespace Headway.Dynamo.Runtime
{
    /// <summary>
    /// Interface to objects that have dynamic properties.
    /// </summary>
    public interface IPropertyAccessor
    {
        /// <summary>
        /// Gets the value of the specified property
        /// </summary>
        /// <typeparam name="T">
        /// Type of value to return.
        /// </typeparam>
        /// <param name="propertyName">
        /// Name of property to retrieve.
        /// </param>
        /// <returns>
        /// Returns the value of the property or null
        /// if not found.
        /// </returns>
        T GetPropertyValue<T>(string propertyName);

        /// <summary>
        /// Sets the value of the specified property.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        /// <returns>Reference to this</returns>
        IPropertyAccessor SetPropertyValue<T>(string propertyName, T value);

        /// <summary>
        /// Returns a collection of all available property names.
        /// </summary>
        /// <returns>
        /// An enumerable of property names.
        /// </returns>
        IEnumerable<string> GetPropertyNames();
    }
}
