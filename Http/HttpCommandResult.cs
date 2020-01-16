////////////////////////////////////////////////////////////////////////////////
// The MIT License(MIT)
// Copyright(c) 2020 Jeff Boenig
// This file is part of Headway.Dynamo
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

using System.Net.Http;
using Newtonsoft.Json.Linq;
using Headway.Dynamo.Commands;

namespace Headway.Dynamo.Http
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
