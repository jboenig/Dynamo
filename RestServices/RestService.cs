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
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using Headway.Dynamo.Metadata;
using Headway.Dynamo.Runtime;

namespace Headway.Dynamo.RestServices
{
    /// <summary>
    /// This class encapsulates a rest-based web service.
    /// </summary>
    public sealed class RestService
    {
        private RestApi owner;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public RestService()
        {
            this.Parameters = new List<Parameter>();
            this.Version = 1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="owner"></param>
        public RestService(RestApi owner)
        {
            this.owner = owner;
            this.Parameters = new List<Parameter>();
            this.Version = 1;
        }

        /// <summary>
        /// Gets or sets the name of the service.
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the relative path to the
        /// rest-based web service.
        /// </summary>
        public string RelativePath
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the version of the service.
        /// </summary>
        public int Version
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the parameters to the service.
        /// </summary>
        public IEnumerable<Parameter> Parameters
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the URI for this service.
        /// </summary>
        /// <param name="parameters">
        /// Parameter object containing property names
        /// that match the declared parameters for this
        /// service.
        /// </param>
        public Uri GetUri(object parameters)
        {
            if (this.owner == null)
            {
                throw new InvalidOperationException();
            }

            var uriStringBuilder = new StringBuilder();
            uriStringBuilder.Append(this.owner.RootEndpointUri);
            if (!this.owner.RootEndpointUri.EndsWith("/"))
            {
                uriStringBuilder.Append("/");
            }
            uriStringBuilder.Append(this.RelativePath);

            /////////////////////////////////////////////////////////////////////////////
            // HACK: Find generic way to do this
            if (this.Parameters.Count() == 1)
            {
                var param0Value = PropertyResolver.GetPropertyValue<object>(parameters, this.Parameters.First().Name);
                uriStringBuilder.Append("/");
                uriStringBuilder.Append(param0Value);
            }

            return new Uri(uriStringBuilder.ToString());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public Task<HttpResponseMessage> Invoke(object parameters)
        {
            if (this.owner == null)
            {
                throw new InvalidOperationException();
            }

            HttpClient httpClient = this.owner.GetHttpClient();
            var uri = this.GetUri(parameters);

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, uri);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("text/html"));
            request.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
            request.Headers.AcceptCharset.Add(new StringWithQualityHeaderValue("UTF-8"));
            request.Headers.AcceptLanguage.Add(new StringWithQualityHeaderValue("english"));

            return httpClient.SendAsync(request);
        }

        internal void SetOwner(RestApi restApi)
        {
            this.owner = restApi;
        }
    }
}
