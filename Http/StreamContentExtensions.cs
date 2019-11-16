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

using System.Net.Http;
using System.IO.Compression;
using System.IO;
using Newtonsoft.Json.Linq;

namespace Headway.Dynamo.Http
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
