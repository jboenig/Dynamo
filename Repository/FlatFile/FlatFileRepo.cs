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
using System.Linq;
using System.IO;
using System.Collections.Generic;
using Headway.Dynamo.Metadata;
using Headway.Dynamo.Serialization;
using Headway.Dynamo.Exceptions;
using Newtonsoft.Json;

namespace Headway.Dynamo.Repository.FlatFileRepo
{
    /// <summary>
    /// Implements a simple flat file repository.
    /// </summary>
    /// <typeparam name="TObject">
    /// Type of object to store in the repository
    /// </typeparam>
    public sealed class FlatFileRepo<TObject> : IObjectRepository<TObject>
        where TObject : class
    {
        private ObjectType objType;
        private string filePath;
        private IServiceProvider svcProvider;
//        private IJsonConverterService converterService;
        private List<TObject> objects;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objType"></param>
        /// <param name="filePath"></param>
        /// <param name="svcProvider"></param>
        public FlatFileRepo(ObjectType objType,
            string filePath,
            IServiceProvider svcProvider)
        {
            if (objType == null)
            {
                throw new ArgumentNullException(nameof(objType));
            }

            if (string.IsNullOrEmpty(filePath))
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            if (!typeof(TObject).IsAssignableFrom(objType.CLRType))
            {
                var msg = string.Format("CLR type {0} not compatible with {1}", typeof(TObject).Name, objType.CLRType.Name);
                throw new ArgumentException(msg, nameof(objType));
            }

            this.objType = objType;
            this.filePath = filePath;
            this.svcProvider = svcProvider;
            this.LoadRepo();
        }

        /// <summary>
        /// Gets a queryable object from the data source for the specified class and connection.
        /// </summary>
        /// <returns></returns>
        public IQueryable<TObject> GetQueryable()
        {
            return this.objects.AsQueryable();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        public void Add(TObject obj)
        {
            this.objects.Add(obj);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        public void Update(TObject obj)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        public void Remove(TObject obj)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        public void SaveChanges()
        {
            this.SaveRepo();
        }

        private void LoadRepo()
        {
            if (File.Exists(this.filePath))
            {
                var serializerConfigSvc = this.svcProvider.GetService(typeof(ISerializerConfigService)) as ISerializerConfigService;
                if (serializerConfigSvc == null)
                {
                    throw new ServiceNotFoundException(typeof(ISerializerConfigService));
                }

                var jsonSettings = serializerConfigSvc.ConfigureJsonSerializerSettings(
                    this.objType,
                    this.svcProvider);

                using (var stream = File.Open(this.filePath, FileMode.Open, FileAccess.Read))
                using (StreamReader sr = new StreamReader(stream))
                {
                    this.objects = (List<TObject>)JsonConvert.DeserializeObject<List<TObject>>(sr.ReadToEnd(), jsonSettings);
                }
            }
            else
            {
                this.objects = new List<TObject>();
            }
        }

        private void SaveRepo()
        {
            var serializerConfigSvc = this.svcProvider.GetService(typeof(ISerializerConfigService)) as ISerializerConfigService;
            if (serializerConfigSvc == null)
            {
                throw new ServiceNotFoundException(typeof(ISerializerConfigService));
            }

            var jsonSettings = serializerConfigSvc.ConfigureJsonSerializerSettings(
                this.objType,
                this.svcProvider);

            var json = JsonConvert.SerializeObject(this.objects, jsonSettings);

            using (var stream = File.Open(this.filePath, FileMode.Create, FileAccess.Write))
            using (StreamWriter sw = new StreamWriter(stream))
            {
                sw.WriteLine(json);
            }
        }
    }
}
