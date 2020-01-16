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

using System;
using System.Text;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Headway.Dynamo.Metadata;
using Headway.Dynamo.Runtime;

namespace Headway.Dynamo.Restful.Models
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
