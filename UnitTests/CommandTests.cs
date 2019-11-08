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
using Headway.Dynamo.Commands;
using Headway.Dynamo.Conditions;
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
        public void NullConditionalSynchronousCommand()
        {
            var person = new Person();
            var cmdConditional = new ConditionalCommand()
            {
                ExecuteWhen = null,
                Command = new SetPropertyValueCommand()
                {
                    PropertyName = "FirstName",
                    Value = "Dude"
                }
            };
            var cmdTask = cmdConditional.Execute(this.svcProvider, person);
            cmdTask.RunSynchronously();
            Assert.IsTrue(cmdTask.Result.IsSuccess);
            Assert.AreEqual(person.FirstName, "Dude");
        }

        [TestMethod]
        public void FalseConditionalSynchronousCommand()
        {
            var person = new Person();
            var cmdConditional = new ConditionalCommand()
            {
                ExecuteWhen = Condition.False,
                Command = new SetPropertyValueCommand()
                {
                    PropertyName = "FirstName",
                    Value = "Dude"
                }
            };
            var cmdTask = cmdConditional.Execute(this.svcProvider, person);
            cmdTask.RunSynchronously();
            Assert.IsTrue(cmdTask.Result.IsSuccess);
            Assert.AreEqual(person.FirstName, null);
        }

        [TestMethod]
        public void TrueConditionalSynchronousCommand()
        {
            var person = new Person();
            var cmdConditional = new ConditionalCommand()
            {
                ExecuteWhen = Condition.True,
                Command = new SetPropertyValueCommand()
                {
                    PropertyName = "FirstName",
                    Value = "Dude"
                }
            };
            var cmdTask = cmdConditional.Execute(this.svcProvider, person);
            cmdTask.RunSynchronously();
            Assert.IsTrue(cmdTask.Result.IsSuccess);
            Assert.AreEqual(person.FirstName, "Dude");
        }

        [TestMethod]
        public void SyncronousMacroCommand()
        {
            var cmdNoop = new DelegateCommand(() => true);
            var cmdAddNumbers = new DelegateCommand((s,c) =>
            {
                var v1 = PropertyResolver.GetPropertyValue<int>(c, "Val1");
                var v2 = PropertyResolver.GetPropertyValue<int>(c, "Val2");
                var sum = v1 + v2;
                return new BoolCommandResult(true, string.Format("Sum of {0} + {1} = {2}", v1, v2, sum));
            });
            var cmdMacro = new MacroCommand();
            cmdMacro.Commands.Add(cmdNoop);
            cmdMacro.Commands.Add(cmdAddNumbers);
            cmdMacro.Commands.Add(cmdNoop);
            cmdMacro.Commands.Add(cmdAddNumbers);
            cmdMacro.ExecuteAsync = false;
            var contextObj = new
            {
                Val1 = 2,
                Val2 = 5,
                Result = -1
            };
            var cmdMacroTask = cmdMacro.Execute(this.svcProvider, contextObj);
            cmdMacroTask.RunSynchronously();
            var cmdRes = cmdMacroTask.Result;
            Assert.IsTrue(cmdRes.IsSuccess);
            Assert.AreEqual(cmdRes.Description, "4 commands executed - 4 successful and 0 failed");
        }

        [TestMethod]
        public void AsyncronousMacroCommand()
        {
            var cmdNoop = new DelegateCommand(() =>
            {
                System.Threading.Thread.Sleep(20);
                return true;
            });
            var cmdAddNumbers = new DelegateCommand((s, c) =>
            {
                var v1 = PropertyResolver.GetPropertyValue<int>(c, "Val1");
                var v2 = PropertyResolver.GetPropertyValue<int>(c, "Val2");
                var sum = v1 + v2;
                return new BoolCommandResult(true, string.Format("Sum of {0} + {1} = {2}", v1, v2, sum));
            });
            var cmdMacro = new MacroCommand();
            cmdMacro.Commands.Add(cmdNoop);
            cmdMacro.Commands.Add(cmdAddNumbers);
            cmdMacro.Commands.Add(cmdNoop);
            cmdMacro.Commands.Add(cmdAddNumbers);
            cmdMacro.ExecuteAsync = true;
            var contextObj = new
            {
                Val1 = 2,
                Val2 = 5,
                Result = -1
            };
            var cmdMacroTask = cmdMacro.Execute(this.svcProvider, contextObj);
            cmdMacroTask.RunSynchronously();
            var cmdRes = cmdMacroTask.Result;
            Assert.IsTrue(cmdRes.IsSuccess);
            Assert.AreEqual(cmdRes.Description, "4 commands executed - 4 successful and 0 failed");
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
