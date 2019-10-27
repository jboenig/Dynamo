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
using System.ComponentModel.Design;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Headway.Dynamo.Metadata;
using Headway.Dynamo.Serialization;
using Headway.Dynamo.Repository.FlatFileRepo;
using Headway.Dynamo.RestServices;
using Newtonsoft.Json.Linq;

namespace Headway.Dynamo.UnitTests
{
    [TestClass]
    public class RestApiTests
    {
        private ServiceContainer svcProvider;

        [TestInitialize]
        public void TestInitialize()
        {
            this.svcProvider = new ServiceContainer();

            // Use StandardMetadataProvider
            var metadataProvider = new StandardMetadataProvider();
            this.svcProvider.AddService(typeof(IMetadataProvider), metadataProvider);
            this.svcProvider.AddService(typeof(ISerializerConfigService), new StandardSerializerConfigService(null));

            var restApiRepo = new FlatFileRepo<RestApi>(
                metadataProvider.GetDataType<ObjectType>(typeof(RestApi)),
                @"MockData/RestApis.json",
                svcProvider
                );

            var restApiService = new RestApiServiceImpl();
            restApiService.Load(restApiRepo.GetQueryable());

            this.svcProvider.AddService(typeof(IRestApiService), restApiService);
        }

        [TestMethod]
        public void GetApiByName()
        {
            var restApiService = this.svcProvider.GetService(typeof(IRestApiService)) as IRestApiService;
            var test1Api = restApiService.GetApiByName("Test1");
            Assert.IsNotNull(test1Api);
        }

        [TestMethod]
        public void GetServiceByName()
        {
            var restApiService = this.svcProvider.GetService(typeof(IRestApiService)) as IRestApiService;
            var test1Api = restApiService.GetApiByName("Test1");
            Assert.IsNotNull(test1Api);
            var todoService = test1Api.GetServiceByName("todos");
            Assert.IsNotNull(todoService);
        }

        [TestMethod]
        public void InvokeGetSinglePathParam()
        {
            var restApiService = this.svcProvider.GetService(typeof(IRestApiService)) as IRestApiService;
            Assert.IsNotNull(restApiService);
            var taskTodos = restApiService.Invoke("Test1", "todos", new { Id = 1 });
            taskTodos.Wait();
            Assert.IsTrue(taskTodos.Result.IsSuccessStatusCode);

            var resObj = taskTodos.Result.Content.GetAsJObject();
            var idVal = resObj.Value<int>("id");
            Assert.AreEqual(idVal, 1);
        }

        [TestMethod]
        public void InvokePost()
        {
            var restApiService = this.svcProvider.GetService(typeof(IRestApiService)) as IRestApiService;
            Assert.IsNotNull(restApiService);
            var taskPosts = restApiService.Invoke("Test1", "posts", new { id = 1 });
            taskPosts.Wait();
            Assert.IsTrue(taskPosts.Result.IsSuccessStatusCode);
        }
    }
}
