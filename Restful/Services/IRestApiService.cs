////////////////////////////////////////////////////////////////////////////////
// Copyright 2019 Jeff Boenig
//
// This file is part of Headway.Dynamo.Restful.
//
// Headway.Dynamo.Restful is free software: you can redistribute it and/or
// modify it under the terms of the GNU General Public License as published
// by the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// Headway.Dynamo.Restful is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR PARTICULAR PURPOSE. See the GNU General
// Public License for more details.
//
// You should have received a copy of the GNU General Public License along with
// Headway.Dynamo.Restful. If not, see http://www.gnu.org/licenses/.
////////////////////////////////////////////////////////////////////////////////

using System.Threading.Tasks;
using System.Net.Http;
using Headway.Dynamo.Restful.Models;

namespace Headway.Dynamo.Restful.Services
{
    /// <summary>
    /// Service interface for discovering and invoking Restful
    /// web services.
    /// </summary>
    public interface IRestApiService
    {
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
        RestApi GetApiByName(string apiName);

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
        /// <param name="paramObj">
        /// Parameter object containing properties that match the
        /// parameter names declared in the service definition.
        /// </param>
        /// <param name="contentObj">
        /// Object containing content to send with web service request.
        /// </param>
        /// <returns>
        /// Returns a Task that executes the call to the web service
        /// and returns an HttpResponseMessage.
        /// </returns>
        Task<HttpResponseMessage> Invoke(string apiName, string serviceName, object paramObj, object contentObj = null);
    }
}
