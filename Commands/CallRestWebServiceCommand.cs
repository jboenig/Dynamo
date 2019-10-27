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
using System.Threading.Tasks;
using System.Net.Http;
using Headway.Dynamo.RestServices;
using Headway.Dynamo.Exceptions;

namespace Headway.Dynamo.Commands
{
    /// <summary>
    /// Implements the <see cref="Command"/> pattern for a
    /// rest-based web service call.
    /// </summary>
    public sealed class CallRestWebServiceCommand : Command
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
                this.taskWebServiceCall = restApiService.Invoke(this.ApiName, this.ServiceName, context);
                this.taskWebServiceCall.Wait();
                return new HttpCommandResult(this.taskWebServiceCall.Result);
            });
        }
    }
}
