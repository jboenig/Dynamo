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

using System.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Net.Http;

namespace Headway.Dynamo.Restful.Models
{
    /// <summary>
    /// Encapsulates a rest-based web service API. A
    /// rest API is a collection of restful services.
    /// </summary>
    public sealed class RestApi
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public RestApi()
        {
            this.Services = new List<RestService>();
        }

        /// <summary>
        /// Gets or sets the name of the API.
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the root endpoint URI for the
        /// rest-based API. This root endpoint URI is combined
        /// with the <see cref="RestService.RelativePath"/> to
        /// determine the endpoint of every service in the API.
        /// </summary>
        public string RootEndpointUri
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the collection of <see cref="RestService"/>
        /// objects in this API.
        /// </summary>
        public IEnumerable<RestService> Services
        {
            get;
            private set;
        }

        /// <summary>
        /// Returns the <see cref="RestService"/> matching
        /// the specified name or null if not found.
        /// </summary>
        /// <param name="serviceName">
        /// Name of service to return.
        /// </param>
        /// <returns>
        /// Returns the <see cref="RestService"/> matching the
        /// specified name or null if the service does not exist
        /// in this API.
        /// </returns>
        public RestService GetServiceByName(string serviceName)
        {
            return (from sd in this.Services
                    where sd.Name == serviceName
                    select sd).FirstOrDefault();
        }

        #region Implementation

        internal HttpClient GetHttpClient()
        {
            return new HttpClient();
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            foreach (var curSvc in this.Services)
            {
                curSvc.SetOwner(this);
            }
        }

        #endregion
    }
}
