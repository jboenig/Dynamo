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
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Headway.Dynamo.Serialization
{
    /// <summary>
    /// Standard implementation of <see cref="ISerializerConfigService"/>
    /// </summary>
    public sealed class StandardSerializerConfigService : ISerializerConfigService
    {
        private readonly IJsonConverterService converterService;

        /// <summary>
        /// Default constructor
        /// </summary>
        public StandardSerializerConfigService()
        {
        }

        /// <summary>
        /// Constructs a <see cref="StandardSerializerConfigService"/>
        /// given an <see cref="IJsonConverterService"/>.
        /// </summary>
        /// <param name="converterService">
        /// Service that returns a collection of JsonConverter objects.
        /// </param>
        public StandardSerializerConfigService(IJsonConverterService converterService)
        {
            this.converterService = converterService;
        }

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
        public JsonSerializerSettings ConfigureJsonSerializerSettings(
            Type objType,
            IServiceProvider svcProvider)
        {
            var jsonSettings = new JsonSerializerSettings();
            var streamingCtx = new StreamingContext(
                StreamingContextStates.Persistence,
                svcProvider);
            jsonSettings.Context = streamingCtx;
            jsonSettings.TypeNameHandling = TypeNameHandling.Objects;

            // Use IJsonConverterService to get any converters needed
            jsonSettings.Converters = (this.converterService != null) ?
                (this.converterService.GetConverters(objType)) :
                (null);

            return jsonSettings;
        }
    }
}
