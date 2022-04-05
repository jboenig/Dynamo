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

using System.IO.Compression;
using Newtonsoft.Json.Linq;

namespace Headway.Dynamo.Http.IO
{
    /// <summary>
    /// Extension methods for HttpContent class
    /// </summary>
    public static class StreamContentExtensions
    {
        /// <summary>
        /// Retrieve content as a string.
        /// </summary>
        /// <param name="httpContent">
        /// HttpContent object to retrieve content from.
        /// </param>
        /// <returns>
        /// Content as a string value
        /// </returns>
        public static string GetAsString(this HttpContent httpContent)
        {
            string resString = null;

            var strmTask = httpContent.ReadAsStreamAsync();
            strmTask.Wait();

            if (httpContent.Headers.ContentEncoding.Contains("gzip"))
            {
                using (var strm = strmTask.Result)
                using (var gsr = new GZipStream(strm, CompressionMode.Decompress))
                using (var sr = new StreamReader(gsr))
                {
                    resString = sr.ReadToEnd();
                }
            }
            else
            {
                using (var sr = new StreamReader(strmTask.Result))
                {
                    resString = sr.ReadToEnd();
                }
            }

            return resString;
        }

        /// <summary>
        /// Retrieve content as a Json object.
        /// </summary>
        /// <param name="httpContent">
        /// HttpContent object to retrieve content from.
        /// </param>
        /// <returns>
        /// Content as a JObject
        /// </returns>
        public static JObject GetAsJObject(this HttpContent httpContent)
        {
            var jsonString = httpContent.GetAsString();
            return JObject.Parse(jsonString);
        }
    }
}
