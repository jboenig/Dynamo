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

using System;
using System.Threading.Tasks;
using System.Net.Http;
using Headway.Dynamo.Exceptions;
using Headway.Dynamo.Commands;
using Headway.Dynamo.Runtime;
using Headway.Dynamo.Restful.Services;

namespace Headway.Dynamo.Restful.Commands
{
    /// <summary>
    /// Implements the <see cref="Command"/> pattern for a
    /// rest-based web service call.
    /// </summary>
    public sealed class CallRestServiceCommand : Command
    {
        private Task<HttpResponseMessage> taskWebServiceCall;

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
        public override Task<CommandResult> Execute(IServiceProvider serviceProvider, object context)
        {
            var restApiService = serviceProvider.GetService(typeof(IRestApiService)) as IRestApiService;
            if (restApiService == null)
            {
                throw new ServiceNotFoundException(typeof(IRestApiService));
            }

            return new Task<CommandResult>(() =>
            {
                object contentObj = null;

                if (!string.IsNullOrEmpty(this.RequestContentPropertyName))
                {
                    // Get content to pass to web service
                    contentObj = PropertyResolver.GetPropertyValue<object>(context, this.RequestContentPropertyName);
                }

                this.taskWebServiceCall = restApiService.Invoke(this.ApiName, this.ServiceName, context, contentObj);
                this.taskWebServiceCall.Wait();
                if (this.taskWebServiceCall.Result.IsSuccessStatusCode)
                {
                    if (!string.IsNullOrEmpty(this.ResponseContentPropertyName))
                    {
                        // Push response to context object
                        var resultJsonObj = this.taskWebServiceCall.Result.Content.GetAsJObject();
                        var setPropValCmd = new SetPropertyValueCommand()
                        {
                            PropertyName = this.ResponseContentPropertyName,
                            Value = resultJsonObj
                        };
                        setPropValCmd.Execute(serviceProvider, context).RunSynchronously();
                    }
                }
                return new HttpCommandResult(this.taskWebServiceCall.Result);
            });
        }
    }
}
