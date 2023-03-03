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

namespace Headway.Dynamo.Conditions;

/// <summary>
/// Comparions operators that can be used to compare
/// two property values.
/// </summary>
/// <seealso cref="PropertyCompare"/>
public enum PropertyCompareOps
{
    /// <summary>
    /// Test for equality
    /// </summary>
    AreEqual,

    /// <summary>
    /// Test for inequality
    /// </summary>
    AreNotEqual,

    /// <summary>
    /// Test for greater than
    /// </summary>
    GreaterThan,

    /// <summary>
    /// Test for less than
    /// </summary>
    LessThan,

    /// <summary>
    /// Test for greater than or equal
    /// </summary>
    GreaterThanOrEqual,

    /// <summary>
    /// Test for less than or equal
    /// </summary>
    LessThanOrEqual,

    /// <summary>
    /// Test to see if a string contains another string
    /// </summary>
    Contains,

    /// <summary>
    /// Test to see if a string starts with another string
    /// </summary>
    StartsWith,

    /// <summary>
    /// Test to see if a string ends with another string
    /// </summary>
    EndsWith
}
