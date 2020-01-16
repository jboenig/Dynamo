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
using Newtonsoft.Json;

namespace Headway.Dynamo.Serialization
{
    /// <summary>
    /// Interface to service that configures JsonSerializerSettings
    /// to be used during Json serialization.
    /// </summary>
    public interface ISerializerConfigService
    {
        /// <summary>
        /// Returns a JsonSerializerSettings object to be used
        /// for the Json serializer.
        /// </summary>
        /// <param name="objType">
        /// Type of object to be serialized.
        /// </param>
        /// <param name="svcProvider">
        /// Service provider to attach.
        /// </param>
        /// <returns>
        /// Returns a JsonSerializerSettings object to be used
        /// by the Json serializer.
        /// </returns>
        JsonSerializerSettings ConfigureJsonSerializerSettings(
            Type objType,
            IServiceProvider svcProvider);
    }
}
