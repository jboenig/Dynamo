////////////////////////////////////////////////////////////////////////////////
// Copyright 2019 Jeff Boenig
//
// This file is part of Headway.Dynamo.
//
// Headway.Dynamo is free software: you can redistribute it and/or modify it under
// the terms of the GNU General Public License as published by the Free Software
// Foundation, either version 3 of the License, or (at your option) any later
// version.
//
// Headway.Dynamo is distributed in the hope that it will be useful, but WITHOUT
// ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
// FOR PARTICULAR PURPOSE. See the GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License along with
// Headway.Dynamo. If not, see http://www.gnu.org/licenses/.
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
