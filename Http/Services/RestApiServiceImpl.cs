////////////////////////////////////////////////////////////////////////////////
// The MIT License(MIT)
// Copyright(c) 2020 Jeff Boenig
// This file is part of Headway.Dynamo.Restful
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

using Headway.Dynamo.Http.Models;

namespace Headway.Dynamo.Http.Services
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
        public Task<HttpResponseMessage> Invoke(string apiName, string serviceName, object paramObj, object contentObj = null)
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

            return restSvc.Invoke(paramObj, contentObj);
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
