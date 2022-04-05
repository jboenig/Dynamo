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

using Headway.Dynamo.Exceptions;
using Headway.Dynamo.Commands;
using Headway.Dynamo.Runtime;
using Headway.Dynamo.Http;
using Headway.Dynamo.Restful.Services;

namespace Headway.Dynamo.Restful.Commands
{
    /// <summary>
    /// Implements the <see cref="Command"/> pattern for a
    /// rest-based web service call.
    /// </summary>
    public sealed class CallRestServiceCommand : Command
    {
        /// <summary>
        /// Gets or sets the name of the rest-based API.
        /// </summary>
        public string ApiName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name of the rest-based service.
        /// </summary>
        public string ServiceName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name of the property on the
        /// context object that contains the content (if post)
        /// to be sent to with the web service request.
        /// </summary>
        public string RequestContentPropertyName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name of the property on the
        /// context object to receive the content of the
        /// web service response.
        /// </summary>
        public string ResponseContentPropertyName
        {
            get;
            set;
        }

        /// <summary>
        /// Executes the call to the configured REST-based
        /// web service.
        /// </summary>
        /// <param name="serviceProvider">Interface to service provider</param>
        /// <param name="context">User defined context data</param>
        /// <returns>
        /// Returns a <see cref="CommandResult"/> object that describes
        /// the result.
        /// </returns>
        public override async Task<CommandResult> ExecuteAsync(IServiceProvider serviceProvider, object context)
        {
            var restApiService = serviceProvider.GetService(typeof(IRestApiService)) as IRestApiService;
            if (restApiService == null)
            {
                throw new ServiceNotFoundException(typeof(IRestApiService));
            }

            object contentObj = null;

            if (!string.IsNullOrEmpty(this.RequestContentPropertyName))
            {
                // Get content to pass to web service
                contentObj = PropertyResolver.GetPropertyValue<object>(context, this.RequestContentPropertyName);
            }

            var response = await restApiService.Invoke(this.ApiName, this.ServiceName, context, contentObj);
            if (response.IsSuccessStatusCode)
            {
                if (!string.IsNullOrEmpty(this.ResponseContentPropertyName))
                {
                    // Push response to context object
                    var resultJsonObj = response.Content.GetAsJObject();
                    var setPropValCmd = new SetPropertyValueCommand()
                    {
                        PropertyName = this.ResponseContentPropertyName,
                        Value = resultJsonObj
                    };
                    await setPropValCmd.ExecuteAsync(serviceProvider, context);
                }
            }
            return new HttpCommandResult(response);
        }
    }
}
