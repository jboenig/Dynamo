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
    /// 
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    public sealed class JsonResourceObjectResolver<TResult> : IObjectResolver<string, TResult> where TResult : class
    {
        private Assembly sourceAssembly;
        private IServiceProvider svcProvider;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="svcProvider"></param>
        /// <param name="sourceAssembly"></param>
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
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
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

            var objType = metadataProvider.GetDataType<ObjectType>(typeof(TResult));
            if (objType == null)
            {
                var msg = string.Format("Unable to resolve data type {0}", typeof(TResult).FullName);
                throw new InvalidOperationException(msg);
            }

            var jsonSettings = serializerConfigSvc.ConfigureJsonSerializerSettings(
                objType,
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
