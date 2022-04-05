////////////////////////////////////////////////////////////////////////////////
// The MIT License(MIT)
// Copyright(c) 2020 Jeff Boenig
// This file is part of Headway.Dynamo.Restful
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
using System.ComponentModel.Design;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Headway.Dynamo.Metadata;
using Headway.Dynamo.Serialization;
using Headway.Dynamo.Repository.FlatFile;
using Headway.Dynamo.Runtime;
using Headway.Dynamo.Restful.Services;
using Headway.Dynamo.Restful.Models;
using Headway.Dynamo.Restful.Commands;
using Headway.Dynamo.Restful.UnitTests.Mockdata;

namespace Headway.Dynamo.Restful.UnitTests
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
                @"MockData/RestApis.json",
                svcProvider.GetService(typeof(ISerializerConfigService)) as ISerializerConfigService,
                svcProvider
                );

            var restApiService = new RestApiServiceImpl();
            restApiService.Load(restApiRepo.GetQueryable());

            this.svcProvider.AddService(typeof(IRestApiService), restApiService);
        }

        [TestMethod]
        public void CallRestServiceGetNoParams()
        {
            var cmd = new CallRestServiceCommand()
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
            var cmd = new CallRestServiceCommand()
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
