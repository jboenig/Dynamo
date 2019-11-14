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
using Headway.Dynamo.Metadata;

namespace Headway.Dynamo.Serialization
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class StandardSerializerConfigService : ISerializerConfigService
    {
        private IJsonConverterService converterService;

        /// <summary>
        /// 
        /// </summary>
        public StandardSerializerConfigService()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="converterService"></param>
        public StandardSerializerConfigService(IJsonConverterService converterService)
        {
            this.converterService = converterService;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objType"></param>
        /// <param name="svcProvider"></param>
        /// <returns></returns>
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
            jsonSettings.Converters = (this.converterService != null) ?
                (this.converterService.GetConverters(objType)) :
                (null);
            return jsonSettings;
        }
    }
}
