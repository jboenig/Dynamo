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
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
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
            this.owner = null;
            this.Parameters = new List<Parameter>();
            this.Version = 1;
            this.Post = false;
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
            this.Post = false;
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
        /// Gets or sets a flag indicating whether or not
        /// to post or get.
        /// </summary>
        public bool Post
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the URI for this service.
        /// </summary>
        /// <param name="paramObj">
        /// Parameter object containing property names
        /// that match the declared parameters for this
        /// service.
        /// </param>
        public Uri GetUri(object paramObj)
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

            var pathWithPathParamValues = PropertyResolver.ResolvePropertyValues(paramObj, this.RelativePath);
            uriStringBuilder.Append(pathWithPathParamValues);

            // Append URL query string parameters
            int numQueryStringParams = 0;
            foreach (var curParam in this.Parameters)
            {
                if (curParam is UrlQueryStringParameter)
                {
                    var curParamVal = PropertyResolver.GetPropertyValue<object>(paramObj, curParam.Name);
                    if (numQueryStringParams == 0)
                    {
                        uriStringBuilder.Append("?");
                    }
                    else
                    {
                        uriStringBuilder.Append("&");
                    }
                    uriStringBuilder.Append(curParam.Name);
                    uriStringBuilder.Append("=");
                    uriStringBuilder.Append(curParamVal.ToString());
                    numQueryStringParams++;
                }
            }

            return new Uri(uriStringBuilder.ToString());
        }

        /// <summary>
        /// Invokes this restful web service given a parameters object.
        /// </summary>
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
        public Task<HttpResponseMessage> Invoke(object paramObj, object contentObj = null)
        {
            if (this.owner == null)
            {
                throw new InvalidOperationException();
            }

            HttpClient httpClient = this.owner.GetHttpClient();
            var uri = this.GetUri(paramObj);

            HttpRequestMessage request = new HttpRequestMessage(this.GetHttpMethod(), uri);
            //request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("text/html"));
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));
            request.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
            request.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("deflate"));
            request.Headers.AcceptCharset.Add(new StringWithQualityHeaderValue("UTF-8"));
            request.Headers.AcceptLanguage.Add(new StringWithQualityHeaderValue("english"));
            request.Headers.CacheControl = new CacheControlHeaderValue()
            {
                NoCache = true
            };
            request.Headers.Connection.Add("keep-alive");
            request.Headers.UserAgent.Add(new ProductInfoHeaderValue("Dynamo", "1.0.0"));
//            request.Headers.Range = new RangeHeaderValue(null, 10000);

            if (this.Post && contentObj != null)
            {
                request.Content = new StringContent(
                                JsonConvert.SerializeObject(contentObj),
                                Encoding.UTF8, "application/json");
            }

            return httpClient.SendAsync(request);
        }

        private HttpMethod GetHttpMethod()
        {
            if (this.Post)
            {
                return HttpMethod.Post;
            }
            return HttpMethod.Get;
        }

        internal void SetOwner(RestApi restApi)
        {
            this.owner = restApi;
        }
    }
}
