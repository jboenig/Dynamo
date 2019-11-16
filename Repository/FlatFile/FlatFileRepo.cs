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
    public class FlatFileRepo<TObject> : IObjectRepository<TObject>
        where TObject : class
    {
        private string filePath;
        private IServiceProvider svcProvider;
        private JsonSerializerSettings jsonSettings;
        private List<TObject> objects;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="serializerConfigSvc"></param>
        /// <param name="svcProvider"></param>
        public FlatFileRepo(string filePath,
            ISerializerConfigService serializerConfigSvc,
            IServiceProvider svcProvider)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            if (serializerConfigSvc == null)
            {
                throw new ArgumentNullException(nameof(serializerConfigSvc));
            }

            this.filePath = filePath;
            this.svcProvider = svcProvider;

            this.jsonSettings = serializerConfigSvc.ConfigureJsonSerializerSettings(
                typeof(TObject),
                this.svcProvider);

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
                using (var stream = File.Open(this.filePath, FileMode.Open, FileAccess.Read))
                using (StreamReader sr = new StreamReader(stream))
                {
                    this.objects = (List<TObject>)JsonConvert.DeserializeObject<List<TObject>>(sr.ReadToEnd(), this.jsonSettings);
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

            var json = JsonConvert.SerializeObject(this.objects, this.jsonSettings);

            using (var stream = File.Open(this.filePath, FileMode.Create, FileAccess.Write))
            using (StreamWriter sw = new StreamWriter(stream))
            {
                sw.WriteLine(json);
            }
        }
    }
}
