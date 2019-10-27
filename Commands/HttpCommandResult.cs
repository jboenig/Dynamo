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
using System.Net.Http;
using Newtonsoft.Json.Linq;
using Headway.Dynamo.RestServices;

namespace Headway.Dynamo.Commands
{
    /// <summary>
    /// Encapsulates an HttpResponseMessage as a
    /// <see cref="CommandResult"/> object.
    /// </summary>
    public sealed class HttpCommandResult : CommandResult
    {
        private readonly HttpResponseMessage response;

        /// <summary>
        /// Constructs an <see cref="HttpCommandResult"/> given
        /// an HttpResponseMessage.
        /// </summary>
        /// <param name="response">
        /// HttpResponseMessage to wrap in this command result.
        /// </param>
        public HttpCommandResult(HttpResponseMessage response)
        {
            this.response = response;
        }

        /// <summary>
        /// Gets the description.
        /// </summary>
        public override string Description
        {
            get
            {
                return this.response.ReasonPhrase;
            }
        }

        /// <summary>
        /// Gets a flag indicating whether result is successful or not.
        /// </summary>
        public override bool IsSuccess
        {
            get
            {
                return this.response.IsSuccessStatusCode;
            }
        }

        /// <summary>
        /// Gets the content from the HTTP response.
        /// </summary>
        public HttpContent Content
        {
            get
            {
                return this.response.Content;
            }
        }

        /// <summary>
        /// Returns the result content as a string.
        /// </summary>
        public string ContentAsString
        {
            get
            {
                if (this.response.Content != null && this.response.IsSuccessStatusCode)
                {
                    return this.response.Content.GetAsString();
                }
                return null;
            }
        }

        /// <summary>
        /// Returns the result content as a JSON object.
        /// </summary>
        public JObject ContentAsJObject
        {
            get
            {
                if (this.response.Content != null && this.response.IsSuccessStatusCode)
                {
                    return this.response.Content.GetAsJObject();
                }
                return null;
            }
        }
    }
}
