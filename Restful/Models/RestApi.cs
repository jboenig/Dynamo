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
