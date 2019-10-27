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
using Newtonsoft.Json.Linq;
using Headway.Dynamo.Commands;
using Headway.Dynamo.Metadata;
using Headway.Dynamo.Serialization;
using Headway.Dynamo.RestServices;
using Headway.Dynamo.Repository.FlatFileRepo;
using Headway.Dynamo.Runtime;
using Headway.Dynamo.UnitTests.Mockdata;

namespace Headway.Dynamo.UnitTests
{
    [TestClass]
    public class CommandTests
    {
        private ServiceContainer svcProvider;

        [TestInitialize]
        public void TestInitialize()
        {
            this.svcProvider = new ServiceContainer();

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
        public void CallRestServiceGetNoParams()
        {
            var cmd = new CallRestWebServiceCommand()
            {
                ApiName = "Test1",
                ServiceName = "todos",
                ResponseContentPropertyName = "ResponseContent"
            };
            var context = new JsonPlaceholderContext { Id = 1 };
            var restCallTask = cmd.Execute(this.svcProvider, context);
            restCallTask.RunSynchronously();
            Assert.AreEqual(restCallTask.Result.IsSuccess, true);
            var idVal = context.ResponseContent.Value<int>("id");
            Assert.AreEqual(idVal, 1);
            var title = PropertyResolver.GetPropertyValue<string>(context.ResponseContent, "title");
            Assert.IsTrue(title.StartsWith("delectus"));
        }

        [TestMethod]
        public void CallRestServicePostTodo()
        {
            var cmd = new CallRestWebServiceCommand()
            {
                ApiName = "Test1",
                ServiceName = "posts",
                ResponseContentPropertyName = "ResponseContent",
                RequestContentPropertyName = "RequestContent"
            };

            var context = new JsonPlaceholderContext
            {
                RequestContent = new
                {
                    title = "hello world",
                    body = "dude",
                    userId = 10101
                }
            };

            var restCallTask = cmd.Execute(this.svcProvider, context);
            restCallTask.RunSynchronously();
            Assert.AreEqual(restCallTask.Result.IsSuccess, true);

            var userIdVal = context.ResponseContent.Value<int>("userId");
            Assert.AreEqual(userIdVal, 10101);

            var title = PropertyResolver.GetPropertyValue<string>(context.ResponseContent, "title");
            Assert.IsTrue(title.StartsWith("hello world"));
        }
    }
}
