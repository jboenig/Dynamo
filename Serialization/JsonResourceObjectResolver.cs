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
        /// Assembly.GetExecutingAssembly().
        /// </remarks>
        public JsonResourceObjectResolver(IServiceProvider svcProvider)
        {
            if (svcProvider == null)
            {
                throw new ArgumentNullException(nameof(svcProvider));
            }
            this.svcProvider = svcProvider;
            this.sourceAssembly = Assembly.GetExecutingAssembly();
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
