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
/// Interface to object that contains named
/// Attributes.
/// </summary>
public interface IAttributeContainer
{
    /// <summary>
    /// Gets all attributes matching the specified name.
    /// </summary>
    /// <typeparam name="T">
    /// Type of attributes to return.
    /// </typeparam>
    /// <param name="attributeName">
    /// Name of attributes to get
    /// </param>
    /// <returns>
    /// Collection of attributes matching the specified name.
    /// </returns>
    IEnumerable<T> GetAttributes<T>(string attributeName) where T : Attribute;

    /// <summary>
    /// Gets a list of the names of all available attributes.
    /// </summary>
    /// <returns>Enumerable collection of attribute names.</returns>
    IEnumerable<string> GetAttributeNames();
}

/// <summary>
/// 
/// </summary>
public static class AttributeContainerHelper
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="attributeContainer"></param>
    /// <param name="attributeName"></param>
    /// <returns></returns>
    public static T GetAttribute<T>(this IAttributeContainer attributeContainer, string attributeName) where T : Attribute
    {
        return attributeContainer.GetAttributes<T>(attributeName).FirstOrDefault();
    }
}
