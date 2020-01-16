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

using System;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using Headway.Dynamo.Runtime;
using Headway.Dynamo.Metadata;
using Headway.Dynamo.Exceptions;

namespace Headway.Dynamo.Serialization
{
    /// <summary>
    /// Implements <see cref="IObjectResolver{TKey, TResult}"/> for
    /// the specified object type using a JSON file stored as an
    /// assembly resource.
    /// </summary>
    /// <typeparam name="TResult">
    /// Type of objects to resolve.
    /// </typeparam>
    /// <remarks>
    /// This resolver is useful for JSON objects that are known
    /// at compile-time and can be compiled into an assembly resource.
    /// </remarks>
    public sealed class JsonResourceObjectResolver<TResult> : IObjectResolver<string, TResult> where TResult : class
    {
        private Assembly sourceAssembly;
        private IServiceProvider svcProvider;

        /// <summary>
        /// Constructs a <see cref="JsonResourceObjectResolver{TResult}"/>
        /// given a service provider and source assembly.
        /// </summary>
        /// <param name="svcProvider">
        /// Service provider used for deserialization
        /// </param>
        /// <param name="sourceAssembly">
        /// Reference to assembly that contains the JSON file
        /// as a resource.
        /// </param>
        public JsonResourceObjectResolver(IServiceProvider svcProvider,
            Assembly sourceAssembly)
        {
            if (svcProvider == null)
            {
                throw new ArgumentNullException(nameof(svcProvider));
            }
            this.svcProvider = svcProvider;

            if (sourceAssembly == null)
            {
                throw new ArgumentNullException(nameof(sourceAssembly));
            }
            this.sourceAssembly = sourceAssembly;
        }

        /// <summary>
        /// Constructs a <see cref="JsonResourceObjectResolver{TResult}"/>
        /// given a service provider.
        /// </summary>
        /// <param name="svcProvider">
        /// Service provider used for deserialization
        /// </param>
        /// <remarks>
        /// Source assembly is determined by calling
        /// Assembly.GetEntryAssembly().
        /// </remarks>
        public JsonResourceObjectResolver(IServiceProvider svcProvider)
        {
            if (svcProvider == null)
            {
                throw new ArgumentNullException(nameof(svcProvider));
            }
            this.svcProvider = svcProvider;
            this.sourceAssembly = Assembly.GetEntryAssembly();
        }

        /// <summary>
        /// Finds the object matching the specified key value.
        /// </summary>
        /// <param name="key">
        /// Key value of object to resolve.
        /// </param>
        /// <returns>
        /// Returns the object matching the specified key or null
        /// if the object is not found.
        /// </returns>
        public TResult Resolve(string key)
        {
            TResult result = default(TResult);

            var resourceNames = this.sourceAssembly.GetManifestResourceNames();
            var prefix = this.sourceAssembly.GetName().Name;

            string resourceName;
            if (key.StartsWith(prefix))
            {
                resourceName = string.Format("{0}.json", key);
            }
            else
            {
                resourceName = string.Format("{0}.{1}.json", prefix, key);
            }

            var serializerConfigSvc = this.svcProvider.GetService(typeof(ISerializerConfigService)) as ISerializerConfigService;
            if (serializerConfigSvc == null)
            {
                throw new ServiceNotFoundException(typeof(ISerializerConfigService));
            }

            var metadataProvider = this.svcProvider.GetService(typeof(IMetadataProvider)) as IMetadataProvider;
            if (metadataProvider == null)
            {
                throw new ServiceNotFoundException(typeof(IMetadataProvider));
            }

            var jsonSettings = serializerConfigSvc.ConfigureJsonSerializerSettings(
                typeof(TResult),
                this.svcProvider);

            using (var stream = this.sourceAssembly.GetManifestResourceStream(resourceName))
            using (StreamReader sr = new StreamReader(stream))
            {
                result = (TResult)JsonConvert.DeserializeObject<TResult>(sr.ReadToEnd(), jsonSettings);
            }

            return result;
        }
    }
}
