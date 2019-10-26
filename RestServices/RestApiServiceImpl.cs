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
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.Collections.Generic;

namespace Headway.Dynamo.RestServices
{
    /// <summary>
    /// Basic implementation of the <see cref="IRestApiService"/>
    /// interface.
    /// </summary>
    public sealed class RestApiServiceImpl : IRestApiService
    {
        private IEnumerable<RestApi> restServiceApis;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public RestApiServiceImpl()
        {
            this.restServiceApis = null;
        }

        /// <summary>
        /// Gets the <see cref="RestApi"/> matching the specified name.
        /// </summary>
        /// <param name="apiName">
        /// Name of API to get.
        /// </param>
        /// <returns>
        /// Returns the <see cref="RestApi"/> matching the specified name
        /// or null if the API is not found.
        /// </returns>
        public RestApi GetApiByName(string apiName)
        {
            if (this.restServiceApis != null)
            {
                return (from a in this.restServiceApis
                        where a.Name == apiName
                        select a).FirstOrDefault();
            }
            return null;
        }

        /// <summary>
        /// Invokes a restful web service given the name of the API,
        /// name of the service, and a parameters object.
        /// </summary>
        /// <param name="apiName">
        /// Name of the API containing the service to invoke.
        /// </param>
        /// <param name="serviceName">
        /// Name of the service to invoke.
        /// </param>
        /// <param name="parameters">
        /// Parameter object containing properties that match the
        /// parameter names declared in the service definition.
        /// </param>
        /// <returns>
        /// Returns a Task that executes the call to the web service
        /// and returns an HttpResponseMessage.
        /// </returns>
        public Task<HttpResponseMessage> Invoke(string apiName, string serviceName, object parameters)
        {
            var restApi = this.GetApiByName(apiName);
            if (restApi == null)
            {
                var msg = string.Format("API not found - {0}", apiName);
                throw new ArgumentException(msg, nameof(apiName));
            }

            var restSvc = restApi.GetServiceByName(serviceName);
            if (restSvc == null)
            {
                var msg = string.Format("Rest service not found - {0}", serviceName);
                throw new ArgumentException(msg, nameof(serviceName));
            }

            return restSvc.Invoke(parameters);
        }

        /// <summary>
        /// Loads the service with <see cref="RestApi"/> objects.
        /// </summary>
        /// <param name="source">
        /// Source collection containing <see cref="RestApi"/>
        /// objects to load.
        /// </param>
        public void Load(IEnumerable<RestApi> source)
        {
            this.restServiceApis = source;
        }
    }
}
