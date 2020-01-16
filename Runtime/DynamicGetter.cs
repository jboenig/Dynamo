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

namespace Headway.Dynamo.Runtime
{
    /// <summary>
    /// This class provides an abstract interface to objects that
    /// can be used to retrieve a piece of data from a dynamic
    /// context.
    /// </summary>
    /// <typeparam name="T">Type of data to retrieve</typeparam>
    /// <remarks>
    /// The purpose of dynamic getters is to allow the consumer of
    /// the getter to retrieve a piece of data from the dynamic
    /// context without any knowledge of how or where it is stored
    /// in the context.
    /// </remarks>
    public abstract class DynamicGetter<T>
    {
        /// <summary>
        /// Returns the value in the dynamic context that the
        /// getter is designed to retrieve.
        /// </summary>
        /// <param name="serviceProvider">Interface to service provider</param>
        /// <param name="context">User defined context data</param>
        /// <returns>
        /// A value from the dynamic context of type T.
        /// </returns>
        public abstract T Get(IServiceProvider serviceProvider, dynamic context);
    }
}
